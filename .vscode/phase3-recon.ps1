# Phase 3 recon - doc du lieu THAT tu SQL Server (READ ONLY)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Data

$connString = 'Server=localhost\SQLEXPRESS;Database=QuanLyTrungTamNgoaiNgu;Integrated Security=True'
$connection = New-Object System.Data.SqlClient.SqlConnection $connString
$connection.Open()

function Invoke-Query {
    param([string]$Title, [string]$Sql)
    Write-Output ''
    Write-Output "=== $Title ==="
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = $Sql
    $reader = $cmd.ExecuteReader()
    while ($reader.Read()) {
        $row = @()
        for ($i = 0; $i -lt $reader.FieldCount; $i++) {
            $row += [string]$reader.GetValue($i)
        }
        Write-Output ('  ' + ($row -join ' | '))
    }
    $reader.Close()
}

Invoke-Query 'MA HOC VIEN hien co (max suffix)' @"
SELECT COUNT(*) AS SoLuong,
       MAX(MaHocVien) AS MaxChuoi,
       MAX(CAST(SUBSTRING(MaHocVien,3,10) AS INT)) AS MaxSo
FROM HocVien WHERE MaHocVien LIKE 'HV[0-9][0-9][0-9]'
"@

Invoke-Query 'MA GIAO VIEN hien co' @"
SELECT MaGiaoVien, LoaiGiaoVien, TrangThai, HoTen FROM GiaoVien ORDER BY MaGiaoVien
"@

Invoke-Query 'MA NHAN VIEN hien co' @"
SELECT MaNhanVien, ChucVu, TrangThaiLamViec FROM NhanVien ORDER BY MaNhanVien
"@

Invoke-Query 'DISTINCT TrangThaiHocTap' @"
SELECT TrangThaiHocTap, COUNT(*) AS SoLuong FROM HocVien GROUP BY TrangThaiHocTap
"@

Invoke-Query 'DISTINCT GiaoVien.TrangThai' @"
SELECT TrangThai, COUNT(*) AS SoLuong FROM GiaoVien GROUP BY TrangThai
"@

Invoke-Query 'DISTINCT NhanVien.TrangThaiLamViec' @"
SELECT TrangThaiLamViec, COUNT(*) AS SoLuong FROM NhanVien GROUP BY TrangThaiLamViec
"@

Invoke-Query 'PHU HUYNH - so hoc vien dung chung' @"
SELECT p.MaPhuHuynh, p.HoTen, COUNT(h.MaHocVien) AS SoHocVien
FROM PhuHuynh p LEFT JOIN HocVien h ON h.MaPhuHuynh = p.MaPhuHuynh
GROUP BY p.MaPhuHuynh, p.HoTen ORDER BY p.MaPhuHuynh
"@

Invoke-Query 'HOC VIEN hien co' @"
SELECT MaHocVien, HoTen, MaPhuHuynh, MoiQuanHePhuHuynh, TrangThaiHocTap, Email FROM HocVien ORDER BY MaHocVien
"@

Invoke-Query 'EMAIL dong bo TaiKhoan vs profile (ky vong 0 lech)' @"
SELECT 'HocVien' AS Bang, COUNT(*) AS SoLech FROM HocVien h JOIN TaiKhoan t ON t.TenDangNhap = h.MaHocVien WHERE ISNULL(t.Email,'') <> ISNULL(h.Email,'')
UNION ALL
SELECT 'GiaoVien', COUNT(*) FROM GiaoVien g JOIN TaiKhoan t ON t.TenDangNhap = g.MaGiaoVien WHERE ISNULL(t.Email,'') <> ISNULL(g.Email,'')
UNION ALL
SELECT 'NhanVien', COUNT(*) FROM NhanVien n JOIN TaiKhoan t ON t.TenDangNhap = n.MaNhanVien WHERE ISNULL(t.Email,'') <> ISNULL(n.Email,'')
"@

Invoke-Query 'CHECK CONSTRAINTS lien quan' @"
SELECT OBJECT_NAME(parent_object_id) AS Bang, name AS TenConstraint, definition
FROM sys.check_constraints
WHERE parent_object_id IN (OBJECT_ID('HocVien'), OBJECT_ID('GiaoVien'), OBJECT_ID('TaiKhoan'), OBJECT_ID('PhuHuynh'))
ORDER BY Bang, TenConstraint
"@

Invoke-Query 'UNIQUE INDEX / CONSTRAINT tren TaiKhoan + profile' @"
SELECT OBJECT_NAME(i.object_id) AS Bang, i.name AS TenIndex, i.is_unique, i.is_primary_key
FROM sys.indexes i
WHERE i.object_id IN (OBJECT_ID('TaiKhoan'), OBJECT_ID('HocVien'), OBJECT_ID('GiaoVien'), OBJECT_ID('PhuHuynh'))
  AND (i.is_unique = 1)
ORDER BY Bang, TenIndex
"@

Invoke-Query 'SQL Server version + isolation' @"
SELECT @@VERSION AS Ver, CAST(SESSIONPROPERTY('TRANSACTION ISOLATION LEVEL') AS VARCHAR(20)) AS Iso
"@

$connection.Close()
Write-Output ''
Write-Output 'PHASE3_RECON_DONE'
