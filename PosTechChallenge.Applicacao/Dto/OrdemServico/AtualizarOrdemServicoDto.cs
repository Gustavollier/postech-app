namespace PosTechChallenge.Aplicacao.Dto.OrdemServico;

public sealed record AtualizarOrdemServicoDto(
    int IdCliente,
    int IdVeiculo,
    int IdFuncionario);
