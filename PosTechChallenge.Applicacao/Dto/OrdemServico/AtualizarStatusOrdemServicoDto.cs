using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Aplicacao.Dto.OrdemServico;

public sealed record AtualizarStatusOrdemServicoDto(
    int IdFuncionario,
    EStatusOrdemServico Status);
