namespace PosTechChallenge.Aplicacao.Dto.OrdemServico;

public sealed record ObterStatusHistoricoOrdemServicoDto
{
    public int Id { get; init; }
    public int IdOS { get; init; }
    public DateTime UpdatedAt { get; init; }
    public int IdFuncionario { get; init; }
    public int StatusAtual { get; init; }
}
