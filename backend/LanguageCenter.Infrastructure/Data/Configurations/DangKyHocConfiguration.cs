using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class DangKyHocConfiguration : IEntityTypeConfiguration<DangKyHoc>
{
    public void Configure(EntityTypeBuilder<DangKyHoc> builder)
    {
        builder.ToTable("DangKyHoc");

        builder.HasKey(x => x.MaDangKy);
        builder.Property(x => x.MaDangKy).ValueGeneratedOnAdd();

        builder.Property(x => x.MaHocVien).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.MaNhanVienTuVan).HasMaxLength(20).IsUnicode(false).IsRequired();

        builder.Property(x => x.NgayDangKy).HasDefaultValueSql("SYSDATETIME()");
        builder.Property(x => x.LichHocMongMuon).HasMaxLength(500);

        builder.Property(x => x.HocPhiGoc).HasPrecision(18, 2);
        builder.Property(x => x.SoTienGiam).HasPrecision(18, 2).HasDefaultValue(0m);

        // Cột computed PERSISTED trong database -> KHÔNG bao giờ ghi giá trị này khi INSERT/UPDATE.
        builder.Property(x => x.HocPhiSauGiam)
            .HasPrecision(18, 2)
            .HasComputedColumnSql("[HocPhiGoc] - [SoTienGiam]", stored: true);

        builder.Property(x => x.TrangThaiDangKy)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Chờ xử lý");

        builder.Property(x => x.GhiChu).HasMaxLength(500);

        builder.HasOne(x => x.HocVien)
            .WithMany(x => x.DangKyHocs)
            .HasForeignKey(x => x.MaHocVien)
            .HasConstraintName("FK_DangKyHoc_HocVien")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.KhoaHoc)
            .WithMany(x => x.DangKyHocs)
            .HasForeignKey(x => x.MaKhoaHoc)
            .HasConstraintName("FK_DangKyHoc_KhoaHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        // Chưa xếp lớp thì MaLopHoc = NULL.
        builder.HasOne(x => x.LopHoc)
            .WithMany(x => x.DangKyHocs)
            .HasForeignKey(x => x.MaLopHoc)
            .HasConstraintName("FK_DangKyHoc_LopHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.NhanVienTuVan)
            .WithMany(x => x.DangKyHocTuVans)
            .HasForeignKey(x => x.MaNhanVienTuVan)
            .HasConstraintName("FK_DangKyHoc_NhanVien")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.UuDai)
            .WithMany(x => x.DangKyHocs)
            .HasForeignKey(x => x.MaUuDai)
            .HasConstraintName("FK_DangKyHoc_UuDai")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        // Quan hệ 1-1 tới HocPhi được cấu hình trong HocPhiConfiguration.
    }
}
