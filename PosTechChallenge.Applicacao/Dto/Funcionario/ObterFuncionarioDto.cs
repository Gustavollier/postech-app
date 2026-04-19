using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Aplicacao.Dto.Funcionario
{
    public sealed record ObterFuncionarioDto
    {
        public int Id { get; init; }
        public string Nome { get; init; }
        public string Contato { get; init; }
        public string CPF { get; init; }
        public ECargoFuncionario Cargo { get; init; }
        public int ValorHora { get; init; }
    }
}
