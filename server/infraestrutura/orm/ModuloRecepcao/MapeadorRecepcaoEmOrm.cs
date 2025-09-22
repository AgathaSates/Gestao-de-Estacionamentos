using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;
using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloRecepcao;
public class MapeadorRecepcaoEmOrm : IEntityTypeConfiguration<CheckIn>
{
    public void Configure(EntityTypeBuilder<CheckIn> builder)
    {
        builder.Property(c => c.Id)
               .ValueGeneratedNever()
               .IsRequired();

        builder.HasOne(c => c.Veiculo)
            .WithOne(v => v.CheckIn)
            .HasForeignKey<Veiculo>(v => v.CheckInId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(c => c.CPF)
            .IsRequired()
            .HasMaxLength(11);

        builder.Property(c => c.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(c => c.Ticket)
            .WithOne(t => t.CheckIn)
            .HasForeignKey<Ticket>(t => t.CheckInId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
