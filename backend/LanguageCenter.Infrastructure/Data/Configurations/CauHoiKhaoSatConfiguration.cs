using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class CauHoiKhaoSatConfiguration : IEntityTypeConfiguration<CauHoiKhaoSat>
{
    public void Configure(EntityTypeBuilder<CauHoiKhaoSat> builder)
    {
        builder.ToTable("CauHoiKhaoSat");

        builder.HasKey(x => x.MaCauHoi);
        builder.Property(x => x.MaCauHoi).ValueGeneratedOnAdd();

        builder.Property(x => x.NoiDungCauHoi).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.LoaiCauHoi).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ThuTu).HasDefaultValue(1);
        builder.Property(x => x.BatBuoc).HasDefaultValue(false);

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Hoạt động");

        builder.HasOne(x => x.KhaoSat)
            .WithMany(x => x.CauHoiKhaoSats)
            .HasForeignKey(x => x.MaKhaoSat)
            .HasConstraintName("FK_CauHoi_KhaoSat")
            .OnDelete(DeleteBehavior.Cascade); // DB: ON DELETE CASCADE

        builder.HasIndex(x => new { x.MaKhaoSat, x.ThuTu })
            .IsUnique()
            .HasDatabaseName("UQ_CauHoiKhaoSat_ThuTu");
    }
}
