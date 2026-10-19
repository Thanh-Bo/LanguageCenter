using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.GradeTypes.Models;
using LanguageCenter.Application.Reports.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IReportStore"/>
/// <remarks>
/// Toàn bộ chỉ số được tính trực tiếp bằng truy vấn tổng hợp trên SQL Server.
/// Không tải toàn bộ bảng lên bộ nhớ, trừ phần điểm số của MỘT lớp (có giới hạn tự nhiên).
/// </remarks>
public sealed class ReportStore : IReportStore
{
    private readonly AppDbContext _dbContext;

    public ReportStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ========================================================================
    // TỔNG QUAN
    // ========================================================================

    public async Task<ReportOverviewDto> GetOverviewAsync(CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var tongTaiKhoan = await _dbContext.TaiKhoans.CountAsync(cancellationToken);
        var tongHocVien = await _dbContext.HocViens.CountAsync(cancellationToken);
        var soHocVienDangHoc = await _dbContext.HocViens
            .CountAsync(x => x.TrangThaiHocTap == StudentStatuses.DangHoc, cancellationToken);
        var tongGiaoVien = await _dbContext.GiaoViens.CountAsync(cancellationToken);
        var tongNhanVien = await _dbContext.NhanViens.CountAsync(cancellationToken);
        var tongChuongTrinh = await _dbContext.ChuongTrinhs.CountAsync(cancellationToken);
        var tongCapDo = await _dbContext.CapDos.CountAsync(cancellationToken);
        var tongKhoaHoc = await _dbContext.KhoaHocs.CountAsync(cancellationToken);
        var tongLopHoc = await _dbContext.LopHocs.CountAsync(cancellationToken);
        var soLopDangHoc = await _dbContext.LopHocs
            .CountAsync(x => x.TrangThai == ClassStatuses.DangHoc, cancellationToken);

        var dangKy = new EnrollmentSummaryDto
        {
            TongSo = await _dbContext.DangKyHocs.CountAsync(cancellationToken),
            ChoXuLy = await _dbContext.DangKyHocs
                .CountAsync(x => x.TrangThaiDangKy == EnrollmentStatuses.ChoXuLy, cancellationToken),
            DaXepLop = await _dbContext.DangKyHocs
                .CountAsync(x => x.TrangThaiDangKy == EnrollmentStatuses.DaXepLop, cancellationToken),
            HoanThanh = await _dbContext.DangKyHocs
                .CountAsync(x => x.TrangThaiDangKy == EnrollmentStatuses.HoanThanh, cancellationToken),
            DaHuy = await _dbContext.DangKyHocs
                .CountAsync(x => x.TrangThaiDangKy == EnrollmentStatuses.DaHuy, cancellationToken)
        };

        var caHoc = new SessionSummaryDto
        {
            TongSo = await _dbContext.CaHocs.CountAsync(cancellationToken),
            ChuaDienRa = await _dbContext.CaHocs
                .CountAsync(x => x.TrangThai == SessionStatuses.ChuaDienRa, cancellationToken),
            DangDienRa = await _dbContext.CaHocs
                .CountAsync(x => x.TrangThai == SessionStatuses.DangDienRa, cancellationToken),
            DaHoanThanh = await _dbContext.CaHocs
                .CountAsync(x => x.TrangThai == SessionStatuses.DaHoanThanh, cancellationToken),
            DaHuy = await _dbContext.CaHocs
                .CountAsync(x => x.TrangThai == SessionStatuses.DaHuy, cancellationToken),
            SapDienRa = await _dbContext.CaHocs.CountAsync(
                x => x.NgayHoc >= today
                     && x.TrangThai != SessionStatuses.DaHuy
                     && x.TrangThai != SessionStatuses.DaHoanThanh,
                cancellationToken)
        };

        var tongLuotDiemDanh = await _dbContext.DiemDanhs.CountAsync(cancellationToken);
        var coMat = await _dbContext.DiemDanhs
            .CountAsync(x => x.TrangThaiDiemDanh == AttendanceStatuses.CoMat, cancellationToken);
        var vang = await _dbContext.DiemDanhs
            .CountAsync(x => x.TrangThaiDiemDanh == AttendanceStatuses.Vang, cancellationToken);
        var vangCoPhep = await _dbContext.DiemDanhs
            .CountAsync(x => x.TrangThaiDiemDanh == AttendanceStatuses.VangCoPhep, cancellationToken);
        var diTre = await _dbContext.DiemDanhs
            .CountAsync(x => x.TrangThaiDiemDanh == AttendanceStatuses.DiTre, cancellationToken);

        var diemDanh = new AttendanceSummaryDto
        {
            TongLuot = tongLuotDiemDanh,
            CoMat = coMat,
            Vang = vang,
            VangCoPhep = vangCoPhep,
            DiTre = diTre,
            TiLeCoMat = tongLuotDiemDanh == 0
                ? 0m
                : Math.Round(coMat * 100m / tongLuotDiemDanh, 2, MidpointRounding.AwayFromZero)
        };

        var hocPhi = new TuitionOverviewDto
        {
            TongPhaiThu = await _dbContext.HocPhis
                .SumAsync(x => (decimal?)x.SoTienPhaiDong, cancellationToken) ?? 0m,
            TongDaThu = await _dbContext.HocPhis
                .SumAsync(x => (decimal?)x.SoTienDaDong, cancellationToken) ?? 0m,
            TongConThieu = await _dbContext.HocPhis
                .SumAsync(x => (decimal?)x.SoTienConThieu, cancellationToken) ?? 0m,
            SoHocPhiConNo = await _dbContext.HocPhis
                .CountAsync(x => x.SoTienConThieu > 0, cancellationToken),
            SoHocVienConNo = await _dbContext.HocPhis
                .Where(x => x.SoTienConThieu > 0)
                .Select(x => x.DangKyHoc.MaHocVien)
                .Distinct()
                .CountAsync(cancellationToken)
        };

        var khaoSat = new SurveyOverviewDto
        {
            TongSo = await _dbContext.KhaoSats.CountAsync(cancellationToken),
            SoHoatDong = await _dbContext.KhaoSats
                .CountAsync(x => x.TrangThai == SurveyStatuses.HoatDong, cancellationToken),
            TongCauHoi = await _dbContext.CauHoiKhaoSats.CountAsync(cancellationToken),
            TongPhanHoi = await _dbContext.PhanHoiKhaoSats.CountAsync(cancellationToken)
        };

        return new ReportOverviewDto
        {
            TongTaiKhoan = tongTaiKhoan,
            TongHocVien = tongHocVien,
            SoHocVienDangHoc = soHocVienDangHoc,
            TongGiaoVien = tongGiaoVien,
            TongNhanVien = tongNhanVien,
            TongChuongTrinh = tongChuongTrinh,
            TongCapDo = tongCapDo,
            TongKhoaHoc = tongKhoaHoc,
            TongLopHoc = tongLopHoc,
            SoLopDangHoc = soLopDangHoc,
            DangKy = dangKy,
            CaHoc = caHoc,
            DiemDanh = diemDanh,
            HocPhi = hocPhi,
            KhaoSat = khaoSat
        };
    }

    // ========================================================================
    // HỌC TẬP
    // ========================================================================

    public async Task<AcademicReportDto?> GetAcademicAsync(
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        var lop = await LoadClassHeaderAsync(maLopHoc, cancellationToken);

        if (lop is null)
        {
            return null;
        }

        var components = await LoadComponentsAsync(lop.MaKhoaHoc, cancellationToken);
        var tongTrongSo = components.Sum(x => x.TrongSo);

        var enrollments = await LoadEnrollmentsAsync(maLopHoc, cancellationToken);

        var scoreMap = await LoadScoreMapAsync(
            enrollments.Select(x => x.MaDangKy).ToList(),
            cancellationToken);

        var rows = new List<AcademicStudentRowDto>(enrollments.Count);

        foreach (var enrollment in enrollments)
        {
            var scores = scoreMap.TryGetValue(enrollment.MaDangKy, out var map)
                ? map
                : new Dictionary<int, decimal>();

            var thanhPhan = new List<AcademicComponentScoreDto>(components.Count);
            var conThieu = new List<string>();

            foreach (var component in components)
            {
                var coDiem = scores.TryGetValue(component.MaLoaiDiem, out var diem);

                thanhPhan.Add(new AcademicComponentScoreDto
                {
                    MaLoaiDiem = component.MaLoaiDiem,
                    TenLoaiDiem = component.TenLoaiDiem,
                    TrongSo = component.TrongSo,
                    DiemToiDa = component.DiemToiDa,
                    Diem = coDiem ? diem : null
                });

                if (!coDiem)
                {
                    conThieu.Add(component.TenLoaiDiem);
                }
            }

            var duDieuKien = conThieu.Count == 0 && thanhPhan.Count > 0;

            rows.Add(new AcademicStudentRowDto
            {
                MaDangKy = enrollment.MaDangKy,
                MaHocVien = enrollment.MaHocVien,
                HoTen = enrollment.HoTen,
                DuDieuKien = duDieuKien,
                DiemTongKet = duDieuKien
                    ? ComputeFinalScore(components, scores)
                    : null,
                ThanhPhan = thanhPhan,
                ThanhPhanConThieu = conThieu
            });
        }

        var diemTongKet = rows
            .Where(x => x.DiemTongKet is not null)
            .Select(x => x.DiemTongKet!.Value)
            .ToList();

        return new AcademicReportDto
        {
            MaLopHoc = lop.MaLopHoc,
            MaLop = lop.MaLop,
            TenLop = lop.TenLop,
            MaKhoaHoc = lop.MaKhoaHoc,
            TenKhoaHoc = lop.TenKhoaHoc,
            SoHocVien = rows.Count,
            TongTrongSo = tongTrongSo,
            CanhBaoTrongSo = tongTrongSo != 100m,
            ThanhPhan = components,
            SoDuDieuKien = rows.Count(x => x.DuDieuKien),
            SoThieuDiem = rows.Count(x => !x.DuDieuKien),
            DiemTongKetTrungBinh = diemTongKet.Count == 0
                ? null
                : Math.Round(diemTongKet.Average(), 2, MidpointRounding.AwayFromZero),
            DiemCaoNhat = diemTongKet.Count == 0 ? null : diemTongKet.Max(),
            DiemThapNhat = diemTongKet.Count == 0 ? null : diemTongKet.Min(),
            HocViens = rows
        };
    }

    // ========================================================================
    // ĐIỂM DANH
    // ========================================================================

    public async Task<AttendanceReportDto?> GetAttendanceAsync(
        int maLopHoc,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        int nguongVang,
        CancellationToken cancellationToken)
    {
        var lop = await LoadClassHeaderAsync(maLopHoc, cancellationToken);

        if (lop is null)
        {
            return null;
        }

        var enrollments = await LoadEnrollmentsAsync(maLopHoc, cancellationToken);

        // Ca học đã hủy KHÔNG được tính vào chuyên cần.
        var sessionQuery = _dbContext.CaHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == maLopHoc && x.TrangThai != SessionStatuses.DaHuy);

        if (tuNgay is { } tu)
        {
            sessionQuery = sessionQuery.Where(x => x.NgayHoc >= tu);
        }

        if (denNgay is { } den)
        {
            sessionQuery = sessionQuery.Where(x => x.NgayHoc <= den);
        }

        var sessionIds = await sessionQuery
            .Select(x => x.MaCaHoc)
            .ToListAsync(cancellationToken);

        var attendanceRows = sessionIds.Count == 0
            ? []
            : await _dbContext.DiemDanhs
                .AsNoTracking()
                .Where(x => sessionIds.Contains(x.MaCaHoc))
                .Select(x => new { x.MaCaHoc, x.MaHocVien, x.TrangThaiDiemDanh })
                .ToListAsync(cancellationToken);

        var soCaHocDaDiemDanh = attendanceRows
            .Select(x => x.MaCaHoc)
            .Distinct()
            .Count();

        var byStudent = attendanceRows
            .GroupBy(x => x.MaHocVien)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.Ordinal);

        var rows = new List<AttendanceStudentRowDto>(enrollments.Count);

        foreach (var enrollment in enrollments)
        {
            var items = byStudent.TryGetValue(enrollment.MaHocVien, out var list) ? list : [];

            var tong = items.Count;
            var coMat = items.Count(x => x.TrangThaiDiemDanh == AttendanceStatuses.CoMat);
            var vang = items.Count(x => x.TrangThaiDiemDanh == AttendanceStatuses.Vang);
            var vangCoPhep = items.Count(x => x.TrangThaiDiemDanh == AttendanceStatuses.VangCoPhep);
            var diTre = items.Count(x => x.TrangThaiDiemDanh == AttendanceStatuses.DiTre);

            var tiLeCoMat = tong == 0
                ? 0m
                : Math.Round(coMat * 100m / tong, 2, MidpointRounding.AwayFromZero);

            var tiLeVang = tong == 0
                ? 0m
                : Math.Round((vang + vangCoPhep) * 100m / tong, 2, MidpointRounding.AwayFromZero);

            rows.Add(new AttendanceStudentRowDto
            {
                MaHocVien = enrollment.MaHocVien,
                HoTen = enrollment.HoTen,
                TongLuot = tong,
                CoMat = coMat,
                Vang = vang,
                VangCoPhep = vangCoPhep,
                DiTre = diTre,
                TiLeCoMat = tiLeCoMat,
                TiLeVang = tiLeVang,
                VangCao = tong > 0 && tiLeVang >= nguongVang
            });
        }

        var tongLuot = rows.Sum(x => x.TongLuot);
        var tongCoMat = rows.Sum(x => x.CoMat);

        return new AttendanceReportDto
        {
            MaLopHoc = lop.MaLopHoc,
            MaLop = lop.MaLop,
            TenLop = lop.TenLop,
            MaKhoaHoc = lop.MaKhoaHoc,
            TenKhoaHoc = lop.TenKhoaHoc,
            TuNgay = tuNgay,
            DenNgay = denNgay,
            SoCaHocDaDiemDanh = soCaHocDaDiemDanh,
            NguongVang = nguongVang,
            TiLeCoMatToanLop = tongLuot == 0
                ? 0m
                : Math.Round(tongCoMat * 100m / tongLuot, 2, MidpointRounding.AwayFromZero),
            HocViens = rows
                .OrderBy(x => x.HoTen)
                .ToList(),
            HocVienVangCao = rows
                .Where(x => x.VangCao)
                .OrderByDescending(x => x.TiLeVang)
                .ThenBy(x => x.HoTen)
                .ToList()
        };
    }

    // ========================================================================
    // KHẢO SÁT
    // ========================================================================

    public async Task<SurveyReportDto> GetSurveyReportAsync(CancellationToken cancellationToken)
    {
        var tongHocVien = await _dbContext.HocViens.CountAsync(cancellationToken);

        var surveys = await _dbContext.KhaoSats
            .AsNoTracking()
            .OrderByDescending(x => x.NgayBatDau)
            .ThenByDescending(x => x.MaKhaoSat)
            .Select(x => new
            {
                x.MaKhaoSat,
                x.TenKhaoSat,
                x.TrangThai,
                x.NgayBatDau,
                x.NgayKetThuc,
                SoCauHoi = x.CauHoiKhaoSats.Count(),
                SoPhanHoi = x.PhanHoiKhaoSats.Count(),
                DiemTrungBinh = x.PhanHoiKhaoSats
                    .Where(p => p.DiemDanhGiaChung != null)
                    .Average(p => (decimal?)p.DiemDanhGiaChung)
            })
            .ToListAsync(cancellationToken);

        var rows = surveys
            .Select(x => new SurveyReportRowDto
            {
                MaKhaoSat = x.MaKhaoSat,
                TenKhaoSat = x.TenKhaoSat,
                TrangThai = x.TrangThai,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                SoCauHoi = x.SoCauHoi,
                SoPhanHoi = x.SoPhanHoi,
                DiemTrungBinh = x.DiemTrungBinh is null
                    ? null
                    : Math.Round(x.DiemTrungBinh.Value, 2, MidpointRounding.AwayFromZero),
                TiLePhanHoi = tongHocVien == 0
                    ? null
                    : Math.Round(x.SoPhanHoi * 100m / tongHocVien, 2, MidpointRounding.AwayFromZero)
            })
            .ToList();

        var diemChung = await _dbContext.PhanHoiKhaoSats
            .AsNoTracking()
            .Where(x => x.DiemDanhGiaChung != null)
            .Select(x => x.DiemDanhGiaChung!.Value)
            .ToListAsync(cancellationToken);

        return new SurveyReportDto
        {
            TongKhaoSat = rows.Count,
            SoHoatDong = rows.Count(x => x.TrangThai == SurveyStatuses.HoatDong),
            SoTamDung = rows.Count(x => x.TrangThai == SurveyStatuses.TamDung),
            SoDaKetThuc = rows.Count(x => x.TrangThai == SurveyStatuses.DaKetThuc),
            TongCauHoi = await _dbContext.CauHoiKhaoSats.CountAsync(cancellationToken),
            TongPhanHoi = await _dbContext.PhanHoiKhaoSats.CountAsync(cancellationToken),
            DiemDanhGiaChungTrungBinh = diemChung.Count == 0
                ? null
                : Math.Round(diemChung.Average(), 2, MidpointRounding.AwayFromZero),
            TongHocVien = tongHocVien,
            KhaoSats = rows
        };
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private sealed record ClassHeader(int MaLopHoc, string MaLop, string TenLop, int MaKhoaHoc, string TenKhoaHoc);

    private sealed record EnrollmentRow(int MaDangKy, string MaHocVien, string HoTen);

    private Task<ClassHeader?> LoadClassHeaderAsync(int maLopHoc, CancellationToken cancellationToken)
        => _dbContext.LopHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == maLopHoc)
            .Select(x => new ClassHeader(
                x.MaLopHoc,
                x.MaLop,
                x.TenLop,
                x.MaKhoaHoc,
                x.KhoaHoc.TenKhoaHoc))
            .FirstOrDefaultAsync(cancellationToken);

    private async Task<IReadOnlyList<GradeComponentInfo>> LoadComponentsAsync(
        int maKhoaHoc,
        CancellationToken cancellationToken)
        => await _dbContext.LoaiDiems
            .AsNoTracking()
            .Where(x => x.MaKhoaHoc == maKhoaHoc && x.TrangThai == GradeTypeStatuses.HoatDong)
            .OrderBy(x => x.ThuTu)
            .Select(x => new GradeComponentInfo(
                x.MaLoaiDiem,
                x.TenLoaiDiem,
                x.TrongSo,
                x.DiemToiDa,
                x.ThuTu))
            .ToListAsync(cancellationToken);

    private async Task<IReadOnlyList<EnrollmentRow>> LoadEnrollmentsAsync(
        int maLopHoc,
        CancellationToken cancellationToken)
        => await _dbContext.DangKyHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == maLopHoc
                        && (x.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                            || x.TrangThaiDangKy == EnrollmentStatuses.HoanThanh))
            .OrderBy(x => x.HocVien.HoTen)
            .Select(x => new EnrollmentRow(x.MaDangKy, x.MaHocVien, x.HocVien.HoTen))
            .ToListAsync(cancellationToken);

    private async Task<Dictionary<int, Dictionary<int, decimal>>> LoadScoreMapAsync(
        IReadOnlyList<int> maDangKys,
        CancellationToken cancellationToken)
    {
        if (maDangKys.Count == 0)
        {
            return [];
        }

        var rows = await _dbContext.DiemSos
            .AsNoTracking()
            .Where(x => maDangKys.Contains(x.MaDangKy))
            .Select(x => new { x.MaDangKy, x.MaLoaiDiem, x.Diem })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(x => x.MaDangKy)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(x => x.MaLoaiDiem, x => x.Diem));
    }

    /// <summary>
    /// Công thức điểm tổng kết đã chốt ở Phase 5 (Q6):
    /// <c>10 × Σ((Diem_i / DiemToiDa_i) × TrongSo_i) / Σ(TrongSo_i)</c>, làm tròn 2 chữ số.
    /// </summary>
    private static decimal? ComputeFinalScore(
        IReadOnlyList<GradeComponentInfo> components,
        IReadOnlyDictionary<int, decimal> scores)
    {
        var tongTrongSo = components.Sum(x => x.TrongSo);

        if (tongTrongSo <= 0m)
        {
            return null;
        }

        var tuSo = 0m;

        foreach (var component in components)
        {
            if (!scores.TryGetValue(component.MaLoaiDiem, out var diem))
            {
                return null;
            }

            var tyLe = component.DiemToiDa > 0m ? diem / component.DiemToiDa : 0m;

            tuSo += tyLe * component.TrongSo;
        }

        return Math.Round(10m * tuSo / tongTrongSo, 2, MidpointRounding.AwayFromZero);
    }
}
