:inicio
ECHO off
CLS

CHOICE /C RD /M "Elija la configuraci¢n: presione R para Release o D para Debug."
IF errorlevel 1 set conf=Release
IF errorlevel 2 set conf=Debug

CHOICE /C 123 /M "Elija la plataforma: presione 1 para x86, 2 para x64 o 3 para Any CPU."
IF errorlevel 1 set plat=x86
IF errorlevel 2 set plat=x64
IF errorlevel 3 set plat=Any CPU

CHOICE /C SN /M "Ha elejido la configuraci¢n  %conf% y la plataforma %plat%, ¨es correcto?"
IF errorlevel 2 goto inicio

%windir%\microsoft.net\framework\v4.0.30319\msbuild Tinke.sln /v:minimal /p:Configuration=%conf% "/p:Platform=%plat%" "/p:OutputPath=%CD%\build\"

REM Compilación de plugins de los juegos
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\LAYTON\LAYTON.sln /v:minimal /p:Configuration=%conf% "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild "Plugins\KIRBY DRO\KIRBY DRO.sln" /v:minimal /p:Configuration=%conf% "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"

REM Compilación de los plugins de formatos
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\NARC\NARC.sln /v:minimal /p:Configuration=%conf% "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\PCM\PCM.sln /v:minimal /p:Configuration=%conf% "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\TXT\TXT.sln /v:minimal /p:Configuration=%conf% "/p:Platform=Any CPU" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\Comun\Comun.sln /v:minimal /p:Configuration=%conf% "/p:Platform=x86" "/p:OutputPath=%CD%\build\Plugins\"
%windir%\microsoft.net\framework\v4.0.30319\msbuild Plugins\Nintendo\Nintendo.sln /v:minimal /p:Configuration=%conf% "/p:Platform=x86" "/p:OutputPath=%CD%\build\Plugins\"

PAUSE