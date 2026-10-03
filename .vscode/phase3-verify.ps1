$ErrorActionPreference = 'Continue'
$out = "d:\LanguageCenter\.vscode\phase3-verify-out.txt"
if (Test-Path $out) { Remove-Item $out -Force }

function Say([string]$t = "") {
    Write-Host $t
    Add-Content -Path $out -Value $t -Encoding UTF8
}

Add-Type -AssemblyName System.Data
$ConnStr = "Server=localhost\SQLEXPRESS;Database=QuanLyTrungTamNgoaiNgu;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"

function TryQ([string]$label, [string]$sql) {
    Say "### $label"
    try {
        $cn = New-Object System.Data.SqlClient.SqlConnection($ConnStr)
        $cn.Open()
        $cmd = $cn.CreateCommand()
        $cmd.CommandText = $sql
        $cmd.CommandTimeout = 30
        $r = $cmd.ExecuteReader()
        $n = 0
        while ($r.Read()) {
            $vals = @()
            for ($i = 0; $i -lt $r.FieldCount; $i++) {
                $v = $r.GetValue($i)
                if ($v -is [System.DBNull]) { $v = 'NULL' }
                $vals += "$($r.GetName($i))=$v"
            }
            Say "    " + ($vals -join '  |  ')
            $n++
        }
        $r.Close()
        $cn.Close()
        if ($n -eq 0) { Say "    (khong co dong nao)" }
        Say "    --> OK ($n dong)"
    } catch {
        Say "    !!! LOI: $($_.Exception.Message)"
    }
    Say ""
}

# 1. So luong
TryQ 'Q1 - So luong ban ghi' "SELECT (SELECT COUNT(*) FROM TaiKhoan) AS TaiKhoan, (SELECT COUNT(*) FROM HocVien) AS HocVien, (SELECT COUNT(*) FROM GiaoVien) AS GiaoVien, (SELECT COUNT(*) FROM NhanVien) AS NhanVien, (SELECT COUNT(*) FROM VaiTro) AS VaiTro"

# 2. Dong bo email (TaiKhoan vs ho so)
TryQ 'Q2 - Lech email TaiKhoan vs ho so' "SELECT (SELECT COUNT(*) FROM HocVien h JOIN TaiKhoan t ON t.TenDangNhap = h.MaHocVien WHERE ISNULL(t.Email,'x') <> ISNULL(h.Email,'y')) AS LechEmailHocVien, (SELECT COUNT(*) FROM GiaoVien g JOIN TaiKhoan t ON t.TenDangNhap = g.MaGiaoVien WHERE ISNULL(t.Email,'x') <> ISNULL(g.Email,'y')) AS LechEmailGiaoVien"

# 3. FK nhat quan
TryQ 'Q3 - MaTaiKhoan trong ho so phai khop TaiKhoan' "SELECT (SELECT COUNT(*) FROM HocVien h JOIN TaiKhoan t ON t.TenDangNhap = h.MaHocVien WHERE h.MaTaiKhoan <> t.MaTaiKhoan) AS HocVienFkSai, (SELECT COUNT(*) FROM GiaoVien g JOIN TaiKhoan t ON t.TenDangNhap = g.MaGiaoVien WHERE g.MaTaiKhoan <> t.MaTaiKhoan) AS GiaoVienFkSai"

# 4. Tai khoan khong co ho so
TryQ 'Q4 - Tai khoan khong co ho so (chi tinh vai tro GiaoVien/Tro giang/Hoc vien)' "SELECT t.TenDangNhap, v.TenVaiTro FROM TaiKhoan t JOIN VaiTro v ON v.MaVaiTro = t.MaVaiTro WHERE v.TenVaiTro LIKE '%gi%' AND NOT EXISTS (SELECT 1 FROM HocVien h WHERE h.MaHocVien = t.TenDangNhap) AND NOT EXISTS (SELECT 1 FROM GiaoVien g WHERE g.MaGiaoVien = t.TenDangNhap)"

# 5. Trang thai tai khoan goc
TryQ 'Q5 - Trang thai 11 tai khoan goc' "SELECT TOP 11 t.TenDangNhap, t.TrangThai, t.Email FROM TaiKhoan t WHERE t.TenDangNhap IN ('ADMIN001','NVHV001','NVTV001','NVKT001','GV001','GV002','TG001','HV001','HV002','HV003','HV004') ORDER BY t.TenDangNhap"

# 6. Ma trung
TryQ 'Q6 - Ma trung (phai 0 dong)' "SELECT MaHocVien AS Ma FROM HocVien GROUP BY MaHocVien HAVING COUNT(*) > 1 UNION ALL SELECT MaGiaoVien FROM GiaoVien GROUP BY MaGiaoVien HAVING COUNT(*) > 1 UNION ALL SELECT TenDangNhap FROM TaiKhoan GROUP BY TenDangNhap HAVING COUNT(*) > 1"

# 7. Hash
TryQ 'Q7 - Mat khau hash' "SELECT COUNT(*) AS Tong, SUM(CASE WHEN MatKhauHash LIKE 'AQAAAA%' THEN 1 ELSE 0 END) AS Pbkdf2That, SUM(CASE WHEN MatKhauHash IS NULL OR LEN(MatKhauHash) < 40 THEN 1 ELSE 0 END) AS BatThuong FROM TaiKhoan"

# 8. TenDangNhap == Ma ho so
TryQ 'Q8 - TenDangNhap phai bang MaHocVien/MaGiaoVien' "SELECT h.MaHocVien, h.MaTaiKhoan, t.TenDangNhap, t.Email AS EmailTaiKhoan, h.Email AS EmailHocVien, h.TrangThaiHocTap FROM HocVien h JOIN TaiKhoan t ON t.MaTaiKhoan = h.MaTaiKhoan ORDER BY h.MaHocVien"

# 9. Giao vien
TryQ 'Q9 - GiaoVien: ma, loai, trang thai, email' "SELECT g.MaGiaoVien, g.LoaiGiaoVien, g.TrangThai, g.Email, t.Email AS EmailTaiKhoan FROM GiaoVien g JOIN TaiKhoan t ON t.MaTaiKhoan = g.MaTaiKhoan ORDER BY g.MaGiaoVien"

Say "============================================================"
Say " XONG"
Say "============================================================"
