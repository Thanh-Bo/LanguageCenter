$ErrorActionPreference = 'Continue'
$out = "d:\LanguageCenter\.vscode\phase3-verify2-out.txt"
if (Test-Path $out) { Remove-Item $out -Force }

function Say([string]$t) {
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
            $parts = @()
            for ($i = 0; $i -lt $r.FieldCount; $i++) {
                $v = $r.GetValue($i)
                if ($v -is [System.DBNull]) { $v = 'NULL' }
                $parts += ($r.GetName($i) + '=' + [string]$v)
            }
            $line = '    ' + ($parts -join '  |  ')
            Say $line
            $n++
        }
        $r.Close()
        $cn.Close()
        if ($n -eq 0) { Say '    (khong co dong nao)' }
        Say ('    --> OK (' + $n + ' dong)')
    } catch {
        Say ('    !!! LOI: ' + $_.Exception.Message)
    }
    Say ''
}

TryQ 'Q1 - So luong ban ghi' 'SELECT (SELECT COUNT(*) FROM TaiKhoan) AS TaiKhoan, (SELECT COUNT(*) FROM HocVien) AS HocVien, (SELECT COUNT(*) FROM GiaoVien) AS GiaoVien, (SELECT COUNT(*) FROM NhanVien) AS NhanVien, (SELECT COUNT(*) FROM VaiTro) AS VaiTro, (SELECT COUNT(*) FROM PhuHuynh) AS PhuHuynh'

TryQ 'Q2 - Cot that cua TaiKhoan' "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='TaiKhoan' ORDER BY ORDINAL_POSITION"

TryQ 'Q3 - Cot that cua HocVien va GiaoVien (chi cot khoa/trang thai/email)' "SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME IN ('HocVien','GiaoVien') AND (COLUMN_NAME LIKE 'Ma%' OR COLUMN_NAME LIKE '%TrangThai%' OR COLUMN_NAME LIKE '%Email%' OR COLUMN_NAME LIKE 'Loai%') ORDER BY TABLE_NAME, ORDINAL_POSITION"

TryQ 'Q4 - 11 TAI KHOAN GOC: trang thai + vai tro + email' "SELECT t.TenDangNhap, v.TenVaiTro, t.TrangThaiTaiKhoan, t.Email FROM TaiKhoan t JOIN VaiTro v ON v.MaVaiTro=t.MaVaiTro WHERE t.TenDangNhap IN ('ADMIN001','NVHV001','NVTV001','NVKT001','GV001','GV002','TG001','HV001','HV002','HV003','HV004') ORDER BY t.TenDangNhap"

TryQ 'Q5 - DONG BO EMAIL: so dong lech (phai = 0)' "SELECT (SELECT COUNT(*) FROM HocVien h JOIN TaiKhoan t ON t.TenDangNhap=h.MaHocVien WHERE ISNULL(t.Email,'x')<>ISNULL(h.Email,'y')) AS LechEmailHocVien, (SELECT COUNT(*) FROM GiaoVien g JOIN TaiKhoan t ON t.TenDangNhap=g.MaGiaoVien WHERE ISNULL(t.Email,'x')<>ISNULL(g.Email,'y')) AS LechEmailGiaoVien"

TryQ 'Q6 - MA TRUNG (phai 0 dong)' 'SELECT MaHocVien AS Ma FROM HocVien GROUP BY MaHocVien HAVING COUNT(*)>1 UNION ALL SELECT MaGiaoVien FROM GiaoVien GROUP BY MaGiaoVien HAVING COUNT(*)>1 UNION ALL SELECT TenDangNhap FROM TaiKhoan GROUP BY TenDangNhap HAVING COUNT(*)>1'

TryQ 'Q7 - MAT KHAU HASH' "SELECT COUNT(*) AS Tong, SUM(CASE WHEN MatKhauHash LIKE 'AQAAAA%' THEN 1 ELSE 0 END) AS Pbkdf2That, SUM(CASE WHEN MatKhauHash IS NULL OR LEN(MatKhauHash)<40 THEN 1 ELSE 0 END) AS BatThuong FROM TaiKhoan"

TryQ 'Q8 - HOC VIEN: ma / trang thai hoc tap / email 2 bang' 'SELECT h.MaHocVien, h.TrangThaiHocTap, t.TrangThaiTaiKhoan, h.Email AS EmailHocVien, t.Email AS EmailTaiKhoan, CASE WHEN h.Email = t.Email THEN ''KHOP'' ELSE ''LECH'' END AS DongBo FROM HocVien h JOIN TaiKhoan t ON t.TenDangNhap = h.MaHocVien ORDER BY h.MaHocVien'

TryQ 'Q9 - GIAO VIEN / TRO GIANG: ma / loai / trang thai' 'SELECT g.MaGiaoVien, g.LoaiGiaoVien, g.TrangThai, t.TrangThaiTaiKhoan, CASE WHEN g.Email = t.Email THEN ''KHOP'' ELSE ''LECH'' END AS DongBo FROM GiaoVien g JOIN TaiKhoan t ON t.TenDangNhap = g.MaGiaoVien ORDER BY g.MaGiaoVien'

TryQ 'Q10 - TAI KHOAN HV/GV/TG KHONG CO HO SO (phai 0 dong)' "SELECT t.TenDangNhap FROM TaiKhoan t WHERE (t.TenDangNhap LIKE 'HV%' OR t.TenDangNhap LIKE 'GV%' OR t.TenDangNhap LIKE 'TG%') AND NOT EXISTS (SELECT 1 FROM HocVien h WHERE h.MaHocVien=t.TenDangNhap) AND NOT EXISTS (SELECT 1 FROM GiaoVien g WHERE g.MaGiaoVien=t.TenDangNhap)"

TryQ 'Q11 - VaiTro' 'SELECT MaVaiTro, TenVaiTro FROM VaiTro ORDER BY MaVaiTro'

Say '============================================================'
Say ' XONG'
Say '============================================================'
