using PosTechChallenge.Aplicacao.Dto.Peca;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Interface.Services;

public interface IPecaService
{
    Task<Resultado> CriarAsync(CriarPecaDto dto);
    Task<Resultado<IEnumerable<ObterPecaDto>>> ObterTodosAsync(bool estoqueBaixo, int limiteEstoqueBaixo);
    Task<Resultado> AtualizarAsync(int id, AtualizarPecaDto dto);
    Task<Resultado> AjustarEstoqueAsync(int id, AjustarEstoquePecaDto dto);
    Task<Resultado> DesativarAsync(int id);
}