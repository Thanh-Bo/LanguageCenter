using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class ThanhToanConfiguration : IEntityTypeConfiguration<ThanhToan>
{
    public void Configure(EntityTypeBuilder<ThanhToan> builder)
    {
        builder.ToTable("ThanhToan");

        builder.HasKey(x => x.MaThanhToan);
        builder.Property(x => x.MaThanhToan).ValueGeneratedOnAdd();

        builder.Property(x => x.MaNhanVienKeToan).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.SoTien).HasPrecision(18, 2);
        builder.Property(x => x.PhuongThucThanhToan).HasMaxLength(100).IsRequired();
        builder.Property(x => x.MaGiaoDich).HasMaxLength(150).IsUnicode(false);

        builder.Property(x => x.NgayThanhToan).HasDefaultValueSql("SYSDATETIME()");

        builder.Property(x => x.LoaiGiaoDich)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Thu tiền");

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Thành công");

        builder.Property(x => x.GhiChu).HasMaxLength(500);

        builder.HasOne(x => x.HocPhi)
            .WithMany(x => x.ThanhToans)
            .HasForeignKey(x => x.MaHocPhi)
            .HasConstraintName("FK_ThanhToan_HocPhi")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.NhanVienKeToan)
            .WithMany(x => x.ThanhToans)
            .HasForeignKey(x => x.MaNhanVienKeToan)
            .HasConstraintName("FK_ThanhToan_NhanVien")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        // Unique index CÓ ĐIỀU KIỆN: chỉ áp dụng khi MaGiaoDich IS NOT NULL.
        builder.HasIndex(x => x.MaGiaoDich)
            .IsUnique()
            .HasFilter("[MaGiaoDich] IS NOT NULL")
            .HasDatabaseName("UX_ThanhToan_MaGiaoDich");

        // Quan hệ 1-1 tới BienNhan được cấu hình trong BienNhanConfiguration.
    }
}
