using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Aplicacao.UseCases.OrdemServico;

public sealed class DeletarOrdemServicoUseCase
{
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;

    public DeletarOrdemServicoUseCase(IOrdemServicoRepositorio ordemServicoRepositorio)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio;
    }

    public async Task<Resultado> DeletarAsync(int id)
    {
        try
        {
            var ordemServico = await _ordemServicoRepositorio.ObterPorIdAsync(id);

            if (ordemServico == null)
                return Resultado.Falha($"Ordem de serviço com ID {id} não encontrada.");

            if (ordemServico.Status is EStatusOrdemServico.EmExecucao or EStatusOrdemServico.Finalizada or EStatusOrdemServico.Entregue)
                return Resultado.Falha("Não é permitido remover uma ordem de serviço em execução, finalizada ou entregue.");

            var removido = await _ordemServicoRepositorio.DeletarAsync(id);

            return removido
                ? Resultado.Sucesso("Ordem de serviço removida com sucesso.")
                : Resultado.Falha("Não foi possível remover a ordem de serviço.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }
}
