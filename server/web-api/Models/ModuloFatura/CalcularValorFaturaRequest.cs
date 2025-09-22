namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloFaturamento;

public record CalcularValorFaturaRequest(DateTime dataInicio, DateTime dataFim);

public record CalcularValorFaturaResponse(decimal valor);
