namespace PosTechChallenge.Aplicacao.Dto.OrdemServico;

public sealed record CriarOrdemServicoDto(
    int IdCliente,
    int IdVeiculo,
    int IdFuncionario);
