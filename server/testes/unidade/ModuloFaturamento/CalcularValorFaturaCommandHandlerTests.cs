using System;
using System.Collections.Generic;
using System.Threading;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Handlers;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Microsoft.Extensions.Logging;
using Moq;

[TestClass]
[TestCategory("Teste de Unidade de CalcularValorFaturaCommandHandler")]
public class CalcularValorFaturaCommandHandlerTests
{
    private Mock<IMapper> _mapper;
    private Mock<IRepositorioFatura> _repoFatura;
    private Mock<IRepositorioConfiguracao> _repoConfiguracao;
    private Mock<IValidator<CalcularValorFaturaCommand>> _validator;
    private Mock<ILogger<CalcularValorFaturaCommandHandler>> _logger;
    private CalcularValorFaturaCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _mapper = new Mock<IMapper>();
        _repoFatura = new Mock<IRepositorioFatura>();
        _repoConfiguracao = new Mock<IRepositorioConfiguracao>();
        _validator = new Mock<IValidator<CalcularValorFaturaCommand>>();
        _logger = new Mock<ILogger<CalcularValorFaturaCommandHandler>>();
        _handler = new CalcularValorFaturaCommandHandler(
            _mapper.Object,
            _repoFatura.Object,
            _repoConfiguracao.Object,
            _validator.Object,
            _logger.Object
        );
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Validacao_De_Dados_Eh_Invalida()
    {
        // Arrange
        var command = new CalcularValorFaturaCommand(DateTime.Today, DateTime.Today.AddDays(2));
        var validationResult = new ValidationResult(new List<ValidationFailure> {
            new("dataInicio", "Data de início inválida")
        });

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repoFatura.Verify(r => r.CalcularValorFatura(It.IsAny<int>(), It.IsAny<decimal>()), Times.Never);
    }

    [TestMethod]
    public async Task Deve_Calcular_Valor_Fatura_Com_Sucesso()
    {
        // Arrange
        var dataInicio = DateTime.Today;
        var dataFim = DateTime.Today.AddDays(2);
        var command = new CalcularValorFaturaCommand(dataInicio, dataFim);
        var validationResult = new ValidationResult();
        var valorDiaria = 50m;
        var numeroDiarias = (dataFim - dataInicio).Days + 1;
        var valorCalculado = 150m;
        var resultDto = new CalcularValorFaturaResult(valorCalculado);

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
        _repoConfiguracao.Setup(r => r.ObterValorDiaria()).Returns(valorDiaria);
        _repoFatura.Setup(r => r.CalcularValorFatura(numeroDiarias, valorDiaria)).Returns(valorCalculado);
        _mapper.Setup(m => m.Map<CalcularValorFaturaResult>(valorCalculado)).Returns(resultDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(resultDto, result.Value);
        _repoFatura.Verify(r => r.CalcularValorFatura(numeroDiarias, valorDiaria), Times.Once);
    }
}
