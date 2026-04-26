namespace PosTechChallenge.Dtos.Responses.OrdemServico;

public sealed record OrdemServicoResponse
{
    public int Id { get; init; }
    public int IdCliente { get; init; }
    public int IdVeiculo { get; init; }
    public int IdFuncionario { get; init; }
    public int Status { get; init; }
    public DateTime CriadoEm { get; init; }
    public DateTime AtualizadoEm { get; init; }
}
