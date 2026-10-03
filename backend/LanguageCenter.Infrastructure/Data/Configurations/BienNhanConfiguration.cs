using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class BienNhanConfiguration : IEntityTypeConfiguration<BienNhan>
{
    public void Configure(EntityTypeBuilder<BienNhan> builder)
    {
        builder.ToTable("BienNhan");

        builder.HasKey(x => x.MaBienNhan);
        builder.Property(x => x.MaBienNhan).ValueGeneratedOnAdd();

        builder.Property(x => x.MaNhanVienLap).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.SoBienNhan).HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(x => x.SoTienXacNhan).HasPrecision(18, 2);
        builder.Property(x => x.NoiDung).HasMaxLength(500);

        builder.Property(x => x.NgayLap).HasDefaultValueSql("SYSDATETIME()");

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Đã phát hành");

        builder.HasIndex(x => x.SoBienNhan).IsUnique();

        // Quan hệ 1-1: MaThanhToan vừa là UNIQUE vừa là FK.
        builder.HasOne(x => x.ThanhToan)
            .WithOne(x => x.BienNhan)
            .HasForeignKey<BienNhan>(x => x.MaThanhToan)
            .HasConstraintName("FK_BienNhan_ThanhToan")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.NhanVienLap)
            .WithMany(x => x.BienNhans)
            .HasForeignKey(x => x.MaNhanVienLap)
            .HasConstraintName("FK_BienNhan_NhanVien")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION
    }
}
