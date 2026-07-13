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

    public Task<Resultado> CriarAsync(CriarFuncionarioDto funcionarioDto, CancellationToken cancellationToken = default)
        => _criarFuncionarioUseCase.CriarAsync(funcionarioDto, cancellationToken);

    public Task<Resultado<ObterFuncionarioDto>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        => _obterFuncionarioUseCase.ObterPorIdAsync(id, cancellationToken);

    public Task<Resultado<ObterFuncionarioDto>> ObterPorCpfAsync(string cpf, CancellationToken cancellationToken = default)
        => _obterFuncionarioUseCase.ObterPorCpfAsync(cpf, cancellationToken);

    public Task<Resultado<ObterFuncionarioDto>> ObterPorNomeAsync(string nome, CancellationToken cancellationToken = default)
        => _obterFuncionarioUseCase.ObterPorNomeAsync(nome, cancellationToken);

    public Task<Resultado<IEnumerable<ObterFuncionarioDto>>> ObterTodosAsync(CancellationToken cancellationToken = default)
        => _obterFuncionarioUseCase.ObterTodosAsync(cancellationToken);

    public Task<Resultado> AtualizarAsync(string cpf, AtualizarFuncionarioDto funcionarioDto, CancellationToken cancellationToken = default)
        => _atualizarFuncionarioUseCase.AtualizarAsync(cpf, funcionarioDto, cancellationToken);

    public Task<Resultado> DeletarAsync(string cpf, CancellationToken cancellationToken = default)
        => _deletarFuncionarioUseCase.DeletarPorCpfAsync(cpf, cancellationToken);
}
