using AutoMapper;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloFaturamento;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Gestao_de_Estacionamentos.WebApi.Controllers;

[ApiController]
[Route("api/faturamento")]
public class FaturamentoController(IMediator mediator, IMapper mapper) : ControllerBase
{
    [HttpGet("calcular")]
    [SwaggerOperation(
       Summary = "Calcular o valor de uma fatura",
       Description = "Calcula automaticamente o preço que ficará" +
        " sua fatura com base no tempo de permanencia informado.",
        Tags = new[] { "Faturamento" }
    )]
    public async Task<ActionResult<CalcularValorFaturaResponse>> CalcularValorFatura(
        [FromQuery] CalcularValorFaturaRequest request)
    {
        var command = mapper.Map<CalcularValorFaturaCommand>(request);

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

        var response = mapper.Map<CalcularValorFaturaResponse>(result.Value);

        return Ok(response);
    }

    [HttpPost("gerar-relatorio")]
    [SwaggerOperation(
        Summary = "Gerar Relatório de Faturamento",
        Description = "Gera um relatório detalhado de faturamento para um período específico.",
        Tags = new[] { "Relatórios" }
    )]
    public async Task<ActionResult<GerarRelatorioResponse>> GerarRelatorio(GerarRelatorioRequest request)
    {
        var command = mapper.Map<GerarRelatorioCommand>(request);

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
        var response = mapper.Map<GerarRelatorioResponse>(result.Value);

        return Created(string.Empty, response);
    }

    [HttpGet("{id:guid}")]
    [SwaggerOperation(
        Summary = "Obter Fatura por ID",
        Description = "Recupera os detalhes de uma fatura específica usando seu ID.",
        Tags = new[] { "Faturamento" }
    )]
    public async Task<ActionResult<ObterFaturaResponse>> ObterFaturaPorId(Guid id)
    {
        var query = mapper.Map<ObterFaturaQuery>(id);

        var result = await mediator.Send(query);

        if (result.IsFailed)
            return NotFound(id);

        var response = mapper.Map<ObterFaturaResponse>(result.Value);

        return Ok(response);
    }

    [HttpGet("listar")]
    [SwaggerOperation(
       Summary = "Listar Faturas",
       Description = "Recupera uma lista de todas as faturas.",
       Tags = new[] { "Faturamento" }
   )]
    public async Task<ActionResult<List<ObterFaturaResponse>>> ListarFaturas(
       [FromQuery] ObterFaturasRequest? request,
       CancellationToken cancellationToken)
    {
        var query = mapper.Map<ObterFaturasQuery>(request);

        var resultado = await mediator.Send(query, cancellationToken);

        if (resultado.IsFailed) return NotFound();

        var response = mapper.Map<ObterFaturasResponse>(resultado.Value);

        return Ok(response);
    }
}