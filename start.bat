@echo off
setlocal

REM Validate input
IF "%1"=="" GOTO HELP
IF NOT EXIST "%1" (
    echo [ERROR] Input file does not exist: %1
    GOTO EXIT
)
IF /I NOT "%~x1"==".cs" (
    echo [ERROR] Input file must have a .cs extension.
    GOTO EXIT
)

REM Check for --env-renew flag and appsettings.json path
set "envRenewFlag="
set "appSettingsPath=appsettings.json"
IF /I "%2"=="--env-renew" (
    set "envRenewFlag=--env-renew"
    IF NOT "%3"=="" (
        set "appSettingsPath=%3"
    )
) ELSE (
    IF NOT "%2"=="" (
        set "appSettingsPath=%2"
    )
)

REM Validate appsettings.json path
IF NOT EXIST "%appSettingsPath%" (
    echo [ERROR] Appsettings file does not exist at the specified path: %appSettingsPath%
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

REM Add package references for System.Text.Json
dotnet add package System.Text.Json

REM Run the .NET project with the optional flag
echo [INFO] Running the C# file...
IF "%envRenewFlag%"=="" (
    dotnet run -- "%appSettingsPath%"
) ELSE (
    dotnet run -- "%appSettingsPath%" %envRenewFlag%
)

REM Cleanup and exit
cd ..
rmdir /Q /S "%tempDir%" >NUL
GOTO EXIT

:HELP
echo Runs any C# file with a static Main() directly
echo Usage: %~nx0 [file.cs] [--env-renew] [path-to-appsettings.json]
echo You can modify the 'framework' variable to specify a different .NET target framework.

:EXIT
endlocal
