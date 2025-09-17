using DotNet.Testcontainers.Containers;
using FizzWare.NBuilder;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Infraestutura.Orm.Compartilhado;
using Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloRecepcao;
using Testcontainers.PostgreSql;

namespace Gestao_de_Estacionamentos.Testes.Integracao.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected AppDbContext? _dbContext;
    
    protected RepositorioRecepcaoEmOrm? _repositorioRecepcao;


    protected static IDatabaseContainer? _container;

    [AssemblyInitialize]
    public static async Task Setup(TestContext _)
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:16")
            .WithName("gestao-de-estacionamento-testes")
            .WithDatabase("gestao-de-estacionamento-testes")
            .WithUsername("postgres")
            .WithPassword("YourStrongPassword")
            .WithCleanUp(true)
            .Build();

        await InicializarBancoDadosAsync(_container);
    }

    [AssemblyCleanup]
    public static async Task Teardown()
    {
        await EncerrarBancoDadosAsync();
    }

    [TestInitialize]
    public void ConfigurarTestes()
    {
        if (_container is null)
            throw new ArgumentNullException("O banco de dados não foi inicializado.");

        _dbContext = AppDbContextFactory.CriarDbContext(_container.GetConnectionString());

        ConfigurarTabelas(_dbContext);

        _repositorioRecepcao = new RepositorioRecepcaoEmOrm(_dbContext);

        BuilderSetup.SetCreatePersistenceMethod<CheckIn>(async (CheckIn novoRegistro) =>
        {
              await _repositorioRecepcao.CadastrarAsync(novoRegistro);
        });

        BuilderSetup.SetCreatePersistenceMethod<List<CheckIn>>(async (List<CheckIn> novosRegistros) =>
        {
              await _repositorioRecepcao.CadastrarEntidadesAsync(novosRegistros);
        });
    }

    private static void ConfigurarTabelas(AppDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();
    
        dbContext.CheckIns.RemoveRange(dbContext.CheckIns);
        dbContext.Veiculos.RemoveRange(dbContext.Veiculos);
        dbContext.Tickets.RemoveRange(dbContext.Tickets);

        dbContext.SaveChanges();
    }

    private static async Task InicializarBancoDadosAsync(IDatabaseContainer container)
    {
        await container.StartAsync();
    }

    private static async Task EncerrarBancoDadosAsync()
    {
        if (_container is null)
            throw new ArgumentNullException("O Banco de dados não foi inicializado.");
        
        await _container.StopAsync();
        await _container.DisposeAsync();
    }
}