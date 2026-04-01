namespace PosTechChallenge.Dtos.Responses.Funcionario;

public sealed record FuncionarioResponse
{
    public int Id { get; init; }
    public string Nome { get; init; }
    public string Contato { get; init; }
    public string CPF { get; init; }
    public int Cargo { get; init; }
    public int ValorHora { get; init; }
}
