using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class GiaoVienConfiguration : IEntityTypeConfiguration<GiaoVien>
{
    public void Configure(EntityTypeBuilder<GiaoVien> builder)
    {
        builder.ToTable("GiaoVien");

        builder.HasKey(x => x.MaGiaoVien);
        builder.Property(x => x.MaGiaoVien).HasMaxLength(20).IsUnicode(false);

        builder.Property(x => x.HoTen).HasMaxLength(150).IsRequired();
        builder.Property(x => x.GioiTinh).HasMaxLength(20);
        builder.Property(x => x.SoDienThoai).HasMaxLength(20).IsUnicode(false);
        builder.Property(x => x.Email).HasMaxLength(255).IsUnicode(false);
        builder.Property(x => x.DiaChi).HasMaxLength(500);
        builder.Property(x => x.LoaiGiaoVien).HasMaxLength(50).IsRequired();
        builder.Property(x => x.ChuyenMon).HasMaxLength(500);

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Đang làm việc");

        // MaGiaoVien vừa là PK vừa là FK trỏ tới TaiKhoan.TenDangNhap (alternate key).
        builder.HasOne(x => x.TaiKhoan)
            .WithOne(x => x.GiaoVien)
            .HasForeignKey<GiaoVien>(x => x.MaGiaoVien)
            .HasPrincipalKey<TaiKhoan>(x => x.TenDangNhap)
            .HasConstraintName("FK_GiaoVien_TaiKhoan");
    }
}
