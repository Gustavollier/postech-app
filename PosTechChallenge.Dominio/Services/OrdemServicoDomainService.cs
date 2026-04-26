using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dominio.Services;

public sealed class OrdemServicoDomainService
{
    private readonly IClienteRepositorio _clienteRepositorio;
    private readonly IVeiculosRepositorio _veiculosRepositorio;
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;
    private readonly IItemsRepositorio _itemsRepositorio;
    private readonly IPecasRepositorio _pecasRepositorio;

    public OrdemServicoDomainService(
        IClienteRepositorio clienteRepositorio,
        IVeiculosRepositorio veiculosRepositorio,
        IFuncionarioRepositorio funcionarioRepositorio,
        IItemsRepositorio itemsRepositorio,
        IPecasRepositorio pecasRepositorio)
    {
        _clienteRepositorio = clienteRepositorio;
        _veiculosRepositorio = veiculosRepositorio;
        _funcionarioRepositorio = funcionarioRepositorio;
        _itemsRepositorio = itemsRepositorio;
        _pecasRepositorio = pecasRepositorio;
    }

    public async Task<Resultado> ValidarCriacaoAsync(OrdemServico ordemServico)
    {
        if (ordemServico.Status != EStatusOrdemServico.Recebida)
            return Resultado.Falha("Uma OS deve ser criada com status Recebida.");

        return await ValidarRelacionamentosAsync(ordemServico);
    }

    public async Task<Resultado> ValidarAtualizacaoAsync(OrdemServico ordemServico)
    {
        var validacaoStatus = ValidarStatusParaEdicao(ordemServico.Status);
        if (!validacaoStatus.IsValid)
            return validacaoStatus;

        return await ValidarRelacionamentosAsync(ordemServico);
    }

    public async Task<Resultado> ValidarTransicaoStatusAsync(OrdemServico ordemServico, EStatusOrdemServico novoStatus, int idFuncionario)
    {
        if (!Enum.IsDefined(typeof(EStatusOrdemServico), novoStatus))
            return Resultado.Falha("Status informado é inválido.");

        var funcionario = await _funcionarioRepositorio.ObterPorIdAsync(idFuncionario);
        if (funcionario == null)
            return Resultado.Falha($"Funcionário com ID {idFuncionario} não encontrado.");

        if (ordemServico.Status == novoStatus)
            return Resultado.Falha("A OS já está no status informado.");

        var statusEsperado = ObterProximoStatus(ordemServico.Status);
        if (statusEsperado == null || statusEsperado != novoStatus)
            return Resultado.Falha($"Transição inválida. A OS em {ordemServico.Status} só pode avançar para {statusEsperado}.");

        return novoStatus switch
        {
            EStatusOrdemServico.AguardandoAprovacao => await ValidarItensDaOsAsync(ordemServico.Id),
            EStatusOrdemServico.EmExecucao => await ValidarItensDaOsAsync(ordemServico.Id),
            EStatusOrdemServico.Finalizada => await ValidarFinalizacaoAsync(ordemServico.Id),
            _ => Resultado.Sucesso()
        };
    }

    private async Task<Resultado> ValidarRelacionamentosAsync(OrdemServico ordemServico)
    {
        var cliente = await _clienteRepositorio.ObterPorIdAsync(ordemServico.IdCliente);
        if (cliente == null)
            return Resultado.Falha($"Cliente com ID {ordemServico.IdCliente} não encontrado.");

        if (!cliente.Ativo)
            return Resultado.Falha("O cliente informado está inativo.");

        var veiculo = await _veiculosRepositorio.ObterPorIdAsync(ordemServico.IdVeiculo);
        if (veiculo == null)
            return Resultado.Falha($"Veículo com ID {ordemServico.IdVeiculo} não encontrado.");

        if (!veiculo.Ativo)
            return Resultado.Falha("O veículo informado está inativo.");

        if (veiculo.ClienteId != ordemServico.IdCliente)
            return Resultado.Falha("O veículo informado não pertence ao cliente da OS.");

        var funcionario = await _funcionarioRepositorio.ObterPorIdAsync(ordemServico.IdFuncionario);
        return funcionario != null
            ? Resultado.Sucesso()
            : Resultado.Falha($"Funcionário com ID {ordemServico.IdFuncionario} não encontrado.");
    }

    private static Resultado ValidarStatusParaEdicao(EStatusOrdemServico statusAtual)
    {
        return statusAtual switch
        {
            EStatusOrdemServico.EmExecucao => Resultado.Falha("Não é permitido alterar os dados da OS em execução."),
            EStatusOrdemServico.Finalizada => Resultado.Falha("Não é permitido alterar os dados da OS finalizada."),
            EStatusOrdemServico.Entregue => Resultado.Falha("Não é permitido alterar os dados da OS entregue."),
            _ => Resultado.Sucesso()
        };
    }

    private async Task<Resultado> ValidarItensDaOsAsync(int ordemServicoId)
    {
        var itens = (await _itemsRepositorio.ObterPorOrdemServicoIdAsync(ordemServicoId)).ToList();

        return itens.Count > 0
            ? Resultado.Sucesso()
            : Resultado.Falha("A OS precisa ter ao menos um item para avançar neste status.");
    }

    private async Task<Resultado> ValidarFinalizacaoAsync(int ordemServicoId)
    {
        var validacaoItens = await ValidarItensDaOsAsync(ordemServicoId);
        if (!validacaoItens.IsValid)
            return validacaoItens;

        var itens = await _itemsRepositorio.ObterPorOrdemServicoIdAsync(ordemServicoId);
        foreach (var item in itens.Where(i => i.IdPeca > 0))
        {
            var peca = await _pecasRepositorio.ObterPorIdAsync(item.IdPeca);
            if (peca == null)
                return Resultado.Falha($"Peça com ID {item.IdPeca} não encontrada.");

            if (!peca.Ativo)
                return Resultado.Falha($"A peça {peca.Id} está inativa.");

            if (peca.QuantidadeEstoque < item.QuantidadeItem)
                return Resultado.Falha($"Estoque insuficiente para a peça {peca.Id}. Disponível: {peca.QuantidadeEstoque}.");
        }

        return Resultado.Sucesso();
    }

    private static EStatusOrdemServico? ObterProximoStatus(EStatusOrdemServico statusAtual)
    {
        return statusAtual switch
        {
            EStatusOrdemServico.Recebida => EStatusOrdemServico.EmDiagnostico,
            EStatusOrdemServico.EmDiagnostico => EStatusOrdemServico.AguardandoAprovacao,
            EStatusOrdemServico.AguardandoAprovacao => EStatusOrdemServico.EmExecucao,
            EStatusOrdemServico.EmExecucao => EStatusOrdemServico.Finalizada,
            EStatusOrdemServico.Finalizada => EStatusOrdemServico.Entregue,
            _ => null
        };
    }
}
