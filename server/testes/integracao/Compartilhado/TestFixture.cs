using DotNet.Testcontainers.Containers;
using FizzWare.NBuilder;
using Testcontainers.PostgreSql;

namespace Gestao_de_Estacionamentos.Testes.Integracao.Compartilhado;

[TestClass]
public abstract class TestFixture
{
    // colocar dbcontext
    // colocar repositorios

    private static IDatabaseContainer? _container;

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

        // Configurar dbcontext e repositorios
    }

    private static void ConfigurarTabelas()// passar de parametro o dbcontext
    {
        // criar tabelas 
        // remover dados das tabelas
        // salvar mudanças
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