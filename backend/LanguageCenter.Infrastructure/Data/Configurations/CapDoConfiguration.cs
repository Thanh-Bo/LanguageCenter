using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class CapDoConfiguration : IEntityTypeConfiguration<CapDo>
{
    public void Configure(EntityTypeBuilder<CapDo> builder)
    {
        builder.ToTable("CapDo");

        builder.HasKey(x => x.MaCapDo);
        builder.Property(x => x.MaCapDo).ValueGeneratedOnAdd();

        builder.Property(x => x.KyHieuCapDo).HasMaxLength(30).IsUnicode(false);
        builder.Property(x => x.TenCapDo).HasMaxLength(150).IsRequired();
        builder.Property(x => x.ThuTu).HasDefaultValue(1);
        builder.Property(x => x.DieuKienDauVao).HasMaxLength(1000);
        builder.Property(x => x.KetQuaDauRa).HasMaxLength(1000);

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Hoạt động");

        builder.HasOne(x => x.ChuongTrinh)
            .WithMany(x => x.CapDos)
            .HasForeignKey(x => x.MaChuongTrinh)
            .HasConstraintName("FK_CapDo_ChuongTrinh")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasIndex(x => new { x.MaChuongTrinh, x.TenCapDo })
            .IsUnique()
            .HasDatabaseName("UQ_CapDo_ChuongTrinh_Ten");

        builder.HasIndex(x => new { x.MaChuongTrinh, x.ThuTu })
            .IsUnique()
            .HasDatabaseName("UQ_CapDo_ChuongTrinh_ThuTu");
    }
}
