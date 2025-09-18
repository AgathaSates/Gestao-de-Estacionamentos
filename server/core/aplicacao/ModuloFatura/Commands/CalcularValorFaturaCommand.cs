using FluentResults;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands
{
    public class CalcularValorFaturaCommand(DateTime dataInicio, DateTime dataFim) : IRequest<Result<CalcularValorFaturaResult>>;

    public record CalcularValorFaturaResult(decimal valor);

}