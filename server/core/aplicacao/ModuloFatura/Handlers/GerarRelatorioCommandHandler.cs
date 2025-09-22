using AutoMapper;
using FluentResults;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFatura;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Handlers
{
    public class GerarRelatorioCommandHandler(
        IRepositorioRelatorio repositorioRelatorio, IRepositorioFatura repositorioFatura,
        ITenantProvider tenantProvider, IUnitOfWork unitOfWork, IMapper mapper,
        IDistributedCache cache, IValidator<GerarRelatorioCommand> validator,
        ILogger<GerarRelatorioCommandHandler> logger
    ) : IRequestHandler<GerarRelatorioCommand, Result<GerarRelatorioResult>>
    {
        public async Task<Result<GerarRelatorioResult>> Handle(GerarRelatorioCommand command, CancellationToken cancellationToken)
        {
            // [1] Validação de dados

            var resultadoValidacao = await validator.ValidateAsync(command, cancellationToken);

            if (!resultadoValidacao.IsValid)
            {
                var erros = resultadoValidacao.Errors.Select(e => e.ErrorMessage).ToList();
                var erroFormatado = ResultadosErro.RequisicaoInvalidaErro(erros);
                return Result.Fail(erroFormatado);
            }

            // [2] Geração do relatório
            try
            {
                // Cria o relatório com base nas faturas no período
                DateTime dataInicioUtc = DateTime.SpecifyKind(command.dataInicio, DateTimeKind.Utc);
                DateTime dataFimUtc = DateTime.SpecifyKind(command.dataFim, DateTimeKind.Utc);

                var faturas = await repositorioFatura.SelecionarFaturasPorPeriodoAsync(dataInicioUtc, dataFimUtc);

                var relatorio = new Relatorio(dataInicioUtc, dataFimUtc, faturas);

                await repositorioRelatorio.CadastrarAsync(relatorio);

                await unitOfWork.CommitAsync();

                // Invalida o cache

                var cacheKey = $"relatorios:u={tenantProvider.UsuarioId.GetValueOrDefault()}:q=all";

                await cache.RemoveAsync(cacheKey, cancellationToken);

                // Mapeia a entidade para o resultado

                var result = mapper.Map<GerarRelatorioResult>(relatorio);

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
}
