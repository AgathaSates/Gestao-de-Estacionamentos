using Gestao_de_Estacionamentos.Core.Dominio.ModuloRecepcao.EntidadeVeiculo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gestao_de_Estacionamentos.Infraestutura.Orm.ModuloRecepcao;
public class MapeadorVeiculoEmOrm : IEntityTypeConfiguration<Veiculo>
{
    public void Configure(EntityTypeBuilder<Veiculo> builder)
    {
        builder.Property(v => v.Id)
               .ValueGeneratedNever()
               .IsRequired();

        builder.Property(v => v.Placa)
            .IsRequired()
            .HasMaxLength(7);

        builder.Property(v => v.Modelo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(v => v.Cor)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(v => v.Observacoes)
            .HasMaxLength(200);

        builder.Property(v => v.CheckInId)
            .IsRequired();

        builder.HasIndex(v => v.CheckInId)
            .IsUnique();
    }
}