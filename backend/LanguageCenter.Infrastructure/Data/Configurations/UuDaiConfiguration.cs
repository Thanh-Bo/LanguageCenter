using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class UuDaiConfiguration : IEntityTypeConfiguration<UuDai>
{
    public void Configure(EntityTypeBuilder<UuDai> builder)
    {
        builder.ToTable("UuDai");

        builder.HasKey(x => x.MaUuDai);
        builder.Property(x => x.MaUuDai).ValueGeneratedOnAdd();

        builder.Property(x => x.TenUuDai).HasMaxLength(200).IsRequired();
        builder.Property(x => x.LoaiGiamGia).HasMaxLength(50).IsRequired();
        builder.Property(x => x.GiaTriGiam).HasPrecision(18, 2);
        builder.Property(x => x.SoKhoaHocToiThieu).HasDefaultValue(1);
        builder.Property(x => x.Poster).HasMaxLength(500);
        builder.Property(x => x.MoTa).HasMaxLength(1000);

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Hoạt động");

        // UuDai là danh mục độc lập — không có khóa ngoại trong database.
        // Quan hệ 1-N từ DangKyHoc được cấu hình trong DangKyHocConfiguration.
    }
}
