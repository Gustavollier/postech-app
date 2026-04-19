using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Applicacao.Interface.Services;
using PosTechChallenge.Dtos.Requests.Autenticacao;
using PosTechChallenge.Dtos.Responses.Autenticacao;
using System.ComponentModel.DataAnnotations;

namespace PosTechChallenge.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;

    public AutenticacaoController(IAutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService ?? throw new ArgumentNullException(nameof(autenticacaoService));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody][Required] LoginBodyRequest bodyRequest)
    {
        if (string.IsNullOrWhiteSpace(bodyRequest.CPF) || string.IsNullOrWhiteSpace(bodyRequest.Senha))
            return BadRequest(new { message = "CPF e Senha são obrigatórios." });

        var resultado = await _autenticacaoService.LoginAsync(bodyRequest.CPF, bodyRequest.Senha);

        if (!resultado.IsValid)
            return Unauthorized(new { message = resultado.Message });

        var response = new TokenResponse(
            AccessToken: resultado.Output!.AccessToken,
            RefreshToken: resultado.Output.RefreshToken,
            ExpiresIn: resultado.Output.ExpiresIn,
            FuncionarioId: resultado.Output.FuncionarioId,
            Cargo: resultado.Output.Cargo);

        return Ok(response);
    }
}