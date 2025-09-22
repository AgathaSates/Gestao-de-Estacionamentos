using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Infraestutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloFatura
{
    public class RepositorioFaturaEmOrm(AppDbContext context)
        : RepositorioBaseEmOrm<Fatura>(context), IRepositorioFatura
    {
        public decimal CalcularValorFatura(int NumeroDiarias, decimal valorDiaria)
        {
            return NumeroDiarias * valorDiaria / 100;
        }

        public async Task<List<Fatura>> SelecionarFaturasPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await context.Faturas
                 .Where(f => f.DataEntrada >= dataInicio && f.DataSaida <= dataFim)
                 .OrderBy(f => f.DataEntrada)
                 .ToListAsync();
        }
    }
}
