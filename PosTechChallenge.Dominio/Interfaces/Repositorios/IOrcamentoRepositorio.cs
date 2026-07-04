using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios;

public interface IOrcamentoRepositorio
{
    Task<Orcamento?> ObterPorOrdemServicoIdAsync(int ordemServicoId, CancellationToken cancellationToken = default);
    Task<OrcamentoValores> CalcularValoresAsync(int ordemServicoId, CancellationToken cancellationToken = default);
    Task<int> CriarAsync(Orcamento orcamento, CancellationToken cancellationToken = default);
    Task<bool> AtualizarAsync(Orcamento orcamento, CancellationToken cancellationToken = default);
}
