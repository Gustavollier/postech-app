using System.Text.RegularExpressions;

namespace PosTechChallenge.Dominio.Model
{
    public class Veiculo
    {
        public int Id { get; private set; }
        public string Marca { get; private set; }
        public string Modelo { get; private set; }
        public Placa Placa { get; private set; }
        public string Cor { get; private set; }
        public int AnoModelo { get; private set; }
        public int AnoFabricacao { get; private set; }
        public int KmEntrada { get; private set; }

        private Veiculo() { }

        public Veiculo(string marca, string modelo, string placa, string cor, int anoModelo, int anoFabricacao, int kmEntrada)
        {
            if (string.IsNullOrWhiteSpace(marca)) throw new ArgumentException("Marca é obrigatória.");
            if (string.IsNullOrWhiteSpace(modelo)) throw new ArgumentException("Modelo é obrigatório.");
            if (anoModelo < 1900) throw new ArgumentException("Ano do modelo inválido.");
            if (kmEntrada < 0) throw new ArgumentException("KM de entrada não pode ser negativa.");

            Marca = marca;
            Modelo = modelo;
            Placa = new Placa(placa);
            Cor = cor;
            AnoModelo = anoModelo;
            AnoFabricacao = anoFabricacao;
            KmEntrada = kmEntrada;
        }
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
