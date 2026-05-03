using PosTechChallenge.Aplicacao.Dto.Autenticacao;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.UseCases.Autenticacao;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Services;

public sealed class AutenticacaoService : IAutenticacaoService
{
    private readonly LoginUseCase _loginUseCase;
    private readonly CriarSenhaUseCase _criarSenhaUseCase;
    private readonly AlterarSenhaUseCase _alterarSenhaUseCase;

    public AutenticacaoService(
        LoginUseCase loginUseCase,
        CriarSenhaUseCase criarSenhaUseCase,
        AlterarSenhaUseCase alterarSenhaUseCase)
    {
        _loginUseCase = loginUseCase;
        _criarSenhaUseCase = criarSenhaUseCase;
        _alterarSenhaUseCase = alterarSenhaUseCase;
    }

    public async Task<Resultado<TokenResponseDto>> LoginAsync(string cpf, string senha) 
        => await _loginUseCase.LoginAsync(cpf, senha);
    public async Task<Resultado> CriarSenhaAsync(string cpf, string senha, string confirmacaoSenha) 
            => await _criarSenhaUseCase.CriarSenhaAsync(cpf, senha, confirmacaoSenha);

    public async Task<Resultado> AlterarSenhaAsync(
        int funcionarioId,
        string senhaAtual,
        string novaSenha,
        string confirmacaoSenha)
        => await _alterarSenhaUseCase.AlterarSenhaAsync(funcionarioId, senhaAtual, novaSenha, confirmacaoSenha);
}
