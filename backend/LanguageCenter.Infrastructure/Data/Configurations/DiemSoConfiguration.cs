using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class DiemSoConfiguration : IEntityTypeConfiguration<DiemSo>
{
    public void Configure(EntityTypeBuilder<DiemSo> builder)
    {
        builder.ToTable("DiemSo");

        builder.HasKey(x => x.MaDiemSo);
        builder.Property(x => x.MaDiemSo).ValueGeneratedOnAdd();

        builder.Property(x => x.MaGiaoVien).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.Diem).HasPrecision(6, 2);
        builder.Property(x => x.NhanXet).HasMaxLength(1000);
        builder.Property(x => x.TrangThaiKetQua).HasMaxLength(50);
        builder.Property(x => x.LanKiemTra).HasDefaultValue(1);
        builder.Property(x => x.NgayCham).HasDefaultValueSql("SYSDATETIME()");

        builder.HasOne(x => x.DangKyHoc)
            .WithMany(x => x.DiemSos)
            .HasForeignKey(x => x.MaDangKy)
            .HasConstraintName("FK_DiemSo_DangKyHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.LoaiDiem)
            .WithMany(x => x.DiemSos)
            .HasForeignKey(x => x.MaLoaiDiem)
            .HasConstraintName("FK_DiemSo_LoaiDiem")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.GiaoVien)
            .WithMany(x => x.DiemSos)
            .HasForeignKey(x => x.MaGiaoVien)
            .HasConstraintName("FK_DiemSo_GiaoVien")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        // Buổi thi gắn với bài kiểm tra (NULL = chấm ngoài lịch).
        builder.HasOne(x => x.CaHoc)
            .WithMany()
            .HasForeignKey(x => x.MaCaHoc)
            .HasConstraintName("FK_DiemSo_CaHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        // Một đầu điểm có thể có nhiều lần kiểm tra → UNIQUE gồm cả LanKiemTra.
        builder.HasIndex(x => new { x.MaDangKy, x.MaLoaiDiem, x.LanKiemTra })
            .IsUnique()
            .HasDatabaseName("UQ_DiemSo");
    }
}
