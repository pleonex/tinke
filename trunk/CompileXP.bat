:start
ECHO off
CLS

SET /P conf=Choose the configuration. Press R for Release or D for Debug: 
IF /I "%conf%"=="R" SET conf=Release
IF /I "%conf%"=="D" ( SET conf=Debug
) ELSE (IF NOT "%conf%"=="Release" GOTO start)

:secif

SET /P plat=Choose the platform. Press 1 for x86 or 2 for x64: 
IF "%plat%"=="1" SET plat=x86
IF "%plat%"=="2" ( SET plat=x64
) ELSE (IF NOT "%plat%"=="x64" GOTO secif)

:check

SET /P ans=You have choosen the configuration %conf% and the platform %plat%, Is this correct? (y/n) 
IF /I "%ans%"=="N" (GOTO start
) ELSE (IF /I NOT "%ans%"=="Y" GOTO check)

RMDIR /S /Q "%cd%\build"

REM Create the plugin DLL needed for the plugins (pluginInterface)
%windir%\microsoft.net\framework\v4.0.30319\msbuild Tinke.sln /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=%plat%"
REM Compiling program
%windir%\microsoft.net\framework\v4.0.30319\msbuild Tinke.sln /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=%plat%" "/p:OutputPath=%CD%\build\"

REM Compiling game plugins
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\LAYTON\LAYTON.sln /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\KIRBY DRO\KIRBY DRO.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\AI IGO DS\AI IGO DS.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\LASTWINDOW\LASTWINDOW.sln /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\TETRIS DS\TETRIS DS.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\999HRPERDOOR\999HRPERDOOR.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\EDGEWORTH\EDGEWORTH.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\GYAKUKEN\GYAKUKEN.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\DBK ULTIMATE\DBK ULTIMATE.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\MAPLESTORYDS\MAPLESTORYDS.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\NINOKUNI\NINOKUNI.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\SUBARASHIKI\SUBARASHIKI.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\TOKIMEKIGS3S\TOKIMEKIGS3S.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\BLOODBAHAMUT\BLOODBAHAMUT.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\RUNEFACTORY3\RUNEFACTORY3.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\SF FEATHER\SF FEATHER.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\DEATHNOTEDS\DEATHNOTEDS.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\INAZUMA11\INAZUMA11.sln" /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"


REM Compiling format plugins
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\Pack\Pack.sln /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\TXT\TXT.sln /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\Common\Common.sln /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\Images\Images.sln /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\SDAT\SDAT.sln /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\Sounds\Sounds.sln /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\Fonts\Fonts.sln /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\3DModels\3DModels.sln /v:minimal /p:Configuration=%conf%;TarjetFrameworkVersion=v3.5 "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"

COPY "%cd%\changelog.txt" "%cd%\build\"
COPY "%cd%\Licence.txt" "%cd%\build\"
COPY "%cd%\Plugins\3DModels\OpenTK.dll" "%cd%\build\"
COPY "%cd%\Plugins\3DModels\OpenTK.GLControl.dll" "%cd%\build\"

DEL /S /Q "%cd%\build\*.pdb"

:end
PAUSE