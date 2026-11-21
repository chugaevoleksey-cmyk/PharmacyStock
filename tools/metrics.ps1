$ErrorActionPreference = 'Stop'
$target = Join-Path (Get-Location) 'coverage/metrics-tool'
New-Item -ItemType Directory -Force $target | Out-Null
Set-Content (Join-Path $target 'Directory.Build.props') '<Project />'
Copy-Item "$PSScriptRoot/metrics/Program.cs.txt" (Join-Path $target 'Program.cs')
Copy-Item "$PSScriptRoot/metrics/Metrics.csproj.txt" (Join-Path $target 'Metrics.csproj')
foreach ($part in @('src','tests')) {
  $text = & dotnet run --project (Join-Path $target 'Metrics.csproj') -- (Join-Path (Get-Location) $part)
  if ($LASTEXITCODE -ne 0) { throw 'Metrics failed' }
  $joined = $text -join "`n"
  $joined.Substring($joined.IndexOf('{')) | Set-Content -Encoding utf8 "coverage/metrics-$part.json"
}
