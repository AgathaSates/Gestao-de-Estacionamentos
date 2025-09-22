using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao.Commands;
using Gestao_de_Estacionamentos.Core.Aplicacao.ModuloAutenticacao.Handlers;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Microsoft.AspNetCore.Identity;
using Moq;

[TestClass]
[TestCategory("Teste de Unidade de RegistrarUsuarioCommandHandler")]
public class RegistrarUsuarioCommandHandlerTests
{
    private UserManager<Usuario> _userManager;
    private Mock<IUserPasswordStore<Usuario>> _userStore;
    private Mock<ITokenProvider> _tokenProvider;
    private RegistrarUsuarioCommandHandler _handler;

    [TestInitialize]
    public void Setup()
    {
        _userStore = new Mock<IUserPasswordStore<Usuario>>();
        var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
        options.Setup(o => o.Value).Returns(new IdentityOptions());
        var passwordHasher = new Mock<IPasswordHasher<Usuario>>();
        var userValidators = new List<IUserValidator<Usuario>>();
        var passwordValidators = new List<IPasswordValidator<Usuario>>();
        var keyNormalizer = new Mock<ILookupNormalizer>();
        var errors = new Mock<IdentityErrorDescriber>();
        var services = new Mock<IServiceProvider>();
        var loggerUserManager = new Mock<Microsoft.Extensions.Logging.ILogger<UserManager<Usuario>>>();

        _userManager = new UserManager<Usuario>(
            _userStore.Object,
            options.Object,
            passwordHasher.Object,
            userValidators,
            passwordValidators,
            keyNormalizer.Object,
            errors.Object,
            services.Object,
            loggerUserManager.Object
        );

        _tokenProvider = new Mock<ITokenProvider>();
        _handler = new RegistrarUsuarioCommandHandler(
            _userManager,
            _tokenProvider.Object
        );
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Confirmacao_De_Senha_Eh_Invalida()
    {
        // Arrange
        var command = new RegistrarUsuarioCommand("nome", "email@teste.com", "senha123", "senhaErrada");
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailed);
        Assert.AreEqual("Requisição inválida", result.Errors[0].Message);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Criacao_De_Usuario_Falha_Com_Erros_Padrao()
    {
        // Arrange
        var command = new RegistrarUsuarioCommand("nome", "email@teste.com", "senha123", "senha123");
        var identityErrors = new List<IdentityError>
        {
            new IdentityError { Code = "DuplicateUserName", Description = "Usuário já existe" },
            new IdentityError { Code = "PasswordTooShort", Description = "Senha curta" }
        };
        var identityResult = IdentityResult.Failed(identityErrors.ToArray());

        _userStore.Setup(u => u.CreateAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(identityResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual("Requisição inválida", result.Errors[0].Message);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_Criacao_De_Usuario_Falha_Com_Erro_Desconhecido()
    {
        // Arrange
        var command = new RegistrarUsuarioCommand("nome", "email@teste.com", "senha123", "senha123");
        var identityErrors = new List<IdentityError>
        {
            new IdentityError { Code = "CustomError", Description = "Erro customizado" }
        };
        var identityResult = IdentityResult.Failed(identityErrors.ToArray());

        _userStore.Setup(u => u.CreateAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(identityResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual("Requisição inválida", result.Errors[0].Message);
    }

    [TestMethod]
    public async Task Deve_Falhar_Quando_GerarAccessToken_Retorna_Nulo()
    {
        // Arrange
        var command = new RegistrarUsuarioCommand("nome", "email@teste.com", "senha123", "senha123");
        var identityResult = IdentityResult.Success;
        _userStore.Setup(u => u.CreateAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(identityResult);
        _tokenProvider.Setup(t => t.GerarAccessToken(It.IsAny<Usuario>())).Returns((AccessToken)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.AreEqual("Ocorreu um erro interno do servidor", result.Errors[0].Message);
    }

    [TestMethod]
    public async Task Deve_Registrar_Usuario_Com_Sucesso()
    {
        // Arrange
        var command = new RegistrarUsuarioCommand("nome", "email@teste.com", "senha123", "senha123");
        var usuario = new Usuario
        {
            FullName = command.NomeCompleto,
            UserName = command.Email,
            Email = command.Email
        };
        var identityResult = IdentityResult.Success;
        var accessToken = new AccessToken("token", DateTime.UtcNow.AddSeconds(3600), new UsuarioAutenticado(usuario.Id, usuario.UserName, usuario.Email));

        _userStore.Setup(u => u.CreateAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(identityResult);
        _tokenProvider.Setup(t => t.GerarAccessToken(It.IsAny<Usuario>())).Returns(accessToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(accessToken, result.Value);
    }
}