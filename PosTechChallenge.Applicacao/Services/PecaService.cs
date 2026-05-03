using Microsoft.Extensions.Logging;
using PosTechChallenge.Aplicacao.Dto.Peca;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Services;

public sealed class PecaService : IPecaService
{
    private const string MensagemErroInterno = "Erro interno ao processar peça.";

    private readonly IPecasRepositorio _pecasRepositorio;
    private readonly ILogger<PecaService> _logger;

    public PecaService(
        IPecasRepositorio pecasRepositorio,
        ILogger<PecaService> logger)
    {
        _pecasRepositorio = pecasRepositorio;
        _logger = logger;
    }

    public async Task<Resultado> CriarAsync(CriarPecaDto dto)
    {
        try
        {
            var agora = DateTime.UtcNow;

            var peca = new Pecas
            {
                Nome = dto.Nome,
                Marca = dto.Marca,
                Codigo = dto.Codigo,
                Preco = dto.Preco,
                UnidadeMedida = dto.UnidadeMedida,
                QuantidadeEstoque = dto.QuantidadeEstoque,
                CriadoEm = agora,
                AtualizadoEm = agora,
                Ativo = true
            };

            await _pecasRepositorio.CriarAsync(peca);
            return Resultado.Sucesso("Peça/insumo cadastrada com sucesso.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar peca.");
            return Resultado.Falha(MensagemErroInterno);
        }
    }

    public async Task<Resultado<IEnumerable<ObterPecaDto>>> ObterTodosAsync(bool estoqueBaixo, int limiteEstoqueBaixo)
    {
        try
        {
            var pecas = await _pecasRepositorio.ObterTodosAsync();

            if (estoqueBaixo)
                pecas = pecas.Where(p => p.QuantidadeEstoque <= limiteEstoqueBaixo);

            var lista = pecas.Select(Mapear).ToList();

            if (!lista.Any())
                return Resultado<IEnumerable<ObterPecaDto>>.Falha("Nenhuma peça encontrada.");

            return Resultado<IEnumerable<ObterPecaDto>>.Sucesso(lista);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pecas. Estoque baixo: {EstoqueBaixo}, limite: {LimiteEstoqueBaixo}.", estoqueBaixo, limiteEstoqueBaixo);
            return Resultado<IEnumerable<ObterPecaDto>>.Falha(MensagemErroInterno);
        }
    }

    public async Task<Resultado> AtualizarAsync(int id, AtualizarPecaDto dto)
    {
        try
        {
            var peca = await _pecasRepositorio.ObterPorIdAsync(id);

            if (peca == null)
                return Resultado.Falha($"Peça com ID {id} não encontrada.");

            peca.Nome = dto.Nome;
            peca.Marca = dto.Marca;
            peca.Codigo = dto.Codigo;
            peca.Preco = dto.Preco;
            peca.UnidadeMedida = dto.UnidadeMedida;
            peca.QuantidadeEstoque = dto.QuantidadeEstoque;
            peca.AtualizadoEm = DateTime.UtcNow;
            peca.Ativo = true;

            var atualizado = await _pecasRepositorio.AtualizarAsync(peca);
            return atualizado
                ? Resultado.Sucesso("Peça atualizada com sucesso.")
                : Resultado.Falha("Não foi possível atualizar a peça.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar peca {PecaId}.", id);
            return Resultado.Falha(MensagemErroInterno);
        }
    }

    public async Task<Resultado> AjustarEstoqueAsync(int id, AjustarEstoquePecaDto dto)
    {
        try
        {
            var peca = await _pecasRepositorio.ObterPorIdAsync(id);

            if (peca == null)
                return Resultado.Falha($"Peça com ID {id} não encontrada.");

            var novaQuantidade = peca.QuantidadeEstoque + dto.Quantidade;

            if (novaQuantidade < 0)
                return Resultado.Falha("Ajuste inválido. O estoque não pode ficar negativo.");

            var atualizado = await _pecasRepositorio.AjustarEstoqueAsync(id, novaQuantidade, DateTime.UtcNow);
            return atualizado
                ? Resultado.Sucesso("Estoque ajustado com sucesso.")
                : Resultado.Falha("Não foi possível ajustar o estoque.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ajustar estoque da peca {PecaId}.", id);
            return Resultado.Falha(MensagemErroInterno);
        }
    }

    public async Task<Resultado> DesativarAsync(int id)
    {
        try
        {
            var peca = await _pecasRepositorio.ObterPorIdAsync(id);

            if (peca == null)
                return Resultado.Falha($"Peça com ID {id} não encontrada.");

            var desativado = await _pecasRepositorio.DeletarAsync(id, DateTime.UtcNow);
            return desativado
                ? Resultado.Sucesso("Peça desativada com sucesso.")
                : Resultado.Falha("Não foi possível desativar a peça.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar peca {PecaId}.", id);
            return Resultado.Falha(MensagemErroInterno);
        }
    }

    private static ObterPecaDto Mapear(Pecas peca)
    {
        return new ObterPecaDto
        {
            Id = peca.Id,
            Nome = peca.Nome,
            Marca = peca.Marca,
            Codigo = peca.Codigo,
            Preco = peca.Preco,
            UnidadeMedida = peca.UnidadeMedida,
            QuantidadeEstoque = peca.QuantidadeEstoque,
            CriadoEm = peca.CriadoEm,
            AtualizadoEm = peca.AtualizadoEm,
            Ativo = peca.Ativo
        };
    }
}
