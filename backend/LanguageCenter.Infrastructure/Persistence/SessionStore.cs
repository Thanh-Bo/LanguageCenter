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
            // Mỗi buổi cố định 2 tiếng — không cho lệch giờ kết thúc.
            GioKetThuc = request.GioBatDau.AddHours(2),
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
            // Mỗi buổi cố định 2 tiếng → giờ kết thúc luôn suy ra từ giờ bắt đầu.
            entity.GioKetThuc = changes.GioBatDau.Value.AddHours(2);
        }
        else if (changes.GioKetThuc is not null)
        {
            entity.GioKetThuc = entity.GioBatDau.AddHours(2);
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
    // TẠO HÀNG LOẠT TỪ LỊCH ĐỊNH KỲ — KHUNG CỐ ĐỊNH 20 BUỔI / 10 TUẦN
    // ========================================================================

    /// <summary>Số buổi cố định của mỗi lớp: 10 tuần × 2 buổi.</summary>
    public const int SoBuoiCoDinh = 20;

    public async Task<GenerateOutcome> GenerateSessionsAsync(
        int maLichHoc,
        CancellationToken cancellationToken)
    {
        // EnableRetryOnFailure cấm BeginTransaction trực tiếp -> phải bọc ExecutionStrategy.
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(
            () => GenerateCoreAsync(maLichHoc, cancellationToken));
    }

    /// <summary>
    /// Sinh ĐÚNG 20 buổi cho lớp của lịch <paramref name="maLichHoc"/>:
    /// <list type="number">
    ///   <item>Lấy 2 lịch định kỳ của lớp (2 thứ khác nhau) → buổi lẻ / buổi chẵn.</item>
    ///   <item>Buổi 1 bắt đầu từ <c>LopHoc.NgayBatDau</c>, mỗi tuần 2 buổi.</item>
    ///   <item>Buổi nào rơi vào <c>NgayLe</c> thì dời +7 ngày VÀ dời toàn bộ buổi phía sau
    ///         để luôn giữ 2 buổi/tuần.</item>
    ///   <item>Buổi thứ i lấy nội dung <c>NoiDungBaiHoc</c> có <c>SoBuoi = i</c> của khóa.</item>
    ///   <item>Cuối cùng cập nhật <c>LopHoc.NgayKetThucDuKien</c> = ngày buổi 20.</item>
    /// </list>
    /// </summary>
    private async Task<GenerateOutcome> GenerateCoreAsync(
        int maLichHoc,
        CancellationToken cancellationToken)
    {
        var lichGoc = await _dbContext.LichHocs
            .AsNoTracking()
            .Where(x => x.MaLichHoc == maLichHoc)
            .Select(x => new { x.MaLichHoc, x.MaLopHoc })
            .FirstOrDefaultAsync(cancellationToken);

        if (lichGoc is null)
        {
            return new GenerateOutcome(false, maLichHoc, default, default, null, null, 0, [], [], []);
        }

        var lop = await _dbContext.LopHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == lichGoc.MaLopHoc)
            .Select(x => new
            {
                x.MaLopHoc,
                x.MaKhoaHoc,
                x.NgayBatDau,
                x.ThuBuoi1,
                x.ThuBuoi2,
                x.GioBatDau,
                x.GioKetThuc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (lop is null)
        {
            return new GenerateOutcome(false, maLichHoc, default, default, null, null, 0, [], [], []);
        }

        // Lịch định kỳ của lớp (nguồn PHÒNG HỌC + dự phòng thứ/giờ).
        var lichCuaLop = await _dbContext.LichHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == lop.MaLopHoc && x.TrangThai != ScheduleStatuses.NgungApDung)
            .OrderBy(x => x.ThuTrongTuan)
            .Select(x => new
            {
                x.MaLichHoc,
                x.MaPhongHoc,
                x.ThuTrongTuan,
                x.GioBatDau,
                x.GioKetThuc
            })
            .ToListAsync(cancellationToken);

        // Thứ học: ƯU TIÊN 2 thứ khai báo trên lớp; chưa có thì lấy từ Lịch học tuần.
        byte? thu1 = lop.ThuBuoi1;
        byte? thu2 = lop.ThuBuoi2;

        if (thu1 is null || thu2 is null)
        {
            if (lichCuaLop.Count < 2)
            {
                return new GenerateOutcome(
                    true, maLichHoc, lop.NgayBatDau, lop.NgayBatDau, null, null, 0, [], [], [])
                {
                    Loi = "Lớp chưa khai báo 2 thứ học. Hãy vào Lớp học → Sửa lớp để chọn "
                          + "Thứ buổi 1 / Thứ buổi 2 trước khi sinh buổi."
                };
            }

            thu1 = lichCuaLop[0].ThuTrongTuan;
            thu2 = lichCuaLop[1].ThuTrongTuan;
        }

        // Sắp xếp để buổi 1 luôn là thứ sớm hơn trong tuần.
        if (thu1 > thu2)
        {
            (thu1, thu2) = (thu2, thu1);
        }

        // Giờ học: ưu tiên giờ của lớp, chưa có thì lấy của lịch, cuối cùng 18:00.
        var gioBatDau = lop.GioBatDau
                        ?? lichCuaLop.FirstOrDefault()?.GioBatDau
                        ?? new TimeOnly(18, 0);

        // MỖI BUỔI CỐ ĐỊNH 2 GIỜ → giờ kết thúc luôn = giờ bắt đầu + 2 giờ.
        var gioKetThuc = gioBatDau.AddHours(2);

        // Phòng học: lịch của lớp → lịch bất kỳ → phòng đầu tiên trong hệ thống.
        var maPhongHoc = lichCuaLop.FirstOrDefault()?.MaPhongHoc
                         ?? await _dbContext.PhongHocs
                             .AsNoTracking()
                             .OrderBy(x => x.MaPhongHoc)
                             .Select(x => (int?)x.MaPhongHoc)
                             .FirstOrDefaultAsync(cancellationToken);

        if (maPhongHoc is null)
        {
            return new GenerateOutcome(
                true, maLichHoc, lop.NgayBatDau, lop.NgayBatDau, null, null, 0, [], [], [])
            {
                Loi = "Hệ thống chưa có phòng học nào. Hãy tạo ít nhất 1 phòng trước khi sinh buổi."
            };
        }

        var maLichLe = lichCuaLop.FirstOrDefault(x => x.ThuTrongTuan == thu1)?.MaLichHoc;
        var maLichChan = lichCuaLop.FirstOrDefault(x => x.ThuTrongTuan == thu2)?.MaLichHoc;

        // Ngày lễ (để dời buổi).
        var ngayLe = (await _dbContext.NgayLes
                .AsNoTracking()
                .Select(x => x.Ngay)
                .ToListAsync(cancellationToken))
            .ToHashSet();

        // Nội dung 20 buổi của khóa.
        var noiDung = await _dbContext.NoiDungBaiHocs
            .AsNoTracking()
            .Where(x => x.MaKhoaHoc == lop.MaKhoaHoc)
            .ToDictionaryAsync(x => x.SoBuoi, x => x.TieuDe, cancellationToken);

        // Giáo viên chính / trợ giảng đang được phân công.
        var phanCong = await _dbContext.PhanCongGiangDays
            .AsNoTracking()
            .Where(x => x.MaLopHoc == lop.MaLopHoc
                        && x.TrangThai == AssignmentStatuses.DangPhanCong
                        && x.MaGiaoVien != null)
            .OrderByDescending(x => x.MaPhanCong)
            .Select(x => new { x.MaGiaoVien, x.VaiTroPhanCong })
            .ToListAsync(cancellationToken);

        var maGiaoVienChinh = phanCong
            .FirstOrDefault(x => x.VaiTroPhanCong == AssignmentRoles.GiaoVienChinh)?.MaGiaoVien;
        var maTroGiang = phanCong
            .FirstOrDefault(x => x.VaiTroPhanCong == AssignmentRoles.TroGiang)?.MaGiaoVien;

        // Ngày đã có ca của lớp (sinh lại không tạo trùng).
        var ngayDaCoSet = (await _dbContext.CaHocs
                .AsNoTracking()
                .Where(x => x.MaLopHoc == lop.MaLopHoc)
                .Select(x => x.NgayHoc)
                .ToListAsync(cancellationToken))
            .ToHashSet();

        // Ca học đã chiếm phòng (kiểm tra xung đột phòng).
        var caTheoPhong = await _dbContext.CaHocs
            .AsNoTracking()
            .Where(x => x.MaPhongHoc == maPhongHoc.Value
                        && x.TrangThai != SessionStatuses.DaHuy)
            .Select(x => new { x.NgayHoc, x.GioBatDau, x.GioKetThuc })
            .ToListAsync(cancellationToken);

        var ngayTao = new List<DateOnly>();
        var ngayBoQua = new List<DateOnly>();
        var ngayXungDot = new List<GenerateConflict>();

        var ngayBuoiDau = TimNgayDauTien(lop.NgayBatDau, thu1!.Value);
        var doLechThu = ((int)thu2!.Value - (int)thu1.Value + 7) % 7;

        // Số ngày phải dời thêm do trùng ngày lễ (cộng dồn cho mọi buổi phía sau).
        var soNgayDoi = 0;

        for (var buoi = 1; buoi <= SoBuoiCoDinh; buoi++)
        {
            var laBuoiLe = buoi % 2 == 1;

            var tuan = (buoi - 1) / 2;
            var lechTrongTuan = laBuoiLe ? 0 : doLechThu;

            var ngay = ngayBuoiDau.AddDays(tuan * 7 + lechTrongTuan + soNgayDoi);

            // Dời nếu trùng ngày lễ (mỗi lần +7 ngày, lặp nếu tuần sau cũng lễ).
            while (ngayLe.Contains(ngay))
            {
                ngay = ngay.AddDays(7);
                soNgayDoi += 7;
                ngayXungDot.Add(new GenerateConflict(
                    ngay.AddDays(-7),
                    $"Buổi {buoi} trùng ngày lễ → dời sang {ngay:dd/MM/yyyy}."));
            }

            if (ngayDaCoSet.Contains(ngay))
            {
                ngayBoQua.Add(ngay);
                continue;
            }

            var xungDotPhong = caTheoPhong.Any(
                x => x.NgayHoc == ngay
                     && x.GioBatDau < gioKetThuc
                     && gioBatDau < x.GioKetThuc);

            if (xungDotPhong)
            {
                ngayXungDot.Add(new GenerateConflict(ngay, "Phòng học đã có ca khác trùng giờ."));
                continue;
            }

            noiDung.TryGetValue(buoi, out var tieuDe);

            _dbContext.CaHocs.Add(new CaHoc
            {
                MaLopHoc = lop.MaLopHoc,
                MaLichHoc = laBuoiLe ? maLichLe : maLichChan,
                MaPhongHoc = maPhongHoc.Value,
                NgayHoc = ngay,
                GioBatDau = gioBatDau,
                GioKetThuc = gioKetThuc,
                MaGiaoVienThucTe = maGiaoVienChinh,
                MaTroGiangThucTe = maTroGiang,
                NoiDungBaiHoc = TaoNoiDungBuoi(buoi, tieuDe),
                TrangThai = SessionStatuses.ChuaDienRa,
                LyDoThayDoi = null
            });

            ngayTao.Add(ngay);
        }

        // Ngày kết thúc lớp = ngày của buổi cuối cùng trong khung 20 buổi.
        var ngayKetThuc = ngayTao.Count > 0            ? ngayTao.Max()
            : ngayBuoiDau.AddDays(9 * 7 + doLechThu + soNgayDoi);

        var lopEntity = await _dbContext.LopHocs
            .FirstOrDefaultAsync(x => x.MaLopHoc == lop.MaLopHoc, cancellationToken);

        if (lopEntity is not null)
        {
            lopEntity.NgayKetThucDuKien = ngayKetThuc;
        }

        if (ngayTao.Count > 0 || lopEntity is not null)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new GenerateOutcome(
            true,
            maLichHoc,
            ngayBuoiDau,
            ngayKetThuc,
            ngayBuoiDau,
            ngayKetThuc,
            SoBuoiCoDinh,
            ngayTao,
            ngayBoQua,
            ngayXungDot);
    }

    /// <summary>Ngày đầu tiên ≥ <paramref name="tuNgay"/> rơi vào <paramref name="thu"/> (1 = Thứ Hai … 7 = CN).</summary>
    private static DateOnly TimNgayDauTien(DateOnly tuNgay, byte thu)
    {
        var dow = tuNgay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)tuNgay.DayOfWeek;
        var lech = ((int)thu - dow + 7) % 7;

        return tuNgay.AddDays(lech);
    }

    /// <summary>DateOnly.DayOfWeek: Chủ Nhật = 0 → quy đổi sang 1..7 với Thứ Hai = 1.</summary>
    private static bool MatchesWeekday(DateOnly ngay, byte thuTrongTuan)
    {
        var dow = ngay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)ngay.DayOfWeek;

        return dow == thuTrongTuan;
    }

    /// <summary>
    /// Nội dung buổi học ghi vào <c>CaHoc.NoiDungBaiHoc</c> dạng "Buổi N — Tiêu đề".
    /// <para>
    /// Dữ liệu seed cũ có tiêu đề đã kèm sẵn tiền tố "Buổi N — " nên phải bỏ đi
    /// để tránh lặp thành "Buổi 1 — Buổi 1 — …".
    /// </para>
    /// </summary>
    private static string TaoNoiDungBuoi(int buoi, string? tieuDe)
    {
        if (string.IsNullOrWhiteSpace(tieuDe))
        {
            return $"Buổi {buoi}";
        }

        var tieuDeSach = tieuDe.Trim();
        var tienTo = $"Buổi {buoi} — ";

        if (tieuDeSach.StartsWith(tienTo, StringComparison.OrdinalIgnoreCase))
        {
            tieuDeSach = tieuDeSach[tienTo.Length..].Trim();
        }

        return tieuDeSach.Length == 0 ? $"Buổi {buoi}" : $"Buổi {buoi} — {tieuDeSach}";
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
