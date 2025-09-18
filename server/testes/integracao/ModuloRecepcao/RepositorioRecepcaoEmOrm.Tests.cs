using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Gestao_de_Estacionamentos.Infraestutura.Orm.Compartilhado;
using Gestao_de_Estacionamentos.Testes.Integracao.Compartilhado;
using System.Threading.Tasks;

namespace Gestao_de_Estacionamentos.Testes.Integracao.ModuloRecepcao;

[TestClass]
[TestCategory("Testes de Integração de Recepção")]
public class RepositorioRecepcaoEmOrm : TestFixture
{

    [TestMethod]
    public void Deve_Cadastrar_CheckIn_Corretamente()
    {
        //Arrange
        var veiculo = Builder<Veiculo>.CreateNew().With(v => v.Id = Guid.NewGuid()).Build();
        var checkIn = new CheckIn(veiculo, "12345678900", "João Silva");

        //Act

        _repositorioRecepcao!.CadastrarAsync(checkIn).Wait();
        _dbContext!.SaveChanges();

        //Assert
        var checkInEncontrado = _repositorioRecepcao.SelecionarRegistroPorIdAsync(checkIn.Id).Result;

        Assert.IsNotNull(checkInEncontrado);
        Assert.AreEqual(checkIn, checkInEncontrado);
    }

    [TestMethod]
    public void Deve_Editar_CheckIn_Corretamente()
    {
        //Arrange
        var veiculo = Builder<Veiculo>.CreateNew().With(v => v.Id = Guid.NewGuid()).Build();
        var checkIn = new CheckIn(veiculo, "12345678900", "João Silva");

        _repositorioRecepcao!.CadastrarAsync(checkIn).Wait();
        _dbContext!.SaveChanges();

        //Act
        checkIn.Nome = "Maria Souza";
        _repositorioRecepcao.EditarAsync(checkIn.Id, checkIn).Wait();
        _dbContext.SaveChanges();

        //Assert
        var checkInEncontrado = _repositorioRecepcao.SelecionarRegistroPorIdAsync(checkIn.Id).Result;
        Assert.IsNotNull(checkInEncontrado);
        Assert.AreEqual("Maria Souza", checkInEncontrado.Nome);
    }

    [TestMethod]
    public void Deve_Selecionar_CheckIn_Por_Id_Corretamente()
    {
        //Arrange
        var veiculo = Builder<Veiculo>.CreateNew().With(v => v.Id = Guid.NewGuid()).Build();
        var checkIn = new CheckIn(veiculo, "12345678900", "João Silva");

        _repositorioRecepcao!.CadastrarAsync(checkIn).Wait();
        _dbContext!.SaveChanges();

        //Act
        var checkInEncontrado = _repositorioRecepcao.SelecionarRegistroPorIdAsync(checkIn.Id).Result;

        //Assert
        Assert.IsNotNull(checkInEncontrado);
        Assert.AreEqual(checkIn, checkInEncontrado);
    }

    [TestMethod]
    public void Deve_Retornar_Null_Ao_Selecionar_CheckIn_Por_Id_Inexistente()
    {
        //Arrange
        var idInexistente = Guid.NewGuid();

        //Act
        var generoEncontrado = _repositorioRecepcao!.SelecionarRegistroPorIdAsync(idInexistente).Result;

        //Assert
        Assert.IsNull(generoEncontrado);
    }

    [TestMethod]
    public void Deve_Selecionar_Todos_os_CheckIn_Corretamente()
    {
        //Arrange
        var veiculos = Builder<Veiculo>.CreateListOfSize(3)
             .All()
             .With(v => v.Id = Guid.NewGuid())
             .Build()
             .ToList();

        var checkInsCriados = new List<CheckIn>();

        foreach (var veiculo in veiculos)
        {
            var checkIn = new CheckIn(veiculo, "12345678900", "João Silva");

            checkInsCriados.Add(checkIn);
        }

        checkInsCriados.OrderBy(c => c.Veiculo.Placa).ToList();

        _repositorioRecepcao!.CadastrarEntidadesAsync(checkInsCriados).Wait();
        _dbContext!.SaveChanges();

        //Act
        var checkInsEncontrados = _repositorioRecepcao.SelecionarRegistrosAsync().Result;

        //Assert
        CollectionAssert.AreEqual(checkInsCriados, checkInsEncontrados);
    }

    [TestMethod]
    public async Task Deve_Persistir_Veiculo_Quando_Cadastrar()
    {
        //Arrange
        var veiculo = Builder<Veiculo>.CreateNew().With(v => v.Id = Guid.NewGuid()).Build();
        var checkIn = new CheckIn(veiculo, "12345678900", "João Silva");

        //Act
        _repositorioRecepcao!.CadastrarAsync(checkIn).Wait();
        _dbContext!.SaveChanges();

        using var freshDbContext = AppDbContextFactory.CriarDbContext(_container!.GetConnectionString());

        var veiculoExiste = await freshDbContext.Veiculos
            .AsNoTracking()
            .AnyAsync(v => v.Id == veiculo.Id);

        var checkInExiste = await freshDbContext.CheckIns
            .AsNoTracking()
            .AnyAsync(c => c.Id == checkIn.Id);

        // Assert existência nas TABELAS
        Assert.IsTrue(checkInExiste);
        Assert.IsTrue(veiculoExiste);
    }
}