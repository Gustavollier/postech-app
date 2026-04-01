using Microsoft.Extensions.DependencyInjection;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.Services;
using PosTechChallenge.Aplicacao.UseCases.Funcionario;

namespace PosTechChallenge.Aplicacao;

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
