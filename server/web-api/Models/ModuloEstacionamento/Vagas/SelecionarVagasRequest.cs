using System.Collections.Immutable;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;

namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloEstacionamento.Vagas;

public record SelecionarVagasRequest(int? quantidade);

public record SelecionarVagasResponse(int quantidade, ImmutableList<VagaDto> vagas);
