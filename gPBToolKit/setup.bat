@echo off
set /p _mode="Enter mode Install(I)/Uninstall(U))  [I]: "
if "%_mode%"=="" (
  set _mode=I
)

if "%_mode%"=="I" (
  echo Mode Install
  REG ADD "HKEY_LOCAL_MACHINE\Software\PISystem\PI - ProcessBook\Addins\gPBToolKit.Connect" /v FriendlyName /t REG_SZ /d "PBToolKit" /f
  REG ADD "HKEY_LOCAL_MACHINE\Software\PISystem\PI - ProcessBook\Addins\gPBToolKit.Connect" /v Description /t REG_SZ /d "ToolKit for PB" /f
  REG ADD "HKEY_LOCAL_MACHINE\Software\PISystem\PI - ProcessBook\Addins\gPBToolKit.Connect" /v LoadBehavior /t REG_DWORD  /d 0000003 /f
)
if "%_mode%"=="U" (
  echo Mode Uninstall
  REG DELETE "HKEY_LOCAL_MACHINE\Software\PISystem\PI - ProcessBook\Addins\gPBToolKit.Connect" /f
)