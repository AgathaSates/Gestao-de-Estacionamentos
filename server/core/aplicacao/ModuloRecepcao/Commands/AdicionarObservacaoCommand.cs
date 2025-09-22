
using FluentResults;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
public record AdicionarObservacaoCommand(Guid id, string? observacao) : IRequest<Result<AdicionarObservacaoResult>>;

public record AdicionarObservacaoResult(VisualizarVeiculoDto VeiculoDto);
