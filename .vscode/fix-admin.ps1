$ErrorActionPreference = 'Continue'
$out = "d:\LanguageCenter\.vscode\fix-admin-out.txt"
if (Test-Path $out) { Remove-Item $out -Force }

function Say([string]$t) {
    Write-Host $t
    Add-Content -Path $out -Value $t -Encoding UTF8
}

$sqlFile = "d:\LanguageCenter\database\KhoiPhuc_QuanTriVien.sql"
$ConnStr = "Server=localhost\SQLEXPRESS;Database=QuanLyTrungTamNgoaiNgu;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True"

Say "============================================================"
Say " KHOI PHUC TAI KHOAN QUAN TRI VIEN"
Say " Thoi diem: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
Say " File SQL : $sqlFile"
Say "============================================================"
Say ""

if (-not (Test-Path $sqlFile)) {
    Say "!!! KHONG TIM THAY FILE SQL"
    exit 1
}

Add-Type -AssemblyName System.Data

# Doc file bang UTF8 de giu dung tieng Viet
$text = Get-Content -Path $sqlFile -Raw -Encoding UTF8
$batches = $text -split '(?im)^[ \t]*GO[ \t]*\r?\n'

Say "So batch tach duoc: $($batches.Count)"
Say ""

$cn = New-Object System.Data.SqlClient.SqlConnection($ConnStr)
$infoHandler = [System.Data.SqlClient.SqlInfoMessageEventHandler]{
    param($sender, $e)
    foreach ($m in $e.Errors) { Write-Host "    [SQL] $($m.Message)"; Add-Content -Path $out -Value "    [SQL] $($m.Message)" -Encoding UTF8 }
}
$cn.add_InfoMessage($infoHandler)

try {
    $cn.Open()
    Say "Ket noi OK (SQL Server $($cn.ServerVersion))"
    Say ""
} catch {
    Say "!!! LOI KET NOI: $($_.Exception.Message)"
    exit 1
}

$i = 0
$errCount = 0
foreach ($batch in $batches) {
    $i++
    $b = $batch.Trim()
    if ([string]::IsNullOrWhiteSpace($b)) { continue }
    # bo qua batch chi co comment
    $noComment = ($b -split "`r?`n" | Where-Object { $_.Trim() -notmatch '^\s*(--|/\*|\*|\*/)' }) -join "`n"
    if ([string]::IsNullOrWhiteSpace($noComment)) { continue }

    Say "--- BATCH $i ---"
    try {
        $cmd = $cn.CreateCommand()
        $cmd.CommandText = $b
        $cmd.CommandTimeout = 60
        $r = $cmd.ExecuteReader()
        $rows = 0
        do {
            while ($r.Read()) {
                $parts = @()
                for ($k = 0; $k -lt $r.FieldCount; $k++) {
                    $v = $r.GetValue($k)
                    if ($v -is [System.DBNull]) { $v = 'NULL' }
                    $parts += ($r.GetName($k) + '=' + [string]$v)
                }
                Say ("    " + ($parts -join '  |  '))
                $rows++
            }
        } while ($r.NextResult())
        $r.Close()
        if ($rows -gt 0) { Say "    --> $rows dong" }
    } catch {
        $errCount++
        Say "    !!! LOI: $($_.Exception.Message)"
    }
    Say ""
}

$cn.Close()
Say "============================================================"
if ($errCount -eq 0) { Say " HOAN TAT - khong co loi" } else { Say " HOAN TAT - CO $errCount LOI" }
Say "============================================================"
