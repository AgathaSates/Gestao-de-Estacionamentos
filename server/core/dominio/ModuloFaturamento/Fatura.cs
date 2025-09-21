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
        public decimal Valortotal { get; set; }

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

        //para gerar o valor total da fatura -> [Valortotal] -> para quando criar a fatura
        public void CalcularValorTotal(int NumeroDiarias, decimal valorDiaria)
        {
            Valortotal = NumeroDiarias * valorDiaria /100;
        }

        //para gerar o numero de diarias -> [NumeroDiarias] -> para quando criar a fatura
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