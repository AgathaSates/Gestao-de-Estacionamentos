using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Gestao_de_Estacionamentos.Testes.Integracao.Compartilhado;

namespace Gestao_de_Estacionamentos.Testes.Integracao.ModuloRecepcao;

[TestClass]
[TestCategory("Testes de Integração de Recepção")]
public class RepositorioRecepcaoEmOrm : TestFixture
{
    Veiculo veiculo = new Veiculo( "TEST567", "modelo", "cor", "vidro quebrado");
    Veiculo veiculo2 = new Veiculo("OUTRA12", "modelo2", "cor2");
    Veiculo veiculo3 = new Veiculo("VEIC789", "modelo3", "cor3", "pneu furado");

    [TestMethod]
    public async Task Deve_Cadastrar_CheckIn_Corretamente()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");

        // Act
        await _repositorioRecepcao!.CadastrarAsync(checkIn);
        await _dbContext!.SaveChangesAsync(); 

        CheckIn? checkInEncontrado = await _repositorioRecepcao!
            .SelecionarRegistroPorIdAsync(checkIn.Id);

        // Assert
        Assert.AreEqual(checkIn, checkInEncontrado);
    }

    [TestMethod]
    public async Task Deve_Editar_CheckIn_Corretamente()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");

        await _repositorioRecepcao!.CadastrarAsync(checkIn);
        await _dbContext!.SaveChangesAsync();

        CheckIn checkInEditado = new CheckIn(veiculo2, "99988877766", "Zezinho");

        //Act
        await _repositorioRecepcao!.EditarAsync(checkIn.Id, checkInEditado);
        await _dbContext.SaveChangesAsync();

        CheckIn? checkInEncontrado = await _repositorioRecepcao!
            .SelecionarRegistroPorIdAsync(checkIn.Id);

        //Asset
        Assert.AreEqual(checkIn, checkInEncontrado);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Por_Id_Corretamente()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");
        await _repositorioRecepcao!.CadastrarAsync(checkIn);
        await _dbContext!.SaveChangesAsync();

        // Act
        CheckIn? checkInEncontrado = await _repositorioRecepcao!
            .SelecionarRegistroPorIdAsync(checkIn.Id);

        // Assert
        Assert.AreEqual(checkIn, checkInEncontrado);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Por_Id_Retornar_Nulo_Quando_Nao_Encontrar()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");

        await _repositorioRecepcao!.CadastrarAsync(checkIn);
        await _dbContext!.SaveChangesAsync();

        // Act
        CheckIn? checkInEncontrado = await _repositorioRecepcao!
            .SelecionarRegistroPorIdAsync(Guid.NewGuid());

        // Assert
        Assert.IsNull(checkInEncontrado);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Todos_Corretamente()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");
        CheckIn checkIn2 = new CheckIn(veiculo2, "99988877766", "Zezinho");
        CheckIn checkIn3 = new CheckIn(veiculo3, "55566677788", "Mariazinha");

        List<CheckIn> checkInsEsperados = new List<CheckIn> { checkIn, checkIn2, checkIn3 };
        await _repositorioRecepcao!.CadastrarEntidadesAsync(checkInsEsperados);
        await _dbContext!.SaveChangesAsync();

        var checkInsEsperadosOrdenados = checkInsEsperados
            .OrderBy(c => c.Veiculo.Placa)
            .ToList();

        // Act
        var checkInsEncontrados = await _repositorioRecepcao!
            .SelecionarRegistrosAsync();

        // Assert
        CollectionAssert.AreEqual(checkInsEsperadosOrdenados, checkInsEncontrados);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Todos_Por_Quantidade_Corretamente()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");
        CheckIn checkIn2 = new CheckIn(veiculo2, "99988877766", "Zezinho");
        CheckIn checkIn3 = new CheckIn(veiculo3, "55566677788", "Mariazinha");

        List<CheckIn> checkInsEsperados = new List<CheckIn> { checkIn, checkIn2, checkIn3 };

        await _repositorioRecepcao!.CadastrarEntidadesAsync(checkInsEsperados);
        await _dbContext!.SaveChangesAsync();

        var checkInsEsperadosOrdenados = checkInsEsperados
            .OrderBy(c => c.Veiculo.Placa)
            .Take(2)
            .ToList();

        // Act
        var checkInsEncontrados = await _repositorioRecepcao!
            .SelecionarRegistrosAsync(2);

        // Assert
        CollectionAssert.AreEqual(checkInsEsperadosOrdenados, checkInsEncontrados);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Veiculo_Por_Placa_Corretamente()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");

        await _repositorioRecepcao!.CadastrarAsync(checkIn);
        await _dbContext!.SaveChangesAsync();

        // Act
        var veiculoEncontrado = await _repositorioRecepcao!
            .SelecionarVeiculoPorPlaca(veiculo.Placa);

        // Assert
        Assert.AreEqual(veiculo, veiculoEncontrado);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Veiculo_Por_Placa_Retornar_Nulo_Quando_Nao_Encontrar()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");

        await _repositorioRecepcao!.CadastrarAsync(checkIn);
        await _dbContext!.SaveChangesAsync();

        // Act
        var veiculoEncontrado = await _repositorioRecepcao!
            .SelecionarVeiculoPorPlaca("PLACA123");

        // Assert
        Assert.IsNull(veiculoEncontrado);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Veiculo_Por_Numero_Do_Ticket_Corretamente()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");

        await _repositorioRecepcao!.CadastrarAsync(checkIn);
        await _dbContext!.SaveChangesAsync();

        // Act
        var veiculoEncontrado = await _repositorioRecepcao!
            .SelecionarVeiculoPorTicket(checkIn.Ticket.NumeroSequencial);

        // Assert
        Assert.AreEqual(veiculo, veiculoEncontrado);
    }
}