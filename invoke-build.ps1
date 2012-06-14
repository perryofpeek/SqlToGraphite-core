 param($TaskList, $Config) 
 Import-Module .\packages\psake.4.1.0\tools\psake.psm1 
 	try 
 	{ 
 		invoke-psake -framework 4.0 -taskList $TaskList -properties @{config=$Config} 
	} 
	catch 
	{ 
		. default.ps1; 
		#if(![string]::IsNullOrEmpty($env:TEAMCITY_VERSION)) 
		#{ 
		#	TeamCity-ReportBuildStatus -status FAILURE -text $_ 
		#} 
		#else 
		#{
		#	Write-Host ERROR: $_ -ForegroundColor RED 
		#} exit $Error.Count
	} 
	finally 
	{
		remove-module psake
	}