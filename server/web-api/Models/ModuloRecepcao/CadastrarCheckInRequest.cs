using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;

namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloRecepcao;

public record CadastrarCheckInRequest(
   CadastrarVeiculoDto Veiculo,
    string CPF,
    string Nome);

public record CadastrarCheckInResponse(
    Guid Id,
    NumeroDoTicket NumeroTicket);