using PosTechChallenge.Dominio.Model;
using System.Threading.Tasks;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios
{
    public interface ISegurancaRepositorio
    {
        Task<Seguranca?> ObterPorFuncionarioIdAsync(int funcionarioId);
        Task SalvarSenhaAsync(int funcionarioId, string senhaHash);
    }
}
