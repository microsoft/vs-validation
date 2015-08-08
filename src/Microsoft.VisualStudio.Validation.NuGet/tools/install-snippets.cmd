@ECHO OFF 
SETLOCAL
IF EXIST "%USERPROFILE%\My Documents" SET DOCS=%USERPROFILE%\My Documents
IF EXIST "%USERPROFILE%\Documents" SET DOCS=%USERPROFILE%\Documents
IF "%DOCS%"=="" (
	ECHO Unable to find user documents folder.
	EXIT /b 1
)

FOR %%V IN (2010 2012 2013) DO (
	IF EXIST "%DOCS%\Visual Studio %%V" (
		ECHO Installing snippets for Visual Studio %%V...
		ROBOCOPY /NJH /NJS /NDL /NFL "%~dp02013" "%DOCS%\Visual Studio %%V\Code Snippets\Visual C#\My Code Snippets" *.snippet
	)
)

FOR %%V IN (2015) DO (
	IF EXIST "%DOCS%\Visual Studio %%V" (
		ECHO Installing snippets for Visual Studio %%V...
		ROBOCOPY /NJH /NJS /NDL /NFL "%~dp02015" "%DOCS%\Visual Studio %%V\Code Snippets\Visual C#\My Code Snippets" *.snippet
	)
)
