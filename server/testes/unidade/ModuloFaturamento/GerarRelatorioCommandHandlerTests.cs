using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Commands;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloFatura.Handlers;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFatura;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;

[TestClass]
[TestCategory("Teste de Unidade de GerarRelatorioCommandHandler")]
public class GerarRelatorioCommandHandlerTests
{
    private Mock<IRepositorioRelatorio> _repoRelatorio;
    private Mock<IRepositorioFatura> _repoFatura;
    private Mock<ITenantProvider> _tenantProvider;
    private Mock<IUnitOfWork> _unitOfWork;
    private Mock<IMapper> _mapper;
    private Mock<IDistributedCache> _cache;
    private Mock<IValidator<GerarRelatorioCommand>> _validator;
    private Mock<ILogger<GerarRelatorioCommandHandler>> _logger;
    private GerarRelatorioCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _repoRelatorio = new Mock<IRepositorioRelatorio>();
        _repoFatura = new Mock<IRepositorioFatura>();
        _tenantProvider = new Mock<ITenantProvider>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _cache = new Mock<IDistributedCache>();
        _validator = new Mock<IValidator<GerarRelatorioCommand>>();
        _logger = new Mock<ILogger<GerarRelatorioCommandHandler>>();
        _handler = new GerarRelatorioCommandHandler(
            _repoRelatorio.Object,
            _repoFatura.Object,
            _tenantProvider.Object,
            _unitOfWork.Object,
            _mapper.Object,
            _cache.Object,
            _validator.Object,
            _logger.Object
        );
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Validacao_De_Dados_Eh_Invalida()
    {
        // Arrange
        var command = new GerarRelatorioCommand(DateTime.Today, DateTime.Today.AddDays(2));
        var validationResult = new ValidationResult(new List<ValidationFailure> {
            new("dataInicio", "Data de início inválida")
        });

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _repoFatura.Verify(r => r.SelecionarFaturasPorPeriodoAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        _repoRelatorio.Verify(r => r.CadastrarAsync(It.IsAny<Relatorio>()), Times.Never);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
    }

    [TestMethod]
    public async Task Deve_Gerar_Relatorio_Com_Sucesso()
    {
        // Arrange
        var dataInicio = DateTime.Today;
        var dataFim = DateTime.Today.AddDays(2);
        var command = new GerarRelatorioCommand(dataInicio, dataFim);
        var validationResult = new ValidationResult();
        var faturas = new List<Fatura> { new Fatura(Guid.NewGuid(), "ABC1234", dataInicio, dataFim) };
        var relatorio = new Relatorio(dataInicio, dataFim, faturas);
        var relatorioDto = new RelatorioDto(relatorio.DataInicial, relatorio.DataFinal, relatorio.Faturas.Count, relatorio.ValorTotal);
        var resultDto = new GerarRelatorioResult(relatorioDto);

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
        _repoFatura.Setup(r => r.SelecionarFaturasPorPeriodoAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(faturas);
        _repoRelatorio.Setup(r => r.CadastrarAsync(It.IsAny<Relatorio>())).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
        _tenantProvider.Setup(t => t.UsuarioId).Returns(Guid.NewGuid());
        _cache.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mapper.Setup(m => m.Map<GerarRelatorioResult>(It.IsAny<Relatorio>())).Returns(resultDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(resultDto, result.Value);
        _repoFatura.Verify(r => r.SelecionarFaturasPorPeriodoAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        _repoRelatorio.Verify(r => r.CadastrarAsync(It.IsAny<Relatorio>()), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        _cache.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Ocorre_Excecao_No_Processo()
    {
        // Arrange
        var dataInicio = DateTime.Today;
        var dataFim = DateTime.Today.AddDays(2);
        var command = new GerarRelatorioCommand(dataInicio, dataFim);
        var validationResult = new ValidationResult();
        var faturas = new List<Fatura> { new Fatura(Guid.NewGuid(), "ABC1234", dataInicio, dataFim) };

        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
        _repoFatura.Setup(r => r.SelecionarFaturasPorPeriodoAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(faturas);
        _repoRelatorio.Setup(r => r.CadastrarAsync(It.IsAny<Relatorio>())).ThrowsAsync(new Exception("Erro ao cadastrar relatório"));
        _unitOfWork.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _unitOfWork.Verify(u => u.RollbackAsync(), Times.Once);
        _logger.Verify(l => l.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Ocorreu um erro durante o registro")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception, string>>()
        ), Times.Once);
    }
}