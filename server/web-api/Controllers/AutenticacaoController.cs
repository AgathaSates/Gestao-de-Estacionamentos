using AutoMapper;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloAutenticacao;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gestao_de_Estacionamentos.WebApi.Controllers;

[ApiController]
[Route("auth")]
public class AutenticacaoController(IMediator mediator, IMapper mapper) : Controller
{
    [HttpPost("registrar")]
    [SwaggerOperation(
        Summary = "Registrar usuário",
        Description = "Registra um novo usuário no sistema.",
        Tags = new[] { "Autenticação" }
    )]
    public async Task<ActionResult<AccessToken>> Registrar(RegistrarUsuarioRequest request)
    {
        var command = mapper.Map<RegistrarUsuarioCommand>(request);

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            if (result.HasError(e => e.HasMetadata("TipoErro", m => m.Equals("RequisicaoInvalida"))))
            {
                var errosDeValidacao = result.Errors
                    .SelectMany(e => e.Reasons.OfType<IError>())
                    .Select(e => e.Message);

                return BadRequest(errosDeValidacao);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok(result.Value);
    }

    [HttpPost("autenticar")]
    [SwaggerOperation(
        Summary = "Autenticar usuário",
        Description = "Autentica um usuário existente no sistema.",
        Tags = new[] { "Autenticação" }
    )]
    public async Task<ActionResult<AccessToken>> Autenticar(AutenticarUsuarioRequest request)
    {
        var command = mapper.Map<AutenticarUsuarioCommand>(request);

        var result = await mediator.Send(command);

        if (result.IsFailed)
        {
            if (result.HasError(e => e.HasMetadata("TipoErro", m => m.Equals("RequisicaoInvalida"))))
            {
                var errosDeValidacao = result.Errors
                    .SelectMany(e => e.Reasons.OfType<IError>())
                    .Select(e => e.Message);

                return BadRequest(errosDeValidacao);
            }

            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok(result.Value);
    }

    [HttpPost("sair")]
    [SwaggerOperation(
        Summary = "Sair",
        Description = "Encerra a sessão do usuário autenticado.",
        Tags = new[] { "Autenticação" }
    )]
    [Authorize]
    public async Task<IActionResult> Sair()
    {
        var result = await mediator.Send(new SairCommand());

        if (result.IsFailed)
            return StatusCode(StatusCodes.Status500InternalServerError);

        return NoContent();
    }
}