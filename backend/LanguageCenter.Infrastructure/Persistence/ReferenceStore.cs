using LanguageCenter.Application.Abstractions;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IReferenceStore"/>
public sealed class ReferenceStore : IReferenceStore
{
    private readonly AppDbContext _dbContext;

    public ReferenceStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> StaffExistsAsync(string maNhanVien, CancellationToken cancellationToken)
        => _dbContext.NhanViens
            .AsNoTracking()
            .AnyAsync(x => x.MaNhanVien == maNhanVien, cancellationToken);

    public Task<bool> TeacherExistsAsync(string maGiaoVien, CancellationToken cancellationToken)
        => _dbContext.GiaoViens
            .AsNoTracking()
            .AnyAsync(x => x.MaGiaoVien == maGiaoVien, cancellationToken);

    public Task<string?> GetTeacherTypeAsync(string maGiaoVien, CancellationToken cancellationToken)
        => _dbContext.GiaoViens
            .AsNoTracking()
            .Where(x => x.MaGiaoVien == maGiaoVien)
            .Select(x => (string?)x.LoaiGiaoVien)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> StudentExistsAsync(string maHocVien, CancellationToken cancellationToken)
        => _dbContext.HocViens
            .AsNoTracking()
            .AnyAsync(x => x.MaHocVien == maHocVien, cancellationToken);

    public Task<string?> GetStudentStatusAsync(string maHocVien, CancellationToken cancellationToken)
        => _dbContext.HocViens
            .AsNoTracking()
            .Where(x => x.MaHocVien == maHocVien)
            .Select(x => (string?)x.TrangThaiHocTap)
            .FirstOrDefaultAsync(cancellationToken);
}
