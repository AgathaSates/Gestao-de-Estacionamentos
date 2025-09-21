using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;

namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
public class Veiculo : EntidadeBase<Veiculo>
{
    public string Placa { get; set; }
    public string Modelo { get; set; }
    public string Cor { get; set; }
    public string? Observacoes { get; set; }
    public Guid CheckInId { get; set; }
    public CheckIn CheckIn { get; set; }
    public Guid? VagaId { get; set; }
    public Vaga? Vaga { get; set; }
    public Guid? TicketId { get; set; }

    [NotMapped]
    public Ticket? Ticket => CheckIn?.Ticket;


    [ExcludeFromCodeCoverage]
    public Veiculo()  { }

    public Veiculo(string placa, string modelo, string cor, string? observacoes = null) : this()
    {
        Id = Guid.NewGuid();
        Placa = placa;
        Modelo = modelo;
        Cor = cor;
        Observacoes = observacoes;
    }

    public void AdicionarObservacoes(string observacoes)
    {
        Observacoes = observacoes;
    }

    public void AdicionarCheckIn(CheckIn checkIn)
    {
        CheckIn = checkIn;
        CheckInId = checkIn.Id;
    }

    public void AdicionarVaga(Vaga vaga)
    {
        Vaga = vaga;
        VagaId = vaga.Id;
    }

    public override void AtualizarRegistro(Veiculo registroEditado)
    {
        Placa = registroEditado.Placa;
        Modelo = registroEditado.Modelo;
        Cor = registroEditado.Cor;
        Observacoes = registroEditado.Observacoes;
    }
}