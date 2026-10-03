using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class CauTraLoiKhaoSatConfiguration : IEntityTypeConfiguration<CauTraLoiKhaoSat>
{
    public void Configure(EntityTypeBuilder<CauTraLoiKhaoSat> builder)
    {
        builder.ToTable("CauTraLoiKhaoSat");

        builder.HasKey(x => x.MaCauTraLoi);
        builder.Property(x => x.MaCauTraLoi).ValueGeneratedOnAdd();

        builder.Property(x => x.GiaTriDanhGia).HasPrecision(5, 2);
        builder.Property(x => x.NoiDungTraLoi).HasMaxLength(2000);

        builder.HasOne(x => x.PhanHoiKhaoSat)
            .WithMany(x => x.CauTraLoiKhaoSats)
            .HasForeignKey(x => x.MaPhanHoiKhaoSat)
            .HasConstraintName("FK_CauTraLoi_PhanHoi")
            .OnDelete(DeleteBehavior.Cascade); // DB: ON DELETE CASCADE

        builder.HasOne(x => x.CauHoiKhaoSat)
            .WithMany(x => x.CauTraLoiKhaoSats)
            .HasForeignKey(x => x.MaCauHoi)
            .HasConstraintName("FK_CauTraLoi_CauHoi")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION

        builder.HasIndex(x => new { x.MaPhanHoiKhaoSat, x.MaCauHoi })
            .IsUnique()
            .HasDatabaseName("UQ_CauTraLoi");
    }
}
