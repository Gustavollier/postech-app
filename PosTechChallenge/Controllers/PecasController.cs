using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Aplicacao.Dto.Peca;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dtos.Requests.Peca;
using PosTechChallenge.Dtos.Responses.Peca;

namespace PosTechChallenge.Controllers;

[ApiController]
[Route("api/v1/pecas")]
public class PecasController : ControllerBase
{
    private readonly IPecaService _pecaService;

    public PecasController(IPecaService pecaService)
    {
        _pecaService = pecaService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPecaBodyRequest bodyRequest)
    {
        if (bodyRequest.QuantidadeEstoque < 0)
            return BadRequest(new { message = "Quantidade de estoque não pode ser negativa." });
        var dto = new CriarPecaDto(
            Nome: bodyRequest.Nome,
            Marca: bodyRequest.Marca,
            Codigo: bodyRequest.Codigo,
            Preco: bodyRequest.Preco,
            UnidadeMedida: bodyRequest.UnidadeMedida,
            QuantidadeEstoque: bodyRequest.QuantidadeEstoque);

        var resultado = await _pecaService.CriarAsync(dto);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Created(string.Empty, new { message = resultado.Message });
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos([FromQuery] bool estoqueBaixo = false, [FromQuery] int limiteEstoqueBaixo = 10)
    {
        if (limiteEstoqueBaixo < 0)
            return BadRequest(new { message = "Limite de estoque baixo não pode ser negativo." });

        var resultado = await _pecaService.ObterTodosAsync(estoqueBaixo, limiteEstoqueBaixo);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        var response = resultado.Output?.Select(MapearParaResponse).ToList() ?? [];
        return Ok(response);
    }

    [Authorize(Roles = "Gerente")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] AtualizarPecaBodyRequest bodyRequest)
    {
        if (bodyRequest.QuantidadeEstoque < 0)
            return BadRequest(new { message = "Quantidade de estoque não pode ser negativa." });

        var dto = new AtualizarPecaDto(
            Nome: bodyRequest.Nome,
            Marca: bodyRequest.Marca,
            Codigo: bodyRequest.Codigo,
            Preco: bodyRequest.Preco,
            UnidadeMedida: bodyRequest.UnidadeMedida,
            QuantidadeEstoque: bodyRequest.QuantidadeEstoque);

        var resultado = await _pecaService.AtualizarAsync(id, dto);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    [Authorize(Roles = "Gerente")]
    [HttpPatch("{id:int}/estoque")]
    public async Task<IActionResult> AjustarEstoque([FromRoute] int id, [FromBody] AjustarEstoquePecaBodyRequest bodyRequest)
    {
        if (bodyRequest.Quantidade == 0)
            return BadRequest(new { message = "A quantidade do ajuste não pode ser zero." });

        var dto = new AjustarEstoquePecaDto(bodyRequest.Quantidade);
        var resultado = await _pecaService.AjustarEstoqueAsync(id, dto);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    [Authorize(Roles = "Gerente")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Desativar([FromRoute] int id)
    {
        var resultado = await _pecaService.DesativarAsync(id);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    private static PecaResponse MapearParaResponse(ObterPecaDto dto)
    {
        return new PecaResponse
        {
            Id = dto.Id,
            Nome = dto.Nome,
            Marca = dto.Marca,
            Codigo = dto.Codigo,
            Preco = dto.Preco,
            UnidadeMedida = dto.UnidadeMedida,
            QuantidadeEstoque = dto.QuantidadeEstoque,
            CriadoEm = dto.CriadoEm,
            AtualizadoEm = dto.AtualizadoEm
        };
    }
}