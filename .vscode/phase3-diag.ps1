# Phase 3 - chan doan nhanh: API co song khong, login co duoc khong
$ErrorActionPreference = 'Continue'
$BaseUrl = 'http://localhost:5204'
$reportPath = Join-Path $PSScriptRoot 'phase3-diag.txt'
$lines = [System.Collections.Generic.List[string]]::new()

function Call {
    param([string]$Method, [string]$Path, [string]$Body)
    try {
        $params = @{ Uri = "$BaseUrl$Path"; Method = $Method; UseBasicParsing = $true; ErrorAction = 'Stop' }
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

$lines.Add('--- GET /health ---')
$h = Call 'GET' '/health' $null
$lines.Add("status=" + $h.Status + " body=" + $h.Text)

$lines.Add('--- POST /api/auth/login (ADMIN001) ---')
$body = '{"username":"ADMIN001","password":"LanguageCenter@2026"}'
$l = Call 'POST' '/api/auth/login' $body
$lines.Add("status=" + $l.Status)
$lines.Add("body(first 300)=" + $l.Text.Substring(0, [Math]::Min(300, $l.Text.Length)))

$lines.Add('--- GET /api/admin/accounts (khong token) ---')
$a = Call 'GET' '/api/admin/accounts' $null
$lines.Add("status=" + $a.Status)

# Kiem tra assembly System.Net.Http
$lines.Add('--- Assembly System.Net.Http ---')
try {
    Add-Type -AssemblyName System.Net.Http -ErrorAction Stop
    $lines.Add("Add-Type System.Net.Http OK -> " + [System.Net.Http.HttpClient].FullName)
} catch {
    $lines.Add("FAILED: " + $_.Exception.Message)
}

$lines | Set-Content -Path $reportPath -Encoding utf8
$lines | ForEach-Object { Write-Output $_ }
