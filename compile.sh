#!/bin/bash

# Get Tinke directory for relative paths.
TINKE_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
BUILD_DIR="$TINKE_DIR/build"

# Ask for release or debug configuration
if [[ "$1" != "Release" && "$1" != "Debug" ]] ; then
    echo "Choose the configuration. Press '1' for Release and '2' for Debug: "
    select rd in "Release" "Debug"; do
        case $rd in
            Release) CONF="Release"; break;;
            Debug) CONF="Debug"; break;;
        esac
    done
    echo
else
    echo "Using $1 configuration."
    CONF=$1
fi

# Ask for 64 or 32 bits.
if [[ "$2" != "x86" && "$2" != "x64" ]] ; then
    echo "Choose the platform. Press '1' for x86 or '2' for x64: "
    select pl in "x86" "x64"; do
        case $pl in
            x86) PLAT="x86"; break;;
            x64) PLAT="x64"; break;;
        esac
    done
    echo
else
    echo "Compiling for platform $2."
    PLAT=$2
fi

# Remove previous builds
if [ -d "$BUILD_DIR" ]; then
    echo "Deleting old build directory"
    rm -rf "$BUILD_DIR"
fi

# Get compiler and params
XBUILD="msbuild /v:minimal /p:Configuration=$CONF"

# Compile program in standard directory, to allow plugins find Ekona
echo "Compiling base library..."
msbuild /v:minimal Tinke.sln > build.log
if [ $? -ne 0 ] ; then
    echo "Error compiling Tinke into the default directory. Aborting."
    cat build.log
    exit -1
fi

# Compile Tinke
echo "Compiling Tinke..."
$XBUILD "/p:Platform=$PLAT;OutputPath=$BUILD_DIR/" Tinke.sln > build.log
if [ $? -ne 0 ] ; then
    echo "Error compiling Tinke into the build dir. Aborting."
    cat build.log
    exit -1
fi

function compile_plugin {
    echo "Compiling plugin $1..."
    $XBUILD "/p:OutputPath=$BUILD_DIR/Plugins/" "$1" > build.log
    if [ $? -ne 0 ] ; then
        echo "Error compiling $1. Aborting."
        cat build.log
        exit -1
    fi
}

# Compile game plugins
compile_plugin "Plugins/LAYTON/LAYTON.sln"
compile_plugin "Plugins/KIRBY DRO/KIRBY DRO.sln"
compile_plugin "Plugins/AI IGO DS/AI IGO DS.sln"
compile_plugin "Plugins/LASTWINDOW/LASTWINDOW.sln"
compile_plugin "Plugins/TETRIS DS/TETRIS DS.sln"
compile_plugin "Plugins/999HRPERDOOR/999HRPERDOOR.sln"
compile_plugin "Plugins/EDGEWORTH/EDGEWORTH.sln"
compile_plugin "Plugins/GYAKUKEN/GYAKUKEN.sln"
compile_plugin "Plugins/DBK ULTIMATE/DBK ULTIMATE.sln"
compile_plugin "Plugins/MAPLESTORYDS/MAPLESTORYDS.sln"
compile_plugin "Plugins/NINOKUNI/NINOKUNI.sln"
compile_plugin "Plugins/TOKIMEKIGS3S/TOKIMEKIGS3S.sln"
compile_plugin "Plugins/BLOODBAHAMUT/BLOODBAHAMUT.sln"
compile_plugin "Plugins/SF FEATHER/SF FEATHER.sln"
compile_plugin "Plugins/DEATHNOTEDS/DEATHNOTEDS.sln"
compile_plugin "Plugins/INAZUMA11/INAZUMA11.sln"
compile_plugin "Plugins/TC UTK/TC UTK.sln"
compile_plugin "Plugins/PSL/PSL.sln"
compile_plugin "Plugins/HETALIA/HETALIA.sln"
compile_plugin "Plugins/TIMEACE/TIMEACE.sln"
compile_plugin "Plugins/WITCHTALE/WITCHTALE.sln"
compile_plugin "Plugins/Tokimemo1/Tokimemo1.sln"
compile_plugin "Plugins/Teniprimgaku/Teniprimgaku.sln"
compile_plugin "Plugins/SONICRUSHADV/SONICRUSHADV.sln"
compile_plugin "Plugins/1stPlayable/1stPlayable.sln"
compile_plugin "Plugins/JUS/JUS.sln"

# Compiling format plugins
compile_plugin "Plugins/Pack/Pack.sln"
compile_plugin "Plugins/TXT/TXT.sln"
compile_plugin "Plugins/Common/Common.sln"
compile_plugin "Plugins/Images/Images.sln"
compile_plugin "Plugins/SDAT/SDAT.sln"
compile_plugin "Plugins/Sounds/Sounds.sln"
compile_plugin "Plugins/Fonts/Fonts.sln"
compile_plugin "Plugins/3DModels/3DModels.sln"

# Copy dependencies
cp "$TINKE_DIR/Plugins/3DModels/OpenTK.dll" "$BUILD_DIR/"
cp "$TINKE_DIR/Plugins/3DModels/OpenTK.GLControl.dll" "$BUILD_DIR/"

# Copy license and changelog
cp "$TINKE_DIR/changelog.txt" "$BUILD_DIR/"
cp "$TINKE_DIR/Licence.txt" "$BUILD_DIR/"
cp "$TINKE_DIR/Tinke/app.config" "$BUILD_DIR/"

# Delete debug files
rm build.log

echo "Done!"
