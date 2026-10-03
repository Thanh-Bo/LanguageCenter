using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class PhanHoiDichVuConfiguration : IEntityTypeConfiguration<PhanHoiDichVu>
{
    public void Configure(EntityTypeBuilder<PhanHoiDichVu> builder)
    {
        builder.ToTable("PhanHoiDichVu");

        builder.HasKey(x => x.MaPhanHoiDichVu);
        builder.Property(x => x.MaPhanHoiDichVu).ValueGeneratedOnAdd();

        builder.Property(x => x.MaHocVien).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.NoiDungPhanHoi).HasMaxLength(2000);

        builder.Property(x => x.ThoiGianPhanHoi).HasDefaultValueSql("SYSDATETIME()");

        builder.Property(x => x.TrangThai)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Đã gửi");

        builder.HasOne(x => x.DichVu)
            .WithMany(x => x.PhanHoiDichVus)
            .HasForeignKey(x => x.MaDichVu)
            .HasConstraintName("FK_PhanHoiDichVu_DichVu")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasOne(x => x.HocVien)
            .WithMany(x => x.PhanHoiDichVus)
            .HasForeignKey(x => x.MaHocVien)
            .HasConstraintName("FK_PhanHoiDichVu_HocVien")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION
    }
}
