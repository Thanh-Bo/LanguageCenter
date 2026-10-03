using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class HocVienConfiguration : IEntityTypeConfiguration<HocVien>
{
    public void Configure(EntityTypeBuilder<HocVien> builder)
    {
        builder.ToTable("HocVien");

        builder.HasKey(x => x.MaHocVien);
        builder.Property(x => x.MaHocVien).HasMaxLength(20).IsUnicode(false);

        builder.Property(x => x.HoTen).HasMaxLength(150).IsRequired();
        builder.Property(x => x.GioiTinh).HasMaxLength(20);
        builder.Property(x => x.SoDienThoai).HasMaxLength(20).IsUnicode(false);
        builder.Property(x => x.Email).HasMaxLength(255).IsUnicode(false);
        builder.Property(x => x.DiaChi).HasMaxLength(500);
        builder.Property(x => x.MoiQuanHePhuHuynh).HasMaxLength(50);
        builder.Property(x => x.MucTieuHocTap).HasMaxLength(1000);
        builder.Property(x => x.TrinhDoHienTai).HasMaxLength(200);

        builder.Property(x => x.TrangThaiHocTap)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Đang tư vấn");

        builder.Property(x => x.NgayTaoHoSo).HasDefaultValueSql("SYSDATETIME()");

        // MaHocVien vừa là PK vừa là FK trỏ tới TaiKhoan.TenDangNhap (alternate key).
        builder.HasOne(x => x.TaiKhoan)
            .WithOne(x => x.HocVien)
            .HasForeignKey<HocVien>(x => x.MaHocVien)
            .HasPrincipalKey<TaiKhoan>(x => x.TenDangNhap)
            .HasConstraintName("FK_HocVien_TaiKhoan")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        // Mỗi học viên lưu tối đa 1 phụ huynh. Xếp lớp qua HocVien.MaPhuHuynh.
        builder.HasOne(x => x.PhuHuynh)
            .WithMany(x => x.HocViens)
            .HasForeignKey(x => x.MaPhuHuynh)
            .HasConstraintName("FK_HocVien_PhuHuynh")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION
    }
}
