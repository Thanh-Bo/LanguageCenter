using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Enrollments.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IEnrollmentStore"/>
public sealed class EnrollmentStore : IEnrollmentStore
{
    private readonly AppDbContext _dbContext;

    public EnrollmentStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<EnrollmentListItemDto>> ListAsync(
        EnrollmentListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.DangKyHocs.AsNoTracking();

        if (query.MaKhoaHoc is { } maKhoaHoc)
        {
            source = source.Where(x => x.MaKhoaHoc == maKhoaHoc);
        }

        if (query.MaLopHoc is { } maLopHoc)
        {
            source = source.Where(x => x.MaLopHoc == maLopHoc);
        }

        if (!string.IsNullOrWhiteSpace(query.MaHocVien))
        {
            var maHocVien = query.MaHocVien.Trim();
            source = source.Where(x => x.MaHocVien == maHocVien);
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThaiDangKy == trangThai);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            source = source.Where(
                x => x.MaHocVien.Contains(search)
                     || x.HocVien.HoTen.Contains(search)
                     || x.KhoaHoc.TenKhoaHoc.Contains(search));
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderByDescending(x => x.MaDangKy)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new EnrollmentListItemDto
            {
                MaDangKy = x.MaDangKy,
                MaHocVien = x.MaHocVien,
                TenHocVien = x.HocVien.HoTen,
                // Thông tin cá nhân: lấy từ hồ sơ học viên để màn Xếp lớp không bị trống.
                NgaySinh = x.HocVien.NgaySinh,
                GioiTinh = x.HocVien.GioiTinh,
                SoDienThoai = x.HocVien.SoDienThoai,
                Email = x.HocVien.Email,
                MaKhoaHoc = x.MaKhoaHoc,
                TenKhoaHoc = x.KhoaHoc.TenKhoaHoc,
                MaLopHoc = x.MaLopHoc,
                MaLop = x.LopHoc != null ? x.LopHoc.MaLop : null,
                MaLopDuKien = x.MaLopDuKien,
                MaLopDuKienText = x.MaLopDuKien != null
                    ? _dbContext.LopHocs.Where(l => l.MaLopHoc == x.MaLopDuKien)
                        .Select(l => l.MaLop + " — " + l.TenLop).FirstOrDefault()
                    : null,
                SoTienConThieuHocPhi = x.HocPhi != null ? x.HocPhi.SoTienConThieu : null,
                TrangThaiHocPhi = x.HocPhi != null ? x.HocPhi.TrangThaiHocPhi : null,
                DaThuDuChuaXepDuocLop =
                    x.MaLopHoc == null
                    && x.HocPhi != null
                    && x.HocPhi.SoTienConThieu <= 0m
                    && x.TrangThaiDangKy == EnrollmentStatuses.ChoXuLy,
                NgayDangKy = x.NgayDangKy,
                HocPhiGoc = x.HocPhiGoc,
                SoTienGiam = x.SoTienGiam,
                HocPhiSauGiam = x.HocPhiSauGiam,
                TrangThaiDangKy = x.TrangThaiDangKy,
                NgayXepLop = x.NgayXepLop
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<EnrollmentListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<EnrollmentDetailDto?> GetDetailAsync(int maDangKy, CancellationToken cancellationToken)
        => _dbContext.DangKyHocs
            .AsNoTracking()
            .Where(x => x.MaDangKy == maDangKy)
            .Select(x => new EnrollmentDetailDto
            {
                MaDangKy = x.MaDangKy,
                MaHocVien = x.MaHocVien,
                TenHocVien = x.HocVien.HoTen,
                MaKhoaHoc = x.MaKhoaHoc,
                TenKhoaHoc = x.KhoaHoc.TenKhoaHoc,
                MaLopHoc = x.MaLopHoc,
                MaLop = x.LopHoc != null ? x.LopHoc.MaLop : null,
                MaLopDuKien = x.MaLopDuKien,
                MaLopDuKienText = x.MaLopDuKien != null
                    ? _dbContext.LopHocs.Where(l => l.MaLopHoc == x.MaLopDuKien)
                        .Select(l => l.MaLop + " — " + l.TenLop).FirstOrDefault()
                    : null,
                MaNhanVienTuVan = x.MaNhanVienTuVan,
                TenNhanVienTuVan = x.NhanVienTuVan.HoTen,
                MaUuDai = x.MaUuDai,
                TenUuDai = x.UuDai != null ? x.UuDai.TenUuDai : null,
                NgayDangKy = x.NgayDangKy,
                LichHocMongMuon = x.LichHocMongMuon,
                HocPhiGoc = x.HocPhiGoc,
                SoTienGiam = x.SoTienGiam,
                HocPhiSauGiam = x.HocPhiSauGiam,
                NgayXepLop = x.NgayXepLop,
                TrangThaiDangKy = x.TrangThaiDangKy,
                GhiChu = x.GhiChu
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maDangKy, CancellationToken cancellationToken)
        => _dbContext.DangKyHocs
            .AsNoTracking()
            .AnyAsync(x => x.MaDangKy == maDangKy, cancellationToken);

    public Task<bool> HasActiveDuplicateAsync(
        string maHocVien,
        int maKhoaHoc,
        int? exceptId,
        CancellationToken cancellationToken)
    {
        var code = maHocVien.Trim();

        return _dbContext.DangKyHocs
            .AsNoTracking()
            .AnyAsync(
                x => x.MaHocVien == code
                     && x.MaKhoaHoc == maKhoaHoc
                     && (x.TrangThaiDangKy == EnrollmentStatuses.ChoXuLy
                         || x.TrangThaiDangKy == EnrollmentStatuses.DaXepLop)
                     && (exceptId == null || x.MaDangKy != exceptId),
                cancellationToken);
    }

    public async Task<WriteResult> CreateAsync(
        EnrollmentCreateData data,
        CancellationToken cancellationToken)
    {
        var entity = new DangKyHoc
        {
            MaHocVien = data.MaHocVien,
            MaKhoaHoc = data.MaKhoaHoc,
            MaLopHoc = null, // chỉ được gán khi thu đủ học phí (tự xếp) hoặc xếp tay
            MaLopDuKien = data.MaLopDuKien,
            MaNhanVienTuVan = data.MaNhanVienTuVan,
            MaUuDai = data.MaUuDai,
            LichHocMongMuon = data.LichHocMongMuon,
            HocPhiGoc = data.HocPhiGoc,
            SoTienGiam = data.SoTienGiam,
            GhiChu = data.GhiChu,
            TrangThaiDangKy = EnrollmentStatuses.ChoXuLy
            // NgayDangKy: database tự set SYSDATETIME()
            // HocPhiSauGiam: cột computed PERSISTED, KHÔNG gán
        };

        _dbContext.DangKyHocs.Add(entity);

        // Phiếu học phí được sinh NGAY khi tạo đăng ký, trong CÙNG transaction:
        // trung tâm thu tiền trước khi xếp lớp nên màn Học phí phải có phiếu để thu.
        // Gắn qua navigation để EF tự điền MaDangKy sau khi INSERT đăng ký.
        HocPhi? hocPhi = null;

        if (EnrollmentRules.TuDongTaoPhieuHocPhi)
        {
            var soTienPhaiDong = data.HocPhiGoc - data.SoTienGiam;

            hocPhi = new HocPhi
            {
                DangKyHoc = entity,
                SoTienGoc = data.HocPhiGoc,
                SoTienGiam = data.SoTienGiam,
                SoTienDaDong = 0m,
                HanThanhToan = null,
                TrangThaiHocPhi = TuitionStatuses.FromAmounts(soTienPhaiDong, 0m),
                NgayTao = DateTime.Now
                // SoTienPhaiDong / SoTienConThieu: computed PERSISTED — KHÔNG gán
            };

            _dbContext.HocPhis.Add(hocPhi);
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;

            if (hocPhi is not null)
            {
                _dbContext.Entry(hocPhi).State = EntityState.Detached;
            }

            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaDangKy);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maDangKy,
        EnrollmentUpdateData changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.DangKyHocs
            .FirstOrDefaultAsync(x => x.MaDangKy == maDangKy, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.MaUuDai = changes.MaUuDai;
        entity.SoTienGiam = changes.SoTienGiam;
        entity.LichHocMongMuon = changes.LichHocMongMuon;
        entity.GhiChu = changes.GhiChu;

        if (changes.MaLopDuKien is { } maLopDuKien)
        {
            entity.MaLopDuKien = maLopDuKien;
        }

        // Phiếu học phí đi kèm phải phản ánh đúng số tiền của đăng ký.
        // CHỈ đồng bộ khi phiếu chưa có đồng nào được thu (giữ dấu vết tài chính).
        var hocPhi = await _dbContext.HocPhis
            .FirstOrDefaultAsync(x => x.MaDangKy == maDangKy, cancellationToken);

        if (hocPhi is not null && hocPhi.SoTienDaDong <= 0m)
        {
            hocPhi.SoTienGoc = entity.HocPhiGoc;
            hocPhi.SoTienGiam = entity.SoTienGiam;
            hocPhi.TrangThaiHocPhi = TuitionStatuses.FromAmounts(
                entity.HocPhiGoc - entity.SoTienGiam,
                hocPhi.SoTienDaDong);
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            // Ví dụ CK_DangKyHoc_HocPhi: SoTienGiam > HocPhiGoc.
            return WriteOutcome.Conflict;
        }

        return WriteOutcome.Success;
    }

    /// <summary>Bỏ lớp: xóa lớp đã gán, trả đăng ký về trạng thái "Chờ xử lý".</summary>
    public async Task<WriteOutcome> UnassignClassAsync(int maDangKy, CancellationToken cancellationToken)
    {
        var enrollment = await _dbContext.DangKyHocs
            .FirstOrDefaultAsync(x => x.MaDangKy == maDangKy, cancellationToken);

        if (enrollment is null)
        {
            return WriteOutcome.NotFound;
        }

        if (enrollment.MaLopHoc is null)
        {
            return WriteOutcome.Conflict;
        }

        if (enrollment.TrangThaiDangKy == EnrollmentStatuses.HoanThanh)
        {
            return WriteOutcome.Conflict;
        }

        enrollment.MaLopHoc = null;
        enrollment.NgayXepLop = null;
        enrollment.TrangThaiDangKy = EnrollmentStatuses.ChoXuLy;

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return WriteOutcome.Success;
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            return WriteOutcome.Conflict;
        }
    }

    public async Task<AssignClassOutcome> AssignClassAsync(
        int maDangKy,
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        // EnableRetryOnFailure() được bật ở Phase 1 -> transaction tự mở phải nằm trong ExecutionStrategy.
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(
            state: (MaDangKy: maDangKy, MaLopHoc: maLopHoc),
            operation: (_, state, token) => AssignClassCoreAsync(state.MaDangKy, state.MaLopHoc, token),
            verifySucceeded: null,
            cancellationToken: cancellationToken);
    }

    private async Task<AssignClassOutcome> AssignClassCoreAsync(
        int maDangKy,
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            var enrollment = await _dbContext.DangKyHocs
                .FirstOrDefaultAsync(x => x.MaDangKy == maDangKy, cancellationToken);

            if (enrollment is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AssignClassOutcome.EnrollmentNotFound;
            }

            if (enrollment.MaLopHoc is not null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AssignClassOutcome.AlreadyAssigned;
            }

            // KHÓA dòng lớp: các request đồng thời phải chờ nhau -> không thể cùng vượt sĩ số.
            var lopHoc = await _dbContext.LopHocs
                .FromSqlInterpolated(
                    $"SELECT * FROM LopHoc WITH (UPDLOCK, HOLDLOCK) WHERE MaLopHoc = {maLopHoc}")
                .FirstOrDefaultAsync(cancellationToken);

            if (lopHoc is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AssignClassOutcome.ClassNotFound;
            }

            if (lopHoc.MaKhoaHoc != enrollment.MaKhoaHoc)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AssignClassOutcome.WrongCourse;
            }

            if (!ClassStatuses.CanAcceptStudents(lopHoc.TrangThai))
            {
                await transaction.RollbackAsync(cancellationToken);
                return AssignClassOutcome.ClassClosed;
            }

            var currentCount = await _dbContext.DangKyHocs.CountAsync(
                d => d.MaLopHoc == maLopHoc
                     && (d.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                         || d.TrangThaiDangKy == EnrollmentStatuses.HoanThanh),
                cancellationToken);

            if (currentCount >= lopHoc.SiSoToiDa)
            {
                await transaction.RollbackAsync(cancellationToken);
                return AssignClassOutcome.ClassFull;
            }

            enrollment.MaLopHoc = maLopHoc;
            enrollment.NgayXepLop = DateTime.Now;
            enrollment.TrangThaiDangKy = EnrollmentStatuses.DaXepLop;

            // Ngày nhập học = ngày BẮT ĐẦU của lớp (đồng bộ sang hồ sơ học viên để màn Quản lý học viên hiển thị đúng).
            // Học viên học nhiều lớp → giữ ngày bắt đầu SỚM NHẤT.
            var hocVien = await _dbContext.HocViens
                .FirstOrDefaultAsync(x => x.MaHocVien == enrollment.MaHocVien, cancellationToken);

            if (hocVien is not null
                && (hocVien.NgayNhapHoc is not { } ngayCu || ngayCu > lopHoc.NgayBatDau))
            {
                hocVien.NgayNhapHoc = lopHoc.NgayBatDau;
            }

            // Vào lớp = đã nhập học. CHỈ nâng từ "Đang tư vấn" → "Đang học",
            // không ghi đè "Bảo lưu" / "Đã nghỉ" mà nhân viên đã cố ý đặt.
            if (hocVien is not null
                && hocVien.TrangThaiHocTap == StudentStatuses.DangTuVan)
            {
                hocVien.TrangThaiHocTap = StudentStatuses.DangHoc;
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return AssignClassOutcome.Success;
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            await transaction.RollbackAsync(cancellationToken);
            return AssignClassOutcome.Conflict;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<WriteOutcome> UpdateStatusAsync(
        int maDangKy,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.DangKyHocs
            .FirstOrDefaultAsync(x => x.MaDangKy == maDangKy, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.TrangThaiDangKy = trangThai;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return WriteOutcome.Success;
    }

    public async Task<PlacementContext?> GetPlacementContextAsync(
        int maDangKy,
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        var maHocVien = await _dbContext.DangKyHocs
            .AsNoTracking()
            .Where(x => x.MaDangKy == maDangKy)
            .Select(x => x.MaHocVien)
            .FirstOrDefaultAsync(cancellationToken);

        if (maHocVien is null)
        {
            return null;
        }

        return await BuildPlacementContextAsync(maHocVien, maLopHoc, maDangKy, cancellationToken);
    }

    public Task<PlacementContext?> GetPlacementContextForStudentAsync(
        string maHocVien,
        int maLopHoc,
        int? exceptDangKyId,
        CancellationToken cancellationToken)
        => BuildPlacementContextAsync(maHocVien, maLopHoc, exceptDangKyId, cancellationToken);

    /// <summary>
    /// Dựng ngữ cảnh kiểm tra xếp lớp. Dùng chung cho 2 tình huống:
    /// <list type="bullet">
    ///   <item>Đã có đăng ký (<paramref name="maDangKyKiemTra"/>) — lúc xếp lớp / kiểm tra lại sau khi thu tiền.</item>
    ///   <item>Chưa có đăng ký (truyền <c>null</c>) — lúc TẠO đăng ký, chỉ có học viên + lớp dự kiến.</item>
    /// </list>
    /// </summary>
    private async Task<PlacementContext?> BuildPlacementContextAsync(
        string maHocVien,
        int maLopHoc,
        int? maDangKyKiemTra,
        CancellationToken cancellationToken)
    {
        var lop = await _dbContext.LopHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == maLopHoc)
            .Select(x => new
            {
                x.MaLopHoc,
                x.MaLop,
                x.TenLop,
                x.NgayBatDau,
                x.NgayKetThucDuKien,
                x.SiSoToiDa,
                SoBuoi = x.KhoaHoc.SoBuoi,
                DoTuoiTu = x.KhoaHoc.CapDo.ChuongTrinh.DoTuoiTu,
                DoTuoiDen = x.KhoaHoc.CapDo.ChuongTrinh.DoTuoiDen
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (lop is null)
        {
            return null;
        }

        var ngaySinh = await _dbContext.HocViens
            .AsNoTracking()
            .Where(x => x.MaHocVien == maHocVien)
            .Select(x => (DateOnly?)x.NgaySinh)
            .FirstOrDefaultAsync(cancellationToken);

        if (ngaySinh is null)
        {
            return null;
        }

        var today = DateOnly.FromDateTime(DateTime.Today);

        var soBuoiDaDienRa = await _dbContext.CaHocs
            .AsNoTracking()
            .CountAsync(
                x => x.MaLopHoc == maLopHoc
                     && x.TrangThai != SessionStatuses.DaHuy
                     && x.NgayHoc <= today,
                cancellationToken);

        // Sĩ số: đã xếp lớp thật + đang giữ chỗ (đăng ký chờ xử lý có lớp dự kiến = lớp này).
        var soDaXepLop = await _dbContext.DangKyHocs
            .AsNoTracking()
            .CountAsync(
                x => x.MaLopHoc == maLopHoc
                     && (x.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                         || x.TrangThaiDangKy == EnrollmentStatuses.HoanThanh),
                cancellationToken);

        var soGiuCho = await _dbContext.DangKyHocs
            .AsNoTracking()
            .CountAsync(
                x => x.MaLopDuKien == maLopHoc
                     && x.MaLopHoc == null
                     && x.TrangThaiDangKy == EnrollmentStatuses.ChoXuLy
                     && (maDangKyKiemTra == null || x.MaDangKy != maDangKyKiemTra),
                cancellationToken);

        // Các lớp khác mà học viên đang theo học (để kiểm tra trùng giờ).
        var lopKhacIds = await _dbContext.DangKyHocs
            .AsNoTracking()
            .Where(x => x.MaHocVien == maHocVien
                        && x.MaLopHoc != null
                        && x.MaLopHoc != maLopHoc
                        && (x.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                            || x.TrangThaiDangKy == EnrollmentStatuses.HoanThanh)
                        && (maDangKyKiemTra == null || x.MaDangKy != maDangKyKiemTra))
            .Select(x => x.MaLopHoc!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);

        var classIds = new List<int> { maLopHoc };
        classIds.AddRange(lopKhacIds);

        var lops = await _dbContext.LopHocs
            .AsNoTracking()
            .Where(x => classIds.Contains(x.MaLopHoc))
            .Select(x => new
            {
                x.MaLopHoc,
                x.MaLop,
                x.ThuBuoi1,
                x.ThuBuoi2,
                x.GioBatDau,
                x.GioKetThuc,
                x.NgayBatDau,
                NgayDen = x.NgayKetThucDuKien
            })
            .ToListAsync(cancellationToken);

        var lichRows = await _dbContext.LichHocs
            .AsNoTracking()
            .Where(x => classIds.Contains(x.MaLopHoc)
                        && x.TrangThai != ScheduleStatuses.NgungApDung)
            .Select(x => new
            {
                x.MaLopHoc,
                x.ThuTrongTuan,
                x.GioBatDau,
                x.GioKetThuc,
                x.NgayApDungTu,
                x.NgayApDungDen
            })
            .ToListAsync(cancellationToken);

        var slotsTheoLop = new Dictionary<int, List<ClassScheduleSlot>>();

        foreach (var item in lops)
        {
            var slots = new List<ClassScheduleSlot>();

            var lichCuaLop = lichRows.Where(x => x.MaLopHoc == item.MaLopHoc).ToList();

            if (lichCuaLop.Count > 0)
            {
                slots.AddRange(lichCuaLop.Select(x => new ClassScheduleSlot(
                    item.MaLopHoc,
                    item.MaLop,
                    x.ThuTrongTuan,
                    x.GioBatDau,
                    x.GioKetThuc,
                    x.NgayApDungTu,
                    // Ngày kết thúc phải là của CHÍNH LỚP NÀY, không phải của lớp đang xếp.
                    // Trước đây để null → service fallback sang NgayKetThucDuKien (lớp MỚI)
                    // nên vừa bỏ sót trùng lịch, vừa báo sai lớp.
                    x.NgayApDungDen ?? item.NgayDen)));
            }
            else if (item.GioBatDau is { } gioBatDau && item.GioKetThuc is { } gioKetThuc)
            {
                // Lớp khai 2 thứ trực tiếp trên LopHoc (không có dòng LichHoc).
                if (item.ThuBuoi1 is { } thu1)
                {
                    slots.Add(new ClassScheduleSlot(
                        item.MaLopHoc, item.MaLop, thu1, gioBatDau, gioKetThuc, item.NgayBatDau, item.NgayDen));
                }

                if (item.ThuBuoi2 is { } thu2)
                {
                    slots.Add(new ClassScheduleSlot(
                        item.MaLopHoc, item.MaLop, thu2, gioBatDau, gioKetThuc, item.NgayBatDau, item.NgayDen));
                }
            }

            slotsTheoLop[item.MaLopHoc] = slots;
        }

        var slotsMoi = slotsTheoLop.TryGetValue(maLopHoc, out var sm)
            ? sm
            : new List<ClassScheduleSlot>();

        var slotsCu = new List<ClassScheduleSlot>();

        foreach (var id in lopKhacIds)
        {
            if (slotsTheoLop.TryGetValue(id, out var sc))
            {
                slotsCu.AddRange(sc);
            }
        }

        // Chỉ tra phiếu học phí khi đang kiểm tra một đăng ký có thật.
        var hocPhi = maDangKyKiemTra is { } idKiemTra
            ? await _dbContext.HocPhis
                .AsNoTracking()
                .Where(x => x.MaDangKy == idKiemTra)
                .Select(x => new { x.SoTienConThieu, x.TrangThaiHocPhi })
                .FirstOrDefaultAsync(cancellationToken)
            : null;

        return new PlacementContext(
            lop.MaLopHoc,
            lop.MaLop,
            lop.TenLop,
            lop.NgayBatDau,
            lop.NgayKetThucDuKien,
            maHocVien,
            ngaySinh.Value,
            lop.DoTuoiTu,
            lop.DoTuoiDen,
            soBuoiDaDienRa,
            lop.SoBuoi,
            slotsMoi,
            slotsCu)
        {
            SoTienConThieuHocPhi = hocPhi?.SoTienConThieu,
            TrangThaiHocPhi = hocPhi?.TrangThaiHocPhi,
            SiSoToiDa = lop.SiSoToiDa,
            SoDaXepLop = soDaXepLop,
            SoGiuCho = soGiuCho
        };
    }

    public async Task<WriteOutcome> DeleteAsync(int maDangKy, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.DangKyHocs
            .FirstOrDefaultAsync(x => x.MaDangKy == maDangKy, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        // Phiếu học phí sinh kèm đăng ký → phải xoá trước (FK Restrict).
        var hocPhi = await _dbContext.HocPhis
            .FirstOrDefaultAsync(x => x.MaDangKy == maDangKy, cancellationToken);

        if (hocPhi is not null)
        {
            // Đã thu tiền rồi thì KHÔNG xoá — giữ dấu vết tài chính.
            var daThuTien = await _dbContext.ThanhToans
                .AnyAsync(x => x.MaHocPhi == hocPhi.MaHocPhi, cancellationToken);

            if (daThuTien)
            {
                return WriteOutcome.Conflict;
            }

            _dbContext.HocPhis.Remove(hocPhi);
        }

        _dbContext.DangKyHocs.Remove(entity);

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
}
