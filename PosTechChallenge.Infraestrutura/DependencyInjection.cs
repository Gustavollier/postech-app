using Microsoft.Extensions.DependencyInjection;
using PosTechChallenge.Infraestrutura.Repositories;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
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

            services.AddScoped<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString));
            services.AddScoped<IFuncionarioRepositorio, FuncionariosRepositorio>();
            services.AddScoped<IVeiculosRepositorio, VeiculosRepositorio>();
            services.AddScoped<IItemsRepositorio, ItemsRepositorio>();
            services.AddScoped<IStatusRepositorio, StatusRepositorio>();
            services.AddScoped<IOrdemServicoRepositorio, OrdemServicoRepositorio>();
            services.AddScoped<ISegurancaFuncionarioRepositorio, SegurancaRepositorio>();

            return services;
        }
    }
}
