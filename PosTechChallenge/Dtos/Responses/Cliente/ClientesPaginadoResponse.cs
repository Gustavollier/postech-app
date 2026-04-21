namespace PosTechChallenge.Dtos.Responses.Cliente;

public sealed record ClientesPaginadoResponse
{
    public IEnumerable<ClienteResponse> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
}