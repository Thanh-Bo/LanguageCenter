using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Teachers.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Teachers;

/// <inheritdoc cref="ITeacherService"/>
/// <remarks>
/// Dùng CHUNG cho Giáo viên và Trợ giảng: <c>LoaiGiaoVien</c> quyết định mã sinh ra và vai trò.
/// Toàn bộ logic nằm ở đây — Controller không chứa logic.
/// </remarks>
public sealed class TeacherService : ITeacherService
{
    private readonly ITeacherStore _teacherStore;
    private readonly IAccountStore _accountStore;
    private readonly IAccountProvisioningStore _provisioningStore;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ITemporaryPasswordGenerator _temporaryPasswordGenerator;

    public TeacherService(
        ITeacherStore teacherStore,
        IAccountStore accountStore,
        IAccountProvisioningStore provisioningStore,
        IPasswordHasherService passwordHasherService,
        ITemporaryPasswordGenerator temporaryPasswordGenerator)
    {
        _teacherStore = teacherStore;
        _accountStore = accountStore;
        _provisioningStore = provisioningStore;
        _passwordHasherService = passwordHasherService;
        _temporaryPasswordGenerator = temporaryPasswordGenerator;
    }

    // ========================================================================
    // ADMIN
    // ========================================================================

    public async Task<OperationResult<CreateTeacherResponse>> CreateAsync(
        CreateTeacherRequest request,
        CancellationToken cancellationToken)
    {
        var errors = ValidateCreate(request);

        if (errors.Count > 0)
        {
            return OperationResult<CreateTeacherResponse>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        // LoaiGiaoVien quyết định VAI TRÒ. Client KHÔNG bao giờ gửi role.
        var loaiGiaoVien = request.LoaiGiaoVien.Trim();
        var vaiTro = ResolveRole(loaiGiaoVien);

        var email = request.Email.Trim();

        if (await _accountStore.EmailExistsAsync(email, exceptUsername: null, cancellationToken))
        {
            return OperationResult<CreateTeacherResponse>.Conflict(
                $"Email '{email}' đã được sử dụng bởi tài khoản khác.");
        }

        var soDienThoai = TextValue.Optional(request.SoDienThoai);

        if (soDienThoai is not null
            && await _teacherStore.PhoneExistsAsync(soDienThoai, exceptMaGiaoVien: null, cancellationToken))
        {
            return OperationResult<CreateTeacherResponse>.Conflict(
                $"Số điện thoại '{soDienThoai}' đã được dùng cho hồ sơ giáo viên / trợ giảng khác.");
        }

        var temporaryPassword = _temporaryPasswordGenerator.Generate();
        var passwordHash = _passwordHasherService.HashPassword(temporaryPassword);

        var provisionData = new TeacherProvisionData(
            VaiTro: vaiTro,
            HoTen: request.HoTen.Trim(),
            NgaySinh: request.NgaySinh,
            GioiTinh: TextValue.Optional(request.GioiTinh),
            Email: email,
            SoDienThoai: TextValue.Optional(request.SoDienThoai),
            DiaChi: TextValue.Optional(request.DiaChi),
            LoaiGiaoVien: loaiGiaoVien,
            ChuyenMon: TextValue.Optional(request.ChuyenMon),
            SoNamKinhNghiem: request.SoNamKinhNghiem,
            NgayVaoLam: request.NgayVaoLam,
            TrangThai: WorkStatuses.DangLamViec);

        var result = await _provisioningStore.CreateTeacherAsync(provisionData, passwordHash, cancellationToken);

        if (!result.Succeeded)
        {
            return MapProvisionFailure(result, email, vaiTro);
        }

        var code = result.Code!;

        return OperationResult<CreateTeacherResponse>.Created(
            new CreateTeacherResponse
            {
                TeacherId = code,
                Username = code,
                TemporaryPassword = temporaryPassword,
                FullName = provisionData.HoTen,
                Email = email,
                LoaiGiaoVien = loaiGiaoVien,
                Role = vaiTro,
                Message = $"Tạo {loaiGiaoVien.ToLowerInvariant()} thành công. Mật khẩu mặc định: 111111 — hãy gửi ngay cho người dùng và nhắc đổi mật khẩu sau khi đăng nhập."
            },
            $"Tạo {loaiGiaoVien} thành công.");
    }

    public async Task<OperationResult<PagedResult<TeacherListItemDto>>> ListAsync(
        TeacherListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.LoaiGiaoVien) && !TeacherTypes.IsValid(query.LoaiGiaoVien))
        {
            return OperationResult<PagedResult<TeacherListItemDto>>.BadRequest(
                "Loại giáo viên không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", TeacherTypes.All)}."]);
        }

        var page = await _teacherStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<TeacherListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<TeacherDetailDto>> GetDetailAsync(
        string maGiaoVien,
        CancellationToken cancellationToken)
    {
        var code = maGiaoVien.Trim();
        var profile = await _teacherStore.GetByCodeAsync(code, cancellationToken);

        return profile is null
            ? OperationResult<TeacherDetailDto>.NotFound($"Không tìm thấy giáo viên '{code}'.")
            : OperationResult<TeacherDetailDto>.Ok(profile);
    }

    public async Task<OperationResult<TeacherDetailDto>> UpdateAsync(
        string maGiaoVien,
        UpdateTeacherRequest request,
        CancellationToken cancellationToken)
    {
        var errors = ValidateUpdate(request);

        if (errors.Count > 0)
        {
            return OperationResult<TeacherDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        var code = maGiaoVien.Trim();

        if (await _teacherStore.GetByCodeAsync(code, cancellationToken) is null)
        {
            return OperationResult<TeacherDetailDto>.NotFound($"Không tìm thấy giáo viên '{code}'.");
        }

        if (IsEmptyUpdate(request))
        {
            return OperationResult<TeacherDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        var outcome = await _teacherStore.UpdateAsync(code, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return MapWriteOutcome(outcome, code);
        }

        var updated = await _teacherStore.GetByCodeAsync(code, cancellationToken);

        return OperationResult<TeacherDetailDto>.Ok(updated!);
    }

    // ========================================================================
    // SELF PROFILE
    // ========================================================================

    public async Task<OperationResult<TeacherDetailDto>> GetMyProfileAsync(
        string tenDangNhap,
        CancellationToken cancellationToken)
    {
        // tenDangNhap lấy từ JWT. MaGiaoVien == TenDangNhap (GV001 ↔ GV001, TG001 ↔ TG001).
        var code = tenDangNhap.Trim();
        var profile = await _teacherStore.GetByCodeAsync(code, cancellationToken);

        return profile is null
            ? OperationResult<TeacherDetailDto>.NotFound($"Tài khoản '{code}' chưa có hồ sơ giáo viên.")
            : OperationResult<TeacherDetailDto>.Ok(profile);
    }

    public Task<OperationResult<TeacherDetailDto>> UpdateMyProfileAsync(
        string tenDangNhap,
        UpdateMyTeacherProfileRequest request,
        CancellationToken cancellationToken)
    {
        // CHỈ mang các field giáo viên được phép tự sửa.
        // LoaiGiaoVien / ChuyenMon / SoNamKinhNghiem / NgayVaoLam / TrangThai KHÔNG được set.
        var internalRequest = new UpdateTeacherRequest
        {
            HoTen = request.HoTen,
            GioiTinh = request.GioiTinh,
            SoDienThoai = request.SoDienThoai,
            Email = request.Email,
            DiaChi = request.DiaChi
        };

        return UpdateAsync(tenDangNhap, internalRequest, cancellationToken);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    public async Task<OperationResult<bool>> DeleteAsync(string maGiaoVien, CancellationToken cancellationToken)
    {
        var account = await _accountStore.FindByUsernameAsync(maGiaoVien, cancellationToken);

        if (account is null
            || (account.TenVaiTro != RoleNames.GiaoVien && account.TenVaiTro != RoleNames.TroGiang))
        {
            return OperationResult<bool>.NotFound($"Không tìm thấy giáo viên / trợ giảng '{maGiaoVien}'.");
        }

        var check = await _accountStore.CheckDeletableAsync(account.MaTaiKhoan, cancellationToken);

        if (!check.CanDelete)
        {
            return OperationResult<bool>.Conflict(
                $"Không thể xóa '{maGiaoVien}' vì đang có dữ liệu liên quan ({check.Reason}). "
                + "Hãy dùng chức năng Sửa và đổi Trạng thái làm việc thay vì xóa.");
        }

        var outcome = await _accountStore.DeleteAsync(account.MaTaiKhoan, cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.NoContent(),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound($"Không tìm thấy giáo viên / trợ giảng '{maGiaoVien}'."),
            _ => OperationResult<bool>.Conflict("Không thể xóa giáo viên / trợ giảng do ràng buộc dữ liệu.")
        };
    }

    /// <summary>"Giáo viên" → RoleNames.GiaoVien; "Trợ giảng" → RoleNames.TroGiang.</summary>
    private static string ResolveRole(string loaiGiaoVien)
        => loaiGiaoVien == TeacherTypes.GiaoVien ? RoleNames.GiaoVien : RoleNames.TroGiang;

    private static List<string> ValidateCreate(CreateTeacherRequest request)
    {
        var errors = new List<string>();

        if (!TeacherTypes.IsValid(request.LoaiGiaoVien?.Trim()))
        {
            errors.Add($"Loại giáo viên không hợp lệ. Chỉ chấp nhận: {string.Join(", ", TeacherTypes.All)}.");
        }

        if (request.NgaySinh is { } ngaySinh && !TextValue.IsPlausibleBirthDate(ngaySinh, out var birthDateError))
        {
            errors.Add(birthDateError!);
        }

        if (!TextValue.IsPlausiblePhone(request.SoDienThoai, out var phoneError))
        {
            errors.Add(phoneError!);
        }

        // Khớp CHECK constraint CK_GiaoVien_KinhNghiem và CK_GiaoVien_Loai của database.
        if (request.SoNamKinhNghiem is < 0)
        {
            errors.Add("Số năm kinh nghiệm không được âm.");
        }

        if (request.NgayVaoLam is { } ngayVaoLam && ngayVaoLam > DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add("Ngày vào làm không được ở tương lai.");
        }

        return errors;
    }

    private static List<string> ValidateUpdate(UpdateTeacherRequest request)
    {
        var errors = new List<string>();

        if (request.HoTen is not null && string.IsNullOrWhiteSpace(request.HoTen))
        {
            errors.Add("Họ tên không được để trống.");
        }

        if (request.Email is not null && string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add("Email không được để trống.");
        }

        if (request.NgaySinh is { } ngaySinh && !TextValue.IsPlausibleBirthDate(ngaySinh, out var birthDateError))
        {
            errors.Add(birthDateError!);
        }

        if (!TextValue.IsPlausiblePhone(request.SoDienThoai, out var phoneError))
        {
            errors.Add(phoneError!);
        }

        if (request.SoNamKinhNghiem is < 0)
        {
            errors.Add("Số năm kinh nghiệm không được âm.");
        }

        if (request.NgayVaoLam is { } ngayVaoLam && ngayVaoLam > DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add("Ngày vào làm không được ở tương lai.");
        }

        if (request.TrangThai is not null && !WorkStatuses.IsValid(request.TrangThai))
        {
            errors.Add($"Trạng thái không hợp lệ. Chỉ chấp nhận: {string.Join(", ", WorkStatuses.All)}.");
        }

        return errors;
    }

    private static bool IsEmptyUpdate(UpdateTeacherRequest request)
        => request.HoTen is null
           && request.NgaySinh is null
           && request.GioiTinh is null
           && request.Email is null
           && request.SoDienThoai is null
           && request.DiaChi is null
           && request.ChuyenMon is null
           && request.SoNamKinhNghiem is null
           && request.NgayVaoLam is null
           && request.TrangThai is null;

    private static OperationResult<CreateTeacherResponse> MapProvisionFailure(
        ProvisionResult result,
        string email,
        string vaiTro)
        => result.FailureReason switch
        {
            ProvisionFailureReason.EmailAlreadyExists =>
                OperationResult<CreateTeacherResponse>.Conflict($"Email '{email}' đã được sử dụng bởi tài khoản khác."),

            ProvisionFailureReason.RoleNotFound =>
                OperationResult<CreateTeacherResponse>.BadRequest(
                    $"Không tìm thấy vai trò '{vaiTro}' trong bảng VaiTro.",
                    ["Hãy kiểm tra dữ liệu bảng VaiTro."]),

            _ => OperationResult<CreateTeacherResponse>.Conflict(
                "Không sinh được mã giáo viên duy nhất sau nhiều lần thử. Vui lòng thử lại.")
        };

    private static OperationResult<TeacherDetailDto> MapWriteOutcome(WriteOutcome outcome, string code)
        => outcome switch
        {
            WriteOutcome.NotFound =>
                OperationResult<TeacherDetailDto>.NotFound($"Không tìm thấy giáo viên '{code}'."),

            WriteOutcome.DuplicateEmail =>
                OperationResult<TeacherDetailDto>.Conflict("Email đã được sử dụng bởi tài khoản khác."),

            _ => OperationResult<TeacherDetailDto>.Conflict("Không thể cập nhật hồ sơ giáo viên.")
        };
}
