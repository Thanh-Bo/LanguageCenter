# ============================================================================
# Phase 3 - Test account management + student/teacher profile
# YEU CAU: API dang chay tai http://localhost:5204
# KHONG bao gio in MatKhauHash / JWT key / full token
# ============================================================================
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName System.Data
Add-Type -AssemblyName System.Net.Http

$BaseUrl = 'http://localhost:5204'
$DevPassword = 'LanguageCenter@2026'
$ConnectionString = 'Server=localhost\SQLEXPRESS;Database=QuanLyTrungTamNgoaiNgu;Integrated Security=True'

# --- Chuoi tieng Viet: PS 5.1 doc .ps1 theo ANSI nen phai tao tu ma Unicode ---
$RoleHocVien      = [string]::Concat('H', [char]0x1ECD, 'c vi', [char]0x00EA, 'n')
$RoleGiaoVien     = [string]::Concat('Gi', [char]0x00E1, 'o vi', [char]0x00EA, 'n')
$RoleQuanTriVien  = [string]::Concat('Qu', [char]0x1EA3, 'n tr', [char]0x1ECB, ' vi', [char]0x00EA, 'n')
$RoleTroGiang     = [string]::Concat('Tr', [char]0x1EE3, ' gi', [char]0x1EA3, 'ng')
$StHoatDong       = [string]::Concat('Ho', [char]0x1EA1, 't ', [char]0x0111, [char]0x1ED9, 'ng')
$StBiKhoa         = [string]::Concat('B', [char]0x1ECB, ' kh', [char]0x00F3, 'a')
$StDangTuVan      = [string]::Concat([char]0x0110, 'ang t', [char]0x01B0, ' v', [char]0x1EA5, 'n')
$TenHocVienMoi    = [string]::Concat('Nguy', [char]0x1EC5, 'n V', [char]0x0103, 'n Test')
$LoaiGiaoVienGV   = $RoleGiaoVien
$LoaiGiaoVienTG   = $RoleTroGiang

$reportPath = Join-Path $PSScriptRoot 'phase3-test-report.txt'
$log = [System.Collections.Generic.List[string]]::new()
$passed = 0
$failed = 0

function Write-Log { param([string]$Text) $log.Add($Text); Write-Output $Text }

function Assert-Case {
    param([string]$CaseId, [string]$Description, $Expected, $Actual)
    $ok = ($Expected -eq $Actual)
    if ($ok) { $script:passed++ } else { $script:failed++ }
    $mark = if ($ok) { 'PASS' } else { 'FAIL' }
    Write-Log ("[{0}] {1} - {2}" -f $mark, $CaseId, $Description)
    Write-Log ("        expected={0}  actual={1}" -f $Expected, $Actual)
}

function Invoke-Api {
    param([string]$Method, [string]$Path, [string]$Body, [string]$Token)
    $headers = @{}
    if ($Token) { $headers['Authorization'] = "Bearer $Token" }
    try {
        $params = @{ Uri = "$BaseUrl$Path"; Method = $Method; Headers = $headers; UseBasicParsing = $true; ErrorAction = 'Stop' }
        if ($Body) {
            $params['ContentType'] = 'application/json; charset=utf-8'
            $params['Body'] = [System.Text.Encoding]::UTF8.GetBytes($Body)
        }
        $r = Invoke-WebRequest @params
        $reader = New-Object System.IO.StreamReader($r.RawContentStream, [System.Text.Encoding]::UTF8)
        return @{ Status = [int]$r.StatusCode; Text = $reader.ReadToEnd() }
    } catch {
        $resp = $_.Exception.Response
        if ($null -eq $resp) { return @{ Status = -1; Text = $_.Exception.Message } }
        $reader = New-Object System.IO.StreamReader($resp.GetResponseStream(), [System.Text.Encoding]::UTF8)
        return @{ Status = [int]$resp.StatusCode; Text = $reader.ReadToEnd() }
    }
}

function Get-SqlScalar {
    param([string]$Sql)
    $c = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
    $c.Open(); $cmd = $c.CreateCommand(); $cmd.CommandText = $Sql
    $v = $cmd.ExecuteScalar(); $c.Close(); return $v
}

function Invoke-Sql {
    param([string]$Sql)
    $c = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
    $c.Open(); $cmd = $c.CreateCommand(); $cmd.CommandText = $Sql
    $n = $cmd.ExecuteNonQuery(); $c.Close(); return $n
}

function Login-Token {
    param([string]$Username, [string]$Password = $DevPassword)
    $body = (@{ username = $Username; password = $Password } | ConvertTo-Json -Compress)
    $r = Invoke-Api -Method 'POST' -Path '/api/auth/login' -Body $body
    if ($r.Status -ne 200) { return $null }
    return ($r.Text | ConvertFrom-Json).accessToken
}

function Login-Status {
    param([string]$Username, [string]$Password = $DevPassword)
    $body = (@{ username = $Username; password = $Password } | ConvertTo-Json -Compress)
    return (Invoke-Api -Method 'POST' -Path '/api/auth/login' -Body $body).Status
}

# Tao nhieu request THAT SU dong thoi bang HttpClient de test concurrency
function Invoke-Concurrent {
    param([string]$Path, [string]$Token, [string[]]$Bodies)
    $client = New-Object System.Net.Http.HttpClient
    $client.DefaultRequestHeaders.Authorization =
        New-Object System.Net.Http.Headers.AuthenticationHeaderValue('Bearer', $Token)

    $tasks = @()
    foreach ($b in $Bodies) {
        $content = New-Object System.Net.Http.StringContent($b, [System.Text.Encoding]::UTF8, 'application/json')
        $tasks += $client.PostAsync("$BaseUrl$Path", $content)
    }

    [System.Threading.Tasks.Task]::WaitAll($tasks)

    $results = @()
    foreach ($t in $tasks) {
        $json = $t.Result.Content.ReadAsStringAsync().Result
        $results += @{ Status = [int]$t.Result.StatusCode; Body = $json }
    }
    $client.Dispose()
    return $results
}

Write-Log '============================================================'
Write-Log ' PHASE 3 - ACCOUNT MANAGEMENT / PROFILE TEST REPORT'
Write-Log (" Thoi diem: " + (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'))
Write-Log '============================================================'
Write-Log ''

# ------------------------------------------------------------
# LAY TOKEN
# ------------------------------------------------------------
Write-Log '--- Lay token ---'
$adminToken = Login-Token 'ADMIN001'
$teacherToken = Login-Token 'GV001'
$studentToken = Login-Token 'HV001'
Write-Log ("  adminToken   : " + [bool]$adminToken)
Write-Log ("  teacherToken : " + [bool]$teacherToken)
Write-Log ("  studentToken : " + [bool]$studentToken)
Assert-Case 'SETUP' 'Lay duoc 3 token' 'True True True' ("$([bool]$adminToken) $([bool]$teacherToken) $([bool]$studentToken)")
Write-Log ''

$stamp = Get-Date -Format 'HHmmss'
$createdStudents = @()
$createdTeachers = @()

# ------------------------------------------------------------
# CASE 1: Admin tao student hop le (khong phu huynh)
# ------------------------------------------------------------
Write-Log '--- CASE 1 / 3: Tao hoc vien (KHONG phu huynh) ---'
$body1 = @{ hoTen = $TenHocVienMoi; ngaySinh = '2008-05-20'; gioiTinh = 'Nam';
            email = "ph3.s1.$stamp@test.local"; soDienThoai = '0901234567'; diaChi = 'Quan 1';
            mucTieuHocTap = 'IELTS 6.5'; trinhDoHienTai = 'A2' } | ConvertTo-Json -Compress
$c1 = Invoke-Api -Method 'POST' -Path '/api/admin/students' -Body $body1 -Token $adminToken
Assert-Case 'CASE 1' 'Tao hoc vien -> 201' 201 $c1.Status

$s1 = $null
if ($c1.Status -eq 201) {
    $s1 = $c1.Text | ConvertFrom-Json
    $createdStudents += $s1.studentId
    Write-Log ("        studentId={0} username={1} role={2}" -f $s1.studentId, $s1.username, $s1.role)
    Write-Log ("        co temporaryPassword? {0}  (do dai {1})" -f [bool]$s1.temporaryPassword, $s1.temporaryPassword.Length)
    Assert-Case 'CASE 1b' 'Ma co dang HVxxx' $true ($s1.studentId -match '^HV\d{3}$')
    Assert-Case 'CASE 1c' 'username == studentId' $s1.studentId $s1.username
    Assert-Case 'CASE 1d' 'Role = Hoc vien' $RoleHocVien $s1.role
    Assert-Case 'CASE 1e' 'Co temporaryPassword' $true ([bool]$s1.temporaryPassword)
    Assert-Case 'CASE 1f' 'KHONG lo MatKhauHash' $false ($c1.Text -like '*MatKhauHash*')

    # Database: TaiKhoan + HocVien + MaHocVien == TenDangNhap
    Assert-Case 'CASE 1g' 'Co TaiKhoan trong DB' 1 (Get-SqlScalar "SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap='$($s1.studentId)'")
    Assert-Case 'CASE 1h' 'Co HocVien trong DB' 1 (Get-SqlScalar "SELECT COUNT(*) FROM HocVien WHERE MaHocVien='$($s1.studentId)'")
    Assert-Case 'CASE 1i' 'MaHocVien == TenDangNhap' 1 (Get-SqlScalar "SELECT COUNT(*) FROM HocVien h JOIN TaiKhoan t ON t.TenDangNhap=h.MaHocVien WHERE h.MaHocVien='$($s1.studentId)'")
    Assert-Case 'CASE 1j' 'TrangThaiTaiKhoan = Hoat dong' $StHoatDong (Get-SqlScalar "SELECT TrangThaiTaiKhoan FROM TaiKhoan WHERE TenDangNhap='$($s1.studentId)'")
    Assert-Case 'CASE 1k' 'TrangThaiHocTap = Dang tu van' $StDangTuVan (Get-SqlScalar "SELECT TrangThaiHocTap FROM HocVien WHERE MaHocVien='$($s1.studentId)'")
    Assert-Case 'CASE 1l' 'MatKhauHash la hash that (khong phai placeholder)' 'AQAAAA' ((Get-SqlScalar "SELECT MatKhauHash FROM TaiKhoan WHERE TenDangNhap='$($s1.studentId)'").Substring(0,6))

    # Login duoc bang mat khau tam
    Assert-Case 'CASE 1m' 'Login bang ma + mat khau tam' 200 (Login-Status $s1.studentId $s1.temporaryPassword)
    Assert-Case 'CASE 1n' 'Login bang mat khau dev -> 401' 401 (Login-Status $s1.studentId $DevPassword)
}
Write-Log ''

# ------------------------------------------------------------
# CASE 2: Tao student CO phu huynh
# ------------------------------------------------------------
Write-Log '--- CASE 2: Tao hoc vien CO phu huynh ---'
$body2 = @{ hoTen = "Ph3 Student Two $stamp"; ngaySinh = '2010-01-15'; email = "ph3.s2.$stamp@test.local";
            phuHuynh = @{ hoTen = "Ph3 Parent $stamp"; soDienThoai = '0907654321'; email = "ph3.p.$stamp@test.local";
                          diaChi = 'Quan 3'; ngheNghiep = 'Bac si'; moiQuanHe = 'Cha' } } | ConvertTo-Json -Compress
$c2 = Invoke-Api -Method 'POST' -Path '/api/admin/students' -Body $body2 -Token $adminToken
Assert-Case 'CASE 2' 'Tao hoc vien co phu huynh -> 201' 201 $c2.Status
if ($c2.Status -eq 201) {
    $s2 = $c2.Text | ConvertFrom-Json
    $createdStudents += $s2.studentId
    Assert-Case 'CASE 2b' 'HocVien co MaPhuHuynh' 1 (Get-SqlScalar "SELECT COUNT(*) FROM HocVien WHERE MaHocVien='$($s2.studentId)' AND MaPhuHuynh IS NOT NULL")
    Assert-Case 'CASE 2c' 'MoiQuanHePhuHuynh duoc luu' 1 (Get-SqlScalar "SELECT COUNT(*) FROM HocVien WHERE MaHocVien='$($s2.studentId)' AND MoiQuanHePhuHuynh IS NOT NULL")
    # Mo hinh 1-N: phu huynh moi chi co 1 hoc vien
    Assert-Case 'CASE 2d' 'PhuHuynh moi chi co 1 hoc vien' 1 (Get-SqlScalar "SELECT COUNT(*) FROM HocVien WHERE MaPhuHuynh=(SELECT MaPhuHuynh FROM HocVien WHERE MaHocVien='$($s2.studentId)')")
}

# Thieu moiQuanHe -> phai 400 (CHECK CK_HocVien_PhuHuynh)
$body2b = @{ hoTen = "Ph3 NoRelation $stamp"; ngaySinh = '2010-01-15'; email = "ph3.s2b.$stamp@test.local";
             phuHuynh = @{ hoTen = "Ph3 Parent B $stamp" } } | ConvertTo-Json -Compress
Assert-Case 'CASE 2e' 'Co phu huynh nhung thieu moiQuanHe -> 400' 400 (Invoke-Api -Method 'POST' -Path '/api/admin/students' -Body $body2b -Token $adminToken).Status
Write-Log ''

# ------------------------------------------------------------
# CASE 4: Email da ton tai -> 409
# ------------------------------------------------------------
Write-Log '--- CASE 4: Email trung ---'
$bodyDup = @{ hoTen = "Ph3 Dup $stamp"; ngaySinh = '2009-02-02'; email = "ph3.s1.$stamp@test.local" } | ConvertTo-Json -Compress
Assert-Case 'CASE 4' 'Email da ton tai -> 409' 409 (Invoke-Api -Method 'POST' -Path '/api/admin/students' -Body $bodyDup -Token $adminToken).Status
Write-Log ''

# ------------------------------------------------------------
# CASE 5: Giao vien goi POST /api/admin/students -> 403
# ------------------------------------------------------------
Write-Log '--- CASE 5: Phan quyen ---'
$bodyForbidden = @{ hoTen = 'Hacker Test'; ngaySinh = '2009-02-02'; email = "ph3.hack.$stamp@test.local" } | ConvertTo-Json -Compress
Assert-Case 'CASE 5' 'Giao vien tao hoc vien -> 403' 403 (Invoke-Api -Method 'POST' -Path '/api/admin/students' -Body $bodyForbidden -Token $teacherToken).Status
Assert-Case 'CASE 5b' 'Hoc vien tao hoc vien -> 403' 403 (Invoke-Api -Method 'POST' -Path '/api/admin/students' -Body $bodyForbidden -Token $studentToken).Status
Assert-Case 'CASE 5c' 'Khong token tao hoc vien -> 401' 401 (Invoke-Api -Method 'POST' -Path '/api/admin/students' -Body $bodyForbidden).Status
Write-Log ''

# ------------------------------------------------------------
# CASE 6: 2+ request tao student DONG THOI -> khong trung ma
# ------------------------------------------------------------
Write-Log '--- CASE 6: Tao hoc vien DONG THOI (concurrency) ---'
$maxBefore = Get-SqlScalar "SELECT MAX(CAST(SUBSTRING(MaHocVien,3,10) AS INT)) FROM HocVien WHERE MaHocVien LIKE 'HV[0-9][0-9][0-9]'"
Write-Log ("        Max HV truoc khi test: $maxBefore")

$bodies = @()
foreach ($i in 1..5) {
    $bodies += (@{ hoTen = "Ph3 Concurrent $i $stamp"; ngaySinh = '2010-06-06';
                   email = "ph3.cc.$i.$stamp@test.local" } | ConvertTo-Json -Compress)
}
$concurrent = Invoke-Concurrent -Path '/api/admin/students' -Token $adminToken -Bodies $bodies

$statuses = ($concurrent | ForEach-Object { $_.Status }) -join ','
$codes = @()
foreach ($r in $concurrent) {
    if ($r.Status -eq 201) { $codes += (($r.Body | ConvertFrom-Json).studentId) }
}
Write-Log ("        Status cac request: $statuses")
Write-Log ("        Ma sinh ra: " + ($codes -join ', '))
Write-Log ("        So request thanh cong: " + $codes.Count)

$uniqueCodes = @($codes | Sort-Object -Unique)
Assert-Case 'CASE 6' 'Tat ca request thanh cong' 5 $codes.Count
Assert-Case 'CASE 6b' 'KHONG co ma trung' $codes.Count $uniqueCodes.Count
Assert-Case 'CASE 6c' 'DB khong co ma trung' 0 (Get-SqlScalar "SELECT COUNT(*) FROM (SELECT MaHocVien FROM HocVien GROUP BY MaHocVien HAVING COUNT(*) > 1) x")
$createdStudents += $codes
Write-Log ''

# ------------------------------------------------------------
# CASE 7 + 8: Tao Giao vien (GVxxx) va Tro giang (TGxxx)
# ------------------------------------------------------------
Write-Log '--- CASE 7/8: Tao giao vien va tro giang ---'
$gvBody = @{ hoTen = "Ph3 Teacher $stamp"; ngaySinh = '1995-03-03'; gioiTinh = 'N';
             email = "ph3.gv.$stamp@test.local"; soDienThoai = '0912345678';
             loaiGiaoVien = $LoaiGiaoVienGV; chuyenMon = 'IELTS Speaking'; soNamKinhNghiem = 5;
             ngayVaoLam = '2026-01-01' } | ConvertTo-Json -Compress
$c7 = Invoke-Api -Method 'POST' -Path '/api/admin/teachers' -Body $gvBody -Token $adminToken
Assert-Case 'CASE 7' 'Tao giao vien -> 201' 201 $c7.Status
if ($c7.Status -eq 201) {
    $gv = $c7.Text | ConvertFrom-Json
    $createdTeachers += $gv.teacherId
    Write-Log ("        teacherId={0} loai={1} role={2}" -f $gv.teacherId, $gv.loaiGiaoVien, $gv.role)
    Assert-Case 'CASE 7b' 'Ma co dang GVxxx' $true ($gv.teacherId -match '^GV\d{3}$')
    Assert-Case 'CASE 7c' 'Role = Giao vien' $RoleGiaoVien $gv.role
    Assert-Case 'CASE 7d' 'Ma == TenDangNhap' 1 (Get-SqlScalar "SELECT COUNT(*) FROM GiaoVien g JOIN TaiKhoan t ON t.TenDangNhap=g.MaGiaoVien WHERE g.MaGiaoVien='$($gv.teacherId)'")
    Assert-Case 'CASE 7e' 'LoaiGiaoVien trong DB' $LoaiGiaoVienGV (Get-SqlScalar "SELECT LoaiGiaoVien FROM GiaoVien WHERE MaGiaoVien='$($gv.teacherId)'")
}

$tgBody = @{ hoTen = "Ph3 Assistant $stamp"; email = "ph3.tg.$stamp@test.local";
             loaiGiaoVien = $LoaiGiaoVienTG; soNamKinhNghiem = 1 } | ConvertTo-Json -Compress
$c8 = Invoke-Api -Method 'POST' -Path '/api/admin/teachers' -Body $tgBody -Token $adminToken
Assert-Case 'CASE 8' 'Tao tro giang -> 201' 201 $c8.Status
if ($c8.Status -eq 201) {
    $tg = $c8.Text | ConvertFrom-Json
    $createdTeachers += $tg.teacherId
    Assert-Case 'CASE 8b' 'Ma co dang TGxxx' $true ($tg.teacherId -match '^TG\d{3}$')
    Assert-Case 'CASE 8c' 'Role = Tro giang' $RoleTroGiang $tg.role
}

# LoaiGiaoVien khong hop le -> 400
$badBody = @{ hoTen = 'Bad Type'; email = "ph3.bad.$stamp@test.local"; loaiGiaoVien = 'Teacher' } | ConvertTo-Json -Compress
Assert-Case 'CASE 8d' 'LoaiGiaoVien khong hop le -> 400' 400 (Invoke-Api -Method 'POST' -Path '/api/admin/teachers' -Body $badBody -Token $adminToken).Status
Assert-Case 'CASE 8e' 'Giao vien tao giao vien -> 403' 403 (Invoke-Api -Method 'POST' -Path '/api/admin/teachers' -Body $gvBody -Token $teacherToken).Status
Write-Log ''

# ------------------------------------------------------------
# CASE 9: Tao giao vien DONG THOI
# ------------------------------------------------------------
Write-Log '--- CASE 9: Tao GV/TG DONG THOI ---'
$gvBodies = @()
foreach ($i in 1..4) {
    $gvBodies += (@{ hoTen = "Ph3 Cc Teacher $i $stamp"; email = "ph3.ccgv.$i.$stamp@test.local";
                     loaiGiaoVien = $LoaiGiaoVienGV } | ConvertTo-Json -Compress)
}
$gvConcurrent = Invoke-Concurrent -Path '/api/admin/teachers' -Token $adminToken -Bodies $gvBodies
$gvCodes = @()
foreach ($r in $gvConcurrent) { if ($r.Status -eq 201) { $gvCodes += (($r.Body | ConvertFrom-Json).teacherId) } }
Write-Log ("        Ma GV sinh ra: " + ($gvCodes -join ', '))
Assert-Case 'CASE 9' 'Tat ca request GV thanh cong' 4 $gvCodes.Count
Assert-Case 'CASE 9b' 'KHONG trung ma GV' $gvCodes.Count (@($gvCodes | Sort-Object -Unique).Count)
$createdTeachers += $gvCodes
Write-Log ''

$log | Set-Content -Path $reportPath -Encoding utf8
Write-Log ''
Write-Log (" TAM DUNG - PASS = {0} / FAIL = {1}" -f $passed, $failed)
$log | Set-Content -Path $reportPath -Encoding utf8
