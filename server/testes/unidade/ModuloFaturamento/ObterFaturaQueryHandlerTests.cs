
using System.Runtime.Serialization;
using AutoMapper;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Handlers;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloFaturamento;

[TestClass]
[TestCategory("Teste de Unidade de ObterFaturaQueryHandler")]
public class ObterFaturaQueryHandlerTests
{
    private Mock<IMapper> _mapper;
    private Mock<IRepositorioFatura> _repositorioFatura;
    private Mock<ILogger<ObterFaturaQueryHandler>> _logger;
    private ObterFaturaQueryHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _mapper = new Mock<IMapper>();
        _repositorioFatura = new Mock<IRepositorioFatura>();
        _logger = new Mock<ILogger<ObterFaturaQueryHandler>>();

        _handler = new ObterFaturaQueryHandler(
            _mapper.Object,
            _repositorioFatura.Object,
            _logger.Object
        );
    }


    [TestMethod]
    public async Task Deve_Retornar_Fatura_Quando_Id_Existe()
    {
        // Arrange
        Guid identificadorFatura = Guid.NewGuid();
        var consulta = new ObterFaturaQuery(identificadorFatura);

        // Instância "dummy" de Fatura sem depender do construtor
        var entidadeFatura = (Fatura)FormatterServices.GetUninitializedObject(typeof(Fatura));

        _repositorioFatura
            .Setup(r => r.SelecionarRegistroPorIdAsync(identificadorFatura))
            .ReturnsAsync(entidadeFatura);

        // Cria o DTO e o Result conforme seu record
        var dataEntrada = new DateTime(2025, 09, 01, 8, 30, 0, DateTimeKind.Utc);
        var dataSaida = new DateTime(2025, 09, 01, 18, 0, 0, DateTimeKind.Utc);
        var faturaDto = new FaturaDto(
            ticket: Guid.NewGuid(),
            placaVeiculo: "ABC1234",
            dataEntrada: dataEntrada,
            dataSaida: dataSaida,
            valorTotal: 100.50m
        );
        var resultadoMapeado = new ObterFaturaResult(faturaDto);

        _mapper
            .Setup(m => m.Map<ObterFaturaResult>(entidadeFatura))
            .Returns(resultadoMapeado);

        // Act
        Result<ObterFaturaResult> resultado =
            await _handler.Handle(consulta, CancellationToken.None);

        // Assert (contrato)
        Assert.IsTrue(resultado.IsSuccess, "Esperava sucesso quando a fatura existe.");
        Assert.AreEqual(resultadoMapeado, resultado.Value, "O DTO retornado deveria ser o mapeado pelo AutoMapper.");

        _repositorioFatura.Verify(r => r.SelecionarRegistroPorIdAsync(identificadorFatura), Times.Once);
        _mapper.Verify(m => m.Map<ObterFaturaResult>(entidadeFatura), Times.Once);

        // Não deve registrar erro
        _logger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Fatura_Nao_Encontrada()
    {
        // Arrange
        Guid identificadorFatura = Guid.NewGuid();
        var consulta = new ObterFaturaQuery(identificadorFatura);

        _repositorioFatura
            .Setup(r => r.SelecionarRegistroPorIdAsync(identificadorFatura))
            .ReturnsAsync((Fatura)null!);

        // Act
        Result<ObterFaturaResult> resultado =
            await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        _repositorioFatura.Verify(
            r => r.SelecionarRegistroPorIdAsync(identificadorFatura),
            Times.Once);

        _mapper.Verify(m => m.Map<ObterFaturaResult>(It.IsAny<Fatura>()), Times.Never);

        _logger.Verify(
            x => x.Log(
                It.Is<LogLevel>(l => l == LogLevel.Error),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Never);
    }

    [TestMethod]
    public async Task Deve_Falhar_E_Logar_Erro_Quando_Excecao_Ocorre()
    {
        // Arrange
        Guid identificadorFatura = Guid.NewGuid();
        var consulta = new ObterFaturaQuery(identificadorFatura);

        _repositorioFatura
            .Setup(r => r.SelecionarRegistroPorIdAsync(identificadorFatura))
            .ThrowsAsync(new Exception("Falha inesperada no repositório"));

        // Act
        Result<ObterFaturaResult> resultado =
            await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsFailed, "Resultado deveria ser falha quando ocorre exceção.");

        _repositorioFatura.Verify(
            r => r.SelecionarRegistroPorIdAsync(identificadorFatura),
            Times.Once);

        _mapper.Verify(m => m.Map<ObterFaturaResult>(It.IsAny<Fatura>()), Times.Never);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString()!
                    .Contains("Ocorreu um erro durante a seleção", StringComparison.Ordinal)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}
