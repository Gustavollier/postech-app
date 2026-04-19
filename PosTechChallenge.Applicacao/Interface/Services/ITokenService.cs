using PosTechChallenge.Applicacao.Dto.Autenticacao;

namespace PosTechChallenge.Applicacao.Interface.Services;

public interface ITokenService
{
    TokenResponseDto GerarToken(int funcionarioId, string cargo);
}
