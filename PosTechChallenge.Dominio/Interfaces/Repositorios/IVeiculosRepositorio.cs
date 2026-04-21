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
        Task<Veiculo?> ObterPorPlacaAsync(string placa);
        Task<IEnumerable<Veiculo>> ObterPorClienteIdAsync(int clienteId);
        Task<int> CriarAsync(Veiculo veiculo);
        Task<bool> AtualizarAsync(Veiculo veiculo);
        Task<bool> DeletarAsync(int id);
    }
}
