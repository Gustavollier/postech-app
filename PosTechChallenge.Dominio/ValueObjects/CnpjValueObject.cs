namespace PosTechChallenge.Dominio.ValueObjects
{
    public class CnpjValueObject
    {
        private const int TamanhoCnpj = 14;

        public string Valor { get; }

        public CnpjValueObject(string cnpj)
        {
            var cnpjLimpo = ExtrairDigitos(cnpj);

            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ArgumentException("CNPJ não pode ser vazio.");

            if (cnpjLimpo.Length != TamanhoCnpj)
                throw new ArgumentException("CNPJ deve conter 14 dígitos.");

            if (cnpjLimpo.Distinct().Count() == 1)
                throw new ArgumentException("CNPJ inválido.");

            if (!ValidarDigitosVerificadores(cnpjLimpo))
                throw new ArgumentException("CNPJ inválido.");

            Valor = cnpjLimpo;
        }

        public static implicit operator string(CnpjValueObject cnpj) => cnpj.Valor;

        public override string ToString() => Valor;

        public override bool Equals(object? obj) => obj is CnpjValueObject cnpj && cnpj.Valor == Valor;

        public override int GetHashCode() => Valor.GetHashCode();

        private static bool ValidarDigitosVerificadores(string cnpj)
        {
            var primeiroDigito = CalcularDigito(cnpj, new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }, 12);
            var segundoDigito = CalcularDigito(cnpj, new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 }, 13);

            return cnpj[12] - '0' == primeiroDigito && cnpj[13] - '0' == segundoDigito;
        }

        private static int CalcularDigito(string cnpj, int[] pesos, int quantidadeDigitos)
        {
            var soma = 0;

            for (var indice = 0; indice < quantidadeDigitos; indice++)
            {
                soma += (cnpj[indice] - '0') * pesos[indice];
            }

            var resto = soma % 11;
            return resto < 2 ? 0 : 11 - resto;
        }

        private static string ExtrairDigitos(string valor)
        {
            return new string((valor ?? string.Empty).Where(char.IsDigit).ToArray());
        }
    }
}
