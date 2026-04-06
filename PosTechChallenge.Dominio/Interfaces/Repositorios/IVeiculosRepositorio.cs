using PosTechChallenge.Dominio.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IVeiculosRepositorio
    {
        Task<IEnumerable<Veiculo>> ObterTodosAsync();
        Task<Veiculo?> ObterPorIdAsync(int id);
        Task<int> CriarAsync(Veiculo funcionario);
        Task<bool> AtualizarAsync(Veiculo funcionario);
        Task<bool> DeletarAsync(int id);
    }
}
