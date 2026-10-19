# ============================================================
# PHASE 7 - Thong bao / KhaoSat / Bao cao + regression Phase 1..6
# ------------------------------------------------------------
# YEU CAU: API dang chay tai http://localhost:5204
#
# AN TOAN DU LIEU:
#   - Chi TAO: 1 khao sat test, 2 cau hoi, 1 phan hoi, vai thong bao test.
#   - finally: xoa phan hoi -> cau tra loi (cascade) -> khao sat (cau hoi cascade),
#     va xoa cac thong bao test. KHONG dung vao du lieu seed.
#
# ENCODING: PowerShell 5.1 doc .ps1 theo ANSI => khong viet literal tieng Viet.
# ============================================================
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName System.Data

$BaseUrl          = 'http://localhost:5204'
$DevPassword      = 'LanguageCenter@2026'
$ConnectionString = 'Server=localhost\SQLEXPRESS;Database=QuanLyTrungTamNgoaiNgu;Integrated Security=True'

$reportPath = Join-Path $PSScriptRoot 'phase7-test-report.txt'
$log     = [System.Collections.Generic.List[string]]::new()
$script:passed = 0
$script:failed = 0
$script:warns  = 0

function VN {
    param([object[]]$Parts)
    $sb = New-Object System.Text.StringBuilder
    foreach ($p in $Parts) {
        if ($p -is [int]) { [void]$sb.Append([char]$p) } else { [void]$sb.Append([string]$p) }
    }
    return $sb.ToString()
}

$S_HoatDong      = VN @('Ho', 0x1EA1, 't ', 0x0110, 0x1ED9, 'ng')
$S_DaKetThuc     = VN @(0x0110, 0x00E3, ' k', 0x1EBF, 't th', 0x00FA, 'c')
$S_DaThuHoi      = VN @(0x0110, 0x00E3, ' thu h', 0x1ED3, 'i')
$S_DanhGia       = VN @(0x0110, 0x00E1, 'nh gi', 0x00E1, ' 1-5')
$S_TuLuan        = VN @('T', 0x1EF1, ' lu', 0x1EAD, 'n')
$S_DangPhanCong  = VN @(0x0110, 'ang ph', 0x00E2, 'n c', 0x00F4, 'ng')
$S_ThuTien       = VN @('Thu ti', 0x1EC1, 'n')
$S_TienMat       = VN @('Ti', 0x1EC1, 'n m', 0x1EB7, 't')

function Write-Log { param([string]$Text) $log.Add($Text); Write-Output $Text }

function Assert {
    param([string]$Name, [bool]$Condition, [string]$Detail = '')
    if ($Condition) { $script:passed++; Write-Log ("  [PASS] {0}" -f $Name) }
    else { $script:failed++; Write-Log ("  [FAIL] {0}  -> {1}" -f $Name, $Detail) }
}

function Warn { param([string]$Name, [string]$Detail) $script:warns++; Write-Log ("  [WARN] {0}  -> {1}" -f $Name, $Detail) }

function Invoke-ApiOnce {
    param([string]$Method, [string]$Path, [object]$Body, [string]$Token)
    $uri = "$BaseUrl$Path"
    $headers = @{}
    if ($Token) { $headers['Authorization'] = "Bearer $Token" }
    try {
        $params = @{ Uri = $uri; Method = $Method; Headers = $headers; UseBasicParsing = $true; ErrorAction = 'Stop'; DisableKeepAlive = $true }
        if ($null -ne $Body) {
            $params['ContentType'] = 'application/json; charset=utf-8'
            $params['Body'] = [System.Text.Encoding]::UTF8.GetBytes(($Body | ConvertTo-Json -Depth 6 -Compress))
        }
        $response = Invoke-WebRequest @params
        $reader = New-Object System.IO.StreamReader($response.RawContentStream, [System.Text.Encoding]::UTF8)
        return @{ Status = [int]$response.StatusCode; Text = $reader.ReadToEnd() }
    }
    catch {
        $response = $_.Exception.Response
        if ($null -eq $response) { return @{ Status = -1; Text = $_.Exception.Message } }
        $reader = New-Object System.IO.StreamReader($response.GetResponseStream(), [System.Text.Encoding]::UTF8)
        return @{ Status = [int]$response.StatusCode; Text = $reader.ReadToEnd() }
    }
}

# PowerShell 5.1 thinh thoang dong keep-alive connection -> thu lai mot lan.
function Invoke-Api {
    param([string]$Method, [string]$Path, [object]$Body, [string]$Token)
    $result = Invoke-ApiOnce -Method $Method -Path $Path -Body $Body -Token $Token
    if ($result.Status -eq -1) {
        Start-Sleep -Milliseconds 300
        $result = Invoke-ApiOnce -Method $Method -Path $Path -Body $Body -Token $Token
    }
    return $result
}

function Get-UrlStatus {
    param([string]$Url)
    try {
        $req = [System.Net.WebRequest]::Create($Url)
        $req.Method = 'GET'
        $req.Timeout = 15000
        $resp = $req.GetResponse()
        $status = [int]$resp.StatusCode
        $resp.Close()
        return $status
    }
    catch [System.Net.WebException] {
        if ($null -ne $_.Exception.Response) { return [int]$_.Exception.Response.StatusCode }
        return -1
    }
}

function From-Json-Safe {
    param([string]$Text)
    if ([string]::IsNullOrWhiteSpace($Text)) { return $null }
    try { return $Text | ConvertFrom-Json } catch { return $null }
}

function Invoke-Query {
    param([string]$Sql, [hashtable]$Params = @{})
    $conn = New-Object System.Data.SqlClient.SqlConnection($ConnectionString)
    $conn.Open()
    try {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $Sql
        $cmd.CommandTimeout = 60
        foreach ($key in $Params.Keys) {
            [void]$cmd.Parameters.AddWithValue("@$key", $Params[$key])
        }
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($cmd)
        $table = New-Object System.Data.DataTable
        [void]$adapter.Fill($table)
        return ,$table
    }
    finally { $conn.Close() }
}

function Invoke-Scalar {
    param([string]$Sql, [hashtable]$Params = @{})
    $table = Invoke-Query -Sql $Sql -Params $Params
    if ($table.Rows.Count -eq 0) { return $null }
    return $table.Rows[0][0]
}

function Login {
    param([string]$User)
    $r = Invoke-Api -Method POST -Path '/api/auth/login' -Body @{ username = $User; password = $DevPassword }
    return (From-Json-Safe $r.Text).accessToken
}

Write-Log '============================================================'
Write-Log ' PHASE 7 - TEST SUITE (thong bao / khao sat / bao cao)'
Write-Log '============================================================'

$createdThongBaoIds = [System.Collections.Generic.List[int]]::new()
$createdKhaoSatIds  = [System.Collections.Generic.List[int]]::new()

$health = Invoke-Api -Method GET -Path '/health'
Write-Log ("Health check: HTTP {0}" -f $health.Status)
if ($health.Status -ne 200) {
    Write-Log 'API chua san sang. Ket thuc.'
    $log | Out-File -FilePath $reportPath -Encoding utf8
    exit 1
}

$adminToken = Login 'ADMIN001'
$nvhvToken  = Login 'NVHV001'
$gvToken    = Login 'GV001'
$hvToken    = Login 'HV001'
$hv2Token   = Login 'HV002'

Assert 'LOGIN ADMIN001' ([bool]$adminToken) 'khong lay duoc token'
Assert 'LOGIN NVHV001' ([bool]$nvhvToken) 'khong lay duoc token'
Assert 'LOGIN GV001' ([bool]$gvToken) 'khong lay duoc token'
Assert 'LOGIN HV001' ([bool]$hvToken) 'khong lay duoc token'
Assert 'LOGIN HV002' ([bool]$hv2Token) 'khong lay duoc token'

$hvAccountId  = [int](Invoke-Scalar -Sql "SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'HV001'")
$hv2AccountId = [int](Invoke-Scalar -Sql "SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'HV002'")

try {
    # ========================================================
    # T01 - Thong bao: pham vi hien thi
    # ========================================================
    Write-Log ''
    Write-Log '--- T01: thong bao - pham vi hien thi ---'

    $r = Invoke-Api -Method GET -Path '/api/notifications' -Token $hvToken
    $myNoti = From-Json-Safe $r.Text
    Assert 'T01.1 HV001 GET /api/notifications -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $saiPhamVi = $false
    if ($null -ne $myNoti.items) {
        foreach ($n in $myNoti.items) {
            if ($n.maTaiKhoanNhan -ne $null -and [int]$n.maTaiKhoanNhan -ne $hvAccountId) { $saiPhamVi = $true }
        }
    }
    Assert 'T01.2 khong lo thong bao gui rieng cho nguoi khac' (-not $saiPhamVi) 'co thong bao cua tai khoan khac'

    $notiCuaHv2 = Invoke-Scalar -Sql 'SELECT TOP 1 MaThongBao FROM ThongBao WHERE MaTaiKhoanNhan = @id ORDER BY MaThongBao DESC' -Params @{ id = $hv2AccountId }
    if ($null -ne $notiCuaHv2) {
        $r = Invoke-Api -Method GET -Path ("/api/notifications/{0}" -f [int]$notiCuaHv2) -Token $hvToken
        Assert 'T01.3 xem thong bao cua nguoi khac -> 404' ($r.Status -eq 404) ("HTTP $($r.Status)")
    } else {
        Warn 'T01.3' 'khong co thong bao seed gui cho HV002'
    }

    $r = Invoke-Api -Method GET -Path '/api/notifications/summary' -Token $hvToken
    $sum = From-Json-Safe $r.Text
    Assert 'T01.4 GET summary -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")
    Assert 'T01.5 summary co tongSo >= 1' ([int]$sum.tongSo -ge 1) ("tongSo=$($sum.tongSo)")
    Assert 'T01.6 soChuaDoc = null (schema khong ho tro)' ($null -eq $sum.soChuaDoc) ("soChuaDoc=$($sum.soChuaDoc)")

    $r = Invoke-Api -Method GET -Path '/api/notifications'
    Assert 'T01.7 khong co token -> 401' ($r.Status -eq 401) ("HTTP $($r.Status)")

    # ========================================================
    # T02 - Thong bao: gui / sua / thu hoi / xoa
    # ========================================================
    Write-Log ''
    Write-Log '--- T02: thong bao - gui / thu hoi / xoa ---'

    $r = Invoke-Api -Method POST -Path '/api/admin/notifications' -Token $adminToken -Body @{
        maTaiKhoanNhan = $hvAccountId
        tieuDe         = 'Phase 7 test notification'
        noiDung        = 'Noi dung kiem thu tu dong'
        loaiThongBao   = $S_HoatDong
    }
    $created = From-Json-Safe $r.Text
    Assert 'T02.1 admin gui thong bao -> 201' ($r.Status -eq 201) ("HTTP $($r.Status): $($r.Text)")
    $newNotiId = [int]$created.maThongBao
    if ($newNotiId -gt 0) { $createdThongBaoIds.Add($newNotiId) }
    Assert 'T02.2 nguoi gui lay tu JWT (ADMIN001)' ($created.tenDangNhapNguoiGui -eq 'ADMIN001') ("nhan: $($created.tenDangNhapNguoiGui)")

    $r = Invoke-Api -Method GET -Path '/api/notifications' -Token $hvToken
    $afterCreate = From-Json-Safe $r.Text
    Assert 'T02.3 HV001 thay thong bao moi' ([bool]($afterCreate.items | Where-Object { $_.maThongBao -eq $newNotiId })) 'khong thay'

    $r = Invoke-Api -Method PATCH -Path ("/api/admin/notifications/{0}" -f $newNotiId) -Token $adminToken -Body @{ trangThai = $S_DaThuHoi }
    Assert 'T02.4 thu hoi thong bao -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/notifications' -Token $hvToken
    $afterRevoke = From-Json-Safe $r.Text
    Assert 'T02.5 thong bao da thu hoi khong con hien thi' (-not [bool]($afterRevoke.items | Where-Object { $_.maThongBao -eq $newNotiId })) 'van con hien thi'

    $r = Invoke-Api -Method POST -Path '/api/admin/notifications' -Token $adminToken -Body @{
        tieuDe = 'Thieu nguoi nhan'; noiDung = 'Noi dung'
    }
    Assert 'T02.6 thieu ca hai dich -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method POST -Path '/api/admin/notifications' -Token $adminToken -Body @{
        maTaiKhoanNhan = 999999; tieuDe = 'Tai khoan khong ton tai'; noiDung = 'Noi dung'
    }
    Assert 'T02.7 tai khoan nhan khong ton tai -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method POST -Path '/api/admin/notifications' -Token $gvToken -Body @{
        maTaiKhoanNhan = $hvAccountId; tieuDe = 'GV gui truc tiep'; noiDung = 'Noi dung'
    }
    Assert 'T02.8 giao vien gui truc tiep cho tai khoan -> 403' ($r.Status -eq 403) ("HTTP $($r.Status)")

    $gvClass = Invoke-Scalar -Sql 'SELECT TOP 1 MaLopHoc FROM PhanCongGiangDay WHERE MaGiaoVien = @gv AND TrangThai = @tt ORDER BY MaLopHoc' -Params @{ gv = 'GV001'; tt = $S_DangPhanCong }
    if ($null -ne $gvClass) {
        $r = Invoke-Api -Method POST -Path '/api/admin/notifications' -Token $gvToken -Body @{
            maLopHoc = [int]$gvClass; tieuDe = 'GV thong bao lop phu trach'; noiDung = 'Noi dung kiem thu'
        }
        Assert 'T02.9 giao vien gui cho lop phu trach -> 201' ($r.Status -eq 201) ("HTTP $($r.Status): $($r.Text)")
        if ($r.Status -eq 201) {
            $gvNoti = From-Json-Safe $r.Text
            if ([int]$gvNoti.maThongBao -gt 0) { $createdThongBaoIds.Add([int]$gvNoti.maThongBao) }
        }
    } else {
        Warn 'T02.9' 'GV001 khong co phan cong dang hieu luc'
    }

    $r = Invoke-Api -Method POST -Path '/api/admin/notifications' -Token $hvToken -Body @{
        maLopHoc = 1; tieuDe = 'Hoc vien gui'; noiDung = 'Noi dung'
    }
    Assert 'T02.10 hoc vien gui thong bao -> 403' ($r.Status -eq 403) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method DELETE -Path ("/api/admin/notifications/{0}" -f $newNotiId) -Token $nvhvToken
    Assert 'T02.11 NVHV xoa thong bao -> 204' ($r.Status -eq 204) ("HTTP $($r.Status)")
    if ($r.Status -eq 204) { [void]$createdThongBaoIds.Remove($newNotiId) }

    # ========================================================
    # T03 - Khao sat: tao / cau hoi
    # ========================================================
    Write-Log ''
    Write-Log '--- T03: khao sat - tao moi ---'

    $homNay = (Get-Date).ToString('yyyy-MM-dd')
    $r = Invoke-Api -Method POST -Path '/api/admin/surveys' -Token $adminToken -Body @{
        tenKhaoSat = 'Phase 7 automated survey'
        moTa       = 'Khao sat do kiem thu tu dong tao'
        ngayBatDau = $homNay
    }
    $survey = From-Json-Safe $r.Text
    Assert 'T03.1 tao khao sat -> 201' ($r.Status -eq 201) ("HTTP $($r.Status): $($r.Text)")
    $maKhaoSat = [int]$survey.maKhaoSat
    if ($maKhaoSat -gt 0) { $createdKhaoSatIds.Add($maKhaoSat) }
    Assert 'T03.2 trang thai mac dinh = Hoat dong' ($survey.trangThai -eq $S_HoatDong) ("nhan: $($survey.trangThai)")
    Assert 'T03.3 dangMo = true' ($survey.dangMo -eq $true) ("dangMo=$($survey.dangMo)")

    $r = Invoke-Api -Method POST -Path ("/api/admin/surveys/{0}/questions" -f $maKhaoSat) -Token $adminToken -Body @{
        noiDungCauHoi = 'Ban danh gia chat luong giang day the nao?'
        loaiCauHoi    = $S_DanhGia
        thuTu         = 1
        batBuoc       = $true
    }
    $q1 = From-Json-Safe $r.Text
    Assert 'T03.4 tao cau hoi danh gia -> 201' ($r.Status -eq 201) ("HTTP $($r.Status): $($r.Text)")
    $maCauHoi1 = [int]$q1.maCauHoi

    $r = Invoke-Api -Method POST -Path ("/api/admin/surveys/{0}/questions" -f $maKhaoSat) -Token $adminToken -Body @{
        noiDungCauHoi = 'Ban co gop y gi them?'
        loaiCauHoi    = $S_TuLuan
        thuTu         = 2
        batBuoc       = $false
    }
    $q2 = From-Json-Safe $r.Text
    Assert 'T03.5 tao cau hoi tu luan -> 201' ($r.Status -eq 201) ("HTTP $($r.Status): $($r.Text)")
    $maCauHoi2 = [int]$q2.maCauHoi

    $r = Invoke-Api -Method POST -Path ("/api/admin/surveys/{0}/questions" -f $maKhaoSat) -Token $adminToken -Body @{
        noiDungCauHoi = 'Cau hoi trung thu tu'; loaiCauHoi = $S_TuLuan; thuTu = 1
    }
    Assert 'T03.6 thu tu cau hoi trung -> 409' ($r.Status -eq 409) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path ("/api/admin/surveys/{0}" -f $maKhaoSat) -Token $adminToken
    $detail = From-Json-Safe $r.Text
    Assert 'T03.7 chi tiet khao sat co 2 cau hoi' ([int]$detail.soCauHoi -eq 2) ("soCauHoi=$($detail.soCauHoi)")

    # ========================================================
    # T04 - Khao sat: tra loi
    # ========================================================
    Write-Log ''
    Write-Log '--- T04: khao sat - tra loi ---'

    $r = Invoke-Api -Method GET -Path '/api/students/me/surveys' -Token $hvToken
    $mySurveys = From-Json-Safe $r.Text
    Assert 'T04.1 HV001 GET /students/me/surveys -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")
    $found = $mySurveys | Where-Object { $_.maKhaoSat -eq $maKhaoSat }
    Assert 'T04.2 danh sach co khao sat moi, chua tra loi' ([bool]$found -and $found.daTraLoi -eq $false) 'khong thay hoac daTraLoi sai'

    $r = Invoke-Api -Method POST -Path ("/api/students/me/surveys/{0}/responses" -f $maKhaoSat) -Token $hvToken -Body @{
        cauTraLois = @(@{ maCauHoi = $maCauHoi2; noiDungTraLoi = 'Chi gop y' })
    }
    Assert 'T04.3 thieu cau tra loi bat buoc -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method POST -Path ("/api/students/me/surveys/{0}/responses" -f $maKhaoSat) -Token $hvToken -Body @{
        cauTraLois = @(
            @{ maCauHoi = $maCauHoi1; giaTriDanhGia = 5 },
            @{ maCauHoi = $maCauHoi2; giaTriDanhGia = 3 }
        )
    }
    Assert 'T04.4 tra loi sai kieu cau hoi -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method POST -Path ("/api/students/me/surveys/{0}/responses" -f $maKhaoSat) -Token $hvToken -Body @{
        cauTraLois = @(@{ maCauHoi = 999999; giaTriDanhGia = 4 })
    }
    Assert 'T04.5 cau hoi khong thuoc khao sat -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method POST -Path ("/api/students/me/surveys/{0}/responses" -f $maKhaoSat) -Token $hvToken -Body @{
        diemDanhGiaChung = 4.5
        ghiChu           = 'Kiem thu tu dong'
        cauTraLois       = @(
            @{ maCauHoi = $maCauHoi1; giaTriDanhGia = 5 },
            @{ maCauHoi = $maCauHoi2; noiDungTraLoi = 'Rat tot' }
        )
    }
    $resp = From-Json-Safe $r.Text
    Assert 'T04.6 gui phan hoi hop le -> 201' ($r.Status -eq 201) ("HTTP $($r.Status): $($r.Text)")
    Assert 'T04.7 phan hoi co 2 cau tra loi' ($resp.cauTraLois.Count -eq 2) ("count=$($resp.cauTraLois.Count)")

    $r = Invoke-Api -Method POST -Path ("/api/students/me/surveys/{0}/responses" -f $maKhaoSat) -Token $hvToken -Body @{
        cauTraLois = @(@{ maCauHoi = $maCauHoi1; giaTriDanhGia = 3 })
    }
    Assert 'T04.8 tra loi lan 2 -> 409' ($r.Status -eq 409) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path ("/api/students/me/surveys/{0}/response" -f $maKhaoSat) -Token $hvToken
    $myResp = From-Json-Safe $r.Text
    Assert 'T04.9 GET phan hoi cua toi -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")
    Assert 'T04.10 phan hoi dung chu so huu' ($myResp.maHocVien -eq 'HV001') ("nhan: $($myResp.maHocVien)")

    # ========================================================
    # T05 - Khao sat: thong ke
    # ========================================================
    Write-Log ''
    Write-Log '--- T05: khao sat - thong ke ---'

    $r = Invoke-Api -Method GET -Path ("/api/admin/surveys/{0}/statistics" -f $maKhaoSat) -Token $adminToken
    $stats = From-Json-Safe $r.Text
    Assert 'T05.1 GET statistics -> 200' ($r.Status -eq 200) ("HTTP $($r.Status): $($r.Text)")
    Assert 'T05.2 tongSoPhanHoi = 1' ([int]$stats.tongSoPhanHoi -eq 1) ("tongSoPhanHoi=$($stats.tongSoPhanHoi)")
    Assert 'T05.3 diemDanhGiaChungTrungBinh = 4.5' ([decimal]$stats.diemDanhGiaChungTrungBinh -eq 4.5) ("diem=$($stats.diemDanhGiaChungTrungBinh)")

    $statCau1 = $stats.cauHois | Where-Object { $_.maCauHoi -eq $maCauHoi1 }
    Assert 'T05.4 cau hoi danh gia co giaTriTrungBinh = 5' ([decimal]$statCau1.giaTriTrungBinh -eq 5) ("gt=$($statCau1.giaTriTrungBinh)")

    $statCau2 = $stats.cauHois | Where-Object { $_.maCauHoi -eq $maCauHoi2 }
    Assert 'T05.5 cau hoi tu luan co 1 noi dung' ($statCau2.noiDungTuLuan.Count -eq 1) ("count=$($statCau2.noiDungTuLuan.Count)")

    $r = Invoke-Api -Method GET -Path ("/api/admin/surveys/{0}/responses" -f $maKhaoSat) -Token $adminToken
    $respList = From-Json-Safe $r.Text
    Assert 'T05.6 GET danh sach phan hoi -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")
    Assert 'T05.7 danh sach co >= 1 phan hoi' ([int]$respList.totalItems -ge 1) ("total=$($respList.totalItems)")

    $r = Invoke-Api -Method DELETE -Path ("/api/admin/surveys/{0}" -f $maKhaoSat) -Token $adminToken
    Assert 'T05.8 xoa khao sat da co phan hoi -> 409' ($r.Status -eq 409) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method PATCH -Path ("/api/admin/surveys/{0}/status" -f $maKhaoSat) -Token $adminToken -Body @{ trangThai = $S_DaKetThuc }
    Assert 'T05.9 ket thuc khao sat -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method POST -Path ("/api/students/me/surveys/{0}/responses" -f $maKhaoSat) -Token $hv2Token -Body @{
        cauTraLois = @(@{ maCauHoi = $maCauHoi1; giaTriDanhGia = 4 })
    }
    Assert 'T05.10 khao sat da ket thuc -> khong nhan phan hoi (400)' ($r.Status -eq 400) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path ("/api/students/me/surveys/{0}" -f $maKhaoSat) -Token $hv2Token
    Assert 'T05.11 HV002 xem khao sat khong con hoat dong -> 404' ($r.Status -eq 404) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/surveys' -Token $hvToken
    Assert 'T05.12 hoc vien GET /admin/surveys -> 403' ($r.Status -eq 403) ("HTTP $($r.Status)")

    # ========================================================
    # T06 - Bao cao
    # ========================================================
    Write-Log ''
    Write-Log '--- T06: bao cao ---'

    $r = Invoke-Api -Method GET -Path '/api/admin/reports/overview' -Token $adminToken
    $ov = From-Json-Safe $r.Text
    Assert 'T06.1 GET reports/overview (ADMIN) -> 200' ($r.Status -eq 200) ("HTTP $($r.Status): $($r.Text)")
    Assert 'T06.2 overview co tongHocVien >= 1' ([int]$ov.tongHocVien -ge 1) ("tongHocVien=$($ov.tongHocVien)")
    Assert 'T06.3 overview co hocPhi' ($null -ne $ov.hocPhi) 'thieu hocPhi'
    Assert 'T06.4 overview tongPhaiThu >= tongDaThu' ([decimal]$ov.hocPhi.tongPhaiThu -ge [decimal]$ov.hocPhi.tongDaThu) 'sai'

    $r = Invoke-Api -Method GET -Path '/api/admin/reports/overview' -Token $nvhvToken
    Assert 'T06.5 NVHV xem overview -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/reports/overview' -Token $hvToken
    Assert 'T06.6 hoc vien xem overview -> 403' ($r.Status -eq 403) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/reports/overview' -Token $gvToken
    Assert 'T06.7 giao vien xem overview -> 403' ($r.Status -eq 403) ("HTTP $($r.Status)")

    $lopCoHocVien = Invoke-Scalar -Sql 'SELECT TOP 1 MaLopHoc FROM DangKyHoc WHERE MaLopHoc IS NOT NULL GROUP BY MaLopHoc ORDER BY COUNT(*) DESC'
    if ($null -ne $lopCoHocVien) {
        $r = Invoke-Api -Method GET -Path ("/api/admin/reports/academic?maLopHoc={0}" -f [int]$lopCoHocVien) -Token $adminToken
        $acc = From-Json-Safe $r.Text
        Assert 'T06.8 GET reports/academic -> 200' ($r.Status -eq 200) ("HTTP $($r.Status): $($r.Text)")
        Assert 'T06.9 academic co soHocVien >= 1' ([int]$acc.soHocVien -ge 1) ("soHocVien=$($acc.soHocVien)")

        $r = Invoke-Api -Method GET -Path ("/api/admin/reports/attendance?maLopHoc={0}&nguongVang=30" -f [int]$lopCoHocVien) -Token $adminToken
        $att = From-Json-Safe $r.Text
        Assert 'T06.10 GET reports/attendance -> 200' ($r.Status -eq 200) ("HTTP $($r.Status): $($r.Text)")
        Assert 'T06.11 attendance co danh sach hoc vien' ($null -ne $att.hocViens) 'thieu hocViens'
    } else {
        Warn 'T06.8-T06.11' 'khong co lop nao co hoc vien'
    }

    $r = Invoke-Api -Method GET -Path '/api/admin/reports/academic' -Token $adminToken
    Assert 'T06.12 academic thieu maLopHoc -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/reports/academic?maLopHoc=999999' -Token $adminToken
    Assert 'T06.13 academic lop khong ton tai -> 404' ($r.Status -eq 404) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/reports/attendance?maLopHoc=1&tuNgay=2026-12-31&denNgay=2026-01-01' -Token $adminToken
    Assert 'T06.14 attendance khoang ngay sai -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")

    $lopKhongThuocGv = Invoke-Scalar -Sql 'SELECT TOP 1 l.MaLopHoc FROM LopHoc l WHERE NOT EXISTS (SELECT 1 FROM PhanCongGiangDay p WHERE p.MaLopHoc = l.MaLopHoc AND p.MaGiaoVien = @gv AND p.TrangThai = @tt) ORDER BY l.MaLopHoc' -Params @{ gv = 'GV001'; tt = $S_DangPhanCong }
    if ($null -ne $lopKhongThuocGv) {
        $r = Invoke-Api -Method GET -Path ("/api/admin/reports/academic?maLopHoc={0}" -f [int]$lopKhongThuocGv) -Token $gvToken
        Assert 'T06.15 giao vien xem lop khong phu trach -> 403' ($r.Status -eq 403) ("HTTP $($r.Status)")
    } else {
        Warn 'T06.15' 'GV001 duoc phan cong tat ca cac lop'
    }

    $r = Invoke-Api -Method GET -Path '/api/admin/reports/surveys' -Token $adminToken
    $sr = From-Json-Safe $r.Text
    Assert 'T06.16 GET reports/surveys -> 200' ($r.Status -eq 200) ("HTTP $($r.Status): $($r.Text)")
    Assert 'T06.17 reports/surveys co tongKhaoSat >= 1' ([int]$sr.tongKhaoSat -ge 1) ("tongKhaoSat=$($sr.tongKhaoSat)")

    $r = Invoke-Api -Method GET -Path '/api/admin/reports/surveys' -Token $hvToken
    Assert 'T06.18 hoc vien xem reports/surveys -> 403' ($r.Status -eq 403) ("HTTP $($r.Status)")

    # ========================================================
    # T07 - Regression Phase 1..6
    # ========================================================
    Write-Log ''
    Write-Log '--- T07: regression Phase 1-6 ---'

    $r = Invoke-Api -Method GET -Path '/health'
    Assert 'T07.1 /health -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/health/database'
    Assert 'T07.2 /health/database -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/auth/me' -Token $adminToken
    Assert 'T07.3 Phase 2 /auth/me -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/students' -Token $adminToken
    Assert 'T07.4 Phase 3 /admin/students -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/courses' -Token $adminToken
    Assert 'T07.5 Phase 4 /admin/courses -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/students/me/grades' -Token $hvToken
    Assert 'T07.6 Phase 5 /students/me/grades -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/students/me/tuition-fees' -Token $hvToken
    Assert 'T07.7 Phase 6 /students/me/tuition-fees -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/tuition-fees/reports/summary' -Token $adminToken
    Assert 'T07.8 Phase 6 bao cao hoc phi -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/students'
    Assert 'T07.9 khong co token -> 401' ($r.Status -eq 401) ("HTTP $($r.Status)")

    $swaggerStatus = Get-UrlStatus "$BaseUrl/swagger/v1/swagger.json"
    Assert 'T07.10 Swagger JSON -> 200' ($swaggerStatus -eq 200) ("HTTP $swaggerStatus")
}
catch {
    Write-Log ("  [ERROR] {0}" -f $_.Exception.Message)
    $script:failed++
}
finally {
    Write-Log ''
    Write-Log '--- DON DEP DU LIEU TEST ---'

    if ($createdKhaoSatIds.Count -gt 0) {
        $idList = ($createdKhaoSatIds | ForEach-Object { [string]$_ }) -join ','
        try {
            Invoke-Query -Sql "DELETE FROM PhanHoiKhaoSat WHERE MaKhaoSat IN ($idList)" | Out-Null
            Invoke-Query -Sql "DELETE FROM KhaoSat WHERE MaKhaoSat IN ($idList)" | Out-Null
            Write-Log ("  da xoa khao sat test: {0}" -f $idList)
        } catch {
            Write-Log ("  [CANH BAO] don dep khao sat that bai: {0}" -f $_.Exception.Message)
        }
    }

    if ($createdThongBaoIds.Count -gt 0) {
        $idList = ($createdThongBaoIds | ForEach-Object { [string]$_ }) -join ','
        try {
            Invoke-Query -Sql "DELETE FROM ThongBao WHERE MaThongBao IN ($idList)" | Out-Null
            Write-Log ("  da xoa thong bao test: {0}" -f $idList)
        } catch {
            Write-Log ("  [CANH BAO] don dep thong bao that bai: {0}" -f $_.Exception.Message)
        }
    }

    $soKhaoSat = Invoke-Scalar -Sql 'SELECT COUNT(*) FROM KhaoSat'
    $soThongBao = Invoke-Scalar -Sql 'SELECT COUNT(*) FROM ThongBao'
    Write-Log ("  con lai trong DB: KhaoSat={0} | ThongBao={1}" -f $soKhaoSat, $soThongBao)

    Write-Log ''
    Write-Log '============================================================'
    Write-Log (" KET QUA: PASS = {0} | FAIL = {1} | WARN = {2}" -f $script:passed, $script:failed, $script:warns)
    Write-Log '============================================================'

    $log | Out-File -FilePath $reportPath -Encoding utf8
    Write-Output ("PHASE7_TEST_DONE pass={0} fail={1} warn={2}" -f $script:passed, $script:failed, $script:warns)
}
