@echo off
REM #################################################################################################################################
REM #                                                                                                                               #
REM # ENTWIKCLUNG EINES SOFTWARESYSTEMS                                                                                             #
REM # Kaminski, Nils                                                                                                                #
REM # Prueflingsnummer: 101 20546                                                                                                   #
REM #                                                                                                                               #
REM # DIESES SCRIPT COMPILIERT DAS PROGRAMM      																					#
REM # - der Pfad der MSBuild.exe, in den Versionen 3.5 oder 4.0, muss gesucht werden. #                                             #
REM #   Dafür muss die Variable WinDirNet, auf das Installationsverzeichnis der .NET Frameworks verweisen                           #
REM # - der Pfad zu der Parkettroboter-Projektdatei, Parkettroboter.sln, muss in der Variablen PFAD_PROJEKT_DATEI stehen            #
REM #                                                                                                                               #
REM #################################################################################################################################


set WinDirNet=%WinDir%\Microsoft.NET\Framework
IF EXIST "%WinDirNet%\v3.5\MSBuild.exe" set msbuild="%WinDirNet%\v3.5\MSBuild.exe"
IF EXIST "%WinDirNet%\v4.0.30319\MSBuild.exe" set msbuild="%WinDirNet%\v4.0.30319\MSBuild.exe"

REM Variablen setzen
SET PFAD_PROJEKT_DATEI=LaserCameraCSaC.sln


call %msbuild% %PFAD_PROJEKT_DATEI% /p:OutputDir=%PFAD_AUSGABE_ORDNER% /p:Configuration=Release /p:Optimize=true

