namespace PosTechChallenge.Aplicacao.Dto.Cliente;

public sealed record AtualizarClienteDto(
    string NomeCompleto,
    string? CPF,
    string? CNPJ,
    string? Telefone,
    string? Email);