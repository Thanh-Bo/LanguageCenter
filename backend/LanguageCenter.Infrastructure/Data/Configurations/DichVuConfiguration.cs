using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class DichVuConfiguration : IEntityTypeConfiguration<DichVu>
{
    public void Configure(EntityTypeBuilder<DichVu> builder)
    {
        builder.ToTable("DichVu");

        builder.HasKey(x => x.MaDichVu);
        builder.Property(x => x.MaDichVu).ValueGeneratedOnAdd();

        builder.Property(x => x.TenDichVu).HasMaxLength(200).IsRequired();
        builder.Property(x => x.MoTa).HasMaxLength(1000);

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Hoạt động");

        builder.HasIndex(x => x.TenDichVu).IsUnique();

        // DichVu là danh mục độc lập — không có khóa ngoại trong database.
    }
}
