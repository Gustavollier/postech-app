using PosTechChallenge.Aplicacao.Dto.OrdemServico;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dominio.Services;
using static PosTechChallenge.Dominio.Utils.Enums;
using OrdemServicoModel = PosTechChallenge.Dominio.Model.OrdemServico;

namespace PosTechChallenge.Aplicacao.UseCases.OrdemServico;

public sealed class CriarOrdemServicoUseCase
{
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;
    private readonly IStatusRepositorio _statusRepositorio;
    private readonly OrdemServicoDomainService _ordemServicoDomainService;

    public CriarOrdemServicoUseCase(
        IOrdemServicoRepositorio ordemServicoRepositorio,
        IStatusRepositorio statusRepositorio,
        OrdemServicoDomainService ordemServicoDomainService)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _statusRepositorio = statusRepositorio;
        _ordemServicoDomainService = ordemServicoDomainService;
    }

    public async Task<Resultado> CriarAsync(CriarOrdemServicoDto dto)
    {
        try
        {
            var agora = DateTime.UtcNow;
            var ordemServico = new OrdemServicoModel
            {
                IdCliente = dto.IdCliente,
                IdVeiculo = dto.IdVeiculo,
                IdFuncionario = dto.IdFuncionario,
                Status = EStatusOrdemServico.Recebida,
                CriadoEm = agora,
                AtualizadoEm = agora
            };

            var validacao = await _ordemServicoDomainService.ValidarCriacaoAsync(ordemServico);
            if (!validacao.IsValid)
                return validacao;

            var ordemServicoId = await _ordemServicoRepositorio.CriarAsync(ordemServico);

            await _statusRepositorio.CriarAsync(new Status
            {
                IdOS = ordemServicoId,
                IdFuncionario = dto.IdFuncionario,
                StatusAtual = EStatusOrdemServico.Recebida,
                UpdatedAt = agora
            });

            return Resultado.Sucesso("Ordem de serviço criada com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }
}
