using AutoMapper;
using FluentResults;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Handlers.Veiculos;
public class AdicionarVeiculoAVagaCommandHandler(
    IRepositorioEstacionamento repositorioEstacionamento, IRepositorioRecepcao repositorioRecepcao,
    IUnitOfWork unitOfWork, IMapper mapper, IValidator<AdicionarVeiculoAVagaCommand> validator,
    ILogger<AdicionarVeiculoAVagaCommandHandler> logger
) : IRequestHandler<AdicionarVeiculoAVagaCommand, Result<AdicionarVeiculoAVagaResult>>
{
    public async Task<Result<AdicionarVeiculoAVagaResult>> Handle(AdicionarVeiculoAVagaCommand command, CancellationToken cancellationToken)
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

        Vaga? vaga = null;
        Veiculo? veiculo = null;

        // [1.1] Valida se a vaga foi encontrada

        if (command.vagaId.HasValue)
            vaga = await repositorioEstacionamento.SelecionarRegistroPorIdAsync(command.vagaId.Value);

        else if (command.numeroVaga.HasValue)
            vaga = await repositorioEstacionamento.SelecionarPorNumeroDaVaga(command.numeroVaga.Value);

        if (vaga == null)
        {
            var erro = ResultadosErro.RequisicaoInvalidaErro($"A vaga com número {command.numeroVaga} não foi encontrada.");
            return Result.Fail(erro);
        }

        //[2.2] Valida se o veiculo foi encontrado

        if (command.placaVeiculo is not null)
            veiculo = await repositorioRecepcao.SelecionarVeiculoPorPlaca(command.placaVeiculo);

        else if (command.numeroTicket is not null)
            veiculo = await repositorioRecepcao.SelecionarVeiculoPorTicket(command.numeroTicket);

        if (veiculo == null)
        {
            var erro = ResultadosErro.RequisicaoInvalidaErro($"Veículo com placa {command.placaVeiculo} não encontrado.");
            return Result.Fail(erro);
        }

        // [3.3] Valida se a vaga está disponivel

        if (vaga.EstaOcupada)
        {
            var erro = ResultadosErro.RequisicaoInvalidaErro($"A vaga {vaga.NumeroVaga} já está ocupada.");
            return Result.Fail(erro);
        }

      
        // [4.4] Valida se o veiculo já esta estacionado

        if (veiculo.VagaId.HasValue)
        {
            var erro = ResultadosErro.RequisicaoInvalidaErro($"O veículo com placa {veiculo.Placa} já está estacionado.");
            return Result.Fail(erro);
        }

        // [3] Adiciona o veículo à vaga
        try
        {
            repositorioEstacionamento.AdicionarVeiculoAVaga(vaga!, veiculo);

            await unitOfWork.CommitAsync();

            var result = mapper.Map<AdicionarVeiculoAVagaResult>(vaga);

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