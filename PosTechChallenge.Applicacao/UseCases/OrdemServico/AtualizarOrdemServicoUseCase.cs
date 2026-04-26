using PosTechChallenge.Aplicacao.Dto.OrdemServico;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dominio.Services;

namespace PosTechChallenge.Aplicacao.UseCases.OrdemServico;

public sealed class AtualizarOrdemServicoUseCase
{
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;
    private readonly OrdemServicoDomainService _ordemServicoDomainService;

    public AtualizarOrdemServicoUseCase(
        IOrdemServicoRepositorio ordemServicoRepositorio,
        OrdemServicoDomainService ordemServicoDomainService)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _ordemServicoDomainService = ordemServicoDomainService;
    }

    public async Task<Resultado> AtualizarAsync(int id, AtualizarOrdemServicoDto dto)
    {
        try
        {
            var ordemServico = await _ordemServicoRepositorio.ObterPorIdAsync(id);

            if (ordemServico == null)
                return Resultado.Falha($"Ordem de serviço com ID {id} não encontrada.");

            ordemServico.IdCliente = dto.IdCliente;
            ordemServico.IdVeiculo = dto.IdVeiculo;
            ordemServico.IdFuncionario = dto.IdFuncionario;
            ordemServico.AtualizadoEm = DateTime.UtcNow;

            var validacao = await _ordemServicoDomainService.ValidarAtualizacaoAsync(ordemServico);
            if (!validacao.IsValid)
                return validacao;

            var atualizado = await _ordemServicoRepositorio.AtualizarAsync(ordemServico);

            return atualizado
                ? Resultado.Sucesso("Ordem de serviço atualizada com sucesso.")
                : Resultado.Falha("Não foi possível atualizar a ordem de serviço.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }
}
