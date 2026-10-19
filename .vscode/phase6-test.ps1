# ============================================================
# PHASE 6 - Hoc phi / Thanh toan / Bien nhan
# ------------------------------------------------------------
# YEU CAU:
#   - API dang chay tai http://localhost:5204
#   - SQL Server: localhost\SQLEXPRESS, Windows Auth
#
# AN TOAN DU LIEU:
#   - Script CHI tao 2 nghia vu hoc phi MOI tren 2 dang ky CHUA co hoc phi.
#   - Moi giao dich / bien nhan sinh ra deu thuoc 2 hoc phi do.
#   - finally: xoa BienNhan -> ThanhToan -> HocPhi cua 2 hoc phi test.
#   - KHONG dung vao du lieu seed (HV001..HV004).
#
# LUU Y ENCODING:
#   Windows PowerShell 5.1 doc .ps1 theo ANSI => KHONG viet literal tieng Viet.
#   Moi chuoi tieng Viet duoc tao tu ma Unicode qua ham VN.
# ============================================================
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName System.Data

$BaseUrl          = 'http://localhost:5204'
$DevPassword      = 'LanguageCenter@2026'
$ConnectionString = 'Server=localhost\SQLEXPRESS;Database=QuanLyTrungTamNgoaiNgu;Integrated Security=True'

$reportPath = Join-Path $PSScriptRoot 'phase6-test-report.txt'
$log     = [System.Collections.Generic.List[string]]::new()
$script:passed = 0
$script:failed = 0
$script:warns  = 0

# ------------------------------------------------------------
# Chuoi tieng Viet tao tu ma Unicode
# ------------------------------------------------------------
function VN {
    param([object[]]$Parts)
    $sb = New-Object System.Text.StringBuilder
    foreach ($p in $Parts) {
        if ($p -is [int]) { [void]$sb.Append([char]$p) } else { [void]$sb.Append([string]$p) }
    }
    return $sb.ToString()
}

$S_ChuaThanhToan   = VN @('Ch', 0x01B0, 'a thanh to', 0x00E1, 'n')
$S_ThanhToanMotPhan= VN @('Thanh to', 0x00E1, 'n m', 0x1ED9, 't ph', 0x1EA7, 'n')
$S_DaThanhToan     = VN @(0x0110, 0x00E3, ' thanh to', 0x00E1, 'n')
$S_TienMat         = VN @('Ti', 0x1EC1, 'n m', 0x1EB7, 't')
$S_ChuyenKhoan     = VN @('Chuy', 0x1EC3, 'n kho', 0x1EA3, 'n')
$S_ThuTien         = VN @('Thu ti', 0x1EC1, 'n')
$S_HoanTien        = VN @('Ho', 0x00E0, 'n ti', 0x1EC1, 'n')
$S_ThanhCong       = VN @('Th', 0x00E0, 'nh c', 0x00F4, 'ng')
$S_DaHuy           = VN @(0x0110, 0x00E3, ' h', 0x1EE7, 'y')
$S_DangLamViec     = VN @(0x0110, 'ang l', 0x00E0, 'm vi', 0x1EC7, 'c')

function Write-Log { param([string]$Text) $log.Add($Text); Write-Output $Text }

function Assert {
    param([string]$Name, [bool]$Condition, [string]$Detail)
    if ($Condition) {
        $script:passed++
        Write-Log ("  [PASS] {0}" -f $Name)
    } else {
        $script:failed++
        Write-Log ("  [FAIL] {0}  -> {1}" -f $Name, $Detail)
    }
}

function Warn {
    param([string]$Name, [string]$Detail)
    $script:warns++
    Write-Log ("  [WARN] {0}  -> {1}" -f $Name, $Detail)
}

# ------------------------------------------------------------
# HTTP
# ------------------------------------------------------------
function Invoke-Api {
    param([string]$Method, [string]$Path, [object]$Body, [string]$Token)
    $uri = "$BaseUrl$Path"
    $headers = @{}
    if ($Token) { $headers['Authorization'] = "Bearer $Token" }

    try {
        $params = @{
            Uri             = $uri
            Method          = $Method
            Headers         = $headers
            UseBasicParsing = $true
            ErrorAction     = 'Stop'
        }
        if ($null -ne $Body) {
            $json = ($Body | ConvertTo-Json -Depth 6 -Compress)
            $params['ContentType'] = 'application/json; charset=utf-8'
            $params['Body'] = [System.Text.Encoding]::UTF8.GetBytes($json)
        }
        $response = Invoke-WebRequest @params
        $reader = New-Object System.IO.StreamReader($response.RawContentStream, [System.Text.Encoding]::UTF8)
        return @{ Status = [int]$response.StatusCode; Text = $reader.ReadToEnd() }
    }
    catch {
        $response = $_.Exception.Response
        if ($null -eq $response) { return @{ Status = -1; Text = $_.Exception.Message } }
        $stream = $response.GetResponseStream()
        $reader = New-Object System.IO.StreamReader($stream, [System.Text.Encoding]::UTF8)
        return @{ Status = [int]$response.StatusCode; Text = $reader.ReadToEnd() }
    }
}

function From-Json-Safe {
    param([string]$Text)
    if ([string]::IsNullOrWhiteSpace($Text)) { return $null }
    try { return $Text | ConvertFrom-Json } catch { return $null }
}

# ------------------------------------------------------------
# SQL
# ------------------------------------------------------------
function Invoke-Query {
    param([string]$Sql, [hashtable]$Params = @{})
    $conn = New-Object System.Data.SqlClient.SqlConnection($ConnectionString)
    $conn.Open()
    try {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $Sql
        $cmd.CommandTimeout = 60
        foreach ($key in $Params.Keys) {
            $p = $cmd.Parameters.AddWithValue("@$key", $Params[$key])
            if ($null -eq $Params[$key]) { $p.Value = [System.DBNull]::Value }
        }
        $adapter = New-Object System.Data.SqlClient.SqlDataAdapter($cmd)
        $table = New-Object System.Data.DataTable
        [void]$adapter.Fill($table)
        # Dau phay ngan PowerShell tu bung DataTable thanh tap DataRow khi tra ve.
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

# ------------------------------------------------------------
# Khoi tao
# ------------------------------------------------------------
Write-Log '============================================================'
Write-Log ' PHASE 6 - TEST SUITE (hoc phi / thanh toan / bien nhan)'
Write-Log '============================================================'

$testHocPhiIds   = [System.Collections.Generic.List[int]]::new()
$testDangKyIds   = [System.Collections.Generic.List[int]]::new()

$health = Invoke-Api -Method GET -Path '/health'
Write-Log ("Health check: HTTP {0}" -f $health.Status)
if ($health.Status -ne 200) {
    Write-Log 'API chua san sang. Ket thuc.'
    $log | Out-File -FilePath $reportPath -Encoding utf8
    exit 1
}

# ---- Dang nhap ----
$loginAdmin = Invoke-Api -Method POST -Path '/api/auth/login' -Body @{ username = 'ADMIN001'; password = $DevPassword }
$adminToken = (From-Json-Safe $loginAdmin.Text).accessToken
Assert 'LOGIN admin (ADMIN001)' ($loginAdmin.Status -eq 200 -and $adminToken) ("HTTP $($loginAdmin.Status)")

$loginKt = Invoke-Api -Method POST -Path '/api/auth/login' -Body @{ username = 'NVKT001'; password = $DevPassword }
$ktToken = (From-Json-Safe $loginKt.Text).accessToken
Assert 'LOGIN ke toan (NVKT001)' ($loginKt.Status -eq 200 -and $ktToken) ("HTTP $($loginKt.Status)")

$loginTuvan = Invoke-Api -Method POST -Path '/api/auth/login' -Body @{ username = 'NVTV001'; password = $DevPassword }
$tuVanToken = (From-Json-Safe $loginTuvan.Text).accessToken
Assert 'LOGIN tu van (NVTV001)' ($loginTuvan.Status -eq 200 -and $tuVanToken) ("HTTP $($loginTuvan.Status)")

$loginHv = Invoke-Api -Method POST -Path '/api/auth/login' -Body @{ username = 'HV001'; password = $DevPassword }
$hvToken = (From-Json-Safe $loginHv.Text).accessToken
Assert 'LOGIN hoc vien (HV001)' ($loginHv.Status -eq 200 -and $hvToken) ("HTTP $($loginHv.Status)")

try {
    # ========================================================
    # Chon 2 dang ky CHUA co hoc phi
    # ========================================================
    $pickSql = @"
SELECT TOP (@Top) d.MaDangKy, d.MaHocVien, d.HocPhiGoc, d.SoTienGiam
FROM DangKyHoc d
WHERE NOT EXISTS (SELECT 1 FROM HocPhi h WHERE h.MaDangKy = d.MaDangKy)
  AND d.TrangThaiDangKy <> @DaHuy
  AND (d.HocPhiGoc - d.SoTienGiam) > 0
ORDER BY d.MaDangKy DESC
"@
    $candidates = Invoke-Query -Sql $pickSql -Params @{ Top = 2; DaHuy = $S_DaHuy }

    $tongDangKy = Invoke-Scalar -Sql 'SELECT COUNT(*) FROM DangKyHoc'
    $tongHocPhi = Invoke-Scalar -Sql 'SELECT COUNT(*) FROM HocPhi'
    Write-Log ("  chan doan: DangKyHoc={0} | HocPhi={1} | ung vien san co={2}" -f $tongDangKy, $tongHocPhi, $candidates.Rows.Count)

    $dks = [System.Collections.Generic.List[object]]::new()

    foreach ($row in $candidates.Rows) {
        $dks.Add([pscustomobject]@{
            MaDangKy   = [int]$row['MaDangKy']
            MaHocVien  = [string]$row['MaHocVien']
            HocPhiGoc  = [decimal]$row['HocPhiGoc']
            SoTienGiam = [decimal]$row['SoTienGiam']
        })
    }

    # Khong du du lieu san co -> tao dang ky test qua API (se duoc xoa trong finally).
    if ($dks.Count -lt 2) {
        $need = 2 - $dks.Count
        Write-Log ("  tao {0} dang ky test qua API..." -f $need)

        $pairSql = @"
SELECT TOP (@Top) hv.MaHocVien, kh.MaKhoaHoc
FROM HocVien hv
CROSS JOIN KhoaHoc kh
WHERE NOT EXISTS (SELECT 1 FROM DangKyHoc d WHERE d.MaHocVien = hv.MaHocVien AND d.MaKhoaHoc = kh.MaKhoaHoc)
  AND kh.HocPhiCoBan > 0
  AND hv.MaHocVien <> @Owner
ORDER BY hv.MaHocVien, kh.MaKhoaHoc
"@
        $pairs = Invoke-Query -Sql $pairSql -Params @{ Top = $need; Owner = 'HV001' }

        foreach ($p in $pairs.Rows) {
            $maHocVienMoi = [string]$p['MaHocVien']
            $maKhoaHocMoi = [int]$p['MaKhoaHoc']

            $rc = Invoke-Api -Method POST -Path '/api/admin/enrollments' -Token $tuVanToken -Body @{
                maHocVien = $maHocVienMoi
                maKhoaHoc = $maKhoaHocMoi
                ghiChu    = 'Phase 6 automated test'
            }

            if ($rc.Status -ne 201) {
                Warn 'tao dang ky test' ("hocVien=$maHocVienMoi khoaHoc=$maKhoaHocMoi -> HTTP $($rc.Status): $($rc.Text)")
                continue
            }

            $enr = From-Json-Safe $rc.Text
            $newDangKy = [int]$enr.maDangKy
            $testDangKyIds.Add($newDangKy)

            $rowNew = (Invoke-Query -Sql 'SELECT MaDangKy, MaHocVien, HocPhiGoc, SoTienGiam FROM DangKyHoc WHERE MaDangKy = @id' -Params @{ id = $newDangKy }).Rows[0]

            $dks.Add([pscustomobject]@{
                MaDangKy   = [int]$rowNew['MaDangKy']
                MaHocVien  = [string]$rowNew['MaHocVien']
                HocPhiGoc  = [decimal]$rowNew['HocPhiGoc']
                SoTienGiam = [decimal]$rowNew['SoTienGiam']
            })
        }
    }

    if ($dks.Count -lt 2) {
        Write-Log '  [SKIP] Khong tao duoc du 2 dang ky test. Ket thuc.'
        throw 'NOT_ENOUGH_DATA'
    }

    $dk1 = $dks[0]
    $dk2 = $dks[1]
    $maDangKy1 = $dk1.MaDangKy
    $maDangKy2 = $dk2.MaDangKy
    $chuSoHuu1 = $dk1.MaHocVien

    Write-Log ("Dang ky test #1 = {0} (hoc vien {1})" -f $maDangKy1, $chuSoHuu1)
    Write-Log ("Dang ky test #2 = {0} (hoc vien {1})" -f $maDangKy2, $dk2.MaHocVien)

    # ========================================================
    # T01 - Tao hoc phi hop le
    # ========================================================
    Write-Log ''
    Write-Log '--- T01: tao nghia vu hoc phi ---'
    $r = Invoke-Api -Method POST -Path '/api/admin/tuition-fees' -Token $adminToken -Body @{ maDangKy = $maDangKy1 }
    $dto = From-Json-Safe $r.Text
    Assert 'T01.1 tao hoc phi -> 201' ($r.Status -eq 201) ("HTTP $($r.Status): $($r.Text)")
    Assert 'T01.2 trang thai ban dau = Chua thanh toan' ($dto.trangThaiHocPhi -eq $S_ChuaThanhToan) ("nhan: $($dto.trangThaiHocPhi)")
    Assert 'T01.3 soTienDaDong = 0' ([decimal]$dto.soTienDaDong -eq 0) ("nhan: $($dto.soTienDaDong)")

    $maHocPhi1 = [int]$dto.maHocPhi
    if ($maHocPhi1 -gt 0) { $testHocPhiIds.Add($maHocPhi1) }

    $dbRow = Invoke-Query -Sql 'SELECT SoTienGoc, SoTienGiam, SoTienPhaiDong, SoTienDaDong, SoTienConThieu, TrangThaiHocPhi FROM HocPhi WHERE MaHocPhi = @id' -Params @{ id = $maHocPhi1 }
    if ($dbRow.Rows.Count -eq 1) {
        $db = $dbRow.Rows[0]
        Assert 'T01.4 DB soTienGoc = DangKyHoc.HocPhiGoc' ([decimal]$db['SoTienGoc'] -eq [decimal]$dk1.HocPhiGoc) ("DB=$($db['SoTienGoc']) DangKy=$($dk1.HocPhiGoc)")
        Assert 'T01.5 DB soTienGiam = DangKyHoc.SoTienGiam' ([decimal]$db['SoTienGiam'] -eq [decimal]$dk1.SoTienGiam) ("DB=$($db['SoTienGiam']) DangKy=$($dk1.SoTienGiam)")
        Assert 'T01.6 DB soTienPhaiDong = goc - giam' ([decimal]$db['SoTienPhaiDong'] -eq ([decimal]$db['SoTienGoc'] - [decimal]$db['SoTienGiam'])) ("phaiDong=$($db['SoTienPhaiDong'])")
        Assert 'T01.7 DB soTienConThieu = phaiDong' ([decimal]$db['SoTienConThieu'] -eq [decimal]$db['SoTienPhaiDong']) ("conThieu=$($db['SoTienConThieu'])")
    } else {
        Assert 'T01.4-7 doc lai DB' $false 'khong tim thay dong HocPhi vua tao'
    }

    # ========================================================
    # T02 - Tao trung dang ky -> 409
    # ========================================================
    Write-Log ''
    Write-Log '--- T02: chong tao trung nghia vu hoc phi ---'
    $r = Invoke-Api -Method POST -Path '/api/admin/tuition-fees' -Token $adminToken -Body @{ maDangKy = $maDangKy1 }
    Assert 'T02.1 tao lai cung maDangKy -> 409' ($r.Status -eq 409) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method POST -Path '/api/admin/tuition-fees' -Token $adminToken -Body @{ maDangKy = 999999 }
    Assert 'T02.2 dang ky khong ton tai -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")

    # ========================================================
    # T03 - Tra cuu
    # ========================================================
    Write-Log ''
    Write-Log '--- T03: tra cuu hoc phi ---'
    $r = Invoke-Api -Method GET -Path ("/api/admin/tuition-fees?maHocVien={0}" -f $chuSoHuu1) -Token $adminToken
    $page = From-Json-Safe $r.Text
    Assert 'T03.1 GET danh sach -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")
    Assert 'T03.2 danh sach co ban ghi vua tao' ([bool]($page.items | Where-Object { $_.maHocPhi -eq $maHocPhi1 })) ("total=$($page.totalItems)")

    $r = Invoke-Api -Method GET -Path ("/api/admin/tuition-fees/{0}" -f $maHocPhi1) -Token $adminToken
    $detail = From-Json-Safe $r.Text
    Assert 'T03.3 GET chi tiet -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")
    Assert 'T03.4 chi tiet dung chu so huu' ($detail.maHocVien -eq $chuSoHuu1) ("nhan: $($detail.maHocVien)")

    $r = Invoke-Api -Method GET -Path '/api/admin/tuition-fees/999999' -Token $adminToken
    Assert 'T03.5 hoc phi khong ton tai -> 404' ($r.Status -eq 404) ("HTTP $($r.Status)")

    # ========================================================
    # T04 - Validate so tien
    # ========================================================
    Write-Log ''
    Write-Log '--- T04: validate so tien ---'
    $tongPhaiDong = [decimal]$detail.soTienPhaiDong
    $soTienMotPhan = [math]::Floor($tongPhaiDong / 2)
    if ($soTienMotPhan -lt 1) { $soTienMotPhan = 1 }

    $r = Invoke-Api -Method POST -Path ("/api/admin/tuition-fees/{0}/payments" -f $maHocPhi1) -Token $ktToken -Body @{
        maGiaoDich = 'GDTEST-ZERO-001'; soTien = 0; phuongThucThanhToan = $S_TienMat
    }
    Assert 'T04.1 soTien = 0 -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method POST -Path ("/api/admin/tuition-fees/{0}/payments" -f $maHocPhi1) -Token $ktToken -Body @{
        maGiaoDich = 'GDTEST-NEG-001'; soTien = -50000; phuongThucThanhToan = $S_TienMat
    }
    Assert 'T04.2 soTien am -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method POST -Path ("/api/admin/tuition-fees/{0}/payments" -f $maHocPhi1) -Token $ktToken -Body @{
        maGiaoDich = 'GDTEST-OVER-001'; soTien = ($tongPhaiDong + 100000); phuongThucThanhToan = $S_ChuyenKhoan
    }
    Assert 'T04.3 vuot so du -> 409' ($r.Status -eq 409) ("HTTP $($r.Status)")
    $daDongSauVuot = Invoke-Scalar -Sql 'SELECT SoTienDaDong FROM HocPhi WHERE MaHocPhi = @id' -Params @{ id = $maHocPhi1 }
    Assert 'T04.4 vuot so du KHONG lam thay doi soTienDaDong' ([decimal]$daDongSauVuot -eq 0) ("daDong=$($daDongSauVuot)")

    $r = Invoke-Api -Method POST -Path ("/api/admin/tuition-fees/{0}/payments" -f $maHocPhi1) -Token $ktToken -Body @{
        maGiaoDich = 'GDTEST-HOANTIEN-001'; soTien = 100000; phuongThucThanhToan = $S_TienMat; loaiGiaoDich = $S_HoanTien
    }
    Assert 'T04.5 loaiGiaoDich Hoan tien -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method POST -Path '/api/admin/tuition-fees/999999/payments' -Token $ktToken -Body @{
        maGiaoDich = 'GDTEST-404-001'; soTien = 100000; phuongThucThanhToan = $S_TienMat
    }
    Assert 'T04.6 hoc phi khong ton tai -> 404' ($r.Status -eq 404) ("HTTP $($r.Status)")

    # ========================================================
    # T05 - Thanh toan hop le (mot phan)
    # ========================================================
    Write-Log ''
    Write-Log '--- T05: ghi nhan thanh toan hop le ---'
    $gd1 = 'GDTEST-' + (Get-Date -Format 'yyyyMMddHHmmss') + '-A'
    $r = Invoke-Api -Method POST -Path ("/api/admin/tuition-fees/{0}/payments" -f $maHocPhi1) -Token $ktToken -Body @{
        maGiaoDich           = $gd1
        soTien               = $soTienMotPhan
        phuongThucThanhToan  = $S_ChuyenKhoan
        ghiChu               = 'Phase 6 automated test'
        noiDungBienNhan      = 'Thu hoc phi - automated test'
    }
    $pay1 = From-Json-Safe $r.Text
    Assert 'T05.1 thanh toan -> 201' ($r.Status -eq 201) ("HTTP $($r.Status): $($r.Text)")
    Assert 'T05.2 giao dich co trang thai Thanh cong' ($pay1.trangThai -eq $S_ThanhCong) ("nhan: $($pay1.trangThai)")
    Assert 'T05.3 ma nhan vien ke toan lay tu JWT (NVKT001)' ($pay1.maNhanVienKeToan -eq 'NVKT001') ("nhan: $($pay1.maNhanVienKeToan)")
    Assert 'T05.4 co bien nhan kem theo' ($null -ne $pay1.bienNhan -and [int]$pay1.bienNhan.maBienNhan -gt 0) ("bienNhan: $($pay1.bienNhan)")
    Assert 'T05.5 so bien nhan dung dinh dang BN+yyyyMMdd+3 so' ($pay1.bienNhan.soBienNhan -match '^BN\d{8}\d{3}$') ("soBienNhan: $($pay1.bienNhan.soBienNhan)")

    $maThanhToan1 = [int]$pay1.maThanhToan

    $db = (Invoke-Query -Sql 'SELECT SoTienDaDong, SoTienConThieu, TrangThaiHocPhi FROM HocPhi WHERE MaHocPhi = @id' -Params @{ id = $maHocPhi1 }).Rows[0]
    Assert 'T05.6 DB soTienDaDong = so tien vua dong' ([decimal]$db['SoTienDaDong'] -eq $soTienMotPhan) ("DB=$($db['SoTienDaDong']) gui=$soTienMotPhan")
    Assert 'T05.7 DB trang thai = Thanh toan mot phan' ($db['TrangThaiHocPhi'] -eq $S_ThanhToanMotPhan) ("nhan: $($db['TrangThaiHocPhi'])")
    Assert 'T05.8 DB soTienConThieu dung' ([decimal]$db['SoTienConThieu'] -eq ($tongPhaiDong - $soTienMotPhan)) ("conThieu=$($db['SoTienConThieu'])")

    # ========================================================
    # T06 - Idempotency
    # ========================================================
    Write-Log ''
    Write-Log '--- T06: chong giao dich trung lap ---'
    $r = Invoke-Api -Method POST -Path ("/api/admin/tuition-fees/{0}/payments" -f $maHocPhi1) -Token $ktToken -Body @{
        maGiaoDich = $gd1; soTien = $soTienMotPhan; phuongThucThanhToan = $S_ChuyenKhoan
    }
    $pay1b = From-Json-Safe $r.Text
    Assert 'T06.1 gui lai cung maGiaoDich -> 200' ($r.Status -eq 200) ("HTTP $($r.Status): $($r.Text)")
    Assert 'T06.2 tra ve dung giao dich cu' ([int]$pay1b.maThanhToan -eq $maThanhToan1) ("maThanhToan: $($pay1b.maThanhToan) vs $maThanhToan1")

    $dem = Invoke-Scalar -Sql 'SELECT COUNT(*) FROM ThanhToan WHERE MaGiaoDich = @gd' -Params @{ gd = $gd1 }
    Assert 'T06.3 DB chi co 1 giao dich voi ma nay' ([int]$dem -eq 1) ("count=$dem")

    $daDongSauLap = Invoke-Scalar -Sql 'SELECT SoTienDaDong FROM HocPhi WHERE MaHocPhi = @id' -Params @{ id = $maHocPhi1 }
    Assert 'T06.4 soTienDaDong khong bi cong doi' ([decimal]$daDongSauLap -eq $soTienMotPhan) ("daDong=$($daDongSauLap)")

    # ========================================================
    # T07 - Thanh toan phan con lai
    # ========================================================
    Write-Log ''
    Write-Log '--- T07: thanh toan du ---'
    $conLai = $tongPhaiDong - $soTienMotPhan
    $gd2 = 'GDTEST-' + (Get-Date -Format 'yyyyMMddHHmmss') + '-B'
    $r = Invoke-Api -Method POST -Path ("/api/admin/tuition-fees/{0}/payments" -f $maHocPhi1) -Token $adminToken -Body @{
        maGiaoDich = $gd2; soTien = $conLai; phuongThucThanhToan = $S_TienMat
    }
    Assert 'T07.1 thanh toan phan con lai -> 201' ($r.Status -eq 201) ("HTTP $($r.Status): $($r.Text)")

    $db = (Invoke-Query -Sql 'SELECT SoTienDaDong, SoTienConThieu, TrangThaiHocPhi FROM HocPhi WHERE MaHocPhi = @id' -Params @{ id = $maHocPhi1 }).Rows[0]
    Assert 'T07.2 DB trang thai = Da thanh toan' ($db['TrangThaiHocPhi'] -eq $S_DaThanhToan) ("nhan: $($db['TrangThaiHocPhi'])")
    Assert 'T07.3 DB soTienConThieu = 0' ([decimal]$db['SoTienConThieu'] -eq 0) ("conThieu=$($db['SoTienConThieu'])")

    # ========================================================
    # T08 - Bien nhan
    # ========================================================
    Write-Log ''
    Write-Log '--- T08: bien nhan ---'
    $soBienNhan1 = $pay1.bienNhan.soBienNhan
    $demBn = Invoke-Scalar -Sql 'SELECT COUNT(*) FROM BienNhan WHERE MaThanhToan = @mt' -Params @{ mt = $maThanhToan1 }
    Assert 'T08.1 moi giao dich chi co 1 bien nhan' ([int]$demBn -eq 1) ("count=$demBn")

    $r = Invoke-Api -Method GET -Path ("/api/admin/receipts/by-number/{0}" -f $soBienNhan1) -Token $ktToken
    $rec = From-Json-Safe $r.Text
    Assert 'T08.2 tra cuu bien nhan theo so -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")
    Assert 'T08.3 bien nhan gan dung giao dich' ([int]$rec.maThanhToan -eq $maThanhToan1) ("maThanhToan: $($rec.maThanhToan)")
    Assert 'T08.4 so tien bien nhan = so tien giao dich' ([decimal]$rec.soTienXacNhan -eq $soTienMotPhan) ("soTienXacNhan=$($rec.soTienXacNhan)")

    $r = Invoke-Api -Method GET -Path ("/api/admin/receipts?maHocVien={0}" -f $chuSoHuu1) -Token $ktToken
    $recPage = From-Json-Safe $r.Text
    Assert 'T08.5 GET danh sach bien nhan -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")
    Assert 'T08.6 danh sach co bien nhan vua tao' ([bool]($recPage.items | Where-Object { $_.soBienNhan -eq $soBienNhan1 })) ("total=$($recPage.totalItems)")

    # ========================================================
    # T09 - Tra cuu giao dich
    # ========================================================
    Write-Log ''
    Write-Log '--- T09: tra cuu giao dich ---'
    $r = Invoke-Api -Method GET -Path ("/api/admin/tuition-fees/{0}/payments" -f $maHocPhi1) -Token $adminToken
    $payList = From-Json-Safe $r.Text
    Assert 'T09.1 GET giao dich cua hoc phi -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")
    Assert 'T09.2 co dung 2 giao dich' ([int]$payList.totalItems -eq 2) ("total=$($payList.totalItems)")

    $r = Invoke-Api -Method GET -Path ("/api/admin/payments/{0}" -f $maThanhToan1) -Token $adminToken
    $payDetail = From-Json-Safe $r.Text
    Assert 'T09.3 GET chi tiet giao dich -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")
    Assert 'T09.4 chi tiet co bien nhan' ($null -ne $payDetail.bienNhan) ("bienNhan=$($payDetail.bienNhan)")

    $r = Invoke-Api -Method GET -Path '/api/admin/payments/999999' -Token $adminToken
    Assert 'T09.5 giao dich khong ton tai -> 404' ($r.Status -eq 404) ("HTTP $($r.Status)")

    # ========================================================
    # T10 - Phan quyen
    # ========================================================
    Write-Log ''
    Write-Log '--- T10: phan quyen ---'
    $r = Invoke-Api -Method POST -Path ("/api/admin/tuition-fees/{0}/payments" -f $maHocPhi1) -Token $hvToken -Body @{
        maGiaoDich = 'GDTEST-HV-001'; soTien = 1000; phuongThucThanhToan = $S_TienMat
    }
    Assert 'T10.1 hoc vien tu ghi nhan thanh toan -> 403' ($r.Status -eq 403) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/payments' -Token $hvToken
    Assert 'T10.2 hoc vien xem danh sach giao dich admin -> 403' ($r.Status -eq 403) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/tuition-fees' -Token $hvToken
    Assert 'T10.3 hoc vien xem danh sach hoc phi admin -> 403' ($r.Status -eq 403) ("HTTP $($r.Status)")

    # Nhan vien tu van: chi doc
    $r = Invoke-Api -Method GET -Path '/api/admin/tuition-fees' -Token $tuVanToken
    Assert 'T10.4 tu van duoc XEM hoc phi -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method POST -Path ("/api/admin/tuition-fees/{0}/payments" -f $maHocPhi1) -Token $tuVanToken -Body @{
        maGiaoDich = 'GDTEST-TV-001'; soTien = 1000; phuongThucThanhToan = $S_TienMat
    }
    Assert 'T10.5 tu van KHONG duoc ghi nhan thanh toan -> 403' ($r.Status -eq 403) ("HTTP $($r.Status)")

    # ========================================================
    # T11 - Hoc vien xem du lieu cua chinh minh
    # ========================================================
    Write-Log ''
    Write-Log '--- T11: API tu phuc vu cua hoc vien ---'
    $r = Invoke-Api -Method GET -Path '/api/students/me/tuition-fees' -Token $hvToken
    $myPage = From-Json-Safe $r.Text
    Assert 'T11.1 GET /students/me/tuition-fees -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")
    $saiChuSoHuu = $false
    if ($null -ne $myPage.items) {
        foreach ($it in $myPage.items) { if ($it.maHocVien -ne 'HV001') { $saiChuSoHuu = $true } }
    }
    Assert 'T11.2 chi tra du lieu cua HV001' (-not $saiChuSoHuu) 'co ban ghi cua hoc vien khac'

    $r = Invoke-Api -Method GET -Path '/api/students/me/payments' -Token $hvToken
    Assert 'T11.3 GET /students/me/payments -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/students/me/receipts' -Token $hvToken
    Assert 'T11.4 GET /students/me/receipts -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")

    # Xem hoc phi cua nguoi khac -> 404 (neu hoc phi test khong thuoc HV001)
    if ($chuSoHuu1 -ne 'HV001') {
        $r = Invoke-Api -Method GET -Path ("/api/students/me/tuition-fees/{0}" -f $maHocPhi1) -Token $hvToken
        Assert 'T11.5 xem hoc phi nguoi khac -> 404' ($r.Status -eq 404) ("HTTP $($r.Status)")

        $r = Invoke-Api -Method GET -Path ("/api/students/me/receipts/{0}" -f $pay1.bienNhan.maBienNhan) -Token $hvToken
        Assert 'T11.6 xem bien nhan nguoi khac -> 404' ($r.Status -eq 404) ("HTTP $($r.Status)")
    } else {
        Warn 'T11.5/T11.6' 'hoc phi test thuoc HV001 -> bo qua kiem tra cheo chu so huu'
    }

    # ========================================================
    # T12 - Dong bo so tien tu dang ky
    # ========================================================
    Write-Log ''
    Write-Log '--- T12: dong bo so tien tu dang ky ---'
    $r = Invoke-Api -Method PATCH -Path ("/api/admin/tuition-fees/{0}" -f $maHocPhi1) -Token $adminToken -Body @{ dongBoTuDangKy = $true }
    Assert 'T12.1 da thanh toan du -> dong bo bi chan 409' ($r.Status -eq 409) ("HTTP $($r.Status): $($r.Text)")

    $r = Invoke-Api -Method PATCH -Path ("/api/admin/tuition-fees/{0}" -f $maHocPhi1) -Token $adminToken -Body @{}
    Assert 'T12.2 body rong -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")

    # ========================================================
    # T13 - Dong thoi (concurrency)
    # ========================================================
    Write-Log ''
    Write-Log '--- T13: ghi nhan dong thoi, khong duoc am so du ---'
    $r = Invoke-Api -Method POST -Path '/api/admin/tuition-fees' -Token $adminToken -Body @{ maDangKy = $maDangKy2 }
    $dto2 = From-Json-Safe $r.Text
    Assert 'T13.1 tao hoc phi thu 2 -> 201' ($r.Status -eq 201) ("HTTP $($r.Status): $($r.Text)")
    $maHocPhi2 = [int]$dto2.maHocPhi
    if ($maHocPhi2 -gt 0) { $testHocPhiIds.Add($maHocPhi2) }

    $tong2 = [decimal]$dto2.soTienPhaiDong
    $moiLan = [math]::Ceiling($tong2 * 0.6)

    $jobScript = {
        param($Uri, $Body, $Token)
        try {
            $headers = @{ Authorization = "Bearer $Token" }
            $resp = Invoke-WebRequest -Uri $Uri -Method POST -Headers $headers `
                -ContentType 'application/json; charset=utf-8' `
                -Body ([System.Text.Encoding]::UTF8.GetBytes($Body)) `
                -UseBasicParsing -ErrorAction Stop
            return [int]$resp.StatusCode
        } catch {
            if ($_.Exception.Response) { return [int]$_.Exception.Response.StatusCode }
            return -1
        }
    }

    $uri2 = "$BaseUrl/api/admin/tuition-fees/$maHocPhi2/payments"
    $bodyA = (@{ maGiaoDich = ('GDTEST-CONC-' + (Get-Date -Format 'HHmmssfff') + '-1'); soTien = $moiLan; phuongThucThanhToan = $S_TienMat } | ConvertTo-Json -Compress)
    $bodyB = (@{ maGiaoDich = ('GDTEST-CONC-' + (Get-Date -Format 'HHmmssfff') + '-2'); soTien = $moiLan; phuongThucThanhToan = $S_TienMat } | ConvertTo-Json -Compress)

    $j1 = Start-Job -ScriptBlock $jobScript -ArgumentList $uri2, $bodyA, $ktToken
    $j2 = Start-Job -ScriptBlock $jobScript -ArgumentList $uri2, $bodyB, $ktToken
    Wait-Job $j1, $j2 -Timeout 120 | Out-Null
    $out1 = @(Receive-Job $j1)
    $out2 = @(Receive-Job $j2)
    $s1 = if ($out1.Count -gt 0) { [string]$out1[-1] } else { 'none' }
    $s2 = if ($out2.Count -gt 0) { [string]$out2[-1] } else { 'none' }
    Remove-Job $j1, $j2 -Force

    Write-Log ("  dong thoi: status1={0}, status2={1}, moiLan={2}, tong={3}" -f $s1, $s2, $moiLan, $tong2)

    $soThanhCong = 0
    if ($s1 -eq '201') { $soThanhCong++ }
    if ($s2 -eq '201') { $soThanhCong++ }

    Assert 'T13.2 chi 1 giao dich duoc chap nhan' ($soThanhCong -eq 1) ("soThanhCong=$soThanhCong (s1=$s1, s2=$s2)")

    $db2 = (Invoke-Query -Sql 'SELECT SoTienDaDong, SoTienGoc, SoTienGiam, SoTienConThieu FROM HocPhi WHERE MaHocPhi = @id' -Params @{ id = $maHocPhi2 }).Rows[0]
    $daDong2 = [decimal]$db2['SoTienDaDong']
    Assert 'T13.3 soTienDaDong khong am' ($daDong2 -ge 0) ("daDong=$daDong2")
    Assert 'T13.4 soTienDaDong <= soTienPhaiDong' ($daDong2 -le ($tong2)) ("daDong=$daDong2 phaiDong=$tong2")
    Assert 'T13.5 soTienDaDong = dung 1 lan thanh toan' ($daDong2 -eq $moiLan) ("daDong=$daDong2 moiLan=$moiLan")

    # ========================================================
    # T14 - Bao cao
    # ========================================================
    Write-Log ''
    Write-Log '--- T14: bao cao hoc phi ---'
    $r = Invoke-Api -Method GET -Path '/api/admin/tuition-fees/reports/summary' -Token $ktToken
    $rep = From-Json-Safe $r.Text
    Assert 'T14.1 GET bao cao -> 200' ($r.Status -eq 200) ("HTTP $($r.Status)")
    Assert 'T14.2 tongPhaiThu = tongDaThu + tongConThieu' (
        [math]::Abs(([decimal]$rep.tongPhaiThu) - ([decimal]$rep.tongDaThu) - ([decimal]$rep.tongConThieu)) -lt 0.01
    ) ("phaiThu=$($rep.tongPhaiThu) daThu=$($rep.tongDaThu) conThieu=$($rep.tongConThieu)")
    Assert 'T14.3 tongPhaiThu > 0' ([decimal]$rep.tongPhaiThu -gt 0) ("tongPhaiThu=$($rep.tongPhaiThu)")

    $r = Invoke-Api -Method GET -Path '/api/admin/tuition-fees/reports/summary' -Token $hvToken
    Assert 'T14.4 hoc vien xem bao cao -> 403' ($r.Status -eq 403) ("HTTP $($r.Status)")

    # ========================================================
    # T15 - Regression Phase 1..5
    # ========================================================
    Write-Log ''
    Write-Log '--- T15: regression Phase 1-5 ---'
    $r = Invoke-Api -Method GET -Path '/api/students/me/grades' -Token $hvToken
    Assert 'T15.1 Phase 5 /students/me/grades van hoat dong' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/students/me/schedule' -Token $hvToken
    Assert 'T15.2 Phase 5 /students/me/schedule van hoat dong' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/enrollments' -Token $adminToken
    Assert 'T15.3 Phase 4 /admin/enrollments van hoat dong' ($r.Status -eq 200) ("HTTP $($r.Status)")

    $r = Invoke-Api -Method GET -Path '/api/admin/tuition-fees?trangThai=KHONG_HOP_LE' -Token $adminToken
    Assert 'T15.4 trang thai loc khong hop le -> 400' ($r.Status -eq 400) ("HTTP $($r.Status)")
}
catch {
    if ($_.Exception.Message -ne 'NOT_ENOUGH_DATA') {
        Write-Log ("  [ERROR] {0}" -f $_.Exception.Message)
        $script:failed++
    }
}
finally {
    # --------------------------------------------------------
    # ROLLBACK: xoa dung du lieu test, khong dung du lieu seed
    # --------------------------------------------------------
    Write-Log ''
    Write-Log '--- DON DEP DU LIEU TEST ---'

    if ($testHocPhiIds.Count -gt 0) {
        $idList = ($testHocPhiIds | ForEach-Object { [string]$_ }) -join ','
        try {
            Invoke-Query -Sql "DELETE FROM BienNhan WHERE MaThanhToan IN (SELECT MaThanhToan FROM ThanhToan WHERE MaHocPhi IN ($idList))" | Out-Null
            Invoke-Query -Sql "DELETE FROM ThanhToan WHERE MaHocPhi IN ($idList)" | Out-Null
            Invoke-Query -Sql "DELETE FROM HocPhi WHERE MaHocPhi IN ($idList)" | Out-Null
            Write-Log ("  da xoa hoc phi test: {0}" -f $idList)
        } catch {
            Write-Log ("  [CANH BAO] don dep that bai: {0}" -f $_.Exception.Message)
            Write-Log ("  hay xoa tay: HocPhi MaHocPhi IN ({0})" -f $idList)
        }
    } else {
        Write-Log '  khong co du lieu test can xoa.'
    }

    if ($testDangKyIds.Count -gt 0) {
        $dkList = (($testDangKyIds | Select-Object -Unique) | ForEach-Object { [string]$_ }) -join ','
        try {
            Invoke-Query -Sql "DELETE FROM DangKyHoc WHERE MaDangKy IN ($dkList)" | Out-Null
            Write-Log ("  da xoa dang ky test: {0}" -f $dkList)
        } catch {
            Write-Log ("  [CANH BAO] khong xoa duoc dang ky test {0}" -f $dkList)
        }
    }

    $conLai = Invoke-Scalar -Sql 'SELECT COUNT(*) FROM HocPhi' 
    Write-Log ("  so dong HocPhi con lai trong DB: {0}" -f $conLai)

    Write-Log ''
    Write-Log '============================================================'
    Write-Log (" KET QUA: PASS = {0} | FAIL = {1} | WARN = {2}" -f $script:passed, $script:failed, $script:warns)
    Write-Log '============================================================'

    $log | Out-File -FilePath $reportPath -Encoding utf8
    Write-Output ("PHASE6_TEST_DONE pass={0} fail={1} warn={2}" -f $script:passed, $script:failed, $script:warns)
}
