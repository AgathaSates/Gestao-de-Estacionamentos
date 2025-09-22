using AutoMapper;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloEstacionamento.Vagas;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloEstacionamento.Veiculos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gestao_de_Estacionamentos.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/estacionamento")]
public class EstacionamentoController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpPost("cadastrar")]
    [SwaggerOperation(
        Summary = "Cadastrar vagas",
        Description = "Cadastra novas vagas de estacionamento.",
        Tags = new[] { "Estacionamento Vagas" }
    )]
    public async Task<ActionResult<CadastrarVagasResponse>> Cadastrar(CadastrarVagasRequest request)
    {
        var command = mapper.Map<CadastrarVagasCommand>(request);

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

        var respose = mapper.Map<CadastrarVagasResponse>(result.Value);

        return Created(string.Empty, respose);
    }

    [HttpGet("Selecionar-vaga")]
    [SwaggerOperation(
        Summary = "Selecionar vaga",
        Description = "Seleciona uma vaga de estacionamento disponível.",
        Tags = new[] { "Estacionamento Vagas" }
    )]
    public async Task<ActionResult<SelecionarVagaResponse>> SelecionarVaga([FromQuery] SelecionarVagaRequest request)
    {
        var command = mapper.Map<SelecionarVagaQuery>(request);

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

        var response = mapper.Map<SelecionarVagaResponse>(result.Value);

        return Ok(response);
    }

    [HttpGet("Selecionar-vagas")]
    [SwaggerOperation(
        Summary = "Selecionar vagas",
        Description = "Seleciona todas as vagas de estacionamento.",
        Tags = new[] { "Estacionamento Vagas" }
    )]
    public async Task<ActionResult<List<SelecionarVagasResponse>>> SelecionarVagas([FromQuery] SelecionarVagasRequest? request, CancellationToken cancellationToken)
    {
        var query = mapper.Map<SelecionarVagasQuery>(request);

        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed) return NotFound();

        var response = mapper.Map<SelecionarVagasResponse>(result.Value);

        return Ok(response);
    }

    [HttpPost("adicionar-veiculo")]
    [SwaggerOperation(
        Summary = "Adicionar veículo a vaga",
        Description = "Adiciona um veículo a uma vaga de estacionamento.",
        Tags = new[] { "Estacionamento Veiculos" }
    )]
    public async Task<ActionResult<AdicionarVeiculoAVagaResponse>> AdicionarVeiculo([FromBody]AdicionarVeiculoAVagaRequest request)
    {
        var command = mapper.Map<AdicionarVeiculoAVagaCommand>(request);

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

        var response = mapper.Map<AdicionarVeiculoAVagaResponse>(result.Value);

        return Created(string.Empty, response);
    }

    [HttpPost("remover-veiculo")]
    [SwaggerOperation(
        Summary = "Remover veículo de vaga e fazer check-out",
        Description = "Remove um veículo de uma vaga de estacionamento.",
        Tags = new[] { "Estacionamento Veiculos" }
    )]
    public async Task<ActionResult<RemoverVeiculoDaVagaResponse>> RemoverVeiculo([FromBody]RemoverVeiculoDaVagaRequest request)
    {
        var command = mapper.Map<RemoverVeiculoDaVagaCommand>(request);

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

        var response = mapper.Map<RemoverVeiculoDaVagaResponse>(result.Value);

        return Ok(response);
    }

    [HttpGet("selecionar-veiculos-estacionados")]
    [SwaggerOperation(
        Summary = "Selecionar veículos estacionados",
        Description = "Seleciona todos os veículos atualmente estacionados ou um veiculo estacionado por placa.",
        Tags = new[] { "Estacionamento Veiculos" }
    )]
    public async Task<ActionResult<SelecionarVeiculosEstacionadosResponse>> SelecionarVeiculosEstacionados([FromQuery] SelecionarVeiculosEstacionadosRequest? request, CancellationToken cancellationToken)
    {
        var query = mapper.Map<SelecionarVeiculosEstacionadosQuery>(request);

        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailed)
            return NotFound();

        var response = mapper.Map<SelecionarVeiculosEstacionadosResponse>(result.Value);

        return Ok(response);
    }
}