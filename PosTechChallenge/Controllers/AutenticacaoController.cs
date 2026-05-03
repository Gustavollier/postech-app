using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PosTechChallenge.Aplicacao.Interface.Services;
using PosTechChallenge.Dominio.ValueObjects;
using PosTechChallenge.Dtos.Requests.Autenticacao;
using PosTechChallenge.Dtos.Responses.Autenticacao;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PosTechChallenge.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AutenticacaoController : ControllerBase
{
    private readonly IAutenticacaoService _autenticacaoService;

    public AutenticacaoController(IAutenticacaoService autenticacaoService)
    {
        _autenticacaoService = autenticacaoService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody][Required] LoginBodyRequest bodyRequest)
    {
        if (string.IsNullOrWhiteSpace(bodyRequest.CPF) || string.IsNullOrWhiteSpace(bodyRequest.Senha))
            return BadRequest(new { message = "CPF e Senha são obrigatórios." });

        var validacaoCpf = ValidarCpf(bodyRequest.CPF);
        if (validacaoCpf != null)
            return BadRequest(new { message = validacaoCpf });

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

    [Authorize]
    [HttpPatch("alterar-senha")]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaBodyRequest bodyRequest)
    {
        if (string.IsNullOrWhiteSpace(bodyRequest.SenhaAtual) ||
            string.IsNullOrWhiteSpace(bodyRequest.NovaSenha) ||
            string.IsNullOrWhiteSpace(bodyRequest.ConfirmacaoSenha))
            return BadRequest(new { message = "Senha atual, nova senha e confirmação de senha são obrigatórios." });

        var funcionarioIdClaim =
            User.FindFirstValue("FuncionarioId") ??
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (int.TryParse(funcionarioIdClaim, out var funcionarioId) is false)
            return Unauthorized(new { message = "Token inválido." });

        var result = await _autenticacaoService.AlterarSenhaAsync(
            funcionarioId,
            bodyRequest.SenhaAtual,
            bodyRequest.NovaSenha,
            bodyRequest.ConfirmacaoSenha);

        if (result.IsValid)
            return Ok(new { message = result.Message });

        return BadRequest(new { message = result.Message });
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
