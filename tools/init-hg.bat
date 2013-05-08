@echo off
set url=%1
set name=%2
set target=%name%

if not exist "%target%\.hg\hgrc" (
	xecho /a:%col_stat% "Downloading library %name%..."
	hg clone %url% "%target%"|xecho /a:%col_proc% /f:"\t{}"
) else (
	xecho /a:%col_stat% "Updating library %name%..."
	pushd "%target%"
  hg pull|xecho /a:%col_proc% /f:"\t{}"
  hg update|xecho /a:%col_proc% /f:"\t{}"
  popd
)