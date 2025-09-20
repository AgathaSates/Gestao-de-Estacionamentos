using System.Collections.Immutable;
using AutoMapper;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.AutoMapper
{
    public class FaturamentoMappingProfile : Profile
    {
        public FaturamentoMappingProfile()
        {
            // [1] Commands/Results de [apoio]

            CreateMap<Fatura, FaturaDto>()
                 .ConstructUsing(f => new FaturaDto(
                     f.TicketId,
                     f.PlacaVeiculo,
                     f.DataEntrada,
                     f.DataSaida,
                     f.Valortotal
                 ));

            CreateMap<Fatura, FaturasDto>()
                .ConstructUsing(f => new FaturasDto(
                     f.Id,
                     f.TicketId,
                     f.PlacaVeiculo,
                     f.DataEntrada,
                     f.DataSaida,
                     f.Valortotal
                ));

            CreateMap<Relatorio, RelatorioDto>()
                 .ConstructUsing(r => new RelatorioDto(
                     r.DataInicial,
                     r.DataFinal,
                     r.Faturas.Count,
                     r.ValorTotal
                 ));

            // [2] Commands/Results de [Calcular valor fatura]

            CreateMap<decimal, CalcularValorFaturaResult>()
                .ConstructUsing(v => new CalcularValorFaturaResult(v));

            // [3] Commands/Results de [Obter fatura]

            CreateMap<Fatura, ObterFaturaResult>()
                .ConstructUsing((src, ctx) => new ObterFaturaResult(
                    ctx.Mapper.Map<FaturaDto>(src)));

            // [4] commands/results de [Obter faturaS]

            CreateMap<IEnumerable<Fatura>, ObterFaturasResult>()
                  .ConstructUsing((src, ctx) =>
                  new ObterFaturasResult(
                  src?.Select(fatura => ctx.Mapper.Map<FaturasDto>(fatura))
                  .ToImmutableList() ?? ImmutableList<FaturasDto>.Empty));


            // [5] Commands/Results de [Gerar Relatorio]

            CreateMap<Relatorio, GerarRelatorioResult>()
                .ConstructUsing((src, ctx) => new GerarRelatorioResult(
                    ctx.Mapper.Map<RelatorioDto>(src)));
        }
    }
}