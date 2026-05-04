using PosTechChallenge.Aplicacao.Dto.Orcamento;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.UseCases.Orcamento;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Services;

public sealed class OrcamentoService : IOrcamentoService
{
    private readonly ObterOrcamentoUseCase _obterOrcamentoUseCase;
    private readonly CalcularOrcamentoUseCase _calcularOrcamentoUseCase;
    private readonly EnviarOrcamentoUseCase _enviarOrcamentoUseCase;
    private readonly ResponderOrcamentoUseCase _responderOrcamentoUseCase;

    public OrcamentoService(
        ObterOrcamentoUseCase obterOrcamentoUseCase,
        CalcularOrcamentoUseCase calcularOrcamentoUseCase,
        EnviarOrcamentoUseCase enviarOrcamentoUseCase,
        ResponderOrcamentoUseCase responderOrcamentoUseCase)
    {
        _obterOrcamentoUseCase = obterOrcamentoUseCase;
        _calcularOrcamentoUseCase = calcularOrcamentoUseCase;
        _enviarOrcamentoUseCase = enviarOrcamentoUseCase;
        _responderOrcamentoUseCase = responderOrcamentoUseCase;
    }

    public async Task<Resultado<ObterOrcamentoDto>> ObterPorOrdemServicoIdAsync(int ordemServicoId)
        => await _obterOrcamentoUseCase.ObterPorOrdemServicoIdAsync(ordemServicoId);

    public async Task<Resultado<ObterOrcamentoDto>> CalcularAsync(int ordemServicoId)
        => await _calcularOrcamentoUseCase.CalcularAsync(ordemServicoId);

    public async Task<Resultado<ObterOrcamentoDto>> EnviarAsync(int ordemServicoId)
        => await _enviarOrcamentoUseCase.EnviarAsync(ordemServicoId);

    public async Task<Resultado> ResponderAsync(int ordemServicoId, ResponderOrcamentoDto dto)
        => await _responderOrcamentoUseCase.ResponderAsync(ordemServicoId, dto);
}
