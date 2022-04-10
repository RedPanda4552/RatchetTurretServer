# Ratchet Turret Server
A companion application to run alongside PCSX2 that fixes the endless bobbing of Megaturrets on Ratchet & Clank 2: Locked and Loaded.

This is essentially an IPC version of [Vogtinator's rc2hack fork](https://github.com/Vogtinator/pcsx2/tree/rc2hack). The objective of Ratchet Turret Server is to allow people to use the hack while also keeping their PCSX2 versions up-to-date.

Ratchet Turret Server uses [PINE](https://github.com/GovanifY/pine) by GovanifY to interface with PCSX2.

## Download
Check the [Releases](https://github.com/RedPanda4552/RatchetTurretServer/releases) tab for the latest release. Make sure you're actually clicking on the release, not the source code.

## How To Use
1. Extract the zip archive wherever you like
2. Launch PCSX2
3. Enable PINE, if it is not already (System > Game Settings > Configure PINE)
4. Start the game
5. Run `RatchetTurretServer.bat` (Windows) or `RatchetTurretServer.sh` (Linux)
6. Leave the command shell which appears open in the background

*Ratchet Turret Server will automatically shut itself down once it detects the game has changed or PCSX2 has closed.*

## Building
### All Operating Systems
#### .NET Core
Ratchet Turret Server builds as a .NET Core 3.1 application. As such at minimum you will need the [.NET Core 3.1 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/3.1). I believe the .NET 5.0 SDK and .NET 6.0 SDK can also compile a .NET Core 3.1 target, but your mileage may vary.

Yes, I know .NET Core 3.1 is end of life at the end of the year, there are other things blocking me from making the upgrade at this moment.

#### Fixing .NET Core's Build System
The PINE submodule can cause problems with .NET Core because the build system thinks the sample `.cs` files in PINE are sources for the application too. I've tried to see if I can just exclude `pine/` from source generation, and I cannot. Furthermore moving the `.csproj` and `src/` to a subdirectory (technically the right thing to do) means making changes to the stock Visual Studio Code build and launch tasks and writing up documentation for that just to build, so I instead have the brilliant (/s) solution of deleting `csharp/` from `pine/bindings/` prior to building. It's pretty lame, but it works. Microsoft, please just let us specify a source directory in `dotnet publish`, I beg of you.

### Windows
#### Setup
1. [Meson](https://github.com/mesonbuild/meson/releases) is required to build PINE. Use their MSI installer and make sure Ninja is also selected when you install.
2. Already have Meson but not [Ninja](https://github.com/ninja-build/ninja/releases)? Grab a release from here.
3. Install [Visual Studio](https://visualstudio.microsoft.com/vs/community/) to handle the actual compilation of the PINE library.
4. If Meson, Ninja, or Visual Studio ask you to reboot to complete the installation, do not ignore it, reboot your system; nothing will work until you do.

#### Build
1. Search for and open `x64 Native Tools Command Prompt`
2. Run `build-win10.bat` (make sure your working directory is the project root)
3. Application will be zipped in `bin\publish\RatchetTurretServer-win10-x64.zip`

### Linux
#### Setup
1. Install Meson using your distro's package manager
2. Install Ninja using your distro's package manager

#### Build
1. Run `build-linux.sh` (make sure your working directory is the project root)
2. Application will be zipped in `bin\publish\RatchetTurretServer-linux-x64.zip`

## Debugging
The PINE library will not be generated by the default build task in Visual Studio Code.
1. Run `build-linux.sh` or `build-win10.bat`
2. Run the Visual Studio Code debugger
3. Copy the PINE library (Windows = `win10-x64\pine_c.dll`, Linux = `linux-x64/libpine_c.so`) from the `publish` folder
4. Paste it next to the RatchetTurretServer executable in the `Debug` folder.
