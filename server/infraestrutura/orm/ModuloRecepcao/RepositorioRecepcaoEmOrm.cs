using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;
using Gestao_de_Estacionamentos.Infraestutura.Orm.Compartilhado;
using Gestao_de_Estacionamentos.Infraestutura.Orm.Migrations;
using Microsoft.EntityFrameworkCore;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloRecepcao;

public class RepositorioRecepcaoEmOrm(AppDbContext context)
    : RepositorioBaseEmOrm<CheckIn>(context), IRepositorioRecepcao
{
    public override Task CadastrarAsync(CheckIn novoRegistro)
    {
        var novoticket = new Ticket(DateTime.UtcNow);

        novoRegistro.AdicionarTicket(novoticket);
        novoticket.AdicionarCheckIn(novoRegistro);
        novoRegistro.Veiculo.AdicionarCheckIn(novoRegistro);

        return base.CadastrarAsync(novoRegistro);
    }
    
    public override Task<CheckIn?> SelecionarRegistroPorIdAsync(Guid idRegistro)
    {
        return registros
            .IgnoreQueryFilters()
            .Include(c => c.Veiculo)
            .Include(c => c.Ticket)
            .OrderBy(c => c.Veiculo.Placa)
            .FirstOrDefaultAsync(x => x.Id.Equals(idRegistro));
    }

    public override Task<List<CheckIn>> SelecionarRegistrosAsync()
    {
        return registros
            .IgnoreQueryFilters()
            .Include(c => c.Veiculo)
            .Include(c => c.Ticket)
            .OrderBy(c => c.Veiculo.Placa)
            .ToListAsync();
    }
}