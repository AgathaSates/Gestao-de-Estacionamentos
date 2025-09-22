using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
public record AdicionarVeiculoAVagaCommand(Guid? vagaId, int? numeroVaga,
    string? placaVeiculo, int? numeroTicket) : IRequest<Result<AdicionarVeiculoAVagaResult>>;

public record AdicionarVeiculoAVagaResult(VagaDto vagaDto);

public record VagaDto(
    Guid Id,
    int numeroVaga,
    char zona,
    bool estaOcupada,
    VisualizarVeiculoDto? veiculoEstacionado
);
