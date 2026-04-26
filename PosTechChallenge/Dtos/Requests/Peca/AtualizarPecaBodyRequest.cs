using System.ComponentModel.DataAnnotations;

namespace PosTechChallenge.Dtos.Requests.Peca;

public sealed record AtualizarPecaBodyRequest
{
    [Required]
    public string Nome { get; init; } = string.Empty;

    public string? Marca { get; init; }

    public string? Codigo { get; init; }

    [Required]
    public decimal Preco { get; init; }

    [Required]
    public int UnidadeMedida { get; init; }

    [Required]
    public int QuantidadeEstoque { get; init; }
}