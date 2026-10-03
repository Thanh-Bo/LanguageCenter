# Phase 2 - Dat thoi han access token (phut) vao User Secrets.
# Dung de test CASE 10 (token het han): -Minutes 0 -> token het han ngay lap tuc.
param([int]$Minutes = 60)

$workspaceRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$projectPath = Join-Path $workspaceRoot 'backend\LanguageCenter.Api\LanguageCenter.Api.csproj'

$null = & dotnet user-secrets set 'Jwt:AccessTokenExpirationMinutes' $Minutes --project $projectPath 2>&1

$secretsId = (Select-String -Path $projectPath -Pattern '<UserSecretsId>(.*?)</UserSecretsId>').Matches[0].Groups[1].Value
$secretsFile = Join-Path $env:APPDATA "Microsoft\UserSecrets\$secretsId\secrets.json"
$obj = (Get-Content $secretsFile -Raw) | ConvertFrom-Json
$value = $obj.'Jwt:AccessTokenExpirationMinutes'

Write-Output "EXIT_CODE = $LASTEXITCODE"
Write-Output "Jwt:AccessTokenExpirationMinutes = $value"
Write-Output 'EXPIRY_SETTING_DONE'
