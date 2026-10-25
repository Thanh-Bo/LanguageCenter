using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class NgayLeConfiguration : IEntityTypeConfiguration<NgayLe>
{
    public void Configure(EntityTypeBuilder<NgayLe> builder)
    {
        builder.ToTable("NgayLe");

        builder.HasKey(x => x.MaNgayLe);
        builder.Property(x => x.MaNgayLe).ValueGeneratedOnAdd();

        builder.Property(x => x.Ngay).IsRequired();

        builder.Property(x => x.TenNgayLe).HasMaxLength(100).IsRequired();
        builder.Property(x => x.GhiChu).HasMaxLength(200);

        builder.HasIndex(x => x.Ngay)
            .IsUnique()
            .HasDatabaseName("UQ_NgayLe_Ngay");
    }
}
