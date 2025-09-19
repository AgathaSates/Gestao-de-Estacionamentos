using System.Collections.Immutable;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;

namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloFaturamento;

public record ObterFaturasRequest(int? quantidade);

public record ObterFaturasResponse(
    int quatidade,
    ImmutableList<FaturasDto> faturas);