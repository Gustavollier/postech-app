using PosTechChallenge.Aplicacao.Dto.Autenticacao;
using PosTechChallenge.Applicacao.Dto.Autenticacao;
using PosTechChallenge.Applicacao.Interface.Services;
using PosTechChallenge.Applicacao.UseCases.Autenticacao;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Applicacao.Services;

public sealed class AutenticacaoService : IAutenticacaoService
{
    private readonly LoginUseCase _loginUseCase;

    public AutenticacaoService(LoginUseCase loginUseCase)
    {
        _loginUseCase = loginUseCase ?? throw new ArgumentNullException(nameof(loginUseCase));
    }

    public async Task<Resultado<TokenResponseDto>> LoginAsync(string cpf, string senha)
    {
        var dto = new LoginDto(cpf, senha);
        return await _loginUseCase.LoginAsync(dto);
    }
}
