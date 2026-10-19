using LanguageCenter.Application.Abstractions;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IAcademicLookupStore"/>
public sealed class AcademicLookupStore : IAcademicLookupStore
{
    private readonly AppDbContext _dbContext;

    public AcademicLookupStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> RoomExistsAsync(int maPhongHoc, CancellationToken cancellationToken)
        => _dbContext.PhongHocs
            .AsNoTracking()
            .AnyAsync(x => x.MaPhongHoc == maPhongHoc, cancellationToken);

    public Task<bool> CourseExistsAsync(int maKhoaHoc, CancellationToken cancellationToken)
        => _dbContext.KhoaHocs
            .AsNoTracking()
            .AnyAsync(x => x.MaKhoaHoc == maKhoaHoc, cancellationToken);

    public Task<ClassInfo?> GetClassInfoAsync(int maLopHoc, CancellationToken cancellationToken)
        => _dbContext.LopHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == maLopHoc)
            .Select(x => new ClassInfo(
                x.MaLopHoc,
                x.MaLop,
                x.TenLop,
                x.MaKhoaHoc,
                x.TrangThai,
                x.MaNhanVienHocVu))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> TeacherAssignedToClassAsync(
        int maLopHoc,
        string maGiaoVien,
        CancellationToken cancellationToken)
    {
        var code = maGiaoVien.Trim();

        return _dbContext.PhanCongGiangDays
            .AsNoTracking()
            .AnyAsync(
                x => x.MaLopHoc == maLopHoc
                     && x.MaGiaoVien == code
                     && x.TrangThai == AssignmentStatuses.DangPhanCong,
                cancellationToken);
    }

    public Task<bool> StudentInClassAsync(
        int maLopHoc,
        string maHocVien,
        CancellationToken cancellationToken)
    {
        var code = maHocVien.Trim();

        return _dbContext.DangKyHocs
            .AsNoTracking()
            .AnyAsync(
                x => x.MaLopHoc == maLopHoc
                     && x.MaHocVien == code
                     && (x.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                         || x.TrangThaiDangKy == EnrollmentStatuses.HoanThanh),
                cancellationToken);
    }

    public async Task<IReadOnlyList<ClassStudentInfo>> GetClassStudentsAsync(
        int maLopHoc,
        CancellationToken cancellationToken)
        => await _dbContext.DangKyHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == maLopHoc
                        && (x.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                            || x.TrangThaiDangKy == EnrollmentStatuses.HoanThanh))
            .OrderBy(x => x.MaHocVien)
            .Select(x => new ClassStudentInfo(x.MaHocVien, x.HocVien.HoTen, x.MaDangKy))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<int>> GetStudentClassIdsAsync(
        string maHocVien,
        CancellationToken cancellationToken)
    {
        var code = maHocVien.Trim();

        return await _dbContext.DangKyHocs
            .AsNoTracking()
            .Where(x => x.MaHocVien == code
                        && x.MaLopHoc != null
                        && (x.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                            || x.TrangThaiDangKy == EnrollmentStatuses.HoanThanh))
            .Select(x => x.MaLopHoc!.Value)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public Task<EnrollmentInfo?> GetEnrollmentInfoAsync(
        int maDangKy,
        CancellationToken cancellationToken)
        => _dbContext.DangKyHocs
            .AsNoTracking()
            .Where(x => x.MaDangKy == maDangKy)
            .Select(x => new EnrollmentInfo(
                x.MaDangKy,
                x.MaHocVien,
                x.MaKhoaHoc,
                x.MaLopHoc,
                x.TrangThaiDangKy))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<int>> GetTeacherClassIdsAsync(
        string maGiaoVien,
        CancellationToken cancellationToken)
    {
        var code = maGiaoVien.Trim();

        return await _dbContext.PhanCongGiangDays
            .AsNoTracking()
            .Where(x => x.MaGiaoVien == code && x.TrangThai == AssignmentStatuses.DangPhanCong)
            .Select(x => x.MaLopHoc)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public Task<bool> TaiKhoanExistsAsync(int maTaiKhoan, CancellationToken cancellationToken)
        => _dbContext.TaiKhoans
            .AsNoTracking()
            .AnyAsync(x => x.MaTaiKhoan == maTaiKhoan, cancellationToken);

    public Task<bool> ClassExistsAsync(int maLopHoc, CancellationToken cancellationToken)
        => _dbContext.LopHocs
            .AsNoTracking()
            .AnyAsync(x => x.MaLopHoc == maLopHoc, cancellationToken);

    public Task<bool> TeacherAssignedToEnrollmentClassAsync(
        int maDangKy,
        string maGiaoVien,
        CancellationToken cancellationToken)
    {
        var code = maGiaoVien.Trim();

        return _dbContext.DangKyHocs
            .AsNoTracking()
            .AnyAsync(
                d => d.MaDangKy == maDangKy
                     && d.MaLopHoc != null
                     && _dbContext.PhanCongGiangDays.Any(
                         p => p.MaLopHoc == d.MaLopHoc
                              && p.MaGiaoVien == code
                              && p.TrangThai == AssignmentStatuses.DangPhanCong),
                cancellationToken);
    }
}

