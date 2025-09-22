using System.Collections.Immutable;
using AutoMapper;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.AutoMapper;
public class CheckInMappingProfile : Profile
{
    public CheckInMappingProfile()
    {
        // [1] Commands/Results de [apoio]

        CreateMap<CadastrarVeiculoDto, Veiculo>()
            .ConstructUsing(dto => new Veiculo(
                dto.placa, dto.modelo, dto.cor, dto.observacoes));

        CreateMap<EditarVeiculoDto, Veiculo>()
            .ConstructUsing(dto => new Veiculo(
                dto.placa, dto.modelo, dto.cor, dto.observacoes)).ReverseMap();

        CreateMap<Veiculo, VisualizarVeiculoDto>()
            .ConstructUsing(veiculo => new VisualizarVeiculoDto(
                veiculo.Placa, veiculo.Modelo, veiculo.Cor, veiculo.Observacoes));

        CreateMap<Ticket, NumeroDoTicket>()
            .ConstructUsing(ticket => new NumeroDoTicket(ticket.NumeroSequencial));

        // [2] Commands/Results de [Cadastro]

        CreateMap<CadastrarCheckInCommand, CheckIn>()
            .ConstructUsing((src, ctx) =>
            new CheckIn(
                ctx.Mapper.Map<Veiculo>(src.veiculo),
                src.Nome,
                src.CPF));

        CreateMap<CheckIn, CadastrarCheckInResult>()
            .ConstructUsing((src, ctx) =>
                new CadastrarCheckInResult(
                src.Id,
                ctx.Mapper.Map<NumeroDoTicket>(src.Ticket)));

        // [3] Commands/Results de [Edição]

        CreateMap<EditarCheckInCommand, CheckIn>()
            .ConstructUsing((src, ctx) =>
            new CheckIn(
                ctx.Mapper.Map<Veiculo>(src.Veiculo),
                src.Nome,
                src.CPF
            ));

        CreateMap<CheckIn, EditarCheckInResult>()
            .ConstructUsing((src, ctx) =>
                new EditarCheckInResult(
                ctx.Mapper.Map<EditarVeiculoDto>(src.Veiculo),
                src.Nome,
                src.CPF,
                ctx.Mapper.Map<NumeroDoTicket>(src.Ticket)));

        // [4] Commands/Results de [adicionar observação]

        CreateMap<Veiculo, AdicionarObservacaoResult>()
            .ConstructUsing((src, ctx) =>
                new AdicionarObservacaoResult(
                ctx.Mapper.Map<VisualizarVeiculoDto>(src)));

        // [5] Commands/Results de [Seleção Por Id]

        CreateMap<CheckIn, SelecionarCheckInPorIdResult>()
            .ConstructUsing((src, ctx) =>
                new SelecionarCheckInPorIdResult(
                src.Id,
                ctx.Mapper.Map<VisualizarVeiculoDto>(src.Veiculo),
                src.Nome,
                src.CPF,
                ctx.Mapper.Map<NumeroDoTicket>(src.Ticket)));

        // [6] Commands/Results de [Selecionar Todos]

        CreateMap<CheckIn, SelecionarCheckInsDto>()
            .ConstructUsing((src, ctx) =>
                new SelecionarCheckInsDto(
                src.Id,
                ctx.Mapper.Map<VisualizarVeiculoDto>(src.Veiculo),
                src.CPF,
                src.Nome,
                ctx.Mapper.Map<NumeroDoTicket>(src.Ticket)));

        CreateMap<IEnumerable<CheckIn>, SelecionarCheckInsResult>()
            .ConstructUsing((src, ctx) =>
                new SelecionarCheckInsResult(
                    src.Select(checkIn => ctx.Mapper.Map<SelecionarCheckInsDto>(checkIn))
                    .ToImmutableList() ?? ImmutableList<SelecionarCheckInsDto>.Empty));
    }
}
