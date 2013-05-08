@echo off
set url=%1
set name=%2
set target=%name%

if not exist "%target%\.svn\wc.db" (
	xecho /a:%col_stat% "Downloading library %name%..."
	svn co --non-interactive --trust-server-cert %url% "%target%" | xecho /a:%col_proc% /f:"\t{}"
) else (
	xecho /a:%col_stat% "Updating library %name%..."
	svn cleanup --non-interactive "%target%" | xecho /a:%col_proc% /f:"\t{}"
	svn up --non-interactive "%target%"  | xecho /a:%col_proc% /f:"\t{}"
)
