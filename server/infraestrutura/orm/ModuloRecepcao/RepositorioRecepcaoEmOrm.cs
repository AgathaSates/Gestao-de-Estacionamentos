using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Gestao_de_Estacionamentos.Infraestutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloRecepcao;

public class RepositorioRecepcaoEmOrm(AppDbContext context)
    : RepositorioBaseEmOrm<CheckIn>(context), IRepositorioRecepcao
{
    public override async Task CadastrarAsync(CheckIn novoRegistro)
    {
        var novoTicket = new Ticket(DateTime.UtcNow);

        novoRegistro.AdicionarTicket(novoTicket);
        novoTicket.AdicionarCheckIn(novoRegistro);
        novoRegistro.Veiculo.AdicionarCheckIn(novoRegistro);

        await registros.AddAsync(novoRegistro);
    }

    public override async Task CadastrarEntidadesAsync(IList<CheckIn> checkIns)
    {
        foreach (var entidade in checkIns)
        {
            var novoTicket = new Ticket(DateTime.UtcNow);
            entidade.AdicionarTicket(novoTicket);
            novoTicket.AdicionarCheckIn(entidade);
            entidade.Veiculo.AdicionarCheckIn(entidade);
        }

        await registros.AddRangeAsync(checkIns);
    }

    public override async Task<CheckIn?> SelecionarRegistroPorIdAsync(Guid idRegistro)
    {
        return await registros
            .IgnoreQueryFilters()
            .Include(c => c.Veiculo)
            .Include(c => c.Ticket)
            .OrderBy(c => c.Veiculo.Placa)
            .FirstOrDefaultAsync(x => x.Id.Equals(idRegistro));
    }

    public override async Task<List<CheckIn>> SelecionarRegistrosAsync()
    {
        return await registros
            .IgnoreQueryFilters()
            .Include(c => c.Veiculo)
            .Include(c => c.Ticket)
            .OrderBy(c => c.Veiculo.Placa)
            .ToListAsync();
    }

    public override async Task<List<CheckIn>> SelecionarRegistrosAsync(int quantidade)
    {
        return await registros
            .IgnoreQueryFilters()
            .Include(c => c.Veiculo)
            .Include(c => c.Ticket)
            .OrderBy(c => c.Veiculo.Placa)
            .Take(quantidade)
            .ToListAsync();
    }

    public async Task<Veiculo?> SelecionarVeiculoPorPlaca(string placa)
    {
        return await registros
            .IgnoreQueryFilters()
            .Include(c => c.Veiculo)
            .Include(c => c.Ticket)
            .Where(c => c.Veiculo.Placa == placa)
            .Select(c => c.Veiculo)
            .FirstOrDefaultAsync();
    }

    public Task<Veiculo?> SelecionarVeiculoPorTicket(int? numeroTicket)
    {
        return registros
            .IgnoreQueryFilters()
            .Include(c => c.Veiculo)
            .Include(c => c.Ticket)
            .Where(c => c.Ticket.NumeroSequencial == numeroTicket)
            .Select(c => c.Veiculo)
            .FirstOrDefaultAsync();
    }
}