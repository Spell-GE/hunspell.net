$ErrorActionPreference = "Stop"

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$BuildDir = Join-Path $ScriptDir "build"
$OutputDir = Join-Path $ScriptDir "out"

New-Item -ItemType Directory -Force -Path $BuildDir | Out-Null

cmake -S $ScriptDir -B $BuildDir `
    -DCMAKE_BUILD_TYPE=Release `
    -A x64

cmake --build $BuildDir --config Release --parallel

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

Copy-Item (Join-Path $BuildDir "Release" "hunspell.dll") $OutputDir -Force

Write-Host "Build complete. Output in: $OutputDir"
