using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;

namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento
{
    public interface IRepositorioFatura : IRepositorio<Fatura>
    {
        decimal CalcularValorFatura(int NumeroDiarias, decimal valorDiaria);
        Task<List<Fatura>> SelecionarFaturasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
    }
}