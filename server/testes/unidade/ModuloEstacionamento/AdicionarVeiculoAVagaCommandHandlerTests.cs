using AutoMapper;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Handlers.Veiculos;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloEstacionamento;

[TestClass]
[TestCategory("Teste de Unidade de AdicionarVeiculoAVagaCommandHandler")]
public class AdicionarVeiculoAVagaCommandHandlerTests
{
    private Mock<IRepositorioEstacionamento> _repoEstacionamento;
    private Mock<IRepositorioRecepcao> _repoRecepcao;
    private Mock<IUnitOfWork> _unitOfWork;
    private Mock<IMapper> _mapper;
    private Mock<IValidator<AdicionarVeiculoAVagaCommand>> _validator;
    private Mock<ILogger<AdicionarVeiculoAVagaCommandHandler>> _logger;
    private AdicionarVeiculoAVagaCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _repoEstacionamento = new Mock<IRepositorioEstacionamento>();
        _repoRecepcao = new Mock<IRepositorioRecepcao>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _validator = new Mock<IValidator<AdicionarVeiculoAVagaCommand>>();
        _logger = new Mock<ILogger<AdicionarVeiculoAVagaCommandHandler>>();
        _handler = new AdicionarVeiculoAVagaCommandHandler(
            _repoEstacionamento.Object,
            _repoRecepcao.Object,
            _unitOfWork.Object,
            _mapper.Object,
            _validator.Object,
            _logger.Object
        );
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Validacao_De_Dados_Eh_Invalida()
    {
        // Arrange
        var command = new AdicionarVeiculoAVagaCommand(Guid.NewGuid(), null, "ABC1234", null);
        var validationResult = new FluentValidation.Results.ValidationResult(new List<FluentValidation.Results.ValidationFailure> {
            new("Field", "Erro de validação")
        });

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Vaga_Nao_Encontrada()
    {
        // Arrange
        var command = new AdicionarVeiculoAVagaCommand(Guid.NewGuid(), null, "ABC1234", null);

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarRegistroPorIdAsync(It.IsAny<Guid>())).ReturnsAsync((Vaga)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Veiculo_Nao_Encontrado()
    {
        // Arrange
        var vaga = new Vaga('A') { NumeroVaga = 1 };
        var command = new AdicionarVeiculoAVagaCommand(vaga.Id, null, "ABC1234", null);

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarRegistroPorIdAsync(vaga.Id)).ReturnsAsync(vaga);
        _repoRecepcao.Setup(r => r.SelecionarVeiculoPorPlaca("ABC1234")).ReturnsAsync((Veiculo)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Vaga_Ja_Ocupada()
    {
        // Arrange
        var vaga = new Vaga('A') { NumeroVaga = 1, EstaOcupada = true };
        var veiculo = new Veiculo("ABC1234", "Modelo", "Cor");
        var command = new AdicionarVeiculoAVagaCommand(vaga.Id, null, "ABC1234", null);

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarRegistroPorIdAsync(vaga.Id)).ReturnsAsync(vaga);
        _repoRecepcao.Setup(r => r.SelecionarVeiculoPorPlaca("ABC1234")).ReturnsAsync(veiculo);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Veiculo_Ja_Esta_Estacionado()
    {
        // Arrange
        var vaga = new Vaga('A') { NumeroVaga = 1 };
        var veiculo = new Veiculo("ABC1234", "Modelo", "Cor") { VagaId = Guid.NewGuid() };
        var command = new AdicionarVeiculoAVagaCommand(vaga.Id, null, "ABC1234", null);

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarRegistroPorIdAsync(vaga.Id)).ReturnsAsync(vaga);
        _repoRecepcao.Setup(r => r.SelecionarVeiculoPorPlaca("ABC1234")).ReturnsAsync(veiculo);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Deve_Adicionar_Veiculo_A_Vaga_Com_Sucesso()
    {
        // Arrange
        var vaga = new Vaga('A') { NumeroVaga = 1 };
        var veiculo = new Veiculo("ABC1234", "Modelo", "Cor");
        var command = new AdicionarVeiculoAVagaCommand(vaga.Id, null, "ABC1234", null);
        var resultDto = new AdicionarVeiculoAVagaResult(new VagaDto(vaga.Id, vaga.NumeroVaga, vaga.Zona, true, null));

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarRegistroPorIdAsync(vaga.Id)).ReturnsAsync(vaga);
        _repoRecepcao.Setup(r => r.SelecionarVeiculoPorPlaca("ABC1234")).ReturnsAsync(veiculo);
        _mapper.Setup(m => m.Map<AdicionarVeiculoAVagaResult>(vaga)).Returns(resultDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _repoEstacionamento.Verify(r => r.AdicionarVeiculoAVaga(vaga, veiculo), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Fazer_Rollback_Em_Excecao()
    {
        // Arrange
        var vaga = new Vaga('A') { NumeroVaga = 1 };
        var veiculo = new Veiculo("ABC1234", "Modelo", "Cor");
        var command = new AdicionarVeiculoAVagaCommand(vaga.Id, null, "ABC1234", null);

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarRegistroPorIdAsync(vaga.Id)).ReturnsAsync(vaga);
        _repoRecepcao.Setup(r => r.SelecionarVeiculoPorPlaca("ABC1234")).ReturnsAsync(veiculo);
        _repoEstacionamento.Setup(r => r.AdicionarVeiculoAVaga(vaga, veiculo)).Throws(new Exception("Erro ao adicionar"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        _unitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
    }
}
