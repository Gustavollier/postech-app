using PosTech.Contratos;
using PosTech.Execucao;

namespace PosTech.Fase4.Testes;

public class GestorDaExecucaoTests
{
    private static readonly DateTime Agora = new(2026, 11, 1, 12, 0, 0, DateTimeKind.Utc);

    private static OrdemEmExecucao Recebida(int idOrdem = 1) =>
        GestorDaExecucao.Receber(
            new OrdemAberta(idOrdem, 10, "Ana Prado", "ana@oficina.test", 20, "Gol AAA-1234", 30, Agora),
            Agora);

    private static ItemLancado MaoDeObra(string descricao, decimal valor, int qtd = 1) =>
        new() { Descricao = descricao, Tipo = ETipoItem.MaoDeObra, Quantidade = qtd, ValorUnitario = valor };

    private static ItemLancado PecaItem(string descricao, string idPeca, decimal valor, int qtd = 1) =>
        new() { Descricao = descricao, Tipo = ETipoItem.Peca, IdPeca = idPeca, Quantidade = qtd, ValorUnitario = valor };

    [Fact]
    public void Receber_poe_a_ordem_na_fila_sem_publicar_nada()
    {
        // Receber não é fato novo para ninguém de fora: quem abriu a ordem já
        // sabe que ela existe.
        var ordem = Recebida();

        Assert.Equal(EEtapaDeExecucao.AguardandoDiagnostico, ordem.Etapa);
        Assert.Equal("Gol AAA-1234", ordem.DescricaoVeiculo);
        Assert.Empty(ordem.EventosPendentes);
    }

    [Fact]
    public void Diagnostico_calcula_o_total_e_publica_os_itens_junto()
    {
        var ordem = Recebida();

        GestorDaExecucao.ConcluirDiagnostico(
            ordem,
            [MaoDeObra("Troca de óleo", 120.50m), PecaItem("Filtro", "peca-1", 38.90m, 2)],
            "corr-1",
            Agora);

        Assert.Equal(EEtapaDeExecucao.AguardandoLiberacao, ordem.Etapa);
        Assert.Equal(198.30m, ordem.ValorTotal);

        var evento = Assert.Single(ordem.EventosPendentes);
        Assert.Equal("DiagnosticoConcluido", evento.Tipo);

        // Os itens viajam dentro do evento. É o que permite ao Billing montar o
        // orçamento sem ler o banco do Execução.
        Assert.Contains("Troca de óleo", evento.Payload);
        Assert.Contains("Filtro", evento.Payload);

        // Acentuação legível e enum por nome: este payload é lido por humanos
        // no painel do RabbitMQ durante a demonstração.
        Assert.DoesNotContain("\\u00F3", evento.Payload);
        Assert.Contains("\"MaoDeObra\"", evento.Payload);
    }

    [Fact]
    public void O_cliente_recebido_na_abertura_viaja_no_diagnostico()
    {
        // O Execução é o elo: recebe o cliente no OrdemAberta, guarda, e
        // repassa no DiagnosticoConcluido. Se parar de repassar, o Billing fica
        // sem e-mail para criar a cobrança e a saga trava no passo do
        // orçamento — sem erro nenhum aparecer aqui. Daí este teste.
        var ordem = Recebida();

        Assert.Equal(10, ordem.IdCliente);
        Assert.Equal("ana@oficina.test", ordem.EmailCliente);

        GestorDaExecucao.ConcluirDiagnostico(ordem, [MaoDeObra("Troca de óleo", 120.50m)], "corr-1", Agora);

        var evento = Assert.Single(ordem.EventosPendentes);
        var diagnostico = SerializacaoDeEvento.Desserializar<DiagnosticoConcluido>(evento.Payload)?.Dados;

        Assert.NotNull(diagnostico);
        Assert.Equal(10, diagnostico.IdCliente);
        Assert.Equal("Ana Prado", diagnostico.NomeCliente);
        Assert.Equal("ana@oficina.test", diagnostico.EmailCliente);
    }

    [Fact]
    public void Item_de_peca_sem_peca_nao_passa()
    {
        var ordem = Recebida();

        var invalido = new ItemLancado
        {
            Descricao = "Filtro", Tipo = ETipoItem.Peca, Quantidade = 1, ValorUnitario = 38.90m,
        };

        Assert.Throws<ArgumentException>(
            () => GestorDaExecucao.ConcluirDiagnostico(ordem, [invalido], "corr-1", Agora));
    }

    [Fact]
    public void Mao_de_obra_apontando_para_peca_nao_passa()
    {
        // É a regra que o CHECK recomendado no modelo relacional garantiria.
        var ordem = Recebida();

        var invalido = new ItemLancado
        {
            Descricao = "Mão de obra", Tipo = ETipoItem.MaoDeObra, IdPeca = "peca-1",
            Quantidade = 1, ValorUnitario = 100m,
        };

        Assert.Throws<ArgumentException>(
            () => GestorDaExecucao.ConcluirDiagnostico(ordem, [invalido], "corr-1", Agora));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Quantidade_nao_positiva_nao_passa(int quantidade)
    {
        var ordem = Recebida();

        Assert.Throws<ArgumentException>(() => GestorDaExecucao.ConcluirDiagnostico(
            ordem, [MaoDeObra("Troca", 100m, quantidade)], "corr-1", Agora));
    }

    [Fact]
    public void Diagnosticar_duas_vezes_nao_passa()
    {
        var ordem = Recebida();
        GestorDaExecucao.ConcluirDiagnostico(ordem, [MaoDeObra("Troca", 100m)], "corr-1", Agora);

        Assert.Throws<InvalidOperationException>(() => GestorDaExecucao.ConcluirDiagnostico(
            ordem, [MaoDeObra("Outra", 50m)], "corr-1", Agora));
    }

    [Fact]
    public void Liberar_duas_vezes_e_no_op()
    {
        // PagamentoConfirmado chega ao menos uma vez.
        var ordem = Recebida();
        GestorDaExecucao.ConcluirDiagnostico(ordem, [MaoDeObra("Troca", 100m)], "corr-1", Agora);

        GestorDaExecucao.Liberar(ordem, Agora);
        GestorDaExecucao.Liberar(ordem, Agora);

        Assert.Equal(EEtapaDeExecucao.EmExecucao, ordem.Etapa);
    }

    [Fact]
    public void Nao_da_para_liberar_uma_ordem_sem_diagnostico()
    {
        Assert.Throws<InvalidOperationException>(() => GestorDaExecucao.Liberar(Recebida(), Agora));
    }

    [Fact]
    public void Finalizar_publica_ExecucaoFinalizada()
    {
        var ordem = Recebida();
        GestorDaExecucao.ConcluirDiagnostico(ordem, [MaoDeObra("Troca", 100m)], "corr-1", Agora);
        GestorDaExecucao.Liberar(ordem, Agora);
        ordem.EventosPendentes.Clear();

        GestorDaExecucao.Finalizar(ordem, "corr-1", Agora);

        Assert.Equal(EEtapaDeExecucao.Finalizada, ordem.Etapa);
        Assert.Equal("ExecucaoFinalizada", Assert.Single(ordem.EventosPendentes).Tipo);
    }

    [Fact]
    public void Cancelamento_que_chega_depois_do_trabalho_pronto_nao_apaga_o_trabalho()
    {
        // A corrida que uma fila produz de verdade: o varredor de timeout
        // decide cancelar no mesmo instante em que o mecânico fecha a ordem.
        var ordem = Recebida();
        GestorDaExecucao.ConcluirDiagnostico(ordem, [MaoDeObra("Troca", 100m)], "corr-1", Agora);
        GestorDaExecucao.Liberar(ordem, Agora);
        GestorDaExecucao.Finalizar(ordem, "corr-1", Agora);

        var cancelou = GestorDaExecucao.Cancelar(ordem, "execução não assumiu", Agora);

        Assert.False(cancelou);
        Assert.Equal(EEtapaDeExecucao.Finalizada, ordem.Etapa);
    }

    [Fact]
    public void Cancelar_duas_vezes_e_no_op_e_continua_dizendo_que_cancelou()
    {
        var ordem = Recebida();

        Assert.True(GestorDaExecucao.Cancelar(ordem, "cliente desistiu", Agora));
        Assert.True(GestorDaExecucao.Cancelar(ordem, "cliente desistiu", Agora));
        Assert.Equal(EEtapaDeExecucao.Cancelada, ordem.Etapa);
    }

    [Fact]
    public void As_pecas_da_ordem_saem_agrupadas_e_sem_mao_de_obra()
    {
        var ordem = Recebida();

        GestorDaExecucao.ConcluirDiagnostico(
            ordem,
            [
                MaoDeObra("Troca de óleo", 120m),
                PecaItem("Filtro", "peca-1", 38.90m, 2),
                PecaItem("Filtro extra", "peca-1", 38.90m, 1),
                PecaItem("Pastilha", "peca-2", 89.90m, 4),
            ],
            "corr-1",
            Agora);

        var pecas = GestorDaExecucao.PecasDa(ordem);

        Assert.Equal(2, pecas.Count);
        Assert.Equal(3, pecas["peca-1"]);
        Assert.Equal(4, pecas["peca-2"]);
    }
}

public class EstoqueTests
{
    private static Peca UmaPeca(int emEstoque = 10, int reservada = 0) => new()
    {
        Nome = "Filtro de óleo", Marca = "Bosch", Codigo = "FO-123",
        Preco = 38.90m, QuantidadeEmEstoque = emEstoque, QuantidadeReservada = reservada,
    };

    [Fact]
    public void Reservar_separa_a_peca_sem_tirar_do_estoque()
    {
        var peca = UmaPeca();

        Estoque.Reservar(peca, 3);

        Assert.Equal(10, peca.QuantidadeEmEstoque);
        Assert.Equal(3, peca.QuantidadeReservada);
        Assert.Equal(7, peca.Disponivel);
    }

    [Fact]
    public void Dois_orcamentos_nao_prometem_a_mesma_peca()
    {
        // É exatamente para isto que a reserva acontece no diagnóstico, e não
        // na execução.
        var peca = UmaPeca(emEstoque: 5);

        Estoque.Reservar(peca, 3);

        Assert.Throws<InvalidOperationException>(() => Estoque.Reservar(peca, 3));
        Assert.Equal(3, peca.QuantidadeReservada);
    }

    [Fact]
    public void Peca_inativa_nao_entra_em_orcamento_novo()
    {
        var peca = UmaPeca();
        peca.Ativo = false;

        Assert.Throws<InvalidOperationException>(() => Estoque.Reservar(peca, 1));
    }

    [Fact]
    public void Devolver_repetido_nao_cria_estoque_do_nada()
    {
        // OrdemCancelada chega ao menos uma vez. Sem o piso em zero, a segunda
        // entrega inventaria peça.
        var peca = UmaPeca(emEstoque: 10, reservada: 2);

        Estoque.Devolver(peca, 2);
        Estoque.Devolver(peca, 2);

        Assert.Equal(0, peca.QuantidadeReservada);
        Assert.Equal(10, peca.QuantidadeEmEstoque);
    }

    [Fact]
    public void Baixar_tira_do_estoque_e_libera_a_reserva()
    {
        var peca = UmaPeca(emEstoque: 10, reservada: 3);

        Estoque.Baixar(peca, 3);

        Assert.Equal(7, peca.QuantidadeEmEstoque);
        Assert.Equal(0, peca.QuantidadeReservada);
        Assert.Equal(7, peca.Disponivel);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Reservar_quantidade_nao_positiva_nao_passa(int quantidade)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Estoque.Reservar(UmaPeca(), quantidade));
    }
}
