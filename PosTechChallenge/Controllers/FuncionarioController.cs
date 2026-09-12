using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Aplicacao.Dto.Funcionario;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dtos.Requests.Funcionario;
using PosTechChallenge.Dtos.Responses.Funcionario;
using PosTechChallenge.Dominio.ValueObjects;
using static PosTechChallenge.Dominio.Utils.Enums;

namespace PosTechChallenge.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FuncionarioController : ControllerBase
{
    private readonly IFuncionarioService _funcionarioService;

    public FuncionarioController(IFuncionarioService funcionarioService)
    {
        _funcionarioService = funcionarioService;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarFuncionarioBodyRequest bodyRequest, CancellationToken cancellationToken)
    {
        var validacaoCpf = ValidarCpf(bodyRequest.CPF);
        if (validacaoCpf != null)
            return BadRequest(new { message = validacaoCpf });

        var criarFuncionarioDto = new CriarFuncionarioDto(
            Nome: bodyRequest.Nome,
            Contato: bodyRequest.Contato,
            CPF: bodyRequest.CPF,
            Cargo: bodyRequest.Cargo,
            ValorHora: bodyRequest.ValorHora,
            Senha: bodyRequest.Senha,
            ConfirmacaoSenha: bodyRequest.ConfirmacaoSenha
        );

        var resultado = await _funcionarioService.CriarAsync(criarFuncionarioDto, cancellationToken);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Created(string.Empty, new { message = resultado.Message });
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos(CancellationToken cancellationToken)
    {
        var resultado = await _funcionarioService.ObterTodosAsync(cancellationToken);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        var responses = resultado.Output?.Select(f => new FuncionarioResponse
        {
            Id = f.Id,
            Nome = f.Nome,
            Contato = f.Contato,
            CPF = f.CPF,
            Cargo = f.Cargo.ToString(),
            ValorHora = f.ValorHora
        }).ToList();

        return Ok(responses);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var resultado = await _funcionarioService.ObterPorIdAsync(id, cancellationToken);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        var response = new FuncionarioResponse
        {
            Id = resultado.Output.Id,
            Nome = resultado.Output.Nome,
            Contato = resultado.Output.Contato,
            CPF = resultado.Output.CPF,
            Cargo = resultado.Output.Cargo.ToString(),
            ValorHora = resultado.Output.ValorHora
        };

        return Ok(response);
    }

    [HttpGet("cpf")]
    public async Task<IActionResult> ObterPorCpf([FromQuery] string cpf, CancellationToken cancellationToken)
    {
        var validacaoCpf = ValidarCpf(cpf);
        if (validacaoCpf != null)
            return BadRequest(new { message = validacaoCpf });

        var resultado = await _funcionarioService.ObterPorCpfAsync(cpf, cancellationToken);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        var response = new FuncionarioResponse
        {
            Id = resultado.Output.Id,
            Nome = resultado.Output.Nome,
            Contato = resultado.Output.Contato,
            CPF = resultado.Output.CPF,
            Cargo = resultado.Output.Cargo.ToString(),
            ValorHora = resultado.Output.ValorHora
        };

        return Ok(response);
    }

    [HttpGet("nome")]
    public async Task<IActionResult> ObterPorNome([FromQuery] string nome, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return BadRequest(new { message = "Nome é obrigatório." });

        var resultado = await _funcionarioService.ObterPorNomeAsync(nome, cancellationToken);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        var response = new FuncionarioResponse
        {
            Id = resultado.Output.Id,
            Nome = resultado.Output.Nome,
            Contato = resultado.Output.Contato,
            CPF = resultado.Output.CPF,
            Cargo = resultado.Output.Cargo.ToString(),
            ValorHora = resultado.Output.ValorHora
        };

        return Ok(response);
    }

    [Authorize(Roles = "Gerente")]
    [HttpPut]
    public async Task<IActionResult> Atualizar([FromQuery] string cpf, [FromBody] AtualizarFuncionarioBodyRequest bodyRequest, CancellationToken cancellationToken)
    {
        var validacaoCpf = ValidarCpf(cpf);
        if (validacaoCpf != null)
            return BadRequest(new { message = validacaoCpf });

        var atualizarFuncionarioDto = new AtualizarFuncionarioDto(
            Nome: bodyRequest.Nome,
            Contato: bodyRequest.Contato,
            CPF: cpf,
            Cargo: bodyRequest.Cargo,
            ValorHora: bodyRequest.ValorHora
        );

        var resultado = await _funcionarioService.AtualizarAsync(cpf, atualizarFuncionarioDto, cancellationToken);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    [Authorize(Roles = "Gerente")]
    [HttpDelete]
    public async Task<IActionResult> Deletar([FromQuery] string cpf, CancellationToken cancellationToken)
    {
        var validacaoCpf = ValidarCpf(cpf);
        if (validacaoCpf != null)
            return BadRequest(new { message = validacaoCpf });

        var resultado = await _funcionarioService.DeletarAsync(cpf, cancellationToken);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    private static string? ValidarCpf(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return "CPF é obrigatório.";

        try
        {
            _ = new CpfValueObject(cpf);
            return null;
        }
        catch (ArgumentException ex)
        {
            return ex.Message;
        }
    }
}
