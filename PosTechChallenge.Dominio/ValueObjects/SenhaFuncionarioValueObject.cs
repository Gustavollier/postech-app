using System;
using System.Text.RegularExpressions;

namespace PosTechChallenge.Dominio.ValueObjects
{
    public class SenhaFuncionarioValueObject
    {
        public string Valor { get; }

        public SenhaFuncionarioValueObject(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("Senha não pode ser vazia.");
            if (senha.Length < 8)
                throw new ArgumentException("Senha deve ter no mínimo 8 caracteres.");
            if (!Regex.IsMatch(senha, "[A-Z]"))
                throw new ArgumentException("Senha deve conter ao menos uma letra maiúscula.");
            Valor = senha;
        }

        public bool ConfirmarSenha(string confirmacao)
        {
            return string.Equals(Valor, confirmacao);
        }
    }
}
