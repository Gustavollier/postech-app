using PosTechChallenge.Aplicacao.Dto.Orcamento;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Aplicacao.UseCases.Orcamento;

public sealed class CalcularOrcamentoUseCase
{
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;
    private readonly IItemsRepositorio _itemsRepositorio;
    private readonly IOrcamentoRepositorio _orcamentoRepositorio;

    public CalcularOrcamentoUseCase(
        IOrdemServicoRepositorio ordemServicoRepositorio,
        IItemsRepositorio itemsRepositorio,
        IOrcamentoRepositorio orcamentoRepositorio)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio;
        _itemsRepositorio = itemsRepositorio;
        _orcamentoRepositorio = orcamentoRepositorio;
    }

    public async Task<Resultado<ObterOrcamentoDto>> CalcularAsync(int ordemServicoId)
    {
        var resultado = await CalcularOuAtualizarAsync(ordemServicoId);
        return resultado.IsValid
            ? Resultado<ObterOrcamentoDto>.Sucesso(MapearParaDto(resultado.Output!))
            : Resultado<ObterOrcamentoDto>.Falha(resultado.Message!);
    }

    public async Task<Resultado<Dominio.Model.Orcamento>> CalcularOuAtualizarAsync(int ordemServicoId)
    {
        try
        {
            var ordemServico = await _ordemServicoRepositorio.ObterPorIdAsync(ordemServicoId);
            if (ordemServico == null)
                return Resultado<Dominio.Model.Orcamento>.Falha($"Ordem de servico com ID {ordemServicoId} nao encontrada.");

            if (ordemServico.Status is not (EStatusOrdemServico.Recebida or EStatusOrdemServico.EmDiagnostico or EStatusOrdemServico.AguardandoAprovacao))
                return Resultado<Dominio.Model.Orcamento>.Falha("Orcamento so pode ser calculado para OS recebida, em diagnostico ou aguardando aprovacao.");

            var itens = (await _itemsRepositorio.ObterPorOrdemServicoIdAsync(ordemServicoId)).ToList();
            if (itens.Count == 0)
                return Resultado<Dominio.Model.Orcamento>.Falha("A OS precisa ter ao menos um item para gerar orcamento.");

            var valores = await _orcamentoRepositorio.CalcularValoresAsync(ordemServicoId);
            var agora = DateTime.UtcNow;
            var orcamento = await _orcamentoRepositorio.ObterPorOrdemServicoIdAsync(ordemServicoId);

            if (orcamento == null)
            {
                orcamento = new Dominio.Model.Orcamento
                {
                    IdOS = ordemServicoId,
                    CriadoEm = agora
                };
            }

            orcamento.ValorMaoDeObra = valores.ValorMaoDeObra;
            orcamento.ValorPecas = valores.ValorPecas;
            orcamento.ValorTotal = valores.ValorTotal;
            orcamento.Status = EStatusOrcamento.Pendente;
            orcamento.AtualizadoEm = agora;

            if (orcamento.Id == 0)
                orcamento.Id = await _orcamentoRepositorio.CriarAsync(orcamento);
            else if (!await _orcamentoRepositorio.AtualizarAsync(orcamento))
                return Resultado<Dominio.Model.Orcamento>.Falha("Nao foi possivel atualizar o orcamento.");

            return Resultado<Dominio.Model.Orcamento>.Sucesso(orcamento);
        }
        catch (Exception ex)
        {
            return Resultado<Dominio.Model.Orcamento>.Falha(ex.Message);
        }
    }

    private static ObterOrcamentoDto MapearParaDto(Dominio.Model.Orcamento orcamento)
    {
        return new ObterOrcamentoDto
        {
            Id = orcamento.Id,
            IdOS = orcamento.IdOS,
            ValorMaoDeObra = orcamento.ValorMaoDeObra,
            ValorPecas = orcamento.ValorPecas,
            ValorTotal = orcamento.ValorTotal,
            Status = (int)orcamento.Status,
            CriadoEm = orcamento.CriadoEm,
            AtualizadoEm = orcamento.AtualizadoEm
        };
    }
}
