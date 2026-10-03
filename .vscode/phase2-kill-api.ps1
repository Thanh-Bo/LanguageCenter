# Phase 2 - Kill TAT CA process API + kiem tra cong 5204 con bi chiem khong.
$ErrorActionPreference = 'Continue'

$before = @(Get-Process -Name 'LanguageCenter.Api' -ErrorAction SilentlyContinue)
Write-Output ("API processes BEFORE = " + $before.Count + " | pids = " + (($before | ForEach-Object { $_.Id }) -join ','))

foreach ($p in $before) {
    Write-Output ("  killing pid " + $p.Id + " (started " + $p.StartTime + ")")
    Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue
}

# Kill ca dotnet.exe dang chay 'run' (neu con)
$dotnets = @(Get-Process -Name 'dotnet' -ErrorAction SilentlyContinue)
Write-Output ("dotnet processes = " + $dotnets.Count)

Start-Sleep -Seconds 3

$after = @(Get-Process -Name 'LanguageCenter.Api' -ErrorAction SilentlyContinue)
Write-Output ("API processes AFTER = " + $after.Count)

# Cong 5204 con listen khong?
$listening = @(Get-NetTCPConnection -LocalPort 5204 -State Listen -ErrorAction SilentlyContinue)
if ($listening.Count -gt 0) {
    Write-Output ("PORT 5204 STILL LISTENING by pid(s): " + (($listening | ForEach-Object { $_.OwningProcess }) -join ','))
} else {
    Write-Output 'PORT 5204 FREE'
}

try {
    $r = Invoke-WebRequest -Uri 'http://localhost:5204/health' -UseBasicParsing -TimeoutSec 3 -ErrorAction Stop
    Write-Output ('STILL RESPONDING: ' + $r.Content)
} catch {
    Write-Output 'API khong con phan hoi -> da dung hoan toan'
}

Write-Output 'KILL_API_DONE'
