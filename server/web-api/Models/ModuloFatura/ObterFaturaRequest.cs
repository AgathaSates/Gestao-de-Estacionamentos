using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;

namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloFaturamento;

public record ObterFaturaRequest(Guid id);

public record ObterFaturaResponse(FaturaDto faturaDto);
