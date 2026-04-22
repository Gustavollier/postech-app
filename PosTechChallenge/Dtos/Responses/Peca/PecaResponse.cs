namespace PosTechChallenge.Dtos.Responses.Peca;

public sealed record PecaResponse
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Marca { get; init; }
    public string? Codigo { get; init; }
    public string Preco { get; init; } = string.Empty;
    public int UnidadeMedida { get; init; }
    public int QuantidadeEstoque { get; init; }
    public DateTime CriadoEm { get; init; }
    public DateTime AtualizadoEm { get; init; }
}