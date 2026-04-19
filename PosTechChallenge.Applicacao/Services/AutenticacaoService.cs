using PosTechChallenge.Aplicacao.Dto.Autenticacao;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.UseCases.Autenticacao;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Services;

public sealed class AutenticacaoService : IAutenticacaoService
{
    private readonly LoginUseCase _loginUseCase;
    private readonly CriarSenhaUseCase _criarSenhaUseCase;
    public AutenticacaoService(LoginUseCase loginUseCase, CriarSenhaUseCase criarSenhaUseCase)
    {
        _loginUseCase = loginUseCase;
        _criarSenhaUseCase = criarSenhaUseCase;
    }

    public async Task<Resultado<TokenResponseDto>> LoginAsync(string cpf, string senha) 
        => await _loginUseCase.LoginAsync(cpf, senha);
    public async Task<Resultado> CriarSenhaAsync(string cpf, string senha, string confirmacaoSenha) 
            => await _criarSenhaUseCase.CriarSenhaAsync(cpf, senha, confirmacaoSenha);
}
