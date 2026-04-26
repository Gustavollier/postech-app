namespace PosTechChallenge.Aplicacao.Dto.Peca;

public sealed record CriarPecaDto(
    string Nome,
    string? Marca,
    string? Codigo,
    int Preco,
    int UnidadeMedida,
    int QuantidadeEstoque);