namespace PosTechChallenge.Dtos.Responses.Orcamento;

public sealed record OrcamentoResponse
{
    public int Id { get; init; }
    public int IdOS { get; init; }
    public decimal ValorMaoDeObra { get; init; }
    public decimal ValorPecas { get; init; }
    public decimal ValorTotal { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CriadoEm { get; init; }
    public DateTime AtualizadoEm { get; init; }
}
