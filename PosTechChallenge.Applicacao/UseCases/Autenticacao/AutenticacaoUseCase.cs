using BCrypt.Net;
using PosTechChallenge.Aplicacao.Dto.Autenticacao;
using PosTechChallenge.Aplicacao.Dto.Autenticacao;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.Utils;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dominio.ValueObjects;

namespace PosTechChallenge.Aplicacao.UseCases.Autenticacao;

public class LoginUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;
    private readonly ISegurancaRepositorio _segurancaRepositorio;
    private readonly ITokenService _tokenService;

    public LoginUseCase(
        IFuncionarioRepositorio funcionarioRepositorio,
        ISegurancaRepositorio segurancaRepositorio,
        ITokenService tokenService)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
        _segurancaRepositorio = segurancaRepositorio;
        _tokenService = tokenService;
    }

    public async Task<Resultado<TokenResponseDto>> LoginAsync(string cpf, string senha)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(senha))
                return Resultado<TokenResponseDto>.Falha("CPF e Senha são obrigatórios.");

            Dominio.Model.Funcionario? funcionario = await _funcionarioRepositorio.ObterPorCPFAsync(cpf);
            
            if (funcionario == null)
                return Resultado<TokenResponseDto>.Falha("CPF ou Senha inválidos.");

            var seguranca = await _segurancaRepositorio.ObterPorFuncionarioIdAsync(funcionario.Id);

            if (seguranca == null)
                return Resultado<TokenResponseDto>.Falha("Funcionário não tem senha configurada.");

            if (PasswordHasher.VerifyPassword(senha, seguranca.SenhaHash) is false)
                return Resultado<TokenResponseDto>.Falha("CPF ou Senha inválidos.");

            var cargo = funcionario.Cargo.ToString();
            
            var token = _tokenService.GerarToken(funcionario.Id, cargo);

            return Resultado<TokenResponseDto>.Sucesso(token, "Login realizado com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado<TokenResponseDto>.Falha($"Erro ao fazer login: {ex.Message}");
        }
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

            SenhaFuncionarioValueObject senhaFuncionarioValueObject = new(senha);

            if(senhaFuncionarioValueObject.ConfirmarSenha(ConfirmacaoSenha) is false)
                return Resultado.Falha("As senhas não conferem.");

            string senhaHash = BCrypt.Net.BCrypt.HashPassword(senha);

            await _segurancaRepositorio.CriarSenhaAsync(funcionario.Id, senhaHash);

            return Resultado.Sucesso("Senha criada com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado.Falha($"Erro ao criar senha: {ex.Message}");
        }
    }
}
