using System.Collections.Immutable;
using AutoMapper;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloRecepcao;

namespace Gestao_de_Estacionamentos.WebApi.AutoMapper;

public class CheckInModelsMappingProfile : Profile
{
    public CheckInModelsMappingProfile()
    {
        // [1] Requests/Response de Cadastro
        CreateMap<CadastrarCheckInRequest, CadastrarCheckInCommand>();
        CreateMap<CadastrarCheckInResult, CadastrarCheckInResponse>();

        // [2] Requests/Response de Edição
        CreateMap<(Guid, EditarCheckInRequest), EditarCheckInCommand>()
            .ConvertUsing(src => new EditarCheckInCommand(
                src.Item1, 
                src.Item2.veiculo,
                src.Item2.CPF,
                src.Item2.Nome));

        CreateMap<EditarCheckInResult, EditarCheckInResponse>()
            .ConvertUsing( src => new EditarCheckInResponse(
                src.veiculo,
                src.CPF,
                src.Nome,
                src.NumeroTicket));

        // [3] Results de Seleção por id
        CreateMap<Guid, SelecionarCheckInPorIdQuery>()
            .ConvertUsing(id => new SelecionarCheckInPorIdQuery(id));

        CreateMap<SelecionarCheckInPorIdResult, SelecionarCheckInPorIdResponse>()
            .ConvertUsing(src => new SelecionarCheckInPorIdResponse(
                src.Id,
                src.veiculo,
                src.CPF,
                src.Nome,
                src.NumeroTicket));

        // [4] Results de Seleção de todos

        CreateMap<SelecionarChecInsRequest, SelecionarCheckInsQuery>();

        CreateMap<SelecionarCheckInsResult, SelecionarChecInsResponse>()
            .ConvertUsing((src, dest, ctx) => new SelecionarChecInsResponse(
                src.checkIns.Count,
                src?.checkIns?.Select(ci => ctx.Mapper.Map<SelecionarCheckInsDto>(ci))
                .ToImmutableList() ?? ImmutableList<SelecionarCheckInsDto>.Empty
                ));
    }
}