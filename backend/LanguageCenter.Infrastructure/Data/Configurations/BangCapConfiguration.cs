using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LanguageCenter.Infrastructure.Data.Configurations;

public sealed class BangCapConfiguration : IEntityTypeConfiguration<BangCap>
{
    public void Configure(EntityTypeBuilder<BangCap> builder)
    {
        builder.ToTable("BangCap");

        builder.HasKey(x => x.MaBangCap);
        builder.Property(x => x.MaBangCap).ValueGeneratedOnAdd();

        builder.Property(x => x.MaGiaoVien).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(x => x.TenBangCap).HasMaxLength(150).IsRequired();
        builder.Property(x => x.ChuyenNganh).HasMaxLength(200);
        builder.Property(x => x.CoSoDaoTao).HasMaxLength(255);
        builder.Property(x => x.SoBang).HasMaxLength(100);
        builder.Property(x => x.TepMinhChung).HasMaxLength(500);

        builder.HasOne(x => x.GiaoVien)
            .WithMany(x => x.BangCaps)
            .HasForeignKey(x => x.MaGiaoVien)
            .HasConstraintName("FK_BangCap_GiaoVien")
            .OnDelete(DeleteBehavior.Cascade); // DB: ON DELETE CASCADE
    }
}
