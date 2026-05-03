using PosTechChallenge.Aplicacao.Utils;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dominio.ValueObjects;

namespace PosTechChallenge.Aplicacao.UseCases.Autenticacao;

public class AlterarSenhaUseCase
{
    private readonly ISegurancaRepositorio _segurancaRepositorio;

    public AlterarSenhaUseCase(ISegurancaRepositorio segurancaRepositorio)
    {
        _segurancaRepositorio = segurancaRepositorio;
    }

    public async Task<Resultado> AlterarSenhaAsync(
        int funcionarioId,
        string senhaAtual,
        string novaSenha,
        string confirmacaoSenha)
    {
        try
        {
            if (funcionarioId <= 0 || string.IsNullOrWhiteSpace(senhaAtual) || string.IsNullOrWhiteSpace(novaSenha))
                return Resultado.Falha("Senha atual, nova senha e confirmação de senha são obrigatórios.");

            var seguranca = await _segurancaRepositorio.ObterPorFuncionarioIdAsync(funcionarioId);

            if (seguranca is null)
                return Resultado.Falha("Senha atual inválida.");

            if (PasswordHasher.VerifyPassword(senhaAtual, seguranca.SenhaHash) is false)
                return Resultado.Falha("Senha atual inválida.");

            SenhaValueObject novaSenhaValueObject = new(novaSenha);

            if (novaSenhaValueObject.ConfirmarSenha(confirmacaoSenha) is false)
                return Resultado.Falha("As senhas não conferem.");

            var senhaHash = PasswordHasher.HashPassword(novaSenhaValueObject.Valor);

            await _segurancaRepositorio.CriarSenhaAsync(funcionarioId, senhaHash);

            return Resultado.Sucesso("Senha alterada com sucesso.");
        }
        catch (Exception)
        {
            return Resultado.Falha("Erro ao alterar senha.");
        }
    }
}
