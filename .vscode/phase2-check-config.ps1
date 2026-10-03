# Phase 2 - Do cau hinh JWT hieu dung (secret store + hanh vi that cua API).
$ErrorActionPreference = 'Continue'

$workspaceRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$projectPath = Join-Path $workspaceRoot 'backend\LanguageCenter.Api\LanguageCenter.Api.csproj'
$reportPath = Join-Path $PSScriptRoot 'config-check.txt'
$lines = [System.Collections.Generic.List[string]]::new()

$lines.Add('=== [1] User Secrets store ===')
$secretsId = (Select-String -Path $projectPath -Pattern '<UserSecretsId>(.*?)</UserSecretsId>').Matches[0].Groups[1].Value
$secretsFile = Join-Path $env:APPDATA "Microsoft\UserSecrets\$secretsId\secrets.json"
if (Test-Path $secretsFile) {
    $json = Get-Content $secretsFile -Raw | ConvertFrom-Json
    foreach ($p in $json.PSObject.Properties) {
        if ($p.Name -eq 'Jwt:Key') {
            $lines.Add("  $($p.Name) = <REDACTED, $(([string]$p.Value).Length) chars>")
        } else {
            $lines.Add("  $($p.Name) = $($p.Value)")
        }
    }
} else {
    $lines.Add('  secrets.json NOT FOUND')
}

$lines.Add('')
$lines.Add('=== [2] appsettings.json (Jwt section) ===')
# .NET cho phep comment trong appsettings.json nhung ConvertFrom-Json cua PS 5.1 thi khong,
# nen phai bo cac dong comment truoc khi parse.
$rawAppSettings = Get-Content (Join-Path $workspaceRoot 'backend\LanguageCenter.Api\appsettings.json') -Raw
$noComments = (($rawAppSettings -split "`r?`n") | Where-Object { $_.Trim() -notmatch '^\s*//' }) -join "`n"
$appSettings = $noComments | ConvertFrom-Json
$lines.Add('  Issuer = ' + $appSettings.Jwt.Issuer)
$lines.Add('  Audience = ' + $appSettings.Jwt.Audience)
$lines.Add('  AccessTokenExpirationMinutes = ' + $appSettings.Jwt.AccessTokenExpirationMinutes)
$lines.Add('  Key = ' + $(if ($appSettings.Jwt.Key) { '<PRESENT - KHONG DUOC PHEP!' } else { '<absent - dung thiet ke>' }))

$lines.Add('')
$lines.Add('=== [3] Hanh vi THAT cua API dang chay ===')
$body = (@{ username = 'ADMIN001'; password = 'LanguageCenter@2026' } | ConvertTo-Json -Compress)
try {
    $login = Invoke-WebRequest -Uri 'http://localhost:5204/api/auth/login' -Method Post `
        -ContentType 'application/json; charset=utf-8' `
        -Body ([System.Text.Encoding]::UTF8.GetBytes($body)) -UseBasicParsing -ErrorAction Stop
    $parsed = $login.Content | ConvertFrom-Json
    $claims = $parsed.accessToken.Split('.')[1].Replace('-', '+').Replace('_', '/')
    switch ($claims.Length % 4) { 2 { $claims += '==' } 3 { $claims += '=' } }
    $payload = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($claims)) | ConvertFrom-Json

    $iat = [DateTimeOffset]::FromUnixTimeSeconds([long]$payload.iat).UtcDateTime
    $exp = [DateTimeOffset]::FromUnixTimeSeconds([long]$payload.exp).UtcDateTime
    $lifetime = ($exp - $iat).TotalMinutes

    $lines.Add("  login status = 200")
    $lines.Add("  iat = $($iat.ToString('u'))")
    $lines.Add("  exp = $($exp.ToString('u'))")
    $lines.Add("  ==> THOI HAN TOKEN HIEU DUNG = $lifetime phut")
} catch {
    $lines.Add('  login FAILED: ' + $_.Exception.Message)
}

$lines | Set-Content -Path $reportPath -Encoding utf8
$lines | ForEach-Object { Write-Output $_ }
