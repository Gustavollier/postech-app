namespace PosTechChallenge.Aplicacao.Dto.ItemOS;

public sealed record AtualizarItemOSDto(
    int TipoItem,
    int QuantidadeItem,
    int? IdFuncionario,
    int? IdPeca);
