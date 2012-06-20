properties {
  $testMessage = 'Executed Test!'
  $compileMessage = 'Executed Compile!'
  $cleanMessage = 'Executed Clean!'
  $Build_Solution = 'SqlToGraphite.sln'
  $Build_Configuration = 'Release'
  $Build_Artifacts = 'output'
  $fullPath= 'src\SqlToGraphite.host\output'
}

task default -depends Package

task Test -depends Compile, Clean, StartOracle { 
 # write-output "hello"
  Exec { packages\NUnit.Runners.2.6.0.12051\tools\nunit-console-x86.exe /nologo /config=SqlToGraphite.UnitTests.dll.config test\SqlToGraphite.UnitTests\output\SqlToGraphite.UnitTests.dll }
  Exec { packages\NUnit.Runners.2.6.0.12051\tools\nunit-console-x86.exe .\test\SqlToGraphite.Host.UnitTests\output\SqlToGraphite.Host.UnitTests.dll }
}

task Compile -depends Clean { 
   Exec {  C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /p:OutDir=""$Build_Artifacts\"" /t:Rebuild /p:Configuration=$Build_Configuration $Build_Solution }
}

task Clean {   
  rmdir $Build_Artifacts -Force -Recurse;
  Exec {  C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /p:OutDir=""$Build_Artifacts\"" /t:Clean $Build_Solution }  
}

task Ilmerge -depends Test  {
    mkdir $Build_Artifacts;
	#$var = "" + "$fullPath" + "" + "$fullPath" + "\log4net.dll " + "$fullPath" + "\SqlToGraphite.dll " + "$fullPath" + "\Topshelf.dll";
	#Write-Host $var;
	Exec { tools\ilmerge.exe /closed /t:exe /out:output\sqlToGraphite.exe /targetplatform:v4 src\SqlToGraphite.host\output\SqlToGraphite.host.exe src\SqlToGraphite.host\output\Graphite.dll  src\SqlToGraphite.host\output\Topshelf.dll src\SqlToGraphite.host\output\log4net.dll };
	Copy-Item  $fullPath\app.config.Template output\SqlToGraphite.exe.config;
}

task Package -depends Ilmerge {
}

task StartOracle {
	 Start-Service "OracleServiceXE";
	 Start-Service "OracleXETNSListener";
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}