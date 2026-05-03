using System;
using System.Text.RegularExpressions;

namespace PosTechChallenge.Dominio.ValueObjects
{
    public class SenhaValueObject
    {
        public string Valor { get; }

        public SenhaValueObject(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
                throw new ArgumentException("Senha não pode ser vazia.");
            
            if (senha.Length < 8)
                throw new ArgumentException("Senha deve ter no mínimo 8 caracteres.");
            
            if (!Regex.IsMatch(senha, "[A-Z]"))
                throw new ArgumentException("Senha deve conter ao menos uma letra maiúscula.");
            
            if (!Regex.IsMatch(senha, "[0-9]"))
                throw new ArgumentException("Senha deve conter ao menos um número.");

            if (!Regex.IsMatch(senha, @"[^a-zA-Z0-9]"))
                throw new ArgumentException("Senha deve conter ao menos um caractere especial.");

            Valor = senha;
        }

        public bool ConfirmarSenha(string confirmacao)
        {
            return string.Equals(Valor, confirmacao);
        }
    }
}
