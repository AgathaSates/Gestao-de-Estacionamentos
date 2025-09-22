using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;

namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloRecepcao;

public record AdicionarObservacaoRequest(string? observacao);

public record AdicionarObservacaoResponse(VisualizarVeiculoDto VeiculoDto);
