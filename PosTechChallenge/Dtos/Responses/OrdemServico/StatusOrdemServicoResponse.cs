namespace PosTechChallenge.Dtos.Responses.OrdemServico;

public sealed record StatusOrdemServicoResponse
{
    public int OrdemServicoId { get; init; }
    public string StatusAtual { get; init; } = string.Empty;
    public DateTime AtualizadoEm { get; init; }
    public IEnumerable<StatusHistoricoOrdemServicoResponse> Historico { get; init; } = [];
}
