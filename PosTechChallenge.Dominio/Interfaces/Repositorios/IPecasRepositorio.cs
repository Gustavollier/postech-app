using PosTechChallenge.Dominio.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IPecasRepositorio
    {
        Task<IEnumerable<Pecas>> ObterTodosAsync();
        Task<Pecas> ObterPorIdAsync(int id);
        Task<int> CriarAsync(Pecas peca);
        Task<bool> AtualizarAsync(Pecas peca);
        Task<bool> DeletarAsync(int id);
    }
}
