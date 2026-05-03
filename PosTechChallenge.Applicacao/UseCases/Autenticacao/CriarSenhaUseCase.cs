using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.Utils;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dominio.ValueObjects;

namespace PosTechChallenge.Aplicacao.UseCases.Autenticacao;

public class CriarSenhaUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;
    private readonly ISegurancaRepositorio _segurancaRepositorio;
    private readonly ITokenService _tokenService;

    public CriarSenhaUseCase(
        IFuncionarioRepositorio funcionarioRepositorio,
        ISegurancaRepositorio segurancaRepositorio,
        ITokenService tokenService)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
        _segurancaRepositorio = segurancaRepositorio;
        _tokenService = tokenService;
    }

    public async Task<Resultado> CriarSenhaAsync(string cpf, string senha, string ConfirmacaoSenha)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(senha))
                return Resultado.Falha("CPF e Senha são obrigatórios.");

            Dominio.Model.Funcionario? funcionario = await _funcionarioRepositorio.ObterPorCPFAsync(cpf);

            if (funcionario == null)
                return Resultado.Falha("CPF ou Senha inválidos.");

            SenhaValueObject senhaFuncionarioValueObject = new(senha);

            if(senhaFuncionarioValueObject.ConfirmarSenha(ConfirmacaoSenha) is false)
                return Resultado.Falha("As senhas não conferem.");

            string senhaHash = PasswordHasher.HashPassword(senhaFuncionarioValueObject.Valor);

            await _segurancaRepositorio.CriarSenhaAsync(funcionario.Id, senhaHash);

            return Resultado.Sucesso("Senha criada com sucesso.");
        }
        catch (Exception)
        {
            return Resultado.Falha("Erro ao criar senha.");
        }
    }
}
