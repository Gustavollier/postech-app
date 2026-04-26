namespace PosTechChallenge.Aplicacao.Dto.OrdemServico;

public sealed record ObterStatusOrdemServicoDto
{
    public int OrdemServicoId { get; init; }
    public int StatusAtual { get; init; }
    public DateTime AtualizadoEm { get; init; }
    public IEnumerable<ObterStatusHistoricoOrdemServicoDto> Historico { get; init; } = [];
}
