using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dtos.Responses.ItemOS;

public sealed record ItemOSResponse
{
    public int Id { get; init; }
    public int IdOS { get; init; }
    public string TipoItem { get; init; }
    public int QuantidadeItem { get; init; }
    public int? IdFuncionario { get; init; }
    public int? IdPeca { get; init; }
}
