using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Dtos.Requests.Orcamento;

public sealed record ResponderOrcamentoBodyRequest
{
    public EStatusOrcamento Status { get; init; }
}
