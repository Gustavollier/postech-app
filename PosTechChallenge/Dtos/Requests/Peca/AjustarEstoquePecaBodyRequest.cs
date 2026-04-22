using System.ComponentModel.DataAnnotations;

namespace PosTechChallenge.Dtos.Requests.Peca;

public sealed record AjustarEstoquePecaBodyRequest
{
    [Required]
    public int Quantidade { get; init; }
}