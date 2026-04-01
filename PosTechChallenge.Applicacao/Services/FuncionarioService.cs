using PosTechChallenge.Applicacao.Dto.Funcionario;
using PosTechChallenge.Applicacao.UseCases.Funcionario;
using PosTechChallenge.Applicacao.Interface.Services;

namespace PosTechChallenge.Applicacao.Services;

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

    public async Task CriarAsync(CriarFuncionarioDto funcionarioDto) 
        => await _criarFuncionarioUseCase.CriarAsync(funcionarioDto);

    public async Task<ObterFuncionarioDto?> ObterPorCpfAsync(string cpf) 
        => await _obterFuncionarioUseCase.ObterPorCpfAsync(cpf);

    public async Task<ObterFuncionarioDto?> ObterPorNomeAsync(string nome) 
        => await _obterFuncionarioUseCase.ObterPorNomeAsync(nome);

    public async Task<IEnumerable<ObterFuncionarioDto>> ObterTodosAsync() 
        => await _obterFuncionarioUseCase.ObterTodosAsync();

    public async Task<bool> AtualizarAsync(string cpf, AtualizarFuncionarioDto funcionarioDto) 
        => await _atualizarFuncionarioUseCase.AtualizarAsync(cpf, funcionarioDto);

    public async Task<bool> DeletarAsync(string cpf) 
        => await _deletarFuncionarioUseCase.DeletarPorCpfAsync(cpf);
}
