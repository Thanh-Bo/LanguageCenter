# Phase 2 - CASE 10: token het han phai bi tu choi (401).
# YEU CAU: API dang chay voi Jwt:AccessTokenExpirationMinutes = 0.
$ErrorActionPreference = 'Continue'

$BaseUrl = 'http://localhost:5204'
$DevPassword = 'LanguageCenter@2026'
$reportPath = Join-Path $PSScriptRoot 'phase2-expired-report.txt'
$log = [System.Collections.Generic.List[string]]::new()

function Invoke-Api {
    param([string]$Method, [string]$Path, [string]$Body, [string]$Token)
    $headers = @{}
    if ($Token) { $headers['Authorization'] = "Bearer $Token" }
    try {
        $params = @{ Uri = "$BaseUrl$Path"; Method = $Method; Headers = $headers; UseBasicParsing = $true; ErrorAction = 'Stop' }
        if ($Body) {
            $params['ContentType'] = 'application/json; charset=utf-8'
            $params['Body'] = [System.Text.Encoding]::UTF8.GetBytes($Body)
        }
        $r = Invoke-WebRequest @params
        return @{ Status = [int]$r.StatusCode; Text = $r.Content }
    } catch {
        $resp = $_.Exception.Response
        if ($null -eq $resp) { return @{ Status = -1; Text = $_.Exception.Message } }
        $reader = New-Object System.IO.StreamReader($resp.GetResponseStream(), [System.Text.Encoding]::UTF8)
        return @{ Status = [int]$resp.StatusCode; Text = $reader.ReadToEnd() }
    }
}

$log.Add('============================================================')
$log.Add(' CASE 10 - TOKEN HET HAN (Jwt:AccessTokenExpirationMinutes = 0)')
$log.Add('============================================================')

$body = (@{ username = 'ADMIN001'; password = $DevPassword } | ConvertTo-Json -Compress)
$login = Invoke-Api -Method 'POST' -Path '/api/auth/login' -Body $body
$log.Add("Login status = " + $login.Status)

if ($login.Status -ne 200) {
    $log.Add('KHONG LAY DUOC TOKEN -> khong the test CASE 10')
    $log.Add("body: " + $login.Text)
    $log | Set-Content -Path $reportPath -Encoding utf8
    $log | ForEach-Object { Write-Output $_ }
    exit 1
}

$json = $login.Text | ConvertFrom-Json
$token = $json.accessToken

$claims = $token.Split('.')[1].Replace('-', '+').Replace('_', '/')
switch ($claims.Length % 4) { 2 { $claims += '==' } 3 { $claims += '=' } }
$payload = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($claims)) | ConvertFrom-Json

$exp = [DateTimeOffset]::FromUnixTimeSeconds([long]$payload.exp).UtcDateTime
$iat = [DateTimeOffset]::FromUnixTimeSeconds([long]$payload.iat).UtcDateTime
$log.Add("expiresAt (API tra ve) = $($json.expiresAt)")
$log.Add("iat = $($iat.ToString('u'))")
$log.Add("exp = $($exp.ToString('u'))")
$log.Add("Hien tai UTC = $((Get-Date).ToUniversalTime().ToString('u'))")

$me = Invoke-Api -Method 'GET' -Path '/api/auth/me' -Token $token
$log.Add("GET /api/auth/me voi token het han = " + $me.Status)
$log.Add("body: " + $me.Text)

if ($me.Status -eq 401) {
    $log.Add('RESULT = PASS - token het han bi tu choi 401')
    $log.Add('WWW-Authenticate xac nhan token bi coi la het han.')
} else {
    $log.Add('RESULT = FAIL - token het han VAN duoc chap nhan (status ' + $me.Status + ')')
}

$log | Set-Content -Path $reportPath -Encoding utf8
$log | ForEach-Object { Write-Output $_ }
