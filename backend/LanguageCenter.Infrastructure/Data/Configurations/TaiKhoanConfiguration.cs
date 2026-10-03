using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class TaiKhoanConfiguration : IEntityTypeConfiguration<TaiKhoan>
{
    public void Configure(EntityTypeBuilder<TaiKhoan> builder)
    {
        builder.ToTable("TaiKhoan");

        builder.HasKey(x => x.MaTaiKhoan);
        builder.Property(x => x.MaTaiKhoan).ValueGeneratedOnAdd();

        // TenDangNhap là alternate key — NhanVien / GiaoVien / HocVien tham chiếu tới cột này.
        builder.Property(x => x.TenDangNhap).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.HasAlternateKey(x => x.TenDangNhap);

        builder.Property(x => x.Email).HasMaxLength(255).IsUnicode(false).IsRequired();
        builder.Property(x => x.MatKhauHash).HasMaxLength(500).IsRequired();

        builder.Property(x => x.TrangThaiTaiKhoan)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Hoạt động");

        builder.Property(x => x.NgayTao).HasDefaultValueSql("SYSDATETIME()");

        builder.HasIndex(x => x.Email).IsUnique();

        // FK_TaiKhoan_VaiTro được cấu hình trong VaiTroConfiguration.
        // Quan hệ 1-1 tới NhanVien / GiaoVien / HocVien được cấu hình ở phía dependent.
    }
}
