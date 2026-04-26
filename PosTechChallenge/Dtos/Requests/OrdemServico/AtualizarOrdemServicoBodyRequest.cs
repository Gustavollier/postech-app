using System.ComponentModel.DataAnnotations;

namespace PosTechChallenge.Dtos.Requests.OrdemServico;

public sealed record AtualizarOrdemServicoBodyRequest
{
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "IdCliente deve ser um número positivo.")]
    public int IdCliente { get; init; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "IdVeiculo deve ser um número positivo.")]
    public int IdVeiculo { get; init; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "IdFuncionario deve ser um número positivo.")]
    public int IdFuncionario { get; init; }
}
