using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;

namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloRecepcao;

public record EditarCheckInRequest(
   EditarVeiculoDto veiculo,
    string CPF,
    string Nome
);

public record EditarCheckInResponse(
    EditarVeiculoDto veiculo,
    string CPF,
    string Nome,
    NumeroDoTicket NumeroTicket
);
