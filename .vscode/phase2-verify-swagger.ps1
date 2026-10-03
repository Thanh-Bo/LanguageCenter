# Phase 2 - Kiem chung Swagger da ap dung Bearer security scheme (§19).
$ErrorActionPreference = 'Continue'

$reportPath = Join-Path $PSScriptRoot 'swagger-check.txt'
$lines = [System.Collections.Generic.List[string]]::new()

$spec = Invoke-RestMethod -Uri 'http://localhost:5204/swagger/v1/swagger.json' -UseBasicParsing
$raw = ($spec | ConvertTo-Json -Depth 30)

$lines.Add('=== Swagger / OpenAPI check ===')

# 1. securitySchemes
$schemeNames = @($spec.components.securitySchemes.PSObject.Properties.Name)
$lines.Add('securitySchemes = ' + ($schemeNames -join ', '))
if ($spec.components.securitySchemes.Bearer) {
    $b = $spec.components.securitySchemes.Bearer
    $lines.Add('  Bearer.type      = ' + $b.type)
    $lines.Add('  Bearer.scheme    = ' + $b.scheme)
    $lines.Add('  Bearer.bearerFormat = ' + $b.bearerFormat)
}

# 2. security requirement toan cuc
$globalSecurity = @($spec.security)
$lines.Add('global security requirement count = ' + $globalSecurity.Count)
if ($globalSecurity.Count -gt 0) {
    $lines.Add('  -> ' + (($globalSecurity | ForEach-Object { ($_.PSObject.Properties.Name -join '+') }) -join '; '))
}

# 3. cac endpoint moi cua Phase 2 co ton tai khong
$paths = @($spec.paths.PSObject.Properties.Name) | Sort-Object
$lines.Add('total paths = ' + $paths.Count)
foreach ($p in $paths) { $lines.Add('  ' + $p) }

$expect = @('/api/auth/login', '/api/auth/me', '/api/auth/change-password',
            '/api/development/authenticated', '/api/development/admin-only', '/api/development/teacher-only',
            '/health', '/health/database', '/api/development/database-info')
$missing = @($expect | Where-Object { $paths -notcontains $_ })
if ($missing.Count -eq 0) {
    $lines.Add('ALL_EXPECTED_ENDPOINTS_PRESENT = TRUE')
} else {
    $lines.Add('MISSING_ENDPOINTS = ' + ($missing -join ', '))
}

# 4. response cua /api/auth/me co khai bao 401 khong
$lines.Add('/api/auth/me responses = ' + ((@($spec.paths.'/api/auth/me'.get.responses.PSObject.Properties.Name)) -join ', '))
$lines.Add('/api/auth/login responses = ' + ((@($spec.paths.'/api/auth/login'.post.responses.PSObject.Properties.Name)) -join ', '))

$lines | Set-Content -Path $reportPath -Encoding utf8
$lines | ForEach-Object { Write-Output $_ }
