using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class PhuHuynhConfiguration : IEntityTypeConfiguration<PhuHuynh>
{
    public void Configure(EntityTypeBuilder<PhuHuynh> builder)
    {
        builder.ToTable("PhuHuynh");

        builder.HasKey(x => x.MaPhuHuynh);
        builder.Property(x => x.MaPhuHuynh).ValueGeneratedOnAdd();

        builder.Property(x => x.HoTen).HasMaxLength(150).IsRequired();
        builder.Property(x => x.SoDienThoai).HasMaxLength(20).IsUnicode(false);
        builder.Property(x => x.Email).HasMaxLength(255).IsUnicode(false);
        builder.Property(x => x.DiaChi).HasMaxLength(500);
        builder.Property(x => x.NgheNghiep).HasMaxLength(200);

        // Quan hệ 1-N tới HocVien được cấu hình trong HocVienConfiguration.
    }
}
