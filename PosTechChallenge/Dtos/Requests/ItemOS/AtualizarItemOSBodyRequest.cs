using System.ComponentModel.DataAnnotations;

namespace PosTechChallenge.Dtos.Requests.ItemOS;

public sealed record AtualizarItemOSBodyRequest
{
    [Required]
    public int TipoItem { get; init; }

    [Required]
    public int QuantidadeItem { get; init; }

    public int? IdFuncionario { get; init; }

    public int? IdPeca { get; init; }
}
