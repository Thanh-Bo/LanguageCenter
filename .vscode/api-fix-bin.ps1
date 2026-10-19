# Xoá các DLL bị kẹt trong bin của API (nếu tiến trình cũ đã chết thì xoá được luôn).
# Nếu vẫn bị khoá, script ghi lại PID đang giữ file (nhờ MSBuild-like probing nếu có).

$root = Split-Path -Parent $PSScriptRoot
$log = Join-Path $root '.api-fix-bin.txt'
Remove-Item -Force -ErrorAction SilentlyContinue $log

$binApi = Join-Path $root 'backend\LanguageCenter.Api\bin\Debug\net10.0'

"--- THƯ MỤC: $binApi ---" | Out-File -FilePath $log -Encoding utf8

if (-not (Test-Path $binApi)) {
    'KHONG_TIM_THAY_THU_MUC_BIN' | Out-File -FilePath $log -Append -Encoding utf8
    return
}

foreach ($ten in @('LanguageCenter.Application.dll', 'LanguageCenter.Infrastructure.dll')) {
    $f = Join-Path $binApi $ten

    if (-not (Test-Path $f)) {
        "SKIP (không tồn tại): $ten" | Out-File -FilePath $log -Append -Encoding utf8
        continue
    }

    try {
        Remove-Item -LiteralPath $f -Force -ErrorAction Stop
        "DELETED: $ten" | Out-File -FilePath $log -Append -Encoding utf8
    }
    catch {
        "LOCKED: $ten -> $($_.Exception.Message)" | Out-File -FilePath $log -Append -Encoding utf8
    }
}

'--- TIẾN TRÌNH LanguageCenter* ---' | Out-File -FilePath $log -Append -Encoding utf8
$ds = Get-CimInstance Win32_Process | Where-Object { $_.Name -like '*LanguageCenter*' }
if ($ds) {
    $ds | Select-Object ProcessId, Name, ExecutablePath | Format-List | Out-String |
        Out-File -FilePath $log -Append -Encoding utf8
}
else {
    'KHONG_CO' | Out-File -FilePath $log -Append -Encoding utf8
}

'FIX_DONE' | Out-File -FilePath $log -Append -Encoding utf8
