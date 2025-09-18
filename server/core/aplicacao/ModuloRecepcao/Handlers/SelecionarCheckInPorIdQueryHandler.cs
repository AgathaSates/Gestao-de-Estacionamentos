using AutoMapper;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Handlers;
public class SelecionarCheckInPorIdQueryHandler(
    IMapper mapper, IRepositorioRecepcao repositorioRecepcao,
    ILogger<SelecionarCheckInPorIdQueryHandler> logger
) : IRequestHandler<SelecionarCheckInPorIdQuery, Result<SelecionarCheckInPorIdResult>>
{
    public async Task<Result<SelecionarCheckInPorIdResult>> Handle(SelecionarCheckInPorIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var checkIn = await repositorioRecepcao.SelecionarRegistroPorIdAsync(query.Id);
           
            if (checkIn is null)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro(query.Id));

            var result = mapper.Map<SelecionarCheckInPorIdResult>(checkIn);

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