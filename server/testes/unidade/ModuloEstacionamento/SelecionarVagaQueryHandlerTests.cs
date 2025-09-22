using AutoMapper;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Handlers.Vagas;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloEstacionamento;

[TestClass]
[TestCategory("Teste de Unidade de SelecionarVagaQueryHandler")]
public class SelecionarVagaQueryHandlerTests
{
    private Mock<IMapper> _mapper;
    private Mock<IRepositorioEstacionamento> _repoEstacionamento;
    private Mock<ILogger<SelecionarVagaQueryHandler>> _logger;
    private Mock<IValidator<SelecionarVagaQuery>> _validator;
    private SelecionarVagaQueryHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _mapper = new Mock<IMapper>();
        _repoEstacionamento = new Mock<IRepositorioEstacionamento>();
        _logger = new Mock<ILogger<SelecionarVagaQueryHandler>>();
        _validator = new Mock<IValidator<SelecionarVagaQuery>>();
        _handler = new SelecionarVagaQueryHandler(
            _mapper.Object,
            _repoEstacionamento.Object,
            _logger.Object,
            _validator.Object
        );
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Validacao_De_Dados_Eh_Invalida()
    {
        // Arrange
        var query = new SelecionarVagaQuery(Guid.NewGuid(), null, null);
        var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure> {
            new("Field", "Erro de validação")
        });

        _validator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
    }

    [TestMethod]
    public async Task Deve_Retornar_Vaga_Por_Id()
    {
        // Arrange
        var vaga = new Vaga('A') { NumeroVaga = 1 };
        var query = new SelecionarVagaQuery(vaga.Id, null, null);

        _validator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarRegistroPorIdAsync(vaga.Id)).ReturnsAsync(vaga);

        var vagaDto = new VagaDto(
            vaga.Id,
            vaga.NumeroVaga,
            vaga.Zona,
            vaga.EstaOcupada,
            null
        );

        var resultDto = new SelecionarVagaResult(vagaDto);
        _mapper.Setup(m => m.Map<SelecionarVagaResult>(vaga)).Returns(resultDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _repoEstacionamento.Verify(r => r.SelecionarRegistroPorIdAsync(vaga.Id), Times.Once);
        _mapper.Verify(m => m.Map<SelecionarVagaResult>(vaga), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Retornar_Vaga_Por_Numero()
    {
        // Arrange
        var vaga = new Vaga('B') { NumeroVaga = 10 };
        var query = new SelecionarVagaQuery(null, 10, null);

        _validator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarPorNumeroDaVaga(10)).ReturnsAsync(vaga);

        var vagaDto = new VagaDto(
            vaga.Id,
            vaga.NumeroVaga,
            vaga.Zona,
            vaga.EstaOcupada,
            null
        );

        var resultDto = new SelecionarVagaResult(vagaDto);
        _mapper.Setup(m => m.Map<SelecionarVagaResult>(vaga)).Returns(resultDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _repoEstacionamento.Verify(r => r.SelecionarPorNumeroDaVaga(10), Times.Once);
        _mapper.Verify(m => m.Map<SelecionarVagaResult>(vaga), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Retornar_Vaga_Por_Placa()
    {
        // Arrange
        var vaga = new Vaga('C') { NumeroVaga = 20 };
        var query = new SelecionarVagaQuery(null, null, "ABC1234");

        _validator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarPorPlacaDoVeiculo("ABC1234")).ReturnsAsync(vaga);

        var vagaDto = new VagaDto(
            vaga.Id,
            vaga.NumeroVaga,
            vaga.Zona,
            vaga.EstaOcupada,
            null
        );

        var resultDto = new SelecionarVagaResult(vagaDto);
        _mapper.Setup(m => m.Map<SelecionarVagaResult>(vaga)).Returns(resultDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _repoEstacionamento.Verify(r => r.SelecionarPorPlacaDoVeiculo("ABC1234"), Times.Once);
        _mapper.Verify(m => m.Map<SelecionarVagaResult>(vaga), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Vaga_Nao_Encontrada()
    {
        // Arrange
        var query = new SelecionarVagaQuery(Guid.NewGuid(), null, null);

        _validator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarRegistroPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Vaga)null!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
    }

    [TestMethod]
    public async Task Deve_Falhar_Em_Excecao()
    {
        // Arrange
        var query = new SelecionarVagaQuery(Guid.NewGuid(), null, null);

        _validator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarRegistroPorIdAsync(It.IsAny<Guid>())).Throws(new Exception("Erro inesperado"));

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
