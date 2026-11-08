using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Employees.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Employees;

/// <inheritdoc cref="IEmployeeService"/>
/// <remarks>
/// Dùng CHUNG cho Nhân viên học vụ / tư vấn / kế toán: <c>ChucVu</c> quyết định
/// VAI TRÒ và TIỀN TỐ mã. Controller không chứa logic.
/// </remarks>
public sealed class EmployeeService : IEmployeeService
{
    /// <summary>Quy tắc nghiệp vụ: nhân viên phải đủ 18 tuổi trở lên.</summary>
    private const int TuoiToiThieu = 18;

    private readonly IEmployeeStore _employeeStore;
    private readonly IAccountStore _accountStore;
    private readonly IAccountProvisioningStore _provisioningStore;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ITemporaryPasswordGenerator _temporaryPasswordGenerator;

    public EmployeeService(
        IEmployeeStore employeeStore,
        IAccountStore accountStore,
        IAccountProvisioningStore provisioningStore,
        IPasswordHasherService passwordHasherService,
        ITemporaryPasswordGenerator temporaryPasswordGenerator)
    {
        _employeeStore = employeeStore;
        _accountStore = accountStore;
        _provisioningStore = provisioningStore;
        _passwordHasherService = passwordHasherService;
        _temporaryPasswordGenerator = temporaryPasswordGenerator;
    }

    public async Task<OperationResult<CreateEmployeeResponse>> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var errors = ValidateCreate(request);

        if (errors.Count > 0)
        {
            return OperationResult<CreateEmployeeResponse>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        // Chức vụ quyết định VAI TRÒ — client không bao giờ gửi role.
        var chucVu = request.ChucVu.Trim();
        var vaiTro = StaffPositions.ResolveRole(chucVu);

        var email = request.Email.Trim();

        if (await _accountStore.EmailExistsAsync(email, exceptUsername: null, cancellationToken))
        {
            return OperationResult<CreateEmployeeResponse>.Conflict(
                $"Email '{email}' đã được sử dụng bởi tài khoản khác.");
        }

        var soDienThoai = TextValue.Optional(request.SoDienThoai);

        if (soDienThoai is not null
            && await _employeeStore.PhoneExistsAsync(soDienThoai, exceptMaNhanVien: null, cancellationToken))
        {
            return OperationResult<CreateEmployeeResponse>.Conflict(
                $"Số điện thoại '{soDienThoai}' đã được dùng cho hồ sơ khác "
                + "(mỗi số điện thoại chỉ gắn với một hồ sơ học viên / phụ huynh / giáo viên / nhân viên).");
        }

        var temporaryPassword = _temporaryPasswordGenerator.Generate();
        var passwordHash = _passwordHasherService.HashPassword(temporaryPassword);

        var provisionData = new EmployeeProvisionData(
            VaiTro: vaiTro,
            TienTo: StaffPositions.ResolvePrefix(chucVu),
            HoTen: request.HoTen.Trim(),
            NgaySinh: request.NgaySinh,
            GioiTinh: TextValue.Optional(request.GioiTinh),
            Email: email,
            SoDienThoai: soDienThoai,
            DiaChi: TextValue.Optional(request.DiaChi),
            ChucVu: chucVu,
            NgayVaoLam: request.NgayVaoLam,
            TrangThai: WorkStatuses.DangLamViec);

        var result = await _provisioningStore.CreateEmployeeAsync(provisionData, passwordHash, cancellationToken);

        if (!result.Succeeded)
        {
            return MapProvisionFailure(result, email, vaiTro);
        }

        var code = result.Code!;

        return OperationResult<CreateEmployeeResponse>.Created(
            new CreateEmployeeResponse
            {
                EmployeeId = code,
                Username = code,
                TemporaryPassword = temporaryPassword,
                FullName = provisionData.HoTen,
                Email = email,
                ChucVu = chucVu,
                Role = vaiTro,
                Message = $"Tạo {chucVu.ToLowerInvariant()} thành công. "
                    + $"Mật khẩu mặc định: {temporaryPassword} — hãy gửi ngay cho người dùng và nhắc đổi mật khẩu sau khi đăng nhập."
            },
            $"Tạo {chucVu} thành công.");
    }

    public async Task<OperationResult<PagedResult<EmployeeListItemDto>>> ListAsync(
        EmployeeListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.ChucVu) && !StaffPositions.IsValid(query.ChucVu))
        {
            return OperationResult<PagedResult<EmployeeListItemDto>>.BadRequest(
                "Chức vụ không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", StaffPositions.All)}."]);
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai) && !WorkStatuses.IsValid(query.TrangThai))
        {
            return OperationResult<PagedResult<EmployeeListItemDto>>.BadRequest(
                "Trạng thái làm việc không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", WorkStatuses.All)}."]);
        }

        var page = await _employeeStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<EmployeeListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<EmployeeDetailDto>> GetDetailAsync(
        string maNhanVien,
        CancellationToken cancellationToken)
    {
        var code = maNhanVien.Trim();
        var profile = await _employeeStore.GetByCodeAsync(code, cancellationToken);

        return profile is null
            ? OperationResult<EmployeeDetailDto>.NotFound($"Không tìm thấy nhân viên '{code}'.")
            : OperationResult<EmployeeDetailDto>.Ok(profile);
    }

    public async Task<OperationResult<EmployeeDetailDto>> UpdateAsync(
        string maNhanVien,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var errors = ValidateUpdate(request);

        if (errors.Count > 0)
        {
            return OperationResult<EmployeeDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        var code = maNhanVien.Trim();

        if (await _employeeStore.GetByCodeAsync(code, cancellationToken) is null)
        {
            return OperationResult<EmployeeDetailDto>.NotFound($"Không tìm thấy nhân viên '{code}'.");
        }

        if (IsEmptyUpdate(request))
        {
            return OperationResult<EmployeeDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        var soDienThoai = TextValue.Optional(request.SoDienThoai);

        if (request.SoDienThoai is not null
            && soDienThoai is not null
            && await _employeeStore.PhoneExistsAsync(soDienThoai, exceptMaNhanVien: code, cancellationToken))
        {
            return OperationResult<EmployeeDetailDto>.Conflict(
                $"Số điện thoại '{soDienThoai}' đã được dùng cho hồ sơ khác.");
        }

        var outcome = await _employeeStore.UpdateAsync(code, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return MapWriteOutcome(outcome, code);
        }

        var updated = await _employeeStore.GetByCodeAsync(code, cancellationToken);

        return OperationResult<EmployeeDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<bool>> DeleteAsync(string maNhanVien, CancellationToken cancellationToken)
    {
        var code = maNhanVien.Trim();

        var account = await _accountStore.FindByUsernameAsync(code, cancellationToken);

        if (account is null
            || (account.TenVaiTro != RoleNames.NhanVienHocVu
                && account.TenVaiTro != RoleNames.NhanVienTuVan
                && account.TenVaiTro != RoleNames.NhanVienKeToan))
        {
            return OperationResult<bool>.NotFound($"Không tìm thấy nhân viên '{code}'.");
        }

        var check = await _accountStore.CheckDeletableAsync(account.MaTaiKhoan, cancellationToken);

        if (!check.CanDelete)
        {
            return OperationResult<bool>.Conflict(
                $"Không thể xóa '{code}' vì đang có dữ liệu liên quan ({check.Reason}). "
                + "Hãy dùng chức năng Sửa và đổi Trạng thái làm việc thay vì xóa.");
        }

        var outcome = await _accountStore.DeleteAsync(account.MaTaiKhoan, cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.NoContent(),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound($"Không tìm thấy nhân viên '{code}'."),
            _ => OperationResult<bool>.Conflict("Không thể xóa nhân viên do ràng buộc dữ liệu.")
        };
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static List<string> ValidateCreate(CreateEmployeeRequest request)
    {
        var errors = new List<string>();

        if (!StaffPositions.IsValid(request.ChucVu))
        {
            errors.Add($"Chức vụ không hợp lệ. Chỉ chấp nhận: {string.Join(", ", StaffPositions.All)}.");
        }

        if (request.NgaySinh is { } ngaySinh && !TextValue.IsPlausibleBirthDate(ngaySinh, out var birthDateError))
        {
            errors.Add(birthDateError!);
        }

        if (request.NgaySinh is { } ngaySinhTuoi
            && !TextValue.IsOldEnough(ngaySinhTuoi, TuoiToiThieu, out var ageError, "Nhân viên"))
        {
            errors.Add(ageError!);
        }

        if (!TextValue.IsPlausiblePhone(request.SoDienThoai, out var phoneError))
        {
            errors.Add(phoneError!);
        }

        if (request.NgayVaoLam is { } ngayVaoLam && ngayVaoLam > DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add("Ngày vào làm không được ở tương lai.");
        }

        return errors;
    }

    private static List<string> ValidateUpdate(UpdateEmployeeRequest request)
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

        if (request.NgaySinh is { } ngaySinhTuoi
            && !TextValue.IsOldEnough(ngaySinhTuoi, TuoiToiThieu, out var ageError, "Nhân viên"))
        {
            errors.Add(ageError!);
        }

        if (!TextValue.IsPlausiblePhone(request.SoDienThoai, out var phoneError))
        {
            errors.Add(phoneError!);
        }

        if (request.NgayVaoLam is { } ngayVaoLam && ngayVaoLam > DateOnly.FromDateTime(DateTime.Today))
        {
            errors.Add("Ngày vào làm không được ở tương lai.");
        }

        if (request.TrangThaiLamViec is not null && !WorkStatuses.IsValid(request.TrangThaiLamViec))
        {
            errors.Add($"Trạng thái làm việc không hợp lệ. Chỉ chấp nhận: {string.Join(", ", WorkStatuses.All)}.");
        }

        return errors;
    }

    private static bool IsEmptyUpdate(UpdateEmployeeRequest request)
        => request.HoTen is null
           && request.NgaySinh is null
           && request.GioiTinh is null
           && request.Email is null
           && request.SoDienThoai is null
           && request.DiaChi is null
           && request.NgayVaoLam is null
           && request.TrangThaiLamViec is null;

    private static OperationResult<CreateEmployeeResponse> MapProvisionFailure(
        ProvisionResult result,
        string email,
        string vaiTro)
        => result.FailureReason switch
        {
            ProvisionFailureReason.EmailAlreadyExists =>
                OperationResult<CreateEmployeeResponse>.Conflict(
                    $"Email '{email}' đã được sử dụng bởi tài khoản khác."),

            ProvisionFailureReason.RoleNotFound =>
                OperationResult<CreateEmployeeResponse>.BadRequest(
                    $"Không tìm thấy vai trò '{vaiTro}' trong bảng VaiTro.",
                    ["Hãy kiểm tra dữ liệu bảng VaiTro."]),

            _ => OperationResult<CreateEmployeeResponse>.Conflict(
                "Không sinh được mã nhân viên duy nhất sau nhiều lần thử. Vui lòng thử lại.")
        };

    private static OperationResult<EmployeeDetailDto> MapWriteOutcome(WriteOutcome outcome, string code)
        => outcome switch
        {
            WriteOutcome.NotFound =>
                OperationResult<EmployeeDetailDto>.NotFound($"Không tìm thấy nhân viên '{code}'."),

            WriteOutcome.DuplicateEmail =>
                OperationResult<EmployeeDetailDto>.Conflict("Email đã được sử dụng bởi tài khoản khác."),

            _ => OperationResult<EmployeeDetailDto>.Conflict("Không thể cập nhật hồ sơ nhân viên.")
        };
}
