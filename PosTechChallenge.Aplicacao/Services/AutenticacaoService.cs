using System.Threading.Tasks;
using PosTechChallenge.Applicacao.Interface.Services;
using PosTechChallenge.Dominio.ValueObjects;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using System;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Applicacao.Services
{
    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly IFuncionarioRepositorio _funcionarioRepositorio;
        private readonly ISegurancaRepositorio _segurancaRepositorio;

        public AutenticacaoService(
            IFuncionarioRepositorio funcionarioRepositorio,
            ISegurancaRepositorio segurancaRepositorio)
        {
            _funcionarioRepositorio = funcionarioRepositorio;
            _segurancaRepositorio = segurancaRepositorio;
        }

        public async Task<Resultado> CriarSenhaAsync(string cpf, string senha, string confirmacaoSenha)
        {
            var funcionario = await _funcionarioRepositorio.ObterPorCPFAsync(cpf);
            if (funcionario == null)
                return Resultado.Falha("Funcionário não encontrado.");

            SenhaFuncionarioValueObject senhaVO;
            try
            {
                senhaVO = new SenhaFuncionarioValueObject(senha);
            }
            catch (Exception ex)
            {
                return Resultado.Falha(ex.Message);
            }

            if (!senhaVO.ConfirmarSenha(confirmacaoSenha))
                return Resultado.Falha("As senhas não conferem.");

            var senhaHash = BCrypt.Net.BCrypt.HashPassword(senhaVO.Valor);
            await _segurancaRepositorio.CriarSenhaAsync(funcionario.Id, senhaHash);
            return Resultado.Sucesso("Senha cadastrada com sucesso.");
        }

        // ... outros métodos, como LoginAsync ...
    }
}