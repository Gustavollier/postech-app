namespace PosTechChallenge.Dominio.Results;

public sealed record Resultado<T>(bool IsValid, string? Message = null, T? Output = default)
{
    public static Resultado<T> Sucesso(T output, string? message = null)
        => new(true, message, output);

    public static Resultado<T> Falha(string message)
        => new(false, message, default);
}

public sealed record Resultado(bool IsValid, string? Message = null)
{
    public static Resultado Sucesso(string? message = null)
        => new(true, message);

    public static Resultado Falha(string message)
        => new(false, message);
}
