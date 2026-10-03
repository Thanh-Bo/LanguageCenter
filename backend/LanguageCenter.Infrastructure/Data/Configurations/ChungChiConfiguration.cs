using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class ChungChiConfiguration : IEntityTypeConfiguration<ChungChi>
{
    public void Configure(EntityTypeBuilder<ChungChi> builder)
    {
        builder.ToTable("ChungChi");

        builder.HasKey(x => x.MaChungChi);
        builder.Property(x => x.MaChungChi).ValueGeneratedOnAdd();

        builder.Property(x => x.MaGiaoVien).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.TenChungChi).HasMaxLength(150).IsRequired();
        builder.Property(x => x.LoaiChungChi).HasMaxLength(100);
        builder.Property(x => x.DonViCap).HasMaxLength(255);
        builder.Property(x => x.KetQua).HasMaxLength(100);
        builder.Property(x => x.SoChungChi).HasMaxLength(100);
        builder.Property(x => x.TepMinhChung).HasMaxLength(500);

        builder.HasOne(x => x.GiaoVien)
            .WithMany(x => x.ChungChis)
            .HasForeignKey(x => x.MaGiaoVien)
            .HasConstraintName("FK_ChungChi_GiaoVien")
            .OnDelete(DeleteBehavior.Cascade); // DB: ON DELETE CASCADE
    }
}
