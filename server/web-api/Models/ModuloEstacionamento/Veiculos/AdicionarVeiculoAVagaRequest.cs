using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloEstacionamento.Veiculos;

public record AdicionarVeiculoAVagaRequest(Guid? vagaId, int? NumeroVaga,
    string? placaVeiculo, int? numeroTicket);

public record AdicionarVeiculoAVagaResponse(VagaDto vagaDto);
