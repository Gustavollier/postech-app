using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dominio.Model
{
    public class Funcionario
    {
        public int Id { get; set; }

        public string? Nome { get; set; }

        public string? Contato { get; set; }

        public string? CPF { get; set; }

        public ECargoFuncionario Cargo { get; set; }

        public int ValorHora { get; set; }
    }
}
