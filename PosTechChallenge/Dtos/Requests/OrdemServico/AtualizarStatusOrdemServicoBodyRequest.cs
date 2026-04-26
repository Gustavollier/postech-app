using System.ComponentModel.DataAnnotations;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dtos.Requests.OrdemServico;

public sealed record AtualizarStatusOrdemServicoBodyRequest
{
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "IdFuncionario deve ser um número positivo.")]   
    public int IdFuncionario { get; init; }

    [Required]
    public EStatusOrdemServico Status { get; init; }
}
