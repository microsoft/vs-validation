@ECHO OFF

if "%1"=="" (
    ECHO USAGE: %0 version
    EXIT /b 1
)

msbuild "%~dp0Microsoft.Validation.csproj" /p:TargetFrameworkVersion=v3.5,Configuration=Release /v:minimal /nologo
msbuild "%~dp0Microsoft.Validation.csproj" /p:TargetFrameworkVersion=v4.0,Configuration=Release	/v:minimal /nologo

@echo on
nuget pack "%~dp0Microsoft.Validation.nuspec" -symbols -OutputDirectory bin -Version %1
