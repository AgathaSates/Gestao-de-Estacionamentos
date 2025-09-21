using System.Collections.Immutable;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
public record SelecionarVeiculosEstacionadosQuery(string? placa) : IRequest<Result<SelecionarVeiculosEstacionadosResult>>;

public record SelecionarVeiculosEstacionadosResult(ImmutableList<VisualizarVeiculoDto> veiculoDtos);