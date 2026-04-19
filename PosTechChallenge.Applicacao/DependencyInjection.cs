using Microsoft.Extensions.DependencyInjection;
using PosTechChallenge.Applicacao.Interface.Services;
using PosTechChallenge.Applicacao.Services;
using PosTechChallenge.Applicacao.UseCases.Funcionario;
using PosTechChallenge.Applicacao.UseCases.Autenticacao;

namespace PosTechChallenge.Applicacao;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CriarFuncionarioUseCase>();
        services.AddScoped<ObterFuncionarioUseCase>();
        services.AddScoped<AtualizarFuncionarioUseCase>();
        services.AddScoped<DeletarFuncionarioUseCase>();
        services.AddScoped<IFuncionarioService, FuncionarioService>();
        
        // Token Service
        services.AddScoped<ITokenService, TokenService>();
        
        // Autenticação UseCases
        services.AddScoped<LoginUseCase>();
        
        // Autenticação Services
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();

        return services;
    }
}
