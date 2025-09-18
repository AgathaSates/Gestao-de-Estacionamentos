using AutoMapper;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloRecepcao;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gestao_de_Estacionamentos.WebApi.Controllers;

[ApiController]
[Route("api/checkIn")]
public class CheckInController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [SwaggerOperation(
        Summary = "Cadastrar Check-In",
        Description = "Cria um novo check-in."
    )]
    public async Task<ActionResult<CadastrarCheckInResponse>> Cadastrar(CadastrarCheckInRequest request)
    {
        var command = mapper.Map<CadastrarCheckInCommand>(request);

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

        var respose = mapper.Map<CadastrarCheckInResponse>(result.Value);

        return Created(string.Empty, respose);
    }

    [HttpPut("{id:guid}")]
    [SwaggerOperation(
        Summary = "Editar Check-In",
        Description = "Atualiza os dados de um check-in existente."
    )]
    public async Task<ActionResult<EditarCheckInResponse>> Editar(Guid id, EditarCheckInRequest request)
    {
        var command = mapper.Map<(Guid, EditarCheckInRequest), EditarCheckInCommand>((id, request));

        var result = await mediator.Send(command);

        if (result.IsFailed)
            return BadRequest();

        var respose = mapper.Map<EditarCheckInResponse>(result.Value);

        return Ok(respose);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Selecionar por id",
        Description = "Retorna um check-in pelo id."
    )]
    public async Task<ActionResult<SelecionarCheckInPorIdResponse>> SelecionarCheckInPorId(Guid id)
    {
        var query = mapper.Map<SelecionarCheckInPorIdQuery>(id);

        var result = await mediator.Send(query);

        if (result.IsFailed)
            return NotFound(id);

        var response = mapper.Map<SelecionarCheckInPorIdResponse>(result.Value);

        return Ok(response);
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Selecionar todos",
        Description = "Retorna todos os check-ins (com quantidadee se aplicável)."
    )]
    public async Task<ActionResult<SelecionarChecInsResponse>> SelecionarCheckIns(
        [FromQuery] SelecionarChecInsRequest? request, CancellationToken cancellationToken)
    {
        var query = mapper.Map<SelecionarCheckInsQuery>(request);

        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return BadRequest();

        var response = mapper.Map<SelecionarChecInsResponse>(result.Value);

        return Ok(response);
    }
}