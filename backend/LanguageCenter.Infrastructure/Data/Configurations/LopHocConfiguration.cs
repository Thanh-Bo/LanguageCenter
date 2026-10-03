using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class LopHocConfiguration : IEntityTypeConfiguration<LopHoc>
{
    public void Configure(EntityTypeBuilder<LopHoc> builder)
    {
        builder.ToTable("LopHoc");

        builder.HasKey(x => x.MaLopHoc);
        builder.Property(x => x.MaLopHoc).ValueGeneratedOnAdd();

        builder.Property(x => x.MaLop).HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(x => x.TenLop).HasMaxLength(200).IsRequired();
        builder.Property(x => x.MaNhanVienHocVu).HasMaxLength(20).IsUnicode(false).IsRequired();

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Chuẩn bị mở");

        builder.HasIndex(x => x.MaLop).IsUnique();

        builder.HasOne(x => x.KhoaHoc)
            .WithMany(x => x.LopHocs)
            .HasForeignKey(x => x.MaKhoaHoc)
            .HasConstraintName("FK_LopHoc_KhoaHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.NhanVienHocVu)
            .WithMany(x => x.LopHocs)
            .HasForeignKey(x => x.MaNhanVienHocVu)
            .HasConstraintName("FK_LopHoc_NhanVienHocVu")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION
    }
}
