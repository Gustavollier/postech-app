using PosTechChallenge.Dominio.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface IFuncionarioRepositorio
    {
        Task<IEnumerable<Funcionario>> ObterTodosAsync();
        Task<Funcionario?> ObterPorIdAsync(int id);
        Task<Funcionario?> ObterPorNomeAsync(string nome);
        Task<Funcionario?> ObterPorCPFAsync(string CPF);
        Task<int> CriarAsync(Funcionario funcionario);
        Task<bool> AtualizarAsync(Funcionario funcionario);
        Task<bool> DeletarAsync(int id);
    }
}
