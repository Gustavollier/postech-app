using PosTechChallenge.Aplicacao.Dto.Autenticacao;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface ITokenService
{
    TokenResponseDto GerarToken(int funcionarioId, string cargo);
}
