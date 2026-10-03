# Phase 2 - Don dep file/cong cu tam sau khi hoan tat.
$ErrorActionPreference = 'Continue'

$workspaceRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$projectPath = Join-Path $workspaceRoot 'backend\LanguageCenter.Api\LanguageCenter.Api.csproj'
$lines = [System.Collections.Generic.List[string]]::new()

# 1. XOA CONG CU TAM sinh password hang loat (ly do bao mat)
$tempTool = Join-Path $workspaceRoot '.tmp-devpassword'
if (Test-Path $tempTool) {
    Remove-Item -Recurse -Force $tempTool -ErrorAction SilentlyContinue
    $lines.Add("REMOVED temp password tool: " + (-not (Test-Path $tempTool)))
} else {
    $lines.Add('temp password tool already absent')
}

# 2. Xoa script chan doan tam
$tempScripts = @(
    'phase2-recon.ps1', 'phase2-recon2.ps1', 'phase2-recon3.ps1',
    'phase2-secrets.ps1', 'phase2-secrets-verify.ps1', 'phase2-secret-debug.ps1'
)
foreach ($name in $tempScripts) {
    $path = Join-Path $PSScriptRoot $name
    if (Test-Path $path) { Remove-Item -Force $path -ErrorAction SilentlyContinue; $lines.Add("REMOVED script: $name") }
}

# 3. Xoa output tam (KHONG xoa phase2-test-report.txt / phase2-expired-report.txt / secrets-check.txt)
$tempOutputs = @(
    'recon2-output.txt', 'recon3-output.txt', 'secret-debug.txt',
    'secrets-output.txt', 'secret-final-out.txt',
    'seed-password-out.txt', 'restore-hv004-out.txt'
)
foreach ($name in $tempOutputs) {
    $path = Join-Path $PSScriptRoot $name
    if (Test-Path $path) { Remove-Item -Force $path -ErrorAction SilentlyContinue; $lines.Add("REMOVED output: $name") }
}

# 3b. Xoa file Postman (nguoi dung khong can import)
$postmanDir = Join-Path $workspaceRoot 'docs\postman'
if (Test-Path $postmanDir) {
    Remove-Item -Recurse -Force $postmanDir -ErrorAction SilentlyContinue
    $lines.Add("REMOVED docs/postman: " + (-not (Test-Path $postmanDir)))
}
$postmanScript = Join-Path $PSScriptRoot 'phase2-validate-postman.ps1'
if (Test-Path $postmanScript) { Remove-Item -Force $postmanScript -ErrorAction SilentlyContinue; $lines.Add('REMOVED script: phase2-validate-postman.ps1') }
$postmanReport = Join-Path $PSScriptRoot 'postman-check.txt'
if (Test-Path $postmanReport) { Remove-Item -Force $postmanReport -ErrorAction SilentlyContinue; $lines.Add('REMOVED output: postman-check.txt') }

# 4. Bo override expiry trong User Secrets -> appsettings.json la nguon duy nhat (60 phut)
$null = & dotnet user-secrets remove 'Jwt:AccessTokenExpirationMinutes' --project $projectPath 2>&1
$lines.Add("Removed Jwt:AccessTokenExpirationMinutes override (exit=$LASTEXITCODE)")

# 5. Xac nhan lai secret store: CHI con Jwt:Key
$secretsId = (Select-String -Path $projectPath -Pattern '<UserSecretsId>(.*?)</UserSecretsId>').Matches[0].Groups[1].Value
$secretsFile = Join-Path $env:APPDATA "Microsoft\UserSecrets\$secretsId\secrets.json"
$keys = @(((Get-Content $secretsFile -Raw) | ConvertFrom-Json).PSObject.Properties.Name)
$lines.Add('Remaining secrets = ' + ($keys -join ', '))

# 6. Liet ke file con lai trong .vscode
$lines.Add('Remaining files in .vscode:')
Get-ChildItem -Path $PSScriptRoot -File | Sort-Object Name | ForEach-Object { $lines.Add('  ' + $_.Name) }

$lines | Set-Content -Path (Join-Path $PSScriptRoot 'cleanup-report.txt') -Encoding utf8
$lines | ForEach-Object { Write-Output $_ }
