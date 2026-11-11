<#
Builds every product-logo derivative from the artwork Store\logo\dsAppLaunch-blue.svg:
  Store\logo\dsAppLaunch-icon.svg / -icon-1024.png   tile cropped full-bleed, no drop shadow, transparent corners
  Assets\applogo.png                                 256 px, embedded resource used in the form headers
  dsAppLaunch.ico                                    16-256 px (PNG frames): exe, window and installer icon
  Store\Assets\*.png                                 MSIX tiles (via Store\make-assets.ps1)
Needs Microsoft Edge (renders the SVG headlessly). Re-run after editing dsAppLaunch-blue.svg.
The Data Spec company logo (the D+bolt) is separate: Assets\dslogo.png.
#>
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
$here = $PSScriptRoot
$root = Split-Path (Split-Path $here -Parent) -Parent
$edge = @("${env:ProgramFiles(x86)}\Microsoft\Edge\Application\msedge.exe", "$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe") | Where-Object { Test-Path $_ } | Select-Object -First 1
if (-not $edge) { throw 'Microsoft Edge not found (needed to render the SVG).' }

# 1. Derive the icon SVG: crop the viewBox to the tile (32..480) and drop the tile's drop-shadow filter.
$svg = Get-Content (Join-Path $here 'dsAppLaunch-blue.svg') -Raw -Encoding UTF8
$svg = $svg.Replace('viewBox="0 0 512 512"', 'viewBox="32 32 448 448"')
$svg = [regex]::Replace($svg, '(<rect x="32" y="32"[^>]*?)\s+filter="url\(#dropShadow\)"', '$1')
$svgPath = Join-Path $here 'dsAppLaunch-icon.svg'
Set-Content $svgPath $svg -Encoding UTF8

# 2. Render at 1024 px with a transparent background.
$html = Join-Path $here 'render-icon.html'
Set-Content $html '<html><body style="margin:0;background:transparent"><img src="dsAppLaunch-icon.svg" width="1024" height="1024" style="display:block"></body></html>'
$png1024 = Join-Path $here 'dsAppLaunch-icon-1024.png'
if (Test-Path $png1024) { Remove-Item $png1024 }
$ErrorActionPreference = 'Continue'
& $edge --headless --disable-gpu --hide-scrollbars --default-background-color=00000000 --window-size=1024,1024 "--screenshot=$png1024" ([uri]$html).AbsoluteUri 2>$null | Out-Null
$ErrorActionPreference = 'Stop'
for ($i = 0; $i -lt 20 -and -not (Test-Path $png1024); $i++) { Start-Sleep -Milliseconds 500 }
if (-not (Test-Path $png1024)) { throw 'Edge did not produce the PNG.' }
Remove-Item $html

function Resize-Png($srcPath, [int]$size) {
    $src = [System.Drawing.Image]::FromFile($srcPath)
    $bmp = New-Object System.Drawing.Bitmap $size, $size, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = 'HighQualityBicubic'; $g.SmoothingMode = 'HighQuality'; $g.PixelOffsetMode = 'HighQuality'
    $g.Clear([System.Drawing.Color]::Transparent); $g.DrawImage($src, 0, 0, $size, $size); $g.Dispose(); $src.Dispose()
    return $bmp
}

# 3. Header logo.
$b = Resize-Png $png1024 256; $b.Save((Join-Path $root 'Assets\applogo.png'), [System.Drawing.Imaging.ImageFormat]::Png); $b.Dispose()

# 4. Multi-size .ico with PNG frames.
$sizes = 16, 24, 32, 48, 64, 128, 256
$frames = foreach ($s in $sizes) {
    $b = Resize-Png $png1024 $s; $ms = New-Object System.IO.MemoryStream
    $b.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png); $b.Dispose(); , $ms.ToArray()
}
$ico = New-Object System.IO.MemoryStream; $w = New-Object System.IO.BinaryWriter $ico
$w.Write([uint16]0); $w.Write([uint16]1); $w.Write([uint16]$sizes.Count)
$offset = 6 + 16 * $sizes.Count
for ($i = 0; $i -lt $sizes.Count; $i++) {
    $s = $sizes[$i]; $dim = if ($s -ge 256) { 0 } else { $s }
    $w.Write([byte]$dim); $w.Write([byte]$dim); $w.Write([byte]0); $w.Write([byte]0)
    $w.Write([uint16]1); $w.Write([uint16]32); $w.Write([uint32]$frames[$i].Length); $w.Write([uint32]$offset)
    $offset += $frames[$i].Length
}
foreach ($f in $frames) { $w.Write($f) }
$w.Flush(); [System.IO.File]::WriteAllBytes((Join-Path $root 'dsAppLaunch.ico'), $ico.ToArray())

# 5. MSIX tiles.
& (Join-Path $root 'Store\make-assets.ps1')
Write-Host 'Brand assets rebuilt.'
