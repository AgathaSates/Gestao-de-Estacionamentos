using AutoMapper;
using FluentResults;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloAutenticacao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Handlers;
public class CadastrarCheckInCommandHandler(IRepositorioRecepcao repositorioRecepcao,
    ITenantProvider tenantProvider, IUnitOfWork unitOfWork, IMapper mapper,
    IDistributedCache cache, IValidator<CadastrarCheckInCommand> validator,
    ILogger<CadastrarCheckInCommandHandler> logger
) : IRequestHandler<CadastrarCheckInCommand, Result<CadastrarCheckInResult>>
{
    public async Task<Result<CadastrarCheckInResult>> Handle(
        CadastrarCheckInCommand command, CancellationToken cancellationToken)
    {
       // [1] Validação de dados
        var resultadoValidacao = await validator.ValidateAsync(command, cancellationToken);

        if (!resultadoValidacao.IsValid)
        {
            var erros = resultadoValidacao.Errors.Select(e => e.ErrorMessage).ToList();
            var erroFormatado = ResultadosErro.RequisicaoInvalidaErro(erros);
            return Result.Fail(erroFormatado);
        }

        // [2] Validação de negócio
        var registros = await repositorioRecepcao.SelecionarRegistrosAsync();

        if (registros.Any(r => r.Veiculo.Placa == command.veiculo.placa && r.Ticket.DataHoraSaida == null))
        {
            var erro = ResultadosErro.RegistroDuplicadoErro(
                $"Já existe um check-in ativo para o veículo com placa {command.veiculo.placa}.");
            return Result.Fail(erro);
        }

        // [3] Cadastro
        try
        {
            var checkin = mapper.Map<CheckIn>(command);

            checkin.UsuarioId = tenantProvider.UsuarioId.GetValueOrDefault();

            await repositorioRecepcao.CadastrarAsync(checkin);

            await unitOfWork.CommitAsync();

            // Invalida o cache
            var cacheKey = $"contatos:u={tenantProvider.UsuarioId.GetValueOrDefault()}:q=all";

            await cache.RemoveAsync(cacheKey, cancellationToken);

            var result = mapper.Map<CadastrarCheckInResult>(checkin);

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync();

            logger.LogError(
                ex,
                "Ocorreu um erro durante o registro de {@Registro}.",
                command
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}