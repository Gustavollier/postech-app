using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PosTech.Contratos;
using PosTech.Mensageria;

namespace PosTech.Execucao;

/// <summary>
/// Onde a ordem está dentro da oficina.
///
/// É o estado operacional, diferente do status que o cliente vê e diferente do
/// passo da saga. O mecânico só precisa saber se a ordem está esperando
/// diagnóstico, esperando liberação ou na bancada.
/// </summary>
public enum EEtapaDeExecucao
{
    AguardandoDiagnostico = 0,
    DiagnosticoConcluido = 1,

    /// <summary>Diagnóstico feito, esperando o pagamento liberar o trabalho.</summary>
    AguardandoLiberacao = 2,

    EmExecucao = 3,
    Finalizada = 4,
    Cancelada = 5,
}

/// <summary>
/// Uma ordem na fila da oficina, como documento.
///
/// Mesmo padrão do Billing: o outbox mora embutido, então gravar a mudança e
/// enfileirar o evento é uma escrita só, atômica sem precisar de transação
/// multi-documento (ADR-0007).
/// </summary>
public sealed class OrdemEmExecucao
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public int IdOrdem { get; set; }

    public int IdVeiculo { get; set; }

    public string DescricaoVeiculo { get; set; } = string.Empty;

    public int IdFuncionarioResponsavel { get; set; }

    public EEtapaDeExecucao Etapa { get; set; } = EEtapaDeExecucao.AguardandoDiagnostico;

    public List<ItemLancado> Itens { get; set; } = [];

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal ValorTotal { get; set; }

    public string? MotivoDoCancelamento { get; set; }

    public DateTime AbertaEm { get; set; }

    public DateTime? DiagnosticadaEm { get; set; }

    public DateTime? FinalizadaEm { get; set; }

    public List<EventoOutbox> EventosPendentes { get; set; } = [];
}

/// <summary>
/// Um item lançado no diagnóstico. Peça tem <see cref="IdPeca"/>; mão de obra
/// não — é o que o <c>CHECK</c> recomendado no modelo relacional garantiria, e
/// aqui a forma do documento já deixa explícito.
/// </summary>
public sealed class ItemLancado
{
    public string Descricao { get; set; } = string.Empty;

    public ETipoItem Tipo { get; set; }

    public string? IdPeca { get; set; }

    public int Quantidade { get; set; }

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal ValorUnitario { get; set; }

    /// <summary>Calculado, não persistido: o driver não mapeia propriedade só-leitura.</summary>
    [BsonIgnore]
    public decimal Subtotal => Quantidade * ValorUnitario;
}

/// <summary>
/// Peça do catálogo, com o saldo em estoque.
/// </summary>
public sealed class Peca
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Nome { get; set; } = string.Empty;

    public string Marca { get; set; } = string.Empty;

    public string Codigo { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Preco { get; set; }

    public int QuantidadeEmEstoque { get; set; }

    /// <summary>
    /// Peças separadas para ordens que ainda não foram executadas.
    ///
    /// Reservar na hora do diagnóstico, e não na hora de executar, é o que
    /// impede dois orçamentos prometerem a mesma peça. E é por isso que
    /// <c>OrdemCancelada</c> precisa devolver: a reserva é mais um passo que a
    /// compensação tem que desfazer.
    /// </summary>
    public int QuantidadeReservada { get; set; }

    public int Disponivel => QuantidadeEmEstoque - QuantidadeReservada;

    public bool Ativo { get; set; } = true;
}
