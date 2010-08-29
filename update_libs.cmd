@ECHO OFF

set basename=Common.Data
set libdir=%basename%\lib

:do_update
echo Updating external libraries ...
if not exist %libdir% mkdir %libdir%

copy /Y ..\Common.Configuration\Release\*.dll %libdir%
if errorlevel 1 goto update_error

goto update_ok

:update_ok
goto end

:update_error
echo Error while updating the project's libraries.
if not "%1" == "batch" pause
exit 2

:end
if not "%1" == "batch" pause
exit 0