using System.Collections.Immutable;
using AutoMapper;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloFaturamento;

namespace Gestao_de_Estacionamentos.WebApi.AutoMapper;

public class FaturamentoModelsMappingProfile : Profile
{
    public FaturamentoModelsMappingProfile()
    {
        // [1] Requests/Response de [Calcular valor fatura]

        CreateMap<CalcularValorFaturaRequest, CalcularValorFaturaCommand>();
        CreateMap<CalcularValorFaturaResult, CalcularValorFaturaResponse>();

        // [2] Requests/Response de [Obter fatura]

        CreateMap<ObterFaturaRequest, ObterFaturaQuery>();
        CreateMap<ObterFaturaResult, ObterFaturaResponse>();


        // [3] Requests/Response de [Obter faturas]

        CreateMap<ObterFaturasRequest, ObterFaturasQuery>();
        CreateMap<ObterFaturasResult, ObterFaturasResponse>()
           .ConvertUsing((src, dest, ctx) => new ObterFaturasResponse(
                src.faturas.Count,
                src?.faturas.Select(ci => ctx.Mapper.Map<FaturasDto>(ci))
                .ToImmutableList() ?? ImmutableList<FaturasDto>.Empty
           ));

        // [4] Requests/Response de [Gerar Relatorio]

        CreateMap<GerarRelatorioRequest, GerarRelatorioCommand>();
        CreateMap<GerarRelatorioResult, GerarRelatorioResponse>();
    }
}
