using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.GradeTypes;
using LanguageCenter.Application.GradeTypes.Models;
using LanguageCenter.Application.Grades.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Grades;

/// <inheritdoc cref="IGradeService"/>
/// <remarks>
/// <para><b>Công thức điểm tổng kết (chỉ tính, không lưu):</b></para>
/// <code>
/// n_i       = Diem_i / DiemToiDa_i               (0..1)
/// TongKet   = 10 × Σ(n_i × TrongSo_i) / Σ(TrongSo_i)
/// </code>
/// <para>
/// Chỉ tính khi ĐÃ CÓ ĐỦ điểm của mọi đầu điểm đang hoạt động; nếu thiếu thì trả
/// <c>diemTongKet = null</c> kèm danh sách đầu điểm còn thiếu. Không xếp loại đạt/không đạt tự động.
/// </para>
/// </remarks>
public sealed class GradeService : IGradeService
{
    private const decimal RoundingFactor = 100m;

    private readonly IGradeStore _gradeStore;
    private readonly IGradeTypeStore _gradeTypeStore;
    private readonly IAcademicLookupStore _lookupStore;
    private readonly IReferenceStore _referenceStore;

    public GradeService(
        IGradeStore gradeStore,
        IGradeTypeStore gradeTypeStore,
        IAcademicLookupStore lookupStore,
        IReferenceStore referenceStore)
    {
        _gradeStore = gradeStore;
        _gradeTypeStore = gradeTypeStore;
        _lookupStore = lookupStore;
        _referenceStore = referenceStore;
    }

    public async Task<OperationResult<PagedResult<GradeListItemDto>>> ListAsync(
        GradeListQuery query,
        CancellationToken cancellationToken)
    {
        var page = await _gradeStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<GradeListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<GradeDetailDto>> GetDetailAsync(
        int maDiemSo,
        CancellationToken cancellationToken)
    {
        var detail = await _gradeStore.GetDetailAsync(maDiemSo, cancellationToken);

        return detail is null
            ? OperationResult<GradeDetailDto>.NotFound($"Không tìm thấy điểm có id = {maDiemSo}.")
            : OperationResult<GradeDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<GradeDetailDto>> CreateAsync(
        CreateGradeRequest request,
        GradeActor actor,
        CancellationToken cancellationToken)
    {
        var enrollment = await _lookupStore.GetEnrollmentInfoAsync(request.MaDangKy, cancellationToken);

        if (enrollment is null)
        {
            return OperationResult<GradeDetailDto>.BadRequest(
                "Đăng ký học không tồn tại.",
                [$"Không tìm thấy đăng ký có id = {request.MaDangKy}."]);
        }

        var gradeType = await _gradeTypeStore.GetDetailAsync(request.MaLoaiDiem, cancellationToken);

        if (gradeType is null)
        {
            return OperationResult<GradeDetailDto>.BadRequest(
                "Đầu điểm không tồn tại.",
                [$"Không tìm thấy đầu điểm có id = {request.MaLoaiDiem}."]);
        }

        if (gradeType.MaKhoaHoc != enrollment.MaKhoaHoc)
        {
            return OperationResult<GradeDetailDto>.BadRequest(
                "Đầu điểm không thuộc khóa học của đăng ký này.",
                [
                    $"Đầu điểm '{gradeType.TenLoaiDiem}' thuộc khóa học {gradeType.MaKhoaHoc}, " +
                    $"còn đăng ký {request.MaDangKy} thuộc khóa học {enrollment.MaKhoaHoc}."
                ]);
        }

        if (!GradeTypeStatuses.IsActive(gradeType.TrangThai))
        {
            return OperationResult<GradeDetailDto>.BadRequest(
                "Đầu điểm đã ngừng sử dụng.",
                [$"Đầu điểm '{gradeType.TenLoaiDiem}' đang ở trạng thái '{gradeType.TrangThai}'."]);
        }

        var errors = ValidateScore(request.Diem, gradeType.DiemToiDa, request.TrangThaiKetQua);

        if (errors.Count > 0)
        {
            return OperationResult<GradeDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (await _gradeStore.DuplicateAsync(
                request.MaDangKy, request.MaLoaiDiem, exceptId: null, cancellationToken))
        {
            return OperationResult<GradeDetailDto>.Conflict(
                $"Đăng ký {request.MaDangKy} đã có điểm cho đầu điểm '{gradeType.TenLoaiDiem}'. " +
                "Hãy dùng PATCH để sửa.");
        }

        // -------- XÁC ĐỊNH GIÁO VIÊN NHẬP ĐIỂM --------
        string maGiaoVien;

        if (actor.IsAdmin)
        {
            var code = TextValue.Required(request.MaGiaoVien)?.Trim();

            if (string.IsNullOrEmpty(code))
            {
                return OperationResult<GradeDetailDto>.BadRequest(
                    "Thiếu giáo viên nhập điểm.",
                    ["Bảng DiemSo bắt buộc có MaGiaoVien. Quản trị viên phải gửi 'maGiaoVien'."]);
            }

            if (!await _referenceStore.TeacherExistsAsync(code, cancellationToken))
            {
                return OperationResult<GradeDetailDto>.BadRequest(
                    "Giáo viên không tồn tại.",
                    [$"Không tìm thấy giáo viên '{code}'."]);
            }

            maGiaoVien = code;
        }
        else
        {
            maGiaoVien = actor.TenDangNhap.Trim();

            if (enrollment.MaLopHoc is null
                || !await _lookupStore.TeacherAssignedToEnrollmentClassAsync(
                    request.MaDangKy, maGiaoVien, cancellationToken))
            {
                return OperationResult<GradeDetailDto>.Forbidden(
                    "Bạn không được phân công giảng dạy lớp của đăng ký này.");
            }
        }

        var result = await _gradeStore.CreateAsync(request, maGiaoVien, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<GradeDetailDto>.Conflict("Không thể tạo điểm.");
        }

        var created = await _gradeStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<GradeDetailDto>.Created(created!, "Nhập điểm thành công.");
    }

    public async Task<OperationResult<GradeDetailDto>> UpdateAsync(
        int maDiemSo,
        UpdateGradeRequest request,
        GradeActor actor,
        CancellationToken cancellationToken)
    {
        var current = await _gradeStore.GetDetailAsync(maDiemSo, cancellationToken);

        if (current is null)
        {
            return OperationResult<GradeDetailDto>.NotFound($"Không tìm thấy điểm có id = {maDiemSo}.");
        }

        if (request.Diem is null && request.NhanXet is null && request.TrangThaiKetQua is null)
        {
            return OperationResult<GradeDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        if (!actor.IsAdmin)
        {
            var code = actor.TenDangNhap.Trim();

            if (current.MaLopHoc is null
                || !await _lookupStore.TeacherAssignedToEnrollmentClassAsync(
                    current.MaDangKy, code, cancellationToken))
            {
                return OperationResult<GradeDetailDto>.Forbidden(
                    "Bạn không được phân công giảng dạy lớp của bản ghi điểm này.");
            }
        }

        var errors = ValidateScore(
            request.Diem ?? current.Diem,
            current.DiemToiDa,
            request.TrangThaiKetQua);

        if (errors.Count > 0)
        {
            return OperationResult<GradeDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        var outcome = await _gradeStore.UpdateAsync(maDiemSo, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<GradeDetailDto>.NotFound($"Không tìm thấy điểm có id = {maDiemSo}.")
                : OperationResult<GradeDetailDto>.Conflict("Không thể cập nhật điểm.");
        }

        var updated = await _gradeStore.GetDetailAsync(maDiemSo, cancellationToken);

        return OperationResult<GradeDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<bool>> DeleteAsync(
        int maDiemSo,
        GradeActor actor,
        CancellationToken cancellationToken)
    {
        var current = await _gradeStore.GetDetailAsync(maDiemSo, cancellationToken);

        if (current is null)
        {
            return OperationResult<bool>.NotFound($"Không tìm thấy điểm có id = {maDiemSo}.");
        }

        if (!actor.IsAdmin)
        {
            var code = actor.TenDangNhap.Trim();

            if (current.MaLopHoc is null
                || !await _lookupStore.TeacherAssignedToEnrollmentClassAsync(
                    current.MaDangKy, code, cancellationToken))
            {
                return OperationResult<bool>.Forbidden(
                    "Bạn không được phân công giảng dạy lớp của bản ghi điểm này.");
            }
        }

        var outcome = await _gradeStore.DeleteAsync(maDiemSo, cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.NoContent(),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound(
                $"Không tìm thấy điểm có id = {maDiemSo}."),
            _ => OperationResult<bool>.Conflict("Không thể xóa điểm do ràng buộc dữ liệu.")
        };
    }

    public async Task<OperationResult<GradeSummaryDto>> GetClassSummaryAsync(
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        var data = await _gradeStore.GetClassGradeDataAsync(maLopHoc, cancellationToken);

        if (data is null)
        {
            return OperationResult<GradeSummaryDto>.NotFound(
                $"Không tìm thấy lớp học có id = {maLopHoc}.");
        }

        var tongTrongSo = data.Components.Sum(x => x.TrongSo);

        var hocViens = data.Students
            .Select(student => BuildSummary(
                student.MaDangKy,
                student.MaHocVien,
                student.HoTen,
                data.Components,
                student.Scores))
            .ToList();

        var dto = new GradeSummaryDto
        {
            MaLopHoc = data.MaLopHoc,
            MaLop = data.MaLop,
            TenLop = data.TenLop,
            MaKhoaHoc = data.MaKhoaHoc,
            TenKhoaHoc = data.TenKhoaHoc,
            TongTrongSo = tongTrongSo,
            CanhBaoTrongSo = tongTrongSo != 100m,
            CanhBao = tongTrongSo == 100m
                ? null
                : $"Tổng trọng số các đầu điểm đang hoạt động là {tongTrongSo} (nên bằng 100). " +
                  "Điểm tổng kết vẫn được tính theo tỉ lệ trọng số thực tế.",
            HocViens = hocViens
        };

        return OperationResult<GradeSummaryDto>.Ok(dto);
    }

    public async Task<OperationResult<IReadOnlyList<StudentCourseGradeDto>>> GetForStudentAsync(
        string maHocVien,
        CancellationToken cancellationToken)
    {
        var data = await _gradeStore.GetStudentGradeDataAsync(maHocVien, cancellationToken);

        if (data is null)
        {
            return OperationResult<IReadOnlyList<StudentCourseGradeDto>>.NotFound(
                $"Không tìm thấy học viên '{maHocVien}'.");
        }

        var result = new List<StudentCourseGradeDto>(data.Groups.Count);

        foreach (var group in data.Groups)
        {
            var summary = BuildSummary(
                group.MaDangKy,
                data.MaHocVien,
                data.HoTen,
                group.Components,
                group.Scores);

            var tongTrongSo = group.Components.Sum(x => x.TrongSo);

            result.Add(new StudentCourseGradeDto
            {
                MaDangKy = group.MaDangKy,
                MaLopHoc = group.MaLopHoc,
                MaLop = group.MaLop,
                TenLop = group.TenLop,
                MaKhoaHoc = group.MaKhoaHoc,
                TenKhoaHoc = group.TenKhoaHoc,
                NgayBatDau = group.NgayBatDau,
                NgayKetThucDuKien = group.NgayKetThucDuKien,
                NgayXepLop = group.NgayXepLop,
                TongTrongSo = tongTrongSo,
                CanhBaoTrongSo = tongTrongSo != 100m,
                DuDieuKien = summary.DuDieuKien,
                DiemTongKet = summary.DiemTongKet,
                ThanhPhan = summary.ThanhPhan,
                ThanhPhanConThieu = summary.ThanhPhanConThieu
            });
        }

        return OperationResult<IReadOnlyList<StudentCourseGradeDto>>.Ok(result);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static List<string> ValidateScore(
        decimal diem,
        decimal diemToiDa,
        string? trangThaiKetQua)
    {
        var errors = new List<string>();

        if (diem < 0m)
        {
            errors.Add("Điểm phải lớn hơn hoặc bằng 0.");
        }

        // DB KHÔNG có CHECK Diem <= DiemToiDa -> phải chặn ở tầng nghiệp vụ.
        if (diemToiDa > 0m && diem > diemToiDa)
        {
            errors.Add($"Điểm không được vượt quá điểm tối đa ({diemToiDa}).");
        }

        if (!string.IsNullOrWhiteSpace(trangThaiKetQua)
            && !GradeResults.IsValid(trangThaiKetQua.Trim()))
        {
            errors.Add($"Kết quả chỉ chấp nhận: {string.Join(", ", GradeResults.All)} (hoặc để trống).");
        }

        return errors;
    }

    /// <summary>Áp dụng công thức Q6 để dựng bảng tổng kết của MỘT học viên.</summary>
    private static StudentGradeSummaryDto BuildSummary(
        int maDangKy,
        string maHocVien,
        string hoTen,
        IReadOnlyList<GradeComponentInfo> components,
        IReadOnlyDictionary<int, decimal> scores)
    {
        var thanhPhan = new List<GradeComponentScoreDto>(components.Count);
        var conThieu = new List<string>();

        foreach (var component in components)
        {
            var coDiem = scores.TryGetValue(component.MaLoaiDiem, out var diem);

            thanhPhan.Add(new GradeComponentScoreDto
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

        decimal? tongKet = null;

        if (duDieuKien)
        {
            var tongTrongSo = components.Sum(x => x.TrongSo);

            if (tongTrongSo > 0m)
            {
                var tuSo = 0m;

                foreach (var component in components)
                {
                    var diem = scores[component.MaLoaiDiem];
                    var tyLe = component.DiemToiDa > 0m ? diem / component.DiemToiDa : 0m;

                    tuSo += tyLe * component.TrongSo;
                }

                tongKet = Math.Round(
                    10m * tuSo / tongTrongSo,
                    2,
                    MidpointRounding.AwayFromZero);
            }
        }

        return new StudentGradeSummaryDto
        {
            MaDangKy = maDangKy,
            MaHocVien = maHocVien,
            HoTen = hoTen,
            DuDieuKien = duDieuKien,
            DiemTongKet = tongKet,
            ThanhPhan = thanhPhan,
            ThanhPhanConThieu = conThieu
        };
    }
}
