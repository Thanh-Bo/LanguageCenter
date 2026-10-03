using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class HocPhiConfiguration : IEntityTypeConfiguration<HocPhi>
{
    public void Configure(EntityTypeBuilder<HocPhi> builder)
    {
        builder.ToTable("HocPhi");

        builder.HasKey(x => x.MaHocPhi);
        builder.Property(x => x.MaHocPhi).ValueGeneratedOnAdd();

        builder.Property(x => x.SoTienGoc).HasPrecision(18, 2);
        builder.Property(x => x.SoTienGiam).HasPrecision(18, 2).HasDefaultValue(0m);
        builder.Property(x => x.SoTienDaDong).HasPrecision(18, 2).HasDefaultValue(0m);

        // Hai cột computed PERSISTED -> KHÔNG bao giờ ghi giá trị khi INSERT/UPDATE.
        builder.Property(x => x.SoTienPhaiDong)
            .HasPrecision(18, 2)
            .HasComputedColumnSql("[SoTienGoc] - [SoTienGiam]", stored: true);

        builder.Property(x => x.SoTienConThieu)
            .HasPrecision(18, 2)
            .HasComputedColumnSql("([SoTienGoc] - [SoTienGiam]) - [SoTienDaDong]", stored: true);

        builder.Property(x => x.TrangThaiHocPhi)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Chưa thanh toán");

        builder.Property(x => x.NgayTao).HasDefaultValueSql("SYSDATETIME()");

        // Quan hệ 1-1: MaDangKy vừa là UNIQUE vừa là FK.
        builder.HasOne(x => x.DangKyHoc)
            .WithOne(x => x.HocPhi)
            .HasForeignKey<HocPhi>(x => x.MaDangKy)
            .HasConstraintName("FK_HocPhi_DangKy")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION
    }
}
