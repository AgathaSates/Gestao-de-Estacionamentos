using AutoMapper;
using FluentResults;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Handlers.Vagas;
public class SelecionarVagaQueryHandler(
    IMapper mapper, IRepositorioEstacionamento repositorioEstacionamento,
    ILogger<SelecionarVagaQueryHandler> logger, IValidator<SelecionarVagaQuery> validator
) : IRequestHandler<SelecionarVagaQuery, Result<SelecionarVagaResult>>
{
    public async Task<Result<SelecionarVagaResult>> Handle(SelecionarVagaQuery query, CancellationToken cancellationToken)
    {
        var resultadoValidacao = await validator.ValidateAsync(query, cancellationToken);

        if (!resultadoValidacao.IsValid)
        {
            var erros = resultadoValidacao.Errors.Select(e => e.ErrorMessage).ToList();
            var erroFormatado = ResultadosErro.RequisicaoInvalidaErro(erros);
            return Result.Fail(erroFormatado);
        }

        try 
        {
            Vaga? vaga = null; 

            if (query.Id.HasValue)
            {
                vaga = await repositorioEstacionamento.SelecionarRegistroPorIdAsync(query.Id.Value);
            }
            else if (query.NumeroVaga.HasValue)
            {
                vaga = await repositorioEstacionamento.SelecionarPorNumeroDaVaga(query.NumeroVaga.Value);
            }
            else if (!string.IsNullOrWhiteSpace(query.placaVeiculo))
            {
                vaga = await repositorioEstacionamento.SelecionarPorPlacaDoVeiculo(query.placaVeiculo!);
            }

            if (vaga is null)
                return Result.Fail(ResultadosErro.RegistroNaoEncontradoErro());

            var result = mapper.Map<SelecionarVagaResult>(vaga);

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