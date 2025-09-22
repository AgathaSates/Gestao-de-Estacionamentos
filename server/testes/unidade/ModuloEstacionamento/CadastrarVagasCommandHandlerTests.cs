using AutoMapper;
using FluentValidation;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Commands.Vagas;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloEstacionamento.Handlers.Vagas;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloEstacionamento;

[TestClass]
[TestCategory("Teste de Unidade de CadastrarVagasCommandHandler")]
public class CadastrarVagasCommandHandlerTests
{
    private Mock<IRepositorioEstacionamento> _repoEstacionamento;
    private Mock<ITenantProvider> _tenantProvider;
    private Mock<IUnitOfWork> _unitOfWork;
    private Mock<IMapper> _mapper;
    private Mock<IDistributedCache> _cache;
    private Mock<IValidator<CadastrarVagasCommand>> _validator;
    private Mock<ILogger<CadastrarVagasCommandHandler>> _logger;
    private CadastrarVagasCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _repoEstacionamento = new Mock<IRepositorioEstacionamento>();
        _tenantProvider = new Mock<ITenantProvider>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _mapper = new Mock<IMapper>();
        _cache = new Mock<IDistributedCache>();
        _validator = new Mock<IValidator<CadastrarVagasCommand>>();
        _logger = new Mock<ILogger<CadastrarVagasCommandHandler>>();
        _handler = new CadastrarVagasCommandHandler(
            _repoEstacionamento.Object,
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
        var command = new CadastrarVagasCommand(5,'A');
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
    public async Task Deve_Cadastrar_Vagas_Com_Sucesso()
    {
        // Arrange
        var command = new CadastrarVagasCommand(3, 'A');
        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _tenantProvider.Setup(t => t.UsuarioId).Returns(Guid.NewGuid());

        var vagas = new List<Vaga>
        {
            new Vaga('B'),
            new Vaga('B'),
            new Vaga('B')
        };

        _repoEstacionamento.Setup(r => r.CadastrarEntidadesAsync(It.IsAny<IList<Vaga>>()))
            .Returns(Task.CompletedTask);

        _unitOfWork.Setup(u => u.CommitAsync()).Returns(Task.CompletedTask);
        _cache.Setup(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var resultDto = new CadastrarVagasResult(3, 'B');
        _mapper.Setup(m => m.Map<CadastrarVagasResult>(It.IsAny<(int, char)>()))
            .Returns(resultDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _repoEstacionamento.Verify(r => r.CadastrarEntidadesAsync(It.IsAny<IList<Vaga>>()), Times.Once);
        _unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        _cache.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Fazer_Rollback_Em_Excecao()
    {
        // Arrange
        var command = new CadastrarVagasCommand(2, 'A');
        _validator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _tenantProvider.Setup(t => t.UsuarioId).Returns(Guid.NewGuid());

        _repoEstacionamento.Setup(r => r.CadastrarEntidadesAsync(It.IsAny<IList<Vaga>>()))
            .Throws(new Exception("Erro ao cadastrar"));

        _unitOfWork.Setup(u => u.RollbackAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        _unitOfWork.Verify(u => u.RollbackAsync(), Times.Once);

        // Verifica chamada ao método base Log
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