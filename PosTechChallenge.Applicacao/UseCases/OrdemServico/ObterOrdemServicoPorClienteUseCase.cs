using PosTechChallenge.Aplicacao.Dto.OrdemServico;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;
using OrdemServicoModel = PosTechChallenge.Dominio.Model.OrdemServico;

namespace PosTechChallenge.Aplicacao.UseCases.OrdemServico;

public sealed class ObterOrdemServicoPorClienteUseCase
{
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;

    public ObterOrdemServicoPorClienteUseCase(IOrdemServicoRepositorio ordemServicoRepositorio)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio ?? throw new ArgumentNullException(nameof(ordemServicoRepositorio));
    }

    public async Task<Resultado<IEnumerable<ObterOrdemServicoDto>>> ExecutarAsync(int idCliente, int pageSize, int page)
    {
        try
        {
            if (idCliente <= 0)
                return Resultado<IEnumerable<ObterOrdemServicoDto>>.Falha("ID do cliente inválido. Deve ser maior que zero.");

            var ordensServico = await _ordemServicoRepositorio.ObterPorClienteIdAsync(idCliente, pageSize, page);

            if (!ordensServico.Any())
                return Resultado<IEnumerable<ObterOrdemServicoDto>>.Falha("Nenhuma ordem de serviço encontrada para este cliente.");

            var lista = ordensServico.Select(MapearParaDto).ToList();

            return Resultado<IEnumerable<ObterOrdemServicoDto>>.Sucesso(lista);
        }
        catch (Exception ex)
        {
            return Resultado<IEnumerable<ObterOrdemServicoDto>>.Falha(ex.Message);
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
