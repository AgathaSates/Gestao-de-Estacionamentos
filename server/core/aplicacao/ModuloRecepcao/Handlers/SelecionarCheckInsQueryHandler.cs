using System.Text.Json;
using AutoMapper;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Handlers;
public class SelecionarCheckInsQueryHandler(
    IRepositorioRecepcao repositorioRecepcao, ITenantProvider tenantProvider,
    IMapper mapper, IDistributedCache cache, ILogger<SelecionarCheckInsQueryHandler> logger
) : IRequestHandler<SelecionarCheckInsQuery, Result<SelecionarCheckInsResult>>
{
    public async Task<Result<SelecionarCheckInsResult>> Handle(SelecionarCheckInsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var cacheQuery = query.quantidade.HasValue ? $"q={query.quantidade.Value}" : "q=all";
            string cacheKey = $"contatos:v=1:scope=global:{cacheQuery}";

            // [1] Tenta acessar o cache
            var jsonString = await cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrWhiteSpace(jsonString))
            {
                var registrosEmCache = JsonSerializer.Deserialize<SelecionarCheckInsResult>(jsonString);
                
                if (registrosEmCache is not null)
                {
                    logger.LogInformation("Cache hit for key {CacheKey}", cacheKey);
                    return Result.Ok(registrosEmCache);
                }
            }

            // [2] Cache miss -> busca no repositório
            var registros = query.quantidade.HasValue ?
               await repositorioRecepcao.SelecionarRegistrosAsync(query.quantidade.Value) :
               await repositorioRecepcao.SelecionarRegistrosAsync();

            var result = mapper.Map<SelecionarCheckInsResult>(registros);

            // [3] Salva os resultados novos no cache
            var jsonPayload = JsonSerializer.Serialize(result);

            var cacheOptions = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60) };

            await cache.SetStringAsync(cacheKey, jsonPayload, cacheOptions, cancellationToken);

            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(
              ex,
              "Ocorreu um erro durante a seleção de {@Registros}.",
              query
            );

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }   
    }
}