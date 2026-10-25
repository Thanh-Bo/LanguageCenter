using LanguageCenter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Data;

/// <summary>
/// DbContext ánh xạ tới database <c>QuanLyTrungTamNgoaiNgu</c> (32 bảng).
/// Database là nguồn dữ liệu chính (Database First) — không dùng migration để thay đổi schema.
/// Toàn bộ mapping chi tiết nằm trong <c>Data/Configurations</c>, KHÔNG nhồi vào Program.cs.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // 1..9
    public DbSet<VaiTro> VaiTros { get; set; } = null!;
    public DbSet<TaiKhoan> TaiKhoans { get; set; } = null!;
    public DbSet<NhanVien> NhanViens { get; set; } = null!;
    public DbSet<GiaoVien> GiaoViens { get; set; } = null!;
    public DbSet<BangCap> BangCaps { get; set; } = null!;
    public DbSet<ChungChi> ChungChis { get; set; } = null!;
    public DbSet<PhuHuynh> PhuHuynhs { get; set; } = null!;
    public DbSet<HocVien> HocViens { get; set; } = null!;

    // 10..13
    public DbSet<ChuongTrinh> ChuongTrinhs { get; set; } = null!;
    public DbSet<CapDo> CapDos { get; set; } = null!;
    public DbSet<KhoaHoc> KhoaHocs { get; set; } = null!;
    public DbSet<NoiDungBaiHoc> NoiDungBaiHocs { get; set; } = null!;
    public DbSet<KiemTraDauVao> KiemTraDauVaos { get; set; } = null!;

    // 14..20
    public DbSet<PhongHoc> PhongHocs { get; set; } = null!;
    public DbSet<LopHoc> LopHocs { get; set; } = null!;
    public DbSet<PhanCongGiangDay> PhanCongGiangDays { get; set; } = null!;
    public DbSet<UuDai> UuDais { get; set; } = null!;
    public DbSet<NgayLe> NgayLes { get; set; } = null!;
    public DbSet<DangKyHoc> DangKyHocs { get; set; } = null!;
    public DbSet<LichHoc> LichHocs { get; set; } = null!;
    public DbSet<CaHoc> CaHocs { get; set; } = null!;

    // 21..25
    public DbSet<DiemDanh> DiemDanhs { get; set; } = null!;
    public DbSet<LoaiDiem> LoaiDiems { get; set; } = null!;
    public DbSet<DiemSo> DiemSos { get; set; } = null!;
    public DbSet<HocPhi> HocPhis { get; set; } = null!;
    public DbSet<ThanhToan> ThanhToans { get; set; } = null!;

    // 26..32
    public DbSet<BienNhan> BienNhans { get; set; } = null!;
    public DbSet<DichVu> DichVus { get; set; } = null!;
    public DbSet<PhanHoiDichVu> PhanHoiDichVus { get; set; } = null!;
    public DbSet<KhaoSat> KhaoSats { get; set; } = null!;
    public DbSet<CauHoiKhaoSat> CauHoiKhaoSats { get; set; } = null!;
    public DbSet<PhanHoiKhaoSat> PhanHoiKhaoSats { get; set; } = null!;
    public DbSet<CauTraLoiKhaoSat> CauTraLoiKhaoSats { get; set; } = null!;
    public DbSet<ThongBao> ThongBaos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tự động nạp toàn bộ IEntityTypeConfiguration<T> trong assembly này.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
