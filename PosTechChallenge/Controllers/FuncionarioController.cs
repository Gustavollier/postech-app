using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Applicacao.Dto.Funcionario;
using PosTechChallenge.Applicacao.Interface.Services;
using PosTechChallenge.Dtos.Requests.Funcionario;
using PosTechChallenge.Dtos.Responses.Funcionario;

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
    public async Task<IActionResult> Criar([FromBody] CriarFuncionarioBodyRequest bodyRequest)
    {
        var criarFuncionarioDto = new CriarFuncionarioDto(
            Nome: bodyRequest.Nome,
            Contato: bodyRequest.Contato,
            CPF: bodyRequest.CPF,
            Cargo: bodyRequest.Cargo,
            ValorHora: bodyRequest.ValorHora
        );

        var resultado = await _funcionarioService.CriarAsync(criarFuncionarioDto);

        if (!resultado.IsValid)
            return BadRequest(new { message = resultado.Message });

        return Created(string.Empty, new { message = resultado.Message });
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        var resultado = await _funcionarioService.ObterTodosAsync();

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        var responses = resultado.Output?.Select(f => new FuncionarioResponse
        {
            Id = f.Id,
            Nome = f.Nome,
            Contato = f.Contato,
            CPF = f.CPF,
            Cargo = f.Cargo,
            ValorHora = f.ValorHora
        }).ToList();

        return Ok(responses);
    }

    [HttpGet("cpf")]
    public async Task<IActionResult> ObterPorCpf([FromQuery] string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return BadRequest(new { message = "CPF é obrigatório." });

        var resultado = await _funcionarioService.ObterPorCpfAsync(cpf);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        var response = new FuncionarioResponse
        {
            Id = resultado.Output.Id,
            Nome = resultado.Output.Nome,
            Contato = resultado.Output.Contato,
            CPF = resultado.Output.CPF,
            Cargo = resultado.Output.Cargo,
            ValorHora = resultado.Output.ValorHora
        };

        return Ok(response);
    }

    [HttpGet("nome")]
    public async Task<IActionResult> ObterPorNome([FromQuery] string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return BadRequest(new { message = "Nome é obrigatório." });

        var resultado = await _funcionarioService.ObterPorNomeAsync(nome);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        var response = new FuncionarioResponse
        {
            Id = resultado.Output.Id,
            Nome = resultado.Output.Nome,
            Contato = resultado.Output.Contato,
            CPF = resultado.Output.CPF,
            Cargo = resultado.Output.Cargo,
            ValorHora = resultado.Output.ValorHora
        };

        return Ok(response);
    }

    [HttpPut]
    public async Task<IActionResult> Atualizar([FromQuery] string cpf, [FromBody] AtualizarFuncionarioBodyRequest bodyRequest)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return BadRequest(new { message = "CPF é obrigatório." });

        var atualizarFuncionarioDto = new AtualizarFuncionarioDto(
            Nome: bodyRequest.Nome,
            Contato: bodyRequest.Contato,
            CPF: cpf,
            Cargo: bodyRequest.Cargo,
            ValorHora: bodyRequest.ValorHora
        );

        var resultado = await _funcionarioService.AtualizarAsync(cpf, atualizarFuncionarioDto);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }

    [HttpDelete]
    public async Task<IActionResult> Deletar([FromQuery] string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return BadRequest(new { message = "CPF é obrigatório." });

        var resultado = await _funcionarioService.DeletarAsync(cpf);

        if (!resultado.IsValid)
            return NotFound(new { message = resultado.Message });

        return Ok(new { message = resultado.Message });
    }
}
