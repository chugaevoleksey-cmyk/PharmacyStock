$ErrorActionPreference = 'Stop'
[xml]$report = Get-Content -Raw 'tests/PharmacyStock.Tests/coverage.cobertura.xml'
$culture = [Globalization.CultureInfo]::InvariantCulture
$line = [double]::Parse($report.coverage.'line-rate', $culture) * 100
$branch = [double]::Parse($report.coverage.'branch-rate', $culture) * 100
if ($line -lt 70) { throw "Line coverage $line below 70" }
New-Item -ItemType Directory -Force coverage | Out-Null
$rows = foreach ($item in $report.coverage.packages.package.classes.class) {
  $name = [Net.WebUtility]::HtmlEncode($item.name)
  "<tr><td>$name</td><td>$($item.'line-rate')</td><td>$($item.'branch-rate')</td></tr>"
}
$html = @"
<!doctype html><html lang="uk"><meta charset="utf-8"><title>PharmacyStock Coverage</title>
<style>body{font:18px Arial;margin:36px;max-width:1100px}td,th{padding:7px;border-bottom:1px solid #ddd;text-align:left}</style>
<h1>PharmacyStock — покриття Core</h1><p>Домен і прикладний шар; Infrastructure та App виключено.</p>
<h2>Рядки: $($line.ToString('F2',$culture))% · Гілки: $($branch.ToString('F2',$culture))%</h2>
<p>Рядки $($report.coverage.'lines-covered') / $($report.coverage.'lines-valid'); гілки $($report.coverage.'branches-covered') / $($report.coverage.'branches-valid'). Поріг 70%: пройдено.</p>
<table><tr><th>Тип</th><th>Частка рядків</th><th>Частка гілок</th></tr>$($rows -join "`n")</table></html>
"@
Set-Content -Encoding utf8 coverage/index.html $html
Write-Output "Coverage: line=$line branch=$branch"
