using PosTechChallenge.Aplicacao.Dto.OrdemServico;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.UseCases.OrdemServico;
using PosTechChallenge.Dominio.Results;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Aplicacao.Services;

public sealed class OrdemServicoService : IOrdemServicoService
{
    private readonly CriarOrdemServicoUseCase _criarOrdemServicoUseCase;
    private readonly ObterOrdemServicoUseCase _obterOrdemServicoUseCase;
    private readonly ObterOrdemServicoPorClienteUseCase _obterOrdemServicoPorClienteUseCase;
    private readonly AtualizarOrdemServicoUseCase _atualizarOrdemServicoUseCase;
    private readonly AtualizarStatusOrdemServicoUseCase _atualizarStatusOrdemServicoUseCase;
    private readonly DeletarOrdemServicoUseCase _deletarOrdemServicoUseCase;
    private readonly ObterValorPorIdUseCase _obterValorPorIdUseCase;

    public OrdemServicoService(
        CriarOrdemServicoUseCase criarOrdemServicoUseCase,
        ObterOrdemServicoUseCase obterOrdemServicoUseCase,
        ObterOrdemServicoPorClienteUseCase obterOrdemServicoPorClienteUseCase,
        AtualizarOrdemServicoUseCase atualizarOrdemServicoUseCase,
        AtualizarStatusOrdemServicoUseCase atualizarStatusOrdemServicoUseCase,
        DeletarOrdemServicoUseCase deletarOrdemServicoUseCase,
        ObterValorPorIdUseCase obterValorPorIdUseCase)
    {
        _criarOrdemServicoUseCase = criarOrdemServicoUseCase;
        _obterOrdemServicoUseCase = obterOrdemServicoUseCase;
        _obterOrdemServicoPorClienteUseCase = obterOrdemServicoPorClienteUseCase;
        _atualizarOrdemServicoUseCase = atualizarOrdemServicoUseCase;
        _atualizarStatusOrdemServicoUseCase = atualizarStatusOrdemServicoUseCase;
        _deletarOrdemServicoUseCase = deletarOrdemServicoUseCase;
        _obterValorPorIdUseCase = obterValorPorIdUseCase;
    }

    public async Task<Resultado> CriarAsync(CriarOrdemServicoDto dto)
        => await _criarOrdemServicoUseCase.CriarAsync(dto);

    public async Task<Resultado<IEnumerable<ObterOrdemServicoDto>>> ObterTodosAsync(EStatusOrdemServico? status, int pageSize, int page)
        => await _obterOrdemServicoUseCase.ObterTodosAsync(status, page, pageSize);

    public async Task<Resultado<IEnumerable<ObterOrdemServicoDto>>> ObterPorClienteIdAsync(int idCliente, int pageSize = 10, int page = 1)
        => await _obterOrdemServicoPorClienteUseCase.ExecutarAsync(idCliente, pageSize, page);

    public async Task<Resultado<decimal>> ObterValorPorIdAsync(int id)
        => await _obterValorPorIdUseCase.ObterValorPorIdAsync(id);

    public async Task<Resultado<ObterOrdemServicoDto>> ObterPorIdAsync(int id)
        => await _obterOrdemServicoUseCase.ObterPorIdAsync(id);

    public async Task<Resultado<ObterStatusOrdemServicoDto>> ObterStatusAsync(int id)
        => await _obterOrdemServicoUseCase.ObterStatusAsync(id);

    public async Task<Resultado> AtualizarAsync(int id, AtualizarOrdemServicoDto dto)
        => await _atualizarOrdemServicoUseCase.AtualizarAsync(id, dto);

    public async Task<Resultado> AtualizarStatusAsync(int id, AtualizarStatusOrdemServicoDto dto)
        => await _atualizarStatusOrdemServicoUseCase.AtualizarAsync(id, dto);

    public async Task<Resultado> DeletarAsync(int id)
        => await _deletarOrdemServicoUseCase.DeletarAsync(id);
}
