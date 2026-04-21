namespace PosTechChallenge.Aplicacao.Dto.Veiculo;

public sealed record CriarVeiculoDto(
    int ClienteId,
    string Marca,
    string Modelo,
    string Placa,
    string? Cor,
    int AnoModelo,
    int AnoFabricacao,
    int KmEntrada);