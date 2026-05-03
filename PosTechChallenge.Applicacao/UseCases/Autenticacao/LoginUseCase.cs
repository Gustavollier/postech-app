using Microsoft.Extensions.Logging;
using PosTechChallenge.Aplicacao.Dto.Autenticacao;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.Utils;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dominio.ValueObjects;

namespace PosTechChallenge.Aplicacao.UseCases.Autenticacao;

public class LoginUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;
    private readonly ISegurancaRepositorio _segurancaRepositorio;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginUseCase> _logger;

    public LoginUseCase(
        IFuncionarioRepositorio funcionarioRepositorio,
        ISegurancaRepositorio segurancaRepositorio,
        ITokenService tokenService,
        ILogger<LoginUseCase> logger)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
        _segurancaRepositorio = segurancaRepositorio;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<Resultado<TokenResponseDto>> LoginAsync(string cpf, string senha)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(senha))
            {
                _logger.LogWarning("Tentativa de login com CPF ou senha ausentes.");
                return Resultado<TokenResponseDto>.Falha("CPF e Senha são obrigatórios.");
            }

            SenhaValueObject senhaValueObject = new(senha);

            Dominio.Model.Funcionario? funcionario = await _funcionarioRepositorio.ObterPorCPFAsync(cpf);
            
            if (funcionario == null)
            {
                _logger.LogWarning("Falha de login para CPF {Cpf}: funcionario nao encontrado.", cpf);
                return Resultado<TokenResponseDto>.Falha("CPF ou Senha inválidos.");
            }

            var seguranca = await _segurancaRepositorio.ObterPorFuncionarioIdAsync(funcionario.Id);

            if (seguranca == null)
            {
                _logger.LogWarning("Falha de login para funcionario {FuncionarioId}: senha nao configurada.", funcionario.Id);
                return Resultado<TokenResponseDto>.Falha("CPF ou Senha inválidos.");
            }

            if (PasswordHasher.VerifyPassword(senhaValueObject.Valor, seguranca.SenhaHash) is false)
            {
                _logger.LogWarning("Falha de login para funcionario {FuncionarioId}: credenciais invalidas.", funcionario.Id);
                return Resultado<TokenResponseDto>.Falha("CPF ou Senha inválidos.");
            }

            var cargo = funcionario.Cargo.ToString();
            
            var token = _tokenService.GerarToken(funcionario.Id, cargo);

            _logger.LogInformation("Login realizado com sucesso para funcionario {FuncionarioId}.", funcionario.Id);
            return Resultado<TokenResponseDto>.Sucesso(token, "Login realizado com sucesso.");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Falha de login por formato invalido de credenciais para CPF {Cpf}.", cpf);
            return Resultado<TokenResponseDto>.Falha("CPF ou Senha inválidos.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao fazer login para CPF {Cpf}.", cpf);
            return Resultado<TokenResponseDto>.Falha("Erro ao fazer login.");
        }
    }
}
