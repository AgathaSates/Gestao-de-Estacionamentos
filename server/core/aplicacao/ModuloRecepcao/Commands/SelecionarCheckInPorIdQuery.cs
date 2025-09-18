using FluentResults;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
public record SelecionarCheckInPorIdQuery
    (Guid Id) : IRequest<Result<SelecionarCheckInPorIdResult>>;

public record SelecionarCheckInPorIdResult(
    Guid Id,
    VisualizarVeiculoDto veiculo,
    string CPF,
    string Nome,
    NumeroDoTicket NumeroTicket
    );

public record VisualizarVeiculoDto(string placa, string modelo, string cor, string? observacoes);