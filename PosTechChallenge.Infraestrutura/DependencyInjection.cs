using Microsoft.Extensions.DependencyInjection;
using PosTechChallenge.Dominio.Interfaces;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Infraestrutura.Data;
using PosTechChallenge.Infraestrutura.Repositorios;


namespace PosTechChallenge.Infraestrutura
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or empty.", nameof(connectionString));
            }

            // Conexão única por requisição (DbSession) + Unit of Work, ambos Scoped.
            // A mesma sessão é compartilhada pelos repositórios e pelo UnitOfWork, de modo
            // que commit/rollback operem sobre a mesma conexão e transação.
            services.AddScoped<IDbSession>(_ => new DbSession(connectionString));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IFuncionarioRepositorio, FuncionariosRepositorio>();
            services.AddScoped<IVeiculosRepositorio, VeiculosRepositorio>();
            services.AddScoped<IItemsRepositorio, ItemsRepositorio>();
            services.AddScoped<IStatusRepositorio, StatusRepositorio>();
            services.AddScoped<IOrdemServicoRepositorio, OrdemServicoRepositorio>();
            services.AddScoped<IOrcamentoRepositorio, OrcamentoRepositorio>();
            services.AddScoped<IEmailOutboxRepositorio, EmailOutboxRepositorio>();
            services.AddScoped<IPecasRepositorio, PecasRepositorio>();
            services.AddScoped<ISegurancaRepositorio, SegurancaRepositorio>();
            services.AddScoped<IClienteRepositorio, ClienteRepositorio>();

            return services;
        }
    }
}
