using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Aplicacao.Dto.Veiculo;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dtos.Requests.Veiculo;
using PosTechChallenge.Dtos.Responses.Veiculo;

namespace PosTechChallenge.Controllers;

[ApiController]
[Route("api/v1/veiculos")]
public class VeiculoController : ControllerBase
{
    private readonly IVeiculoService _veiculoService;

    public VeiculoController(IVeiculoService veiculoService)
    {
        _veiculoService = veiculoService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarVeiculoBodyRequest bodyRequest)
    {
        var validacao = ValidarCampos(bodyRequest.Placa, bodyRequest.AnoModelo, bodyRequest.AnoFabricacao, bodyRequest.KmEntrada);
        if (validacao != null)
            return BadRequest(new { message = validacao });

        var dto = new CriarVeiculoDto(
            ClienteId: bodyRequest.ClienteId,
            Marca: bodyRequest.Marca,
            Modelo: bodyRequest.Modelo,
            Placa: bodyRequest.Placa,
            Cor: bodyRequest.Cor,
            AnoModelo: bodyRequest.AnoModelo,
            AnoFabricacao: bodyRequest.AnoFabricacao,
            KmEntrada: bodyRequest.KmEntrada);

        var resultado = await _veiculoService.CriarAsync(dto);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Created(string.Empty, new { message = resultado.Message });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId([FromRoute] int id)
    {
        var resultado = await _veiculoService.ObterPorIdAsync(id);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(MapearParaResponse(resultado.Output!));
    }

    [HttpGet("placa/{placa}")]
    public async Task<IActionResult> ObterPorPlaca([FromRoute] string placa)
    {
        if (string.IsNullOrWhiteSpace(placa))
            return BadRequest(new { message = "Placa é obrigatória." });

        var resultado = await _veiculoService.ObterPorPlacaAsync(placa);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(MapearParaResponse(resultado.Output!));
    }

    [HttpGet("/api/v1/clientes/{clienteId:int}/veiculos")]
    public async Task<IActionResult> ObterPorClienteId([FromRoute] int clienteId)
    {
        var resultado = await _veiculoService.ObterPorClienteIdAsync(clienteId);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        var responses = resultado.Output?.Select(MapearParaResponse).ToList() ?? [];
        return Ok(responses);
    }

    [Authorize(Roles = "Gerente")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] AtualizarVeiculoBodyRequest bodyRequest)
    {
        var validacao = ValidarCampos(bodyRequest.Placa, bodyRequest.AnoModelo, bodyRequest.AnoFabricacao, bodyRequest.KmEntrada);
        if (validacao != null)
            return BadRequest(new { message = validacao });

        var dto = new AtualizarVeiculoDto(
            ClienteId: bodyRequest.ClienteId,
            Marca: bodyRequest.Marca,
            Modelo: bodyRequest.Modelo,
            Placa: bodyRequest.Placa,
            Cor: bodyRequest.Cor,
            AnoModelo: bodyRequest.AnoModelo,
            AnoFabricacao: bodyRequest.AnoFabricacao,
            KmEntrada: bodyRequest.KmEntrada);

        var resultado = await _veiculoService.AtualizarAsync(id, dto);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    private static string? ValidarCampos(string placa, int anoModelo, int anoFabricacao, int kmEntrada)
    {
        if (string.IsNullOrWhiteSpace(placa))
            return "Placa é obrigatória.";

        if (anoModelo < 1900)
            return "Ano do modelo inválido.";

        if (anoFabricacao < 1900)
            return "Ano de fabricação inválido.";

        if (kmEntrada < 0)
            return "Km de entrada não pode ser negativo.";

        return null;
    }

    private static VeiculoResponse MapearParaResponse(ObterVeiculoDto veiculo)
    {
        return new VeiculoResponse
        {
            Id = veiculo.Id,
            ClienteId = veiculo.ClienteId,
            Marca = veiculo.Marca,
            Modelo = veiculo.Modelo,
            Placa = veiculo.Placa,
            Cor = veiculo.Cor,
            AnoModelo = veiculo.AnoModelo,
            AnoFabricacao = veiculo.AnoFabricacao,
            KmEntrada = veiculo.KmEntrada
        };
    }
}