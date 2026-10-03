using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class LoaiDiemConfiguration : IEntityTypeConfiguration<LoaiDiem>
{
    public void Configure(EntityTypeBuilder<LoaiDiem> builder)
    {
        builder.ToTable("LoaiDiem");

        builder.HasKey(x => x.MaLoaiDiem);
        builder.Property(x => x.MaLoaiDiem).ValueGeneratedOnAdd();

        builder.Property(x => x.TenLoaiDiem).HasMaxLength(150).IsRequired();
        builder.Property(x => x.TrongSo).HasPrecision(5, 2);
        builder.Property(x => x.DiemToiDa).HasPrecision(6, 2);
        builder.Property(x => x.ThuTu).HasDefaultValue(1);

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Hoạt động");

        builder.HasOne(x => x.KhoaHoc)
            .WithMany(x => x.LoaiDiems)
            .HasForeignKey(x => x.MaKhoaHoc)
            .HasConstraintName("FK_LoaiDiem_KhoaHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasIndex(x => new { x.MaKhoaHoc, x.TenLoaiDiem })
            .IsUnique()
            .HasDatabaseName("UQ_LoaiDiem_Ten");

        builder.HasIndex(x => new { x.MaKhoaHoc, x.ThuTu })
            .IsUnique()
            .HasDatabaseName("UQ_LoaiDiem_ThuTu");
    }
}
