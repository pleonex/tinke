#!/bin/bash

# Get Tinke directory for relative paths.
TINKE_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
BUILD_DIR="$TINKE_DIR/build"

# Ask for release or debug configuration
echo "Choose the configuration. Press '1' for Release and '2' for Debug: "
select rd in "R" "D"; do
    case $rd in
        R) CONF="Release"; break;;
        D) CONF="Debug"; break;;
    esac
done

# Ask for 64 or 32 bits.
echo
echo "Choose the platform. Press '1' for x86 or '2' for x64: "
select pl in "1" "2"; do
    case $pl in
        1) PLAT="x86"; break;;
        2) PLAT="x64"; break;;
    esac
done
echo

# Remove previous builds
if [ -d $BUILD_DIR ]; then
    echo "Deleting old build directory"
    rm -rf $BUILD_DIR
fi

# Get compiler and params
XBUILD="xbuild /v:minimal /p:Configuration=$CONF;TarjetFrameworkVersion=v4.0"
XBUILD_PLUGINS="$XBUILD;OutputPath=$BUILD_DIR/Plugins/"

# Compile program in standard (debug) directory, to allow plugins find Ekona
echo "Compiling base library..."
xbuild /v:minimal /p:TarjetFrameworkVersion=v4.0 Tinke.sln  > /dev/null

# Compile Tinke
echo "Compiling Tinke..."
$XBUILD "/p:Platform=$PLAT;OutputPath=$BUILD_DIR/" Tinke.sln > /dev/null

function compile_plugin {
    echo "Compiling plugin $1..."
    $XBUILD_PLUGINS "$1" > /dev/null
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

# Delete debug files
rm "$BUILD_DIR"/*.mdb
rm "$BUILD_DIR"/Plugins/*.mdb

echo "Done!"
