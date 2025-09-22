using System.Collections.Immutable;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;

namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloRecepcao;

public record SelecionarCheckInsRequest(int? quantidade);

public record SelecionarCheckInsResponse(
    int quantidade,
    ImmutableList<SelecionarCheckInsDto> checkIns);
