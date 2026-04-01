using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Applicacao.Dto.Funcionario;
using PosTechChallenge.Applicacao.Interface.Services;
using PosTechChallenge.Dtos.Requests.Funcionario;
using PosTechChallenge.Dtos.Responses.Funcionario;

namespace PosTechChallenge.Controllers;

[ApiController]
[Route("api/[version]/[controller]")]
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
        try
        {
            var criarFuncionarioDto = new CriarFuncionarioDto(
                Nome: bodyRequest.Nome,
                Contato: bodyRequest.Contato,
                CPF: bodyRequest.CPF,
                Cargo: bodyRequest.Cargo,
                ValorHora: bodyRequest.ValorHora
            );

            await _funcionarioService.CriarAsync(criarFuncionarioDto);

            return Created();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodos()
    {
        try
        {
            var funcionarios = await _funcionarioService.ObterTodosAsync();
            
            if (funcionarios == null || !funcionarios.Any())
                return NotFound("Nenhum funcionário encontrado.");

            var responses = funcionarios.Select(f => new FuncionarioResponse
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
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("cpf")]
    public async Task<IActionResult> ObterPorCpf([FromQuery] string cpf)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return BadRequest("CPF é obrigatório.");

            var funcionario = await _funcionarioService.ObterPorCpfAsync(cpf);

            if (funcionario == null)
                return NotFound($"Funcionário com CPF {cpf} não encontrado.");

            var response = new FuncionarioResponse
            {
                Id = funcionario.Id,
                Nome = funcionario.Nome,
                Contato = funcionario.Contato,
                CPF = funcionario.CPF,
                Cargo = funcionario.Cargo,
                ValorHora = funcionario.ValorHora
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("nome")]
    public async Task<IActionResult> ObterPorNome([FromQuery] string nome)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(nome))
                return BadRequest("Nome é obrigatório.");

            var funcionario = await _funcionarioService.ObterPorNomeAsync(nome);

            if (funcionario == null)
                return NotFound($"Funcionário com nome '{nome}' não encontrado.");

            var response = new FuncionarioResponse
            {
                Id = funcionario.Id,
                Nome = funcionario.Nome,
                Contato = funcionario.Contato,
                CPF = funcionario.CPF,
                Cargo = funcionario.Cargo,
                ValorHora = funcionario.ValorHora
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<IActionResult> Atualizar([FromQuery] string cpf, [FromBody] AtualizarFuncionarioBodyRequest bodyRequest)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return BadRequest("CPF é obrigatório.");

            var atualizarFuncionarioDto = new AtualizarFuncionarioDto(
                Nome: bodyRequest.Nome,
                Contato: bodyRequest.Contato,
                CPF: cpf,
                Cargo: bodyRequest.Cargo,
                ValorHora: bodyRequest.ValorHora
            );

            var resultado = await _funcionarioService.AtualizarAsync(cpf, atualizarFuncionarioDto);

            if (!resultado)
                return NotFound($"Funcionário com CPF {cpf} não encontrado.");

            return Ok(new { message = "Funcionário atualizado com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> Deletar([FromQuery] string cpf)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return BadRequest("CPF é obrigatório.");

            var resultado = await _funcionarioService.DeletarAsync(cpf);

            if (!resultado)
                return NotFound($"Funcionário com CPF {cpf} não encontrado.");

            return Ok(new { message = "Funcionário deletado com sucesso." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
