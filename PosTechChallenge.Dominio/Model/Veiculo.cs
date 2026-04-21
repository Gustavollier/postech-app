using System.Text.RegularExpressions;

namespace PosTechChallenge.Dominio.Model
{
    public class Veiculo
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public string? Cor { get; set; }
        public int AnoModelo { get; set; }
        public int AnoFabricacao { get; set; }
        public int KmEntrada { get; set; }
        public bool Ativo { get; set; } = true;
    }

    public sealed class Placa
    {
        private static readonly Regex PlacaRegex = new Regex(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Valor { get; }
        private Placa() { }

        public Placa(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ArgumentException("A placa não pode estar vazia.");

            string valorFormatado = valor.Trim().ToUpper();

            if (!PlacaRegex.IsMatch(valorFormatado))
                throw new ArgumentException($"O formato da placa '{valor}' é inválido.");

            Valor = valorFormatado;
        }

        public static implicit operator string(Placa placa) => placa.Valor;

        public override string ToString() => Valor;
        public override bool Equals(object? obj) => obj is Placa p && p.Valor == Valor;
        public override int GetHashCode() => Valor.GetHashCode();
    }
}
