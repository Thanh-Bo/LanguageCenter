# Kill mọi tiến trình LanguageCenter.Api đang giữ file DLL trong bin/Debug.
# Ghi kết quả ra .api-stop.txt ở thư mục gốc workspace.

$root = Split-Path -Parent $PSScriptRoot
$log = Join-Path $root '.api-stop.txt'

Remove-Item -Force -ErrorAction SilentlyContinue $log

$procs = Get-CimInstance Win32_Process | Where-Object {
    $_.Name -like '*LanguageCenter.Api*' -or
    ($null -ne $_.CommandLine -and $_.CommandLine -like '*LanguageCenter.Api*')
}

if (-not $procs) {
    'NO_PROCESS' | Out-File -FilePath $log -Encoding utf8
}
else {
    foreach ($p in $procs) {
        "KILL $($p.ProcessId) $($p.Name)" | Out-File -FilePath $log -Append -Encoding utf8
        & taskkill /F /PID $p.ProcessId 2>&1 | Out-File -FilePath $log -Append -Encoding utf8
    }
}

Start-Sleep -Seconds 2

# Kiểm tra lại
$conLai = Get-CimInstance Win32_Process | Where-Object {
    $_.Name -like '*LanguageCenter.Api*'
}

if ($conLai) {
    'STILL_RUNNING' | Out-File -FilePath $log -Append -Encoding utf8
}
else {
    'DONE' | Out-File -FilePath $log -Append -Encoding utf8
}
