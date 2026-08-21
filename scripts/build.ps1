$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root
dotnet restore PromptVault.sln
dotnet build PromptVault.sln --configuration Release
