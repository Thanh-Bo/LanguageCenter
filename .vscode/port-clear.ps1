# Giai phong cong 5204: kill moi process dang listen + moi process LanguageCenter.Api
$ErrorActionPreference = 'Continue'

Write-Output '--- PORT CLEAR ---'

$conns = @(Get-NetTCPConnection -LocalPort 5204 -State Listen -ErrorAction SilentlyContinue)
Write-Output ('listen owners = ' + (($conns | ForEach-Object { $_.OwningProcess }) -join ','))

foreach ($c in $conns) {
    try {
        $proc = Get-Process -Id $c.OwningProcess -ErrorAction Stop
        Write-Output ('killing pid ' + $proc.Id + ' (' + $proc.ProcessName + ')')
        Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
    }
    catch {
        Write-Output ('cannot kill pid ' + $c.OwningProcess)
    }
}

$apis = @(Get-Process -Name 'LanguageCenter.Api' -ErrorAction SilentlyContinue)
foreach ($p in $apis) {
    Write-Output ('killing API pid ' + $p.Id)
    Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue
}

Start-Sleep -Seconds 3

$left = @(Get-NetTCPConnection -LocalPort 5204 -State Listen -ErrorAction SilentlyContinue)
Write-Output ('remaining listeners = ' + $left.Count)
Write-Output 'PORT_CLEAR_DONE'
