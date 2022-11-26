@ECHO off
setlocal enableDelayedExpansion

REM Clean variables
SET conf=
SET plat=

REM Ask for Release or Debug configuration
IF [%1] == [] (
    :ask_conf
    SET /P resp=Choose the configuration. Press R for Release or D for Debug: 
    IF /I "!resp!" EQU "R" SET conf=Release
    IF /I "!resp!" EQU "D" SET conf=Debug

    REM If other input repeat
    IF [!conf!] EQU [] GOTO ask_conf
) ELSE (
    SET conf=%1
)
ECHO Configuration: %conf%

REM Ask for platform
IF [%2] == [] (
    :ask_plat
    SET /P resp=Choose the platform. Press 1 for x86 or 2 for x64: 
    IF "!resp!" EQU "1" SET plat=x86
    IF "!resp!" EQU "2" SET plat=x64

    REM If other input repease
    IF [!plat!] EQU [] GOTO ask_plat
) ELSE (
    SET plat=%2
)
ECHO Platform: %plat%

REM Remove previoues build
SET build_dir=%CD%\build
IF EXIST "%build_dir%" RMDIR /S /Q "%build_dir%" || EXIT /B 1

REM Get compiler
SET msbuild_path=MSBuild.exe
SET msbuild=%msbuild_path% /p:Configuration=%conf%
SET msbuild_plugin=%msbuild% /p:OutputPath="%build_dir%\Plugins\\"

REM Compile program in standard directory, to allow plugins find Ekona
ECHO Compiling base library
%msbuild_path% Tinke.sln > error.log || (TYPE error.log & EXIT /B 1)

REM Compiling program
echo Compiling Tinke
%msbuild% /p:Platform=%plat% /p:OutputPath="%build_dir%\\" Tinke.sln > error.log || (TYPE error.log & EXIT /B 1)

REM Compiling format plugins
call :compile_plugin "Plugins\Pack\Pack.sln"
call :compile_plugin "Plugins\TXT\TXT.sln"
call :compile_plugin "Plugins\Common\Common.sln"
call :compile_plugin "Plugins\Images\Images.sln"
call :compile_plugin "Plugins\SDAT\SDAT.sln"
call :compile_plugin "Plugins\Sounds\Sounds.sln"
call :compile_plugin "Plugins\Fonts\Fonts.sln"
call :compile_plugin "Plugins\3DModels\3DModels.sln"

REM Compiling game plugins
call :compile_plugin "Plugins\LAYTON\LAYTON.sln"
call :compile_plugin "Plugins\KIRBY DRO\KIRBY DRO.sln"
call :compile_plugin "Plugins\AI IGO DS\AI IGO DS.sln"
call :compile_plugin "Plugins\LASTWINDOW\LASTWINDOW.sln"
call :compile_plugin "Plugins\TETRIS DS\TETRIS DS.sln"
call :compile_plugin "Plugins\999HRPERDOOR\999HRPERDOOR.sln"
call :compile_plugin "Plugins\EDGEWORTH\EDGEWORTH.sln"
call :compile_plugin "Plugins\GYAKUKEN\GYAKUKEN.sln"
call :compile_plugin "Plugins\DBK ULTIMATE\DBK ULTIMATE.sln"
call :compile_plugin "Plugins\MAPLESTORYDS\MAPLESTORYDS.sln"
call :compile_plugin "Plugins\NINOKUNI\NINOKUNI.sln"
call :compile_plugin "Plugins\TOKIMEKIGS3S\TOKIMEKIGS3S.sln"
call :compile_plugin "Plugins\BLOODBAHAMUT\BLOODBAHAMUT.sln"
call :compile_plugin "Plugins\SF FEATHER\SF FEATHER.sln"
call :compile_plugin "Plugins\DEATHNOTEDS\DEATHNOTEDS.sln"
call :compile_plugin "Plugins\INAZUMA11\INAZUMA11.sln"
call :compile_plugin "Plugins\TC UTK\TC UTK.sln"
call :compile_plugin "Plugins\PSL\PSL.sln"
call :compile_plugin "Plugins\HETALIA\HETALIA.sln"
call :compile_plugin "Plugins\TIMEACE\TIMEACE.sln"
call :compile_plugin "Plugins\WITCHTALE\WITCHTALE.sln"
call :compile_plugin "Plugins\Tokimemo1\Tokimemo1.sln"
call :compile_plugin "Plugins\Teniprimgaku\Teniprimgaku.sln"
call :compile_plugin "Plugins\SONICRUSHADV\SONICRUSHADV.sln"
call :compile_plugin "Plugins\1stPlayable\1stPlayable.sln"
call :compile_plugin "Plugins\JUS\JUS.sln"

REM Remove the error log
DEL error.log

REM Copy dependencies
ECHO Copying dependencies
COPY "%cd%\Plugins\3DModels\OpenTK.dll" "%build_dir%\" > nul || (EXIT /B 1)
COPY "%cd%\Plugins\3DModels\OpenTK.GLControl.dll" "%build_dir%\" > nul || (EXIT /B 1)

REM Copy license and changelog
ECHO Copying license and changelog
COPY "%cd%\changelog.txt" "%build_dir%\" > nul || (EXIT /B 1)
COPY "%cd%\Licence.txt" "%build_dir%\" > nul || (EXIT /B 1)
COPY "%cd%\Tinke\app.config" "%build_dir%\Tinke.exe.config" > nul || (EXIT /B 1)

REM Delete debug files
ECHO Removing debug files
DEL /S /Q "%build_dir%\*.pdb" > nul || (EXIT /B 1)

REM The End
EXIT /B 0

:compile_plugin
echo Compiling plugin %1
%msbuild_plugin% %1 > error.log || (TYPE error.log & EXIT 1)
EXIT /B 0