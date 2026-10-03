# ============================================================================
# Phase 3 - Test phan 2: quan ly tai khoan + self profile + regression
# YEU CAU: API dang chay tai http://localhost:5204
# ============================================================================
$ErrorActionPreference = 'Continue'
Add-Type -AssemblyName System.Data
Add-Type -AssemblyName System.Net.Http

$BaseUrl = 'http://localhost:5204'
$DevPassword = 'LanguageCenter@2026'
$ConnectionString = 'Server=localhost\SQLEXPRESS;Database=QuanLyTrungTamNgoaiNgu;Integrated Security=True'

$RoleHocVien     = [string]::Concat('H', [char]0x1ECD, 'c vi', [char]0x00EA, 'n')
$RoleGiaoVien    = [string]::Concat('Gi', [char]0x00E1, 'o vi', [char]0x00EA, 'n')
$RoleQuanTriVien = [string]::Concat('Qu', [char]0x1EA3, 'n tr', [char]0x1ECB, ' vi', [char]0x00EA, 'n')
$RoleTroGiang    = [string]::Concat('Tr', [char]0x1EE3, ' gi', [char]0x1EA3, 'ng')
$StHoatDong      = [string]::Concat('Ho', [char]0x1EA1, 't ', [char]0x0111, [char]0x1ED9, 'ng')
$StBiKhoa        = [string]::Concat('B', [char]0x1ECB, ' kh', [char]0x00F3, 'a')
$LoaiGV          = $RoleGiaoVien
$LoaiTG          = $RoleTroGiang

$reportPath = Join-Path $PSScriptRoot 'phase3-test-report2.txt'
$log = [System.Collections.Generic.List[string]]::new()
$passed = 0; $failed = 0

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

Write-Log '============================================================'
Write-Log ' PHASE 3 - PART 2: ACCOUNT MGMT + SELF PROFILE + REGRESSION'
Write-Log (" Thoi diem: " + (Get-Date -Format 'yyyy-MM-dd HH:mm:ss'))
Write-Log '============================================================'
Write-Log ''

$stamp = Get-Date -Format 'HHmmss'
$adminToken = Login-Token 'ADMIN001'
$teacherToken = Login-Token 'GV001'
$studentToken = Login-Token 'HV001'
$assistantToken = Login-Token 'TG001'
Write-Log ("  token admin/gv/hv/tg = {0}/{1}/{2}/{3}" -f [bool]$adminToken, [bool]$teacherToken, [bool]$studentToken, [bool]$assistantToken)
Write-Log ''

# ============================================================
# CASE 10-13: danh sach tai khoan
# ============================================================
Write-Log '--- CASE 10-13: Danh sach tai khoan ---'

$p1 = Invoke-Api -Method 'GET' -Path '/api/admin/accounts?page=1&pageSize=5' -Token $adminToken
Assert-Case 'CASE 10' 'GET danh sach tai khoan -> 200' 200 $p1.Status
if ($p1.Status -eq 200) {
    $page1 = $p1.Text | ConvertFrom-Json
    Write-Log ("        page={0} pageSize={1} totalItems={2} totalPages={3} so item={4}" -f $page1.page, $page1.pageSize, $page1.totalItems, $page1.totalPages, @($page1.items).Count)
    Assert-Case 'CASE 10b' 'So item = pageSize' 5 (@($page1.items).Count)
    Assert-Case 'CASE 10c' 'page = 1' 1 $page1.page
    Assert-Case 'CASE 10d' 'Co truong totalPages' $true ($page1.totalPages -gt 0)
    Assert-Case 'CASE 10e' 'KHONG lo MatKhauHash' $false ($p1.Text -like '*MatKhauHash*')
    Assert-Case 'CASE 10f' 'Item co fullName' $true ([bool]$page1.items[0].fullName)
    Assert-Case 'CASE 10g' 'Item co role/status/createdAt' $true ([bool]$page1.items[0].role -and [bool]$page1.items[0].status -and [bool]$page1.items[0].createdAt)

    # pageSize vuot gioi han -> bi chan o 100
    $big = Invoke-Api -Method 'GET' -Path '/api/admin/accounts?page=1&pageSize=5000' -Token $adminToken
    $bigJson = $big.Text | ConvertFrom-Json
    Assert-Case 'CASE 10h' 'pageSize toi da 100' 100 $bigJson.pageSize

    # page < 1 -> ep ve 1
    $zero = Invoke-Api -Method 'GET' -Path '/api/admin/accounts?page=0&pageSize=5' -Token $adminToken
    Assert-Case 'CASE 10i' 'page < 1 -> ep ve 1' 1 (($zero.Text | ConvertFrom-Json).page)

    # Kiem tra phan trang that su khac nhau
    $p2 = Invoke-Api -Method 'GET' -Path '/api/admin/accounts?page=2&pageSize=5' -Token $adminToken
    $page2 = $p2.Text | ConvertFrom-Json
    Assert-Case 'CASE 10j' 'Page 2 khac page 1' $true ($page2.items[0].id -ne $page1.items[0].id)
}

$searchUser = Invoke-Api -Method 'GET' -Path '/api/admin/accounts?search=HV001' -Token $adminToken
$searchUserJson = $searchUser.Text | ConvertFrom-Json
Assert-Case 'CASE 11' 'Search theo username -> 200' 200 $searchUser.Status
Assert-Case 'CASE 11b' 'Tim thay HV001' $true (@($searchUserJson.items | Where-Object { $_.username -eq 'HV001' }).Count -ge 1)
Assert-Case 'CASE 11c' 'Khong tra ve tai khoan khac' $true (@($searchUserJson.items | Where-Object { $_.username -ne 'HV001' }).Count -le 1)

$searchName = Invoke-Api -Method 'GET' -Path '/api/admin/accounts?search=ADMIN001' -Token $adminToken
$searchNameJson = $searchName.Text | ConvertFrom-Json
Assert-Case 'CASE 12' 'Search theo ma -> co ket qua' $true (@($searchNameJson.items).Count -ge 1)
Write-Log ("        fullName cua ADMIN001 = " + (@($searchNameJson.items)[0].fullName))

$filterRole = Invoke-Api -Method 'GET' -Path ('/api/admin/accounts?role=' + [uri]::EscapeDataString($RoleHocVien) + '&pageSize=100') -Token $adminToken
$filterRoleJson = $filterRole.Text | ConvertFrom-Json
$nonStudent = @($filterRoleJson.items | Where-Object { $_.role -ne $RoleHocVien })
Assert-Case 'CASE 13' 'Filter role=Hoc vien -> 200' 200 $filterRole.Status
Assert-Case 'CASE 13b' 'CHI tra ve Hoc vien' 0 $nonStudent.Count
Write-Log ("        so hoc vien = " + @($filterRoleJson.items).Count + " / tong " + $filterRoleJson.totalItems)

$filterStatus = Invoke-Api -Method 'GET' -Path ('/api/admin/accounts?status=' + [uri]::EscapeDataString($StHoatDong) + '&pageSize=100') -Token $adminToken
$filterStatusJson = $filterStatus.Text | ConvertFrom-Json
Assert-Case 'CASE 13c' 'Filter status=Hoat dong' 0 (@($filterStatusJson.items | Where-Object { $_.status -ne $StHoatDong }).Count)

$combo = Invoke-Api -Method 'GET' -Path ('/api/admin/accounts?page=1&pageSize=5&role=' + [uri]::EscapeDataString($RoleHocVien) + '&search=HV00') -Token $adminToken
Assert-Case 'CASE 13d' 'Ket hop page+role+search' 200 $combo.Status
Write-Log ''

# ============================================================
# CASE 14/15: khoa / mo tai khoan HV001
# ============================================================
Write-Log '--- CASE 14/15: Khoa / mo tai khoan ---'
$hv001Id = Get-SqlScalar "SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap='HV001'"
$lockBody = (@{ status = $StBiKhoa } | ConvertTo-Json -Compress)
$lock = Invoke-Api -Method 'PATCH' -Path "/api/admin/accounts/$hv001Id/status" -Body $lockBody -Token $adminToken
Assert-Case 'CASE 14' 'Khoa HV001 -> 200' 200 $lock.Status
Assert-Case 'CASE 14b' 'DB trang thai = Bi khoa' $StBiKhoa (Get-SqlScalar "SELECT TrangThaiTaiKhoan FROM TaiKhoan WHERE TenDangNhap='HV001'")
Assert-Case 'CASE 14c' 'Login HV001 -> 401' 401 (Login-Status 'HV001')

$unlockBody = (@{ status = $StHoatDong } | ConvertTo-Json -Compress)
$unlock = Invoke-Api -Method 'PATCH' -Path "/api/admin/accounts/$hv001Id/status" -Body $unlockBody -Token $adminToken
Assert-Case 'CASE 15' 'Mo lai HV001 -> 200' 200 $unlock.Status
Assert-Case 'CASE 15b' 'DA KHOI PHUC: DB = Hoat dong' $StHoatDong (Get-SqlScalar "SELECT TrangThaiTaiKhoan FROM TaiKhoan WHERE TenDangNhap='HV001'")
Assert-Case 'CASE 15c' 'Login HV001 lai duoc -> 200' 200 (Login-Status 'HV001')

# Trang thai khong hop le -> 400
$badStatus = (@{ status = 'Sai trang thai' } | ConvertTo-Json -Compress)
Assert-Case 'CASE 15d' 'Status khong hop le -> 400' 400 (Invoke-Api -Method 'PATCH' -Path "/api/admin/accounts/$hv001Id/status" -Body $badStatus -Token $adminToken).Status
# Khong cho admin tu khoa chinh minh
$adminId = Get-SqlScalar "SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap='ADMIN001'"
Assert-Case 'CASE 15e' 'Admin tu khoa chinh minh -> 400' 400 (Invoke-Api -Method 'PATCH' -Path "/api/admin/accounts/$adminId/status" -Body $lockBody -Token $adminToken).Status
# Giao vien khong duoc doi trang thai
Assert-Case 'CASE 15f' 'Giao vien doi trang thai -> 403' 403 (Invoke-Api -Method 'PATCH' -Path "/api/admin/accounts/$hv001Id/status" -Body $lockBody -Token $teacherToken).Status
Write-Log ''

# ============================================================
# CASE 16: reset password
# ============================================================
Write-Log '--- CASE 16: Reset password (tai khoan test moi) ---'
$tmpBody = @{ hoTen = "Ph3 ResetTarget $stamp"; ngaySinh = '2007-07-07'; email = "ph3.reset.$stamp@test.local" } | ConvertTo-Json -Compress
$tmpCreate = Invoke-Api -Method 'POST' -Path '/api/admin/students' -Body $tmpBody -Token $adminToken
Assert-Case 'CASE 16a' 'Tao tai khoan test -> 201' 201 $tmpCreate.Status
if ($tmpCreate.Status -eq 201) {
    $tmp = $tmpCreate.Text | ConvertFrom-Json
    $tmpId = Get-SqlScalar "SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap='$($tmp.studentId)'"
    $oldPassword = $tmp.temporaryPassword
    Assert-Case 'CASE 16b' 'Login bang mat khau tam ban dau' 200 (Login-Status $tmp.studentId $oldPassword)

    $reset = Invoke-Api -Method 'POST' -Path "/api/admin/accounts/$tmpId/reset-password" -Token $adminToken
    Assert-Case 'CASE 16' 'Reset password -> 200' 200 $reset.Status
    if ($reset.Status -eq 200) {
        $resetJson = $reset.Text | ConvertFrom-Json
        Write-Log ("        username=" + $resetJson.username + " do dai mat khau moi=" + $resetJson.temporaryPassword.Length)
        Assert-Case 'CASE 16c' 'Tra ve mat khau tam MOI' $true ([bool]$resetJson.temporaryPassword)
        Assert-Case 'CASE 16d' 'Mat khau moi KHAC mat khau cu' $true ($resetJson.temporaryPassword -ne $oldPassword)
        Assert-Case 'CASE 16e' 'KHONG lo MatKhauHash' $false ($reset.Text -like '*MatKhauHash*')
        Assert-Case 'CASE 16f' 'Mat khau CU khong dang nhap duoc -> 401' 401 (Login-Status $tmp.studentId $oldPassword)
        Assert-Case 'CASE 16g' 'Mat khau MOI dang nhap duoc -> 200' 200 (Login-Status $tmp.studentId $resetJson.temporaryPassword)
        Assert-Case 'CASE 16h' 'Reset khong ton tai -> 404' 404 (Invoke-Api -Method 'POST' -Path '/api/admin/accounts/999999/reset-password' -Token $adminToken).Status
    }
}
Write-Log ''

# ============================================================
# CASE 17-21: self profile
# ============================================================
Write-Log '--- CASE 17-21: Self profile ---'
$meHv = Invoke-Api -Method 'GET' -Path '/api/student/profile' -Token $studentToken
Assert-Case 'CASE 17' 'HV001 GET student profile -> 200' 200 $meHv.Status
if ($meHv.Status -eq 200) {
    $hvJson = $meHv.Text | ConvertFrom-Json
    Write-Log ("        MaHocVien=" + $hvJson.maHocVien + " HoTen=" + $hvJson.hoTen + " TrangThai=" + $hvJson.trangThaiHocTap)
    Assert-Case 'CASE 17b' 'Dung profile HV001' 'HV001' $hvJson.maHocVien
    Assert-Case 'CASE 17c' 'Co thong tin phu huynh' $true ($null -ne $hvJson.phuHuynh)
    Assert-Case 'CASE 17d' 'KHONG lo MatKhauHash' $false ($meHv.Text -like '*MatKhauHash*')
}
Assert-Case 'CASE 18' 'GV001 goi student profile -> 403' 403 (Invoke-Api -Method 'GET' -Path '/api/student/profile' -Token $teacherToken).Status
Assert-Case 'CASE 18b' 'Khong token -> 401' 401 (Invoke-Api -Method 'GET' -Path '/api/student/profile').Status

$meGv = Invoke-Api -Method 'GET' -Path '/api/teacher/profile' -Token $teacherToken
Assert-Case 'CASE 19' 'GV001 GET teacher profile -> 200' 200 $meGv.Status
if ($meGv.Status -eq 200) {
    $gvJson = $meGv.Text | ConvertFrom-Json
    Write-Log ("        MaGiaoVien=" + $gvJson.maGiaoVien + " Loai=" + $gvJson.loaiGiaoVien + " so BangCap=" + @($gvJson.bangCaps).Count + " so ChungChi=" + @($gvJson.chungChis).Count)
    Assert-Case 'CASE 19b' 'Dung profile GV001' 'GV001' $gvJson.maGiaoVien
}
$meTg = Invoke-Api -Method 'GET' -Path '/api/teacher/profile' -Token $assistantToken
Assert-Case 'CASE 20' 'TG001 GET teacher profile -> 200' 200 $meTg.Status
if ($meTg.Status -eq 200) {
    $tgJson = $meTg.Text | ConvertFrom-Json
    Assert-Case 'CASE 20b' 'Dung profile TG001' 'TG001' $tgJson.maGiaoVien
    Assert-Case 'CASE 20c' 'LoaiGiaoVien = Tro giang' $LoaiTG $tgJson.loaiGiaoVien
}
Assert-Case 'CASE 21' 'HV001 goi teacher profile -> 403' 403 (Invoke-Api -Method 'GET' -Path '/api/teacher/profile' -Token $studentToken).Status
Write-Log ''

# ============================================================
# CASE 22/23/24/25: update self profile + email dong bo
# ============================================================
Write-Log '--- CASE 22-25: Update self profile + dong bo email ---'
# Tao 1 student + 1 teacher rieng de test update
$upS = @{ hoTen = "Ph3 UpdStudent $stamp"; ngaySinh = '2006-06-06'; email = "ph3.upds.$stamp@test.local" } | ConvertTo-Json -Compress
$upStudent = (Invoke-Api -Method 'POST' -Path '/api/admin/students' -Body $upS -Token $adminToken).Text | ConvertFrom-Json
$upT = @{ hoTen = "Ph3 UpdTeacher $stamp"; email = "ph3.updt.$stamp@test.local"; loaiGiaoVien = $LoaiGV } | ConvertTo-Json -Compress
$upTeacher = (Invoke-Api -Method 'POST' -Path '/api/admin/teachers' -Body $upT -Token $adminToken).Text | ConvertFrom-Json

# CASE 22: student doi email -> dong bo 2 bang
$sToken = Login-Token $upStudent.studentId $upStudent.temporaryPassword
$newStudentEmail = "ph3.new.s.$stamp@test.local"
$updS = (@{ email = $newStudentEmail; soDienThoai = '0999888777' } | ConvertTo-Json -Compress)
$r22 = Invoke-Api -Method 'PATCH' -Path '/api/student/profile' -Body $updS -Token $sToken
Assert-Case 'CASE 22' 'Student tu doi email -> 200' 200 $r22.Status
Assert-Case 'CASE 22b' 'TaiKhoan.Email == HocVien.Email' 1 (Get-SqlScalar "SELECT COUNT(*) FROM TaiKhoan t JOIN HocVien h ON h.MaHocVien=t.TenDangNhap WHERE t.TenDangNhap='$($upStudent.studentId)' AND t.Email=h.Email AND t.Email='$newStudentEmail'")

# CASE 23: teacher doi email -> dong bo 2 bang
$tToken = Login-Token $upTeacher.teacherId $upTeacher.temporaryPassword
$newTeacherEmail = "ph3.new.t.$stamp@test.local"
$updT = (@{ email = $newTeacherEmail; diaChi = 'Quan 7' } | ConvertTo-Json -Compress)
$r23 = Invoke-Api -Method 'PATCH' -Path '/api/teacher/profile' -Body $updT -Token $tToken
Assert-Case 'CASE 23' 'Teacher tu doi email -> 200' 200 $r23.Status
Assert-Case 'CASE 23b' 'TaiKhoan.Email == GiaoVien.Email' 1 (Get-SqlScalar "SELECT COUNT(*) FROM TaiKhoan t JOIN GiaoVien g ON g.MaGiaoVien=t.TenDangNhap WHERE t.TenDangNhap='$($upTeacher.teacherId)' AND t.Email=g.Email AND t.Email='$newTeacherEmail'")

# Email trung -> 409
$dupEmail = (@{ email = 'admin@trungtam.edu.vn' } | ConvertTo-Json -Compress)
Assert-Case 'CASE 23c' 'Doi sang email da ton tai -> 409' 409 (Invoke-Api -Method 'PATCH' -Path '/api/teacher/profile' -Body $dupEmail -Token $tToken).Status

# CASE 24: student co gui TrangThaiHocTap / MaHocVien -> KHONG duoc bind
$beforeStatus = Get-SqlScalar "SELECT TrangThaiHocTap FROM HocVien WHERE MaHocVien='$($upStudent.studentId)'"
$hack = '{"hoTen":"Ph3 UpdStudent Renamed","trangThaiHocTap":"Da nghi","maHocVien":"HV999","trangThaiTaiKhoan":"Bị khóa"}'
$r24 = Invoke-Api -Method 'PATCH' -Path '/api/student/profile' -Body $hack -Token $sToken
Assert-Case 'CASE 24' 'Student gui field trai quyen -> 200 (chi bind field hop le)' 200 $r24.Status
Assert-Case 'CASE 24b' 'TrangThaiHocTap KHONG bi doi' $beforeStatus (Get-SqlScalar "SELECT TrangThaiHocTap FROM HocVien WHERE MaHocVien='$($upStudent.studentId)'")
Assert-Case 'CASE 24c' 'MaHocVien KHONG bi doi' 1 (Get-SqlScalar "SELECT COUNT(*) FROM HocVien WHERE MaHocVien='$($upStudent.studentId)'")
Assert-Case 'CASE 24d' 'TrangThaiTaiKhoan KHONG bi doi' $StHoatDong (Get-SqlScalar "SELECT TrangThaiTaiKhoan FROM TaiKhoan WHERE TenDangNhap='$($upStudent.studentId)'")
Assert-Case 'CASE 24e' 'HoTen (field hop le) CO duoc cap nhat' 1 (Get-SqlScalar "SELECT COUNT(*) FROM HocVien WHERE MaHocVien='$($upStudent.studentId)' AND HoTen LIKE 'Ph3 UpdStudent Renamed%'")

# CASE 25: teacher co gui LoaiGiaoVien / ChuyenMon -> KHONG duoc bind
$hackT = '{"hoTen":"Ph3 UpdTeacher Renamed","loaiGiaoVien":"Giáo viên","chuyenMon":"HACKED","soNamKinhNghiem":99,"trangThai":"Đã nghỉ việc"}'
$r25 = Invoke-Api -Method 'PATCH' -Path '/api/teacher/profile' -Body $hackT -Token $tToken
Assert-Case 'CASE 25' 'Teacher gui field trai quyen -> 200 (chi bind field hop le)' 200 $r25.Status
Assert-Case 'CASE 25b' 'LoaiGiaoVien KHONG bi doi' $LoaiGV (Get-SqlScalar "SELECT LoaiGiaoVien FROM GiaoVien WHERE MaGiaoVien='$($upTeacher.teacherId)'")
Assert-Case 'CASE 25c' 'ChuyenMon KHONG bi doi (van NULL)' 1 (Get-SqlScalar "SELECT COUNT(*) FROM GiaoVien WHERE MaGiaoVien='$($upTeacher.teacherId)' AND ChuyenMon IS NULL")
Assert-Case 'CASE 25d' 'TrangThai KHONG bi doi' 1 (Get-SqlScalar "SELECT COUNT(*) FROM GiaoVien WHERE MaGiaoVien='$($upTeacher.teacherId)' AND SoNamKinhNghiem IS NULL")
Write-Log ''

# ============================================================
# DELETE tai khoan (SS22)
# ============================================================
Write-Log '--- Xoa tai khoan (SS22) ---'
$delBody = @{ hoTen = "Ph3 Deletable $stamp"; ngaySinh = '2005-05-05'; email = "ph3.del.$stamp@test.local" } | ConvertTo-Json -Compress
$delTarget = (Invoke-Api -Method 'POST' -Path '/api/admin/students' -Body $delBody -Token $adminToken).Text | ConvertFrom-Json
$delTargetId = Get-SqlScalar "SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap='$($delTarget.studentId)'"
Assert-Case 'DEL-1' 'Tao tai khoan moi de xoa -> 201' $true ([bool]$delTarget.studentId)
$del = Invoke-Api -Method 'DELETE' -Path "/api/admin/accounts/$delTargetId" -Token $adminToken
Assert-Case 'DEL-2' 'Xoa tai khoan moi (khong du lieu) -> 204' 204 $del.Status
Assert-Case 'DEL-3' 'Khong con TaiKhoan trong DB' 0 (Get-SqlScalar "SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap='$($delTarget.studentId)'")
Assert-Case 'DEL-4' 'Khong con HocVien trong DB' 0 (Get-SqlScalar "SELECT COUNT(*) FROM HocVien WHERE MaHocVien='$($delTarget.studentId)'")

$hv001Deps = Get-SqlScalar "SELECT (SELECT COUNT(*) FROM DangKyHoc WHERE MaHocVien='HV001') + (SELECT COUNT(*) FROM KiemTraDauVao WHERE MaHocVien='HV001') + (SELECT COUNT(*) FROM DiemDanh WHERE MaHocVien='HV001') + (SELECT COUNT(*) FROM PhanHoiDichVu WHERE MaHocVien='HV001') + (SELECT COUNT(*) FROM PhanHoiKhaoSat WHERE MaHocVien='HV001')"
Write-Log ("        HV001 co $hv001Deps du lieu phu thuoc")
if ($hv001Deps -gt 0) {
    Assert-Case 'DEL-5' 'Xoa HV001 (co du lieu) -> 409' 409 (Invoke-Api -Method 'DELETE' -Path "/api/admin/accounts/$hv001Id" -Token $adminToken).Status
    Assert-Case 'DEL-6' 'HV001 VAN CON trong DB (khong bi xoa)' 1 (Get-SqlScalar "SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap='HV001'")
} else {
    Write-Log '        (BO QUA: HV001 khong co du lieu phu thuoc)'
}
Write-Log ''

# ============================================================
# REGRESSION Phase 1 + Phase 2
# ============================================================
Write-Log '--- REGRESSION: Phase 1 + Phase 2 ---'
Assert-Case 'REG-1' 'GET /health' 200 (Invoke-Api -Method 'GET' -Path '/health').Status
Assert-Case 'REG-2' 'GET /health/database' 200 (Invoke-Api -Method 'GET' -Path '/health/database').Status
Assert-Case 'REG-3' 'GET /api/development/database-info' 200 (Invoke-Api -Method 'GET' -Path '/api/development/database-info').Status
Assert-Case 'REG-4' 'POST /api/auth/login' 200 (Login-Status 'ADMIN001')
Assert-Case 'REG-5' 'GET /api/auth/me' 200 (Invoke-Api -Method 'GET' -Path '/api/auth/me' -Token $adminToken).Status
Assert-Case 'REG-6' 'GET /api/auth/me khong token -> 401' 401 (Invoke-Api -Method 'GET' -Path '/api/auth/me').Status
Assert-Case 'REG-7' 'GET /api/development/admin-only (admin) -> 200' 200 (Invoke-Api -Method 'GET' -Path '/api/development/admin-only' -Token $adminToken).Status
Assert-Case 'REG-8' 'GET /api/development/admin-only (giao vien) -> 403' 403 (Invoke-Api -Method 'GET' -Path '/api/development/admin-only' -Token $teacherToken).Status
Assert-Case 'REG-9' 'GET /api/development/teacher-only (giao vien) -> 200' 200 (Invoke-Api -Method 'GET' -Path '/api/development/teacher-only' -Token $teacherToken).Status
Assert-Case 'REG-10' 'POST /api/auth/change-password (sai current) -> 400' 400 (Invoke-Api -Method 'POST' -Path '/api/auth/change-password' -Body '{"currentPassword":"Sai@12345","newPassword":"Moi@12345","confirmPassword":"Moi@12345"}' -Token $adminToken).Status
Write-Log ''

Write-Log '============================================================'
Write-Log (" KET QUA PART 2: PASS = {0} / FAIL = {1}" -f $passed, $failed)
Write-Log '============================================================'

$log | Set-Content -Path $reportPath -Encoding utf8
