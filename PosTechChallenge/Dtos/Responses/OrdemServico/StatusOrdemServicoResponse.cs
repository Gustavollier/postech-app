namespace PosTechChallenge.Dtos.Responses.OrdemServico;

public sealed record StatusOrdemServicoResponse
{
    public int OrdemServicoId { get; init; }
    public int StatusAtual { get; init; }
    public DateTime AtualizadoEm { get; init; }
    public IEnumerable<StatusHistoricoOrdemServicoResponse> Historico { get; init; } = [];
}
