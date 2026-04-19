using System.ComponentModel.DataAnnotations;

namespace PosTechChallenge.Dtos.Requests.Autenticacao;

public sealed record LoginBodyRequest
{
    [Required]
    public string CPF { get; init; } = string.Empty;

    [Required]
    public string Senha { get; init; } = string.Empty;
}