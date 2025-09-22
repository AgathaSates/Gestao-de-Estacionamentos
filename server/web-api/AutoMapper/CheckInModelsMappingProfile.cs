using System.Collections.Immutable;
using AutoMapper;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloFaturamento;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloRecepcao;

namespace Gestao_de_Estacionamentos.WebApi.AutoMapper;

public class CheckInModelsMappingProfile : Profile
{
    public CheckInModelsMappingProfile()
    {
        // [1] Requests/Response de [Cadastro]

        CreateMap<CadastrarCheckInRequest, CadastrarCheckInCommand>();
        CreateMap<CadastrarCheckInResult, CadastrarCheckInResponse>();

        // [2] Requests/Response de [Edição]

        CreateMap<(Guid, EditarCheckInRequest), EditarCheckInCommand>()
            .ConvertUsing(src => new EditarCheckInCommand(
                src.Item1,
                src.Item2.veiculo,
                src.Item2.CPF,
                src.Item2.Nome));

        CreateMap<EditarCheckInResult, EditarCheckInResponse>();

        // [3] Requests/Response de [Adicionar observação]

        CreateMap<(Guid, AdicionarObservacaoRequest), AdicionarObservacaoCommand>()
            .ConvertUsing(src => new AdicionarObservacaoCommand(
                src.Item1,
                src.Item2.observacao));

        CreateMap<AdicionarObservacaoResult, AdicionarObservacaoResponse>();


        // [4] Requests/Response de [Seleção por id]

        CreateMap<SelecionarCheckInPorIdRequest, SelecionarCheckInPorIdQuery>();
        CreateMap<SelecionarCheckInPorIdResult, SelecionarCheckInPorIdResponse>()
            .ConvertUsing(src => new SelecionarCheckInPorIdResponse(
                src.Id,
                src.veiculo,
                src.CPF,
                src.Nome,
                src.NumeroTicket));

        // [5] Requests/Response de [Selecionar todos]

        CreateMap<SelecionarCheckInsRequest, SelecionarCheckInsQuery>();
        CreateMap<SelecionarCheckInsResult, SelecionarCheckInsResponse>()
            .ConvertUsing((src, dest, ctx) => new SelecionarCheckInsResponse(
                src.checkIns.Count,
                src?.checkIns?.Select(ci => ctx.Mapper.Map<SelecionarCheckInsDto>(ci))
                .ToImmutableList() ?? ImmutableList<SelecionarCheckInsDto>.Empty
            ));
    }
}
