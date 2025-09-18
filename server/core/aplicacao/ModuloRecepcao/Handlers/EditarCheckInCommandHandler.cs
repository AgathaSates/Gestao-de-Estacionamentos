using AutoMapper;
using FluentResults;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Handlers;
public class EditarCheckInCommandHandler(IRepositorioRecepcao repositorioRecepcao,
    ITenantProvider tenantProvider, IUnitOfWork unitOfWork, IMapper mapper,
    IDistributedCache cache, IValidator<EditarCheckInCommand> validator,
    ILogger<EditarCheckInCommandHandler> logger
    ) : IRequestHandler<EditarCheckInCommand, Result<EditarCheckInResult>>
{
    public async Task<Result<EditarCheckInResult>> Handle
        (EditarCheckInCommand command, CancellationToken cancellationToken)
    {
        // [1] Validação de dados
        var resultadoValidacao = await validator.ValidateAsync(command, cancellationToken);

        if (!resultadoValidacao.IsValid)
        {
            var erros = resultadoValidacao.Errors.Select(e => e.ErrorMessage).ToList();
            var erroformatado = ResultadosErro.RequisicaoInvalidaErro(erros);
            return Result.Fail(erroformatado);
        }

        // [2] Validação de negócio
        var registros = await repositorioRecepcao.SelecionarRegistrosAsync();

        if (registros.Any(r => r.Veiculo.Placa == command.Veiculo.placa && r.Ticket.DataHoraSaida == null && r.Id != command.Id))
        {
            var erro = ResultadosErro.RegistroDuplicadoErro(
                $"Já existe um check-in ativo para o veículo com placa {command.Veiculo.placa}.");
            return Result.Fail(erro);
        }

        // [3] Edição
        try
        {
            var checkinEditado = mapper.Map<CheckIn>(command);

            var checkInAtualizado = await repositorioRecepcao.EditarAsync(command.Id, checkinEditado);

            await unitOfWork.CommitAsync();

            // Invalida o cache
            var cacheKey = $"contatos:u={tenantProvider.UsuarioId.GetValueOrDefault()}:q=all";

            await cache.RemoveAsync(cacheKey, cancellationToken);

            var result = mapper.Map<EditarCheckInResult>(checkInAtualizado);

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