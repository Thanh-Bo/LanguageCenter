using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class ThongBaoConfiguration : IEntityTypeConfiguration<ThongBao>
{
    public void Configure(EntityTypeBuilder<ThongBao> builder)
    {
        builder.ToTable("ThongBao");

        builder.HasKey(x => x.MaThongBao);
        builder.Property(x => x.MaThongBao).ValueGeneratedOnAdd();

        builder.Property(x => x.TieuDe).HasMaxLength(255).IsRequired();
        // NoiDung là NVARCHAR(MAX) -> mặc định của EF đã là nvarchar(max).
        builder.Property(x => x.NoiDung).IsRequired();

        builder.Property(x => x.LoaiThongBao).HasMaxLength(100);

        builder.Property(x => x.ThoiGianGui).HasDefaultValueSql("SYSDATETIME()");

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Đã gửi");

        // ThongBao có HAI khóa ngoại cùng trỏ tới TaiKhoan -> bắt buộc cấu hình tường minh.
        builder.HasOne(x => x.TaiKhoanGui)
            .WithMany(x => x.ThongBaoGui)
            .HasForeignKey(x => x.MaTaiKhoanGui)
            .HasConstraintName("FK_ThongBao_TaiKhoanGui")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.TaiKhoanNhan)
            .WithMany(x => x.ThongBaoNhan)
            .HasForeignKey(x => x.MaTaiKhoanNhan)
            .HasConstraintName("FK_ThongBao_TaiKhoanNhan")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.LopHoc)
            .WithMany(x => x.ThongBaos)
            .HasForeignKey(x => x.MaLopHoc)
            .HasConstraintName("FK_ThongBao_LopHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION
    }
}
