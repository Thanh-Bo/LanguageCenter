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
                MaKhoaHoc = x.MaKhoaHoc,
                TenKhoaHoc = x.KhoaHoc.TenKhoaHoc,
                MaLopHoc = x.MaLopHoc,
                MaLop = x.LopHoc != null ? x.LopHoc.MaLop : null,
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
            MaLopHoc = null, // luôn tạo trước, xếp lớp sau qua AssignClassAsync
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

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
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

    public async Task<WriteOutcome> DeleteAsync(int maDangKy, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.DangKyHocs
            .FirstOrDefaultAsync(x => x.MaDangKy == maDangKy, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
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
