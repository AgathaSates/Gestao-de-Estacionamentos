using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao.Commands;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Microsoft.AspNetCore.Identity;
using Moq;

[TestClass]
[TestCategory("Teste de Unidade de AutenticarUsuarioCommandHandler")]
public class AutenticarUsuarioCommandHandlerTests
{
    private Mock<SignInManager<Usuario>> _signInManager;
    private Mock<UserManager<Usuario>> _userManager;
    private Mock<ITokenProvider> _tokenProvider;
    private AutenticarUsuarioCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        var userStore = new Mock<IUserStore<Usuario>>();
        var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());
        var passwordHasher = new Mock<IPasswordHasher<Usuario>>();
        var userValidators = new List<IUserValidator<Usuario>>();
        var passwordValidators = new List<IPasswordValidator<Usuario>>();
        var keyNormalizer = new Mock<ILookupNormalizer>();
        var errors = new Mock<IdentityErrorDescriber>();
        var services = new Mock<IServiceProvider>();
        var loggerUserManager = new Mock<Microsoft.Extensions.Logging.ILogger<UserManager<Usuario>>>();

        _userManager = new Mock<UserManager<Usuario>>(
            userStore.Object,
            options.Object,
            passwordHasher.Object,
            userValidators,
            passwordValidators,
            keyNormalizer.Object,
            errors.Object,
            services.Object,
            loggerUserManager.Object
        );

        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<Usuario>>();
        var loggerSignInManager = new Mock<Microsoft.Extensions.Logging.ILogger<SignInManager<Usuario>>>();
        var schemeProvider = new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>();
        var confirmation = new Mock<IUserConfirmation<Usuario>>();

        _signInManager = new Mock<SignInManager<Usuario>>(
            _userManager.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            options.Object,
            loggerSignInManager.Object,
            schemeProvider.Object,
            confirmation.Object
        );

        _tokenProvider = new Mock<ITokenProvider>();
        _handler = new AutenticarUsuarioCommandHandler(
            _signInManager.Object,
            _userManager.Object,
            _tokenProvider.Object
        );
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Usuario_Nao_Encontrado()
    {
        // Arrange
        var command = new AutenticarUsuarioCommand("email@teste.com", "senha123");
        _userManager.Setup(u => u.FindByEmailAsync(command.Email)).ReturnsAsync((Usuario)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual("Registro não encontrado", result.Errors[0].Message);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Login_Eh_Incorreto()
    {
        // Arrange
        var usuario = new Usuario { UserName = "usuario", Email = "email@teste.com" };
        var command = new AutenticarUsuarioCommand(usuario.Email, "senha123");
        _userManager.Setup(u => u.FindByEmailAsync(command.Email)).ReturnsAsync(usuario);

        var signInResult = SignInResult.Failed;
        _signInManager.Setup(s => s.PasswordSignInAsync(usuario.UserName, command.Senha, true, false)).ReturnsAsync(signInResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual("Requisição inválida", result.Errors[0].Message);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Conta_Esta_Bloqueada()
    {
        // Arrange
        var usuario = new Usuario { UserName = "usuario", Email = "email@teste.com" };
        var command = new AutenticarUsuarioCommand(usuario.Email, "senha123");
        _userManager.Setup(u => u.FindByEmailAsync(command.Email)).ReturnsAsync(usuario);

        var signInResult = SignInResult.LockedOut;
        _signInManager.Setup(s => s.PasswordSignInAsync(usuario.UserName, command.Senha, true, false)).ReturnsAsync(signInResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual("Requisição inválida", result.Errors[0].Message);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Login_Nao_E_Permitido()
    {
        // Arrange
        var usuario = new Usuario { UserName = "usuario", Email = "email@teste.com" };
        var command = new AutenticarUsuarioCommand(usuario.Email, "senha123");
        _userManager.Setup(u => u.FindByEmailAsync(command.Email)).ReturnsAsync(usuario);

        var signInResult = SignInResult.NotAllowed;
        _signInManager.Setup(s => s.PasswordSignInAsync(usuario.UserName, command.Senha, true, false)).ReturnsAsync(signInResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual("Requisição inválida", result.Errors[0].Message);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Requer_Dois_Fatores()
    {
        // Arrange
        var usuario = new Usuario { UserName = "usuario", Email = "email@teste.com" };
        var command = new AutenticarUsuarioCommand(usuario.Email, "senha123");
        _userManager.Setup(u => u.FindByEmailAsync(command.Email)).ReturnsAsync(usuario);

        var signInResult = SignInResult.TwoFactorRequired;
        _signInManager.Setup(s => s.PasswordSignInAsync(usuario.UserName, command.Senha, true, false)).ReturnsAsync(signInResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual("Requisição inválida", result.Errors[0].Message);
    }

    [TestMethod]
    public async Task Deve_Autenticar_Usuario_Com_Sucesso()
    {
        // Arrange
        var usuario = new Usuario { UserName = "usuario", Email = "email@teste.com" };
        var command = new AutenticarUsuarioCommand(usuario.Email, "senha123");
        _userManager.Setup(u => u.FindByEmailAsync(command.Email)).ReturnsAsync(usuario);

        var signInResult = SignInResult.Success;
        _signInManager.Setup(s => s.PasswordSignInAsync(usuario.UserName, command.Senha, true, false)).ReturnsAsync(signInResult);

        var usuarioAutenticado = new UsuarioAutenticado(
            usuario.Id,
            usuario.UserName,
            usuario.Email
        );

        var accessToken = new AccessToken(
            "token",
            DateTime.UtcNow.AddSeconds(3600),
            usuarioAutenticado
        );
        _tokenProvider.Setup(t => t.GerarAccessToken(usuario)).Returns(accessToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(accessToken, result.Value);
    }
}