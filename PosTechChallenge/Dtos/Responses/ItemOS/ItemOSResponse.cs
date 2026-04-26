namespace PosTechChallenge.Dtos.Responses.ItemOS;

public sealed record ItemOSResponse
{
    public int Id { get; init; }
    public int IdOS { get; init; }
    public int TipoItem { get; init; }
    public int QuantidadeItem { get; init; }
    public int? IdFuncionario { get; init; }
    public int? IdPeca { get; init; }
}
