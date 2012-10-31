properties {
  $testMessage = 'Executed Test!'
  $compileMessage = 'Executed Compile!'
  $cleanMessage = 'Executed Clean!'
  $Build_Solution = 'SqlToGraphite.sln'
  $Build_Configuration = 'Release'
  $Build_Artifacts = 'output'
  $fullPath= 'src\SqlToGraphite.host\output'
  $version = '0.3.0.0'
  $Debug = 'Debug'
  $pwd = pwd
  $msbuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
  $nunit =  "$pwd\packages\NUnit.Runners.2.6.2\tools\nunit-console-x86.exe"
  $openCover = "$pwd\packages\OpenCover.4.0.804\OpenCover.Console.exe"
  $reportGenerator = "$pwd\packages\ReportGenerator.1.6.1.0\ReportGenerator.exe"
  $TestOutput = "$pwd\TestOutput"
  $UnitTestOutputFolder = "$TestOutput\UnitTestOutput";
}

task default -depends Package

task Test -depends Init, Compile, Clean, StartOracle { 			
	mkdir $TestOutput -Verbose:$false;  
    mkdir $UnitTestOutputFolder -Verbose:$false;  
	
	$unitTestFolders = Get-ChildItem test\* -recurse | Where-Object {$_.PSIsContainer -eq $True} | where-object {$_.Fullname.Contains("bin\x86\Debug")} | where-object {$_.Fullname.Contains("bin\x86\Debug\") -eq $false}| select-object FullName
	foreach($folder in $unitTestFolders)
	{
		$x = [string] $folder.FullName
		copy-item -force -path $x\* -Destination "$UnitTestOutputFolder\" 
	}
	#Copy all the unit test folders into one folder 
	cd $UnitTestOutputFolder
	foreach($file in Get-ChildItem *test*.dll)
	{
		$files = $files + " " + $file.Name
	}
	#write-host " $openCover -target:$nunit -filter:+[SqlToGraphite*]* -register:user -mergebyhash -targetargs:$files /err=err.nunit.txt /noshadow /nologo /config=SqlToGraphite.UnitTests.dll.config"
	Exec { & $openCover "-target:$nunit" -filter:+[SqlToGraphite*]* -register:user -mergebyhash "-targetargs:$files /err=err.nunit.txt /noshadow /nologo /config=SqlToGraphite.UnitTests.dll.config" } 
	Exec { & $reportGenerator "-reports:results.xml" "-targetdir:..\report" "-verbosity:Error"}
	cd $pwd
}

task Compile -depends  Clean { 
   Exec {  & $msbuild /m:4 /verbosity:quiet /nologo /p:OutDir=""$Build_Artifacts\"" /t:Rebuild /p:Configuration=$Build_Configuration $Build_Solution }   	
}

task Clean {
  if((test-path  $Build_Artifacts -pathtype container))
  {
	rmdir -Force -Recurse $Build_Artifacts;
  }     
  if (Test-Path $TestOutput) 
  {
		Remove-Item -force -recurse $TestOutput
  }  
  Exec {  & $msbuild /m:4 /verbosity:quiet /nologo /p:OutDir=""$Build_Artifacts\"" /t:Clean $Build_Solution }  
}

task Init {

	$Company = "peek.org.uk";
	$Description = "Graphite Service for collecting metrics";
	$Product = "SqlToGraphite $version";
	$Title = "SqlToGraphite $version";
	$Copyright = "PerryOfPeek 2012";	

	$files = Get-ChildItem src\* -recurse | Where-Object {$_.Fullname.Contains("AssemblyInfo.cs")}
	foreach ($file in $files)
	{
		Generate-Assembly-Info `
        -file $file.Fullname `
        -title $Title `
        -description $Description `
        -company $Company `
        -product $Product `
        -version $version `
        -copyright $Copyright
	}
}

task Ilmerge -depends Test  {
    
	mkdir $Build_Artifacts;
    #$var = "" + "$fullPath" + "" + "$fullPath" + "\log4net.dll " + "$fullPath" + "\SqlToGraphite.dll " + "$fullPath" + "\Topshelf.dll";
    #Write-Host $var;
    Exec { tools\ilmerge.exe /closed /t:exe /out:output\sqlToGraphite.exe /targetplatform:v4 src\SqlToGraphite.host\output\SqlToGraphite.host.exe src\SqlToGraphite.host\output\Graphite.dll src\SqlToGraphite.host\output\SqlToGraphite.Plugin.Oracle.dll src\SqlToGraphite.host\output\SqlToGraphite.Plugin.Wmi.dll src\SqlToGraphite.host\output\SqlToGraphite.Plugin.SqlServer.dll src\SqlToGraphite.host\output\SqlToGraphiteInterfaces.dll src\Plugin.Oracle.Transactions\output\Plugin.Oracle.Transactions.dll src\SqlToGraphite.host\output\Topshelf.dll src\SqlToGraphite.host\output\log4net.dll };
    Exec { tools\ilmerge.exe /closed /t:winexe /out:output\ConfigUi.exe /targetplatform:v4 src\Configurator\output\Configurator.exe src\SqlToGraphite.host\output\SqlToGraphite.host.exe src\SqlToGraphite.host\output\Graphite.dll src\SqlToGraphite.host\output\SqlToGraphite.Plugin.Oracle.dll src\SqlToGraphite.host\output\SqlToGraphite.Plugin.Wmi.dll src\SqlToGraphite.host\output\SqlToGraphite.Plugin.SqlServer.dll src\SqlToGraphite.host\output\SqlToGraphiteInterfaces.dll src\Plugin.Oracle.Transactions\output\Plugin.Oracle.Transactions.dll src\SqlToGraphite.host\output\log4net.dll };    	
	Copy-Item  $fullPath\app.config.Template output\SqlToGraphite.exe.config;
	Copy-Item  src\ConfigPatcher\output\configpatcher.exe output\configpatcher.exe;	
	Copy-Item  src\Configurator\output\Configurator.exe.config output\ConfigUi.exe.config;
}
# -depends Ilmerge
task Package -depends Ilmerge {
	Exec { c:\Apps\NSIS\makensis.exe /p4 /v2 sqlToGraphite.nsi }	
}

task StartOracle {
     Start-Service "OracleServiceXE";
     Start-Service "OracleXETNSListener";
}

task ? -Description "Helper to display task info" {
    Write-Documentation
}

function Get-Git-Commit
{
    $gitLog = git log --oneline -1
    return $gitLog.Split(' ')[0]
}

function Generate-Assembly-Info
{
param(
    [string]$clsCompliant = "true",
    [string]$title, 
    [string]$description, 
    [string]$company, 
    [string]$product, 
    [string]$copyright, 
    [string]$version,
    [string]$file = $(Throw "file is a required parameter.")
)
  $commit = Get-Git-Commit
  $asmInfo = "using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: CLSCompliantAttribute($clsCompliant)]
[assembly: ComVisibleAttribute(false)]
[assembly: AssemblyTitleAttribute(""$title"")]
[assembly: AssemblyDescriptionAttribute(""$description"")]
[assembly: AssemblyCompanyAttribute(""$company"")]
[assembly: AssemblyProductAttribute(""$product"")]
[assembly: AssemblyCopyrightAttribute(""$copyright"")]
[assembly: AssemblyVersionAttribute(""$version"")]
[assembly: AssemblyInformationalVersionAttribute(""$version / $commit"")]
[assembly: AssemblyFileVersionAttribute(""$version"")]
[assembly: AssemblyDelaySignAttribute(false)]
"

    $dir = [System.IO.Path]::GetDirectoryName($file)
    if ([System.IO.Directory]::Exists($dir) -eq $false)
    {
        Write-Host "Creating directory $dir"
        [System.IO.Directory]::CreateDirectory($dir)
    }
   # Write-Host "Generating assembly info file: $file"
    out-file -filePath $file -encoding UTF8 -inputObject $asmInfo
}