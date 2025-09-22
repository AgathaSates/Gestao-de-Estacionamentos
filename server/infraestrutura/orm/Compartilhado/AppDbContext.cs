using Gestao_de_Estacionamentos.Core.Dominio.Compartilhado;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.Compartilhado;
public class AppDbContext(DbContextOptions options) : IdentityDbContext<Usuario, Cargo, Guid>(options), IUnitOfWork
{
    public DbSet<CheckIn> CheckIns { get; set; }
    public DbSet<Veiculo> Veiculos { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Vaga> Vagas { get; set; }
    public DbSet<Fatura> Faturas { get; set; } 
    public DbSet<Relatorio> Relatorio { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = typeof(AppDbContext).Assembly;

        modelBuilder.ApplyConfigurationsFromAssembly(assembly);

        base.OnModelCreating(modelBuilder);
    }

    public async Task CommitAsync()
    {
        await SaveChangesAsync();
    }

    public async Task RollbackAsync()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Modified:
                    entry.State = EntityState.Unchanged;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Unchanged;
                    break;
            }
        }

        await Task.CompletedTask;
    }
}