using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;

namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloFaturamento;

public record GerarRelatorioRequest(DateTime dataInicio, DateTime dataFim);

public record GerarRelatorioResponse(
    RelatorioDto relatorioDto
    );
