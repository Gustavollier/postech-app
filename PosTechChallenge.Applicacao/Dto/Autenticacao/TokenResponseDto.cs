namespace PosTechChallenge.Applicacao.Dto.Autenticacao;

public sealed record TokenResponseDto(
    string AccessToken, 
    string? RefreshToken, 
    int ExpiresIn, 
    int FuncionarioId, 
    string Cargo);
