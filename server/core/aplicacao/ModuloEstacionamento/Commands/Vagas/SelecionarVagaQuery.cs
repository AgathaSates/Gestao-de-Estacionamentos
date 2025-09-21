using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;
public record SelecionarVagaQuery(Guid? Id, int? NumeroVaga, string? placaVeiculo)
    : IRequest<Result<SelecionarVagaResult>>;

public record SelecionarVagaResult(VagaDto vagaDto);