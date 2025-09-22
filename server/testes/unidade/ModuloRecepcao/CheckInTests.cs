using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloRecepcao;
[TestClass]
[TestCategory("Teste de Unidade de Entidade: Check-In")]
public sealed class CheckInTests
{
    [TestMethod]
    public void Deve_Criar_CheckIn_Com_Propriedades_Corretas()
    {
        // Arrange
        var veiculo = new Veiculo("ABC-1234", "ModeloX", "Vermelho", "Observacao");
        var cpf = "123.456.789-00";
        var nome = "João Silva";

        // Act
        var checkIn = new CheckIn(veiculo, cpf, nome);

        // Assert
        Assert.IsNotNull(checkIn.Id);
        Assert.AreEqual(veiculo, checkIn.Veiculo);
        Assert.AreEqual(cpf, checkIn.CPF);
        Assert.AreEqual(nome, checkIn.Nome);
        Assert.IsNull(checkIn.Ticket);
    }

    [TestMethod]
    public void Deve_Adicionar_Ticket_Ao_CheckIn()
    {
        // Arrange
        var veiculo = new Veiculo("ABC-1234", "ModeloX", "Vermelho", "Observacao");
        var checkIn = new CheckIn(veiculo, "123.456.789-00", "João Silva");
        var ticket = new Ticket(DateTime.Now);

        // Act
        checkIn.AdicionarTicket(ticket);

        // Assert
        Assert.AreEqual(ticket, checkIn.Ticket);
    }

    [TestMethod]
    public void Deve_Atualizar_Registro_Do_CheckIn()
    {
        // Arrange
        var veiculoOriginal = new Veiculo("ABC-1234", "ModeloX", "Vermelho", "Observacao");
        var checkIn = new CheckIn(veiculoOriginal, "123.456.789-00", "João Silva");

        var veiculoEditado = new Veiculo("XYZ-5678", "ModeloY", "Azul", "Nova Observacao");
        var checkInEditado = new CheckIn(veiculoEditado, "987.654.321-00", "Maria Souza");

        // Act
        checkIn.AtualizarRegistro(checkInEditado);

        // Assert
        Assert.AreEqual("XYZ-5678", checkIn.Veiculo.Placa);
        Assert.AreEqual("ModeloY", checkIn.Veiculo.Modelo);
        Assert.AreEqual("Azul", checkIn.Veiculo.Cor);
        Assert.AreEqual("Nova Observacao", checkIn.Veiculo.Observacoes);
        Assert.AreEqual("987.654.321-00", checkIn.CPF);
        Assert.AreEqual("Maria Souza", checkIn.Nome);
    }
}
