using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class CaHocConfiguration : IEntityTypeConfiguration<CaHoc>
{
    public void Configure(EntityTypeBuilder<CaHoc> builder)
    {
        builder.ToTable("CaHoc");

        builder.HasKey(x => x.MaCaHoc);
        builder.Property(x => x.MaCaHoc).ValueGeneratedOnAdd();

        builder.Property(x => x.MaGiaoVienThucTe).HasMaxLength(20).IsUnicode(false);
        builder.Property(x => x.MaTroGiangThucTe).HasMaxLength(20).IsUnicode(false);

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Chưa diễn ra");

        builder.Property(x => x.LyDoThayDoi).HasMaxLength(500);

        builder.HasOne(x => x.LopHoc)
            .WithMany(x => x.CaHocs)
            .HasForeignKey(x => x.MaLopHoc)
            .HasConstraintName("FK_CaHoc_LopHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        // Có thể NULL nếu là ca phát sinh ngoài lịch định kỳ.
        builder.HasOne(x => x.LichHoc)
            .WithMany(x => x.CaHocs)
            .HasForeignKey(x => x.MaLichHoc)
            .HasConstraintName("FK_CaHoc_LichHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.PhongHoc)
            .WithMany(x => x.CaHocs)
            .HasForeignKey(x => x.MaPhongHoc)
            .HasConstraintName("FK_CaHoc_PhongHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        // CaHoc có HAI khóa ngoại cùng trỏ tới GiaoVien -> bắt buộc cấu hình tường minh.
        builder.HasOne(x => x.GiaoVienThucTe)
            .WithMany(x => x.CaHocGiaoVienThucTes)
            .HasForeignKey(x => x.MaGiaoVienThucTe)
            .HasConstraintName("FK_CaHoc_GiaoVien")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.TroGiangThucTe)
            .WithMany(x => x.CaHocTroGiangThucTes)
            .HasForeignKey(x => x.MaTroGiangThucTe)
            .HasConstraintName("FK_CaHoc_TroGiang")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasIndex(x => new { x.MaLopHoc, x.NgayHoc })
            .HasDatabaseName("IX_CaHoc_LopHoc_NgayHoc");
    }
}
