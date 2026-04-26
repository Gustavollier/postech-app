using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Aplicacao.Dto.ItemOS;

public sealed record AtualizarItemOSDto(
    ETipoItemOrdemServico TipoItem,
    int QuantidadeItem,
    int? IdFuncionario,
    int? IdPeca);
