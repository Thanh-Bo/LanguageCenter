using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class KhaoSatConfiguration : IEntityTypeConfiguration<KhaoSat>
{
    public void Configure(EntityTypeBuilder<KhaoSat> builder)
    {
        builder.ToTable("KhaoSat");

        builder.HasKey(x => x.MaKhaoSat);
        builder.Property(x => x.MaKhaoSat).ValueGeneratedOnAdd();

        builder.Property(x => x.TenKhaoSat).HasMaxLength(255).IsRequired();
        builder.Property(x => x.MoTa).HasMaxLength(1000);

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Hoạt động");
    }
}
