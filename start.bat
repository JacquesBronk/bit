@echo off
setlocal

REM Validate input
IF "%1"=="" GOTO HELP
IF NOT EXIST "%1" (
    echo [ERROR] Input file does not exist.
    GOTO EXIT
)
IF /I NOT "%~x1"==".cs" (
    echo [ERROR] Input file must have a .cs extension.
    GOTO EXIT
)

REM Get parameters
set "inputFile=%1"
set "tempDir=_temp"
set "projectFile=%tempDir%\p.csproj"
set "framework=net7.0"

REM Create the temporary directory
mkdir "%tempDir%" 2>NUL

REM Create a temporary .csproj file
echo ^<Project Sdk="Microsoft.NET.Sdk"^>^<PropertyGroup^>^<TargetFramework^>%framework%^</TargetFramework^>^</PropertyGroup^>^</Project^> > "%projectFile%"

REM Copy the input C# file to the temp directory
copy "%inputFile%" "%tempDir%" >NUL
cd "%tempDir%"

REM Run the .NET project
echo [INFO] Running the C# file...
dotnet run

REM Cleanup and exit
cd ..
rmdir /Q /S "%tempDir%" >NUL
GOTO EXIT

:HELP
echo Runs any C# file with a static Main() directly
echo Usage: %~nx0 [file.cs]
echo You can modify the 'framework' variable to specify a different .NET target framework.

:EXIT
endlocal
