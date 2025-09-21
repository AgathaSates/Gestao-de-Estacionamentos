using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.FluentValidation.ModuloEstacionamento.Vagas;
public class CadastrarVagasCommandValidator : AbstractValidator<CadastrarVagasCommand>
{
    public CadastrarVagasCommandValidator()
    {
        RuleFor(c => c.quantidadeParaGerar)
            .NotEmpty().WithMessage("A quantidade de vagas é obrigatória.")
            .GreaterThan(0).WithMessage("A quantidade de vagas deve ser maior que zero.")
            .LessThanOrEqualTo(30).WithMessage("A quantidade de vagas deve ser no máximo 30.");

        RuleFor(c => c.zona)
            .NotEmpty().WithMessage("A zona é obrigatória.")
            .Must(z => z >= 'A' && z <= 'Z').WithMessage("A zona deve ser uma letra maiúscula entre A e Z.");
    }
}