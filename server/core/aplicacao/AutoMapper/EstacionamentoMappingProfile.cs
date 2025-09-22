using System.Collections.Immutable;
using AutoMapper;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.AutoMapper;
public class EstacionamentoMappingProfile : Profile
{
    public EstacionamentoMappingProfile()
    {
        // [1] Commands/Results de [apoio]

        CreateMap<Veiculo, VisualizarVeiculoDto>()
             .ConstructUsing(veiculo => new VisualizarVeiculoDto(
              veiculo.Placa, veiculo.Modelo, veiculo.Cor, veiculo.Observacoes));

        CreateMap<Vaga, VagaDto>()
              .ConstructUsing((src, ctx) => new VagaDto(
                src.Id,
                src.NumeroVaga,
                src.Zona,
                src.EstaOcupada,
                ctx.Mapper.Map<VisualizarVeiculoDto>(src.VeiculoEstacionado)
              ));

        // [2] Commands/Results de [Cadastrar]

        CreateMap<(int quantidade, char zona), CadastrarVagasResult>()
            .ConvertUsing(src => new CadastrarVagasResult(src.quantidade, src.zona));

        // [3] Commands/Results de [Selecionar Uma Vaga]

        CreateMap<Vaga, SelecionarVagaResult>()
            .ConstructUsing((src, ctx) => new SelecionarVagaResult(
               ctx.Mapper.Map<VagaDto>(src)));

        // [4] Commands/Results de [Selecionar Todas As Vagas]

        CreateMap<IEnumerable<Vaga>, SelecionarVagasResult>()
            .ConstructUsing((src, ctx) =>
            new SelecionarVagasResult(
                src?.Select(v => ctx.Mapper.Map<VagaDto>(v))
                .ToImmutableList() ?? ImmutableList<VagaDto>.Empty
            ));

        // [5] Commands/Results de [Adicionar Veículo a Vaga]

        CreateMap<Vaga, AdicionarVeiculoAVagaResult>()
            .ConstructUsing((src, ctx) => new AdicionarVeiculoAVagaResult(
               ctx.Mapper.Map<VagaDto>(src)));

        // [6] Commands/Results de [Remover Veículo da Vaga]

        CreateMap<Vaga, RemoverVeiculoDaVagaResult>()
            .ConstructUsing((src, ctx) => new RemoverVeiculoDaVagaResult(
               ctx.Mapper.Map<VagaDto>(src)));

        // [7] Commands/Results de [Selecionar Veículos Estacionados]
        CreateMap<IEnumerable<Veiculo>, SelecionarVeiculosEstacionadosResult>()
            .ConstructUsing((src, ctx) =>
            new SelecionarVeiculosEstacionadosResult(
                src?.Select(v => ctx.Mapper.Map<VisualizarVeiculoDto>(v))
                .ToImmutableList() ?? ImmutableList<VisualizarVeiculoDto>.Empty
            ));
    }
}
