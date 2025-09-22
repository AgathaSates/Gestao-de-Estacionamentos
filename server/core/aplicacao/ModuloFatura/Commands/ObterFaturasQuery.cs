using System.Collections.Immutable;
using FluentResults;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;
public record ObterFaturasQuery(int? quantidade) : IRequest<Result<ObterFaturasResult>>;

public record ObterFaturasResult(ImmutableList<FaturasDto> faturas);

public record FaturasDto(
    Guid Id,
    Guid TicketId,
    string PlacaVeiculo,
    DateTime DataEntrada,
    DateTime DataSaida,
    decimal Valortotal
    );
