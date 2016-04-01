:start
ECHO off
CLS

REM Ask for release or debug configuration
SET /P conf=Choose the configuration. Press R for Release or D for Debug: 
IF /I "%conf%"=="R" SET conf=Release
IF /I "%conf%"=="D" ( SET conf=Debug
) ELSE (IF NOT "%conf%"=="Release" GOTO start)

:secif

REM Ask for platform
SET /P plat=Choose the platform. Press 1 for x86 or 2 for x64: 
IF "%plat%"=="1" SET plat=x86
IF "%plat%"=="2" ( SET plat=x64
) ELSE (IF NOT "%plat%"=="x86" GOTO secif)

:check

REM Make sure you have choosen everything ok
SET /P ans=You have choosen the configuration %conf% and the platform %plat%, Is this correct? (y/n) 
IF /I "%ans%"=="N" (GOTO start
) ELSE (IF /I NOT "%ans%"=="Y" GOTO check)

REM Remove previoues build
SET build_dir=%CD%\build
IF EXIST "%build_dir%" RMDIR /S /Q "%build_dir%"

REM Get compiler
SET netver=v4.5
SET msbuild_path=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
SET msbuild=%msbuild_path% /v:minimal /p:Configuration=%conf% /p:TargetFrameworkVersion=%netver%
SET msbuild_plugins=%msbuild% /p:OutputPath=%build_dir%\Plugins\

REM Compile program in standard directory, to allow plugins find Ekona
ECHO Compiling base library...
%msbuild_path% /v:minimal /p:TargetFrameworkVersion=%netver% Tinke.sln

REM Compiling program
echo Compiling Tinke...
%msbuild% /p:Platform=%plat% /p:OutputPath=%build_dir%\ Tinke.sln

REM Compiling format plugins
%msbuild_plugins% "Plugins\Pack\Pack.sln"
%msbuild_plugins% "Plugins\TXT\TXT.sln"
%msbuild_plugins% "Plugins\Common\Common.sln"
%msbuild_plugins% "Plugins\Images\Images.sln"
%msbuild_plugins% "Plugins\SDAT\SDAT.sln"
%msbuild_plugins% "Plugins\Sounds\Sounds.sln"
%msbuild_plugins% "Plugins\Fonts\Fonts.sln"
%msbuild_plugins% "Plugins\3DModels\3DModels.sln"

REM Compiling game plugins
%msbuild_plugins% "Plugins\LAYTON\LAYTON.sln"
%msbuild_plugins% "Plugins\KIRBY DRO\KIRBY DRO.sln"
%msbuild_plugins% "Plugins\AI IGO DS\AI IGO DS.sln"
%msbuild_plugins% "Plugins\LASTWINDOW\LASTWINDOW.sln"
%msbuild_plugins% "Plugins\TETRIS DS\TETRIS DS.sln"
%msbuild_plugins% "Plugins\999HRPERDOOR\999HRPERDOOR.sln"
%msbuild_plugins% "Plugins\EDGEWORTH\EDGEWORTH.sln"
%msbuild_plugins% "Plugins\GYAKUKEN\GYAKUKEN.sln"
%msbuild_plugins% "Plugins\DBK ULTIMATE\DBK ULTIMATE.sln"
%msbuild_plugins% "Plugins\MAPLESTORYDS\MAPLESTORYDS.sln"
%msbuild_plugins% "Plugins\NINOKUNI\NINOKUNI.sln"
%msbuild_plugins% "Plugins\TOKIMEKIGS3S\TOKIMEKIGS3S.sln"
%msbuild_plugins% "Plugins\BLOODBAHAMUT\BLOODBAHAMUT.sln"
%msbuild_plugins% "Plugins\SF FEATHER\SF FEATHER.sln"
%msbuild_plugins% "Plugins\DEATHNOTEDS\DEATHNOTEDS.sln"
%msbuild_plugins% "Plugins\INAZUMA11\INAZUMA11.sln"
%msbuild_plugins% "Plugins\TC UTK\TC UTK.sln"
%msbuild_plugins% "Plugins\PSL\PSL.sln"
%msbuild_plugins% "Plugins\HETALIA\HETALIA.sln"
%msbuild_plugins% "Plugins\TIMEACE\TIMEACE.sln"
%msbuild_plugins% "Plugins\WITCHTALE\WITCHTALE.sln"
%msbuild_plugins% "Plugins\Tokimemo1\Tokimemo1.sln"
%msbuild_plugins% "Plugins\Teniprimgaku\Teniprimgaku.sln"

REM Copy dependencies
COPY "%cd%\Plugins\3DModels\OpenTK.dll" "%build_dir%\"
COPY "%cd%\Plugins\3DModels\OpenTK.GLControl.dll" "%build_dir%\"

REM Copy license and changelog
COPY "%cd%\changelog.txt" "%build_dir%\"
COPY "%cd%\Licence.txt" "%build_dir%\"

REM Delete debug files
DEL /S /Q "%build_dir%\*.pdb"

:end
PAUSE
