using PosTechChallenge.Aplicacao.Dto.OrdemServico;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;
using static PosTechChallenge.Dominio.Utils.Enums;
using OrdemServicoModel = PosTechChallenge.Dominio.Model.OrdemServico;

namespace PosTechChallenge.Aplicacao.UseCases.OrdemServico;

public sealed class ObterOrdemServicoUseCase
{
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;
    private readonly IStatusRepositorio _statusRepositorio;

    public ObterOrdemServicoUseCase(
        IOrdemServicoRepositorio ordemServicoRepositorio,
        IStatusRepositorio statusRepositorio)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _statusRepositorio = statusRepositorio;
    }

    public async Task<Resultado<IEnumerable<ObterOrdemServicoDto>>> ObterTodosAsync(EStatusOrdemServico? status, int page, int pageSize)
    {
        try
        {
            var ordensServico = await _ordemServicoRepositorio.ObterTodosAsync(pageSize, page);

            if (status.HasValue)
                ordensServico = ordensServico.Where(os => os.Status == status.Value);

            var lista = ordensServico.Select(MapearParaDto).ToList();

            if (!lista.Any())
                return Resultado<IEnumerable<ObterOrdemServicoDto>>.Falha("Nenhuma ordem de serviço encontrada.");

            return Resultado<IEnumerable<ObterOrdemServicoDto>>.Sucesso(lista);
        }
        catch (Exception ex)
        {
            return Resultado<IEnumerable<ObterOrdemServicoDto>>.Falha(ex.Message);
        }
    }

    public async Task<Resultado<ObterOrdemServicoDto>> ObterPorIdAsync(int id)
    {
        try
        {
            var ordemServico = await _ordemServicoRepositorio.ObterPorIdAsync(id);

            return ordemServico != null
                ? Resultado<ObterOrdemServicoDto>.Sucesso(MapearParaDto(ordemServico))
                : Resultado<ObterOrdemServicoDto>.Falha($"Ordem de serviço com ID {id} não encontrada.");
        }
        catch (Exception ex)
        {
            return Resultado<ObterOrdemServicoDto>.Falha(ex.Message);
        }
    }

    public async Task<Resultado<ObterStatusOrdemServicoDto>> ObterStatusAsync(int id)
    {
        try
        {
            var ordemServico = await _ordemServicoRepositorio.ObterPorIdAsync(id);
            if (ordemServico == null)
                return Resultado<ObterStatusOrdemServicoDto>.Falha($"Ordem de serviço com ID {id} não encontrada.");

            var historico = await _statusRepositorio.ObterPorOrdemServicoIdAsync(id);
            var historicoDto = historico.Select(status => new ObterStatusHistoricoOrdemServicoDto
            {
                Id = status.Id,
                IdOS = status.IdOS,
                IdFuncionario = status.IdFuncionario,
                StatusAtual = (int)status.StatusAtual,
                UpdatedAt = status.UpdatedAt
            }).ToList();

            var dto = new ObterStatusOrdemServicoDto
            {
                OrdemServicoId = ordemServico.Id,
                StatusAtual = (int)ordemServico.Status,
                AtualizadoEm = ordemServico.AtualizadoEm,
                Historico = historicoDto
            };

            return Resultado<ObterStatusOrdemServicoDto>.Sucesso(dto);
        }
        catch (Exception ex)
        {
            return Resultado<ObterStatusOrdemServicoDto>.Falha(ex.Message);
        }
    }

    private static ObterOrdemServicoDto MapearParaDto(OrdemServicoModel ordemServico)
    {
        return new ObterOrdemServicoDto
        {
            Id = ordemServico.Id,
            IdCliente = ordemServico.IdCliente,
            IdVeiculo = ordemServico.IdVeiculo,
            IdFuncionario = ordemServico.IdFuncionario,
            Status = (int)ordemServico.Status,
            CriadoEm = ordemServico.CriadoEm,
            AtualizadoEm = ordemServico.AtualizadoEm
        };
    }
}
