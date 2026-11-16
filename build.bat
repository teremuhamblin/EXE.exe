@echo off
setlocal enabledelayedexpansion

echo ========================================================
echo   EXE.exe - Encrypted_X-Environnement (Build Script)
echo ========================================================

:: 1. Recherche du compilateur C# natif (.NET Framework 4.x)
set "CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe"

if not exist "%CSC%" (
    echo [ERREUR] Compilateur csc.exe introuvable (.NET Framework 4.x requis^).
    if not defined GITHUB_ACTIONS pause
    exit /b 1
)

:: 2. Verification du dossier src
if not exist "src\program.cs" (
    echo [ERREUR] src\program.cs est introuvable.
    if not defined GITHUB_ACTIONS pause
    exit /b 1
)

:: 3. Detection optionnelle du manifest et de l'icone
set "MANIFEST_OPT="
if exist "src\app.manifest" set "MANIFEST_OPT=/win32manifest:src\app.manifest"

set "ICON_OPT="
if exist "assets\icon.ico" set "ICON_OPT=/win32icon:assets\icon.ico"
if exist "src\icon.ico" set "ICON_OPT=/win32icon:src\icon.ico"
if exist "icon.ico" set "ICON_OPT=/win32icon:icon.ico"

:: 4. Nettoyage de l'ancien binaire
if exist "EXE.exe" del /f /q "EXE.exe"

echo [*] Compilateur : %CSC%
echo [*] Compilation de src\*.cs ...

"%CSC%" /nologo /utf8output /target:winexe /optimize+ /platform:anycpu ^
    /out:EXE.exe ^
    %MANIFEST_OPT% ^
    %ICON_OPT% ^
    /reference:System.dll ^
    /reference:System.Core.dll ^
    /reference:System.Drawing.dll ^
    /reference:System.Windows.Forms.dll ^
    src\*.cs

if %ERRORLEVEL% neq 0 (
    echo.
    echo [ERREUR] Echec de la compilation (code %ERRORLEVEL%^).
    if not defined GITHUB_ACTIONS pause
    exit /b %ERRORLEVEL%
)

echo.
echo [OK] EXE.exe compile avec succes a la racine !
if not defined GITHUB_ACTIONS pause
exit /b 0
