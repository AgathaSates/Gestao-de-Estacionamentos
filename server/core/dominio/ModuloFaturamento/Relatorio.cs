using System.Diagnostics.CodeAnalysis;
using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;

namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento
{
    public class Relatorio : EntidadeBase<Relatorio>
    {
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public decimal ValorTotal { get; set; }
        public List<Fatura> Faturas { get; set; }

        [ExcludeFromCodeCoverage]
        public Relatorio()
        {
            Faturas = new List<Fatura>();
        }

        public Relatorio(DateTime dataInicial, DateTime dataFinal, List<Fatura> faturas)
        {
            Id = Guid.NewGuid();
            DataInicial = dataInicial;
            DataFinal = dataFinal;
            Faturas = faturas;
            GerarValorTotal();
        }

        public void GerarValorTotal()
        {
            ValorTotal = Faturas.Sum(f => f.Valortotal);
        }

        public override void AtualizarRegistro(Relatorio registroEditado)
        {
            DataInicial = registroEditado.DataInicial;
            DataFinal = registroEditado.DataFinal;
            ValorTotal = registroEditado.ValorTotal;
            Faturas = registroEditado.Faturas;
        }
    }
}
