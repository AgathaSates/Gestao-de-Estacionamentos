using AutoMapper;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloRecepcao;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gestao_de_Estacionamentos.WebApi.Controllers;

[ApiController]
[Route("api/checkIn")]
public class CheckInController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost]
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
    public async Task<ActionResult<EditarCheckInResponse>> Editar(Guid id, EditarCheckInRequest request)
    {
        var command = mapper.Map<(Guid, EditarCheckInRequest), EditarCheckInCommand>((id, request));

        var result = await mediator.Send(command);

        if (result.IsFailed)
            return BadRequest();

        var respose = mapper.Map<EditarCheckInResponse>(result.Value);

        return Ok(respose);
    }

    [HttpGet]
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