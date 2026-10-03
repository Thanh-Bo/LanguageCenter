using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class PhongHocConfiguration : IEntityTypeConfiguration<PhongHoc>
{
    public void Configure(EntityTypeBuilder<PhongHoc> builder)
    {
        builder.ToTable("PhongHoc");

        builder.HasKey(x => x.MaPhongHoc);
        builder.Property(x => x.MaPhongHoc).ValueGeneratedOnAdd();

        builder.Property(x => x.MaPhong).HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(x => x.TenPhong).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ViTri).HasMaxLength(255);
        builder.Property(x => x.ThietBi).HasMaxLength(1000);

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Hoạt động");

        builder.HasIndex(x => x.MaPhong).IsUnique();
    }
}
