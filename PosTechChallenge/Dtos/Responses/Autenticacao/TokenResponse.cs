namespace PosTechChallenge.Dtos.Responses.Autenticacao;

public sealed record TokenResponse(
    string AccessToken,
    string? RefreshToken,
    int ExpiresIn,
    int FuncionarioId,
    string Cargo);