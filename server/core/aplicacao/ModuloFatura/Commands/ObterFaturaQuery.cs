using FluentResults;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands
{
    public record ObterFaturaQuery(Guid id) : IRequest<Result<ObterFaturaResult>>;

    public record ObterFaturaResult(FaturaDto faturaDto);
    public record FaturaDto(Guid ticket, 
        string placaVeiculo,
        DateTime dataEntrada, 
        DateTime dataSaida,
        decimal valorTotal);

}