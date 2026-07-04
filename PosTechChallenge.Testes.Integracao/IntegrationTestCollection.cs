using Xunit;

namespace PosTechChallenge.Testes.Integracao;

/// <summary>
/// Compartilha uma única <see cref="IntegrationTestFactory"/> (e portanto um único banco)
/// entre as classes de teste, executando-as em série para evitar corridas no banco.
/// </summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class IntegrationTestCollection : ICollectionFixture<IntegrationTestFactory>
{
    public const string Name = "Integração (SQL Server)";
}
