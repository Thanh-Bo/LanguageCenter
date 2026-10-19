using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Attendance.Models;
using LanguageCenter.Application.Common;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IAttendanceStore"/>
public sealed class AttendanceStore : IAttendanceStore
{
    private readonly AppDbContext _dbContext;

    public AttendanceStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SessionAttendanceSheetDto?> GetSheetAsync(
        int maCaHoc,
        CancellationToken cancellationToken)
    {
        var header = await _dbContext.CaHocs
            .AsNoTracking()
            .Where(x => x.MaCaHoc == maCaHoc)
            .Select(x => new
            {
                x.MaCaHoc,
                x.MaLopHoc,
                MaLop = x.LopHoc.MaLop,
                TenLop = x.LopHoc.TenLop,
                x.MaPhongHoc,
                MaPhong = x.PhongHoc.MaPhong,
                TenPhong = x.PhongHoc.TenPhong,
                x.NgayHoc,
                x.GioBatDau,
                x.GioKetThuc,
                x.TrangThai
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (header is null)
        {
            return null;
        }

        var students = await _dbContext.DangKyHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == header.MaLopHoc
                        && (x.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                            || x.TrangThaiDangKy == EnrollmentStatuses.HoanThanh))
            .Select(x => new
            {
                x.MaHocVien,
                HoTen = x.HocVien.HoTen,
                x.MaDangKy
            })
            .ToListAsync(cancellationToken);

        var records = await _dbContext.DiemDanhs
            .AsNoTracking()
            .Where(x => x.MaCaHoc == maCaHoc)
            .Select(x => new
            {
                x.MaHocVien,
                x.MaDiemDanh,
                x.TrangThaiDiemDanh,
                x.GhiChu,
                x.ThoiGianDiemDanh,
                TenTaiKhoan = x.TaiKhoanThucHien.TenDangNhap
            })
            .ToListAsync(cancellationToken);

        var recordMap = records.ToDictionary(x => x.MaHocVien, StringComparer.Ordinal);

        var rows = students
            .OrderBy(x => x.HoTen)
            .ThenBy(x => x.MaHocVien)
            .Select(student =>
            {
                var found = recordMap.GetValueOrDefault(student.MaHocVien);

                return new AttendanceRowDto
                {
                    MaHocVien = student.MaHocVien,
                    HoTen = student.HoTen,
                    MaDangKy = student.MaDangKy,
                    DaDiemDanh = found is not null,
                    MaDiemDanh = found?.MaDiemDanh,
                    TrangThaiDiemDanh = found?.TrangThaiDiemDanh,
                    GhiChu = found?.GhiChu,
                    ThoiGianDiemDanh = found?.ThoiGianDiemDanh,
                    TenTaiKhoanThucHien = found?.TenTaiKhoan
                };
            })
            .ToList();

        return new SessionAttendanceSheetDto
        {
            MaCaHoc = header.MaCaHoc,
            MaLopHoc = header.MaLopHoc,
            MaLop = header.MaLop,
            TenLop = header.TenLop,
            MaPhongHoc = header.MaPhongHoc,
            MaPhong = header.MaPhong,
            TenPhong = header.TenPhong,
            NgayHoc = header.NgayHoc,
            GioBatDau = header.GioBatDau,
            GioKetThuc = header.GioKetThuc,
            TrangThaiCaHoc = header.TrangThai,
            CoTheDiemDanh = SessionStatuses.CanTakeAttendance(header.TrangThai),
            HocViens = rows
        };
    }

    public async Task<WriteOutcome> SaveBatchAsync(
        int maCaHoc,
        IReadOnlyList<AttendanceItemRequest> items,
        int maTaiKhoanThucHien,
        CancellationToken cancellationToken)
    {
        var now = DateTime.Now;

        var existing = await _dbContext.DiemDanhs
            .Where(x => x.MaCaHoc == maCaHoc)
            .ToListAsync(cancellationToken);

        var map = existing.ToDictionary(x => x.MaHocVien, StringComparer.Ordinal);

        foreach (var item in items)
        {
            if (map.TryGetValue(item.MaHocVien, out var row))
            {
                row.TrangThaiDiemDanh = item.TrangThaiDiemDanh;
                row.GhiChu = item.GhiChu;
                row.MaTaiKhoanThucHien = maTaiKhoanThucHien;
                row.ThoiGianDiemDanh = now;
            }
            else
            {
                _dbContext.DiemDanhs.Add(new DiemDanh
                {
                    MaCaHoc = maCaHoc,
                    MaHocVien = item.MaHocVien,
                    TrangThaiDiemDanh = item.TrangThaiDiemDanh,
                    GhiChu = item.GhiChu,
                    MaTaiKhoanThucHien = maTaiKhoanThucHien,
                    ThoiGianDiemDanh = now
                });
            }
        }

        // Một SaveChangesAsync = MỘT transaction ngầm: hoặc ghi hết, hoặc không ghi gì.
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            return WriteOutcome.Conflict;
        }

        return WriteOutcome.Success;
    }

    public Task<bool> AttendanceExistsAsync(int maDiemDanh, CancellationToken cancellationToken)
        => _dbContext.DiemDanhs
            .AsNoTracking()
            .AnyAsync(x => x.MaDiemDanh == maDiemDanh, cancellationToken);

    public Task<int?> GetSessionIdAsync(int maDiemDanh, CancellationToken cancellationToken)
        => _dbContext.DiemDanhs
            .AsNoTracking()
            .Where(x => x.MaDiemDanh == maDiemDanh)
            .Select(x => (int?)x.MaCaHoc)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<WriteOutcome> UpdateAsync(
        int maDiemDanh,
        string trangThaiDiemDanh,
        string? ghiChu,
        int maTaiKhoanThucHien,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.DiemDanhs
            .FirstOrDefaultAsync(x => x.MaDiemDanh == maDiemDanh, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.TrangThaiDiemDanh = trangThaiDiemDanh;
        entity.GhiChu = ghiChu;
        entity.MaTaiKhoanThucHien = maTaiKhoanThucHien;
        entity.ThoiGianDiemDanh = DateTime.Now;

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            return WriteOutcome.Conflict;
        }

        return WriteOutcome.Success;
    }

    public async Task<StudentAttendanceHistoryDto?> GetStudentHistoryAsync(
        string maHocVien,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        CancellationToken cancellationToken)
    {
        var code = maHocVien.Trim();

        var student = await _dbContext.HocViens
            .AsNoTracking()
            .Where(x => x.MaHocVien == code)
            .Select(x => new { x.MaHocVien, x.HoTen })
            .FirstOrDefaultAsync(cancellationToken);

        if (student is null)
        {
            return null;
        }

        var source = _dbContext.DiemDanhs
            .AsNoTracking()
            .Where(x => x.MaHocVien == code);

        if (tuNgay is { } tu)
        {
            source = source.Where(x => x.CaHoc.NgayHoc >= tu);
        }

        if (denNgay is { } den)
        {
            source = source.Where(x => x.CaHoc.NgayHoc <= den);
        }

        var items = await source
            .OrderBy(x => x.CaHoc.NgayHoc)
            .ThenBy(x => x.CaHoc.GioBatDau)
            .Select(x => new StudentAttendanceItemDto
            {
                MaDiemDanh = x.MaDiemDanh,
                MaCaHoc = x.MaCaHoc,
                MaLop = x.CaHoc.LopHoc.MaLop,
                TenLop = x.CaHoc.LopHoc.TenLop,
                TenKhoaHoc = x.CaHoc.LopHoc.KhoaHoc.TenKhoaHoc,
                NgayHoc = x.CaHoc.NgayHoc,
                GioBatDau = x.CaHoc.GioBatDau,
                GioKetThuc = x.CaHoc.GioKetThuc,
                TrangThaiDiemDanh = x.TrangThaiDiemDanh,
                GhiChu = x.GhiChu,
                ThoiGianDiemDanh = x.ThoiGianDiemDanh,
                TrangThaiCaHoc = x.CaHoc.TrangThai
            })
            .ToListAsync(cancellationToken);

        var coMat = items.Count(x => x.TrangThaiDiemDanh == AttendanceStatuses.CoMat);
        var vang = items.Count(x => x.TrangThaiDiemDanh == AttendanceStatuses.Vang);
        var vangCoPhep = items.Count(x => x.TrangThaiDiemDanh == AttendanceStatuses.VangCoPhep);
        var diTre = items.Count(x => x.TrangThaiDiemDanh == AttendanceStatuses.DiTre);
        var tong = items.Count;

        return new StudentAttendanceHistoryDto
        {
            MaHocVien = student.MaHocVien,
            HoTen = student.HoTen,
            TongCaHoc = tong,
            CoMat = coMat,
            Vang = vang,
            VangCoPhep = vangCoPhep,
            DiTre = diTre,
            TiLeCoMat = tong == 0
                ? 0m
                : Math.Round(coMat * 100m / tong, 2, MidpointRounding.AwayFromZero),
            Items = items
        };
    }
}
