using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class VaiTroConfiguration : IEntityTypeConfiguration<VaiTro>
{
    public void Configure(EntityTypeBuilder<VaiTro> builder)
    {
        builder.ToTable("VaiTro");

        builder.HasKey(x => x.MaVaiTro);
        builder.Property(x => x.MaVaiTro).ValueGeneratedOnAdd();

        builder.Property(x => x.TenVaiTro).HasMaxLength(100).IsRequired();
        builder.Property(x => x.MoTa).HasMaxLength(500);
        builder.Property(x => x.TrangThai).HasDefaultValue(true);
        builder.Property(x => x.NgayTao).HasDefaultValueSql("SYSDATETIME()");

        builder.HasIndex(x => x.TenVaiTro).IsUnique();

        builder.HasMany(x => x.TaiKhoans)
            .WithOne(x => x.VaiTro)
            .HasForeignKey(x => x.MaVaiTro)
            .HasConstraintName("FK_TaiKhoan_VaiTro");
    }
}
