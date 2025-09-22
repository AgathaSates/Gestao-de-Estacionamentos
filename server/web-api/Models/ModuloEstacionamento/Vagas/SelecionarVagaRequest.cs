using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloEstacionamento.Vagas;

public record SelecionarVagaRequest(Guid? Id, int? NumeroVaga, string? PlacaVeiculo);

public record SelecionarVagaResponse(VagaDto vagaDto);
