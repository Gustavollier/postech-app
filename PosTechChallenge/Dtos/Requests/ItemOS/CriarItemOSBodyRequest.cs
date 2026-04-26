using System.ComponentModel.DataAnnotations;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dtos.Requests.ItemOS;

public sealed record CriarItemOSBodyRequest
{
    [Required]
    public ETipoItemOrdemServico TipoItem { get; init; }

    [Required]
    public int QuantidadeItem { get; init; }

    public int? IdFuncionario { get; init; }

    public int? IdPeca { get; init; }
}
