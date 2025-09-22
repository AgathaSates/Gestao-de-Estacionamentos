using AutoMapper;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Handlers.Veiculos;
public class SelecionarVeiculosEstacionadosQueryHandler(
    IRepositorioEstacionamento repositorioEstacionamento, IMapper mapper,
    ILogger<SelecionarVeiculosEstacionadosQueryHandler> logger
) : IRequestHandler<SelecionarVeiculosEstacionadosQuery, Result<SelecionarVeiculosEstacionadosResult>>
{
    public async Task<Result<SelecionarVeiculosEstacionadosResult>> Handle(SelecionarVeiculosEstacionadosQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // [1] Valida se a placa foi informada
            if (!string.IsNullOrWhiteSpace(request.placa))
            {
                var veiculoEstacionado = await repositorioEstacionamento.SelecionarVeiculoPorPlaca(request.placa);
                var veiculos = new List<Veiculo>();
                veiculos.Add(veiculoEstacionado);
                var result = mapper.Map<SelecionarVeiculosEstacionadosResult>(veiculos);
                return Result.Ok(result);
            }

            // [2] Se não, retorna todos os veículos estacionados
            var veiculosEstacionados = await repositorioEstacionamento.SelecionaTodosOsVeiculosEstacionados();

            var result2 = mapper.Map<SelecionarVeiculosEstacionadosResult>(veiculosEstacionados);

            return Result.Ok(result2);
        }
        catch (Exception ex)
        {
            logger.LogError(
              ex,
              "Ocorreu um erro durante a seleção de {@Registros}.", request);

            return Result.Fail(ResultadosErro.ExcecaoInternaErro(ex));
        }
    }
}
