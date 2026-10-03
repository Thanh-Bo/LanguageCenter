# ============================================================
# Phase 2 - Chay toan bo test case authentication / authorization.
# YEU CAU: API dang chay tai http://localhost:5204
# Khong bao gio in MatKhauHash hay JWT signing key ra output.
# ============================================================
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName System.Data

$BaseUrl = 'http://localhost:5204'
$DevPassword = 'LanguageCenter@2026'
$NewPassword = 'MatKhauMoi@2026'
$ConnectionString = 'Server=localhost\SQLEXPRESS;Database=QuanLyTrungTamNgoaiNgu;Integrated Security=True'

$reportPath = Join-Path $PSScriptRoot 'phase2-test-report.txt'
$log = [System.Collections.Generic.List[string]]::new()
$passed = 0
$failed = 0

# ------------------------------------------------------------
# QUAN TRONG: Windows PowerShell 5.1 doc file .ps1 theo codepage ANSI,
# nen KHONG duoc viet literal tieng Viet trong script (se bi mojibake va
# CHECK constraint CK_TaiKhoan_TrangThai se tu choi gia tri).
# Cac chuoi trang thai duoc tao tu ma Unicode -> dung 100%.
# ------------------------------------------------------------
$TrangThaiHoatDong = [string]::Concat('Ho', [char]0x1EA1, 't ', [char]0x0111, [char]0x1ED9, 'ng')
$TrangThaiBiKhoa = [string]::Concat('B', [char]0x1ECB, ' kh', [char]0x00F3, 'a')

function Write-Log { param([string]$Text) $log.Add($Text); Write-Output $Text }

function Invoke-Api {
    param([string]$Method, [string]$Path, [string]$Body, [string]$Token)
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
        if ($Body) {
            $params['ContentType'] = 'application/json; charset=utf-8'
            $params['Body'] = [System.Text.Encoding]::UTF8.GetBytes($Body)
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

function Invoke-Sql {
    param([string]$Sql)
    $connection = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = $Sql
    $rows = $command.ExecuteNonQuery()
    $connection.Close()
    return $rows
}

function Get-SqlScalar {
    param([string]$Sql)
    $connection = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = $Sql
    $value = $command.ExecuteScalar()
    $connection.Close()
    return $value
}

function Set-TrangThaiTaiKhoan {
    param([string]$TenDangNhap, [string]$TrangThai)
    $connection = New-Object System.Data.SqlClient.SqlConnection $ConnectionString
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = 'UPDATE TaiKhoan SET TrangThaiTaiKhoan = @TrangThai WHERE TenDangNhap = @TenDangNhap'
    $p1 = New-Object System.Data.SqlClient.SqlParameter('@TrangThai', ([System.Data.SqlDbType]::NVarChar), 50)
    $p1.Value = $TrangThai
    $command.Parameters.Add($p1) | Out-Null
    $p2 = New-Object System.Data.SqlClient.SqlParameter('@TenDangNhap', ([System.Data.SqlDbType]::VarChar), 20)
    $p2.Value = $TenDangNhap
    $command.Parameters.Add($p2) | Out-Null
    $rows = $command.ExecuteNonQuery()
    $connection.Close()
    return $rows
}

function Assert-Case {
    param([string]$CaseId, [string]$Description, $Expected, $Actual)
    $ok = ($Expected -eq $Actual)
    if ($ok) { $script:passed++ } else { $script:failed++ }
    $mark = if ($ok) { 'PASS' } else { 'FAIL' }
    Write-Log ("[{0}] {1} - {2}" -f $mark, $CaseId, $Description)
    Write-Log ("        expected={0}  actual={1}" -f $Expected, $Actual)
}

function Get-TokenFromLogin {
    param([string]$Username, [string]$Password)
    $body = (@{ username = $Username; password = $Password } | ConvertTo-Json -Compress)
    $result = Invoke-Api -Method 'POST' -Path '/api/auth/login' -Body $body
    if ($result.Status -ne 200) { return $null }
    $json = $result.Text | ConvertFrom-Json
    return $json.accessToken
}

function Login-Status {
    param([string]$Username, [string]$Password)
    $body = (@{ username = $Username; password = $Password } | ConvertTo-Json -Compress)
    return (Invoke-Api -Method 'POST' -Path '/api/auth/login' -Body $body).Status
}

Write-Log '============================================================'
Write-Log ' PHASE 2 - AUTHENTICATION / AUTHORIZATION TEST REPORT'
Write-Log (" Thoi diem: " + (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'))
Write-Log '============================================================'
Write-Log ''

# ------------------------------------------------------------
# CASE 1: username dung + password dung -> 200 + co JWT
# ------------------------------------------------------------
Write-Log '--- CASE 1: Dang nhap dung ---'
$loginBody = (@{ username = 'ADMIN001'; password = $DevPassword } | ConvertTo-Json -Compress)
$case1 = Invoke-Api -Method 'POST' -Path '/api/auth/login' -Body $loginBody
Assert-Case 'CASE 1' 'ADMIN001 + password dung' 200 $case1.Status

$adminJson = $null
$adminToken = $null
if ($case1.Status -eq 200) {
    $adminJson = $case1.Text | ConvertFrom-Json
    $adminToken = $adminJson.accessToken
    $claims = $adminToken.Split('.')[1]
    $claims = $claims.Replace('-', '+').Replace('_', '/')
    switch ($claims.Length % 4) { 2 { $claims += '==' } 3 { $claims += '=' } }
    $payload = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($claims)) | ConvertFrom-Json

    Write-Log ("        tokenType={0}  expiresAt={1}" -f $adminJson.tokenType, $adminJson.expiresAt)
    Write-Log ("        user.id={0}  user.username={1}  user.role={2}" -f $adminJson.user.id, $adminJson.user.username, $adminJson.user.role)
    Write-Log ("        JWT claims: sub={0}  unique_name={1}  role={2}  jti={3}" -f $payload.sub, $payload.unique_name, $payload.role, $(if ($payload.jti) { 'present' } else { 'MISSING' }))
    Write-Log ("        token co MatKhauHash? {0}" -f ($case1.Text -like '*MatKhauHash*'))
    Assert-Case 'CASE 1b' 'JWT co claim sub' $true ([bool]$payload.sub)
    Assert-Case 'CASE 1c' 'JWT co claim unique_name' $true ([bool]$payload.unique_name)
    Assert-Case 'CASE 1d' 'JWT co claim role' $true ([bool]$payload.role)
    Assert-Case 'CASE 1e' 'JWT co claim jti' $true ([bool]$payload.jti)
    Assert-Case 'CASE 1f' 'Response KHONG lo MatKhauHash' $false ($case1.Text -like '*MatKhauHash*')
}
Write-Log ''

# ------------------------------------------------------------
# CASE 2: username sai -> 401
# ------------------------------------------------------------
Write-Log '--- CASE 2: Username sai ---'
Assert-Case 'CASE 2' 'Username khong ton tai' 401 (Login-Status 'KHONGTONTAI' $DevPassword)
Write-Log ''

# ------------------------------------------------------------
# CASE 3: password sai -> 401
# ------------------------------------------------------------
Write-Log '--- CASE 3: Password sai ---'
Assert-Case 'CASE 3' 'ADMIN001 + password sai' 401 (Login-Status 'ADMIN001' 'SaiMatKhau@999')
Write-Log ''

# ------------------------------------------------------------
# CASE 4: account Bi khoa -> 401 (tam doi HV003, sau do khoi phuc)
# ------------------------------------------------------------
Write-Log '--- CASE 4: Tai khoan bi khoa ---'
$statusBefore = Get-SqlScalar "SELECT TrangThaiTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'HV003'"
Write-Log ("        Trang thai HV003 truoc khi test: {0}" -f $statusBefore)

Set-TrangThaiTaiKhoan -TenDangNhap 'HV003' -TrangThai $TrangThaiBiKhoa | Out-Null
$statusLocked = Get-SqlScalar "SELECT TrangThaiTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'HV003'"
Write-Log ("        Da tam doi HV003 thanh: {0}" -f $statusLocked)

Assert-Case 'CASE 4' 'HV003 (Bi khoa) dang nhap' 401 (Login-Status 'HV003' $DevPassword)

# Khoi phuc ngay
Set-TrangThaiTaiKhoan -TenDangNhap 'HV003' -TrangThai $TrangThaiHoatDong | Out-Null
$statusAfter = Get-SqlScalar "SELECT TrangThaiTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'HV003'"
Write-Log ("        DA KHOI PHUC HV003 ve: {0}" -f $statusAfter)
Assert-Case 'CASE 4b' 'HV003 duoc khoi phuc ve trang thai Hoat dong' $TrangThaiHoatDong $statusAfter
Assert-Case 'CASE 4c' 'HV003 dang nhap lai duoc sau khoi phuc' 200 (Login-Status 'HV003' $DevPassword)
Write-Log ''

# ------------------------------------------------------------
# CASE 5: khong co token goi /api/auth/me -> 401
# ------------------------------------------------------------
Write-Log '--- CASE 5: /api/auth/me khong co token ---'
Assert-Case 'CASE 5' 'GET /api/auth/me khong token' 401 (Invoke-Api -Method 'GET' -Path '/api/auth/me').Status
Write-Log ''

# ------------------------------------------------------------
# CASE 6: token hop le goi /api/auth/me -> 200 + dung user
# ------------------------------------------------------------
Write-Log '--- CASE 6: /api/auth/me voi token hop le ---'
$me = Invoke-Api -Method 'GET' -Path '/api/auth/me' -Token $adminToken
Assert-Case 'CASE 6' 'GET /api/auth/me voi token ADMIN001' 200 $me.Status
if ($me.Status -eq 200) {
    Write-Log ("        body: " + $me.Text)
    $meJson = $me.Text | ConvertFrom-Json
    Assert-Case 'CASE 6b' 'Tra dung user ADMIN001' 'ADMIN001' $meJson.username
}
Write-Log ''

# ------------------------------------------------------------
# CASE 6c: khong the gia danh nguoi khac bang query string
# ------------------------------------------------------------
Write-Log '--- CASE 6c: Thu gia danh bang query string ---'
$fake = Invoke-Api -Method 'GET' -Path '/api/auth/me?userId=1&username=ADMIN001' -Token (Get-TokenFromLogin 'GV001' $DevPassword)
if ($fake.Status -eq 200) {
    $fakeJson = $fake.Text | ConvertFrom-Json
    Assert-Case 'CASE 6c' 'userId tren query KHONG thay doi danh tinh' 'GV001' $fakeJson.username
} else {
    Assert-Case 'CASE 6c' 'Query string khong lam doi danh tinh' 200 $fake.Status
}
Write-Log ''

# ------------------------------------------------------------
# CASE 7 & 8: role authorization
# ------------------------------------------------------------
Write-Log '--- CASE 7/8: Role authorization ---'
$teacherToken = Get-TokenFromLogin 'GV001' $DevPassword

Assert-Case 'CASE 7' 'Giao vien goi admin-only' 403 (Invoke-Api -Method 'GET' -Path '/api/development/admin-only' -Token $teacherToken).Status
Assert-Case 'CASE 7b' 'Giao vien goi teacher-only' 200 (Invoke-Api -Method 'GET' -Path '/api/development/teacher-only' -Token $teacherToken).Status
Assert-Case 'CASE 8' 'Quan tri vien goi admin-only' 200 (Invoke-Api -Method 'GET' -Path '/api/development/admin-only' -Token $adminToken).Status
Assert-Case 'CASE 8b' 'Quan tri vien goi teacher-only (khong phai Giao vien)' 403 (Invoke-Api -Method 'GET' -Path '/api/development/teacher-only' -Token $adminToken).Status
Assert-Case 'CASE 8c' 'Khong token goi admin-only' 401 (Invoke-Api -Method 'GET' -Path '/api/development/admin-only').Status
Assert-Case 'CASE 8d' 'Hoc vien goi admin-only' 403 (Invoke-Api -Method 'GET' -Path '/api/development/admin-only' -Token (Get-TokenFromLogin 'HV001' $DevPassword)).Status
Assert-Case 'CASE 8e' 'Tro giang goi teacher-only' 403 (Invoke-Api -Method 'GET' -Path '/api/development/teacher-only' -Token (Get-TokenFromLogin 'TG001' $DevPassword)).Status
Write-Log ''

# ------------------------------------------------------------
# CASE 9: token bi sua -> 401
# ------------------------------------------------------------
Write-Log '--- CASE 9: Token bi chinh sua ---'
$parts = $adminToken.Split('.')
$tamperedPayload = if ($parts[1].EndsWith('A')) { $parts[1].Substring(0, $parts[1].Length - 1) + 'B' } else { $parts[1].Substring(0, $parts[1].Length - 1) + 'A' }
$tamperedToken = "$($parts[0]).$tamperedPayload.$($parts[2])"
Assert-Case 'CASE 9' 'Token bi sua payload' 401 (Invoke-Api -Method 'GET' -Path '/api/auth/me' -Token $tamperedToken).Status
Assert-Case 'CASE 9b' 'Chu ky bi sua' 401 (Invoke-Api -Method 'GET' -Path '/api/auth/me' -Token ($adminToken.Substring(0, $adminToken.Length - 4) + 'AAAA')).Status
Assert-Case 'CASE 9c' 'Token rac' 401 (Invoke-Api -Method 'GET' -Path '/api/auth/me' -Token 'khong.phai.token').Status
Write-Log ''

# ------------------------------------------------------------
# CASE 11: change-password voi current password sai
# ------------------------------------------------------------
Write-Log '--- CASE 11: Doi mat khau voi current password sai ---'
$hv004Token = Get-TokenFromLogin 'HV004' $DevPassword
Assert-Case 'CASE 11a' 'Lay duoc token HV004' $true ([bool]$hv004Token)

$wrongBody = (@{ currentPassword = 'SaiMatKhau@999'; newPassword = $NewPassword; confirmPassword = $NewPassword } | ConvertTo-Json -Compress)
Assert-Case 'CASE 11' 'currentPassword sai' 400 (Invoke-Api -Method 'POST' -Path '/api/auth/change-password' -Body $wrongBody -Token $hv004Token).Status

$mismatchBody = (@{ currentPassword = $DevPassword; newPassword = $NewPassword; confirmPassword = 'KhacNhau@2026' } | ConvertTo-Json -Compress)
Assert-Case 'CASE 11b' 'confirmPassword khong khop' 400 (Invoke-Api -Method 'POST' -Path '/api/auth/change-password' -Body $mismatchBody -Token $hv004Token).Status

$weakBody = (@{ currentPassword = $DevPassword; newPassword = 'abc'; confirmPassword = 'abc' } | ConvertTo-Json -Compress)
Assert-Case 'CASE 11c' 'Mat khau moi vi pham policy (qua ngan)' 400 (Invoke-Api -Method 'POST' -Path '/api/auth/change-password' -Body $weakBody -Token $hv004Token).Status

$sameBody = (@{ currentPassword = $DevPassword; newPassword = $DevPassword; confirmPassword = $DevPassword } | ConvertTo-Json -Compress)
Assert-Case 'CASE 11d' 'Mat khau moi trung mat khau cu' 400 (Invoke-Api -Method 'POST' -Path '/api/auth/change-password' -Body $sameBody -Token $hv004Token).Status

Assert-Case 'CASE 11e' 'Doi mat khau khong co token' 401 (Invoke-Api -Method 'POST' -Path '/api/auth/change-password' -Body $wrongBody).Status
Write-Log ''

# ------------------------------------------------------------
# CASE 12: change-password thanh cong
# ------------------------------------------------------------
Write-Log '--- CASE 12: Doi mat khau thanh cong (HV004) ---'
$changeBody = (@{ currentPassword = $DevPassword; newPassword = $NewPassword; confirmPassword = $NewPassword } | ConvertTo-Json -Compress)
Assert-Case 'CASE 12' 'Doi mat khau HV004' 204 (Invoke-Api -Method 'POST' -Path '/api/auth/change-password' -Body $changeBody -Token $hv004Token).Status

Assert-Case 'CASE 12b' 'Mat khau CU khong dang nhap duoc nua' 401 (Login-Status 'HV004' $DevPassword)
Assert-Case 'CASE 12c' 'Mat khau MOI dang nhap duoc' 200 (Login-Status 'HV004' $NewPassword)

# Khoi phuc mat khau dev NGAY TRONG test (khong can cong cu ngoai) -> test chay lai duoc nhieu lan
$hv004NewToken = Get-TokenFromLogin 'HV004' $NewPassword
$restoreBody = (@{ currentPassword = $NewPassword; newPassword = $DevPassword; confirmPassword = $DevPassword } | ConvertTo-Json -Compress)
Assert-Case 'CASE 12d' 'Khoi phuc mat khau dev cho HV004' 204 (Invoke-Api -Method 'POST' -Path '/api/auth/change-password' -Body $restoreBody -Token $hv004NewToken).Status
Assert-Case 'CASE 12e' 'HV004 dang nhap lai bang mat khau dev' 200 (Login-Status 'HV004' $DevPassword)
Write-Log ''

# ------------------------------------------------------------
# Phase 1 regression
# ------------------------------------------------------------
Write-Log '--- REGRESSION: Phase 1 endpoints ---'
$health = Invoke-Api -Method 'GET' -Path '/health'
Assert-Case 'REG-1' 'GET /health' 200 $health.Status
Write-Log ("        body: " + $health.Text)

$healthDb = Invoke-Api -Method 'GET' -Path '/health/database'
Assert-Case 'REG-2' 'GET /health/database' 200 $healthDb.Status
Write-Log ("        body: " + $healthDb.Text)

$devInfo = Invoke-Api -Method 'GET' -Path '/api/development/database-info'
Assert-Case 'REG-3' 'GET /api/development/database-info' 200 $devInfo.Status
Write-Log ("        body: " + $devInfo.Text)
Write-Log ''

Write-Log '============================================================'
Write-Log (" KET QUA: PASS = {0} / FAIL = {1}" -f $passed, $failed)
Write-Log '============================================================'

$log | Set-Content -Path $reportPath -Encoding utf8
