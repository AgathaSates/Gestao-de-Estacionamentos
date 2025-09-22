using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;

namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;
public class Ticket : EntidadeBase<Ticket>
{
    public int NumeroSequencial { get; set; }
    public DateTime DataHoraEntrada { get; set; }
    public DateTime? DataHoraSaida { get; set; }
    public StatusTicket StatusTicket { get; set; }
    public Guid CheckInId { get; set; }
    public CheckIn CheckIn { get; set; }

    [NotMapped]
    public Veiculo Veiculo => CheckIn?.Veiculo;

    [ExcludeFromCodeCoverage]
    public Ticket() { }

    public Ticket(DateTime dataHoraEntrada) : this()
    {
        Id = Guid.NewGuid();
        DataHoraEntrada = dataHoraEntrada;
        StatusTicket = StatusTicket.Aberto;
    }

    public void RegistrarSaida(DateTime dataHoraSaida)
    {
        DataHoraSaida = dataHoraSaida;
        StatusTicket = StatusTicket.Fechado;
    }

    public void AdicionarCheckIn(CheckIn checkIn)
    {
        CheckIn = checkIn;
    }

    public override void AtualizarRegistro(Ticket registroEditado)
    {
        DataHoraEntrada = registroEditado.DataHoraEntrada;
        DataHoraSaida = registroEditado.DataHoraSaida;
        StatusTicket = registroEditado.StatusTicket;
    }
}
