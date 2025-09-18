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
            CreateMap<Fatura, FaturaDto>()
                 .ConstructUsing(f => new FaturaDto(
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

            CreateMap<decimal, CalcularValorFaturaResult>()
                .ConstructUsing(v => new CalcularValorFaturaResult(v));

            CreateMap<Fatura, ObterFaturaResult>()
                .ConstructUsing((src, ctx) => 
                new ObterFaturaResult(ctx.Mapper.Map<FaturaDto>(src)));

            CreateMap<G>

        }
    }
}