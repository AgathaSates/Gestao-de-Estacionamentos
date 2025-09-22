using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;

namespace Gestao_de_Estacionamentos.WebApi.Models.ModuloRecepcao;

public record SelecionarCheckInPorIdRequest(Guid id);

public record SelecionarCheckInPorIdResponse(
    Guid Id,
    VisualizarVeiculoDto veiculo,
    string CPF,
    string Nome,
    NumeroDoTicket NumeroTicket
    );
