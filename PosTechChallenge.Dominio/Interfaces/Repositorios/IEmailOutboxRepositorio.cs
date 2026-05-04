using PosTechChallenge.Dominio.Model;

namespace PosTechChallenge.Dominio.Interfaces.Repositorios;

public interface IEmailOutboxRepositorio
{
    Task<int> CriarAsync(EmailOutbox emailOutbox);
}
