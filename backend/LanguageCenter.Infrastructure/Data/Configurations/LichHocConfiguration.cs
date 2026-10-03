using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class LichHocConfiguration : IEntityTypeConfiguration<LichHoc>
{
    public void Configure(EntityTypeBuilder<LichHoc> builder)
    {
        builder.ToTable("LichHoc");

        builder.HasKey(x => x.MaLichHoc);
        builder.Property(x => x.MaLichHoc).ValueGeneratedOnAdd();

        // ThuTrongTuan là TINYINT (1 = Thứ Hai ... 7 = Chủ Nhật) -> byte.
        builder.Property(x => x.ThuTrongTuan).HasColumnType("tinyint");

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Hoạt động");

        builder.HasOne(x => x.LopHoc)
            .WithMany(x => x.LichHocs)
            .HasForeignKey(x => x.MaLopHoc)
            .HasConstraintName("FK_LichHoc_LopHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.PhongHoc)
            .WithMany(x => x.LichHocs)
            .HasForeignKey(x => x.MaPhongHoc)
            .HasConstraintName("FK_LichHoc_PhongHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION
    }
}
