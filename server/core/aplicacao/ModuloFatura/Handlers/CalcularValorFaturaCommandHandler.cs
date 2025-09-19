using AutoMapper;
using FluentResults;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Handlers;

public class CalcularValorFaturaCommandHandler(
    IMapper mapper, IRepositorioFatura repositorioFatura,
    IRepositorioConfiguracao repositorioConfiguracao, IValidator<CalcularValorFaturaCommand> validator,
    ILogger<CalcularValorFaturaCommandHandler> logger
) : IRequestHandler<CalcularValorFaturaCommand, Result<CalcularValorFaturaResult>>
{
    public async Task<Result<CalcularValorFaturaResult>> Handle(CalcularValorFaturaCommand command, CancellationToken cancellationToken)
    {
        // [1] Validação de dados
        var resultadoValidacao = await validator.ValidateAsync(command, cancellationToken);

        if (!resultadoValidacao.IsValid)
        {
            var erros = resultadoValidacao.Errors.Select(e => e.ErrorMessage).ToList();
            var erroFormatado = ResultadosErro.RequisicaoInvalidaErro(erros);
            return Result.Fail(erroFormatado);
        }

        // [2] Cálculo do valor
        try
        {
            var valorDiaria = repositorioConfiguracao.ObterValorDiaria();
            var numeroDiarias = (command.dataFim - command.dataInicio).Days + 1;

            var valor = repositorioFatura.CalcularValorFatura(numeroDiarias, valorDiaria);

            var result = mapper.Map<CalcularValorFaturaResult>(valor);

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção de {@Registro}.",
                command
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}