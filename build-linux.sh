#!/bin/sh

# Build the application
dotnet publish --runtime linux-x64 --configuration Release --output "bin/publish/linux-x64"
printf "%s%s\n./linux-x64/RatchetTurretServer" "#!" "/bin/sh" > "bin/publish/RatchetTurretServer.sh"

# Build PINE
cd "pine/bindings/c"
rm -rf "build"
meson "build"
cd "build"
ninja
cd "../../../../"
cp "./pine/bindings/c/build/libpine_c.so" "./bin/publish/linux-x64/libpine_c.so"
if [ $? -ne 0 ]
then
    printf "\nPINE compilation failed. Please review the readme for build instructions"
fi

# Assemble zip of the application and launch script
cd "bin/publish"
rm "RatchetTurretServer-linux-x64.zip"
zip -r "RatchetTurretServer-linux-x64.zip" "linux-x64" "RatchetTurretServer.sh"
