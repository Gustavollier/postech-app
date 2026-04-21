namespace PosTechChallenge.Aplicacao.Dto.Cliente;

public sealed record CriarClienteDto(
    string NomeCompleto,
    string? CPF,
    string? CNPJ,
    string? Telefone,
    string? Email);