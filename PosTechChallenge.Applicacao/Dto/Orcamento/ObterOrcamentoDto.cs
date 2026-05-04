namespace PosTechChallenge.Aplicacao.Dto.Orcamento;

public sealed record ObterOrcamentoDto
{
    public int Id { get; init; }
    public int IdOS { get; init; }
    public decimal ValorMaoDeObra { get; init; }
    public decimal ValorPecas { get; init; }
    public decimal ValorTotal { get; init; }
    public int Status { get; init; }
    public DateTime CriadoEm { get; init; }
    public DateTime AtualizadoEm { get; init; }
}
