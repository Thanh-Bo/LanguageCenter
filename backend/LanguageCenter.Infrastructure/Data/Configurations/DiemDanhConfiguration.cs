using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class DiemDanhConfiguration : IEntityTypeConfiguration<DiemDanh>
{
    public void Configure(EntityTypeBuilder<DiemDanh> builder)
    {
        builder.ToTable("DiemDanh");

        builder.HasKey(x => x.MaDiemDanh);
        builder.Property(x => x.MaDiemDanh).ValueGeneratedOnAdd();

        builder.Property(x => x.MaHocVien).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.TrangThaiDiemDanh).HasMaxLength(50).IsRequired();
        builder.Property(x => x.GhiChu).HasMaxLength(500);
        builder.Property(x => x.ThoiGianDiemDanh).HasDefaultValueSql("SYSDATETIME()");

        builder.HasOne(x => x.CaHoc)
            .WithMany(x => x.DiemDanhs)
            .HasForeignKey(x => x.MaCaHoc)
            .HasConstraintName("FK_DiemDanh_CaHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.HocVien)
            .WithMany(x => x.DiemDanhs)
            .HasForeignKey(x => x.MaHocVien)
            .HasConstraintName("FK_DiemDanh_HocVien")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.TaiKhoanThucHien)
            .WithMany(x => x.DiemDanhs)
            .HasForeignKey(x => x.MaTaiKhoanThucHien)
            .HasConstraintName("FK_DiemDanh_TaiKhoan")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasIndex(x => new { x.MaCaHoc, x.MaHocVien })
            .IsUnique()
            .HasDatabaseName("UQ_DiemDanh_CaHoc_HocVien");
    }
}
