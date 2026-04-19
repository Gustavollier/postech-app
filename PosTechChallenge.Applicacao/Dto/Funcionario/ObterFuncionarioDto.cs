namespace PosTechChallenge.Aplicacao.Dto.Funcionario
{
    public sealed record ObterFuncionarioDto
    {
        public int Id { get; init; }
        public string Nome { get; init; }
        public string Contato { get; init; }
        public string CPF { get; init; }
        public int Cargo { get; init; }
        public int ValorHora { get; init; }
    }
}
