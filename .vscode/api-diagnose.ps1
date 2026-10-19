# Chẩn đoán: liệt kê tiến trình đang giữ cổng 5204 và mọi tiến trình dotnet/LanguageCenter.Api.

$root = Split-Path -Parent $PSScriptRoot
$log = Join-Path $root '.api-diag.txt'

Remove-Item -Force -ErrorAction SilentlyContinue $log

'--- TIẾN TRÌNH THEO TÊN ---' | Out-File -FilePath $log -Encoding utf8
Get-Process | Where-Object {
    $_.ProcessName -like '*LanguageCenter*' -or $_.ProcessName -eq 'dotnet'
} | Select-Object Id, ProcessName, Path | Format-Table -AutoSize | Out-String | Out-File -FilePath $log -Append -Encoding utf8

'--- WIN32_PROCESS (LanguageCenter) ---' | Out-File -FilePath $log -Append -Encoding utf8
Get-CimInstance Win32_Process | Where-Object { $_.Name -like '*LanguageCenter*' } |
    Select-Object ProcessId, Name, ExecutablePath, CommandLine |
    Format-List | Out-String | Out-File -FilePath $log -Append -Encoding utf8

'--- CỔNG 5204 ---' | Out-File -FilePath $log -Append -Encoding utf8
$conn = Get-NetTCPConnection -LocalPort 5204 -ErrorAction SilentlyContinue
if ($conn) {
    $conn | Select-Object LocalAddress, LocalPort, State, OwningProcess | Format-Table -AutoSize | Out-String |
        Out-File -FilePath $log -Append -Encoding utf8
}
else {
    'KHÔNG CÓ KẾT NỐI NÀO TRÊN CỔNG 5204' | Out-File -FilePath $log -Append -Encoding utf8
}

'--- DANH SÁCH dotnet ---' | Out-File -FilePath $log -Append -Encoding utf8
Get-CimInstance Win32_Process -Filter "Name='dotnet.exe'" |
    Select-Object ProcessId, CommandLine | Format-List | Out-String |
    Out-File -FilePath $log -Append -Encoding utf8

'DIAG_DONE' | Out-File -FilePath $log -Append -Encoding utf8
