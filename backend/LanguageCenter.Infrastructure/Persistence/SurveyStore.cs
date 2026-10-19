using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Surveys.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="ISurveyStore"/>
public sealed class SurveyStore : ISurveyStore
{
    private const int MaxEssaySamples = 50;

    private readonly AppDbContext _dbContext;

    public SurveyStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ========================================================================
    // KHẢO SÁT
    // ========================================================================

    public async Task<PagedResult<SurveyListItemDto>> ListAsync(
        SurveyListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.KhaoSats.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        if (query.TuNgay is { } tuNgay)
        {
            source = source.Where(x => x.NgayBatDau >= tuNgay);
        }

        if (query.DenNgay is { } denNgay)
        {
            source = source.Where(x => x.NgayBatDau <= denNgay);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            source = source.Where(x => x.TenKhaoSat.Contains(search));
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderByDescending(x => x.NgayBatDau)
            .ThenByDescending(x => x.MaKhaoSat)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new SurveyListItemDto
            {
                MaKhaoSat = x.MaKhaoSat,
                TenKhaoSat = x.TenKhaoSat,
                MoTa = x.MoTa,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                TrangThai = x.TrangThai,
                SoCauHoi = x.CauHoiKhaoSats.Count(),
                SoPhanHoi = x.PhanHoiKhaoSats.Count()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<SurveyListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<SurveyDetailDto?> GetDetailAsync(int maKhaoSat, CancellationToken cancellationToken)
        => _dbContext.KhaoSats
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.MaKhaoSat == maKhaoSat)
            .Select(x => new SurveyDetailDto
            {
                MaKhaoSat = x.MaKhaoSat,
                TenKhaoSat = x.TenKhaoSat,
                MoTa = x.MoTa,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                TrangThai = x.TrangThai,
                SoCauHoi = x.CauHoiKhaoSats.Count(),
                SoPhanHoi = x.PhanHoiKhaoSats.Count(),
                CauHois = x.CauHoiKhaoSats
                    .OrderBy(q => q.ThuTu)
                    .Select(q => new SurveyQuestionDto
                    {
                        MaCauHoi = q.MaCauHoi,
                        MaKhaoSat = q.MaKhaoSat,
                        NoiDungCauHoi = q.NoiDungCauHoi,
                        LoaiCauHoi = q.LoaiCauHoi,
                        ThuTu = q.ThuTu,
                        BatBuoc = q.BatBuoc,
                        TrangThai = q.TrangThai,
                        SoCauTraLoi = q.CauTraLoiKhaoSats.Count()
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maKhaoSat, CancellationToken cancellationToken)
        => _dbContext.KhaoSats
            .AsNoTracking()
            .AnyAsync(x => x.MaKhaoSat == maKhaoSat, cancellationToken);

    public Task<int> CountResponsesAsync(int maKhaoSat, CancellationToken cancellationToken)
        => _dbContext.PhanHoiKhaoSats
            .AsNoTracking()
            .CountAsync(x => x.MaKhaoSat == maKhaoSat, cancellationToken);

    public async Task<WriteResult> CreateAsync(
        CreateSurveyData data,
        CancellationToken cancellationToken)
    {
        var entity = new KhaoSat
        {
            TenKhaoSat = data.TenKhaoSat,
            MoTa = data.MoTa,
            NgayBatDau = data.NgayBatDau,
            NgayKetThuc = data.NgayKetThuc,
            TrangThai = data.TrangThai
        };

        _dbContext.KhaoSats.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaKhaoSat);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maKhaoSat,
        SurveyUpdateData changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.KhaoSats
            .FirstOrDefaultAsync(x => x.MaKhaoSat == maKhaoSat, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.CapNhatTen && changes.TenKhaoSat is not null)
        {
            entity.TenKhaoSat = changes.TenKhaoSat;
        }

        if (changes.CapNhatMoTa)
        {
            entity.MoTa = changes.MoTa;
        }

        if (changes.CapNhatNgayBatDau && changes.NgayBatDau is not null)
        {
            entity.NgayBatDau = changes.NgayBatDau.Value;
        }

        if (changes.CapNhatNgayKetThuc)
        {
            entity.NgayKetThuc = changes.NgayKetThuc;
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            // CK_KhaoSat_Ngay
            return WriteOutcome.Conflict;
        }

        return WriteOutcome.Success;
    }

    public async Task<WriteOutcome> UpdateStatusAsync(
        int maKhaoSat,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.KhaoSats
            .FirstOrDefaultAsync(x => x.MaKhaoSat == maKhaoSat, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.TrangThai = trangThai;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return WriteOutcome.Success;
    }

    public async Task<WriteOutcome> DeleteAsync(int maKhaoSat, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.KhaoSats
            .FirstOrDefaultAsync(x => x.MaKhaoSat == maKhaoSat, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        _dbContext.KhaoSats.Remove(entity);

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
    // CÂU HỎI
    // ========================================================================

    public async Task<IReadOnlyList<SurveyQuestionDto>> ListQuestionsAsync(
        int maKhaoSat,
        CancellationToken cancellationToken)
        => await _dbContext.CauHoiKhaoSats
            .AsNoTracking()
            .Where(x => x.MaKhaoSat == maKhaoSat)
            .OrderBy(x => x.ThuTu)
            .Select(x => new SurveyQuestionDto
            {
                MaCauHoi = x.MaCauHoi,
                MaKhaoSat = x.MaKhaoSat,
                NoiDungCauHoi = x.NoiDungCauHoi,
                LoaiCauHoi = x.LoaiCauHoi,
                ThuTu = x.ThuTu,
                BatBuoc = x.BatBuoc,
                TrangThai = x.TrangThai,
                SoCauTraLoi = x.CauTraLoiKhaoSats.Count()
            })
            .ToListAsync(cancellationToken);

    public Task<SurveyQuestionDto?> GetQuestionAsync(
        int maKhaoSat,
        int maCauHoi,
        CancellationToken cancellationToken)
        => _dbContext.CauHoiKhaoSats
            .AsNoTracking()
            .Where(x => x.MaKhaoSat == maKhaoSat && x.MaCauHoi == maCauHoi)
            .Select(x => new SurveyQuestionDto
            {
                MaCauHoi = x.MaCauHoi,
                MaKhaoSat = x.MaKhaoSat,
                NoiDungCauHoi = x.NoiDungCauHoi,
                LoaiCauHoi = x.LoaiCauHoi,
                ThuTu = x.ThuTu,
                BatBuoc = x.BatBuoc,
                TrangThai = x.TrangThai,
                SoCauTraLoi = x.CauTraLoiKhaoSats.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> QuestionOrderTakenAsync(
        int maKhaoSat,
        int thuTu,
        int? exceptId,
        CancellationToken cancellationToken)
        => _dbContext.CauHoiKhaoSats
            .AsNoTracking()
            .AnyAsync(
                x => x.MaKhaoSat == maKhaoSat
                     && x.ThuTu == thuTu
                     && (exceptId == null || x.MaCauHoi != exceptId),
                cancellationToken);

    public Task<int> CountAnswersAsync(int maCauHoi, CancellationToken cancellationToken)
        => _dbContext.CauTraLoiKhaoSats
            .AsNoTracking()
            .CountAsync(x => x.MaCauHoi == maCauHoi, cancellationToken);

    public async Task<WriteResult> CreateQuestionAsync(
        int maKhaoSat,
        CreateQuestionData data,
        CancellationToken cancellationToken)
    {
        var entity = new CauHoiKhaoSat
        {
            MaKhaoSat = maKhaoSat,
            NoiDungCauHoi = data.NoiDungCauHoi,
            LoaiCauHoi = data.LoaiCauHoi,
            ThuTu = data.ThuTu,
            BatBuoc = data.BatBuoc,
            TrangThai = data.TrangThai
        };

        _dbContext.CauHoiKhaoSats.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsUniqueViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaCauHoi);
    }

    public async Task<WriteOutcome> UpdateQuestionAsync(
        int maCauHoi,
        QuestionUpdateData changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.CauHoiKhaoSats
            .FirstOrDefaultAsync(x => x.MaCauHoi == maCauHoi, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.CapNhatNoiDung && changes.NoiDungCauHoi is not null)
        {
            entity.NoiDungCauHoi = changes.NoiDungCauHoi;
        }

        if (changes.CapNhatLoai && changes.LoaiCauHoi is not null)
        {
            entity.LoaiCauHoi = changes.LoaiCauHoi;
        }

        if (changes.CapNhatThuTu && changes.ThuTu is not null)
        {
            entity.ThuTu = changes.ThuTu.Value;
        }

        if (changes.CapNhatBatBuoc && changes.BatBuoc is not null)
        {
            entity.BatBuoc = changes.BatBuoc.Value;
        }

        if (changes.TrangThai is not null)
        {
            entity.TrangThai = changes.TrangThai;
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

    public async Task<WriteOutcome> DeleteQuestionAsync(int maCauHoi, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.CauHoiKhaoSats
            .FirstOrDefaultAsync(x => x.MaCauHoi == maCauHoi, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        _dbContext.CauHoiKhaoSats.Remove(entity);

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
    // HỌC VIÊN
    // ========================================================================

    public async Task<IReadOnlyList<MySurveyDto>> ListForStudentAsync(
        string maHocVien,
        CancellationToken cancellationToken)
    {
        var code = maHocVien.Trim();

        return await _dbContext.KhaoSats
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.TrangThai == SurveyStatuses.HoatDong
                        || x.PhanHoiKhaoSats.Any(p => p.MaHocVien == code))
            .OrderByDescending(x => x.NgayBatDau)
            .ThenByDescending(x => x.MaKhaoSat)
            .Select(x => new MySurveyDto
            {
                MaKhaoSat = x.MaKhaoSat,
                TenKhaoSat = x.TenKhaoSat,
                MoTa = x.MoTa,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                TrangThai = x.TrangThai,
                SoCauHoi = x.CauHoiKhaoSats.Count(),
                MaPhanHoiKhaoSat = x.PhanHoiKhaoSats
                    .Where(p => p.MaHocVien == code)
                    .Select(p => (int?)p.MaPhanHoiKhaoSat)
                    .FirstOrDefault(),
                ThoiGianTraLoi = x.PhanHoiKhaoSats
                    .Where(p => p.MaHocVien == code)
                    .Select(p => (DateTime?)p.ThoiGianGui)
                    .FirstOrDefault(),
                CauHois = x.CauHoiKhaoSats
                    .Where(q => q.TrangThai == ActivityStatuses.HoatDong)
                    .OrderBy(q => q.ThuTu)
                    .Select(q => new SurveyQuestionDto
                    {
                        MaCauHoi = q.MaCauHoi,
                        MaKhaoSat = q.MaKhaoSat,
                        NoiDungCauHoi = q.NoiDungCauHoi,
                        LoaiCauHoi = q.LoaiCauHoi,
                        ThuTu = q.ThuTu,
                        BatBuoc = q.BatBuoc,
                        TrangThai = q.TrangThai,
                        SoCauTraLoi = q.CauTraLoiKhaoSats.Count()
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }

    public Task<MySurveyDto?> GetForStudentAsync(
        int maKhaoSat,
        string maHocVien,
        CancellationToken cancellationToken)
    {
        var code = maHocVien.Trim();

        return _dbContext.KhaoSats
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.MaKhaoSat == maKhaoSat
                        && (x.TrangThai == SurveyStatuses.HoatDong
                            || x.PhanHoiKhaoSats.Any(p => p.MaHocVien == code)))
            .Select(x => new MySurveyDto
            {
                MaKhaoSat = x.MaKhaoSat,
                TenKhaoSat = x.TenKhaoSat,
                MoTa = x.MoTa,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                TrangThai = x.TrangThai,
                SoCauHoi = x.CauHoiKhaoSats.Count(),
                MaPhanHoiKhaoSat = x.PhanHoiKhaoSats
                    .Where(p => p.MaHocVien == code)
                    .Select(p => (int?)p.MaPhanHoiKhaoSat)
                    .FirstOrDefault(),
                ThoiGianTraLoi = x.PhanHoiKhaoSats
                    .Where(p => p.MaHocVien == code)
                    .Select(p => (DateTime?)p.ThoiGianGui)
                    .FirstOrDefault(),
                CauHois = x.CauHoiKhaoSats
                    .Where(q => q.TrangThai == ActivityStatuses.HoatDong)
                    .OrderBy(q => q.ThuTu)
                    .Select(q => new SurveyQuestionDto
                    {
                        MaCauHoi = q.MaCauHoi,
                        MaKhaoSat = q.MaKhaoSat,
                        NoiDungCauHoi = q.NoiDungCauHoi,
                        LoaiCauHoi = q.LoaiCauHoi,
                        ThuTu = q.ThuTu,
                        BatBuoc = q.BatBuoc,
                        TrangThai = q.TrangThai,
                        SoCauTraLoi = q.CauTraLoiKhaoSats.Count()
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<SurveyResponseDetailDto?> GetResponseOfStudentAsync(
        int maKhaoSat,
        string maHocVien,
        CancellationToken cancellationToken)
    {
        var code = maHocVien.Trim();

        return _dbContext.PhanHoiKhaoSats
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.MaKhaoSat == maKhaoSat && x.MaHocVien == code)
            .Select(x => new SurveyResponseDetailDto
            {
                MaPhanHoiKhaoSat = x.MaPhanHoiKhaoSat,
                MaKhaoSat = x.MaKhaoSat,
                TenKhaoSat = x.KhaoSat.TenKhaoSat,
                MaHocVien = x.MaHocVien,
                HoTenHocVien = x.HocVien.HoTen,
                ThoiGianGui = x.ThoiGianGui,
                DiemDanhGiaChung = x.DiemDanhGiaChung,
                GhiChu = x.GhiChu,
                CauTraLois = x.CauTraLoiKhaoSats
                    .OrderBy(a => a.CauHoiKhaoSat.ThuTu)
                    .Select(a => new SurveyAnswerDto
                    {
                        MaCauTraLoi = a.MaCauTraLoi,
                        MaCauHoi = a.MaCauHoi,
                        NoiDungCauHoi = a.CauHoiKhaoSat.NoiDungCauHoi,
                        LoaiCauHoi = a.CauHoiKhaoSat.LoaiCauHoi,
                        GiaTriDanhGia = a.GiaTriDanhGia,
                        NoiDungTraLoi = a.NoiDungTraLoi
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SurveySubmitResult> SubmitAsync(
        int maKhaoSat,
        string maHocVien,
        SubmitResponseData data,
        CancellationToken cancellationToken)
    {
        var code = maHocVien.Trim();

        var survey = await _dbContext.KhaoSats
            .AsNoTracking()
            .Where(x => x.MaKhaoSat == maKhaoSat)
            .Select(x => new { x.MaKhaoSat, x.TrangThai, x.NgayBatDau, x.NgayKetThuc })
            .FirstOrDefaultAsync(cancellationToken);

        if (survey is null)
        {
            return new SurveySubmitResult(SurveySubmitOutcome.SurveyNotFound, 0, null);
        }

        var lyDo = SurveyStatuses.DescribeBlockReason(
            survey.TrangThai,
            survey.NgayBatDau,
            survey.NgayKetThuc,
            DateOnly.FromDateTime(DateTime.Today));

        if (lyDo is not null)
        {
            return new SurveySubmitResult(SurveySubmitOutcome.NotAccepting, 0, lyDo);
        }

        var daCo = await _dbContext.PhanHoiKhaoSats
            .AsNoTracking()
            .Where(x => x.MaKhaoSat == maKhaoSat && x.MaHocVien == code)
            .Select(x => (int?)x.MaPhanHoiKhaoSat)
            .FirstOrDefaultAsync(cancellationToken);

        if (daCo is not null)
        {
            return new SurveySubmitResult(SurveySubmitOutcome.AlreadyResponded, daCo.Value, null);
        }

        var entity = new PhanHoiKhaoSat
        {
            MaKhaoSat = maKhaoSat,
            MaHocVien = code,
            ThoiGianGui = DateTime.Now,
            DiemDanhGiaChung = data.DiemDanhGiaChung,
            GhiChu = data.GhiChu,
            CauTraLoiKhaoSats = data.CauTraLois
                .Select(a => new CauTraLoiKhaoSat
                {
                    MaCauHoi = a.MaCauHoi,
                    GiaTriDanhGia = a.GiaTriDanhGia,
                    NoiDungTraLoi = a.NoiDungTraLoi
                })
                .ToList()
        };

        _dbContext.PhanHoiKhaoSats.Add(entity);

        try
        {
            // Một SaveChangesAsync = một transaction: phản hồi + toàn bộ câu trả lời.
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsUniqueViolation(exception))
        {
            // UQ_PhanHoiKhaoSat (MaKhaoSat, MaHocVien) hoặc UQ_CauTraLoi.
            _dbContext.ChangeTracker.Clear();

            var existing = await _dbContext.PhanHoiKhaoSats
                .AsNoTracking()
                .Where(x => x.MaKhaoSat == maKhaoSat && x.MaHocVien == code)
                .Select(x => (int?)x.MaPhanHoiKhaoSat)
                .FirstOrDefaultAsync(cancellationToken);

            return existing is not null
                ? new SurveySubmitResult(SurveySubmitOutcome.AlreadyResponded, existing.Value, null)
                : new SurveySubmitResult(SurveySubmitOutcome.Conflict, 0, null);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.ChangeTracker.Clear();

            return new SurveySubmitResult(
                SurveySubmitOutcome.Conflict,
                0,
                "Vi phạm ràng buộc dữ liệu (giá trị đánh giá 0..5, câu hỏi phải thuộc khảo sát).");
        }

        return new SurveySubmitResult(SurveySubmitOutcome.Success, entity.MaPhanHoiKhaoSat, null);
    }

    // ========================================================================
    // QUẢN TRỊ — PHẢN HỒI & THỐNG KÊ
    // ========================================================================

    public async Task<PagedResult<SurveyResponseListItemDto>> ListResponsesAsync(
        int maKhaoSat,
        SurveyResponseListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.PhanHoiKhaoSats
            .AsNoTracking()
            .Where(x => x.MaKhaoSat == maKhaoSat);

        if (!string.IsNullOrWhiteSpace(query.MaHocVien))
        {
            var code = query.MaHocVien.Trim();
            source = source.Where(x => x.MaHocVien == code);
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderByDescending(x => x.ThoiGianGui)
            .ThenByDescending(x => x.MaPhanHoiKhaoSat)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new SurveyResponseListItemDto
            {
                MaPhanHoiKhaoSat = x.MaPhanHoiKhaoSat,
                MaKhaoSat = x.MaKhaoSat,
                MaHocVien = x.MaHocVien,
                HoTenHocVien = x.HocVien.HoTen,
                ThoiGianGui = x.ThoiGianGui,
                DiemDanhGiaChung = x.DiemDanhGiaChung,
                GhiChu = x.GhiChu,
                SoCauTraLoi = x.CauTraLoiKhaoSats.Count()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<SurveyResponseListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<SurveyStatisticsDto?> GetStatisticsAsync(
        int maKhaoSat,
        CancellationToken cancellationToken)
    {
        var survey = await _dbContext.KhaoSats
            .AsNoTracking()
            .Where(x => x.MaKhaoSat == maKhaoSat)
            .Select(x => new
            {
                x.MaKhaoSat,
                x.TenKhaoSat,
                x.TrangThai,
                x.NgayBatDau,
                x.NgayKetThuc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (survey is null)
        {
            return null;
        }

        var questions = await ListQuestionsAsync(maKhaoSat, cancellationToken);

        // MỘT truy vấn cho toàn bộ câu trả lời -> tránh N+1.
        var answers = await _dbContext.CauTraLoiKhaoSats
            .AsNoTracking()
            .Where(x => x.PhanHoiKhaoSat.MaKhaoSat == maKhaoSat)
            .Select(x => new { x.MaCauHoi, x.GiaTriDanhGia, x.NoiDungTraLoi })
            .ToListAsync(cancellationToken);

        var generalScores = await _dbContext.PhanHoiKhaoSats
            .AsNoTracking()
            .Where(x => x.MaKhaoSat == maKhaoSat && x.DiemDanhGiaChung != null)
            .Select(x => x.DiemDanhGiaChung!.Value)
            .ToListAsync(cancellationToken);

        var answersByQuestion = answers
            .GroupBy(x => x.MaCauHoi)
            .ToDictionary(g => g.Key, g => g.ToList());

        var chiTiet = new List<QuestionStatisticDto>(questions.Count);

        foreach (var question in questions)
        {
            var rows = answersByQuestion.TryGetValue(question.MaCauHoi, out var list) ? list : [];

            var giaTri = rows
                .Where(x => x.GiaTriDanhGia is not null)
                .Select(x => x.GiaTriDanhGia!.Value)
                .ToList();

            var phanBo = giaTri
                .GroupBy(x => x)
                .OrderBy(g => g.Key)
                .Select(g => new AnswerDistributionDto { GiaTri = g.Key, SoLuong = g.Count() })
                .ToList();

            var tuLuan = rows
                .Where(x => !string.IsNullOrWhiteSpace(x.NoiDungTraLoi))
                .Select(x => x.NoiDungTraLoi!.Trim())
                .Take(MaxEssaySamples)
                .ToList();

            chiTiet.Add(new QuestionStatisticDto
            {
                MaCauHoi = question.MaCauHoi,
                NoiDungCauHoi = question.NoiDungCauHoi,
                LoaiCauHoi = question.LoaiCauHoi,
                ThuTu = question.ThuTu,
                BatBuoc = question.BatBuoc,
                SoLuotTraLoi = rows.Count,
                GiaTriTrungBinh = giaTri.Count == 0
                    ? null
                    : Math.Round(giaTri.Average(), 2, MidpointRounding.AwayFromZero),
                PhanBo = phanBo,
                NoiDungTuLuan = tuLuan
            });
        }

        var tongPhanHoi = await _dbContext.PhanHoiKhaoSats
            .AsNoTracking()
            .CountAsync(x => x.MaKhaoSat == maKhaoSat, cancellationToken);

        return new SurveyStatisticsDto
        {
            MaKhaoSat = survey.MaKhaoSat,
            TenKhaoSat = survey.TenKhaoSat,
            TrangThai = survey.TrangThai,
            NgayBatDau = survey.NgayBatDau,
            NgayKetThuc = survey.NgayKetThuc,
            TongSoCauHoi = questions.Count,
            TongSoPhanHoi = tongPhanHoi,
            DiemDanhGiaChungTrungBinh = generalScores.Count == 0
                ? null
                : Math.Round(generalScores.Average(), 2, MidpointRounding.AwayFromZero),
            CauHois = chiTiet
        };
    }
}
