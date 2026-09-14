using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Aplicacao.Dto.ItemOS;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Autorizacao;
using PosTechChallenge.Dtos.Requests.ItemOS;
using PosTechChallenge.Dtos.Responses.ItemOS;

namespace PosTechChallenge.Controllers;

[ApiController]
[Route("api/v1/ordens-servico/{ordemServicoId:int}/itens")]
public sealed class ItensOSController : ControllerBase
{
    private readonly IItemOSService _itemOSService;
    // O item so conhece o IdOS. De quem e a ordem, quem responde e a ordem.
    private readonly IOrdemServicoService _ordemServicoService;

    public ItensOSController(IItemOSService itemOSService, IOrdemServicoService ordemServicoService)
    {
        _itemOSService = itemOSService;
        _ordemServicoService = ordemServicoService;
    }

    private async Task<IActionResult?> NegarSeOrdemDeOutroClienteAsync(int ordemServicoId)
    {
        if (await User.ClienteAcessandoOrdemDeOutroAsync(_ordemServicoService, ordemServicoId))
            return StatusCode(StatusCodes.Status403Forbidden,
                new { message = "Esta ordem de servico pertence a outro cliente." });

        return null;
    }

    // Sem atributo, caia na FallbackPolicy: um cliente autenticado lancava item
    // em ordem de qualquer um. Lancar peca e hora e trabalho da oficina.
    [Authorize(Policy = Perfis.Equipe)]
    [HttpPost]
    public async Task<IActionResult> Criar([FromRoute] int ordemServicoId, [FromBody] CriarItemOSBodyRequest bodyRequest)
    {
        if (bodyRequest.QuantidadeItem <= 0)
            return BadRequest(new { message = "Quantidade do item deve ser maior que zero." });

        var dto = new CriarItemOSDto(
            IdOS: ordemServicoId,
            TipoItem: bodyRequest.TipoItem,
            QuantidadeItem: bodyRequest.QuantidadeItem,
            IdFuncionario: bodyRequest.IdFuncionario,
            IdPeca: bodyRequest.IdPeca);

        var resultado = await _itemOSService.CriarAsync(dto);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Created(string.Empty, new { message = resultado.Message });
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ObterPorOrdemServicoId([FromRoute] int ordemServicoId)
    {
        if (await NegarSeOrdemDeOutroClienteAsync(ordemServicoId) is { } negado)
            return negado;

        var resultado = await _itemOSService.ObterPorOrdemServicoIdAsync(ordemServicoId);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        var response = resultado.Output?.Select(MapearParaResponse).ToList() ?? [];
        return Ok(response);
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId([FromRoute] int ordemServicoId, [FromRoute] int id)
    {
        if (await NegarSeOrdemDeOutroClienteAsync(ordemServicoId) is { } negado)
            return negado;

        var resultado = await _itemOSService.ObterPorIdAsync(ordemServicoId, id);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(MapearParaResponse(resultado.Output!));
    }

    [Authorize(Roles = "Gerente")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int ordemServicoId, [FromRoute] int id, [FromBody] AtualizarItemOSBodyRequest bodyRequest)
    {
        if (bodyRequest.QuantidadeItem <= 0)
            return BadRequest(new { message = "Quantidade do item deve ser maior que zero." });

        var dto = new AtualizarItemOSDto(
            TipoItem: bodyRequest.TipoItem,
            QuantidadeItem: bodyRequest.QuantidadeItem,
            IdFuncionario: bodyRequest.IdFuncionario,
            IdPeca: bodyRequest.IdPeca);

        var resultado = await _itemOSService.AtualizarAsync(ordemServicoId, id, dto);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    [Authorize(Roles = "Gerente")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar([FromRoute] int ordemServicoId, [FromRoute] int id)
    {
        var resultado = await _itemOSService.DeletarAsync(ordemServicoId, id);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    private static ItemOSResponse MapearParaResponse(ObterItemOSDto dto)
    {
        return new ItemOSResponse
        {
            Id = dto.Id,
            IdOS = dto.IdOS,
            TipoItem = dto.TipoItem.ToString(),
            QuantidadeItem = dto.QuantidadeItem,
            IdFuncionario = dto.IdFuncionario,
            IdPeca = dto.IdPeca
        };
    }
}
