Write-Host "============================================================"
Write-Host " PHASE 3 - KIEM TRA TRANG THAI DATABASE CUOI"
Write-Host "============================================================"
Write-Host ""

$apiOut = & dotnet build "d:\LanguageCenter\LanguageCenter.sln" -v quiet --nologo 2>&1
$apiExit = $LASTEXITCODE
Write-Host "--- BUILD ---"
Write-Host "ExitCode = $apiExit"
$apiOut | Select-Object -Last 6 | ForEach-Object { Write-Host "  $_" }
Write-Host ""

Add-Type -AssemblyName System.Data
$conn = New-Object System.Data.SqlClient.SqlConnection("Server=localhost\SQLEXPRESS;Database=QuanLyTrungTamNgoaiNgu;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True")
$conn.Open()

function Q([string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    $r = $cmd.ExecuteReader()
    $rows = @()
    while ($r.Read()) {
        $o = @{}
        for ($i = 0; $i -lt $r.FieldCount; $i++) { $o[$r.GetName($i)] = $r.GetValue($i) }
        $rows += [pscustomobject]$o
    }
    $r.Close()
    return $rows
}

Write-Host "--- SO LUONG ---"
Q "SELECT (SELECT COUNT(*) FROM TaiKhoan) AS TaiKhoan, (SELECT COUNT(*) FROM HocVien) AS HocVien, (SELECT COUNT(*) FROM GiaoVien) AS GiaoVien, (SELECT COUNT(*) FROM VaiTro) AS VaiTro, (SELECT COUNT(*) FROM PhuHuynh) AS PhuHuynh" | ForEach-Object {
    Write-Host "  TaiKhoan=$($_.TaiKhoan)  HocVien=$($_.HocVien)  GiaoVien=$($_.GiaoVien)  VaiTro=$($_.VaiTro)  PhuHuynh=$($_.PhuHuynh)"
}

Write-Host ""
Write-Host "--- TINH TOAN VEN (phai = 0) ---"
Q "SELECT (SELECT COUNT(*) FROM TaiKhoan t LEFT JOIN HocVien h ON h.MaHocVien=t.TenDangNhap LEFT JOIN GiaoVien g ON g.MaGiaoVien=t.TenDangNhap WHERE h.MaHocVien IS NULL AND g.MaGiaoVien IS NULL AND t.MaVaiTro NOT IN (SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro IN (N'Quản trị viên',N'Nhân viên học vụ',N'Nhân viên tư vấn',N'Nhân viên kế toán'))) AS TaiKhoanMoCoi, (SELECT COUNT(*) FROM HocVien h JOIN TaiKhoan t ON t.TenDangNhap=h.MaHocVien WHERE ISNULL(t.Email,'') <> ISNULL(h.Email,'')) AS LechEmailHocVien, (SELECT COUNT(*) FROM GiaoVien g JOIN TaiKhoan t ON t.TenDangNhap=g.MaGiaoVien WHERE ISNULL(t.Email,'') <> ISNULL(g.Email,'')) AS LechEmailGiaoVien, (SELECT COUNT(*) FROM HocVien h JOIN TaiKhoan t ON t.TenDangNhap=h.MaHocVien WHERE h.MaTaiKhoan <> t.MaTaiKhoan) AS FkSai" | ForEach-Object {
    Write-Host "  TaiKhoanMoCoi=$($_.TaiKhoanMoCoi)  LechEmailHocVien=$($_.LechEmailHocVien)  LechEmailGiaoVien=$($_.LechEmailGiaoVien)  FkSai=$($_.FkSai)"
}

Write-Host ""
Write-Host "--- MA TRUNG (phai rong) ---"
Q "SELECT MaHocVien FROM HocVien GROUP BY MaHocVien HAVING COUNT(*)>1 UNION SELECT MaGiaoVien FROM GiaoVien GROUP BY MaGiaoVien HAVING COUNT(*)>1 UNION SELECT TenDangNhap FROM TaiKhoan GROUP BY TenDangNhap HAVING COUNT(*)>1" | ForEach-Object {
    Write-Host "  TRUNG: $($_.MaHocVien)$($_.MaGiaoVien)$($_.TenDangNhap)"
}
Write-Host "  (neu khong co dong nao o tren = khong trung)"

Write-Host ""
Write-Host "--- TAI KHOAN GOP (ADMIN001, HV001, GV001, TG001) ---"
Q "SELECT t.TenDangNhap, t.Email, t.TrangThai, v.TenVaiTro FROM TaiKhoan t JOIN VaiTro v ON v.MaVaiTro=t.MaVaiTro WHERE t.TenDangNhap IN ('ADMIN001','HV001','GV001','TG001') ORDER BY t.TenDangNhap" | ForEach-Object {
    Write-Host "  $($_.TenDangNhap) | $($_.TenVaiTro) | $($_.TrangThai) | $($_.Email)"
}

Write-Host ""
Write-Host "--- MAT KHAU HASH (phai bat dau AQAAAA = PBKDF2 that) ---"
Q "SELECT COUNT(*) AS Tong, SUM(CASE WHEN MatKhauHash LIKE 'AQAAAA%' THEN 1 ELSE 0 END) AS HashThat, SUM(CASE WHEN MatKhauHash IS NULL OR LEN(MatKhauHash) < 40 THEN 1 ELSE 0 END) AS NghiVan FROM TaiKhoan" | ForEach-Object {
    Write-Host "  Tong=$($_.Tong)  HashThat=$($_.HashThat)  NghiVan=$($_.NghiVan)"
}

$conn.Close()
Write-Host ""
Write-Host "============================================================"
