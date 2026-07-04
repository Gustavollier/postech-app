using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface ISegurancaRepositorio
    {
        Task<Seguranca?> ObterPorFuncionarioIdAsync(int funcionarioId, CancellationToken cancellationToken = default);
        Task SalvarSenhaAsync(int funcionarioId, string senhaHash, CancellationToken cancellationToken = default);
    }
}
