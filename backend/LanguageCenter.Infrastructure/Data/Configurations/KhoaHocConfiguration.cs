using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class KhoaHocConfiguration : IEntityTypeConfiguration<KhoaHoc>
{
    public void Configure(EntityTypeBuilder<KhoaHoc> builder)
    {
        builder.ToTable("KhoaHoc");

        builder.HasKey(x => x.MaKhoaHoc);
        builder.Property(x => x.MaKhoaHoc).ValueGeneratedOnAdd();

        builder.Property(x => x.TenKhoaHoc).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TongSoGio).HasPrecision(6, 2);
        builder.Property(x => x.HocPhiCoBan).HasPrecision(18, 2);

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Hoạt động");

        builder.HasOne(x => x.CapDo)
            .WithMany(x => x.KhoaHocs)
            .HasForeignKey(x => x.MaCapDo)
            .HasConstraintName("FK_KhoaHoc_CapDo")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasIndex(x => new { x.MaCapDo, x.TenKhoaHoc })
            .IsUnique()
            .HasDatabaseName("UQ_KhoaHoc_CapDo_Ten");
    }
}
