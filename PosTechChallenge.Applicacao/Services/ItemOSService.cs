using PosTechChallenge.Aplicacao.Dto.ItemOS;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.UseCases.ItemOS;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Services;

public sealed class ItemOSService : IItemOSService
{
    private readonly CriarItemOSUseCase _criarItemOSUseCase;
    private readonly ObterItemOSUseCase _obterItemOSUseCase;
    private readonly AtualizarItemOSUseCase _atualizarItemOSUseCase;
    private readonly DeletarItemOSUseCase _deletarItemOSUseCase;

    public ItemOSService(
        CriarItemOSUseCase criarItemOSUseCase,
        ObterItemOSUseCase obterItemOSUseCase,
        AtualizarItemOSUseCase atualizarItemOSUseCase,
        DeletarItemOSUseCase deletarItemOSUseCase)
    {
        _criarItemOSUseCase = criarItemOSUseCase;
        _obterItemOSUseCase = obterItemOSUseCase;
        _atualizarItemOSUseCase = atualizarItemOSUseCase;
        _deletarItemOSUseCase = deletarItemOSUseCase;
    }

    public async Task<Resultado> CriarAsync(CriarItemOSDto dto)
        => await _criarItemOSUseCase.CriarAsync(dto);

    public async Task<Resultado<IEnumerable<ObterItemOSDto>>> ObterPorOrdemServicoIdAsync(int ordemServicoId)
        => await _obterItemOSUseCase.ObterPorOrdemServicoIdAsync(ordemServicoId);

    public async Task<Resultado<ObterItemOSDto>> ObterPorIdAsync(int ordemServicoId, int id)
        => await _obterItemOSUseCase.ObterPorIdAsync(ordemServicoId, id);

    public async Task<Resultado> AtualizarAsync(int ordemServicoId, int id, AtualizarItemOSDto dto)
        => await _atualizarItemOSUseCase.AtualizarAsync(ordemServicoId, id, dto);

    public async Task<Resultado> DeletarAsync(int ordemServicoId, int id)
        => await _deletarItemOSUseCase.DeletarAsync(ordemServicoId, id);
}
