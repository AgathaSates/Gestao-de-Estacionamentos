using FluentResults;
using MediatR;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao.Commands
{
    public record SairCommand : IRequest<Result>;
}
