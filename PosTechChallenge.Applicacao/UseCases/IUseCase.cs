using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.UseCases;

/// <summary>
/// Interface base para todos os Use Cases.
/// Define contrato uniforme: Input → Processo → Output
///
/// FASE 5: Padronizar todos os Use Cases com interface consistente.
/// </summary>
/// <typeparam name="TInput">Tipo de entrada do Use Case</typeparam>
/// <typeparam name="TOutput">Tipo de saída do Use Case</typeparam>
public interface IUseCase<TInput, TOutput>
{
    /// <summary>
    /// Executa o Use Case de forma assíncrona.
    /// </summary>
    /// <param name="input">Dados de entrada</param>
    /// <returns>Resultado contendo saída ou erro</returns>
    Task<Resultado<TOutput>> ExecuteAsync(TInput input);
}

/// <summary>
/// Interface para Use Cases que não retornam valor (apenas Resultado de sucesso/erro).
/// </summary>
/// <typeparam name="TInput">Tipo de entrada do Use Case</typeparam>
public interface IUseCase<TInput>
{
    /// <summary>
    /// Executa o Use Case de forma assíncrona.
    /// </summary>
    /// <param name="input">Dados de entrada</param>
    /// <returns>Resultado de sucesso ou erro</returns>
    Task<Resultado> ExecuteAsync(TInput input);
}
