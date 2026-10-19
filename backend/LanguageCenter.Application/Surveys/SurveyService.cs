using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Surveys.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Surveys;

/// <inheritdoc cref="ISurveyService"/>
/// <remarks>
/// <para>
/// "Đang nhận phản hồi" = <c>TrangThai = 'Hoạt động'</c> VÀ hôm nay nằm trong
/// <c>[NgayBatDau, NgayKetThuc]</c> — xem <see cref="SurveyStatuses.IsAcceptingResponses"/>.
/// </para>
/// <para>
/// Database không có bảng lựa chọn (options) cho câu hỏi, nên câu trả lời chỉ có thể là
/// <c>GiaTriDanhGia</c> (0..5) hoặc <c>NoiDungTraLoi</c>. Không thể thống kê "phân bố theo lựa chọn"
/// cho câu hỏi tự luận.
/// </para>
/// </remarks>
public sealed class SurveyService : ISurveyService
{
    private const int EssayPreviewLimit = 50;

    private readonly ISurveyStore _surveyStore;

    public SurveyService(ISurveyStore surveyStore)
    {
        _surveyStore = surveyStore;
    }

    // ========================================================================
    // QUẢN TRỊ — KHẢO SÁT
    // ========================================================================

    public async Task<OperationResult<PagedResult<SurveyListItemDto>>> ListAsync(
        SurveyListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai)
            && !SurveyStatuses.IsValid(query.TrangThai))
        {
            return OperationResult<PagedResult<SurveyListItemDto>>.BadRequest(
                "Trạng thái khảo sát không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", SurveyStatuses.All)}."]);
        }

        if (query.TuNgay is { } tu && query.DenNgay is { } den && den < tu)
        {
            return OperationResult<PagedResult<SurveyListItemDto>>.BadRequest(
                "Khoảng ngày không hợp lệ.",
                ["Đến ngày phải lớn hơn hoặc bằng từ ngày."]);
        }

        var page = await _surveyStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<SurveyListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<SurveyDetailDto>> GetDetailAsync(
        int maKhaoSat,
        CancellationToken cancellationToken)
    {
        var detail = await _surveyStore.GetDetailAsync(maKhaoSat, cancellationToken);

        return detail is null
            ? OperationResult<SurveyDetailDto>.NotFound($"Không tìm thấy khảo sát có id = {maKhaoSat}.")
            : OperationResult<SurveyDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<SurveyDetailDto>> CreateAsync(
        CreateSurveyRequest request,
        CancellationToken cancellationToken)
    {
        var ten = TextValue.Required(request.TenKhaoSat);
        var moTa = TextValue.Optional(request.MoTa);
        var trangThai = TextValue.Required(request.TrangThai) ?? SurveyStatuses.HoatDong;

        var errors = new List<string>();

        if (ten is null)
        {
            errors.Add("Tên khảo sát là bắt buộc.");
        }
        else if (ten.Length is < 3 or > 255)
        {
            errors.Add("Tên khảo sát phải từ 3 đến 255 ký tự.");
        }

        if (moTa is not null && moTa.Length > 1000)
        {
            errors.Add("Mô tả tối đa 1000 ký tự.");
        }

        if (!SurveyStatuses.IsValid(trangThai))
        {
            errors.Add($"Trạng thái chỉ chấp nhận: {string.Join(", ", SurveyStatuses.All)}.");
        }

        errors.AddRange(ValidateDateRange(request.NgayBatDau, request.NgayKetThuc));

        if (errors.Count > 0)
        {
            return OperationResult<SurveyDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        var data = new CreateSurveyData(
            ten!,
            moTa,
            request.NgayBatDau,
            request.NgayKetThuc,
            trangThai);

        var result = await _surveyStore.CreateAsync(data, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<SurveyDetailDto>.Conflict("Không thể tạo khảo sát.");
        }

        var created = await _surveyStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<SurveyDetailDto>.Created(created!, "Tạo khảo sát thành công.");
    }

    public async Task<OperationResult<SurveyDetailDto>> UpdateAsync(
        int maKhaoSat,
        UpdateSurveyRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _surveyStore.GetDetailAsync(maKhaoSat, cancellationToken);

        if (current is null)
        {
            return OperationResult<SurveyDetailDto>.NotFound($"Không tìm thấy khảo sát có id = {maKhaoSat}.");
        }

        var coThayDoi = request.TenKhaoSat is not null
                        || request.MoTa is not null
                        || request.NgayBatDau is not null
                        || request.NgayKetThuc is not null
                        || request.XoaNgayKetThuc;

        if (!coThayDoi)
        {
            return OperationResult<SurveyDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        if (request.XoaNgayKetThuc && request.NgayKetThuc is not null)
        {
            return OperationResult<SurveyDetailDto>.BadRequest(
                "Tham số mâu thuẫn.",
                ["Không thể vừa đặt 'ngayKetThuc' vừa bật 'xoaNgayKetThuc'."]);
        }

        var ten = request.TenKhaoSat is null ? current.TenKhaoSat : TextValue.Required(request.TenKhaoSat);
        var ngayBatDau = request.NgayBatDau ?? current.NgayBatDau;
        var ngayKetThuc = request.XoaNgayKetThuc ? null : (request.NgayKetThuc ?? current.NgayKetThuc);

        var errors = new List<string>();

        if (ten is null || ten.Length is < 3 or > 255)
        {
            errors.Add("Tên khảo sát phải từ 3 đến 255 ký tự.");
        }

        errors.AddRange(ValidateDateRange(ngayBatDau, ngayKetThuc));

        if (errors.Count > 0)
        {
            return OperationResult<SurveyDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        var changes = new SurveyUpdateData(
            CapNhatTen: request.TenKhaoSat is not null,
            TenKhaoSat: ten,
            CapNhatMoTa: request.MoTa is not null,
            MoTa: TextValue.Optional(request.MoTa),
            CapNhatNgayBatDau: request.NgayBatDau is not null,
            NgayBatDau: request.NgayBatDau,
            CapNhatNgayKetThuc: request.XoaNgayKetThuc || request.NgayKetThuc is not null,
            NgayKetThuc: ngayKetThuc);

        var outcome = await _surveyStore.UpdateAsync(maKhaoSat, changes, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<SurveyDetailDto>.NotFound($"Không tìm thấy khảo sát có id = {maKhaoSat}.")
                : OperationResult<SurveyDetailDto>.Conflict("Không thể cập nhật khảo sát.");
        }

        var updated = await _surveyStore.GetDetailAsync(maKhaoSat, cancellationToken);

        return OperationResult<SurveyDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<SurveyDetailDto>> UpdateStatusAsync(
        int maKhaoSat,
        UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = TextValue.Required(request.TrangThai);

        if (trangThai is null || !SurveyStatuses.IsValid(trangThai))
        {
            return OperationResult<SurveyDetailDto>.BadRequest(
                "Trạng thái khảo sát không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", SurveyStatuses.All)}."]);
        }

        if (!await _surveyStore.ExistsAsync(maKhaoSat, cancellationToken))
        {
            return OperationResult<SurveyDetailDto>.NotFound($"Không tìm thấy khảo sát có id = {maKhaoSat}.");
        }

        var outcome = await _surveyStore.UpdateStatusAsync(maKhaoSat, trangThai, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<SurveyDetailDto>.Conflict("Không thể cập nhật trạng thái khảo sát.");
        }

        var updated = await _surveyStore.GetDetailAsync(maKhaoSat, cancellationToken);

        return OperationResult<SurveyDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<bool>> DeleteAsync(
        int maKhaoSat,
        CancellationToken cancellationToken)
    {
        if (!await _surveyStore.ExistsAsync(maKhaoSat, cancellationToken))
        {
            return OperationResult<bool>.NotFound($"Không tìm thấy khảo sát có id = {maKhaoSat}.");
        }

        var responses = await _surveyStore.CountResponsesAsync(maKhaoSat, cancellationToken);

        if (responses > 0)
        {
            return OperationResult<bool>.Conflict(
                $"Không thể xóa khảo sát vì đã có {responses} phản hồi. " +
                "Hãy chuyển trạng thái sang 'Đã kết thúc' thay vì xóa.");
        }

        var outcome = await _surveyStore.DeleteAsync(maKhaoSat, cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.NoContent(),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound(
                $"Không tìm thấy khảo sát có id = {maKhaoSat}."),
            _ => OperationResult<bool>.Conflict("Không thể xóa khảo sát do ràng buộc dữ liệu.")
        };
    }

    // ========================================================================
    // QUẢN TRỊ — CÂU HỎI
    // ========================================================================

    public async Task<OperationResult<IReadOnlyList<SurveyQuestionDto>>> ListQuestionsAsync(
        int maKhaoSat,
        CancellationToken cancellationToken)
    {
        if (!await _surveyStore.ExistsAsync(maKhaoSat, cancellationToken))
        {
            return OperationResult<IReadOnlyList<SurveyQuestionDto>>.NotFound(
                $"Không tìm thấy khảo sát có id = {maKhaoSat}.");
        }

        var questions = await _surveyStore.ListQuestionsAsync(maKhaoSat, cancellationToken);

        return OperationResult<IReadOnlyList<SurveyQuestionDto>>.Ok(questions);
    }

    public async Task<OperationResult<SurveyQuestionDto>> CreateQuestionAsync(
        int maKhaoSat,
        CreateSurveyQuestionRequest request,
        CancellationToken cancellationToken)
    {
        if (!await _surveyStore.ExistsAsync(maKhaoSat, cancellationToken))
        {
            return OperationResult<SurveyQuestionDto>.NotFound(
                $"Không tìm thấy khảo sát có id = {maKhaoSat}.");
        }

        var noiDung = TextValue.Required(request.NoiDungCauHoi);
        var loai = TextValue.Required(request.LoaiCauHoi);
        var trangThai = TextValue.Required(request.TrangThai) ?? ActivityStatuses.HoatDong;

        var errors = new List<string>();

        if (noiDung is null || noiDung.Length is < 3 or > 1000)
        {
            errors.Add("Nội dung câu hỏi phải từ 3 đến 1000 ký tự.");
        }

        if (loai is null || loai.Length > 100)
        {
            errors.Add("Loại câu hỏi bắt buộc và tối đa 100 ký tự.");
        }

        if (request.ThuTu <= 0)
        {
            errors.Add("Thứ tự phải lớn hơn 0.");
        }

        if (!ActivityStatuses.IsValid(trangThai))
        {
            errors.Add($"Trạng thái chỉ chấp nhận: {string.Join(", ", ActivityStatuses.All)}.");
        }

        if (errors.Count > 0)
        {
            return OperationResult<SurveyQuestionDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (await _surveyStore.QuestionOrderTakenAsync(
                maKhaoSat, request.ThuTu, exceptId: null, cancellationToken))
        {
            return OperationResult<SurveyQuestionDto>.Conflict(
                $"Khảo sát đã có câu hỏi ở thứ tự {request.ThuTu}.");
        }

        var data = new CreateQuestionData(noiDung!, loai!, request.ThuTu, request.BatBuoc, trangThai);

        var result = await _surveyStore.CreateQuestionAsync(maKhaoSat, data, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<SurveyQuestionDto>.Conflict("Không thể tạo câu hỏi.");
        }

        var created = await _surveyStore.GetQuestionAsync(maKhaoSat, result.Id, cancellationToken);

        return OperationResult<SurveyQuestionDto>.Created(created!, "Tạo câu hỏi thành công.");
    }

    public async Task<OperationResult<SurveyQuestionDto>> UpdateQuestionAsync(
        int maKhaoSat,
        int maCauHoi,
        UpdateSurveyQuestionRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _surveyStore.GetQuestionAsync(maKhaoSat, maCauHoi, cancellationToken);

        if (current is null)
        {
            return OperationResult<SurveyQuestionDto>.NotFound(
                $"Không tìm thấy câu hỏi có id = {maCauHoi} trong khảo sát {maKhaoSat}.");
        }

        if (request.NoiDungCauHoi is null
            && request.LoaiCauHoi is null
            && request.ThuTu is null
            && request.BatBuoc is null
            && request.TrangThai is null)
        {
            return OperationResult<SurveyQuestionDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        var noiDung = request.NoiDungCauHoi is null
            ? current.NoiDungCauHoi
            : TextValue.Required(request.NoiDungCauHoi);

        var loai = request.LoaiCauHoi is null ? current.LoaiCauHoi : TextValue.Required(request.LoaiCauHoi);
        var thuTu = request.ThuTu ?? current.ThuTu;
        var trangThai = TextValue.Required(request.TrangThai);

        var errors = new List<string>();

        if (noiDung is null || noiDung.Length is < 3 or > 1000)
        {
            errors.Add("Nội dung câu hỏi phải từ 3 đến 1000 ký tự.");
        }

        if (loai is null || loai.Length > 100)
        {
            errors.Add("Loại câu hỏi bắt buộc và tối đa 100 ký tự.");
        }

        if (thuTu <= 0)
        {
            errors.Add("Thứ tự phải lớn hơn 0.");
        }

        if (trangThai is not null && !ActivityStatuses.IsValid(trangThai))
        {
            errors.Add($"Trạng thái chỉ chấp nhận: {string.Join(", ", ActivityStatuses.All)}.");
        }

        if (errors.Count > 0)
        {
            return OperationResult<SurveyQuestionDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (request.ThuTu is not null
            && await _surveyStore.QuestionOrderTakenAsync(maKhaoSat, thuTu, maCauHoi, cancellationToken))
        {
            return OperationResult<SurveyQuestionDto>.Conflict(
                $"Khảo sát đã có câu hỏi ở thứ tự {thuTu}.");
        }

        var changes = new QuestionUpdateData(
            CapNhatNoiDung: request.NoiDungCauHoi is not null,
            NoiDungCauHoi: noiDung,
            CapNhatLoai: request.LoaiCauHoi is not null,
            LoaiCauHoi: loai,
            CapNhatThuTu: request.ThuTu is not null,
            ThuTu: request.ThuTu,
            CapNhatBatBuoc: request.BatBuoc is not null,
            BatBuoc: request.BatBuoc,
            TrangThai: trangThai);

        var outcome = await _surveyStore.UpdateQuestionAsync(maCauHoi, changes, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<SurveyQuestionDto>.NotFound($"Không tìm thấy câu hỏi có id = {maCauHoi}.")
                : OperationResult<SurveyQuestionDto>.Conflict("Không thể cập nhật câu hỏi.");
        }

        var updated = await _surveyStore.GetQuestionAsync(maKhaoSat, maCauHoi, cancellationToken);

        return OperationResult<SurveyQuestionDto>.Ok(updated!);
    }

    public async Task<OperationResult<bool>> DeleteQuestionAsync(
        int maKhaoSat,
        int maCauHoi,
        CancellationToken cancellationToken)
    {
        var current = await _surveyStore.GetQuestionAsync(maKhaoSat, maCauHoi, cancellationToken);

        if (current is null)
        {
            return OperationResult<bool>.NotFound(
                $"Không tìm thấy câu hỏi có id = {maCauHoi} trong khảo sát {maKhaoSat}.");
        }

        var answers = await _surveyStore.CountAnswersAsync(maCauHoi, cancellationToken);

        if (answers > 0)
        {
            return OperationResult<bool>.Conflict(
                $"Không thể xóa câu hỏi vì đã có {answers} câu trả lời. " +
                "Hãy chuyển trạng thái câu hỏi sang 'Ngừng hoạt động'.");
        }

        var outcome = await _surveyStore.DeleteQuestionAsync(maCauHoi, cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.NoContent(),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound(
                $"Không tìm thấy câu hỏi có id = {maCauHoi}."),
            _ => OperationResult<bool>.Conflict("Không thể xóa câu hỏi do ràng buộc dữ liệu.")
        };
    }

    // ========================================================================
    // QUẢN TRỊ — PHẢN HỒI & THỐNG KÊ
    // ========================================================================

    public async Task<OperationResult<PagedResult<SurveyResponseListItemDto>>> ListResponsesAsync(
        int maKhaoSat,
        SurveyResponseListQuery query,
        CancellationToken cancellationToken)
    {
        if (!await _surveyStore.ExistsAsync(maKhaoSat, cancellationToken))
        {
            return OperationResult<PagedResult<SurveyResponseListItemDto>>.NotFound(
                $"Không tìm thấy khảo sát có id = {maKhaoSat}.");
        }

        var page = await _surveyStore.ListResponsesAsync(maKhaoSat, query, cancellationToken);

        return OperationResult<PagedResult<SurveyResponseListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<SurveyStatisticsDto>> GetStatisticsAsync(
        int maKhaoSat,
        CancellationToken cancellationToken)
    {
        var stats = await _surveyStore.GetStatisticsAsync(maKhaoSat, cancellationToken);

        return stats is null
            ? OperationResult<SurveyStatisticsDto>.NotFound($"Không tìm thấy khảo sát có id = {maKhaoSat}.")
            : OperationResult<SurveyStatisticsDto>.Ok(stats);
    }

    // ========================================================================
    // HỌC VIÊN
    // ========================================================================

    public async Task<OperationResult<IReadOnlyList<MySurveyDto>>> ListForStudentAsync(
        SurveyStudentActor actor,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(actor.MaHocVien))
        {
            return OperationResult<IReadOnlyList<MySurveyDto>>.Forbidden(
                "Không xác định được học viên từ token.");
        }

        var surveys = await _surveyStore.ListForStudentAsync(actor.MaHocVien, cancellationToken);

        return OperationResult<IReadOnlyList<MySurveyDto>>.Ok(surveys);
    }

    public async Task<OperationResult<MySurveyDto>> GetForStudentAsync(
        int maKhaoSat,
        SurveyStudentActor actor,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(actor.MaHocVien))
        {
            return OperationResult<MySurveyDto>.Forbidden("Không xác định được học viên từ token.");
        }

        var survey = await _surveyStore.GetForStudentAsync(maKhaoSat, actor.MaHocVien, cancellationToken);

        return survey is null
            ? OperationResult<MySurveyDto>.NotFound($"Không tìm thấy khảo sát có id = {maKhaoSat}.")
            : OperationResult<MySurveyDto>.Ok(survey);
    }

    public async Task<OperationResult<SurveyResponseDetailDto>> GetMyResponseAsync(
        int maKhaoSat,
        SurveyStudentActor actor,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(actor.MaHocVien))
        {
            return OperationResult<SurveyResponseDetailDto>.Forbidden(
                "Không xác định được học viên từ token.");
        }

        var response = await _surveyStore.GetResponseOfStudentAsync(
            maKhaoSat, actor.MaHocVien, cancellationToken);

        return response is null
            ? OperationResult<SurveyResponseDetailDto>.NotFound(
                $"Bạn chưa gửi phản hồi cho khảo sát {maKhaoSat}.")
            : OperationResult<SurveyResponseDetailDto>.Ok(response);
    }

    public async Task<OperationResult<SurveyResponseDetailDto>> SubmitAsync(
        int maKhaoSat,
        SubmitSurveyResponseRequest request,
        SurveyStudentActor actor,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(actor.MaHocVien))
        {
            return OperationResult<SurveyResponseDetailDto>.Forbidden(
                "Không xác định được học viên từ token.");
        }

        var survey = await _surveyStore.GetDetailAsync(maKhaoSat, cancellationToken);

        if (survey is null)
        {
            return OperationResult<SurveyResponseDetailDto>.NotFound(
                $"Không tìm thấy khảo sát có id = {maKhaoSat}.");
        }

        var lyDoKhongNhan = SurveyStatuses.DescribeBlockReason(
            survey.TrangThai,
            survey.NgayBatDau,
            survey.NgayKetThuc,
            DateOnly.FromDateTime(DateTime.Today));

        if (lyDoKhongNhan is not null)
        {
            return OperationResult<SurveyResponseDetailDto>.BadRequest(
                "Khảo sát không nhận phản hồi.",
                [lyDoKhongNhan]);
        }

        var daTraLoi = await _surveyStore.GetResponseOfStudentAsync(
            maKhaoSat, actor.MaHocVien, cancellationToken);

        if (daTraLoi is not null)
        {
            return OperationResult<SurveyResponseDetailDto>.Conflict(
                $"Bạn đã gửi phản hồi cho khảo sát này (mã phản hồi {daTraLoi.MaPhanHoiKhaoSat}). " +
                "Mỗi học viên chỉ được trả lời một lần.");
        }

        if (request.DiemDanhGiaChung is { } diem && (diem < 0m || diem > 5m))
        {
            return OperationResult<SurveyResponseDetailDto>.BadRequest(
                "Điểm đánh giá chung không hợp lệ.",
                ["Điểm đánh giá chung phải trong khoảng 0..5."]);
        }

        // -------- Validate toàn bộ câu trả lời TRƯỚC khi ghi --------
        var errors = new List<string>();
        var cauHoiTheoId = survey.CauHois.ToDictionary(x => x.MaCauHoi);
        var daGap = new HashSet<int>();
        var cauTraLois = new List<SurveyAnswerData>(request.CauTraLois.Count);

        foreach (var item in request.CauTraLois)
        {
            if (!daGap.Add(item.MaCauHoi))
            {
                errors.Add($"Câu hỏi {item.MaCauHoi} xuất hiện nhiều lần trong yêu cầu.");
                continue;
            }

            if (!cauHoiTheoId.TryGetValue(item.MaCauHoi, out var cauHoi))
            {
                errors.Add($"Câu hỏi {item.MaCauHoi} không thuộc khảo sát này.");
                continue;
            }

            if (!ActivityStatuses.IsValid(cauHoi.TrangThai)
                || !string.Equals(cauHoi.TrangThai, ActivityStatuses.HoatDong, StringComparison.Ordinal))
            {
                errors.Add($"Câu hỏi '{cauHoi.NoiDungCauHoi}' đã ngừng hoạt động, không nhận câu trả lời.");
                continue;
            }

            var loiTraLoi = ValidateAnswer(cauHoi, item);

            if (loiTraLoi is not null)
            {
                errors.Add(loiTraLoi);
                continue;
            }

            cauTraLois.Add(new SurveyAnswerData(
                item.MaCauHoi,
                item.GiaTriDanhGia,
                TextValue.Optional(item.NoiDungTraLoi)));
        }

        // Câu hỏi BẮT BUỘC còn hoạt động phải được trả lời.
        foreach (var cauHoi in survey.CauHois.Where(
                     x => x.BatBuoc
                          && string.Equals(x.TrangThai, ActivityStatuses.HoatDong, StringComparison.Ordinal)))
        {
            if (!daGap.Contains(cauHoi.MaCauHoi))
            {
                errors.Add($"Câu hỏi bắt buộc chưa được trả lời: '{cauHoi.NoiDungCauHoi}'.");
            }
        }

        if (errors.Count > 0)
        {
            return OperationResult<SurveyResponseDetailDto>.BadRequest(
                "Dữ liệu câu trả lời không hợp lệ.",
                errors);
        }

        var data = new SubmitResponseData(
            request.DiemDanhGiaChung,
            TextValue.Optional(request.GhiChu),
            cauTraLois);

        var result = await _surveyStore.SubmitAsync(maKhaoSat, actor.MaHocVien, data, cancellationToken);

        switch (result.Outcome)
        {
            case SurveySubmitOutcome.SurveyNotFound:
                return OperationResult<SurveyResponseDetailDto>.NotFound(
                    $"Không tìm thấy khảo sát có id = {maKhaoSat}.");

            case SurveySubmitOutcome.NotAccepting:
                return OperationResult<SurveyResponseDetailDto>.BadRequest(
                    "Khảo sát không nhận phản hồi.",
                    [result.LyDo ?? "Khảo sát đã đóng."]);

            case SurveySubmitOutcome.AlreadyResponded:
                return OperationResult<SurveyResponseDetailDto>.Conflict(
                    "Bạn đã gửi phản hồi cho khảo sát này.");

            case SurveySubmitOutcome.Conflict:
                return OperationResult<SurveyResponseDetailDto>.Conflict(
                    "Không thể ghi phản hồi do xung đột dữ liệu.");

            default:
                var created = await _surveyStore.GetResponseOfStudentAsync(
                    maKhaoSat, actor.MaHocVien, cancellationToken);

                return created is null
                    ? OperationResult<SurveyResponseDetailDto>.Conflict(
                        "Đã ghi phản hồi nhưng không đọc lại được.")
                    : OperationResult<SurveyResponseDetailDto>.Created(created, "Gửi phản hồi thành công.");
        }
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static IEnumerable<string> ValidateDateRange(DateOnly ngayBatDau, DateOnly? ngayKetThuc)
    {
        if (ngayBatDau == default)
        {
            yield return "Ngày bắt đầu là bắt buộc.";
        }

        if (ngayKetThuc is { } den && ngayBatDau != default && den < ngayBatDau)
        {
            yield return "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.";
        }
    }

    /// <summary>Áp quy tắc theo <c>LoaiCauHoi</c> và hai dạng dữ liệu mà schema lưu được.</summary>
    private static string? ValidateAnswer(SurveyQuestionDto cauHoi, SurveyAnswerItem item)
    {
        var coSo = item.GiaTriDanhGia is not null;
        var coChu = !string.IsNullOrWhiteSpace(item.NoiDungTraLoi);
        var nhan = $"Câu hỏi #{cauHoi.ThuTu}";

        if (coSo && (item.GiaTriDanhGia < 0m || item.GiaTriDanhGia > 5m))
        {
            return $"{nhan}: giá trị đánh giá phải trong khoảng 0..5.";
        }

        if (item.NoiDungTraLoi is { Length: > 2000 })
        {
            return $"{nhan}: nội dung trả lời tối đa 2000 ký tự.";
        }

        if (SurveyQuestionTypes.IsRating(cauHoi.LoaiCauHoi))
        {
            if (!coSo)
            {
                return $"{nhan} là dạng đánh giá, cần gửi 'giaTriDanhGia'.";
            }

            if (coChu)
            {
                return $"{nhan} là dạng đánh giá, không nhận 'noiDungTraLoi'.";
            }

            return null;
        }

        if (SurveyQuestionTypes.IsEssay(cauHoi.LoaiCauHoi))
        {
            if (!coChu)
            {
                return $"{nhan} là dạng tự luận, cần gửi 'noiDungTraLoi'.";
            }

            if (coSo)
            {
                return $"{nhan} là dạng tự luận, không nhận 'giaTriDanhGia'.";
            }

            return null;
        }

        // Loại câu hỏi tùy biến: chấp nhận một trong hai dạng.
        return coSo || coChu ? null : $"{nhan} chưa có câu trả lời.";
    }

    /// <summary>Giới hạn số nội dung tự luận trả về trong thống kê.</summary>
    public const int MaxEssaySamples = EssayPreviewLimit;
}
