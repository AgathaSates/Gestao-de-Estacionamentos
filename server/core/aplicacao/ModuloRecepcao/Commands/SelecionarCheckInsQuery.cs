using System.Collections.Immutable;
using FluentResults;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
public record SelecionarCheckInsQuery
    (int? quantidade) : IRequest<Result<SelecionarCheckInsResult>>;

public record SelecionarCheckInsResult(ImmutableList<SelecionarCheckInsDto> checkIns);

public record SelecionarCheckInsDto(
    Guid Id,
    VisualizarVeiculoDto veiculo,
    string CPF,
    string Nome,
    NumeroDoTicket NumeroTicket
    );