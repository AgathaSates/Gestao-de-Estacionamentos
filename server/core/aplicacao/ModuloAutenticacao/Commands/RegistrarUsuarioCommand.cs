using FluentResults;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao.Commands
{
    public record RegistrarUsuarioCommand(string NomeCompleto, string Email, string Senha, string ConfirmarSenha)
    : IRequest<Result<AccessToken>>;
}