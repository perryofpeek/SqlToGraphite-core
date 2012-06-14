properties {
  $testMessage = 'Executed Test!'
  $compileMessage = 'Executed Compile!'
  $cleanMessage = 'Executed Clean!'
  $Build_Solution = 'SqlToGraphite.sln'
  $Build_Configuration = 'Release'
  $Build_Artifacts = 'output'
  $fullPath="src\SqlToGraphite.host\output"
}

task default -depends Package

task Test -depends Compile, Clean, StartOracle { 
 # write-output "hello"
  Exec { packages\NUnit.Runners.2.6.0.12051\tools\nunit-console-x86.exe /nologo /config=SqlToGraphite.UnitTests.dll.config test\SqlToGraphite.UnitTests\Bin\x86\Release\SqlToGraphite.UnitTests.dll }
  Exec { packages\NUnit.Runners.2.6.0.12051\tools\nunit-console-x86.exe .\test\SqlToGraphite.Host.UnitTests\bin\Release\SqlToGraphite.Host.UnitTests.dll }
}

task Compile -depends Clean { 
   Exec {  C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /p:OutDir=""$Build_Artifacts\"" /t:Rebuild /p:Configuration=$Build_Configuration $Build_Solution }
}

task Clean {   
  Exec {  C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe /p:OutDir=""$Build_Artifacts\"" /t:Clean $Build_Solution }  
}


task Ilmerge -depends Test {
	Exec { tools\ilmerge /t:exe /out:output\sqlToGraphite.exe /targetplatform:v4 "$fullPath\SqlToGraphite.host.exe" "$fullPath\Graphite.dll" "$fullPath\log4net.dll" "$fullPath\SqlToGraphite.dll" "$fullPath\Topshelf.dll"};
	Copy-Item  $fullPath\SqlToGraphite.host.exe.config  output;
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