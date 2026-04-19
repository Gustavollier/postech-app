using System.ComponentModel.DataAnnotations;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dtos.Requests.Funcionario;
public sealed record CriarFuncionarioBodyRequest
{
    [Required]
    public string Nome { get; init; }
    [Required]
    public string Contato { get; init; }
    [Required]
    public string CPF { get; init; }
    [Required]
    public ECargoFuncionario Cargo { get; init; }
    [Required]
    public int ValorHora { get; init; }
}
