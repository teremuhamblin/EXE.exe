@echo off
setlocal EnableExtensions

cd /d "%~dp0"
if errorlevel 1 (
    echo [ERREUR] Impossible d'acceder au dossier du projet.
    exit /b 1
)

echo ========================================================
echo   EXE.exe - Compilation Windows
 echo ========================================================

:: Recherche du compilateur .NET Framework fourni par Windows.
set "CSC=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if not exist "%CSC%" set "CSC=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\csc.exe"
if not exist "%CSC%" (
    echo [ERREUR] csc.exe est introuvable. Le compilateur .NET Framework 4.x est requis.
    goto :fail
)

if not exist "src\Program.cs" (
    echo [ERREUR] src\Program.cs est introuvable.
    goto :fail
)

set "MANIFEST_OPT="
if exist "src\app.manifest" set "MANIFEST_OPT=/win32manifest:src\app.manifest"

set "ICON_OPT="
if exist "assets\icon.ico" set "ICON_OPT=/win32icon:assets\icon.ico"
if exist "src\icon.ico" set "ICON_OPT=/win32icon:src\icon.ico"
if exist "icon.ico" set "ICON_OPT=/win32icon:icon.ico"

:: Compile dans un fichier temporaire : un échec ne détruit pas le binaire précédent.
set "BUILD_OUTPUT=EXE.build.tmp.exe"
if exist "%BUILD_OUTPUT%" del /f /q "%BUILD_OUTPUT%"

echo [INFO] Compilateur : %CSC%
echo [INFO] Compilation des sources C#...
"%CSC%" /nologo /utf8output /target:winexe /optimize+ /platform:anycpu ^
    /out:"%BUILD_OUTPUT%" ^
    %MANIFEST_OPT% ^
    %ICON_OPT% ^
    /reference:System.dll ^
    /reference:System.Core.dll ^
    /reference:System.Drawing.dll ^
    /reference:System.Windows.Forms.dll ^
    src\*.cs
if errorlevel 1 goto :compile_failed

if not exist "%BUILD_OUTPUT%" (
    echo [ERREUR] Le compilateur n'a pas produit le fichier attendu.
    goto :compile_failed
)

move /y "%BUILD_OUTPUT%" "EXE.exe" >nul
if errorlevel 1 (
    echo [ERREUR] Impossible de remplacer EXE.exe. Fermez l'application si elle est en cours d'execution.
    goto :compile_failed
)

echo [OK] Compilation terminee : %CD%\EXE.exe
if not defined GITHUB_ACTIONS pause
exit /b 0

:compile_failed
if exist "%BUILD_OUTPUT%" del /f /q "%BUILD_OUTPUT%"
echo [ERREUR] La compilation a echoue. Le binaire precedent, s'il existait, a ete conserve.
:fail
if not defined GITHUB_ACTIONS pause
exit /b 1
