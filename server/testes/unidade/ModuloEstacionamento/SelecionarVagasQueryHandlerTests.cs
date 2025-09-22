using AutoMapper;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Handlers.Vagas;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;
using System.Text;
using System.Text.Json;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloEstacionamento;

[TestClass]
[TestCategory("Teste de Unidade de SelecionarVagasQueryHandler")]
public class SelecionarVagasQueryHandlerTests
{
    private Mock<IRepositorioEstacionamento> _repoEstacionamento;
    private Mock<IMapper> _mapper;
    private Mock<IDistributedCache> _cache;
    private Mock<ILogger<SelecionarVagasQueryHandler>> _logger;
    private SelecionarVagasQueryHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _repoEstacionamento = new Mock<IRepositorioEstacionamento>();
        _mapper = new Mock<IMapper>();
        _cache = new Mock<IDistributedCache>();
        _logger = new Mock<ILogger<SelecionarVagasQueryHandler>>();
        _handler = new SelecionarVagasQueryHandler(
            _repoEstacionamento.Object,
            _mapper.Object,
            _cache.Object,
            _logger.Object
        );
    }

    [TestMethod]
    public async Task Deve_Retornar_Do_Cache_Quando_Existente()
    {
        // Arrange
        var query = new SelecionarVagasQuery(null);
        var cacheKey = "checkins:v=1:scope=global:q=all";
        var resultDto = new SelecionarVagasResult(ImmutableList<VagaDto>.Empty);
        var jsonString = JsonSerializer.Serialize(resultDto);
        var bytes = Encoding.UTF8.GetBytes(jsonString);

        _cache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>())).ReturnsAsync(bytes);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _cache.Verify(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()), Times.Once);
        _logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Cache hit")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }

    [TestMethod]
    public async Task Deve_Buscar_No_Repositorio_Quando_Cache_Miss()
    {
        // Arrange
        var query = new SelecionarVagasQuery(null);
        var cacheKey = "checkins:v=1:scope=global:q=all";
        _cache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);

        var vagas = new List<Vaga>
        {
            new Vaga('A') { NumeroVaga = 1 },
            new Vaga('A') { NumeroVaga = 2 }
        };
        _repoEstacionamento.Setup(r => r.SelecionarRegistrosAsync()).ReturnsAsync(vagas);

        var resultDto = new SelecionarVagasResult(ImmutableList<VagaDto>.Empty);
        _mapper.Setup(m => m.Map<SelecionarVagasResult>(vagas)).Returns(resultDto);

        _cache.Setup(c => c.SetAsync(
            cacheKey,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()
        )).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _repoEstacionamento.Verify(r => r.SelecionarRegistrosAsync(), Times.Once);
        _mapper.Verify(m => m.Map<SelecionarVagasResult>(vagas), Times.Once);
        _cache.Verify(c => c.SetAsync(
            cacheKey,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Buscar_Com_Quantidade_Quando_Informada()
    {
        // Arrange
        var query = new SelecionarVagasQuery(2);
        var cacheKey = "checkins:v=1:scope=global:q=2";
        _cache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);

        var vagas = new List<Vaga>
        {
            new Vaga('B') { NumeroVaga = 1 },
            new Vaga('B') { NumeroVaga = 2 }
        };
        _repoEstacionamento.Setup(r => r.SelecionarRegistrosAsync(2)).ReturnsAsync(vagas);

        var resultDto = new SelecionarVagasResult(ImmutableList<VagaDto>.Empty);
        _mapper.Setup(m => m.Map<SelecionarVagasResult>(vagas)).Returns(resultDto);

        _cache.Setup(c => c.SetAsync(
            cacheKey,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()
        )).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _repoEstacionamento.Verify(r => r.SelecionarRegistrosAsync(2), Times.Once);
        _mapper.Verify(m => m.Map<SelecionarVagasResult>(vagas), Times.Once);
        _cache.Verify(c => c.SetAsync(
            cacheKey,
            It.IsAny<byte[]>(),
            It.IsAny<DistributedCacheEntryOptions>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Falhar_Em_Excecao()
    {
        // Arrange
        var query = new SelecionarVagasQuery(null);
        var cacheKey = "checkins:v=1:scope=global:q=all";
        _cache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>())).Throws(new Exception("Erro inesperado"));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        _logger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Ocorreu um erro")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()
            ),
            Times.Once
        );
    }
}
