using AutoMapper;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Handlers.Veiculos;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloRecepcao.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Immutable;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloEstacionamento;

[TestClass]
[TestCategory("Teste de Unidade de SelecionarVeiculosEstacionadosQueryHandler")]
public class SelecionarVeiculosEstacionadosQueryHandlerTests
{
    private Mock<IRepositorioEstacionamento> _repoEstacionamento;
    private Mock<IMapper> _mapper;
    private Mock<ILogger<SelecionarVeiculosEstacionadosQueryHandler>> _logger;
    private SelecionarVeiculosEstacionadosQueryHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _repoEstacionamento = new Mock<IRepositorioEstacionamento>();
        _mapper = new Mock<IMapper>();
        _logger = new Mock<ILogger<SelecionarVeiculosEstacionadosQueryHandler>>();
        _handler = new SelecionarVeiculosEstacionadosQueryHandler(
            _repoEstacionamento.Object,
            _mapper.Object,
            _logger.Object
        );
    }

    [TestMethod]
    public async Task Deve_Retornar_Veiculo_Por_Placa()
    {
        // Arrange
        var veiculo = new Veiculo("ABC1234", "Modelo", "Cor");
        var query = new SelecionarVeiculosEstacionadosQuery("ABC1234");
        var resultDto = new SelecionarVeiculosEstacionadosResult(
            ImmutableList.Create<VisualizarVeiculoDto>(
                new VisualizarVeiculoDto("ABC1234", "Modelo", "Cor", null)
            )
        );

        _repoEstacionamento.Setup(r => r.SelecionarVeiculoPorPlaca("ABC1234")).ReturnsAsync(veiculo);
        _mapper.Setup(m => m.Map<SelecionarVeiculosEstacionadosResult>(It.IsAny<List<Veiculo>>())).Returns(resultDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _repoEstacionamento.Verify(r => r.SelecionarVeiculoPorPlaca("ABC1234"), Times.Once);
        _mapper.Verify(m => m.Map<SelecionarVeiculosEstacionadosResult>(It.IsAny<List<Veiculo>>()), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Retornar_Todos_Os_Veiculos_Estacionados()
    {
        // Arrange
        var veiculos = new List<Veiculo>
        {
            new Veiculo("ABC1234", "Modelo", "Cor"),
            new Veiculo("XYZ9876", "Modelo2", "Cor2")
        };
        var query = new SelecionarVeiculosEstacionadosQuery(null);
        var veiculoDtos = ImmutableList.Create<VisualizarVeiculoDto>(
            new VisualizarVeiculoDto("ABC1234", "Modelo", "Cor", null),
            new VisualizarVeiculoDto("XYZ9876", "Modelo2", "Cor2", null)
        );

        var resultDto = new SelecionarVeiculosEstacionadosResult(veiculoDtos);

        _repoEstacionamento.Setup(r => r.SelecionaTodosOsVeiculosEstacionados()).ReturnsAsync(veiculos);
        _mapper.Setup(m => m.Map<SelecionarVeiculosEstacionadosResult>(It.IsAny<List<Veiculo>>()))
            .Returns(resultDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _repoEstacionamento.Verify(r => r.SelecionaTodosOsVeiculosEstacionados(), Times.Once);
        _mapper.Verify(m => m.Map<SelecionarVeiculosEstacionadosResult>(veiculos), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Fazer_Rollback_Em_Excecao()
    {
        // Arrange
        var query = new SelecionarVeiculosEstacionadosQuery(null);

        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new SelecionarVeiculosEstacionadosQueryHandler(
            _repoEstacionamento.Object,
            _mapper.Object,
            _logger.Object
        );

        _repoEstacionamento.Setup(r => r.SelecionaTodosOsVeiculosEstacionados())
            .Throws(new Exception("Erro inesperado"));

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

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