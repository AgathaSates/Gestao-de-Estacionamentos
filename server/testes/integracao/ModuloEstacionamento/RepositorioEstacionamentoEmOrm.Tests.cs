using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Gestao_de_Estacionamentos.Testes.Integracao.Compartilhado;

namespace Gestao_de_Estacionamentos.Testes.Integracao.ModuloEstacionamento;

[TestClass]
[TestCategory("Testes de Integração de Estacionamento")]
public class RepositorioEstacionamentoEmOrm : TestFixture
{
    Veiculo veiculo = new Veiculo("TEST567", "modelo", "cor", "vidro quebrado");

    [TestMethod]
    public async Task Deve_Cadastrar_Vaga_Corretamente()
    {
        // Arrange
        Vaga vaga = new Vaga('A');

        // Act
        await _repositorioEstacionamento!.CadastrarAsync(vaga);
        await _dbContext!.SaveChangesAsync();

        Vaga? vagaEncontrada = await _repositorioEstacionamento!
            .SelecionarRegistroPorIdAsync(vaga.Id);

        // Assert
        Assert.AreEqual(vaga, vagaEncontrada);
    }

    [TestMethod]
    public async Task Deve_Cadastrar_Vagas_Corretamente()
    {
        // Arrange
        List<Vaga> vagas = new List<Vaga>
        {
            new Vaga('A'),
            new Vaga('B'),
            new Vaga('C')
        };

        // Act
        await _repositorioEstacionamento!.CadastrarEntidadesAsync(vagas);
        await _dbContext!.SaveChangesAsync();

        var vagasEsperradasOrdenadas = vagas.OrderBy(v => v.NumeroVaga).ToList();

        var vagasEcontradas = await _repositorioEstacionamento!.SelecionarRegistrosAsync();

        // Assert
        CollectionAssert.AreEquivalent(vagasEsperradasOrdenadas, vagasEcontradas);
    }

    [TestMethod]
    public async Task Deve_Adicionar_Veiculo_A_Vaga_Corretamente()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");
        Vaga vaga = new Vaga('A');

        await _repositorioRecepcao.CadastrarAsync(checkIn);
        await _repositorioEstacionamento!.CadastrarAsync(vaga);
        await _dbContext!.SaveChangesAsync();

        // Act
        _repositorioEstacionamento.AdicionarVeiculoAVaga(vaga, veiculo);
        await _dbContext.SaveChangesAsync();

        Vaga? vagaEncontrada = await _repositorioEstacionamento!
            .SelecionarRegistroPorIdAsync(vaga.Id);

        // Assert
        Assert.AreEqual(vaga, vagaEncontrada);
        Assert.AreEqual(veiculo, vagaEncontrada!.VeiculoEstacionado);
    }

    [TestMethod]
    public async Task Deve_Remover_Veiculo_Da_Vaga_Corretamente()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");
        Vaga vaga = new Vaga('A');
        vaga.AdicionarVeiculo(veiculo);

        await _repositorioRecepcao.CadastrarAsync(checkIn);
        await _repositorioEstacionamento!.CadastrarAsync(vaga);
        await _dbContext!.SaveChangesAsync();

        // Act
        _repositorioEstacionamento.RemoverVeiculoDaVaga(vaga);
        await _dbContext.SaveChangesAsync();
        Vaga? vagaEncontrada = await _repositorioEstacionamento!
            .SelecionarRegistroPorIdAsync(vaga.Id);

        // Assert
        Assert.AreEqual(vaga, vagaEncontrada);
        Assert.IsNull(vagaEncontrada!.VeiculoEstacionado);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Vaga_Por_Id_Corretamente()
    {
        // Arrange
        Vaga vaga = new Vaga('A');

        await _repositorioEstacionamento!.CadastrarAsync(vaga);
        await _dbContext!.SaveChangesAsync();

        // Act
        Vaga? vagaEncontrada = await _repositorioEstacionamento!
            .SelecionarRegistroPorIdAsync(vaga.Id);

        // Assert
        Assert.AreEqual(vaga, vagaEncontrada);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Vaga_Por_Numero_Corretamente()
    {
        // Arrange
        Vaga vaga = new Vaga('A');
        await _repositorioEstacionamento!.CadastrarAsync(vaga);
        await _dbContext!.SaveChangesAsync();

        // Act
        Vaga? vagaEncontrada = await _repositorioEstacionamento!
            .SelecionarPorNumeroDaVaga(vaga.NumeroVaga);

        // Assert
        Assert.AreEqual(vaga, vagaEncontrada);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Vaga_Por_Placa_Do_Veiculo_Estacionado_Corretamente()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");
        Vaga vaga = new Vaga('A');
        vaga.AdicionarVeiculo(veiculo);

        await _repositorioRecepcao.CadastrarAsync(checkIn);
        await _repositorioEstacionamento!.CadastrarAsync(vaga);
        await _dbContext!.SaveChangesAsync();

        // Act

        Vaga? vagaEncontrada = await _repositorioEstacionamento!
            .SelecionarPorPlacaDoVeiculo(veiculo.Placa);

        // Assert
        Assert.AreEqual(vaga, vagaEncontrada);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Veiculo_Por_Placa_Corretamente()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");
        Vaga vaga = new Vaga('A');
        vaga.AdicionarVeiculo(veiculo);

        await _repositorioRecepcao.CadastrarAsync(checkIn);
        await _repositorioEstacionamento!.CadastrarAsync(vaga);
        await _dbContext!.SaveChangesAsync();
        // Act

        Veiculo? veiculoEncontrado = await _repositorioEstacionamento!
            .SelecionarVeiculoPorPlaca(veiculo.Placa);

        // Assert
        Assert.AreEqual(veiculo, veiculoEncontrado);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Veiculo_Por_Ticket_Corretamente()
    {
        // Arrange
        CheckIn checkIn = new CheckIn(veiculo, "11122233345", "Juninho");
        Vaga vaga = new Vaga('A');

        await _repositorioRecepcao.CadastrarAsync(checkIn);
        await _repositorioEstacionamento!.CadastrarAsync(vaga);
        _repositorioEstacionamento.AdicionarVeiculoAVaga(vaga, veiculo);
        await _dbContext!.SaveChangesAsync();
        // Act
        Veiculo? veiculoEncontrado = await _repositorioEstacionamento!
            .SelecionarVeiculoPorTicket(veiculo.CheckIn.Ticket.NumeroSequencial);

        // Assert
        Assert.AreEqual(veiculo, veiculoEncontrado);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Todos_Os_Veiculos_Estacionados_Corretamente()
    {
        // Arrange
        Veiculo veiculo2 = new Veiculo("OUTRA12", "modelo2", "cor2");
        Veiculo veiculo3 = new Veiculo("VEIC789", "modelo3", "cor3", "pneu furado");

        CheckIn checkIn1 = new CheckIn(veiculo, "11122233345", "Juninho");
        CheckIn checkIn2 = new CheckIn(veiculo2, "99988877766", "Zezinho");
        CheckIn checkIn3 = new CheckIn(veiculo3, "55566677788", "Maria");

        Vaga vaga1 = new Vaga('A');
        Vaga vaga2 = new Vaga('B');
        Vaga vaga3 = new Vaga('C');

        var checkIns = new List<CheckIn> { checkIn1, checkIn2, checkIn3 };
        var vagas = new List<Vaga> { vaga1, vaga2, vaga3 };

        await _repositorioRecepcao!.CadastrarEntidadesAsync(checkIns);
        await _repositorioEstacionamento!.CadastrarEntidadesAsync(vagas);

        _repositorioEstacionamento.AdicionarVeiculoAVaga(vaga1, veiculo);
        _repositorioEstacionamento.AdicionarVeiculoAVaga(vaga2, veiculo2);
        _repositorioEstacionamento.AdicionarVeiculoAVaga(vaga3, veiculo3);

        await _dbContext!.SaveChangesAsync();
        List<Veiculo> veiculosEsperados = new List<Veiculo> { veiculo, veiculo2, veiculo3 }
            .OrderBy(v => v.Placa).ToList();

        // Act
        List<Veiculo> veiculosEncontrados = await _repositorioEstacionamento!
            .SelecionaTodosOsVeiculosEstacionados();

        // Assert
        CollectionAssert.AreEquivalent(veiculosEsperados, veiculosEncontrados);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Todas_As_Vagas_Corretamente()
    {
        // Arrange
        Vaga vaga1 = new Vaga('A');
        Vaga vaga2 = new Vaga('B');
        Vaga vaga3 = new Vaga('C');

        List<Vaga> vagas = new List<Vaga> { vaga1, vaga2, vaga3 };

        await _repositorioEstacionamento!.CadastrarEntidadesAsync(vagas);
        await _dbContext!.SaveChangesAsync();

        var vagasEsperradasOrdenadas = vagas.OrderBy(v => v.NumeroVaga).ToList();
        // Act

        var vagasEcontradas = await _repositorioEstacionamento!.SelecionarRegistrosAsync();
        // Assert

        CollectionAssert.AreEquivalent(vagasEsperradasOrdenadas, vagasEcontradas);
    }

    [TestMethod]
    public async Task Deve_Selecionar_Todas_As_vagas_Por_Quantidade_Corretamente()
    {
        // Arrange
        Vaga vaga1 = new Vaga('A');
        Vaga vaga2 = new Vaga('B');
        Vaga vaga3 = new Vaga('C');

        List<Vaga> vagas = new List<Vaga> { vaga1, vaga2, vaga3 };

        await _repositorioEstacionamento!.CadastrarEntidadesAsync(vagas);
        await _dbContext!.SaveChangesAsync();

        var vagasEsperradasOrdenadas = vagas.OrderBy(v => v.NumeroVaga).ToList();

        // Act
        var vagasEcontradas = await _repositorioEstacionamento!
            .SelecionarRegistrosAsync(2);

        // Assert
        CollectionAssert.AreEquivalent(vagasEsperradasOrdenadas.Take(2).ToList(), vagasEcontradas);
    }
}
