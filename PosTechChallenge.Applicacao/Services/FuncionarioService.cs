using PosTechChallenge.Aplicacao.Dto.Funcionario;
using PosTechChallenge.Aplicacao.UseCases.Funcionario;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Services;

public sealed class FuncionarioService : IFuncionarioService
{
    private readonly CriarFuncionarioUseCase _criarFuncionarioUseCase;
    private readonly ObterFuncionarioUseCase _obterFuncionarioUseCase;
    private readonly AtualizarFuncionarioUseCase _atualizarFuncionarioUseCase;
    private readonly DeletarFuncionarioUseCase _deletarFuncionarioUseCase;

    public FuncionarioService(
        CriarFuncionarioUseCase criarFuncionarioUseCase,
        ObterFuncionarioUseCase obterFuncionarioUseCase,
        AtualizarFuncionarioUseCase atualizarFuncionarioUseCase,
        DeletarFuncionarioUseCase deletarFuncionarioUseCase)
    {
        _criarFuncionarioUseCase = criarFuncionarioUseCase;
        _obterFuncionarioUseCase = obterFuncionarioUseCase;
        _atualizarFuncionarioUseCase = atualizarFuncionarioUseCase;
        _deletarFuncionarioUseCase = deletarFuncionarioUseCase;
    }

    public async Task<Resultado> CriarAsync(CriarFuncionarioDto funcionarioDto) 
        => await _criarFuncionarioUseCase.CriarAsync(funcionarioDto);

    public async Task<Resultado<ObterFuncionarioDto>> ObterPorCpfAsync(string cpf) 
        => await _obterFuncionarioUseCase.ObterPorCpfAsync(cpf);

    public async Task<Resultado<ObterFuncionarioDto>> ObterPorNomeAsync(string nome) 
        => await _obterFuncionarioUseCase.ObterPorNomeAsync(nome);

    public async Task<Resultado<IEnumerable<ObterFuncionarioDto>>> ObterTodosAsync() 
        => await _obterFuncionarioUseCase.ObterTodosAsync();

    public async Task<Resultado> AtualizarAsync(string cpf, AtualizarFuncionarioDto funcionarioDto) 
        => await _atualizarFuncionarioUseCase.AtualizarAsync(cpf, funcionarioDto);

    public async Task<Resultado> DeletarAsync(string cpf) 
        => await _deletarFuncionarioUseCase.DeletarPorCpfAsync(cpf);
}
