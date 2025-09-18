using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloFatura
{
    public class MapeadorFaturaEmOrm : IEntityTypeConfiguration<Fatura>
    {
        public void Configure(EntityTypeBuilder<Fatura> builder)
        {
            builder.Property(f => f.Id)
                   .ValueGeneratedNever()
                   .IsRequired();

            builder.Property(f => f.TicketId)
                .IsRequired();

            builder.Property(f => f.PlacaVeiculo)
                .IsRequired()
                .HasMaxLength(7);

            builder.Property(f => f.DataEntrada)
                .IsRequired();

            builder.Property(f => f.DataSaida)
                .IsRequired();

            builder.Property(f => f.Valortotal)
                .IsRequired();

        }
    }
}
