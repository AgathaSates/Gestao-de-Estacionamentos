using FluentResults;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
public record RemoverVeiculoDaVagaCommand(string? placaVeiculo, int? numeroTicket)
    : IRequest<Result<RemoverVeiculoDaVagaResult>>;

public record RemoverVeiculoDaVagaResult(VagaDto vagaDto);
