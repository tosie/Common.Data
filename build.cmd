@ECHO OFF

set framework_version=3.5
set msb=%windir%\Microsoft.NET\Framework\v%framework_version%\MSBuild.exe

set project=Common.Data\Common.Data.csproj
set basename=Common.Data
set outdir=Release

set ilmerge="%programfiles%\Microsoft\ILMerge\ILMerge.exe"
set ilmerge_params=/wildcards /ndebug /lib:%outdir% /lib:%basename%\lib /target:library /xmldocs /zeroPeKind
set package=%outdir%\Common.Data.dll
set assemblies=Common.Data.dll System.Data.SQLite.dll SubSonic.Core.dll
set to_delete=System.Data.SQLite.dll SubSonic.Core.dll

if not exist %msb% goto msbuild_not_found
goto do_build

:do_build
echo Building the project ...
%msb% /target:Build /property:Configuration=Release /property:OutDir=..\%outdir%\ "%project%"
if errorlevel 1 goto build_error
goto build_ok

:build_ok
goto merge_assemblies

:merge_assemblies
if not exist %ilmerge% goto ilmerge_not_found

echo.
echo Merging assemblies ...
echo   Assemblies = %assemblies%
%ilmerge% %ilmerge_params% /out:%package% %assemblies%
if errorlevel 1 goto ilmerge_error
goto ilmerge_ok

:ilmerge_ok
pushd %outdir%
del /q %to_delete%
popd
echo.
echo The assemblies have been merged into "%package%".
goto end

:ilmerge_error
goto end

:ilmerge_not_found
echo.
echo ILMerge not found (expected it at %ilmerge%).
echo Assemblies will not be merged.
echo You can download it from http://research.microsoft.com/en-us/people/mbarnett/ilmerge.aspx
goto end

:msbuild_not_found
echo MSBuild not found (exptected version of .NET Framework: %framework_version%)
if not "%1" == "batch" pause
exit 1

:build_error
echo Error while building the project.
if not "%1" == "batch" pause
exit 2

:end
if not "%1" == "batch" pause
exit 0