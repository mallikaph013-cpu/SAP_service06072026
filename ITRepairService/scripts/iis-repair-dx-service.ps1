param(
    [string]$TargetPath = 'C:\interpub2\DX_service',
    [string]$SiteName   = 'Default Web Site',
    [string]$AppAlias   = 'DX_service',
    [string]$AppPoolName = 'DX_service_pool'
)

$ErrorActionPreference = 'Stop'

$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    throw 'Please run this script as Administrator.'
}

Import-Module WebAdministration

# --- Ensure App Pool exists (No Managed Code for ASP.NET Core) ---
if (-not (Test-Path "IIS:\AppPools\$AppPoolName")) {
    Write-Host "Creating app pool: $AppPoolName"
    New-WebAppPool -Name $AppPoolName | Out-Null
    Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name managedRuntimeVersion -Value ''
    Write-Host "App pool created."
} else {
    Write-Host "App pool exists: $AppPoolName"
}

# --- Ensure the IIS application exists under the site ---
$appPath = "IIS:\Sites\$SiteName\$AppAlias"
if (-not (Test-Path $appPath)) {
    Write-Host "Creating IIS application /$AppAlias under '$SiteName' -> $TargetPath"
    New-WebApplication -Site $SiteName -Name $AppAlias -PhysicalPath $TargetPath -ApplicationPool $AppPoolName | Out-Null
    Write-Host "IIS application created."
} else {
    Write-Host "IIS application already exists: /$AppAlias"
    Set-ItemProperty $appPath -Name applicationPool -Value $AppPoolName
    Set-ItemProperty "$appPath/" -Name physicalPath -Value $TargetPath
    Write-Host "Updated physical path and app pool."
}

# --- Ensure ASPNETCORE_ENVIRONMENT=Production on app pool ---
$envPath = "MACHINE/WEBROOT/APPHOST"
$filter  = "system.applicationHost/applicationPools/add[@name='$AppPoolName']/environmentVariables/add[@name='ASPNETCORE_ENVIRONMENT']"

$existing = Get-WebConfigurationProperty -PSPath $envPath -Filter $filter -Name 'value' -ErrorAction SilentlyContinue
if ($existing) {
    Set-WebConfigurationProperty -PSPath $envPath -Filter $filter -Name 'value' -Value 'Production'
    Write-Host "Updated ASPNETCORE_ENVIRONMENT=Production"
} else {
    Add-WebConfigurationProperty -PSPath $envPath `
        -Filter "system.applicationHost/applicationPools/add[@name='$AppPoolName']/environmentVariables" `
        -Name '.' -Value @{name='ASPNETCORE_ENVIRONMENT';value='Production'}
    Write-Host "Set ASPNETCORE_ENVIRONMENT=Production"
}

# --- Ensure logs directory exists for stdout logs ---
$logsDir = Join-Path $TargetPath 'logs'
if (-not (Test-Path $logsDir)) {
    New-Item -ItemType Directory -Path $logsDir | Out-Null
    Write-Host "Created logs directory."
}

# --- Recycle app pool ---
Restart-WebAppPool -Name $AppPoolName
Write-Host "Recycled app pool: $AppPoolName"

Start-Sleep -Seconds 2

# Show recent Application event logs related to ASP.NET Core/IIS
Write-Host '--- Recent startup logs ---'
Get-WinEvent -LogName Application -MaxEvents 100 |
    Where-Object {
        $_.ProviderName -match 'IIS|ASP.NET Core Module|AspNetCore|\.NET Runtime' -or
        $_.Message -match 'ITRepairService|DX_service|500\.30|failed|exception'
    } |
    Select-Object -First 10 TimeCreated, ProviderName, Id, LevelDisplayName, Message |
    Format-List
