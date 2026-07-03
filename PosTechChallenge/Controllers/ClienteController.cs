using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Aplicacao.Dto.Cliente;
using PosTechChallenge.Aplicacao.Helpers;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dtos.Requests.Cliente;
using PosTechChallenge.Dtos.Responses.Cliente;

namespace PosTechChallenge.Controllers;

[ApiController]
[Route("api/v1/clientes")]
public class ClienteController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClienteController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarClienteBodyRequest bodyRequest)
    {
        var validacaoDocumento = DocumentValidationHelper.ValidateDocument(bodyRequest.CPF, bodyRequest.CNPJ);
        if (validacaoDocumento != null)
            return BadRequest(new { message = validacaoDocumento });

        var criarClienteDto = new CriarClienteDto(
            NomeCompleto: bodyRequest.NomeCompleto,
            CPF: bodyRequest.CPF,
            CNPJ: bodyRequest.CNPJ,
            Telefone: bodyRequest.Telefone,
            Email: bodyRequest.Email);

        var resultado = await _clienteService.CriarAsync(criarClienteDto);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Created(string.Empty, new { message = resultado.Message });
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos(CancellationToken cancellationToken, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page <= 0 || pageSize <= 0)
            return BadRequest(new { message = "Page e PageSize devem ser maiores que zero." });

        var resultado = await _clienteService.ObterTodosAsync(page, pageSize, cancellationToken);

        if (resultado.IsValid is false)
            return NotFound(new { message = resultado.Message });

        return Ok(resultado.Output);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId([FromRoute] int id)
    {
        var resultado = await _clienteService.ObterPorIdAsync(id);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(MapearParaResponse(resultado.Output!));
    }

    [HttpGet("cpf-cnpj/{cpfCnpj}")]
    public async Task<IActionResult> ObterPorCpfCnpj([FromRoute] string cpfCnpj)
    {
        if (string.IsNullOrWhiteSpace(cpfCnpj))
            return BadRequest(new { message = "CPF/CNPJ é obrigatório." });

        var resultado = await _clienteService.ObterPorCpfCnpjAsync(cpfCnpj);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(MapearParaResponse(resultado.Output!));
    }

    [Authorize(Roles = "Gerente")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar([FromRoute] int id, [FromBody] AtualizarClienteBodyRequest bodyRequest)
    {
        var validacaoDocumento = DocumentValidationHelper.ValidateDocument(bodyRequest.CPF, bodyRequest.CNPJ);
        if (validacaoDocumento != null)
            return BadRequest(new { message = validacaoDocumento });

        var atualizarClienteDto = new AtualizarClienteDto(
            NomeCompleto: bodyRequest.NomeCompleto,
            CPF: bodyRequest.CPF,
            CNPJ: bodyRequest.CNPJ,
            Telefone: bodyRequest.Telefone,
            Email: bodyRequest.Email);

        var resultado = await _clienteService.AtualizarAsync(id, atualizarClienteDto);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    [Authorize(Roles = "Gerente")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Desativar([FromRoute] int id)
    {
        var resultado = await _clienteService.DesativarAsync(id);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    private static ClienteResponse MapearParaResponse(ClienteDto dto) => new()
    {
        Id = dto.Id,
        CreatedAt = dto.CreatedAt,
        UpdatedAt = dto.UpdatedAt,
        CPF = dto.CPF,
        CNPJ = dto.CNPJ,
        NomeCompleto = dto.NomeCompleto,
        Telefone = dto.Telefone,
        Email = dto.Email,
        Ativo = dto.Ativo
    };
}