using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace PosTechChallenge.Aplicacao.UseCases.OrdemServico;
public class ObterValorPorIdUseCase
{
    private readonly IOrdemServicoRepositorio _ordemServicoRepositorio;

    public ObterValorPorIdUseCase(IOrdemServicoRepositorio ordemServicoRepositorio)
    {
        _ordemServicoRepositorio = ordemServicoRepositorio;
    }

    public async Task<Resultado<decimal>> ObterValorPorIdAsync(int id)
    {
        try
        {
            var valor = await _ordemServicoRepositorio.ObterValorPorIdAsync(id);
            return Resultado<decimal>.Sucesso(valor);
        }
        catch (Exception ex)
        {
            return Resultado<decimal>.Falha($"Erro ao obter valor da ordem de serviço: {ex.Message}");
        }
    }
}
