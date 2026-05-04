using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Aplicacao.Dto.Orcamento;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dtos.Requests.Orcamento;
using PosTechChallenge.Dtos.Responses.Orcamento;
using System.ComponentModel.DataAnnotations;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Controllers;

[ApiController]
[Route("api/v1/orcamentos")]
public sealed class OrcamentoController : ControllerBase
{
    private readonly IOrcamentoService _orcamentoService;

    public OrcamentoController(IOrcamentoService orcamentoService)
    {
        _orcamentoService = orcamentoService;
    }

    [AllowAnonymous]
    [HttpGet("os/{idOS:int}")]
    public async Task<IActionResult> ObterPorOrdemServicoId([FromRoute] int idOS)
    {
        var resultado = await _orcamentoService.ObterPorOrdemServicoIdAsync(idOS);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(MapearParaResponse(resultado.Output!));
    }

    [Authorize]
    [HttpPost("os/{idOS:int}/calcular")]
    public async Task<IActionResult> Calcular([FromRoute] int idOS)
    {
        var resultado = await _orcamentoService.CalcularAsync(idOS);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Ok(MapearParaResponse(resultado.Output!));
    }

    [Authorize]
    [HttpPost("os/{idOS:int}/enviar")]
    public async Task<IActionResult> Enviar([FromRoute] int idOS)
    {
        var resultado = await _orcamentoService.EnviarAsync(idOS);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Ok(new
        {
            message = resultado.Message,
            orcamento = MapearParaResponse(resultado.Output!)
        });
    }

    [AllowAnonymous]
    [HttpPost("os/{idOS:int}/responder")]
    public async Task<IActionResult> Responder([FromRoute] int idOS, [FromBody][Required] ResponderOrcamentoBodyRequest bodyRequest)
    {
        var dto = new ResponderOrcamentoDto(bodyRequest.Status);
        var resultado = await _orcamentoService.ResponderAsync(idOS, dto);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    private static OrcamentoResponse MapearParaResponse(ObterOrcamentoDto dto)
    {
        return new OrcamentoResponse
        {
            Id = dto.Id,
            IdOS = dto.IdOS,
            ValorMaoDeObra = dto.ValorMaoDeObra,
            ValorPecas = dto.ValorPecas,
            ValorTotal = dto.ValorTotal,
            Status = ((EStatusOrcamento)dto.Status).ToString(),
            CriadoEm = dto.CriadoEm,
            AtualizadoEm = dto.AtualizadoEm
        };
    }
}
