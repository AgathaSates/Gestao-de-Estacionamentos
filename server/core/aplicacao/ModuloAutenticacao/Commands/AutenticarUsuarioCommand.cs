using FluentResults;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao.Commands
{
    public record AutenticarUsuarioCommand(string Email, string Senha) : IRequest<Result<AccessToken>>;
}