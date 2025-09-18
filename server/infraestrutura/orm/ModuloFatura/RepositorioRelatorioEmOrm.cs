using Gestao_de_Estacionamentos.Core.Dominio.ModuloFatura;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Infraestutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloFatura
{
    public class RepositorioRelatorioEmOrm(AppDbContext context)
        : RepositorioBaseEmOrm<Relatorio>(context), IRepositorioRelatorio
    {
        public Task<Relatorio> GerarRelatorioFinanceiroAsync(DateTime dataInicio, DateTime dataFinal, List<Fatura> faturas)
        {
           var relatorio = new Relatorio(dataInicio, dataFinal, faturas);
           relatorio.GerarRelatorio();
           return Task.FromResult(relatorio);
        }
    }
}