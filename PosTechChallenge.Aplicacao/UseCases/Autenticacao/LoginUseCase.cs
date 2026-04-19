using PosTechChallenge.Aplicacao.Dto.Autenticacao;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Aplicacao.Utils;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;

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

    public async Task<Resultado<TokenResponseDto>> LoginAsync(LoginDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.CPF) || string.IsNullOrWhiteSpace(dto.Senha))
                return Resultado<TokenResponseDto>.Falha("CPF e Senha são obrigatórios.");

            var funcionarios = await _funcionarioRepositorio.ObterTodosAsync();
            var funcionario = funcionarios.FirstOrDefault(f => f.CPF == dto.CPF);
            
            if (funcionario == null)
                return Resultado<TokenResponseDto>.Falha("CPF ou Senha inválidos.");

            var seguranca = await _segurancaRepositorio.ObterPorFuncionarioIdAsync(funcionario.Id);
            if (seguranca == null)
                return Resultado<TokenResponseDto>.Falha("Funcionário não tem senha configurada.");

            if (!PasswordHasher.VerifyPassword(dto.Senha, seguranca.SenhaHash))
                return Resultado<TokenResponseDto>.Falha("CPF ou Senha inválidos.");

            var cargo = funcionario.Cargo.ToString();
            var token = await _tokenService.GerarTokenAsync(funcionario.Id, cargo);

            return Resultado<TokenResponseDto>.Sucesso(token, "Login realizado com sucesso.");
        }
        catch (Exception ex)
        {
            return Resultado<TokenResponseDto>.Falha($"Erro ao fazer login: {ex.Message}");
        }
    }
}