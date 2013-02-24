param([string]$task = "default")
Import-Module '.\packages\psake.4.2.0.1\tools\psake.psm1'; 
Invoke-psake  default.ps1 -t $task; 
if ($Error -ne '') 
{ 
    Write-Host "ERROR: $error" -fore RED; 
    exit $error.Count
} 
