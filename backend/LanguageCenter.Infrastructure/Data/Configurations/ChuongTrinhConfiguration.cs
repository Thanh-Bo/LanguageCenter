using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class ChuongTrinhConfiguration : IEntityTypeConfiguration<ChuongTrinh>
{
    public void Configure(EntityTypeBuilder<ChuongTrinh> builder)
    {
        builder.ToTable("ChuongTrinh");

        builder.HasKey(x => x.MaChuongTrinh);
        builder.Property(x => x.MaChuongTrinh).ValueGeneratedOnAdd();

        builder.Property(x => x.TenChuongTrinh).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NhomDoiTuong).HasMaxLength(200);
        builder.Property(x => x.MucTieu).HasMaxLength(1000);
        // MoTa là NVARCHAR(MAX) -> mặc định của EF đã là nvarchar(max).

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Hoạt động");

        builder.HasIndex(x => x.TenChuongTrinh).IsUnique();

        // Quan hệ 1-N tới CapDo được cấu hình trong CapDoConfiguration.
    }
}
