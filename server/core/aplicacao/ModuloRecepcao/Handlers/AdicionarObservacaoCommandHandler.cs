using AutoMapper;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Handlers;
public class AdicionarObservacaoCommandHandler(
    IRepositorioRecepcao repositorioRecepcao, ITenantProvider tenantProvider,
    IUnitOfWork unitOfWork, IMapper mapper, IDistributedCache cache,
    ILogger<AdicionarObservacaoCommandHandler> logger
) : IRequestHandler<AdicionarObservacaoCommand, Result<AdicionarObservacaoResult>>
{
    public async Task<Result<AdicionarObservacaoResult>> Handle(AdicionarObservacaoCommand command, CancellationToken cancellationToken)
    {
        try 
        {
            var checkIn = await repositorioRecepcao.SelecionarRegistroPorIdAsync(command.id);

            checkIn.Veiculo.AdicionarObservacoes(command.observacao);

            await unitOfWork.CommitAsync();

            // Invalida o cache
            var cacheKey = $"checkins:u={tenantProvider.UsuarioId.GetValueOrDefault()}:q=all";
            await cache.RemoveAsync(cacheKey, cancellationToken);

            var result = mapper.Map<AdicionarObservacaoResult>(checkIn.Veiculo);

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