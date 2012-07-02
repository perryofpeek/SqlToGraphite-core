VIProductVersion              "1.0.0.0" ; set version here
VIAddVersionKey "FileVersion" "1.0.0.0" ; and here!
VIAddVersionKey "CompanyName" "peek.org.uk"
VIAddVersionKey "LegalCopyright" "© peek.org.uk 2012"
VIAddVersionKey "FileDescription" "SqlToGraphite installer"
OutFile SqlToGraphite-Setup.exe
RequestExecutionLevel admin

# uncomment the following line to make the installer silent by default.
SilentInstall silent

Section Main    
    SetOutPath $PROGRAMFILES\SqlToGraphite
    SetOverwrite on    
	; Check to see if already installed
	ClearErrors
	ReadRegStr $0 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\SqlToGraphite" ""	
	IfErrors 0 +2
		DetailPrint "Installed aleady"
		ExecWait '"$OUTDIR\uninstall.exe " _?=$OUTDIR /s '		
			
	DetailPrint "Now installing"				
		File output\sqltographite.exe
		File output\sqltographite.exe.config 
		WriteUninstaller $OUTDIR\uninstall.exe  
    
		WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SqlToGraphite" \
                 "DisplayName" "SqlToGraphite record metrics in graphite"
		WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\SqlToGraphite" \
                 "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
		
		ExecWait '"$OUTDIR\sqltographite.exe" install' $0
		DetailPrint "Returned $0"
		ExecWait '"Net" start SqlToGraphite' $0
		DetailPrint "Returned $0"
SectionEnd

Section "Uninstall"  
  ExecWait '"Net" stop SqlToGraphite' $0
  DetailPrint "Returned $0"
  ExecWait '"$INSTDIR\sqltographite.exe" uninstall'
  Delete $INSTDIR\sqltographite.exe
  Delete $INSTDIR\sqltographite.exe.config
  RMDir $INSTDIR\logs
  Delete $INSTDIR\uninstall.exe ; delete self (see explanation below why this works)
  RMDir $INSTDIR  
  Quit  
SectionEnd