@echo off
set CSC=%SystemRoot%\Microsoft.NET\Framework64\v4.0.30319\csc.exe
if not exist "%CSC%" set CSC=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\csc.exe
"%CSC%" /target:winexe /optimize+ /win32manifest:src\app.manifest /out:EXE.exe /r:System.Windows.Forms.dll /r:System.Drawing.dll src\Program.cs
if %errorlevel%==0 (echo [OK] EXE.exe compile avec succes !) else (echo [ERREUR] Echec de compilation.)
pause
