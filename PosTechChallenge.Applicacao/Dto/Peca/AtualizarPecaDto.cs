namespace PosTechChallenge.Aplicacao.Dto.Peca;

public sealed record AtualizarPecaDto(
    string Nome,
    string? Marca,
    string? Codigo,
    decimal Preco,
    int UnidadeMedida,
    int QuantidadeEstoque);