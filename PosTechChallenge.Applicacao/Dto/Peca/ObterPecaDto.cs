namespace PosTechChallenge.Aplicacao.Dto.Peca;

public sealed record ObterPecaDto
{
    public int Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string? Marca { get; init; }
    public string? Codigo { get; init; }
    public decimal Preco { get; init; }
    public int UnidadeMedida { get; init; }
    public int QuantidadeEstoque { get; init; }
    public DateTime CriadoEm { get; init; }
    public DateTime AtualizadoEm { get; init; }
    public bool Ativo { get; init; }
}