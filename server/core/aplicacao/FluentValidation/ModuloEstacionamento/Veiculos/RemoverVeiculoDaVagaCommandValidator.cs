using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.FluentValidation.ModuloEstacionamento.Veiculos;
public class RemoverVeiculoDaVagaCommandValidator : AbstractValidator<RemoverVeiculoDaVagaCommand>
{
    public RemoverVeiculoDaVagaCommandValidator()
    {
        RuleFor(c => c)
            .Must(TemUmValorParaVeiculo)
            .WithMessage("Informe exatamente um dos critérios: placaVeiculo e numeroTicket.");

        When(c => !string.IsNullOrWhiteSpace(c.placaVeiculo), () =>
        {
            RuleFor(c => c.placaVeiculo)
                .Matches("^[A-Z0-9]{7}$")
                .WithMessage("placaVeiculo inválida.");
        });

        When(c => c.numeroTicket.HasValue, () =>
        {
            RuleFor(c => c.numeroTicket!.Value).GreaterThan(0)
                .WithMessage("numeroTicket deve ser maior que zero.");
        });
    }
    private static bool TemUmValorParaVeiculo(RemoverVeiculoDaVagaCommand c)
    {
        int informados =
           (!string.IsNullOrWhiteSpace(c.placaVeiculo) ? 1 : 0) +
           (c.numeroTicket.HasValue ? 1 : 0);
        return informados == 1;
    }
}
