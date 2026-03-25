using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Applicacao.Dto.Funcionario;
using PosTechChallenge.Applicacao.Interface.Services;
using PosTechChallenge.Dtos.Requests.Funcionario;

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
    public async Task<IActionResult> Criar(CriarFuncionarioBodyRequest bodyRequest)
    {
        try
        {
            CriarFuncionarioDto criarFuncionarioDto = new(
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
            return BadRequest(ex.Message);
        }
    }
}
