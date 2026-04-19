using System.ComponentModel.DataAnnotations;

namespace PosTechChallenge.Dtos.Requests.Autenticacao;

public sealed record RefreshTokenBodyRequest
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;
}