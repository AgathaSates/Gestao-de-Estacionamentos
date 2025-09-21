using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloEstacionamento.Veiculos;

public record RemoverVeiculoDaVagaRequest(string? placaVeiculo, int? numeroTicket);

public record RemoverVeiculoDaVagaResponse(VagaDto vagaDto);