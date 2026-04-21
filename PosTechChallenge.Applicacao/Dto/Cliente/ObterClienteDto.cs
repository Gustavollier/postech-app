namespace PosTechChallenge.Aplicacao.Dto.Cliente;

public sealed record ObterClienteDto
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public string? CPF { get; init; }
    public string? CNPJ { get; init; }
    public string? NomeCompleto { get; init; }
    public string? Telefone { get; init; }
    public string? Email { get; init; }
    public bool Ativo { get; init; }
}