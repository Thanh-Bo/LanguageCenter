using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class NoiDungBaiHocConfiguration : IEntityTypeConfiguration<NoiDungBaiHoc>
{
    public void Configure(EntityTypeBuilder<NoiDungBaiHoc> builder)
    {
        builder.ToTable("NoiDungBaiHoc");

        builder.HasKey(x => x.MaNoiDung);
        builder.Property(x => x.MaNoiDung).ValueGeneratedOnAdd();

        builder.Property(x => x.SoBuoi).IsRequired();

        builder.Property(x => x.TieuDe).HasMaxLength(200).IsRequired();
        builder.Property(x => x.NoiDung).HasMaxLength(1000);
        builder.Property(x => x.MucTieu).HasMaxLength(500);

        builder.HasIndex(x => new { x.MaKhoaHoc, x.SoBuoi })
            .IsUnique()
            .HasDatabaseName("UQ_NoiDungBaiHoc_KhoaHoc_SoBuoi");

        builder.HasOne(x => x.KhoaHoc)
            .WithMany()
            .HasForeignKey(x => x.MaKhoaHoc)
            .HasConstraintName("FK_NoiDungBaiHoc_KhoaHoc")
            .OnDelete(DeleteBehavior.Restrict); // DB: NO ACTION
    }
}
