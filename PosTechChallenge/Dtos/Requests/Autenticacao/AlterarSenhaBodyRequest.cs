namespace PosTechChallenge.Dtos.Requests.Autenticacao;

public class AlterarSenhaBodyRequest
{
    public string SenhaAtual { get; set; }
    public string NovaSenha { get; set; }
    public string ConfirmacaoSenha { get; set; }
}
