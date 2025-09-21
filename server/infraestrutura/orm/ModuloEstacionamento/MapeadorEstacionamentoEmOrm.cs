using Gestao_de_Estacionamentos.Core.Dominio.ModuloEstacionamento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloEstacionamento;
public class MapeadorEstacionamentoEmOrm : IEntityTypeConfiguration<Vaga>
{
    public void Configure(EntityTypeBuilder<Vaga> builder)
    {
       builder.Property(v => v.Id)
              .ValueGeneratedNever()
              .IsRequired();

        builder.Property(v => v.NumeroVaga)
               .ValueGeneratedOnAdd()
               .HasIdentityOptions(1)
               .IsRequired();

        builder.HasIndex(v => v.NumeroVaga)
                .IsUnique()
                .HasDatabaseName("UQ_Vaga_NumeroVaga");

        builder.Property(v => v.Zona)
                .IsRequired()
                .HasColumnType("char(1)");

        builder.Property(v => v.EstaOcupada)
                .IsRequired();

        builder.HasOne(v => v.VeiculoEstacionado)
                .WithOne(v => v.Vaga);
    }
}