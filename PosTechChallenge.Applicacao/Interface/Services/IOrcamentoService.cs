using PosTechChallenge.Aplicacao.Dto.Orcamento;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface IOrcamentoService
{
    Task<Resultado<ObterOrcamentoDto>> ObterPorOrdemServicoIdAsync(int ordemServicoId);
    Task<Resultado<ObterOrcamentoDto>> CalcularAsync(int ordemServicoId);
    Task<Resultado<ObterOrcamentoDto>> EnviarAsync(int ordemServicoId);
    Task<Resultado> ResponderAsync(int ordemServicoId, ResponderOrcamentoDto dto);
}
