using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Aplicacao.Dto.Funcionario;

public sealed record AtualizarFuncionarioDto(string Nome, string Contato, string CPF, ECargoFuncionario Cargo, int ValorHora);
