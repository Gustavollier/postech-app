using System.ComponentModel.DataAnnotations;

namespace PosTechChallenge.Dtos.Requests.Cliente;

public sealed record AtualizarClienteBodyRequest
{
    [Required]
    public string NomeCompleto { get; init; } = string.Empty;

    public string? CPF { get; init; }

    public string? CNPJ { get; init; }

    public string? Telefone { get; init; }

    public string? Email { get; init; }
}