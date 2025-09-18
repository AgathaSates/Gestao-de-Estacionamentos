using FluentResults;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
public record CadastrarCheckInCommand(
   CadastrarVeiculoDto veiculo,
    string CPF,
    string Nome) : IRequest<Result<CadastrarCheckInResult>>;

public record CadastrarCheckInResult(Guid Id, NumeroDoTicket NumeroTicket);

public record CadastrarVeiculoDto(string placa, string modelo, string cor, string? observacoes);

public record NumeroDoTicket(int numero);