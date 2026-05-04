using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Aplicacao.Dto.OrdemServico;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dominio.Results;
using PosTechChallenge.Dtos.Requests.OrdemServico;
using PosTechChallenge.Dtos.Responses.OrdemServico;
using System.ComponentModel.DataAnnotations;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Controllers;

[ApiController]
[Route("api/v1/ordens-servico")]
public sealed class OrdemServicoController : ControllerBase
{
    private readonly IOrdemServicoService _ordemServicoService;

    public OrdemServicoController(IOrdemServicoService ordemServicoService)
    {
        _ordemServicoService = ordemServicoService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody][Required] CriarOrdemServicoBodyRequest bodyRequest)
    {
        var dto = new CriarOrdemServicoDto(
            IdCliente: bodyRequest.IdCliente,
            IdVeiculo: bodyRequest.IdVeiculo,
            IdFuncionario: bodyRequest.IdFuncionario);

        Resultado resultado = await _ordemServicoService.CriarAsync(dto);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Created(string.Empty, new { message = resultado.Message });
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodosAsync([FromQuery] EStatusOrdemServico? status = null, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        Resultado<IEnumerable<ObterOrdemServicoDto>> resultado = await _ordemServicoService.ObterTodosAsync(status, pageSize ,page);

        if (resultado.IsValid is false)
            return NotFound(new { message = resultado.Message });

        var response = resultado.Output?.Select(MapearParaResponse).ToList() ?? [];

        return Ok(response);
    }

    [HttpGet("cliente/{idCliente:int}")]
    public async Task<IActionResult> ObterPorClienteIdAsync([FromRoute] int idCliente, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        Resultado<IEnumerable<ObterOrdemServicoDto>> resultado = await _ordemServicoService.ObterPorClienteIdAsync(idCliente, pageSize, page);

        if (resultado.IsValid is false)
            return NotFound(new { message = resultado.Message });

        var response = resultado.Output?.Select(MapearParaResponse).ToList() ?? [];

        return Ok(response);
    }

    [HttpGet("valor/{id:int}")]
    public async Task<IActionResult> ObterValorPorIdAsync([FromRoute] int id)
    {
        Resultado<decimal> resultado = await _ordemServicoService.ObterValorPorIdAsync(id);

        if (resultado.IsValid is false)
            return NotFound(new { message = resultado.Message });

        return Ok(new { valor = resultado.Output });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId([FromRoute] int id)
    {
        Resultado<ObterOrdemServicoDto> resultado = await _ordemServicoService.ObterPorIdAsync(id);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(MapearParaResponse(resultado.Output!));
    }

    [AllowAnonymous]
    [HttpGet("{id:int}/status")]
    public async Task<IActionResult> ObterStatus([FromRoute] int id)
    {
        Resultado<ObterStatusOrdemServicoDto> resultado = await _ordemServicoService.ObterStatusAsync(id);

        if (resultado.IsValid is false)
            return NotFound(new { message = resultado.Message });

        return Ok(MapearStatusParaResponse(resultado.Output!));
    }

    [Authorize(Roles = "Gerente")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute][Required] int id, [FromBody][Required] AtualizarOrdemServicoBodyRequest bodyRequest)
    {
        var dto = new AtualizarOrdemServicoDto(
            IdCliente: bodyRequest.IdCliente,
            IdVeiculo: bodyRequest.IdVeiculo,
            IdFuncionario: bodyRequest.IdFuncionario);

        var resultado = await _ordemServicoService.AtualizarAsync(id, dto);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    [Authorize]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> AtualizarStatus([FromRoute][Required] int id, [FromBody][Required] AtualizarStatusOrdemServicoBodyRequest bodyRequest)
    {
        var dto = new AtualizarStatusOrdemServicoDto(
            IdFuncionario: bodyRequest.IdFuncionario,
            Status: bodyRequest.Status);

        Resultado resultado = await _ordemServicoService.AtualizarStatusAsync(id, dto);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    [Authorize(Roles = "Gerente")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Deletar([FromRoute] int id)
    {
        var resultado = await _ordemServicoService.DeletarAsync(id);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    private static OrdemServicoResponse MapearParaResponse(ObterOrdemServicoDto dto)
    {
        return new OrdemServicoResponse
        {
            Id = dto.Id,
            IdCliente = dto.IdCliente,
            IdVeiculo = dto.IdVeiculo,
            IdFuncionario = dto.IdFuncionario,
            Status = dto.Status,
            CriadoEm = dto.CriadoEm,
            AtualizadoEm = dto.AtualizadoEm
        };
    }

    private static StatusOrdemServicoResponse MapearStatusParaResponse(ObterStatusOrdemServicoDto dto)
    {
        return new StatusOrdemServicoResponse
        {
            OrdemServicoId = dto.OrdemServicoId,
            StatusAtual = dto.StatusAtual,
            AtualizadoEm = dto.AtualizadoEm,
            Historico = dto.Historico.Select(historico => new StatusHistoricoOrdemServicoResponse
            {
                Id = historico.Id,
                IdOS = historico.IdOS,
                IdFuncionario = historico.IdFuncionario,
                StatusAtual = historico.StatusAtual,
                UpdatedAt = historico.UpdatedAt
            }).ToList()
        };
    }
}
