using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class KiemTraDauVaoConfiguration : IEntityTypeConfiguration<KiemTraDauVao>
{
    public void Configure(EntityTypeBuilder<KiemTraDauVao> builder)
    {
        builder.ToTable("KiemTraDauVao");

        builder.HasKey(x => x.MaKiemTra);
        builder.Property(x => x.MaKiemTra).ValueGeneratedOnAdd();

        builder.Property(x => x.MaHocVien).HasMaxLength(20).IsUnicode(false).IsRequired();

        builder.Property(x => x.NgayKiemTra).HasDefaultValueSql("SYSDATETIME()");

        builder.Property(x => x.DiemNghe).HasPrecision(6, 2);
        builder.Property(x => x.DiemNoi).HasPrecision(6, 2);
        builder.Property(x => x.DiemDoc).HasPrecision(6, 2);
        builder.Property(x => x.DiemViet).HasPrecision(6, 2);
        builder.Property(x => x.TongDiem).HasPrecision(7, 2);

        builder.Property(x => x.NhanXet).HasMaxLength(1000);

        builder.HasOne(x => x.HocVien)
            .WithMany(x => x.KiemTraDauVaos)
            .HasForeignKey(x => x.MaHocVien)
            .HasConstraintName("FK_KiemTraDauVao_HocVien")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        // CapDoDanhGia là quan hệ tùy chọn — chỉ rõ principal để tránh nhập nhằng với CapDo.KhoaHoc.
        builder.HasOne(x => x.CapDoDanhGia)
            .WithMany(x => x.KiemTraDauVaos)
            .HasForeignKey(x => x.MaCapDoDanhGia)
            .HasConstraintName("FK_KiemTraDauVao_CapDo")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION
    }
}
