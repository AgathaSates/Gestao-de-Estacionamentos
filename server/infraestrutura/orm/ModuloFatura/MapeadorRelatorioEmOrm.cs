using Gestao_de_Estacionamentos.Core.Dominio.ModuloFaturamento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloFatura
{
    public class MapeadorRelatorioEmOrm : IEntityTypeConfiguration<Relatorio>
    {
        public void Configure(EntityTypeBuilder<Relatorio> builder)
        {
            builder.Property(r => r.Id)
                   .ValueGeneratedNever()
                   .IsRequired();

            builder.Property(r => r.DataInicial)
                .IsRequired();

            builder.Property(r => r.DataFinal)
                .IsRequired();

            builder.Property(r => r.ValorTotal)
                .IsRequired();

            builder.HasMany(r => r.Faturas)
                   .WithOne()
                   .HasForeignKey("RelatorioId")
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
