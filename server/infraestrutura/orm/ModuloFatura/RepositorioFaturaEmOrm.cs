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
            return NumeroDiarias * valorDiaria;
        }           

        public async Task<Fatura?> ObterFaturaAsync(Guid ticketId)
        {
            return await registros.FirstOrDefaultAsync(f => f.TicketId.Equals(ticketId));
        }        
    }
}