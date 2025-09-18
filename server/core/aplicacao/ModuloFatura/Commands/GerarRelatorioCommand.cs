using FluentResults;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands
{
   public record GerarRelatorioCommand(DateTime dataInicio, DateTime dataFim) : IRequest<Result<GerarRelatorioResult>>;

    public record GerarRelatorioResult(RelatorioDto relatorioDto);

    public record RelatorioDto(DateTime dataInicio, 
        DateTime dataFim, 
        int totalFaturas, 
        decimal valorTotalFaturado);

}