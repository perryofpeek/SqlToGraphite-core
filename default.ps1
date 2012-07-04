properties {
  $testMessage = 'Executed Test!'
  $compileMessage = 'Executed Compile!'
  $cleanMessage = 'Executed Clean!'
  $Build_Solution = 'SqlToGraphite.sln'
  $Build_Configuration = 'Release'
  $Build_Artifacts = 'output'
  $fullPath= 'src\SqlToGraphite.host\output'
  $version = '0.3.0.0'
}

task default -depends Package

task Test -depends Init, Compile, Clean, StartOracle {  
  Exec { packages\NUnit.Runners.2.6.0.12051\tools\nunit-console-x86.exe /nologo /config=SqlToGraphite.UnitTests.dll.config test\SqlToGraphite.UnitTests\output\SqlToGraphite.UnitTests.dll }
  Exec { packages\NUnit.Runners.2.6.0.12051\tools\nunit-console-x86.exe .\test\SqlToGraphite.Host.UnitTests\output\SqlToGraphite.Host.UnitTests.dll }
  Exec { packages\NUnit.Runners.2.6.0.12051\tools\nunit-console-x86.exe .\test\SqlToGraphite.Plugin.SqlServer.UnitTests\output\SqlToGraphite.Plugin.SqlServer.UnitTests.dll }
  Exec { packages\NUnit.Runners.2.6.0.12051\tools\nunit-console-x86.exe .\test\SqlToGraphite.Plugin.Oracle.UnitTests\output\SqlToGraphite.Plugin.Oracle.UnitTests.dll }
  Exec { packages\NUnit.Runners.2.6.0.12051\tools\nunit-console-x86.exe .\test\SqlToGraphite.Plugin.Wmi.UnitTests\output\SqlToGraphite.Plugin.Wmi.UnitTests.dll }
}

task Compile -depends  Clean { 
   Exec {  C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /p:OutDir=""$Build_Artifacts\"" /t:Rebuild /p:Configuration=$Build_Configuration $Build_Solution }
}

task Clean {
  if((test-path  $Build_Artifacts -pathtype container))
  {
	rmdir -Force -Recurse $Build_Artifacts;
  }     
  Exec {  C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /p:OutDir=""$Build_Artifacts\"" /t:Clean $Build_Solution }  
}

task Init {

	$Company = "peek.org.uk";
	$Description = "Graphite Service for collecting metrics";
	$Product = "SqlToGraphite $version";
	$Title = "SqlToGraphite $version";
	$Copyright = "PerryOfPeek 2012";	

	Generate-Assembly-Info `
        -file "src\SqlToGraphite.Plugin.Wmi\Properties\AssemblyInfo.cs" `
        -title $Title `
        -description $Description `
        -company $Company `
        -product $Product `
        -version "1.0.0.0" `
        -copyright $Copyright
	
	 Generate-Assembly-Info `
        -file "src\SqlToGraphite.Plugin.Oracle\Properties\AssemblyInfo.cs" `
        -title $Title `
        -description $Description `
        -company $Company `
        -product $Product `
        -version "1.0.0.0" `
        -copyright $Copyright

	 Generate-Assembly-Info `
        -file "src\SqlToGraphite.Plugin.SqlServer\Properties\AssemblyInfo.cs" `
        -title $Title `
        -description $Description `
        -company $Company `
        -product $Product `
        -version "1.0.0.0" `
        -copyright $Copyright

    Generate-Assembly-Info `
        -file "src\SqlToGraphite.host\Properties\AssemblyInfo.cs" `
        -title $Title `
        -description $Description `
        -company $Company `
        -product $Product `
        -version $version `
        -copyright $Copyright

    Generate-Assembly-Info `
        -file "src\SqlToGraphiteInterfaces\Properties\AssemblyInfo.cs" `
        -title $Title `
        -description $Description `
        -company $Company `
        -product $Product `
        -version $version `
        -copyright $Copyright
        
     Generate-Assembly-Info `
        -file "src\SqlToGraphite\Properties\AssemblyInfo.cs" `
        -title $Title `
        -description $Description `
        -company $Company `
        -product $Product `
        -version $version `
        -copyright $Copyright    
}

task Ilmerge -depends Test  {
    
	mkdir $Build_Artifacts;
    #$var = "" + "$fullPath" + "" + "$fullPath" + "\log4net.dll " + "$fullPath" + "\SqlToGraphite.dll " + "$fullPath" + "\Topshelf.dll";
    #Write-Host $var;
    Exec { tools\ilmerge.exe /closed /t:exe /out:output\sqlToGraphite.exe /targetplatform:v4 src\SqlToGraphite.host\output\SqlToGraphite.host.exe src\SqlToGraphite.host\output\Graphite.dll src\SqlToGraphite.host\output\SqlToGraphite.Plugin.Oracle.dll src\SqlToGraphite.host\output\SqlToGraphite.Plugin.Wmi.dll src\SqlToGraphite.host\output\SqlToGraphite.Plugin.SqlServer.dll src\SqlToGraphite.host\output\SqlToGraphiteInterfaces.dll  src\SqlToGraphite.host\output\Topshelf.dll src\SqlToGraphite.host\output\log4net.dll };
    Copy-Item  $fullPath\app.config.Template output\SqlToGraphite.exe.config;
}

#-depends Ilmerge
task Package -depends Ilmerge {
	Exec { "c:\Program Files (x86)\NSIS\makensis.exe sqlToGraphite.nsi" }
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
    Write-Host "Generating assembly info file: $file"
    out-file -filePath $file -encoding UTF8 -inputObject $asmInfo
}