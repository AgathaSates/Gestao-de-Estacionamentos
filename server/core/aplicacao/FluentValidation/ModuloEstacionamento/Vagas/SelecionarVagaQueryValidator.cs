using System.Text.RegularExpressions;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.FluentValidation.ModuloEstacionamento.Vagas;
public class SelecionarVagaQueryValidator : AbstractValidator<SelecionarVagaQuery>
{
    public SelecionarVagaQueryValidator()
    {
        RuleFor(q => q)
            .Must(TemExatamenteUmCriterio)
            .WithMessage("Informe exatamente um critério: Id, NumeroVaga ou PlacaVeiculo.");

        When(q => q.Id.HasValue, () =>
        {
            RuleFor(q => q.Id!.Value)
                .NotEqual(Guid.Empty)
                .WithMessage("Id inválido (Guid.Empty não é permitido).");
        });

        When(q => q.NumeroVaga.HasValue, () =>
        {
            RuleFor(q => q.NumeroVaga!.Value).GreaterThan(0)
                .WithMessage("NumeroVaga deve ser maior que zero.");
        });

        When(q => !string.IsNullOrWhiteSpace(q.placaVeiculo), () =>
        {
            RuleFor(q => q.placaVeiculo!)
                .Must(EPlacaValida)
                .WithMessage("PlacaVeiculo inválida.");
        });
    }

    private static bool TemExatamenteUmCriterio(SelecionarVagaQuery q)
    {
        int informados =
            (q.Id.HasValue ? 1 : 0) +
            (q.NumeroVaga.HasValue ? 1 : 0) +
            (!string.IsNullOrWhiteSpace(q.placaVeiculo) ? 1 : 0);

        return informados == 1;
    }

    private static readonly Regex PlacaRegex =
    new(@"^[A-Z0-9]{7}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static bool EPlacaValida(string? placaVeiculo)
    => placaVeiculo is not null && PlacaRegex.IsMatch(placaVeiculo);
}
