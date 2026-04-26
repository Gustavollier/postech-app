namespace PosTechChallenge.Dtos.Responses.OrdemServico;

public sealed record StatusHistoricoOrdemServicoResponse
{
    public int Id { get; init; }
    public int IdOS { get; init; }
    public DateTime UpdatedAt { get; init; }
    public int IdFuncionario { get; init; }
    public int StatusAtual { get; init; }
}
