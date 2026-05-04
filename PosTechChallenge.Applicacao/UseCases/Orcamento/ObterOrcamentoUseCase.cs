using PosTechChallenge.Aplicacao.Dto.Orcamento;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.UseCases.Orcamento;

public sealed class ObterOrcamentoUseCase
{
    private readonly IOrcamentoRepositorio _orcamentoRepositorio;

    public ObterOrcamentoUseCase(IOrcamentoRepositorio orcamentoRepositorio)
    {
        _orcamentoRepositorio = orcamentoRepositorio;
    }

    public async Task<Resultado<ObterOrcamentoDto>> ObterPorOrdemServicoIdAsync(int ordemServicoId)
    {
        try
        {
            var orcamento = await _orcamentoRepositorio.ObterPorOrdemServicoIdAsync(ordemServicoId);
            if (orcamento == null)
                return Resultado<ObterOrcamentoDto>.Falha($"Orcamento da OS {ordemServicoId} nao encontrado.");

            return Resultado<ObterOrcamentoDto>.Sucesso(new ObterOrcamentoDto
            {
                Id = orcamento.Id,
                IdOS = orcamento.IdOS,
                ValorMaoDeObra = orcamento.ValorMaoDeObra,
                ValorPecas = orcamento.ValorPecas,
                ValorTotal = orcamento.ValorTotal,
                Status = (int)orcamento.Status,
                CriadoEm = orcamento.CriadoEm,
                AtualizadoEm = orcamento.AtualizadoEm
            });
        }
        catch (Exception ex)
        {
            return Resultado<ObterOrcamentoDto>.Falha(ex.Message);
        }
    }
}
