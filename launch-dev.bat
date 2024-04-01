@echo off
set project_name=lc-hax
set window_title=Lethal Company
:: Wait before injecting to avoid crashes caused by injecting too early
set countdown=3

:begin
dotnet build %project_name% -c Release

:check_window
REM Check if the process "Lethal Company.exe" is running
tasklist /fi "imagename eq Lethal Company.exe" 2>nul | find /i "Lethal Company.exe" >nul
if %errorlevel% equ 0 (
    echo Process "Lethal Company.exe" found.
    REM Now check if the window "Lethal Company" exists and is not "BepInEx 5.4.21.0 - Lethal Company"
    tasklist /v | findstr /i "%window_title%" | findstr /v /i "BepInEx 5.4.21.0 - Lethal Company" >nul
    if %errorlevel% equ 0 (
        echo Window "%window_title%" found.
		echo Injecting...
		timeout /t %countdown%
        start /wait /b ./submodules/SharpMonoInjectorCore/dist/SharpMonoInjector.exe inject -p "Lethal Company" -a bin/%project_name%.dll -n Hax -c Loader -m Load
        exit
    ) else (
        echo Window "%window_title%" not found.
    )
) else (
    echo Process "Lethal Company.exe" not found.
)
::timeout /t %countdown% 
::goto check_window