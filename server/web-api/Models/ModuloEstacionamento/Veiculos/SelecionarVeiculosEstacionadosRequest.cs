using System.Collections.Immutable;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloEstacionamento.Veiculos;

public record SelecionarVeiculosEstacionadosRequest(string? Placa);

public record SelecionarVeiculosEstacionadosResponse(int quantidade,
   ImmutableList<VisualizarVeiculoDto> VeiculosEstacionados);