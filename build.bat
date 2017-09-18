@echo off
rem KP 2017-09: convenience build for dev boxes. Packages must already be
rem restored in VS (right-click solution / restore) - this does not do it.
setlocal

set MSBUILD_EXE=C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe
if not exist "%MSBUILD_EXE%" (
    if defined MSBUILD set MSBUILD_EXE=%MSBUILD%
)
if not exist "%MSBUILD_EXE%" (
    echo MSBuild not found. Set MSBUILD env var to a full MSBuild.exe path.
    exit /b 1
)

"%MSBUILD_EXE%" PortfolioViewer.sln /p:Configuration=Release /p:Platform="Any CPU" /v:m /nologo
if errorlevel 1 exit /b 1

echo.
echo Running tests...
packages\NUnit.Runners.2.6.4\tools\nunit-console.exe src\PV.Tests\bin\Release\PV.Tests.dll
