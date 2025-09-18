using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.FluentValidation.ModuloRecepcao;
public class EditarCheckInCommandValidator : AbstractValidator<EditarCheckInCommand>
{
    public EditarCheckInCommandValidator()
    {
        RuleFor(c => c.Veiculo.placa)
           .NotEmpty().WithMessage("A placa do veículo é obrigatória.")
           .Matches(@"^[A-Z0-9]{7}$")
           .WithMessage("A placa do veículo deve conter exatamente 7 caracteres alfanuméricos (letras maiúsculas e números sem hífen).");

        RuleFor(c => c.Veiculo.modelo)
            .NotEmpty().WithMessage("O modelo do veículo é obrigatório.")
            .MinimumLength(3).WithMessage("O modelo do veículo deve ter no mínimo 3 caractere.")
            .MaximumLength(50).WithMessage("O modelo do veículo deve ter no máximo 50 caracteres.");

        RuleFor(c => c.Veiculo.cor)
            .NotEmpty().WithMessage("A cor do veículo é obrigatória.")
            .MinimumLength(3).WithMessage("A cor do veículo deve ter no mínimo 3 caractere.")
            .MaximumLength(20).WithMessage("A cor do veículo deve ter no máximo 20 caracteres.");

        RuleFor(c => c.Veiculo.observacoes)
            .MinimumLength(3).WithMessage("As observações do veículo devem ter no mínimo 3 caractere.")
            .MaximumLength(200).WithMessage("As observações do veículo devem ter no máximo 200 caracteres.")
            .When(c => !string.IsNullOrEmpty(c.Veiculo.observacoes));

        RuleFor(c => c.CPF)
            .NotEmpty().WithMessage("O CPF do cliente é obrigatório.")
            .Matches(@"^\d{11}$").WithMessage("O CPF do cliente deve conter exatamente 11 dígitos numéricos (sem pontos ou hífen).");

        RuleFor(c => c.Nome)
            .NotEmpty().WithMessage("O nome do cliente é obrigatório.")
            .MinimumLength(3).WithMessage("O nome do cliente deve ter no mínimo 3 caractere.")
            .MaximumLength(100).WithMessage("O nome do cliente deve ter no máximo 100 caracteres.");
    }
}