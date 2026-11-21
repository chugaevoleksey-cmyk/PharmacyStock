$ErrorActionPreference = 'Stop'
function Run-Dotnet([string[]] $Arguments) {
  & dotnet @Arguments
  if ($LASTEXITCODE -ne 0) { throw "dotnet failed: $Arguments" }
}
Run-Dotnet @('restore')
Run-Dotnet @('format', '--verify-no-changes', '--no-restore')
Run-Dotnet @('build', '-c', 'Release', '--no-restore', '-warnaserror')
Run-Dotnet @('test', '-c', 'Release', '--no-build', '--no-restore',
  '/p:CollectCoverage=true', '/p:CoverletOutputFormat=cobertura',
  '/p:Include=[PharmacyStock.Core]*', '/p:Threshold=70',
  '/p:ThresholdType=line', '/p:ThresholdStat=total')
& "$PSScriptRoot/coverage.ps1"
