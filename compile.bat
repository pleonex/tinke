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
RMDIR /S /Q "%cd%\build"

REM Get compiler
SET netver=v4.5
SET msbuild=%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

REM Compile program in standard directory, to allow plugins find Ekona
%msbuild% Tinke.sln /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=%plat%"

REM Compiling program
%msbuild% Tinke.sln /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=%plat%;OutputPath=%CD%\build\"

REM Compiling game plugins
%msbuild% "Plugins\LAYTON\LAYTON.sln"             /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\KIRBY DRO\KIRBY DRO.sln"       /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\AI IGO DS\AI IGO DS.sln"       /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\LASTWINDOW\LASTWINDOW.sln"     /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\TETRIS DS\TETRIS DS.sln"       /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\999HRPERDOOR\999HRPERDOOR.sln" /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\EDGEWORTH\EDGEWORTH.sln"       /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\GYAKUKEN\GYAKUKEN.sln"         /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\DBK ULTIMATE\DBK ULTIMATE.sln" /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\MAPLESTORYDS\MAPLESTORYDS.sln" /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\NINOKUNI\NINOKUNI.sln"         /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\TOKIMEKIGS3S\TOKIMEKIGS3S.sln" /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\BLOODBAHAMUT\BLOODBAHAMUT.sln" /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\SF FEATHER\SF FEATHER.sln"     /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\DEATHNOTEDS\DEATHNOTEDS.sln"   /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\INAZUMA11\INAZUMA11.sln"       /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\TC UTK\TC UTK.sln"             /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\PSL\PSL.sln"                   /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\HETALIA\HETALIA.sln"           /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\TIMEACE\TIMEACE.sln"           /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\WITCHTALE\WITCHTALE.sln"       /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\Tokimemo1\Tokimemo1.sln"       /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\Teniprimgaku\Teniprimgaku.sln" /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"

REM Compiling format plugins
%msbuild% "Plugins\Pack\Pack.sln"         /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\TXT\TXT.sln"           /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\Common\Common.sln"     /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\Images\Images.sln"     /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\SDAT\SDAT.sln"         /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\Sounds\Sounds.sln"     /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\Fonts\Fonts.sln"       /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"
%msbuild% "Plugins\3DModels\3DModels.sln" /v:minimal "/p:Configuration=%conf%;TarjetFrameworkVersion=%netver%;Platform=Any CPU;OutputPath=%CD%\build\Plugins\"

REM Copy dependencies
COPY "%cd%\Plugins\3DModels\OpenTK.dll" "%cd%\build\"
COPY "%cd%\Plugins\3DModels\OpenTK.GLControl.dll" "%cd%\build\"

REM Copy license and changelog
COPY "%cd%\changelog.txt" "%cd%\build\"
COPY "%cd%\Licence.txt" "%cd%\build\"

REM Delete debug files
DEL /S /Q "%cd%\build\*.pdb"

:end
PAUSE
