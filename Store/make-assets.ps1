# Generates the MSIX tile/logo PNGs (Store\Assets) from Store\logo\dsAppLaunch-icon-1024.png (rendered by Store\logo\make-brand-assets.ps1).
# Re-run only if the logo changes.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$root = Split-Path $PSScriptRoot -Parent
$src  = [System.Drawing.Image]::FromFile((Join-Path $PSScriptRoot 'logo\dsAppLaunch-icon-1024.png'))
$out  = Join-Path $PSScriptRoot 'Assets'
New-Item -ItemType Directory -Force $out | Out-Null

function New-Tile([string]$name, [int]$w, [int]$h, [double]$fill) {
    $bmp = New-Object System.Drawing.Bitmap $w, $h, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = 'HighQualityBicubic'; $g.SmoothingMode = 'HighQuality'; $g.PixelOffsetMode = 'HighQuality'
    $g.Clear([System.Drawing.Color]::Transparent)
    $box = [Math]::Min($w, $h) * $fill
    $scale = [Math]::Min($box / $src.Width, $box / $src.Height)
    $dw = [int]($src.Width * $scale); $dh = [int]($src.Height * $scale)
    $g.DrawImage($src, [int](($w - $dw) / 2), [int](($h - $dh) / 2), $dw, $dh)
    $g.Dispose()
    $bmp.Save((Join-Path $out $name), [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
}
New-Tile 'Square44x44Logo.png'   44  44  1.0
New-Tile 'Square150x150Logo.png' 150 150 0.96
New-Tile 'Wide310x150Logo.png'   310 150 0.96
New-Tile 'StoreLogo.png'         50  50  1.0
New-Tile 'StoreLogo300.png'      300 300 1.0   # Partner Center listing logo (upload manually)
$src.Dispose()
Write-Host "Assets written to $out"

