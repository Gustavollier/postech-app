using PosTechChallenge.Aplicacao.Dto.Peca;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dominio.Interfaces.Repositorios;
using PosTechChallenge.Dominio.Model;
using PosTechChallenge.Dominio.Results;

namespace PosTechChallenge.Aplicacao.Services;

public sealed class PecaService : IPecaService
{
    private readonly IPecasRepositorio _pecasRepositorio;

    public PecaService(IPecasRepositorio pecasRepositorio)
    {
        _pecasRepositorio = pecasRepositorio;
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
            return Resultado.Falha(ex.Message);
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
            return Resultado<IEnumerable<ObterPecaDto>>.Falha(ex.Message);
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
            return Resultado.Falha(ex.Message);
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
            return Resultado.Falha(ex.Message);
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
            return Resultado.Falha(ex.Message);
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