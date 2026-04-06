
using Microsoft.AspNetCore.Http.HttpResults;
using System.Text.Json.Serialization;
using PosTechChallenge.Infraestrutura;
using PosTechChallenge.Applicacao;
using Dapper;
using PosTechChallenge.Infraestrutura.Mapeamentos;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddControllers();

// Adiciona e configura o Swagger (OpenAPI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "PosTechChallenge API",
        Version = "v1",
        Description = "API para gerenciamento de funcionários, clientes, ordens de serviço e peças.",
        Contact = new Microsoft.OpenApi.OpenApiContact
        {
            Name = "Equipe PosTechChallenge",
            Email = "contato@postech.com"
        }
    });
    // Adicione filtros ou configurações extras aqui se necessário
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDatabaseConfiguration(connectionString);

builder.Services.AddApplicationServices();

SqlMapper.AddTypeHandler(new PlacaDapper());

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "PosTechChallenge API v1");
        options.RoutePrefix = "swagger"; // abre em http://localhost:xxxx/swagger/index.html
    });
}

app.MapControllers();

app.Run();