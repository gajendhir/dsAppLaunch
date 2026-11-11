<#
Builds the Microsoft Store package (MSIX) for dsAppLaunch.
  .\Store\build-msix.ps1              -> Store\out\dsAppLaunch_<ver>_x64.msix   (unsigned; upload to Partner Center, the Store signs it)
  .\Store\build-msix.ps1 -TestSign    -> also signs with a self-signed cert for local sideload testing (never upload that one)
Version is Major.Minor.<days since 2000-01-01>.0, same scheme as dsAppLaunch.vbproj; the 4th segment must stay 0 for the Store.
#>
param([switch]$TestSign)
$ErrorActionPreference = 'Stop'
$root  = Split-Path $PSScriptRoot -Parent
$sdk   = Get-ChildItem 'C:\Program Files (x86)\Windows Kits\10\bin\*\x64\makeappx.exe' | Sort-Object FullName | Select-Object -Last 1
if (-not $sdk) { throw 'Windows 10/11 SDK (makeappx.exe) not found.' }
$bin   = $sdk.DirectoryName

[xml]$proj = Get-Content (Join-Path $root 'dsAppLaunch.vbproj')
$major = $proj.Project.PropertyGroup.MajorVersion | Where-Object { $_ } | Select-Object -First 1
$minor = $proj.Project.PropertyGroup.MinorVersion | Where-Object { $_ } | Select-Object -First 1
$build = ([datetime]::Now - [datetime]'2000-01-01').Days
$ver   = "$major.$minor.$build.0"

$stage = Join-Path $PSScriptRoot 'out\stage'
$outDir = Join-Path $PSScriptRoot 'out'
if (Test-Path $stage) { Remove-Item $stage -Recurse -Force }
New-Item -ItemType Directory -Force $stage | Out-Null

dotnet publish (Join-Path $root 'dsAppLaunch.vbproj') -c Release -r win-x64 --self-contained true -o $stage
if ($LASTEXITCODE) { throw 'dotnet publish failed' }
if (-not (Test-Path (Join-Path $PSScriptRoot 'Assets\StoreLogo.png'))) { & (Join-Path $PSScriptRoot 'make-assets.ps1') }

Copy-Item (Join-Path $PSScriptRoot 'Assets') (Join-Path $stage 'Assets') -Recurse -Force
Remove-Item (Join-Path $stage 'Assets\StoreLogo300.png') -ErrorAction SilentlyContinue
# The WinForms logo resource is embedded; the loose copy under Assets\ (if any) is not needed in the package.
(Get-Content (Join-Path $PSScriptRoot 'AppxManifest.xml') -Raw) -replace 'Version="0\.0\.0\.0"', "Version=`"$ver`"" |
    Set-Content (Join-Path $stage 'AppxManifest.xml') -Encoding UTF8

$msix = Join-Path $outDir "dsAppLaunch_${ver}_x64.msix"
if (Test-Path $msix) { Remove-Item $msix }
& "$bin\makeappx.exe" pack /d $stage /p $msix /o
if ($LASTEXITCODE) { throw 'makeappx failed' }

if ($TestSign) {
    $subject = 'CN=DFDC9FBA-D8EA-4CCB-A064-A1E9BB6B2779'   # must equal the manifest Publisher
    $cert = Get-ChildItem Cert:\CurrentUser\My | Where-Object Subject -eq $subject | Select-Object -First 1
    if (-not $cert) {
        $cert = New-SelfSignedCertificate -Type Custom -Subject $subject -KeyUsage DigitalSignature `
            -FriendlyName 'dsAppLaunch test' -CertStoreLocation Cert:\CurrentUser\My `
            -TextExtension @('2.5.29.37={text}1.3.6.1.5.5.7.3.3', '2.5.29.19={text}')
        $cer = Join-Path $outDir 'dsAppLaunch-test.cer'
        Export-Certificate -Cert $cert -FilePath $cer | Out-Null
        Write-Warning "Trust it once (elevated): Import-Certificate -FilePath '$cer' -CertStoreLocation Cert:\LocalMachine\TrustedPeople"
    }
    & "$bin\signtool.exe" sign /fd SHA256 /sha1 $cert.Thumbprint $msix
    if ($LASTEXITCODE) { throw 'signtool failed' }
}
Write-Host "Built $msix"
