namespace PosTechChallenge.Aplicacao.Dto.Funcionario;

public sealed record CriarFuncionarioDto(string Nome, string Contato, string CPF, int Cargo, int ValorHora);