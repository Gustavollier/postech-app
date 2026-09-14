using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PosTechChallenge.Aplicacao;
using PosTechChallenge.HealthChecks;
using PosTechChallenge.Infraestrutura;
using PosTechChallenge.Infraestrutura.Mapeamentos;
using PosTechChallenge.Middleware;
using PosTechChallenge.Monitoring;
using System.Text;

using PosTechChallenge.Autorizacao;

var builder = WebApplication.CreateBuilder(args);

// Logs estruturados em JSON fora de Development: é o formato que o agente do
// Datadog parseia sem regra custom, e o que permite filtrar por correlationId.
// Em Development mantemos o console legível.
if (builder.Environment.IsDevelopment() is false)
{
    builder.Logging.ClearProviders();
    builder.Logging.AddJsonConsole(options =>
    {
        options.IncludeScopes = true;
        options.UseUtcTimestamp = true;
    });
}

builder.Services.AddControllers();

var openApiServerUrl = builder.Configuration["OpenApi:ServerUrl"];
var useHttpsRedirection = builder.Configuration.GetValue("HttpsRedirection:Enabled", true);

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .GetChildren()
    .Select(origin => origin.Value)
    .Where(origin => string.IsNullOrWhiteSpace(origin) is false)
    .Cast<string>()
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        // 1. Adiciona a definição do esquema de segurança
        if (string.IsNullOrWhiteSpace(openApiServerUrl) is false)
        {
            document.Servers = new List<OpenApiServer>
            {
                new()
                {
                    Url = openApiServerUrl
                }
            };
        }

        document.Components ??= new();
        document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Insira apenas o token JWT (sem a palavra Bearer)"
        });

        // 2. Torna a segurança global para todos os endpoints
        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        return Task.CompletedTask;
    });
});

// --- RESTO DA SUA CONFIGURAÇÃO ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDatabaseConfiguration(connectionString);
builder.Services.AddApplicationServices();
builder.Services.AddSingleton<IExecutionTimeMonitor, ExecutionTimeMonitor>();
builder.Services.AddSingleton(TimeProvider.System);

// Liveness responde sem tocar em dependência alguma; readiness exige o banco.
// A tag "ready" é o que separa os dois endpoints mapeados mais abaixo.
builder.Services.AddHealthChecks()
    .AddCheck<BancoDeDadosHealthCheck>("banco", tags: ["ready"]);

var jwtSecret = builder.Configuration["Jwt:SecretKey"] ?? throw new InvalidOperationException("Jwt:SecretKey missing");
if (Encoding.UTF8.GetByteCount(jwtSecret) < 32)
{
    throw new InvalidOperationException("Jwt:SecretKey must have at least 32 bytes.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // Estar autenticado nao basta para as rotas da operacao. O token de cliente,
    // emitido pela funcao de autenticacao por CPF, carrega role "Cliente" e nao
    // satisfaz esta policy — ele so alcanca o proprio cadastro e as proprias
    // ordens, com a checagem de posse feita no controller.
    options.AddPolicy(Perfis.Equipe, politica => politica.RequireRole(Perfis.Cargos));
});
SqlMapper.AddTypeHandler(new PlacaDapper());

var app = builder.Build();

// --- MIDDLEWARES ---
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Em Development o Swagger sobe sempre. Fora dele, só quando explicitamente
// habilitado por configuração — o ambiente publicado da Fase 3 liga a chave para
// a documentação ficar navegável, mas o default continua desligado, para que
// nenhum ambiente exponha o contrato da API por descuido.
//
// O documento gerado aponta para o gateway do APIM (OpenApi__ServerUrl) e já
// declara o esquema Bearer, entao o botao Authorize do Swagger UI aceita o token
// emitido pela Auth Function.
var swaggerHabilitado = app.Environment.IsDevelopment()
    || app.Configuration.GetValue<bool>("Swagger:Enabled");

if (swaggerHabilitado)
{
    // Gera o endpoint do documento: /openapi/v1.json
    app.MapOpenApi().AllowAnonymous();

    // Habilita a interface visual do Swagger UI apontando para o arquivo nativo
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Minha API Nativa v1");
        options.RoutePrefix = "swagger";
    });
}

if (useHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseCors();
app.Use(async (context, next) =>
{
    context.Response.Headers.XContentTypeOptions = "nosniff";
    context.Response.Headers.XFrameOptions = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
    await next();
});
// Primeiro da cadeia de middlewares nossos: tudo que logar depois disso já sai
// com o correlationId no escopo, inclusive as falhas de autenticação.
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestExecutionTimingMiddleware>();
app.UseMiddleware<LoginRateLimitingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Liveness: o processo está de pé. Não consulta o banco de propósito — se o
// banco cair, o Kubernetes não deve matar e recriar o pod, só tirá-lo do
// balanceador (o que o readiness abaixo faz).
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => false
}).AllowAnonymous();

// Readiness: só entra no balanceador quem consegue falar com o banco.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = EscreverRespostaHealthCheck
}).AllowAnonymous();

app.Run();

// Resposta em JSON para o Datadog conseguir extrair o estado de cada dependência
// em vez de receber apenas a string "Healthy".
static Task EscreverRespostaHealthCheck(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json; charset=utf-8";

    var payload = new
    {
        status = report.Status.ToString(),
        duracaoMs = report.TotalDuration.TotalMilliseconds,
        checks = report.Entries.Select(entrada => new
        {
            nome = entrada.Key,
            status = entrada.Value.Status.ToString(),
            descricao = entrada.Value.Description,
            duracaoMs = entrada.Value.Duration.TotalMilliseconds
        })
    };

    return context.Response.WriteAsJsonAsync(payload);
}

public partial class Program
{
}
