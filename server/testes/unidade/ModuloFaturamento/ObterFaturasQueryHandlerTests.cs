using System.Collections.Immutable;
using AutoMapper;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Handlers;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloFaturamento;

[TestClass]
[TestCategory("Teste de Unidade de ObterFaturasQueryHandler")]
public class ObterFaturasQueryHandlerTests
{
    private Mock<IRepositorioFatura> _repositorioFatura;
    private Mock<ITenantProvider> _tenantProvider;
    private Mock<IMapper> _mapper;
    private Mock<IDistributedCache> _cache;
    private Mock<ILogger<ObterFaturasQueryHandler>> _logger;
    private ObterFaturasQueryHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _repositorioFatura = new Mock<IRepositorioFatura>();
        _tenantProvider = new Mock<ITenantProvider>();
        _mapper = new Mock<IMapper>();
        _cache = new Mock<IDistributedCache>();
        _logger = new Mock<ILogger<ObterFaturasQueryHandler>>();

        _handler = new ObterFaturasQueryHandler(
            _repositorioFatura.Object,
            _tenantProvider.Object,
            _mapper.Object,
            _cache.Object,
            _logger.Object
        );
    }

    [TestMethod]
    public async Task Deve_Buscar_Sem_Quantidade_Quando_Nao_Informada()
    {
        // Arrange
        var consulta = new ObterFaturasQuery(null);
        var chaveCache = "faturas:v=1:scope=global:q=all";
        _cache
            .Setup(c => c.GetAsync(chaveCache, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        var listaFaturasDominio = new List<Fatura>();
        _repositorioFatura
            .Setup(r => r.SelecionarRegistrosAsync())
            .ReturnsAsync(listaFaturasDominio);
        var resultadoMapeado = new ObterFaturasResult(ImmutableList<FaturasDto>.Empty);
        _mapper
            .Setup(m => m.Map<ObterFaturasResult>(listaFaturasDominio))
            .Returns(resultadoMapeado);
        _cache
            .Setup(c => c.SetAsync(
                chaveCache,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ))
            .Returns(Task.CompletedTask);
        // Act
        Result<ObterFaturasResult> resultado = await _handler.Handle(consulta, CancellationToken.None);
        // Assert
        Assert.IsTrue(resultado.IsSuccess);
        _repositorioFatura.Verify(r => r.SelecionarRegistrosAsync(), Times.Once);
        _cache.Verify(c => c.SetAsync(
            chaveCache,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }


    [TestMethod]
    public async Task Deve_Buscar_Com_Quantidade_Quando_Informada()
    {
        // Arrange
        var consulta = new ObterFaturasQuery(2);
        var chaveCache = "faturas:v=1:scope=global:q=2";

        _cache
            .Setup(c => c.GetAsync(chaveCache, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var listaFaturasDominio = new List<Fatura>();
        _repositorioFatura
            .Setup(r => r.SelecionarRegistrosAsync(2))
            .ReturnsAsync(listaFaturasDominio);

        var resultadoMapeado = new ObterFaturasResult(ImmutableList<FaturasDto>.Empty);

        _mapper
            .Setup(m => m.Map<ObterFaturasResult>(listaFaturasDominio))
            .Returns(resultadoMapeado);

        _cache
            .Setup(c => c.SetAsync(
                chaveCache,
                It.IsAny<byte[]>(),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()
            ))
            .Returns(Task.CompletedTask);

        // Act
        Result<ObterFaturasResult> resultado = await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsSuccess);

        _repositorioFatura.Verify(r => r.SelecionarRegistrosAsync(2), Times.Once);

        _cache.Verify(c => c.SetAsync(
            chaveCache,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Falhar_Em_Excecao()
    {
        // Arrange
        var consulta = new ObterFaturasQuery(null);
        var chaveCache = "faturas:v=1:scope=global:q=all";

        _cache
            .Setup(c => c.GetAsync(chaveCache, It.IsAny<CancellationToken>()))
            .Throws(new Exception("Erro inesperado"));

        // Act
        Result<ObterFaturasResult> resultado = await _handler.Handle(consulta, CancellationToken.None);

        // Assert
        Assert.IsTrue(resultado.IsFailed);

        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Ocorreu um erro")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }
}
