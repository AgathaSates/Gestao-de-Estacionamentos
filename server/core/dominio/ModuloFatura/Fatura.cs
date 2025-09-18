using System.Diagnostics.CodeAnalysis;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;

namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento
{    
    public class Fatura : EntidadeBase<Fatura>
    {
        public Guid TicketId { get; set; }
        public string PlacaVeiculo { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime DataSaida { get; set; }
        public int Valortotal { get; set; }

        [ExcludeFromCodeCoverage]
        public Fatura() { }

        public Fatura(Guid ticketId, string placaVeiculo, DateTime dataEntrada, DateTime dataSaida) : this()
        {
            Id = Guid.NewGuid();
            TicketId = ticketId;
            PlacaVeiculo = placaVeiculo;
            DataEntrada = dataEntrada;
            DataSaida = dataSaida;
        }

        public int CalcularNumeroDiarias(DateTime dataEntrada, DateTime dataSaida)
        {          
            return (dataSaida.Date - dataEntrada.Date).Days + 1;                    
        }    

        public override void AtualizarRegistro(Fatura registroEditado)
        {
            PlacaVeiculo = registroEditado.PlacaVeiculo;
            DataEntrada = registroEditado.DataEntrada;
            DataSaida = registroEditado.DataSaida;
            Valortotal = registroEditado.Valortotal;
        }
    }
}