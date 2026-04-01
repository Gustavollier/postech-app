namespace PosTechChallenge.Applicacao.Dto.Funcionario;

public sealed record AtualizarFuncionarioDto(string Nome, string Contato, string CPF, int Cargo, int ValorHora);
