using System.Collections.Immutable;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;

namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloRecepcao;

public record SelecionarChecInsRequest(int? quantidade);

public record SelecionarChecInsResponse(
    int quantidade,
    ImmutableList<SelecionarCheckInsDto> checkIns);
