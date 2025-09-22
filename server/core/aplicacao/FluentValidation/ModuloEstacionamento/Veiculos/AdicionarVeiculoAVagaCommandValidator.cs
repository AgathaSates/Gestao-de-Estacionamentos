using System.Text.RegularExpressions;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.FluentValidation.ModuloEstacionamento.Veiculos;
public class AdicionarVeiculoAVagaCommandValidator : AbstractValidator<AdicionarVeiculoAVagaCommand>
{
    public AdicionarVeiculoAVagaCommandValidator()
    {
        RuleFor(c => c)
            .Must(TemUmValorParaVaga)
            .WithMessage("Informe exatamente um dos critério: vagaId ou NumeroVaga.");

        RuleFor(c => c)
            .Must(TemUmValorParaVeiculo)
            .WithMessage("Informe exatamente um dos critérios: placaVeiculo e numeroTicket.");

        When(c => c.vagaId.HasValue, () =>
        {
            RuleFor(c => c.vagaId!.Value)
                .NotEqual(Guid.Empty)
                .WithMessage("vagaId inválido (Guid.Empty não é permitido).");
        });

        When(c => c.numeroVaga.HasValue, () =>
        {
            RuleFor(c => c.numeroVaga!.Value).GreaterThan(0)
                .WithMessage("NumeroVaga deve ser maior que zero.");
        });

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
    private static bool TemUmValorParaVaga(AdicionarVeiculoAVagaCommand c)
    {
        int informados =
            (c.vagaId.HasValue ? 1 : 0) +
            (c.numeroVaga.HasValue ? 1 : 0);

        return informados == 1;
    }

    private static bool TemUmValorParaVeiculo(AdicionarVeiculoAVagaCommand c)
    {
        int informados =
           (!string.IsNullOrWhiteSpace(c.placaVeiculo) ? 1 : 0) +
           (c.numeroTicket.HasValue ? 1 : 0);

        return informados == 1;
    }
}
