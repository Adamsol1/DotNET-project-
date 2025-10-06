Param()
$ErrorActionPreference = 'Stop'

# Resolve repo root based on script location
$ScriptDir = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
$RepoRoot = Resolve-Path (Join-Path $ScriptDir '..')

$ProjectPath = Join-Path $RepoRoot 'Tools/GitHelperApp/GitHelperApp.csproj'

if (-not (Test-Path $ProjectPath)) {
  Write-Error "Could not find GitHelperApp project at: $ProjectPath"
}

dotnet run --project $ProjectPath



