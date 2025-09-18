using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloRecepcao;

[TestClass]
[TestCategory("Teste de Unidade da Entidade: Check-In")]
public class CheckInTests
{
    [TestMethod]
    public void Deve_Criar_CheckIn_Corretamente()
    {
        // Arrange
        var veiculo = new Veiculo("ABC1234", "Carro", "Vermelho");
        var cpf = "123.456.789-00";
        var nome = "João Silva";

        // Act
        var checkIn = new CheckIn(veiculo, cpf, nome);

        // Assert
        Assert.IsNotNull(checkIn);
        Assert.AreEqual(veiculo, checkIn.Veiculo);
        Assert.AreEqual(cpf, checkIn.CPF);
        Assert.AreEqual(nome, checkIn.Nome);
        Assert.IsNotNull(checkIn.Id);
    }

    [TestMethod]
    public void Deve_Atualizar_CheckIn_Corretamente()
    {
        // Arrange
        var veiculoOriginal = new Veiculo("ABC1234", "Carro", "Vermelho");
        var checkIn = new CheckIn(veiculoOriginal, "123.456.789-00", "João Silva");
        var ticketOriginal = new Ticket(DateTime.Now);
        checkIn.AdicionarTicket(ticketOriginal);

        var veiculoNovo = new Veiculo("XYZ5678", "Moto", "Azul");
        var cpfNovo = "987.654.321-00";
        var nomeNovo = "Maria Oliveira";

        var checkInEditado = new CheckIn(veiculoNovo, cpfNovo, nomeNovo);

        // Act
        checkIn.AtualizarRegistro(checkInEditado);

        // Assert
        Assert.AreEqual(veiculoNovo.Placa, checkIn.Veiculo.Placa);
        Assert.AreEqual(veiculoNovo.Modelo, checkIn.Veiculo.Modelo);
        Assert.AreEqual(veiculoNovo.Cor, checkIn.Veiculo.Cor);
        Assert.AreEqual(veiculoNovo.Observacoes, checkIn.Veiculo.Observacoes);
        Assert.AreEqual(cpfNovo, checkIn.CPF);
        Assert.AreEqual(nomeNovo, checkIn.Nome);
        Assert.AreEqual(checkIn.Ticket, ticketOriginal);
    }

    [TestMethod]
    public void Deve_Adicionar_Ticket_Ao_CheckIn_Corretamente()
    {
        // Arrange
        var veiculo = new Veiculo("ABC1234", "Carro", "Vermelho");
        var checkIn = new CheckIn(veiculo, "123.456.789-00", "João Silva");
        var ticket = new Ticket(DateTime.Now);

        // Act
        checkIn.AdicionarTicket(ticket);

        // Assert
        Assert.IsNotNull(checkIn.Ticket);
        Assert.AreEqual(ticket, checkIn.Ticket);
    }

    [TestMethod]
    public void Deve_Ter_Ticket_E_Veiculo_Associados_Corretamente()
    {
        // Arrange
        var veiculo = new Veiculo("ABC1234", "Carro", "Vermelho");
        var checkIn = new CheckIn(veiculo, "123.456.789-00", "João Silva");
        var ticket = new Ticket(DateTime.Now);

        // Act
        checkIn.AdicionarTicket(ticket);

        // Assert
        Assert.AreEqual(veiculo, checkIn.Veiculo);
        Assert.AreEqual(ticket, checkIn.Ticket);
    }
}