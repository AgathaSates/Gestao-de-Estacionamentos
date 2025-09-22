using FluentResults;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;
public record CadastrarVagasCommand(int quantidadeParaGerar, char zona) : IRequest<Result<CadastrarVagasResult>>;

public record CadastrarVagasResult(int quantidadeParaGerar, char zona);
