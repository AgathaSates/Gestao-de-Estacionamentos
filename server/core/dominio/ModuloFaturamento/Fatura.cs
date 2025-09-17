using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;

namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento
{    
    public class Fatura : EntidadeBase<Fatura>
    {
        public Guid ticketId { get; set; }

        public string PlacaVeiculo { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime DataSaida { get; set; }
        public int Valortotal { get; set; }

        [ExcludeFromCodeCoverage]
        public Fatura() { }

        public Fatura(Guid ticketId, string placaVeiculo, DateTime dataEntrada, DateTime dataSaida) : this()
        {
            Id = Guid.NewGuid();
            this.ticketId = ticketId;
            PlacaVeiculo = placaVeiculo;
            DataEntrada = dataEntrada;
            DataSaida = dataSaida;
        }

        public int CalcularNumeroDiarias(DateTime dataEntrada, DateTime dataSaida)
        {          
            return (dataSaida.Date - dataEntrada.Date).Days + 1;                      
        }

        public decimal CalcularValorTotal(int NumeroDiarias, decimal valorDiaria)
        {     
            return NumeroDiarias * valorDiaria;
        }

        public override void AtualizarRegistro(Fatura registroEditado)
        {
            ticketId = registroEditado.ticketId;
            PlacaVeiculo = registroEditado.PlacaVeiculo;
            DataEntrada = registroEditado.DataEntrada;
            DataSaida = registroEditado.DataSaida;
            Valortotal = registroEditado.Valortotal;
        }
    }
}