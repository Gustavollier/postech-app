using PosTech.Contratos;
using PosTech.Mensageria;

namespace PosTech.Execucao;

/// <summary>
/// As transições da ordem dentro da oficina.
///
/// Função pura, sem banco e sem broker, como o orquestrador e o montador de
/// orçamento. Recebe o documento e devolve ele mudado, com os eventos já
/// enfileirados no próprio agregado.
/// </summary>
public static class GestorDaExecucao
{
    public const string Origem = "execucao-service";

    /// <summary>
    /// Chegou uma ordem. Entra na fila esperando diagnóstico, sem publicar nada:
    /// receber não é fato novo para ninguém de fora.
    /// </summary>
    public static OrdemEmExecucao Receber(OrdemAberta evento, DateTime agora)
    {
        ArgumentNullException.ThrowIfNull(evento);

        return new OrdemEmExecucao
        {
            IdOrdem = evento.IdOrdem,
            IdCliente = evento.IdCliente,
            NomeCliente = evento.NomeCliente,
            EmailCliente = evento.EmailCliente,
            IdVeiculo = evento.IdVeiculo,
            DescricaoVeiculo = evento.DescricaoVeiculo,
            IdFuncionarioResponsavel = evento.IdFuncionario,
            Etapa = EEtapaDeExecucao.AguardandoDiagnostico,
            AbertaEm = evento.AbertaEm,
        };
    }

    /// <summary>
    /// O mecânico terminou o diagnóstico e lançou os itens.
    ///
    /// Este é o passo que o mapeamento do schema revelou: é daqui que sai o
    /// valor do orçamento, e é por isso que o evento leva os itens dentro, em
    /// vez de o Billing ir buscá-los (ver fase4/mapa-do-schema.md).
    /// </summary>
    public static void ConcluirDiagnostico(
        OrdemEmExecucao ordem,
        IReadOnlyList<ItemLancado> itens,
        string correlationId,
        DateTime agora)
    {
        ArgumentNullException.ThrowIfNull(ordem);
        ArgumentNullException.ThrowIfNull(itens);

        if (ordem.Etapa != EEtapaDeExecucao.AguardandoDiagnostico)
            throw new InvalidOperationException(
                $"Só uma ordem aguardando diagnóstico pode ser diagnosticada. Esta está {ordem.Etapa}.");

        if (itens.Count == 0)
            throw new ArgumentException("Um diagnóstico sem itens não fecha.", nameof(itens));

        foreach (var item in itens)
        {
            if (item.Quantidade <= 0)
                throw new ArgumentException($"Quantidade inválida em '{item.Descricao}'.", nameof(itens));

            if (item.Tipo == ETipoItem.Peca && string.IsNullOrWhiteSpace(item.IdPeca))
                throw new ArgumentException($"O item de peça '{item.Descricao}' precisa apontar para uma peça.", nameof(itens));

            if (item.Tipo == ETipoItem.MaoDeObra && !string.IsNullOrWhiteSpace(item.IdPeca))
                throw new ArgumentException($"Mão de obra não aponta para peça: '{item.Descricao}'.", nameof(itens));
        }

        ordem.Itens = [.. itens];
        ordem.ValorTotal = itens.Sum(i => i.Subtotal);
        ordem.Etapa = EEtapaDeExecucao.AguardandoLiberacao;
        ordem.DiagnosticadaEm = agora;

        ordem.EventosPendentes.Add(Outbox.Enfileirar(
            new DiagnosticoConcluido(
                ordem.IdOrdem,
                ordem.IdFuncionarioResponsavel,
                // O cliente viaja junto: é o que permite ao Billing montar o
                // orçamento e criar a cobrança sem consultar o Identidade.
                ordem.IdCliente,
                ordem.NomeCliente,
                ordem.EmailCliente,
                [.. itens.Select(i => new ItemDaOrdem(i.Descricao, i.Tipo, i.Quantidade, i.ValorUnitario))],
                ordem.ValorTotal,
                agora),
            correlationId,
            Origem,
            agora));
    }

    /// <summary>
    /// O pagamento entrou e a oficina pode trabalhar.
    ///
    /// Idempotente de propósito: <c>PagamentoConfirmado</c> chega ao menos uma
    /// vez, e a segunda entrega não pode virar erro.
    /// </summary>
    public static void Liberar(OrdemEmExecucao ordem, DateTime agora)
    {
        ArgumentNullException.ThrowIfNull(ordem);

        if (ordem.Etapa is EEtapaDeExecucao.EmExecucao or EEtapaDeExecucao.Finalizada)
            return;

        if (ordem.Etapa != EEtapaDeExecucao.AguardandoLiberacao)
            throw new InvalidOperationException(
                $"Só uma ordem diagnosticada pode ser liberada. Esta está {ordem.Etapa}.");

        ordem.Etapa = EEtapaDeExecucao.EmExecucao;
    }

    public static void Finalizar(OrdemEmExecucao ordem, string correlationId, DateTime agora)
    {
        ArgumentNullException.ThrowIfNull(ordem);

        if (ordem.Etapa == EEtapaDeExecucao.Finalizada)
            return;

        if (ordem.Etapa != EEtapaDeExecucao.EmExecucao)
            throw new InvalidOperationException(
                $"Só uma ordem em execução pode ser finalizada. Esta está {ordem.Etapa}.");

        ordem.Etapa = EEtapaDeExecucao.Finalizada;
        ordem.FinalizadaEm = agora;

        ordem.EventosPendentes.Add(Outbox.Enfileirar(
            new ExecucaoFinalizada(ordem.IdOrdem, ordem.IdFuncionarioResponsavel, agora),
            correlationId,
            Origem,
            agora));
    }

    /// <summary>
    /// Compensação vinda do orquestrador: a saga desistiu.
    ///
    /// Não publica nada — quem cancelou já anunciou. Devolve <c>false</c> e não
    /// faz nada quando a ordem já está finalizada: um cancelamento que chega
    /// depois do trabalho pronto não pode apagar o trabalho, e é o tipo de
    /// corrida que uma fila produz de verdade.
    /// </summary>
    public static bool Cancelar(OrdemEmExecucao ordem, string motivo, DateTime agora)
    {
        ArgumentNullException.ThrowIfNull(ordem);

        if (ordem.Etapa == EEtapaDeExecucao.Finalizada)
            return false;

        if (ordem.Etapa == EEtapaDeExecucao.Cancelada)
            return true;

        ordem.Etapa = EEtapaDeExecucao.Cancelada;
        ordem.MotivoDoCancelamento = motivo;

        return true;
    }

    /// <summary>
    /// As peças que uma ordem precisa reservar ou devolver, agrupadas por peça.
    /// Separado das transições porque o estoque é outro agregado, com o seu
    /// próprio documento e a sua própria escrita.
    /// </summary>
    public static IReadOnlyDictionary<string, int> PecasDa(OrdemEmExecucao ordem)
    {
        ArgumentNullException.ThrowIfNull(ordem);

        return ordem.Itens
            .Where(i => i.Tipo == ETipoItem.Peca && !string.IsNullOrWhiteSpace(i.IdPeca))
            .GroupBy(i => i.IdPeca!)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.Quantidade));
    }
}

/// <summary>
/// Reserva e devolução de peças.
///
/// Reservar no diagnóstico, e não na execução, é o que impede dois orçamentos
/// prometerem a mesma peça. O preço disso é que a reserva vira mais um passo
/// que a compensação precisa desfazer.
/// </summary>
public static class Estoque
{
    public static void Reservar(Peca peca, int quantidade)
    {
        ArgumentNullException.ThrowIfNull(peca);

        if (quantidade <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade reservada tem que ser positiva.");

        if (!peca.Ativo)
            throw new InvalidOperationException($"A peça '{peca.Nome}' está inativa e não pode ser reservada.");

        if (peca.Disponivel < quantidade)
            throw new InvalidOperationException(
                $"Estoque insuficiente de '{peca.Nome}': disponível {peca.Disponivel}, pedido {quantidade}.");

        peca.QuantidadeReservada += quantidade;
    }

    /// <summary>
    /// Devolve o que tinha sido reservado. Nunca deixa a reserva negativa: uma
    /// devolução repetida, que a fila produz, não pode criar estoque do nada.
    /// </summary>
    public static void Devolver(Peca peca, int quantidade)
    {
        ArgumentNullException.ThrowIfNull(peca);

        peca.QuantidadeReservada = Math.Max(0, peca.QuantidadeReservada - quantidade);
    }

    /// <summary>
    /// A peça saiu do estoque de verdade, quando o serviço foi executado. A
    /// reserva vira baixa.
    /// </summary>
    public static void Baixar(Peca peca, int quantidade)
    {
        ArgumentNullException.ThrowIfNull(peca);

        var baixa = Math.Min(quantidade, peca.QuantidadeEmEstoque);

        peca.QuantidadeEmEstoque -= baixa;
        peca.QuantidadeReservada = Math.Max(0, peca.QuantidadeReservada - quantidade);
    }
}
