using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dominio.Services;

public sealed class ItemOSDomainService
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;
    private readonly IPecasRepositorio _pecasRepositorio;

    public ItemOSDomainService(
        IFuncionarioRepositorio funcionarioRepositorio,
        IPecasRepositorio pecasRepositorio)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
        _pecasRepositorio = pecasRepositorio;
    }

    public async Task<Resultado> ValidarInclusaoAsync(OrdemServico ordemServico, ItemOS item)
    {
        var validacaoStatus = ValidarStatusParaAlteracao(ordemServico);
        if (validacaoStatus.IsValid is false)
            return validacaoStatus;

        var validacaoTipo = ValidarTipoItem(item);
        if (validacaoTipo.IsValid is false)
            return validacaoTipo;

        var validacaoQuantidade = ValidarQuantidade(item);

        if (validacaoQuantidade.IsValid is false)
            return validacaoQuantidade;

        return await ValidarRelacionamentosPorTipoAsync(item);
    }

    public async Task<Resultado> ValidarAtualizacaoAsync(OrdemServico ordemServico, ItemOS item)
    {
        var validacaoStatus = ValidarStatusParaAlteracao(ordemServico);
        if (validacaoStatus.IsValid is false)
            return validacaoStatus;

        var validacaoTipo = ValidarTipoItem(item);
        if (validacaoTipo.IsValid is false)
            return validacaoTipo;

        var validacaoQuantidade = ValidarQuantidade(item);
        if (validacaoQuantidade.IsValid is false)
            return validacaoQuantidade;

        return await ValidarRelacionamentosPorTipoAsync(item);
    }

    public Resultado ValidarRemocao(OrdemServico ordemServico)
        => ValidarStatusParaAlteracao(ordemServico);

    private static Resultado ValidarStatusParaAlteracao(OrdemServico ordemServico)
    {
        return ordemServico.Status switch
        {
            EStatusOrdemServico.EmExecucao => Resultado.Falha("Não é permitido alterar itens de uma OS em execução."),
            EStatusOrdemServico.Finalizada => Resultado.Falha("Não é permitido alterar itens de uma OS finalizada."),
            EStatusOrdemServico.Entregue => Resultado.Falha("Não é permitido alterar itens de uma OS entregue."),
            _ => Resultado.Sucesso()
        };
    }

    private static Resultado ValidarTipoItem(ItemOS item)
    {
        return Enum.IsDefined(typeof(ETipoItemOrdemServico), item.TipoItem)
            ? Resultado.Sucesso()
            : Resultado.Falha("Tipo de item inválido.");
    }

    private static Resultado ValidarQuantidade(ItemOS item)
    {
        return item.QuantidadeItem > 0
            ? Resultado.Sucesso()
            : Resultado.Falha("Quantidade do item deve ser maior que zero.");
    }

    private async Task<Resultado> ValidarRelacionamentosPorTipoAsync(ItemOS item)
    {
        return item.TipoItem == (int)ETipoItemOrdemServico.MaoDeObra
            ? await ValidarItemMaoDeObraAsync(item)
            : await ValidarItemPecaAsync(item);
    }

    private async Task<Resultado> ValidarItemMaoDeObraAsync(ItemOS item)
    {
        var validacaoFuncionario = ValidarFuncionarioObrigatorio(item);
        if (validacaoFuncionario.IsValid is false)
            return validacaoFuncionario;

        var validacaoSemPeca = ValidarAusenciaDePeca(item);
        if (validacaoSemPeca.IsValid is false)
            return validacaoSemPeca;

        return await ValidarFuncionarioExistenteAsync(item);
    }

    private async Task<Resultado> ValidarItemPecaAsync(ItemOS item)
    {
        var validacaoPeca = ValidarPecaObrigatoria(item);
        if (validacaoPeca.IsValid is false)
            return validacaoPeca;

        var validacaoSemFuncionario = ValidarAusenciaDeFuncionario(item);
        if (validacaoSemFuncionario.IsValid is false)
            return validacaoSemFuncionario;

        return await ValidarPecaDisponivelAsync(item);
    }

    private static Resultado ValidarFuncionarioObrigatorio(ItemOS item)
    {
        return item.IdFuncionario > 0
            ? Resultado.Sucesso()
            : Resultado.Falha("Itens de mão de obra exigem um funcionário responsável.");
    }

    private static Resultado ValidarAusenciaDePeca(ItemOS item)
    {
        return item.IdPeca <= 0
            ? Resultado.Sucesso()
            : Resultado.Falha("Itens de mão de obra não devem informar peça.");
    }

    private async Task<Resultado> ValidarFuncionarioExistenteAsync(ItemOS item)
    {
        var funcionario = await _funcionarioRepositorio.ObterPorIdAsync(item.IdFuncionario);
        return funcionario != null
            ? Resultado.Sucesso()
            : Resultado.Falha($"Funcionário com ID {item.IdFuncionario} não encontrado.");
    }

    private static Resultado ValidarPecaObrigatoria(ItemOS item)
    {
        return item.IdPeca > 0
            ? Resultado.Sucesso()
            : Resultado.Falha("Itens de peça/insumo exigem a peça informada.");
    }

    private static Resultado ValidarAusenciaDeFuncionario(ItemOS item)
    {
        return item.IdFuncionario <= 0
            ? Resultado.Sucesso()
            : Resultado.Falha("Itens de peça/insumo não devem informar funcionário.");
    }

    private async Task<Resultado> ValidarPecaDisponivelAsync(ItemOS item)
    {
        var peca = await _pecasRepositorio.ObterPorIdAsync(item.IdPeca);
        if (peca == null)
            return Resultado.Falha($"Peça com ID {item.IdPeca} não encontrada.");

        if (!peca.Ativo)
            return Resultado.Falha("A peça informada está inativa.");

        return peca.QuantidadeEstoque >= item.QuantidadeItem
            ? Resultado.Sucesso()
            : Resultado.Falha($"Estoque insuficiente para a peça {item.IdPeca}. Disponível: {peca.QuantidadeEstoque}.");
    }
}
