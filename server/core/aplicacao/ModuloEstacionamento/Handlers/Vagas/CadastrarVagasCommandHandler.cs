using AutoMapper;
using FluentResults;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Handlers.Vagas;
public class CadastrarVagasCommandHandler(
    IRepositorioEstacionamento repositorioEstacionamento,
    ITenantProvider tenantProvider, IUnitOfWork unitOfWork, IMapper mapper,
    IDistributedCache cache, IValidator<CadastrarVagasCommand> validator,
    ILogger<CadastrarVagasCommandHandler> logger

) : IRequestHandler<CadastrarVagasCommand, Result<CadastrarVagasResult>>
{
    public async Task<Result<CadastrarVagasResult>> Handle(CadastrarVagasCommand command, CancellationToken cancellationToken)
    {
        // [1] Validação de dados
        var resultadoValidacao = await validator.ValidateAsync(command, cancellationToken);

        if (!resultadoValidacao.IsValid)
        {
            var erros = resultadoValidacao.Errors.Select(e => e.ErrorMessage).ToList();
            var erroFormatado = ResultadosErro.RequisicaoInvalidaErro(erros);
            return Result.Fail(erroFormatado);
        }

        // [3] Cadastro
        try
        {
            var vagasParaCadastro = new List<Vaga>(command.quantidadeParaGerar);

            for (int i = 0; i < command.quantidadeParaGerar; i++)
            {
                Vaga novaVaga = new Vaga(command.zona);
                vagasParaCadastro.Add(novaVaga);
            }

            await repositorioEstacionamento.CadastrarEntidadesAsync(vagasParaCadastro);

            await unitOfWork.CommitAsync();

            // Invalida o cache

            var cacheKey = $"checkins:u={tenantProvider.UsuarioId.GetValueOrDefault()}:q=all";

            await cache.RemoveAsync(cacheKey, cancellationToken);

            // Mapeia a entidade para o resultado
            var quantidadeCadastrada = vagasParaCadastro.Count;
            char zonaCadastrada = command.zona;

            var result = mapper.Map<CadastrarVagasResult>((quantidadeCadastrada, zonaCadastrada));

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
