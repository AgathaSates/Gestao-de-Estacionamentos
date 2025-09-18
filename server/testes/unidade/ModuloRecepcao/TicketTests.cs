using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloRecepcao;

[TestClass]
[TestCategory("Teste de Unidade da Entidade:Ticket")]
public class TicketTests
{
    [TestMethod]
    public void Deve_Criar_Ticket_Corretamente()
    {
        // Arrange
        var dataHoraEntrada = DateTime.Now;

        // Act
        var ticket = new Ticket(dataHoraEntrada);

        // Assert
        Assert.IsNotNull(ticket);
        Assert.AreEqual(dataHoraEntrada, ticket.DataHoraEntrada);
        Assert.AreEqual(StatusTicket.Aberto, ticket.StatusTicket);
        Assert.IsNull(ticket.DataHoraSaida);
        Assert.IsNotNull(ticket.Id);
    }

    [TestMethod]
    public void Deve_Registrar_Saida_Corretamente()
    {
        // Arrange
        var ticket = new Ticket(DateTime.Now);
        var dataHoraSaida = DateTime.Now.AddHours(2);

        // Act
        ticket.RegistrarSaida(dataHoraSaida);

        // Assert
        Assert.AreEqual(dataHoraSaida, ticket.DataHoraSaida);
        Assert.AreEqual(StatusTicket.Fechado, ticket.StatusTicket);
    }

    [TestMethod]
    public void Deve_Adicionar_CheckIn_Corretamente()
    {
        // Arrange
        var ticket = new Ticket(DateTime.Now);
        var veiculo = new Veiculo("ABC1234", "Carro", "Vermelho");
        var checkIn = new CheckIn(veiculo, "123.456.789-00", "João Silva");

        // Act
        ticket.AdicionarCheckIn(checkIn);

        // Assert
        Assert.AreEqual(checkIn, ticket.CheckIn);
        Assert.AreEqual(veiculo, ticket.Veiculo);
    }

    [TestMethod]
    public void Deve_Atualizar_Ticket_Corretamente()
    {
        // Arrange
        var ticket = new Ticket(DateTime.Now);
        var ticketEditado = new Ticket(DateTime.Now.AddHours(1))
        {
            DataHoraSaida = DateTime.Now.AddHours(3),
            StatusTicket = StatusTicket.Fechado
        };

        // Act
        ticket.AtualizarRegistro(ticketEditado);

        // Assert
        Assert.AreEqual(ticketEditado.DataHoraEntrada, ticket.DataHoraEntrada);
        Assert.AreEqual(ticketEditado.DataHoraSaida, ticket.DataHoraSaida);
        Assert.AreEqual(StatusTicket.Fechado, ticket.StatusTicket);
    }

    [TestMethod]
    public void Deve_Estar_Com_Status_Aberto_Apos_Criacao()
    {
        // Arrange & Act
        var ticket = new Ticket(DateTime.Now);

        // Assert
        Assert.AreEqual(StatusTicket.Aberto, ticket.StatusTicket);
    }

    [TestMethod]
    public void Deve_Ter_DataHoraSaida_Nula_Apos_Criacao()
    {
        // Arrange & Act
        var ticket = new Ticket(DateTime.Now);

        // Assert
        Assert.IsNull(ticket.DataHoraSaida);
    }

    [TestMethod]
    public void Deve_Estar_Com_Status_Fechado_Apos_Registrar_Saida()
    {
        // Arrange
        var ticket = new Ticket(DateTime.Now);

        // Act
        ticket.RegistrarSaida(DateTime.Now.AddHours(2));

        // Assert
        Assert.AreEqual(StatusTicket.Fechado, ticket.StatusTicket);
    }

    [TestMethod]
    public void Deve_Ter_DataHoraSaida_Definida_Apos_Registrar_Saida()
    {
        // Arrange
        var ticket = new Ticket(DateTime.Now);
        var dataHoraSaida = DateTime.Now.AddHours(2);

        // Act
        ticket.RegistrarSaida(dataHoraSaida);

        // Assert
        Assert.AreEqual(dataHoraSaida, ticket.DataHoraSaida);
    }
}
