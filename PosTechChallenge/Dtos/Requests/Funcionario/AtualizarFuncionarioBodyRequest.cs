using System.ComponentModel.DataAnnotations;

namespace PosTechChallenge.Dtos.Requests.Funcionario;

public sealed record AtualizarFuncionarioBodyRequest
{
    [Required]
    public string Nome { get; init; }

    [Required]
    public string Contato { get; init; }  

    [Required]
    public int Cargo { get; init; }

    [Required]
    public int ValorHora { get; init; }
}
