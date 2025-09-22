using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao.Handlers
{
    public class SairCommandHandler(
    SignInManager<Usuario> signInManager
) : IRequestHandler<SairCommand, Result>
    {
        public async Task<Result> Handle(SairCommand request, CancellationToken cancellationToken)
        {
            await signInManager.SignOutAsync();

            return Result.Ok();
        }
    }
}
