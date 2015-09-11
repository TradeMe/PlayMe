@ECHO OFF
IF EXIST .\Scripts\app\main*-built.* (
ECHO Cleaning...
del .\Scripts\app\main*-built.*
)
ECHO Installing npm packages...
call npm install
ECHO Compiling...
call gulp
ECHO Done.
PAUSE
