using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios;

public interface IOrcamentoRepositorio
{
    Task<Orcamento?> ObterPorOrdemServicoIdAsync(int ordemServicoId);
    Task<OrcamentoValores> CalcularValoresAsync(int ordemServicoId);
    Task<int> CriarAsync(Orcamento orcamento);
    Task<bool> AtualizarAsync(Orcamento orcamento);
}
