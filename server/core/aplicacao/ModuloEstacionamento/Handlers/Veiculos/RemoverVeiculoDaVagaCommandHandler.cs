using AutoMapper;
using FluentResults;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Handlers.Veiculos;
public class RemoverVeiculoDaVagaCommandHandler(
    IRepositorioEstacionamento repositorioEstacionamento, IRepositorioFatura repositorioFaturamento,
    IRepositorioConfiguracao repositorioConfiguracao, IUnitOfWork unitOfWork,
    IMapper mapper, IValidator<RemoverVeiculoDaVagaCommand> validator,
    ILogger<RemoverVeiculoDaVagaCommandHandler> logger

) : IRequestHandler<RemoverVeiculoDaVagaCommand, Result<RemoverVeiculoDaVagaResult>>
{
    public async Task<Result<RemoverVeiculoDaVagaResult>> Handle(RemoverVeiculoDaVagaCommand command, CancellationToken cancellationToken)
    {
        // [1] Validação de dados
        var resultadoValidacao = await validator.ValidateAsync(command, cancellationToken);

        if (!resultadoValidacao.IsValid)
        {
            var erros = resultadoValidacao.Errors.Select(e => e.ErrorMessage).ToList();
            var erroFormatado = ResultadosErro.RequisicaoInvalidaErro(erros);
            return Result.Fail(erroFormatado);
        }

        // [3] Remoção do veículo da vaga
        try
        {
            Veiculo? veiculo = null;

            // [1] Encontra o veiculo 
            if (command.placaVeiculo is not null)
                veiculo = await repositorioEstacionamento.SelecionarVeiculoPorPlaca(command.placaVeiculo);

            else if (command.numeroTicket.HasValue)
                veiculo = await repositorioEstacionamento.SelecionarVeiculoPorTicket(command.numeroTicket.Value);


            if (veiculo == null)
            {
                var erro = ResultadosErro.RequisicaoInvalidaErro("Veículo não encontrado.");
                return Result.Fail(erro);
            }


            // [2] Encontra a vaga onde o veiculo esta estacionado
            var vaga = await repositorioEstacionamento.SelecionarPorPlacaDoVeiculo(veiculo.Placa);

            if (vaga == null)
            {
                var erro = ResultadosErro.RequisicaoInvalidaErro("Vaga não encontrada para o veículo informado.");
                return Result.Fail(erro);
            }

            // [3] Remove o veiculo da vaga
            repositorioEstacionamento.RemoverVeiculoDaVaga(vaga);

            // [4] Gera uma Fatura para o veiculo

            var fatura = new Fatura(veiculo.Ticket.Id, veiculo.Placa, veiculo.Ticket.DataHoraEntrada, DateTime.UtcNow);
            int numeroDiarias = fatura.CalcularNumeroDiarias(veiculo.Ticket.DataHoraEntrada, DateTime.UtcNow);
            var valorDiaria = repositorioConfiguracao.ObterValorDiaria();
            fatura.CalcularValorTotal(numeroDiarias, valorDiaria);

            await repositorioFaturamento.CadastrarAsync(fatura);

            await unitOfWork.CommitAsync();

            var result = mapper.Map<RemoverVeiculoDaVagaResult>(vaga);

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
