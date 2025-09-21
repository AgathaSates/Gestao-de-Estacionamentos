using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Gestao_de_Estacionamentos.Infraestutura.Orm.Compartilhado;
using Microsoft.EntityFrameworkCore;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloEstacionamento;
public class RepositorioEstacionamentoEmOrm(AppDbContext context)
    : RepositorioBaseEmOrm<Vaga>(context), IRepositorioEstacionamento
{
    public void AdicionarVeiculoAVaga(Vaga vaga, Veiculo veiculo)
    {
        vaga.AdicionarVeiculo(veiculo);
        veiculo.AdicionarVaga(vaga);
    }

    public void RemoverVeiculoDaVaga(Vaga vaga)
    {
        vaga.VeiculoEstacionado!.Ticket!.RegistrarSaida(DateTime.UtcNow);
        vaga.RemoverVeiculo();
    }

    public async Task<Vaga?> SelecionarPorNumeroDaVaga(int numeroVaga)
    {
        return await registros
            .IgnoreQueryFilters()
            .Include(v => v.VeiculoEstacionado)
            .FirstOrDefaultAsync(v => v.NumeroVaga.Equals(numeroVaga));
    }

    public async Task<Vaga?> SelecionarPorPlacaDoVeiculo(string placa)
    {
        return await registros
            .IgnoreQueryFilters()
            .Include(v => v.VeiculoEstacionado)
            .FirstOrDefaultAsync(v => v.VeiculoEstacionado!.Placa.Equals(placa));
    }

    public async Task<Veiculo?> SelecionarVeiculoPorPlaca(string placa)
    {       var vaga = await registros
            .IgnoreQueryFilters()
            .Include(v => v.VeiculoEstacionado)
            .ThenInclude(ve => ve.CheckIn)
            .ThenInclude(ci => ci.Ticket)
            .FirstOrDefaultAsync(v => v.VeiculoEstacionado
            != null && v.VeiculoEstacionado.Placa == placa);
        
        var veiculoEncontrado = vaga?.VeiculoEstacionado;
        
        return veiculoEncontrado;
    }

    public async Task<Veiculo?> SelecionarVeiculoPorTicket(int? numeroTicket)
    { 
        var vaga = await registros
            .IgnoreQueryFilters()
            .Include(v => v.VeiculoEstacionado)
            .ThenInclude(ve => ve.CheckIn)
            .ThenInclude(ci => ci.Ticket)
            .FirstOrDefaultAsync(v => v.VeiculoEstacionado
            != null && v.VeiculoEstacionado.CheckIn.Ticket.NumeroSequencial == numeroTicket);
        
        return vaga?.VeiculoEstacionado;
    }

    public async Task<List<Veiculo>> SelecionaTodosOsVeiculosEstacionados()
    {
        return await registros
            .IgnoreQueryFilters()
            .Where(vaga => vaga.EstaOcupada && vaga.VeiculoEstacionado != null)
            .Select(v => v.VeiculoEstacionado!)
            .ToListAsync();
    }

    public override async Task<Vaga?> SelecionarRegistroPorIdAsync(Guid idRegistro)
    {
        return await registros
            .IgnoreQueryFilters()
            .Include(v => v.VeiculoEstacionado)
            .ThenInclude(ve => ve.CheckIn)
            .ThenInclude(ci => ci.Ticket)
            .FirstOrDefaultAsync(v => v.Id == idRegistro);
    }

    public override async Task<List<Vaga>> SelecionarRegistrosAsync()
    {
        return await registros
            .IgnoreQueryFilters()
            .Include(v => v.VeiculoEstacionado)
            .ThenInclude(ve => ve.CheckIn)
            .ThenInclude(ci => ci.Ticket)
            .ToListAsync();
    }

    public override async Task<List<Vaga>> SelecionarRegistrosAsync(int quantidade)
    {
        return await registros
            .IgnoreQueryFilters()
            .Include(v => v.VeiculoEstacionado)
            .ThenInclude(ve => ve.CheckIn)
            .ThenInclude(ci => ci.Ticket)
            .Take(quantidade)
            .ToListAsync();
    }
}