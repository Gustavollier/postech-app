namespace PosTechChallenge.Dtos.Responses.OrdemServico;

public sealed record OrdemServicoResponse
{
    public int Id { get; init; }
    public int IdCliente { get; init; }
    public int IdVeiculo { get; init; }
    public int IdFuncionario { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CriadoEm { get; init; }
    public DateTime AtualizadoEm { get; init; }
}
