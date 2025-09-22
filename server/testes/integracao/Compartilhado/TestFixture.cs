using DotNet.Testcontainers.Containers;
using FizzWare.NBuilder;
using Gestao_de_Estacionamento.Infraestrutura.Conf;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Gestao_de_Estacionamentos.Infraestutura.Orm.Compartilhado;
using Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloFatura;
using Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloRecepcao;
using Testcontainers.PostgreSql;

namespace Gestao_de_Estacionamentos.Testes.Integracao.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    protected AppDbContext? _dbContext;
    
    protected RepositorioRecepcaoEmOrm? _repositorioRecepcao;
    protected RepositorioEstacionamentoEmOrm? _repositorioEstacionamento;
    protected RepositorioFaturaEmOrm? _repositorioFatura;
    protected RepositorioRelatorioEmOrm? _repositorioRelatorio;

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
        _repositorioEstacionamento = new RepositorioEstacionamentoEmOrm(_dbContext);
        _repositorioFatura = new RepositorioFaturaEmOrm(_dbContext);
        _repositorioRelatorio = new RepositorioRelatorioEmOrm(_dbContext);

        BuilderSetup.SetCreatePersistenceMethod<CheckIn>(checkIn =>
       _repositorioRecepcao.CadastrarAsync(checkIn).GetAwaiter().GetResult());

        BuilderSetup.SetCreatePersistenceMethod<IList<Vaga>>(vagas =>
            _repositorioEstacionamento.CadastrarEntidadesAsync(vagas).GetAwaiter().GetResult());

        BuilderSetup.SetCreatePersistenceMethod<Fatura>(fatura =>
        _repositorioFatura.CadastrarAsync(fatura).GetAwaiter().GetResult());

        BuilderSetup.SetCreatePersistenceMethod<Relatorio>(relatorio =>
        _repositorioRelatorio.CadastrarAsync(relatorio).GetAwaiter().GetResult());
    }

    private static void ConfigurarTabelas(AppDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();
    
        dbContext.CheckIns.RemoveRange(dbContext.CheckIns);
        dbContext.Veiculos.RemoveRange(dbContext.Veiculos);
        dbContext.Tickets.RemoveRange(dbContext.Tickets);
        dbContext.Vagas.RemoveRange(dbContext.Vagas);
        dbContext.Faturas.RemoveRange(dbContext.Faturas);
        dbContext.Relatorio.RemoveRange(dbContext.Relatorio);

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