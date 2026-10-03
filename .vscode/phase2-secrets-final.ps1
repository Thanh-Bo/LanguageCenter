# Phase 2 - Chot cau hinh User Secrets cho JWT.
# - Xoa key probe tam
# - Dam bao Jwt:Key ton tai (ho tro CA 2 dang: flat "Jwt:Key" cua .NET 10 va nested {"Jwt":{"Key"}})
# - KHONG in gia tri key, chi bao do dai
$ErrorActionPreference = 'Continue'

$workspaceRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$projectPath = Join-Path $workspaceRoot 'backend\LanguageCenter.Api\LanguageCenter.Api.csproj'
$reportPath = Join-Path $PSScriptRoot 'secrets-check.txt'
$lines = [System.Collections.Generic.List[string]]::new()

$secretsId = (Select-String -Path $projectPath -Pattern '<UserSecretsId>(.*?)</UserSecretsId>').Matches[0].Groups[1].Value
$secretsFile = Join-Path $env:APPDATA "Microsoft\UserSecrets\$secretsId\secrets.json"

function Get-JwtKeyValue {
    param([string]$File)
    if (-not (Test-Path $File)) { return $null }
    $raw = Get-Content $File -Raw
    if ([string]::IsNullOrWhiteSpace($raw)) { return $null }
    $obj = $raw | ConvertFrom-Json

    # Dang flat cua .NET 10
    if ($obj.PSObject.Properties.Name -contains 'Jwt:Key') { return [string]$obj.'Jwt:Key' }
    # Dang nested truyen thong
    if ($obj.Jwt -and $obj.Jwt.Key) { return [string]$obj.Jwt.Key }
    return $null
}

# 1. Xoa key probe tam neu con
$existingKeys = @()
if (Test-Path $secretsFile) {
    $rawObj = (Get-Content $secretsFile -Raw) | ConvertFrom-Json
    $existingKeys = @($rawObj.PSObject.Properties.Name)
}
if ($existingKeys -contains 'Jwt:Probe') {
    $null = & dotnet user-secrets remove 'Jwt:Probe' --project $projectPath 2>&1
    $lines.Add('ACTION = removed probe key Jwt:Probe')
} else {
    $lines.Add('ACTION = no probe key to remove')
}

# 2. Sinh key neu chua co
$key = Get-JwtKeyValue -File $secretsFile
if (-not $key) {
    $bytes = [byte[]]::new(64)
    [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
    $null = & dotnet user-secrets set 'Jwt:Key' ([Convert]::ToBase64String($bytes)) --project $projectPath 2>&1
    $lines.Add("ACTION = generated new 64-byte key (exit=$LASTEXITCODE)")
    $key = Get-JwtKeyValue -File $secretsFile
} else {
    $lines.Add('ACTION = existing Jwt:Key kept as-is')
}

# 3. Xac nhan
$lines.Add("UserSecretsId = $secretsId")
$lines.Add("Secrets file  = $secretsFile")
$lines.Add("Outside repo  = " + (-not $secretsFile.StartsWith($workspaceRoot)))

$finalKeys = @()
if (Test-Path $secretsFile) {
    $finalKeys = @(((Get-Content $secretsFile -Raw) | ConvertFrom-Json).PSObject.Properties.Name)
}
$lines.Add("Keys in store = " + ($finalKeys -join ', '))

if ($key) {
    $keyBytes = [Convert]::FromBase64String($key)
    $lines.Add("Jwt:Key PRESENT - " + $keyBytes.Length + " bytes (minimum 32)")
    if ($keyBytes.Length -ge 32) { $lines.Add('RESULT = OK') } else { $lines.Add('RESULT = KEY_TOO_SHORT') }
} else {
    $lines.Add('RESULT = JWT_KEY_MISSING')
}

$lines | Set-Content -Path $reportPath -Encoding utf8
$lines | ForEach-Object { Write-Output $_ }
