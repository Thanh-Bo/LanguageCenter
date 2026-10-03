using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class PhanHoiKhaoSatConfiguration : IEntityTypeConfiguration<PhanHoiKhaoSat>
{
    public void Configure(EntityTypeBuilder<PhanHoiKhaoSat> builder)
    {
        builder.ToTable("PhanHoiKhaoSat");

        builder.HasKey(x => x.MaPhanHoiKhaoSat);
        builder.Property(x => x.MaPhanHoiKhaoSat).ValueGeneratedOnAdd();

        builder.Property(x => x.MaHocVien).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.DiemDanhGiaChung).HasPrecision(5, 2);
        builder.Property(x => x.GhiChu).HasMaxLength(1000);

        builder.Property(x => x.ThoiGianGui).HasDefaultValueSql("SYSDATETIME()");

        builder.HasOne(x => x.KhaoSat)
            .WithMany(x => x.PhanHoiKhaoSats)
            .HasForeignKey(x => x.MaKhaoSat)
            .HasConstraintName("FK_PhanHoiKhaoSat_KhaoSat")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.HocVien)
            .WithMany(x => x.PhanHoiKhaoSats)
            .HasForeignKey(x => x.MaHocVien)
            .HasConstraintName("FK_PhanHoiKhaoSat_HocVien")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasIndex(x => new { x.MaKhaoSat, x.MaHocVien })
            .IsUnique()
            .HasDatabaseName("UQ_PhanHoiKhaoSat");
    }
}
