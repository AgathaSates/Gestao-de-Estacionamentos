using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;

namespace Gestao_de_Estacionamentos.Testes.Unidade.ModuloRecepcao;
[TestClass]
[TestCategory("Teste de Unidade de Entidade: Ticket")]
public sealed class TicketTests
{
    [TestMethod]
    public void Deve_Criar_Ticket_Com_Propriedades_Corretas()
    {
        // Arrange
        DateTime dataHoraEntrada = DateTime.Now;

        // Act
        Ticket ticket = new Ticket(dataHoraEntrada);

        // Assert
        Assert.AreNotEqual(Guid.Empty, ticket.Id);
        Assert.AreEqual(dataHoraEntrada, ticket.DataHoraEntrada);
        Assert.IsNull(ticket.DataHoraSaida);
        Assert.AreEqual(StatusTicket.Aberto, ticket.StatusTicket);
    }

    [TestMethod]
    public void Deve_Registrar_Saida_Corretamente()
    {
        // Arrange
        DateTime dataHoraEntrada = DateTime.Now.AddHours(-2);
        DateTime dataHoraSaida = DateTime.Now;

        Ticket ticket = new Ticket(dataHoraEntrada);

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
        DateTime dataHoraEntrada = DateTime.Now;
        Ticket ticket = new Ticket(dataHoraEntrada);

        var checkIn = new CheckIn();

        // Act
        ticket.AdicionarCheckIn(checkIn);

        // Assert
        Assert.AreEqual(checkIn, ticket.CheckIn);
    }

    [TestMethod]
    public void Deve_Atualizar_Registro_Corretamente()
    {
        // Arrange
        DateTime dataHoraEntradaOriginal = DateTime.Now.AddHours(-3);
        DateTime dataHoraSaidaOriginal = DateTime.Now.AddHours(-1);
        Ticket ticketOriginal = new Ticket(dataHoraEntradaOriginal);
        ticketOriginal.RegistrarSaida(dataHoraSaidaOriginal);

        DateTime dataHoraEntradaEditada = DateTime.Now.AddHours(-2);
        DateTime dataHoraSaidaEditada = DateTime.Now;
        Ticket ticketEditado = new Ticket(dataHoraEntradaEditada);
        ticketEditado.RegistrarSaida(dataHoraSaidaEditada);

        // Act
        ticketOriginal.AtualizarRegistro(ticketEditado);

        // Assert
        Assert.AreEqual(dataHoraEntradaEditada, ticketOriginal.DataHoraEntrada);
        Assert.AreEqual(dataHoraSaidaEditada, ticketOriginal.DataHoraSaida);
        Assert.AreEqual(StatusTicket.Fechado, ticketOriginal.StatusTicket);
    }
}
