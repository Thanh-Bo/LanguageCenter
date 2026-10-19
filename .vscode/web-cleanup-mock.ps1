# Xoa cac file mock cu cua web HUITLINGO (da duoc thay bang src/lib/api/*).
# Chi xoa file trong danh sach duoi day; khong dung toi file khac.
$root = Split-Path -Parent $PSScriptRoot
$web = Join-Path $root 'web\HUITLINGO\src'

$files = @(
    'lib\goiAPI.ts',
    'lib\khoDuLieu.ts',
    'pages\CongQuanLy.tsx',
    'pages\giaovien\GiaoVienPortal.tsx',
    'pages\giaovien\LopCuaToi.tsx',
    'pages\giaovien\LichDay.tsx',
    'pages\giaovien\DiemDanh.tsx',
    'pages\giaovien\BangDiem.tsx',
    'pages\giaovien\ThongBao.tsx',
    'pages\hocvien\HocVienPortal.tsx',
    'pages\hocvien\TongQuanHocVien.tsx',
    'pages\hocvien\LichHoc.tsx',
    'pages\hocvien\DiemSo.tsx',
    'pages\hocvien\HoSoHocVien.tsx'
)

foreach ($f in $files) {
    $path = Join-Path $web $f
    if (Test-Path -LiteralPath $path) {
        Remove-Item -LiteralPath $path -Force
        Write-Output "DELETED: $f"
    }
    else {
        Write-Output "SKIP (not found): $f"
    }
}

Write-Output 'CLEANUP_DONE'
