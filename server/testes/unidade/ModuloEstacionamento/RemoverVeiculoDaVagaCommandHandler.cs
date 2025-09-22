using AutoMapper;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Veiculos;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Handlers.Veiculos;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloEstacionamento;

[TestClass]
[TestCategory("Teste de Unidade de RemoverVeiculoDaVagaCommandHandler")]
public class RemoverVeiculoDaVagaCommandHandlerTests
{
    private Mock<IRepositorioEstacionamento> _repoEstacionamento;
    private Mock<IRepositorioFatura> _repoFatura;
    private Mock<IRepositorioConfiguracao> _repoConfiguracao;
    private Mock<IUnitOfWork> _unitOfWork;
    private Mock<IMapper> _mapper;
    private Mock<IValidator<RemoverVeiculoDaVagaCommand>> _validator;
    private Mock<ILogger<RemoverVeiculoDaVagaCommandHandler>> _logger;
    private RemoverVeiculoDaVagaCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _repoEstacionamento = new Mock<IRepositorioEstacionamento>();
        _repoFatura = new Mock<IRepositorioFatura>();
        _repoConfiguracao = new Mock<IRepositorioConfiguracao>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _validator = new Mock<IValidator<RemoverVeiculoDaVagaCommand>>();
        _logger = new Mock<ILogger<RemoverVeiculoDaVagaCommandHandler>>();
        _handler = new RemoverVeiculoDaVagaCommandHandler(
            _repoEstacionamento.Object,
            _repoFatura.Object,
            _repoConfiguracao.Object,
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
        var command = new RemoverVeiculoDaVagaCommand("ABC1234", null);
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
    public async Task Deve_Falhar_Quando_Veiculo_Nao_Encontrado()
    {
        // Arrange
        var command = new RemoverVeiculoDaVagaCommand("ABC1234", null);

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarVeiculoPorPlaca("ABC1234")).ReturnsAsync((Veiculo)null!);

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
        var ticket = new Ticket { Id = Guid.NewGuid(), DataHoraEntrada = DateTime.UtcNow.AddHours(-2) };
        var checkIn = new CheckIn { Id = Guid.NewGuid(), Ticket = ticket };
        var veiculo = new Veiculo("ABC1234", "Modelo", "Cor");
        veiculo.AdicionarCheckIn(checkIn);
        var command = new RemoverVeiculoDaVagaCommand("ABC1234", null);

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarVeiculoPorPlaca("ABC1234")).ReturnsAsync(veiculo);
        _repoEstacionamento.Setup(r => r.SelecionarPorPlacaDoVeiculo("ABC1234")).ReturnsAsync((Vaga)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Deve_Remover_Veiculo_Da_Vaga_Com_Sucesso()
    {
        // Arrange
        var ticket = new Ticket { Id = Guid.NewGuid(), DataHoraEntrada = DateTime.UtcNow.AddHours(-2) };
        var checkIn = new CheckIn { Id = Guid.NewGuid(), Ticket = ticket };
        var veiculo = new Veiculo("ABC1234", "Modelo", "Cor");
        veiculo.AdicionarCheckIn(checkIn);
        var vaga = new Vaga('A') { NumeroVaga = 1, VeiculoEstacionado = veiculo };
        var command = new RemoverVeiculoDaVagaCommand("ABC1234", null);

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarVeiculoPorPlaca("ABC1234")).ReturnsAsync(veiculo);
        _repoEstacionamento.Setup(r => r.SelecionarPorPlacaDoVeiculo("ABC1234")).ReturnsAsync(vaga);
        _repoConfiguracao.Setup(r => r.ObterValorDiaria()).Returns(10m);
        _mapper.Setup(m => m.Map<RemoverVeiculoDaVagaResult>(vaga))
            .Returns(new RemoverVeiculoDaVagaResult(
                new VagaDto(
                    vaga.Id,
                    vaga.NumeroVaga,
                    vaga.Zona,
                    vaga.EstaOcupada,
                    null
                )
            ));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _repoEstacionamento.Verify(r => r.RemoverVeiculoDaVaga(vaga), Times.Once);
        _repoFatura.Verify(r => r.CadastrarAsync(It.IsAny<Fatura>()), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Fazer_Rollback_Em_Excecao()
    {
        // Arrange
        var ticket = new Ticket { Id = Guid.NewGuid(), DataHoraEntrada = DateTime.UtcNow.AddHours(-2) };
        var checkIn = new CheckIn { Id = Guid.NewGuid(), Ticket = ticket };
        var veiculo = new Veiculo("ABC1234", "Modelo", "Cor");
        veiculo.AdicionarCheckIn(checkIn);
        var vaga = new Vaga('A') { NumeroVaga = 1, VeiculoEstacionado = veiculo };
        var command = new RemoverVeiculoDaVagaCommand("ABC1234", null);

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _repoEstacionamento.Setup(r => r.SelecionarVeiculoPorPlaca("ABC1234")).ReturnsAsync(veiculo);
        _repoEstacionamento.Setup(r => r.SelecionarPorPlacaDoVeiculo("ABC1234")).ReturnsAsync(vaga);
        _repoEstacionamento.Setup(r => r.RemoverVeiculoDaVaga(vaga)).Throws(new Exception("Erro ao remover"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        _unitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
    }
}
