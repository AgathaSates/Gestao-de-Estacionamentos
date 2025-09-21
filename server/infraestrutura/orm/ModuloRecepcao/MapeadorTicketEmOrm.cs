using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeTicket;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloRecepcao;
public class MapeadorTicketEmOrm : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.Property(t => t.Id)
               .ValueGeneratedNever()
               .IsRequired();

        builder.Property(t => t.NumeroSequencial)
            .ValueGeneratedOnAdd()
            .HasIdentityOptions(1)
            .IsRequired();

        builder.HasIndex(t => t.NumeroSequencial)
             .IsUnique()
             .HasDatabaseName("UQ_Ticket_NumeroSequencial");

        builder.Property(t => t.DataHoraEntrada)
            .IsRequired();

        builder.Property(t => t.DataHoraSaida);

        builder.Property(t => t.StatusTicket)
            .IsRequired();

        builder.Property(t => t.CheckInId)
            .IsRequired();

        builder.HasOne(t => t.CheckIn)
           .WithOne(c => c.Ticket)
           .HasForeignKey<Ticket>(t => t.CheckInId)
           .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(t => t.CheckInId)
            .IsUnique();
    }
}