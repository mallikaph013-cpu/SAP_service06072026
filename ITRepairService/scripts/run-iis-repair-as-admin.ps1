$ErrorActionPreference = 'Stop'

$scriptPath = Join-Path $PSScriptRoot 'iis-repair-dx-service.ps1'
if (-not (Test-Path $scriptPath)) {
    throw "Script not found: $scriptPath"
}

$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if ($isAdmin) {
    & $scriptPath
    exit $LASTEXITCODE
}

Write-Host 'Requesting Administrator privileges (UAC)...'
Start-Process -FilePath 'powershell.exe' -Verb RunAs -ArgumentList @(
    '-NoProfile',
    '-ExecutionPolicy', 'Bypass',
    '-File', ('"' + $scriptPath + '"')
)
