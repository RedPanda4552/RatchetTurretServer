@echo off

rem Build the application
dotnet publish --runtime win10-x64 --configuration Release --output "bin\publish\win10-x64"
echo start win10-x64/RatchetTurretServer.exe > "bin\publish\RatchetTurretServer.bat"

rem Build PINE
cd "pine\bindings\c"
rmdir /S /Q "build"
meson "build"
cd "build"
ninja
cd "..\..\..\..\"
xcopy "pine\bindings\c\build\pine_c.dll" "bin\publish\win10-x64" /Y
if NOT %ERRORLEVEL% EQU 0 (
    echo.
    echo PINE compilation failed. Please review the readme for build instructions.
)

rem Assemble zip of the application and launch script
del "bin\publish\RatchetTurretServer-win10-x64.zip" > nul 2>&1
powershell Compress-Archive -Path "bin\publish\win10-x64","bin\publish\RatchetTurretServer.bat" -DestinationPath "bin\publish\RatchetTurretServer-win10-x64.zip"
