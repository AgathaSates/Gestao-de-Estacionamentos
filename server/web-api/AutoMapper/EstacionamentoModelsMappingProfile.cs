using System.Collections.Immutable;
using AutoMapper;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloEstacionamento.Vagas;
using Gestao_de_Estacionamentos.WebApi.Models.ModuloEstacionamento.Veiculos;

namespace Gestao_de_Estacionamentos.WebApi.AutoMapper;

public class EstacionamentoModelsMappingProfile : Profile
{
    public EstacionamentoModelsMappingProfile()
    {
        // [1] Requests/Response de [Cadastro]

        CreateMap<CadastrarVagasRequest, CadastrarVagasCommand>();
        CreateMap<CadastrarVagasResult, CadastrarVagasResponse>();

        // [2] Requests/Response de [Selecionar Uma Vaga]

        CreateMap<SelecionarVagaRequest, SelecionarVagaQuery>();
        CreateMap<SelecionarVagaResult, SelecionarVagaResponse>();

        // [3] Requests/Response de [Selecionar Todas As Vagas]

        CreateMap<SelecionarVagasRequest, SelecionarVagasQuery>();
        CreateMap<SelecionarVagasResult, SelecionarVagasResponse>()
            .ConvertUsing((src, dest, ctx) => new SelecionarVagasResponse(
                src.vagas.Count,
                src?.vagas?.Select(v => ctx.Mapper.Map<VagaDto>(v))
                .ToImmutableList() ?? ImmutableList<VagaDto>.Empty
            ));

        // [4] Requests/Response de [Adicionar Veículo a Vaga]

        CreateMap<AdicionarVeiculoAVagaRequest, AdicionarVeiculoAVagaCommand>();
        CreateMap<AdicionarVeiculoAVagaResult, AdicionarVeiculoAVagaResponse>();

        // [5] Requests/Response de [Remover Veículo de Vaga]

        CreateMap<RemoverVeiculoDaVagaRequest, RemoverVeiculoDaVagaCommand>();
        CreateMap<RemoverVeiculoDaVagaResult, RemoverVeiculoDaVagaResponse>();

        // [6] Requests/Response de [Selecionar Veículos Estacionados]

        CreateMap<SelecionarVeiculosEstacionadosRequest, SelecionarVeiculosEstacionadosQuery>();
        CreateMap<SelecionarVeiculosEstacionadosResult, SelecionarVeiculosEstacionadosResponse>()
            .ConvertUsing((src, dest, ctx) => new SelecionarVeiculosEstacionadosResponse(
                src.veiculoDtos.Count,
                src?.veiculoDtos?.Select(v => ctx.Mapper.Map<VisualizarVeiculoDto>(v))
                .ToImmutableList() ?? ImmutableList<VisualizarVeiculoDto>.Empty
            ));
    }
}