using AutoMapper;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Handlers;

public class ObterFaturaQueryHandler(
IMapper mapper, IRepositorioFatura repositorioFatura,
ILogger<ObterFaturaQueryHandler> logger
) : IRequestHandler<ObterFaturaQuery, Result<ObterFaturaResult>>
{
    public async Task<Result<ObterFaturaResult>> Handle(ObterFaturaQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var fatura = await repositorioFatura.SelecionarRegistroPorIdAsync(query.id);

            if (fatura is null)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro(query.id));

            var result = mapper.Map<ObterFaturaResult>(fatura);

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Ocorreu um erro durante a seleção de {@Registro}.",
                query
            );
            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
