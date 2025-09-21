using System.Collections.Immutable;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;
public record SelecionarVagasQuery(int? quantidade) : IRequest<Result<SelecionarVagasResult>>;

public record SelecionarVagasResult(ImmutableList<VagaDto> vagas);