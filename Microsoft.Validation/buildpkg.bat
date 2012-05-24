@ECHO OFF

if "%1"=="" (
    ECHO USAGE: %0 version
    EXIT /b 1
)

@echo on
nuget pack -build -symbols  -properties configuration=release -OutputDirectory bin\release -Version %1
