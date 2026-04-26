namespace PosTechChallenge.Aplicacao.Dto.ItemOS;

public sealed record ObterItemOSDto
{
    public int Id { get; init; }
    public int IdOS { get; init; }
    public int TipoItem { get; init; }
    public int QuantidadeItem { get; init; }
    public int? IdFuncionario { get; init; }
    public int? IdPeca { get; init; }
}
