namespace PosTechChallenge.Dtos.Requests.Autenticacao
{
    public class CriarSenhaBodyRequest
    {
        public string CPF { get; set; }
        public string Senha { get; set; }
        public string ConfirmacaoSenha { get; set; }
    }
}
