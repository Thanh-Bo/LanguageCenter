using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Sessions.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="ISessionStore"/>
public sealed class SessionStore : ISessionStore
{
    private readonly AppDbContext _dbContext;

    public SessionStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ========================================================================
    // ĐỌC
    // ========================================================================

    public async Task<PagedResult<SessionListItemDto>> ListAsync(
        SessionListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.CaHocs.AsNoTracking();

        if (query.MaLopHoc is { } maLopHoc)
        {
            source = source.Where(x => x.MaLopHoc == maLopHoc);
        }

        if (query.MaPhongHoc is { } maPhongHoc)
        {
            source = source.Where(x => x.MaPhongHoc == maPhongHoc);
        }

        if (!string.IsNullOrWhiteSpace(query.MaGiaoVien))
        {
            var code = query.MaGiaoVien.Trim();
            source = source.Where(x => x.MaGiaoVienThucTe == code || x.MaTroGiangThucTe == code);
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        if (query.TuNgay is { } tuNgay)
        {
            source = source.Where(x => x.NgayHoc >= tuNgay);
        }

        if (query.DenNgay is { } denNgay)
        {
            source = source.Where(x => x.NgayHoc <= denNgay);
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(x => x.NgayHoc)
            .ThenBy(x => x.GioBatDau)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new SessionListItemDto
            {
                MaCaHoc = x.MaCaHoc,
                MaLopHoc = x.MaLopHoc,
                MaLop = x.LopHoc.MaLop,
                TenLop = x.LopHoc.TenLop,
                MaKhoaHoc = x.LopHoc.MaKhoaHoc,
                TenKhoaHoc = x.LopHoc.KhoaHoc.TenKhoaHoc,
                MaLichHoc = x.MaLichHoc,
                MaPhongHoc = x.MaPhongHoc,
                MaPhong = x.PhongHoc.MaPhong,
                TenPhong = x.PhongHoc.TenPhong,
                MaGiaoVienThucTe = x.MaGiaoVienThucTe,
                TenGiaoVienThucTe = x.GiaoVienThucTe != null ? x.GiaoVienThucTe.HoTen : null,
                MaTroGiangThucTe = x.MaTroGiangThucTe,
                TenTroGiangThucTe = x.TroGiangThucTe != null ? x.TroGiangThucTe.HoTen : null,
                NgayHoc = x.NgayHoc,
                GioBatDau = x.GioBatDau,
                GioKetThuc = x.GioKetThuc,
                NoiDungBaiHoc = x.NoiDungBaiHoc,
                TrangThai = x.TrangThai,
                LyDoThayDoi = x.LyDoThayDoi,
                SoDiemDanh = x.DiemDanhs.Count()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<SessionListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<SessionDetailDto?> GetDetailAsync(int maCaHoc, CancellationToken cancellationToken)
        => _dbContext.CaHocs
            .AsNoTracking()
            .Where(x => x.MaCaHoc == maCaHoc)
            .Select(x => new SessionDetailDto
            {
                MaCaHoc = x.MaCaHoc,
                MaLopHoc = x.MaLopHoc,
                MaLop = x.LopHoc.MaLop,
                TenLop = x.LopHoc.TenLop,
                MaKhoaHoc = x.LopHoc.MaKhoaHoc,
                TenKhoaHoc = x.LopHoc.KhoaHoc.TenKhoaHoc,
                MaLichHoc = x.MaLichHoc,
                MaPhongHoc = x.MaPhongHoc,
                MaPhong = x.PhongHoc.MaPhong,
                TenPhong = x.PhongHoc.TenPhong,
                MaGiaoVienThucTe = x.MaGiaoVienThucTe,
                TenGiaoVienThucTe = x.GiaoVienThucTe != null ? x.GiaoVienThucTe.HoTen : null,
                MaTroGiangThucTe = x.MaTroGiangThucTe,
                TenTroGiangThucTe = x.TroGiangThucTe != null ? x.TroGiangThucTe.HoTen : null,
                NgayHoc = x.NgayHoc,
                GioBatDau = x.GioBatDau,
                GioKetThuc = x.GioKetThuc,
                NoiDungBaiHoc = x.NoiDungBaiHoc,
                TrangThai = x.TrangThai,
                LyDoThayDoi = x.LyDoThayDoi,
                SoDiemDanh = x.DiemDanhs.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maCaHoc, CancellationToken cancellationToken)
        => _dbContext.CaHocs
            .AsNoTracking()
            .AnyAsync(x => x.MaCaHoc == maCaHoc, cancellationToken);

    public Task<int> CountAttendanceAsync(int maCaHoc, CancellationToken cancellationToken)
        => _dbContext.DiemDanhs
            .AsNoTracking()
            .CountAsync(x => x.MaCaHoc == maCaHoc, cancellationToken);

    public Task<bool> HasRoomConflictAsync(
        int maPhongHoc,
        DateOnly ngayHoc,
        TimeOnly gioBatDau,
        TimeOnly gioKetThuc,
        int? exceptId,
        CancellationToken cancellationToken)
        => _dbContext.CaHocs
            .AsNoTracking()
            .AnyAsync(
                x => x.MaPhongHoc == maPhongHoc
                     && x.NgayHoc == ngayHoc
                     && x.TrangThai != SessionStatuses.DaHuy
                     && x.GioBatDau < gioKetThuc
                     && gioBatDau < x.GioKetThuc
                     && (exceptId == null || x.MaCaHoc != exceptId),
                cancellationToken);

    public Task<bool> HasTeacherConflictAsync(
        string maGiaoVien,
        DateOnly ngayHoc,
        TimeOnly gioBatDau,
        TimeOnly gioKetThuc,
        int? exceptId,
        CancellationToken cancellationToken)
        => _dbContext.CaHocs
            .AsNoTracking()
            .AnyAsync(
                x => (x.MaGiaoVienThucTe == maGiaoVien || x.MaTroGiangThucTe == maGiaoVien)
                     && x.NgayHoc == ngayHoc
                     && x.TrangThai != SessionStatuses.DaHuy
                     && x.GioBatDau < gioKetThuc
                     && gioBatDau < x.GioKetThuc
                     && (exceptId == null || x.MaCaHoc != exceptId),
                cancellationToken);

    public Task<bool> HasSameClassSameDayAsync(
        int maLopHoc,
        DateOnly ngayHoc,
        int? exceptId,
        CancellationToken cancellationToken)
        => _dbContext.CaHocs
            .AsNoTracking()
            .AnyAsync(
                x => x.MaLopHoc == maLopHoc
                     && x.NgayHoc == ngayHoc
                     && x.TrangThai != SessionStatuses.DaHuy
                     && (exceptId == null || x.MaCaHoc != exceptId),
                cancellationToken);

    public async Task<IReadOnlyList<SessionListItemDto>> ListByClassIdsAsync(
        IReadOnlyList<int> classIds,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        CancellationToken cancellationToken)
    {
        if (classIds.Count == 0)
        {
            return [];
        }

        var source = _dbContext.CaHocs
            .AsNoTracking()
            .Where(x => classIds.Contains(x.MaLopHoc));

        if (tuNgay is { } tu)
        {
            source = source.Where(x => x.NgayHoc >= tu);
        }

        if (denNgay is { } den)
        {
            source = source.Where(x => x.NgayHoc <= den);
        }

        return await ProjectList(source
                .OrderBy(x => x.NgayHoc)
                .ThenBy(x => x.GioBatDau))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SessionListItemDto>> ListByTeacherAsync(
        string maGiaoVien,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        CancellationToken cancellationToken)
    {
        var code = maGiaoVien.Trim();

        var source = _dbContext.CaHocs
            .AsNoTracking()
            .Where(x => x.MaGiaoVienThucTe == code
                        || x.MaTroGiangThucTe == code
                        // Hoặc được phân công giảng dạy lớp này (kể cả khi chưa gán người dạy thực tế)
                        || _dbContext.PhanCongGiangDays.Any(
                            p => p.MaLopHoc == x.MaLopHoc
                                 && p.MaGiaoVien == code
                                 && p.TrangThai == AssignmentStatuses.DangPhanCong));

        if (tuNgay is { } tu)
        {
            source = source.Where(x => x.NgayHoc >= tu);
        }

        if (denNgay is { } den)
        {
            source = source.Where(x => x.NgayHoc <= den);
        }

        return await ProjectList(source
                .OrderBy(x => x.NgayHoc)
                .ThenBy(x => x.GioBatDau))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TeacherClassDto>> ListTeacherClassesAsync(
        string maGiaoVien,
        CancellationToken cancellationToken)
    {
        var code = maGiaoVien.Trim();

        return await _dbContext.PhanCongGiangDays
            .AsNoTracking()
            .Where(x => x.MaGiaoVien == code && x.TrangThai == AssignmentStatuses.DangPhanCong)
            .OrderBy(x => x.LopHoc.TenLop)
            .Select(x => new TeacherClassDto
            {
                MaLopHoc = x.LopHoc.MaLopHoc,
                MaLop = x.LopHoc.MaLop,
                TenLop = x.LopHoc.TenLop,
                MaKhoaHoc = x.LopHoc.MaKhoaHoc,
                TenKhoaHoc = x.LopHoc.KhoaHoc.TenKhoaHoc,
                SiSo = x.LopHoc.DangKyHocs.Count(
                    d => d.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                         || d.TrangThaiDangKy == EnrollmentStatuses.HoanThanh),
                SiSoToiDa = x.LopHoc.SiSoToiDa,
                TrangThai = x.LopHoc.TrangThai,
                NgayBatDau = x.LopHoc.NgayBatDau,
                NgayKetThucDuKien = x.LopHoc.NgayKetThucDuKien,
                VaiTroPhanCong = x.VaiTroPhanCong
            })
            .ToListAsync(cancellationToken);
    }

    // ========================================================================
    // GHI
    // ========================================================================
    public async Task<WriteResult> CreateAsync(
        CreateSessionRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new CaHoc
        {
            MaLopHoc = request.MaLopHoc,
            MaLichHoc = request.MaLichHoc,
            MaPhongHoc = request.MaPhongHoc,
            MaGiaoVienThucTe = TextValue.Required(request.MaGiaoVienThucTe)?.Trim(),
            MaTroGiangThucTe = TextValue.Required(request.MaTroGiangThucTe)?.Trim(),
            NgayHoc = request.NgayHoc,
            GioBatDau = request.GioBatDau,
            GioKetThuc = request.GioKetThuc,
            NoiDungBaiHoc = TextValue.Required(request.NoiDungBaiHoc),
            TrangThai = SessionStatuses.ChuaDienRa,
            LyDoThayDoi = TextValue.Required(request.LyDoThayDoi)
        };

        _dbContext.CaHocs.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaCaHoc);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maCaHoc,
        UpdateSessionRequest changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.CaHocs
            .FirstOrDefaultAsync(x => x.MaCaHoc == maCaHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.MaPhongHoc is not null)
        {
            entity.MaPhongHoc = changes.MaPhongHoc.Value;
        }

        // Cho phép BỎ TRỐNG giáo viên/trợ giảng bằng chuỗi rỗng.
        if (changes.MaGiaoVienThucTe is not null)
        {
            entity.MaGiaoVienThucTe = TextValue.Required(changes.MaGiaoVienThucTe)?.Trim();
        }

        if (changes.MaTroGiangThucTe is not null)
        {
            entity.MaTroGiangThucTe = TextValue.Required(changes.MaTroGiangThucTe)?.Trim();
        }

        if (changes.NgayHoc is not null)
        {
            entity.NgayHoc = changes.NgayHoc.Value;
        }

        if (changes.GioBatDau is not null)
        {
            entity.GioBatDau = changes.GioBatDau.Value;
        }

        if (changes.GioKetThuc is not null)
        {
            entity.GioKetThuc = changes.GioKetThuc.Value;
        }

        if (changes.NoiDungBaiHoc is not null)
        {
            entity.NoiDungBaiHoc = TextValue.Required(changes.NoiDungBaiHoc);
        }

        if (changes.LyDoThayDoi is not null)
        {
            entity.LyDoThayDoi = TextValue.Required(changes.LyDoThayDoi);
        }

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

    public async Task<WriteOutcome> UpdateStatusAsync(
        int maCaHoc,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.CaHocs
            .FirstOrDefaultAsync(x => x.MaCaHoc == maCaHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.TrangThai = trangThai;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return WriteOutcome.Success;
    }

    public async Task<WriteOutcome> DeleteAsync(int maCaHoc, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.CaHocs
            .FirstOrDefaultAsync(x => x.MaCaHoc == maCaHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        _dbContext.CaHocs.Remove(entity);

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

    // ========================================================================
    // TẠO HÀNG LOẠT TỪ LỊCH ĐỊNH KỲ
    // ========================================================================

    public async Task<GenerateOutcome> GenerateSessionsAsync(
        int maLichHoc,
        DateOnly tuNgay,
        DateOnly denNgay,
        CancellationToken cancellationToken)
    {
        // EnableRetryOnFailure cấm BeginTransaction trực tiếp -> phải bọc ExecutionStrategy.
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(
            () => GenerateCoreAsync(maLichHoc, tuNgay, denNgay, cancellationToken));
    }

    private async Task<GenerateOutcome> GenerateCoreAsync(
        int maLichHoc,
        DateOnly tuNgay,
        DateOnly denNgay,
        CancellationToken cancellationToken)
    {
        var empty = new GenerateOutcome(
            false, maLichHoc, tuNgay, denNgay, null, null, 0, [], [], []);

        var lich = await _dbContext.LichHocs
            .AsNoTracking()
            .Where(x => x.MaLichHoc == maLichHoc)
            .Select(x => new
            {
                x.MaLichHoc,
                x.MaLopHoc,
                x.MaPhongHoc,
                x.ThuTrongTuan,
                x.GioBatDau,
                x.GioKetThuc,
                x.NgayApDungTu,
                x.NgayApDungDen,
                x.TrangThai
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (lich is null)
        {
            return empty;
        }

        // Giao khoảng yêu cầu với khoảng áp dụng của lịch.
        var from = tuNgay > lich.NgayApDungTu ? tuNgay : lich.NgayApDungTu;

        var to = denNgay;

        if (lich.NgayApDungDen is { } gioiHan && gioiHan < to)
        {
            to = gioiHan;
        }

        if (to < from)
        {
            return new GenerateOutcome(
                true, maLichHoc, tuNgay, denNgay, from, to, 0, [], [], []);
        }

        var ngayDaCo = await _dbContext.CaHocs
            .AsNoTracking()
            .Where(x => x.MaLichHoc == maLichHoc && x.NgayHoc >= from && x.NgayHoc <= to)
            .Select(x => x.NgayHoc)
            .ToListAsync(cancellationToken);

        var daCoSet = ngayDaCo.ToHashSet();

        // Ca học đã chiếm phòng trong khoảng này (để kiểm tra xung đột phòng theo từng ngày).
        var caTheoPhong = await _dbContext.CaHocs
            .AsNoTracking()
            .Where(x => x.MaPhongHoc == lich.MaPhongHoc
                        && x.NgayHoc >= from
                        && x.NgayHoc <= to
                        && x.TrangThai != SessionStatuses.DaHuy)
            .Select(x => new { x.NgayHoc, x.GioBatDau, x.GioKetThuc })
            .ToListAsync(cancellationToken);

        var ngayTao = new List<DateOnly>();
        var ngayBoQua = new List<DateOnly>();
        var ngayXungDot = new List<GenerateConflict>();
        var tongNgayKhop = 0;

        for (var ngay = from; ngay <= to; ngay = ngay.AddDays(1))
        {
            if (!MatchesWeekday(ngay, lich.ThuTrongTuan))
            {
                continue;
            }

            tongNgayKhop++;

            if (daCoSet.Contains(ngay))
            {
                ngayBoQua.Add(ngay);
                continue;
            }

            var xungDotPhong = caTheoPhong.Any(
                x => x.NgayHoc == ngay
                     && x.GioBatDau < lich.GioKetThuc
                     && lich.GioBatDau < x.GioKetThuc);

            if (xungDotPhong)
            {
                ngayXungDot.Add(new GenerateConflict(ngay, "Phòng học đã có ca khác trùng giờ."));
                continue;
            }

            _dbContext.CaHocs.Add(new CaHoc
            {
                MaLopHoc = lich.MaLopHoc,
                MaLichHoc = lich.MaLichHoc,
                MaPhongHoc = lich.MaPhongHoc,
                NgayHoc = ngay,
                GioBatDau = lich.GioBatDau,
                GioKetThuc = lich.GioKetThuc,
                TrangThai = SessionStatuses.ChuaDienRa,
                LyDoThayDoi = null
            });

            ngayTao.Add(ngay);
        }

        if (ngayTao.Count > 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new GenerateOutcome(
            true,
            maLichHoc,
            tuNgay,
            denNgay,
            from,
            to,
            tongNgayKhop,
            ngayTao,
            ngayBoQua,
            ngayXungDot);
    }

    /// <summary>DateOnly.DayOfWeek: Chủ Nhật = 0 → quy đổi sang 1..7 với Thứ Hai = 1.</summary>
    private static bool MatchesWeekday(DateOnly ngay, byte thuTrongTuan)
    {
        var dow = ngay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)ngay.DayOfWeek;

        return dow == thuTrongTuan;
    }

    private static IQueryable<SessionListItemDto> ProjectList(IQueryable<CaHoc> source)
        => source.Select(x => new SessionListItemDto
        {
            MaCaHoc = x.MaCaHoc,
            MaLopHoc = x.MaLopHoc,
            MaLop = x.LopHoc.MaLop,
            TenLop = x.LopHoc.TenLop,
            MaKhoaHoc = x.LopHoc.MaKhoaHoc,
            TenKhoaHoc = x.LopHoc.KhoaHoc.TenKhoaHoc,
            MaLichHoc = x.MaLichHoc,
            MaPhongHoc = x.MaPhongHoc,
            MaPhong = x.PhongHoc.MaPhong,
            TenPhong = x.PhongHoc.TenPhong,
            MaGiaoVienThucTe = x.MaGiaoVienThucTe,
            TenGiaoVienThucTe = x.GiaoVienThucTe != null ? x.GiaoVienThucTe.HoTen : null,
            MaTroGiangThucTe = x.MaTroGiangThucTe,
            TenTroGiangThucTe = x.TroGiangThucTe != null ? x.TroGiangThucTe.HoTen : null,
            NgayHoc = x.NgayHoc,
            GioBatDau = x.GioBatDau,
            GioKetThuc = x.GioKetThuc,
            NoiDungBaiHoc = x.NoiDungBaiHoc,
            TrangThai = x.TrangThai,
            LyDoThayDoi = x.LyDoThayDoi,
            SoDiemDanh = x.DiemDanhs.Count()
        });
}
