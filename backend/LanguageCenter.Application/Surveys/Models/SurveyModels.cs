using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Surveys.Models;

// ============================================================================
// ACTOR
// ============================================================================

/// <summary>Danh tính học viên trả lời khảo sát — lấy từ JWT (<c>unique_name</c> = <c>MaHocVien</c>).</summary>
public sealed record SurveyStudentActor(string MaHocVien);

// ============================================================================
// REQUEST — KHẢO SÁT
// ============================================================================

/// <summary>Tham số lọc danh sách khảo sát.</summary>
public sealed class SurveyListQuery : PagedQuery
{
    /// <summary>"Hoạt động" / "Tạm dừng" / "Đã kết thúc".</summary>
    public string? TrangThai { get; set; }

    /// <summary>Lọc theo <c>NgayBatDau</c>.</summary>
    public DateOnly? TuNgay { get; set; }

    public DateOnly? DenNgay { get; set; }

    public string? Search { get; set; }
}

/// <summary>Body của <c>POST /api/admin/surveys</c>.</summary>
public sealed class CreateSurveyRequest
{
    [Required(ErrorMessage = "Tên khảo sát là bắt buộc.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Tên khảo sát phải từ 3 đến 255 ký tự.")]
    public string TenKhaoSat { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự.")]
    public string? MoTa { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
    public DateOnly NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    /// <summary>Để trống thì mặc định "Hoạt động" (khớp DEFAULT của database).</summary>
    [StringLength(50)]
    public string? TrangThai { get; set; }
}

/// <summary>Body của <c>PATCH /api/admin/surveys/{id}</c> — mọi field OPTIONAL.</summary>
public sealed class UpdateSurveyRequest
{
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Tên khảo sát phải từ 3 đến 255 ký tự.")]
    public string? TenKhaoSat { get; set; }

    /// <summary>Gửi chuỗi rỗng để xóa mô tả.</summary>
    [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự.")]
    public string? MoTa { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    /// <summary>Gửi <c>true</c> để xóa ngày kết thúc (không giới hạn).</summary>
    public bool XoaNgayKetThuc { get; set; }
}

// ============================================================================
// REQUEST — CÂU HỎI
// ============================================================================

/// <summary>Body của <c>POST /api/admin/surveys/{id}/questions</c>.</summary>
public sealed class CreateSurveyQuestionRequest
{
    [Required(ErrorMessage = "Nội dung câu hỏi là bắt buộc.")]
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "Nội dung câu hỏi phải từ 3 đến 1000 ký tự.")]
    public string NoiDungCauHoi { get; set; } = string.Empty;

    /// <summary>Ví dụ: "Đánh giá 1-5" hoặc "Tự luận". Không whitelist cứng.</summary>
    [Required(ErrorMessage = "Loại câu hỏi là bắt buộc.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Loại câu hỏi tối đa 100 ký tự.")]
    public string LoaiCauHoi { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Thứ tự phải lớn hơn 0.")]
    public int ThuTu { get; set; }

    public bool BatBuoc { get; set; }

    /// <summary>"Hoạt động" / "Ngừng hoạt động" — để trống thì mặc định "Hoạt động".</summary>
    [StringLength(50)]
    public string? TrangThai { get; set; }
}

/// <summary>Body của <c>PATCH /api/admin/surveys/{id}/questions/{maCauHoi}</c>.</summary>
public sealed class UpdateSurveyQuestionRequest
{
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "Nội dung câu hỏi phải từ 3 đến 1000 ký tự.")]
    public string? NoiDungCauHoi { get; set; }

    [StringLength(100, MinimumLength = 1)]
    public string? LoaiCauHoi { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Thứ tự phải lớn hơn 0.")]
    public int? ThuTu { get; set; }

    public bool? BatBuoc { get; set; }

    /// <summary>"Hoạt động" / "Ngừng hoạt động".</summary>
    [StringLength(50)]
    public string? TrangThai { get; set; }
}

// ============================================================================
// REQUEST — TRẢ LỜI
// ============================================================================

/// <summary>Một câu trả lời.</summary>
public sealed class SurveyAnswerItem
{
    [Range(1, int.MaxValue, ErrorMessage = "Câu hỏi không hợp lệ.")]
    public int MaCauHoi { get; set; }

    /// <summary>Dùng cho câu hỏi dạng đánh giá — database CHECK 0..5.</summary>
    [Range(typeof(decimal), "0", "5", ErrorMessage = "Giá trị đánh giá phải trong khoảng 0..5.")]
    public decimal? GiaTriDanhGia { get; set; }

    /// <summary>Dùng cho câu hỏi tự luận.</summary>
    [StringLength(2000, ErrorMessage = "Nội dung trả lời tối đa 2000 ký tự.")]
    public string? NoiDungTraLoi { get; set; }
}

/// <summary>Body của <c>POST /api/students/me/surveys/{id}/responses</c>.</summary>
public sealed class SubmitSurveyResponseRequest
{
    /// <summary>Điểm đánh giá chung — database CHECK 0..5.</summary>
    [Range(typeof(decimal), "0", "5", ErrorMessage = "Điểm đánh giá chung phải trong khoảng 0..5.")]
    public decimal? DiemDanhGiaChung { get; set; }

    [StringLength(1000, ErrorMessage = "Ghi chú tối đa 1000 ký tự.")]
    public string? GhiChu { get; set; }

    /// <summary>Danh sách câu trả lời. Có thể để trống nếu khảo sát không có câu hỏi bắt buộc.</summary>
    public List<SurveyAnswerItem> CauTraLois { get; set; } = [];
}

/// <summary>Tham số phân trang danh sách phản hồi.</summary>
public sealed class SurveyResponseListQuery : PagedQuery
{
    public string? MaHocVien { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách khảo sát.</summary>
public sealed class SurveyListItemDto
{
    public int MaKhaoSat { get; init; }

    public string TenKhaoSat { get; init; } = string.Empty;

    public string? MoTa { get; init; }

    public DateOnly NgayBatDau { get; init; }

    public DateOnly? NgayKetThuc { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public int SoCauHoi { get; init; }

    public int SoPhanHoi { get; init; }

    /// <summary>Đang nhận phản hồi hay không (trạng thái + khoảng ngày) — giá trị suy ra.</summary>
    public bool DangMo
        => Domain.Constants.SurveyStatuses.IsAcceptingResponses(
            TrangThai,
            NgayBatDau,
            NgayKetThuc,
            DateOnly.FromDateTime(DateTime.Today));
}

/// <summary>Chi tiết khảo sát kèm danh sách câu hỏi.</summary>
public sealed class SurveyDetailDto
{
    public int MaKhaoSat { get; init; }

    public string TenKhaoSat { get; init; } = string.Empty;

    public string? MoTa { get; init; }

    public DateOnly NgayBatDau { get; init; }

    public DateOnly? NgayKetThuc { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public int SoCauHoi { get; init; }

    public int SoPhanHoi { get; init; }

    public bool DangMo
        => Domain.Constants.SurveyStatuses.IsAcceptingResponses(
            TrangThai,
            NgayBatDau,
            NgayKetThuc,
            DateOnly.FromDateTime(DateTime.Today));

    public IReadOnlyList<SurveyQuestionDto> CauHois { get; init; } = [];
}

/// <summary>Một câu hỏi khảo sát.</summary>
public sealed class SurveyQuestionDto
{
    public int MaCauHoi { get; init; }

    public int MaKhaoSat { get; init; }

    public string NoiDungCauHoi { get; init; } = string.Empty;

    public string LoaiCauHoi { get; init; } = string.Empty;

    public int ThuTu { get; init; }

    public bool BatBuoc { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public int SoCauTraLoi { get; init; }
}

/// <summary>Một phản hồi trong danh sách (không kèm chi tiết câu trả lời).</summary>
public sealed class SurveyResponseListItemDto
{
    public int MaPhanHoiKhaoSat { get; init; }

    public int MaKhaoSat { get; init; }

    public string MaHocVien { get; init; } = string.Empty;

    public string HoTenHocVien { get; init; } = string.Empty;

    public DateTime ThoiGianGui { get; init; }

    public decimal? DiemDanhGiaChung { get; init; }

    public string? GhiChu { get; init; }

    public int SoCauTraLoi { get; init; }
}

/// <summary>Một câu trả lời chi tiết.</summary>
public sealed class SurveyAnswerDto
{
    public int MaCauTraLoi { get; init; }

    public int MaCauHoi { get; init; }

    public string NoiDungCauHoi { get; init; } = string.Empty;

    public string LoaiCauHoi { get; init; } = string.Empty;

    public decimal? GiaTriDanhGia { get; init; }

    public string? NoiDungTraLoi { get; init; }
}

/// <summary>Chi tiết một phản hồi.</summary>
public sealed class SurveyResponseDetailDto
{
    public int MaPhanHoiKhaoSat { get; init; }

    public int MaKhaoSat { get; init; }

    public string TenKhaoSat { get; init; } = string.Empty;

    public string MaHocVien { get; init; } = string.Empty;

    public string HoTenHocVien { get; init; } = string.Empty;

    public DateTime ThoiGianGui { get; init; }

    public decimal? DiemDanhGiaChung { get; init; }

    public string? GhiChu { get; init; }

    public IReadOnlyList<SurveyAnswerDto> CauTraLois { get; init; } = [];
}

/// <summary>Khảo sát nhìn từ phía học viên.</summary>
public sealed class MySurveyDto
{
    public int MaKhaoSat { get; init; }

    public string TenKhaoSat { get; init; } = string.Empty;

    public string? MoTa { get; init; }

    public DateOnly NgayBatDau { get; init; }

    public DateOnly? NgayKetThuc { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public int SoCauHoi { get; init; }

    public bool DangMo
        => Domain.Constants.SurveyStatuses.IsAcceptingResponses(
            TrangThai,
            NgayBatDau,
            NgayKetThuc,
            DateOnly.FromDateTime(DateTime.Today));

    /// <summary>Mã phản hồi đã gửi — <c>null</c> nếu chưa trả lời.</summary>
    public int? MaPhanHoiKhaoSat { get; init; }

    public DateTime? ThoiGianTraLoi { get; init; }

    public bool DaTraLoi => MaPhanHoiKhaoSat is not null;

    public IReadOnlyList<SurveyQuestionDto> CauHois { get; init; } = [];
}

// ============================================================================
// THỐNG KÊ
// ============================================================================

/// <summary>Một giá trị trong phân bố câu trả lời.</summary>
public sealed class AnswerDistributionDto
{
    public decimal GiaTri { get; init; }

    public int SoLuong { get; init; }
}

/// <summary>Thống kê của một câu hỏi.</summary>
public sealed class QuestionStatisticDto
{
    public int MaCauHoi { get; init; }

    public string NoiDungCauHoi { get; init; } = string.Empty;

    public string LoaiCauHoi { get; init; } = string.Empty;

    public int ThuTu { get; init; }

    public bool BatBuoc { get; init; }

    public int SoLuotTraLoi { get; init; }

    /// <summary>Trung bình <c>GiaTriDanhGia</c> — chỉ với câu hỏi có giá trị số.</summary>
    public decimal? GiaTriTrungBinh { get; init; }

    /// <summary>Phân bố theo <c>GiaTriDanhGia</c> (sắp tăng dần).</summary>
    public IReadOnlyList<AnswerDistributionDto> PhanBo { get; init; } = [];

    /// <summary>Nội dung tự luận (tối đa 50, KHÔNG kèm danh tính người trả lời).</summary>
    public IReadOnlyList<string> NoiDungTuLuan { get; init; } = [];
}

/// <summary>Thống kê tổng hợp của một khảo sát.</summary>
public sealed class SurveyStatisticsDto
{
    public int MaKhaoSat { get; init; }

    public string TenKhaoSat { get; init; } = string.Empty;

    public string TrangThai { get; init; } = string.Empty;

    public DateOnly NgayBatDau { get; init; }

    public DateOnly? NgayKetThuc { get; init; }

    public int TongSoCauHoi { get; init; }

    public int TongSoPhanHoi { get; init; }

    public decimal? DiemDanhGiaChungTrungBinh { get; init; }

    public IReadOnlyList<QuestionStatisticDto> CauHois { get; init; } = [];
}

// ============================================================================
// KẾT QUẢ GHI (hạ tầng → nghiệp vụ)
// ============================================================================

/// <summary>Dữ liệu tạo khảo sát (đã kiểm tra ở tầng Application).</summary>
public sealed record CreateSurveyData(
    string TenKhaoSat,
    string? MoTa,
    DateOnly NgayBatDau,
    DateOnly? NgayKetThuc,
    string TrangThai);

/// <summary>Dữ liệu cập nhật khảo sát.</summary>
public sealed record SurveyUpdateData(
    bool CapNhatTen,
    string? TenKhaoSat,
    bool CapNhatMoTa,
    string? MoTa,
    bool CapNhatNgayBatDau,
    DateOnly? NgayBatDau,
    bool CapNhatNgayKetThuc,
    DateOnly? NgayKetThuc);

/// <summary>Dữ liệu tạo câu hỏi.</summary>
public sealed record CreateQuestionData(
    string NoiDungCauHoi,
    string LoaiCauHoi,
    int ThuTu,
    bool BatBuoc,
    string TrangThai);

/// <summary>Dữ liệu cập nhật câu hỏi.</summary>
public sealed record QuestionUpdateData(
    bool CapNhatNoiDung,
    string? NoiDungCauHoi,
    bool CapNhatLoai,
    string? LoaiCauHoi,
    bool CapNhatThuTu,
    int? ThuTu,
    bool CapNhatBatBuoc,
    bool? BatBuoc,
    string? TrangThai);

/// <summary>Một câu trả lời đã được chuẩn hóa.</summary>
public sealed record SurveyAnswerData(
    int MaCauHoi,
    decimal? GiaTriDanhGia,
    string? NoiDungTraLoi);

/// <summary>Dữ liệu gửi phản hồi.</summary>
public sealed record SubmitResponseData(
    decimal? DiemDanhGiaChung,
    string? GhiChu,
    IReadOnlyList<SurveyAnswerData> CauTraLois);

/// <summary>Kết quả ghi phản hồi.</summary>
public enum SurveySubmitOutcome
{
    Success = 0,
    SurveyNotFound = 1,
    NotAccepting = 2,
    AlreadyResponded = 3,
    Conflict = 4
}

/// <summary>
/// <paramref name="MaPhanHoiKhaoSat"/> là phản hồi MỚI khi <c>Success</c>,
/// là phản hồi ĐÃ CÓ khi <c>AlreadyResponded</c>.
/// </summary>
public sealed record SurveySubmitResult(
    SurveySubmitOutcome Outcome,
    int MaPhanHoiKhaoSat,
    string? LyDo);
