@echo off

@setlocal enableextensions enabledelayedexpansion
set EnableNuGetPackageRestore=true

::if "%1"=="-self" (
::	xecho /a:%col_stat% "Updating NuGet..."
::	%nuget% update %nugetopt% -Self
::)

xecho /a:%col_stat% "Updating NuGet packages..."

for /r %%i in (packages.config) DO (
	set foo1=%%i
	set foo2=!foo1:\.=!
	set foo2=!foo2:\obj\=!
	set foo2=!foo2:\bin\=!
	if exist "!foo1!" if "!foo1!"=="!foo2!" (
		%nuget% install "!foo1!" -o packages | xecho /a:%col_proc% /f:"\t{}"
	)
)

for /r %%i in (*.sln) DO (
	%nuget% update %nugetopt% "%%i" | xecho /a:%col_proc% /f:"\t{}"
)

endlocal