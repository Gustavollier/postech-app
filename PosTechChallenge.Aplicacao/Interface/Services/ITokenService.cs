using PosTechChallenge.Aplicacao.Dto.Autenticacao;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface ITokenService
{
    Task<TokenResponseDto> GerarTokenAsync(int funcionarioId, string cargo);
}