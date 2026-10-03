using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class NhanVienConfiguration : IEntityTypeConfiguration<NhanVien>
{
    public void Configure(EntityTypeBuilder<NhanVien> builder)
    {
        builder.ToTable("NhanVien");

        builder.HasKey(x => x.MaNhanVien);
        builder.Property(x => x.MaNhanVien).HasMaxLength(20).IsUnicode(false);

        builder.Property(x => x.HoTen).HasMaxLength(150).IsRequired();
        builder.Property(x => x.GioiTinh).HasMaxLength(20);
        builder.Property(x => x.SoDienThoai).HasMaxLength(20).IsUnicode(false);
        builder.Property(x => x.Email).HasMaxLength(255).IsUnicode(false);
        builder.Property(x => x.DiaChi).HasMaxLength(500);
        builder.Property(x => x.ChucVu).HasMaxLength(100).IsRequired();

        builder.Property(x => x.TrangThaiLamViec)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Đang làm việc");

        // MaNhanVien vừa là PK vừa là FK trỏ tới TaiKhoan.TenDangNhap (alternate key).
        builder.HasOne(x => x.TaiKhoan)
            .WithOne(x => x.NhanVien)
            .HasForeignKey<NhanVien>(x => x.MaNhanVien)
            .HasPrincipalKey<TaiKhoan>(x => x.TenDangNhap)
            .HasConstraintName("FK_NhanVien_TaiKhoan");
    }
}
