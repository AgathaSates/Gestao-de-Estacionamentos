using FluentResults;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
public record EditarCheckInCommand(
    Guid Id,
    EditarVeiculoDto Veiculo,
    string CPF,
    string Nome) : IRequest<Result<EditarCheckInResult>>;

public record EditarCheckInResult(
   EditarVeiculoDto veiculo,
    string CPF,
    string Nome,
    NumeroDoTicket NumeroTicket
    );

public record EditarVeiculoDto(string placa, string modelo, string cor, string? observacoes);