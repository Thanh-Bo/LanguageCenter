using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Sessions.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Sessions;

/// <inheritdoc cref="ISessionService"/>
public sealed class SessionService : ISessionService
{
    private readonly ISessionStore _sessionStore;
    private readonly IAcademicLookupStore _lookupStore;
    private readonly IReferenceStore _referenceStore;

    public SessionService(
        ISessionStore sessionStore,
        IAcademicLookupStore lookupStore,
        IReferenceStore referenceStore)
    {
        _sessionStore = sessionStore;
        _lookupStore = lookupStore;
        _referenceStore = referenceStore;
    }

    public async Task<OperationResult<PagedResult<SessionListItemDto>>> ListAsync(
        SessionListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai)
            && !SessionStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<SessionListItemDto>>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", SessionStatuses.All)}."]);
        }

        if (query.TuNgay is { } tu && query.DenNgay is { } den && den < tu)
        {
            return OperationResult<PagedResult<SessionListItemDto>>.BadRequest(
                "Khoảng ngày không hợp lệ.",
                ["Đến ngày phải lớn hơn hoặc bằng từ ngày."]);
        }

        var page = await _sessionStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<SessionListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<SessionDetailDto>> GetDetailAsync(
        int maCaHoc,
        CancellationToken cancellationToken)
    {
        var detail = await _sessionStore.GetDetailAsync(maCaHoc, cancellationToken);

        return detail is null
            ? OperationResult<SessionDetailDto>.NotFound($"Không tìm thấy ca học có id = {maCaHoc}.")
            : OperationResult<SessionDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<SessionDetailDto>> CreateAsync(
        CreateSessionRequest request,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        ValidateTimes(request.GioBatDau, request.GioKetThuc, request.NgayHoc, errors);

        if (errors.Count > 0)
        {
            return OperationResult<SessionDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (await _lookupStore.GetClassInfoAsync(request.MaLopHoc, cancellationToken) is null)
        {
            return OperationResult<SessionDetailDto>.BadRequest(
                "Lớp học không tồn tại.",
                [$"Không tìm thấy lớp có id = {request.MaLopHoc}."]);
        }

        if (!await _lookupStore.RoomExistsAsync(request.MaPhongHoc, cancellationToken))
        {
            return OperationResult<SessionDetailDto>.BadRequest(
                "Phòng học không tồn tại.",
                [$"Không tìm thấy phòng có id = {request.MaPhongHoc}."]);
        }

        var roleErrors = await ValidateTeachersAsync(
            request.MaGiaoVienThucTe,
            request.MaTroGiangThucTe,
            cancellationToken);

        if (roleErrors.Count > 0)
        {
            return OperationResult<SessionDetailDto>.BadRequest("Giáo viên không hợp lệ.", roleErrors);
        }

        var conflict = await DetectConflictAsync(
            request.MaPhongHoc,
            request.NgayHoc,
            request.GioBatDau,
            request.GioKetThuc,
            request.MaLopHoc,
            request.MaGiaoVienThucTe,
            request.MaTroGiangThucTe,
            exceptId: null,
            cancellationToken);

        if (conflict is not null)
        {
            return OperationResult<SessionDetailDto>.Conflict(conflict);
        }

        var result = await _sessionStore.CreateAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<SessionDetailDto>.Conflict("Không thể tạo ca học.");
        }

        var created = await _sessionStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<SessionDetailDto>.Created(created!, "Tạo ca học thành công.");
    }

    public async Task<OperationResult<SessionDetailDto>> UpdateAsync(
        int maCaHoc,
        UpdateSessionRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _sessionStore.GetDetailAsync(maCaHoc, cancellationToken);

        if (current is null)
        {
            return OperationResult<SessionDetailDto>.NotFound($"Không tìm thấy ca học có id = {maCaHoc}.");
        }

        if (IsEmptyUpdate(request))
        {
            return OperationResult<SessionDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        var ngayHoc = request.NgayHoc ?? current.NgayHoc;
        var gioBatDau = request.GioBatDau ?? current.GioBatDau;
        var gioKetThuc = request.GioKetThuc ?? current.GioKetThuc;
        var maPhongHoc = request.MaPhongHoc ?? current.MaPhongHoc;
        var maGiaoVien = request.MaGiaoVienThucTe ?? current.MaGiaoVienThucTe;
        var maTroGiang = request.MaTroGiangThucTe ?? current.MaTroGiangThucTe;

        var errors = new List<string>();
        ValidateTimes(gioBatDau, gioKetThuc, ngayHoc, errors);

        if (errors.Count > 0)
        {
            return OperationResult<SessionDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (request.MaPhongHoc is not null
            && !await _lookupStore.RoomExistsAsync(request.MaPhongHoc.Value, cancellationToken))
        {
            return OperationResult<SessionDetailDto>.BadRequest(
                "Phòng học không tồn tại.",
                [$"Không tìm thấy phòng có id = {request.MaPhongHoc}."]);
        }

        var roleErrors = await ValidateTeachersAsync(
            request.MaGiaoVienThucTe,
            request.MaTroGiangThucTe,
            cancellationToken);

        if (roleErrors.Count > 0)
        {
            return OperationResult<SessionDetailDto>.BadRequest("Giáo viên không hợp lệ.", roleErrors);
        }

        var conflict = await DetectConflictAsync(
            maPhongHoc,
            ngayHoc,
            gioBatDau,
            gioKetThuc,
            current.MaLopHoc,
            maGiaoVien,
            maTroGiang,
            maCaHoc,
            cancellationToken);

        if (conflict is not null)
        {
            return OperationResult<SessionDetailDto>.Conflict(conflict);
        }

        var outcome = await _sessionStore.UpdateAsync(maCaHoc, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<SessionDetailDto>.NotFound($"Không tìm thấy ca học có id = {maCaHoc}.")
                : OperationResult<SessionDetailDto>.Conflict("Không thể cập nhật ca học.");
        }

        var updated = await _sessionStore.GetDetailAsync(maCaHoc, cancellationToken);

        return OperationResult<SessionDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<SessionDetailDto>> UpdateStatusAsync(
        int maCaHoc,
        UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = TextValue.Required(request.TrangThai);

        if (trangThai is null || !SessionStatuses.IsValid(trangThai))
        {
            return OperationResult<SessionDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", SessionStatuses.All)}."]);
        }

        if (!await _sessionStore.ExistsAsync(maCaHoc, cancellationToken))
        {
            return OperationResult<SessionDetailDto>.NotFound($"Không tìm thấy ca học có id = {maCaHoc}.");
        }

        var outcome = await _sessionStore.UpdateStatusAsync(maCaHoc, trangThai, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<SessionDetailDto>.Conflict("Không thể cập nhật trạng thái ca học.");
        }

        var updated = await _sessionStore.GetDetailAsync(maCaHoc, cancellationToken);

        return OperationResult<SessionDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<bool>> DeleteAsync(int maCaHoc, CancellationToken cancellationToken)
    {
        if (!await _sessionStore.ExistsAsync(maCaHoc, cancellationToken))
        {
            return OperationResult<bool>.NotFound($"Không tìm thấy ca học có id = {maCaHoc}.");
        }

        var attendanceCount = await _sessionStore.CountAttendanceAsync(maCaHoc, cancellationToken);

        if (attendanceCount > 0)
        {
            return OperationResult<bool>.Conflict(
                $"Không thể xóa ca học vì đã có {attendanceCount} bản ghi điểm danh. " +
                "Hãy dùng chức năng đổi trạng thái sang 'Đã hủy'.");
        }

        var outcome = await _sessionStore.DeleteAsync(maCaHoc, cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.NoContent(),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound($"Không tìm thấy ca học có id = {maCaHoc}."),
            _ => OperationResult<bool>.Conflict("Không thể xóa ca học do ràng buộc dữ liệu.")
        };
    }

    public async Task<OperationResult<GenerateSessionsResponse>> GenerateAsync(
        GenerateSessionsRequest request,
        CancellationToken cancellationToken)
    {
        if (request.TuNgay == default || request.DenNgay == default)
        {
            return OperationResult<GenerateSessionsResponse>.BadRequest(
                "Khoảng ngày không hợp lệ.",
                ["Từ ngày và đến ngày đều bắt buộc."]);
        }

        if (request.DenNgay < request.TuNgay)
        {
            return OperationResult<GenerateSessionsResponse>.BadRequest(
                "Khoảng ngày không hợp lệ.",
                ["Đến ngày phải lớn hơn hoặc bằng từ ngày."]);
        }

        if ((request.DenNgay.DayNumber - request.TuNgay.DayNumber) > 366)
        {
            return OperationResult<GenerateSessionsResponse>.BadRequest(
                "Khoảng ngày quá lớn.",
                ["Chỉ cho phép tạo tối đa 366 ngày trong một lần."]);
        }

        var outcome = await _sessionStore.GenerateSessionsAsync(
            request.MaLichHoc,
            request.TuNgay,
            request.DenNgay,
            cancellationToken);

        if (!outcome.ScheduleFound)
        {
            return OperationResult<GenerateSessionsResponse>.NotFound(
                $"Không tìm thấy lịch học có id = {request.MaLichHoc}.");
        }

        var response = new GenerateSessionsResponse
        {
            MaLichHoc = outcome.MaLichHoc,
            TuNgay = outcome.TuNgay,
            DenNgay = outcome.DenNgay,
            ApDungTuThucTe = outcome.ApDungTuThucTe,
            ApDungDenThucTe = outcome.ApDungDenThucTe,
            TongNgayKhop = outcome.TongNgayKhop,
            NgayTao = outcome.NgayTao,
            NgayDaCo = outcome.NgayDaCo,
            NgayXungDot = outcome.NgayXungDot
                .Select(x => new GenerateConflictDto { NgayHoc = x.NgayHoc, LyDo = x.LyDo })
                .ToList()
        };

        return OperationResult<GenerateSessionsResponse>.Ok(response);
    }

    public async Task<OperationResult<IReadOnlyList<SessionListItemDto>>> ListForStudentAsync(
        string maHocVien,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        CancellationToken cancellationToken)
    {
        // Danh tính lấy từ JWT -> chỉ lấy lớp mà chính học viên đang theo học.
        var classIds = await _lookupStore.GetStudentClassIdsAsync(maHocVien, cancellationToken);

        if (classIds.Count == 0)
        {
            return OperationResult<IReadOnlyList<SessionListItemDto>>.Ok([]);
        }

        var sessions = await _sessionStore.ListByClassIdsAsync(classIds, tuNgay, denNgay, cancellationToken);

        return OperationResult<IReadOnlyList<SessionListItemDto>>.Ok(sessions);
    }

    public async Task<OperationResult<IReadOnlyList<SessionListItemDto>>> ListForTeacherAsync(
        string maGiaoVien,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        CancellationToken cancellationToken)
    {
        var sessions = await _sessionStore.ListByTeacherAsync(maGiaoVien, tuNgay, denNgay, cancellationToken);

        return OperationResult<IReadOnlyList<SessionListItemDto>>.Ok(sessions);
    }

    public async Task<OperationResult<IReadOnlyList<TeacherClassDto>>> ListMyClassesAsync(
        string maGiaoVien,
        CancellationToken cancellationToken)
    {
        var code = TextValue.Required(maGiaoVien);

        if (code is null)
        {
            return OperationResult<IReadOnlyList<TeacherClassDto>>.Forbidden(
                "Không xác định được giáo viên từ token.");
        }

        var classes = await _sessionStore.ListTeacherClassesAsync(code, cancellationToken);

        return OperationResult<IReadOnlyList<TeacherClassDto>>.Ok(classes);
    }

    public async Task<OperationResult<IReadOnlyList<TeacherClassStudentDto>>> ListMyClassStudentsAsync(
        string maGiaoVien,
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        var code = TextValue.Required(maGiaoVien);

        if (code is null)
        {
            return OperationResult<IReadOnlyList<TeacherClassStudentDto>>.Forbidden(
                "Không xác định được giáo viên từ token.");
        }

        if (!await _lookupStore.TeacherAssignedToClassAsync(maLopHoc, code, cancellationToken))
        {
            return OperationResult<IReadOnlyList<TeacherClassStudentDto>>.Forbidden(
                "Bạn không được phân công giảng dạy lớp này.");
        }

        var students = await _lookupStore.GetClassStudentsAsync(maLopHoc, cancellationToken);

        var result = students
            .Select(x => new TeacherClassStudentDto
            {
                MaHocVien = x.MaHocVien,
                HoTen = x.HoTen,
                MaDangKy = x.MaDangKy
            })
            .ToList();

        return OperationResult<IReadOnlyList<TeacherClassStudentDto>>.Ok(result);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static void ValidateTimes(
        TimeOnly gioBatDau,
        TimeOnly gioKetThuc,
        DateOnly ngayHoc,
        List<string> errors)
    {
        if (ngayHoc == default)
        {
            errors.Add("Ngày học là bắt buộc.");
        }

        if (gioBatDau == default)
        {
            errors.Add("Giờ bắt đầu là bắt buộc.");
        }

        if (gioKetThuc == default)
        {
            errors.Add("Giờ kết thúc là bắt buộc.");
        }

        if (gioBatDau != default && gioKetThuc != default && gioKetThuc <= gioBatDau)
        {
            errors.Add("Giờ kết thúc phải sau giờ bắt đầu.");
        }
    }

    /// <summary>Giáo viên phải có hồ sơ "Giáo viên", trợ giảng phải có hồ sơ "Trợ giảng", và không trùng nhau.</summary>
    private async Task<List<string>> ValidateTeachersAsync(
        string? maGiaoVien,
        string? maTroGiang,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (!string.IsNullOrWhiteSpace(maGiaoVien))
        {
            var code = maGiaoVien.Trim();
            var loai = await _referenceStore.GetTeacherTypeAsync(code, cancellationToken);

            if (loai is null)
            {
                errors.Add($"Không tìm thấy giáo viên '{code}'.");
            }
            else if (loai != TeacherTypes.GiaoVien)
            {
                errors.Add($"'{code}' là '{loai}', không thể đảm nhận vai trò giáo viên thực tế.");
            }
        }

        if (!string.IsNullOrWhiteSpace(maTroGiang))
        {
            var code = maTroGiang.Trim();
            var loai = await _referenceStore.GetTeacherTypeAsync(code, cancellationToken);

            if (loai is null)
            {
                errors.Add($"Không tìm thấy trợ giảng '{code}'.");
            }
            else if (loai != TeacherTypes.TroGiang)
            {
                errors.Add($"'{code}' là '{loai}', không thể đảm nhận vai trò trợ giảng thực tế.");
            }
        }

        if (!string.IsNullOrWhiteSpace(maGiaoVien)
            && !string.IsNullOrWhiteSpace(maTroGiang)
            && string.Equals(maGiaoVien.Trim(), maTroGiang.Trim(), StringComparison.Ordinal))
        {
            errors.Add("Giáo viên và trợ giảng không được là cùng một người.");
        }

        return errors;
    }

    /// <summary>Trả về mô tả xung đột, hoặc <c>null</c> nếu không xung đột.</summary>
    private async Task<string?> DetectConflictAsync(
        int maPhongHoc,
        DateOnly ngayHoc,
        TimeOnly gioBatDau,
        TimeOnly gioKetThuc,
        int maLopHoc,
        string? maGiaoVien,
        string? maTroGiang,
        int? exceptId,
        CancellationToken cancellationToken)
    {
        if (await _sessionStore.HasRoomConflictAsync(
                maPhongHoc, ngayHoc, gioBatDau, gioKetThuc, exceptId, cancellationToken))
        {
            return "Phòng học đã có ca học khác trùng giờ trong cùng ngày.";
        }

        if (!string.IsNullOrWhiteSpace(maGiaoVien)
            && await _sessionStore.HasTeacherConflictAsync(
                maGiaoVien.Trim(), ngayHoc, gioBatDau, gioKetThuc, exceptId, cancellationToken))
        {
            return $"Giáo viên '{maGiaoVien.Trim()}' đã có ca học khác trùng giờ trong cùng ngày.";
        }

        if (!string.IsNullOrWhiteSpace(maTroGiang)
            && await _sessionStore.HasTeacherConflictAsync(
                maTroGiang.Trim(), ngayHoc, gioBatDau, gioKetThuc, exceptId, cancellationToken))
        {
            return $"Trợ giảng '{maTroGiang.Trim()}' đã có ca học khác trùng giờ trong cùng ngày.";
        }

        if (await _sessionStore.HasSameClassSameDayAsync(maLopHoc, ngayHoc, exceptId, cancellationToken))
        {
            return "Lớp này đã có ca học trong cùng ngày.";
        }

        return null;
    }

    private static bool IsEmptyUpdate(UpdateSessionRequest request)
        => request.MaPhongHoc is null
           && request.MaGiaoVienThucTe is null
           && request.MaTroGiangThucTe is null
           && request.NgayHoc is null
           && request.GioBatDau is null
           && request.GioKetThuc is null
           && request.NoiDungBaiHoc is null
           && request.LyDoThayDoi is null;
}
