using Microsoft.Extensions.DependencyInjection;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.Services;
using PosTechChallenge.Aplicacao.UseCases.Funcionario;
using PosTechChallenge.Aplicacao.UseCases.Autenticacao;
using PosTechChallenge.Aplicacao.UseCases.ItemOS;
using PosTechChallenge.Aplicacao.UseCases.Orcamento;
using PosTechChallenge.Aplicacao.UseCases.OrdemServico;
using PosTechChallenge.Dominio.Services;

namespace PosTechChallenge.Aplicacao;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<CriarFuncionarioUseCase>();
        services.AddScoped<ObterFuncionarioUseCase>();
        services.AddScoped<AtualizarFuncionarioUseCase>();
        services.AddScoped<DeletarFuncionarioUseCase>();
        services.AddScoped<CriarItemOSUseCase>();
        services.AddScoped<ObterItemOSUseCase>();
        services.AddScoped<AtualizarItemOSUseCase>();
        services.AddScoped<DeletarItemOSUseCase>();
        services.AddScoped<CriarOrdemServicoUseCase>();
        services.AddScoped<ObterOrdemServicoUseCase>();
        services.AddScoped<ObterOrdemServicoPorClienteUseCase>();
        services.AddScoped<AtualizarOrdemServicoUseCase>();
        services.AddScoped<AtualizarStatusOrdemServicoUseCase>();
        services.AddScoped<DeletarOrdemServicoUseCase>();
        services.AddScoped<ObterValorPorIdUseCase>();
        services.AddScoped<ObterOrcamentoUseCase>();
        services.AddScoped<CalcularOrcamentoUseCase>();
        services.AddScoped<EnviarOrcamentoUseCase>();
        services.AddScoped<ResponderOrcamentoUseCase>();
        services.AddScoped<ItemOSDomainService>();
        services.AddScoped<OrdemServicoDomainService>();
        services.AddScoped<IFuncionarioService, FuncionarioService>();
        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IVeiculoService, VeiculoService>();
        services.AddScoped<IPecaService, PecaService>();
        services.AddScoped<IItemOSService, ItemOSService>();
        services.AddScoped<IOrdemServicoService, OrdemServicoService>();
        services.AddScoped<IOrcamentoService, OrcamentoService>();
        
        // Token Service
        services.AddScoped<ITokenService, TokenService>();
        
        // Autenticação UseCases
        services.AddScoped<LoginUseCase>();
        services.AddScoped<AlterarSenhaUseCase>();

        // Autenticação Services
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();

        return services;
    }
}
