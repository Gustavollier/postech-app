using System.ComponentModel.DataAnnotations;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dtos.Requests.Funcionario;

public sealed record AtualizarFuncionarioBodyRequest
{
    [Required]
    public string Nome { get; init; }

    [Required]
    public string Contato { get; init; }  

    [Required]
    public ECargoFuncionario Cargo { get; init; }

    [Required]
    public int ValorHora { get; init; }
}
