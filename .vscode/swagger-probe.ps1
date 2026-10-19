# Kiem tra Swagger dang phan hoi the nao
$ErrorActionPreference = 'Continue'
$out = @()

foreach ($url in @(
        'http://localhost:5204/swagger/v1/swagger.json',
        'http://localhost:5204/swagger/index.html',
        'http://localhost:5204/api/development/database-info')) {
    try {
        $r = Invoke-WebRequest -Uri $url -UseBasicParsing -TimeoutSec 15 -ErrorAction Stop
        $out += ("URL=$url STATUS=" + [int]$r.StatusCode + " LEN=" + $r.Content.Length)
        if ($r.Content.Length -lt 800) { $out += ('  BODY: ' + $r.Content) }
    }
    catch {
        $resp = $_.Exception.Response
        if ($null -ne $resp) {
            $sr = New-Object System.IO.StreamReader($resp.GetResponseStream())
            $body = $sr.ReadToEnd()
            $out += ("URL=$url STATUS=" + [int]$resp.StatusCode + " BODY=" + $body)
        }
        else {
            $out += ("URL=$url ERROR=" + $_.Exception.Message)
        }
    }
}

$out
