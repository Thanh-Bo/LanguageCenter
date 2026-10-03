$ErrorActionPreference = 'Continue'
$out = "d:\LanguageCenter\.vscode\phase3-final-out.txt"
if (Test-Path $out) { Remove-Item $out -Force }

function Say([string]$t = "") {
    Write-Host $t
    Add-Content -Path $out -Value $t -Encoding UTF8
}

Say "============================================================"
Say " PHASE 3 - BUILD CUOI + KIEM TRA TOAN VEN DATABASE"
Say " Thoi diem: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
Say "============================================================"
Say ""

# ---------- BUILD ----------
Say "--- BUILD SOLUTION ---"
$buildLog = "d:\LanguageCenter\.vscode\build-final.txt"
& dotnet build "d:\LanguageCenter\LanguageCenter.sln" --nologo -v normal *> $buildLog
$buildExit = $LASTEXITCODE
Say "ExitCode = $buildExit"
Get-Content $buildLog | Select-String -Pattern 'Warning\(s\)|Error\(s\)|Build succeeded|Build FAILED|error CS|error MSB' | ForEach-Object { Say "  $($_.Line.Trim())" }
Say ""

# ---------- DB ----------
Say "--- KET NOI DATABASE ---"
try {
    Add-Type -AssemblyName System.Data
    $conn = New-Object System.Data.SqlClient.SqlConnection("Server=localhost\SQLEXPRESS;Database=QuanLyTrungTamNgoaiNgu;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True")
    $conn.Open()
    Say "  Ket noi OK (server version $($conn.ServerVersion))"
} catch {
    Say "  LOI KET NOI: $($_.Exception.Message)"
    exit 1
}
Say ""

function Q([string]$sql) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    $r = $cmd.ExecuteReader()
    $rows = @()
    while ($r.Read()) {
        $o = [ordered]@{}
        for ($i = 0; $i -lt $r.FieldCount; $i++) { $o[$r.GetName($i)] = $r.GetValue($i) }
        $rows += [pscustomobject]$o
    }
    $r.Close()
    return $rows
}

Say "--- SO LUONG BAN GHI ---"
Q "SELECT (SELECT COUNT(*) FROM TaiKhoan) AS TaiKhoan, (SELECT COUNT(*) FROM HocVien) AS HocVien, (SELECT COUNT(*) FROM GiaoVien) AS GiaoVien, (SELECT COUNT(*) FROM VaiTro) AS VaiTro, (SELECT COUNT(*) FROM PhuHuynh) AS PhuHuynh, (SELECT COUNT(*) FROM NhanVien) AS NhanVien" | ForEach-Object {
    Say "  TaiKhoan=$($_.TaiKhoan)  HocVien=$($_.HocVien)  GiaoVien=$($_.GiaoVien)  NhanVien=$($_.NhanVien)  VaiTro=$($_.VaiTro)  PhuHuynh=$($_.PhuHuynh)"
}
Say ""

Say "--- TOAN VEN (tat ca phai = 0) ---"
Q "SELECT (SELECT COUNT(*) FROM HocVien h JOIN TaiKhoan t ON t.TenDangNhap=h.MaHocVien WHERE h.MaTaiKhoan <> t.MaTaiKhoan) AS HocVienFkSai, (SELECT COUNT(*) FROM GiaoVien g JOIN TaiKhoan t ON t.TenDangNhap=g.MaGiaoVien WHERE g.MaTaiKhoan <> t.MaTaiKhoan) AS GiaoVienFkSai, (SELECT COUNT(*) FROM HocVien h JOIN TaiKhoan t ON t.TenDangNhap=h.MaHocVien WHERE ISNULL(t.Email,'') <> ISNULL(h.Email,'')) AS LechEmailHV, (SELECT COUNT(*) FROM GiaoVien g JOIN TaiKhoan t ON t.TenDangNhap=g.MaGiaoVien WHERE ISNULL(t.Email,'') <> ISNULL(g.Email,'')) AS LechEmailGV, (SELECT COUNT(*) FROM TaiKhoan t WHERE NOT EXISTS (SELECT 1 FROM HocVien h WHERE h.MaHocVien=t.TenDangNhap) AND NOT EXISTS (SELECT 1 FROM GiaoVien g WHERE g.MaGiaoVien=t.TenDangNhap) AND NOT EXISTS (SELECT 1 FROM NhanVien n WHERE n.MaNhanVien=t.TenDangNhap)) AS TaiKhoanKhongHoSo" | ForEach-Object {
    Say "  HocVien.FkSai=$($_.HocVienFkSai)  GiaoVien.FkSai=$($_.GiaoVienFkSai)  LechEmailHV=$($_.LechEmailHV)  LechEmailGV=$($_.LechEmailGV)  TaiKhoanKhongHoSo=$($_.TaiKhoanKhongHoSo)"
}

$dup = Q "SELECT MaHocVien AS Ma FROM HocVien GROUP BY MaHocVien HAVING COUNT(*)>1 UNION ALL SELECT MaGiaoVien FROM GiaoVien GROUP BY MaGiaoVien HAVING COUNT(*)>1 UNION ALL SELECT TenDangNhap FROM TaiKhoan GROUP BY TenDangNhap HAVING COUNT(*)>1"
if ($dup.Count -eq 0) { Say "  Ma trung: KHONG CO" } else { $dup | ForEach-Object { Say "  Ma trung: $($_.Ma)" } }
Say ""

Say "--- DU LIEU GOC (trang thai phai nguyen ven) ---"
Q "SELECT t.TenDangNhap, v.TenVaiTro, t.TrangThai, t.Email FROM TaiKhoan t JOIN VaiTro v ON v.MaVaiTro=t.MaVaiTro WHERE t.TenDangNhap IN ('ADMIN001','NVHV001','NVTV001','NVKT001','GV001','GV002','TG001','HV001','HV002','HV003','HV004') ORDER BY t.TenDangNhap" | ForEach-Object {
    Say "  $($_.TenDangNhap) | $($_.TenVaiTro) | $($_.TrangThai) | $($_.Email)"
}
Say ""

Say "--- MAT KHAU HASH ---"
Q "SELECT COUNT(*) AS Tong, SUM(CASE WHEN MatKhauHash LIKE 'AQAAAA%' THEN 1 ELSE 0 END) AS HashThat, SUM(CASE WHEN MatKhauHash IS NULL OR LEN(MatKhauHash) < 40 THEN 1 ELSE 0 END) AS BatThuong FROM TaiKhoan" | ForEach-Object {
    Say "  Tong=$($_.Tong)  PBKDF2 that=$($_.HashThat)  Bat thuong=$($_.BatThuong)"
}
Say ""

Say "--- TAI KHOAN SINH RA TRONG TEST ---"
Q "SELECT t.TenDangNhap, v.TenVaiTro FROM TaiKhoan t JOIN VaiTro v ON v.MaVaiTro=t.MaVaiTro ORDER BY t.TenDangNhap" | ForEach-Object {
    Say "  $($_.TenDangNhap) | $($_.TenVaiTro)"
}
Say ""

$conn.Close()
Say "  Da dong ket noi."
Say ""
Say "============================================================"
Say " HOAN TAT"
Say "============================================================"
