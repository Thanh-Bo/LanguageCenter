using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Students.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Students;

/// <inheritdoc cref="IStudentService"/>
/// <remarks>
/// Toàn bộ logic nghiệp vụ học viên nằm ở đây — Controller không chứa logic.
/// Chỉ phụ thuộc abstraction, KHÔNG phụ thuộc EF Core hay ASP.NET Core.
/// </remarks>
public sealed class StudentService : IStudentService
{
    private readonly IStudentStore _studentStore;
    private readonly IAccountStore _accountStore;
    private readonly IAccountProvisioningStore _provisioningStore;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ITemporaryPasswordGenerator _temporaryPasswordGenerator;

    public StudentService(
        IStudentStore studentStore,
        IAccountStore accountStore,
        IAccountProvisioningStore provisioningStore,
        IPasswordHasherService passwordHasherService,
        ITemporaryPasswordGenerator temporaryPasswordGenerator)
    {
        _studentStore = studentStore;
        _accountStore = accountStore;
        _provisioningStore = provisioningStore;
        _passwordHasherService = passwordHasherService;
        _temporaryPasswordGenerator = temporaryPasswordGenerator;
    }

    // ========================================================================
    // ADMIN
    // ========================================================================

    public async Task<OperationResult<CreateStudentResponse>> CreateAsync(
        CreateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var errors = ValidateCreate(request);

        if (errors.Count > 0)
        {
            return OperationResult<CreateStudentResponse>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        var email = request.Email.Trim();

        // Kiểm tra sớm để có thông báo rõ ràng. Ràng buộc UNIQUE trong database vẫn là chốt cuối
        // và được xử lý tiếp trong store (trường hợp race condition).
        if (await _accountStore.EmailExistsAsync(email, exceptUsername: null, cancellationToken))
        {
            return OperationResult<CreateStudentResponse>.Conflict(
                $"Email '{email}' đã được sử dụng bởi tài khoản khác.");
        }

        // Mật khẩu mặc định của trung tâm ("111111") — chỉ lưu dạng hash, không log plain text.
        var temporaryPassword = _temporaryPasswordGenerator.Generate();
        var passwordHash = _passwordHasherService.HashPassword(temporaryPassword);

        var provisionData = new StudentProvisionData(
            VaiTro: RoleNames.HocVien,
            HoTen: request.HoTen.Trim(),
            NgaySinh: request.NgaySinh,
            GioiTinh: TextValue.Optional(request.GioiTinh),
            Email: email,
            SoDienThoai: TextValue.Optional(request.SoDienThoai),
            DiaChi: TextValue.Optional(request.DiaChi),
            MucTieuHocTap: TextValue.Optional(request.MucTieuHocTap),
            TrinhDoHienTai: TextValue.Optional(request.TrinhDoHienTai),
            TrangThaiHocTap: StudentStatuses.DangTuVan,
            PhuHuynh: MapParent(request.PhuHuynh));

        var result = await _provisioningStore.CreateStudentAsync(provisionData, passwordHash, cancellationToken);

        if (!result.Succeeded)
        {
            return MapProvisionFailure(result, email);
        }

        var code = result.Code!;

        // temporaryPassword CHỈ xuất hiện trong response này — không có endpoint đọc lại.
        return OperationResult<CreateStudentResponse>.Created(
            new CreateStudentResponse
            {
                StudentId = code,
                Username = code,
                TemporaryPassword = temporaryPassword,
                FullName = provisionData.HoTen,
                Email = email,
                Role = RoleNames.HocVien,
                Message = "Tạo học viên thành công. Mật khẩu mặc định: 111111 — hãy gửi ngay cho học viên và nhắc đổi mật khẩu sau khi đăng nhập."
            },
            "Tạo học viên thành công.");
    }

    public async Task<OperationResult<PagedResult<StudentListItemDto>>> ListAsync(
        StudentListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai) && !StudentStatuses.IsValid(query.TrangThai))
        {
            return OperationResult<PagedResult<StudentListItemDto>>.BadRequest(
                "Trạng thái học tập không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", StudentStatuses.All)}."]);
        }

        var page = await _studentStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<StudentListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<StudentDetailDto>> GetDetailAsync(
        string maHocVien,
        CancellationToken cancellationToken)
    {
        var code = maHocVien.Trim();
        var profile = await _studentStore.GetByCodeAsync(code, cancellationToken);

        return profile is null
            ? OperationResult<StudentDetailDto>.NotFound($"Không tìm thấy học viên '{code}'.")
            : OperationResult<StudentDetailDto>.Ok(profile);
    }

    public async Task<OperationResult<StudentDetailDto>> UpdateAsync(
        string maHocVien,
        UpdateStudentRequest request,
        CancellationToken cancellationToken)
    {
        var errors = ValidateUpdate(request);

        if (errors.Count > 0)
        {
            return OperationResult<StudentDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        var code = maHocVien.Trim();

        if (await _studentStore.GetByCodeAsync(code, cancellationToken) is null)
        {
            return OperationResult<StudentDetailDto>.NotFound($"Không tìm thấy học viên '{code}'.");
        }

        if (IsEmptyUpdate(request))
        {
            return OperationResult<StudentDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        var outcome = await _studentStore.UpdateAsync(code, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return MapWriteOutcome(outcome, code);
        }

        var updated = await _studentStore.GetByCodeAsync(code, cancellationToken);

        return OperationResult<StudentDetailDto>.Ok(updated!);
    }

    // ========================================================================
    // SELF PROFILE
    // ========================================================================

    public async Task<OperationResult<StudentDetailDto>> GetMyProfileAsync(
        string tenDangNhap,
        CancellationToken cancellationToken)
    {
        // tenDangNhap lấy từ JWT. Vì MaHocVien == TenDangNhap nên tra thẳng theo mã hồ sơ.
        var code = tenDangNhap.Trim();
        var profile = await _studentStore.GetByCodeAsync(code, cancellationToken);

        return profile is null
            ? OperationResult<StudentDetailDto>.NotFound($"Tài khoản '{code}' chưa có hồ sơ học viên.")
            : OperationResult<StudentDetailDto>.Ok(profile);
    }

    public Task<OperationResult<StudentDetailDto>> UpdateMyProfileAsync(
        string tenDangNhap,
        UpdateMyStudentProfileRequest request,
        CancellationToken cancellationToken)
    {
        // Chuyển sang request nội bộ, CHỈ mang các field học viên được phép tự sửa.
        // TrangThaiHocTap / NgayNhapHoc / NgaySinh KHÔNG được set ở đây nên không thể bị bind.
        var internalRequest = new UpdateStudentRequest
        {
            HoTen = request.HoTen,
            GioiTinh = request.GioiTinh,
            SoDienThoai = request.SoDienThoai,
            Email = request.Email,
            DiaChi = request.DiaChi,
            MucTieuHocTap = request.MucTieuHocTap
        };

        return UpdateAsync(tenDangNhap, internalRequest, cancellationToken);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    public async Task<OperationResult<bool>> DeleteAsync(string maHocVien, CancellationToken cancellationToken)
    {
        var account = await _accountStore.FindByUsernameAsync(maHocVien, cancellationToken);

        if (account is null || account.TenVaiTro != RoleNames.HocVien)
        {
            return OperationResult<bool>.NotFound($"Không tìm thấy học viên '{maHocVien}'.");
        }

        // Xóa cứng chỉ khi không còn dữ liệu nghiệp vụ phụ thuộc (đăng ký, điểm danh, học phí...).
        var check = await _accountStore.CheckDeletableAsync(account.MaTaiKhoan, cancellationToken);

        if (!check.CanDelete)
        {
            return OperationResult<bool>.Conflict(
                $"Không thể xóa học viên '{maHocVien}' vì đang có dữ liệu liên quan ({check.Reason}). "
                + "Hãy dùng chức năng Sửa và đổi Trạng thái học tập sang 'Đã nghỉ' thay vì xóa.");
        }

        var outcome = await _accountStore.DeleteAsync(account.MaTaiKhoan, cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.NoContent(),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound($"Không tìm thấy học viên '{maHocVien}'."),
            _ => OperationResult<bool>.Conflict("Không thể xóa học viên do ràng buộc dữ liệu.")
        };
    }

    private static ParentProvisionData? MapParent(CreateParentRequest? parent)
    {
        if (parent is null)
        {
            return null;
        }

        return new ParentProvisionData(
            HoTen: parent.HoTen.Trim(),
            SoDienThoai: TextValue.Optional(parent.SoDienThoai),
            Email: TextValue.Optional(parent.Email),
            DiaChi: TextValue.Optional(parent.DiaChi),
            NgheNghiep: TextValue.Optional(parent.NgheNghiep),
            MoiQuanHe: parent.MoiQuanHe.Trim());
    }

    private static List<string> ValidateCreate(CreateStudentRequest request)
    {
        var errors = new List<string>();

        if (!TextValue.IsPlausibleBirthDate(request.NgaySinh, out var birthDateError))
        {
            errors.Add(birthDateError!);
        }

        if (!TextValue.IsPlausiblePhone(request.SoDienThoai, out var phoneError))
        {
            errors.Add(phoneError!);
        }

        if (request.PhuHuynh is not null)
        {
            if (string.IsNullOrWhiteSpace(request.PhuHuynh.HoTen))
            {
                errors.Add("Họ tên phụ huynh là bắt buộc.");
            }

            // Bắt buộc vì database có CHECK CK_HocVien_PhuHuynh:
            // MaPhuHuynh và MoiQuanHePhuHuynh phải cùng NULL hoặc cùng NOT NULL.
            if (string.IsNullOrWhiteSpace(request.PhuHuynh.MoiQuanHe))
            {
                errors.Add("Mối quan hệ với học viên là bắt buộc khi có phụ huynh.");
            }

            if (!TextValue.IsPlausiblePhone(request.PhuHuynh.SoDienThoai, out var parentPhoneError))
            {
                errors.Add($"Số điện thoại phụ huynh: {parentPhoneError}");
            }
        }

        return errors;
    }

    private static List<string> ValidateUpdate(UpdateStudentRequest request)
    {
        var errors = new List<string>();

        if (request.HoTen is not null && string.IsNullOrWhiteSpace(request.HoTen))
        {
            errors.Add("Họ tên không được để trống.");
        }

        // Email là field bắt buộc ở database (NOT NULL) nên không thể xóa.
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

        if (request.TrangThaiHocTap is not null && !StudentStatuses.IsValid(request.TrangThaiHocTap))
        {
            errors.Add($"Trạng thái học tập không hợp lệ. Chỉ chấp nhận: {string.Join(", ", StudentStatuses.All)}.");
        }

        if (request.NgayNhapHoc is { } ngayNhapHoc && ngayNhapHoc > DateOnly.FromDateTime(DateTime.Today).AddYears(1))
        {
            errors.Add("Ngày nhập học không hợp lệ.");
        }

        return errors;
    }

    private static bool IsEmptyUpdate(UpdateStudentRequest request)
        => request.HoTen is null
           && request.NgaySinh is null
           && request.GioiTinh is null
           && request.Email is null
           && request.SoDienThoai is null
           && request.DiaChi is null
           && request.MucTieuHocTap is null
           && request.TrinhDoHienTai is null
           && request.TrangThaiHocTap is null
           && request.NgayNhapHoc is null;

    private static OperationResult<CreateStudentResponse> MapProvisionFailure(ProvisionResult result, string email)
        => result.FailureReason switch
        {
            ProvisionFailureReason.EmailAlreadyExists =>
                OperationResult<CreateStudentResponse>.Conflict($"Email '{email}' đã được sử dụng bởi tài khoản khác."),

            ProvisionFailureReason.RoleNotFound =>
                OperationResult<CreateStudentResponse>.BadRequest(
                    $"Không tìm thấy vai trò '{RoleNames.HocVien}' trong bảng VaiTro.",
                    ["Hãy kiểm tra dữ liệu bảng VaiTro."]),

            _ => OperationResult<CreateStudentResponse>.Conflict(
                "Không sinh được mã học viên duy nhất sau nhiều lần thử. Vui lòng thử lại.")
        };

    private static OperationResult<StudentDetailDto> MapWriteOutcome(WriteOutcome outcome, string code)
        => outcome switch
        {
            WriteOutcome.NotFound =>
                OperationResult<StudentDetailDto>.NotFound($"Không tìm thấy học viên '{code}'."),

            WriteOutcome.DuplicateEmail =>
                OperationResult<StudentDetailDto>.Conflict("Email đã được sử dụng bởi tài khoản khác."),

            _ => OperationResult<StudentDetailDto>.Conflict("Không thể cập nhật hồ sơ học viên.")
        };
}
