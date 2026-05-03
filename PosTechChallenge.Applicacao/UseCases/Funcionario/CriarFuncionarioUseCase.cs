using PosTechChallenge.Aplicacao.Dto.Funcionario;
using PosTechChallenge.Aplicacao.Utils;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dominio.ValueObjects;

namespace PosTechChallenge.Aplicacao.UseCases.Funcionario;

public class CriarFuncionarioUseCase
{
    private readonly IFuncionarioRepositorio _funcionarioRepositorio;
    private readonly ISegurancaRepositorio _segurancaRepositorio;

    public CriarFuncionarioUseCase(
        IFuncionarioRepositorio funcionarioRepositorio,
        ISegurancaRepositorio segurancaRepositorio)
    {
        _funcionarioRepositorio = funcionarioRepositorio;
        _segurancaRepositorio = segurancaRepositorio;
    }

    public async Task<Resultado> CriarAsync(CriarFuncionarioDto funcionarioDto)
    {
        var funcionarioId = 0;

        try
        {
            SenhaValueObject senhaValueObject = new(funcionarioDto.Senha);

            if (senhaValueObject.ConfirmarSenha(funcionarioDto.ConfirmacaoSenha) is false)
                return Resultado.Falha("As senhas não conferem.");

            Dominio.Model.Funcionario funcionario = new()
            {
                Nome = funcionarioDto.Nome,
                Contato = funcionarioDto.Contato,
                CPF = funcionarioDto.CPF,
                Cargo = funcionarioDto.Cargo,
                ValorHora = funcionarioDto.ValorHora
            };

            funcionarioId = await _funcionarioRepositorio.CriarAsync(funcionario);
            var senhaHash = PasswordHasher.HashPassword(senhaValueObject.Valor);

            await _segurancaRepositorio.SalvarSenhaAsync(funcionarioId, senhaHash);

            return Resultado.Sucesso("Funcionário criado com sucesso.");
        }
        catch (Exception)
        {
            if (funcionarioId > 0)
                await _funcionarioRepositorio.DeletarAsync(funcionarioId);

            return Resultado.Falha("Erro ao criar funcionário.");
        }
    }
}
