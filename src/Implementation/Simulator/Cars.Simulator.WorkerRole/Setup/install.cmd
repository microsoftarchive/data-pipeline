@REM cd to the location of the script
cd "%~dp0"



@REM allow unsigned script execution

powershell -command Set-ExecutionPolicy RemoteSigned

powershell -command ./install.ps1 2>&1 > install.log

