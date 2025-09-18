using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;

namespace Gestao_de_Estacionamentos.Core.Dominio.ModuloFatura
{
    public interface IRepositorioRelatorio : IRepositorio<Relatorio>
    {
        Task<Relatorio> GerarRelatorioFinanceiroAsync(DateTime dataInicio, DateTime dataFinal, List<Fatura> faturas);
    }
}
