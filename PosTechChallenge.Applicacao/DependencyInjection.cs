using Microsoft.Extensions.DependencyInjection;
using PosTechChallenge.Applicacao.Interface.Services;
using PosTechChallenge.Applicacao.Services;
using PosTechChallenge.Applicacao.UseCases.Funcionario;

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

        return services;
    }
}
