using PosTechChallenge.Aplicacao.Dto.ItemOS;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dominio.Services;

namespace PosTechChallenge.Aplicacao.UseCases.ItemOS;

public sealed class CriarItemOSUseCase
{
    private readonly IItemsRepositorio _itemsRepositorio;
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;
    private readonly ItemOSDomainService _itemOSDomainService;

    public CriarItemOSUseCase(
        IItemsRepositorio itemsRepositorio,
        IOrdemServicoRepositorio ordemServicoRepositorio,
        ItemOSDomainService itemOSDomainService)
    {
        _itemsRepositorio = itemsRepositorio;
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _itemOSDomainService = itemOSDomainService;
    }

    public async Task<Resultado> CriarAsync(CriarItemOSDto dto)
    {
        try
        {
            var ordemServico = await _ordemServicoRepositorio.ObterPorIdAsync(dto.IdOS);

            if (ordemServico == null)
                return Resultado.Falha($"OS com ID {dto.IdOS} não encontrada.");

            var item = new Dominio.Model.ItemOS
            {
                IdOS = dto.IdOS,
                TipoItem = dto.TipoItem,
                QuantidadeItem = dto.QuantidadeItem,
                IdFuncionario = dto.IdFuncionario ?? 0,
                IdPeca = dto.IdPeca ?? 0
            };

            var validacao = await _itemOSDomainService.ValidarInclusaoAsync(ordemServico, item);
            if (!validacao.IsValid)
                return validacao;

            await _itemsRepositorio.CriarAsync(item);
            await AtualizarDataOSAsync(ordemServico);

            return Resultado.Sucesso("Item da OS criado com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha(ex.Message);
        }
    }

    private async Task AtualizarDataOSAsync(Dominio.Model.OrdemServico ordemServico)
    {
        ordemServico.AtualizadoEm = DateTime.UtcNow;
        await _ordemServicoRepositorio.AtualizarAsync(ordemServico);
    }
}
