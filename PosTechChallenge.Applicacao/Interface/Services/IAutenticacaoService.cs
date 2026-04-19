using PosTechChallenge.Applicacao.Dto.Autenticacao;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Applicacao.Interface.Services;

public interface IAutenticacaoService
{
    Task<Resultado<TokenResponseDto>> LoginAsync(string cpf, string senha);
}
