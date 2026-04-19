using PosTechChallenge.Aplicacao.Dto.Autenticacao;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.UseCases.Autenticacao;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Services;

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