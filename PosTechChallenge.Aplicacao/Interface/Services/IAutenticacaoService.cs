using PosTechChallenge.Aplicacao.Dto.Autenticacao;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface IAutenticacaoService
{
    Task<Resultado<TokenResponseDto>> LoginAsync(string cpf, string senha);
}