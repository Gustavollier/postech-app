namespace PosTechChallenge.Aplicacao.Dto.Veiculo;

public sealed record ObterVeiculoDto
{
    public int Id { get; init; }
    public int ClienteId { get; init; }
    public string Marca { get; init; } = string.Empty;
    public string Modelo { get; init; } = string.Empty;
    public string Placa { get; init; } = string.Empty;
    public string? Cor { get; init; }
    public int AnoModelo { get; init; }
    public int AnoFabricacao { get; init; }
    public int KmEntrada { get; init; }
    public bool Ativo { get; init; }
}