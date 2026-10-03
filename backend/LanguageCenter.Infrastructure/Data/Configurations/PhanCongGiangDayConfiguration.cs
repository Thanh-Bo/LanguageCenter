using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class PhanCongGiangDayConfiguration : IEntityTypeConfiguration<PhanCongGiangDay>
{
    public void Configure(EntityTypeBuilder<PhanCongGiangDay> builder)
    {
        builder.ToTable("PhanCongGiangDay");

        builder.HasKey(x => x.MaPhanCong);
        builder.Property(x => x.MaPhanCong).ValueGeneratedOnAdd();

        builder.Property(x => x.MaGiaoVien).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.VaiTroPhanCong).HasMaxLength(50).IsRequired();

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Đang phân công");

        builder.Property(x => x.GhiChu).HasMaxLength(500);

        builder.HasOne(x => x.LopHoc)
            .WithMany(x => x.PhanCongGiangDays)
            .HasForeignKey(x => x.MaLopHoc)
            .HasConstraintName("FK_PhanCong_LopHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.GiaoVien)
            .WithMany(x => x.PhanCongGiangDays)
            .HasForeignKey(x => x.MaGiaoVien)
            .HasConstraintName("FK_PhanCong_GiaoVien")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION
    }
}
