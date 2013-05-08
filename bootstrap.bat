@echo off
path %path%;%cd%\tools
path %path%;%cd%\source\.nuget
path %path%;%programfiles%\Git\bin
path %path%;%programfiles(x86)%\Git\bin
path %path%;%programfiles%\Subversion\bin
path %path%;%programfiles(x86)%\Subversion\bin

mkdir dependencies >NUL 2>NUL

::setlocal enabledelayedexpansion
set col_stat=e
set col_err=c
set col_dbg=8
set col_proc=7
set col_ok=2

call check-nuget     
if %errorlevel% neq 0 goto E_EXIT
call check-svn
if %errorlevel% neq 0 goto E_EXIT
call check-git                   
if %errorlevel% neq 0 goto E_EXIT
call check-hg     
if %errorlevel% neq 0 goto E_EXIT 

call init-svn https://yaml.svn.codeplex.com/svn/Main/ dependencies/yaml
call init-git https://github.com/chkn/AluminumLua.git dependencies/aluminiumlua
call init-hg https://code.google.com/p/csharp-sqlite/ dependencies/csharp-sqlite
call init-nuget

xecho /a:%col_ok% Finished.
exit /B 0

:E_EXIT
xecho /a:%col_err% "Bootstrapping failed, error code: %errorlevel%"
pause
exit /B -1