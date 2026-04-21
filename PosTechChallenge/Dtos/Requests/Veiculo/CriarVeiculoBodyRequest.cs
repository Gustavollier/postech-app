using System.ComponentModel.DataAnnotations;

namespace PosTechChallenge.Dtos.Requests.Veiculo;

public sealed record CriarVeiculoBodyRequest
{
    [Required]
    public int ClienteId { get; init; }

    [Required]
    public string Marca { get; init; } = string.Empty;

    [Required]
    public string Modelo { get; init; } = string.Empty;

    [Required]
    public string Placa { get; init; } = string.Empty;

    public string? Cor { get; init; }

    [Required]
    public int AnoModelo { get; init; }

    [Required]
    public int AnoFabricacao { get; init; }

    [Required]
    public int KmEntrada { get; init; }
}