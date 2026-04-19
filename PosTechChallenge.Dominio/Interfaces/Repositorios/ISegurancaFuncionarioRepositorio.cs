using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios;

public interface ISegurancaFuncionarioRepositorio
{
    Task<SegurancaFuncionario?> ObterPorFuncionarioIdAsync(int funcionarioId);
    Task<int> CriarAsync(SegurancaFuncionario seguranca);
    Task<bool> AtualizarAsync(SegurancaFuncionario seguranca);
}
