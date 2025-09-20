using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.FluentValidation.ModuloFaturamento;
public class CalcularValorFaturaCommandValidator : AbstractValidator<CalcularValorFaturaCommand>
{
    public CalcularValorFaturaCommandValidator()
    {
        RuleFor(c => c.dataInicio)
            .NotEmpty().WithMessage("A data de início é obrigatória.")
            .LessThanOrEqualTo(c => c.dataFim)
            .WithMessage("A data de início deve ser anterior ou igual à data de fim.");

        RuleFor(c => c.dataFim)
            .NotEmpty().WithMessage("A data de fim é obrigatória.")
            .GreaterThanOrEqualTo(c => c.dataInicio)
            .WithMessage("A data de fim deve ser posterior ou igual à data de início.");
    }
}