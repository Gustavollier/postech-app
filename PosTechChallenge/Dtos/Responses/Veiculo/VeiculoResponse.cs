namespace PosTechChallenge.Dtos.Responses.Veiculo;

public sealed record VeiculoResponse
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
}