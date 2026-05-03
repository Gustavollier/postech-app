using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Aplicacao.Dto.Cliente;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dtos.Requests.Cliente;
using PosTechChallenge.Dtos.Responses.Cliente;
using PosTechChallenge.Dominio.ValueObjects;

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
        var validacaoDocumento = ValidarDocumento(bodyRequest.CPF, bodyRequest.CNPJ);
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
        var validacaoDocumento = ValidarDocumento(bodyRequest.CPF, bodyRequest.CNPJ);
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

    private static string? ValidarDocumento(string? cpf, string? cnpj)
    {
        var possuiCpf = !string.IsNullOrWhiteSpace(cpf);
        var possuiCnpj = !string.IsNullOrWhiteSpace(cnpj);

        if (!possuiCpf && !possuiCnpj)
            return "Informe CPF ou CNPJ.";

        if (possuiCpf && possuiCnpj)
            return "Informe apenas CPF ou CNPJ.";

        try
        {
            if (possuiCpf)
                _ = new CpfValueObject(cpf!);
            else
                _ = new CnpjValueObject(cnpj!);
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }

        return null;
    }

    private static ClienteResponse MapearParaResponse(ClienteDto cliente)
    {
        return new ClienteResponse
        {
            Id = cliente.Id,
            CreatedAt = cliente.CreatedAt,
            UpdatedAt = cliente.UpdatedAt,
            CPF = cliente.CPF,
            CNPJ = cliente.CNPJ,
            NomeCompleto = cliente.NomeCompleto,
            Telefone = cliente.Telefone,
            Email = cliente.Email,
            Ativo = cliente.Ativo
        };
    }
}