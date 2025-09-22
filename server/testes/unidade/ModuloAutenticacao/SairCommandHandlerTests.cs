using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao.Commands;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao.Handlers;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
[TestCategory("Teste de Unidade de SairCommandHandler")]
public class SairCommandHandlerTests
{
    private Mock<SignInManager<Usuario>> _signInManager;
    private SairCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        var userStore = new Mock<IUserStore<Usuario>>();
        var userManager = new UserManager<Usuario>(
            userStore.Object,
            new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<Usuario>>().Object,
            new List<IUserValidator<Usuario>>(),
            new List<IPasswordValidator<Usuario>>(),
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<Microsoft.Extensions.Logging.ILogger<UserManager<Usuario>>>().Object
        );
        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<Usuario>>();
        var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());
        var loggerSignInManager = new Mock<Microsoft.Extensions.Logging.ILogger<SignInManager<Usuario>>>();
        var schemeProvider = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<Usuario>>();

        _signInManager = new Mock<SignInManager<Usuario>>(
            userManager,
            contextAccessor.Object,
            claimsFactory.Object,
            options.Object,
            loggerSignInManager.Object,
            schemeProvider.Object,
            confirmation.Object
        );

        _handler = new SairCommandHandler(_signInManager.Object);
    }

    [TestMethod]
    public async Task Deve_Chamar_SignOutAsync_E_Retornar_Sucesso()
    {
        // Arrange
        _signInManager.Setup(s => s.SignOutAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(new SairCommand(), CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        _signInManager.Verify(s => s.SignOutAsync(), Times.Once);
    }

    [TestMethod]
    public async Task Deve_Lancar_Excecao_Quando_SignOutAsync_Falha()
    {
        // Arrange
        _signInManager.Setup(s => s.SignOutAsync()).ThrowsAsync(new System.Exception("Erro ao sair"));

        // Act & Assert
        await Assert.ThrowsExceptionAsync<System.Exception>(async () =>
        {
            await _handler.Handle(new SairCommand(), CancellationToken.None);
        });
        _signInManager.Verify(s => s.SignOutAsync(), Times.Once);
    }
}
