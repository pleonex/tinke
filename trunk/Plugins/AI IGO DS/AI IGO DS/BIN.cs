/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 * Programador: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PluginInterface;

namespace AI_IGO_DS
{
    public static class BIN
    {
        public static Control Leer(string archivo, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(new FileStream(archivo, FileMode.Open));

            // Todos los offset del archivo han de ser multiplicados por 4
            // Cabecera
            uint paletaOffset = br.ReadUInt32() * 4;
            uint tileOffset = br.ReadUInt32() * 4;
            uint mapOffset = br.ReadUInt32() * 4;
            ColorDepth profundidad;
            br.BaseStream.Position = mapOffset;
            if (br.ReadUInt32() == 0x01)
                profundidad = ColorDepth.Depth8Bit;
            else
                profundidad = ColorDepth.Depth4Bit;

            // Paleta
            NCLR paleta = new NCLR();
            if (paletaOffset == 0x00) // No hay paleta
            {
                profundidad = ColorDepth.Depth4Bit;
                paleta.pltt.length = (uint)defaultPaletteData.Length;
                paleta.pltt.paletteLength = (uint)0x20;
                paleta.pltt.nColors = (uint)0x10;
                paleta.pltt.depth = profundidad;
                paleta.pltt.palettes = new NTFP[defaultPaletteData.Length / 0x20];
                for (int i = 0; i < paleta.pltt.palettes.Length; i++)
                {
                    Byte[] paletteData = new Byte[paleta.pltt.paletteLength];
                    Array.Copy(defaultPaletteData, i * paleta.pltt.paletteLength, paletteData, 0, paleta.pltt.paletteLength);
                    paleta.pltt.palettes[i].colors = pluginHost.BGR555ToColor(paletteData);
                }
                goto Tile;
            }

            br.BaseStream.Position = paletaOffset;
            uint pCabeceraSize = br.ReadUInt32() * 4;
            uint pSize = br.ReadUInt32() * 4;
            if (pSize - 0x08 == 0x0200)
                profundidad = ColorDepth.Depth8Bit;
            else if (pSize - 0x08 == 0x20)
                profundidad = ColorDepth.Depth4Bit;

            paleta.pltt.length = pSize - 0x08;
            paleta.pltt.paletteLength = (profundidad == ColorDepth.Depth4Bit) ? 0x20 : pSize - 0x08;
            paleta.pltt.nColors = (profundidad == ColorDepth.Depth4Bit) ? 0x10 : (pSize - 0x08) / 2;
            paleta.pltt.depth = profundidad;
            paleta.pltt.palettes = new NTFP[(profundidad == ColorDepth.Depth4Bit ? (pSize - 0x08) / 0x20 : 1)];
            for (int i = 0; i < paleta.pltt.palettes.Length; i++)
                paleta.pltt.palettes[i].colors = pluginHost.BGR555ToColor(br.ReadBytes((int)paleta.pltt.paletteLength));
            
            // Tile data
            Tile:
            br.BaseStream.Position = tileOffset;
            uint tCabeceraSize = br.ReadUInt32() * 4; // siempre 0x04
            uint tSize = br.ReadUInt32() * 4;
            NCGR tile = new NCGR();
            tile.order = TileOrder.Horizontal;
            tile.rahc.depth = profundidad;
            if (profundidad == ColorDepth.Depth4Bit)
                tile.rahc.nTiles = (ushort)((tSize - 0x08) / 0x20);
            else
                tile.rahc.nTiles = (ushort)((tSize - 0x08) / 0x40);

            tile.rahc.tileData.tiles = new byte[tile.rahc.nTiles][];
            tile.rahc.tileData.nPalette = new byte[tile.rahc.nTiles];
            for (int i = 0; i < tile.rahc.nTiles; i++)
            {
                if (profundidad == ColorDepth.Depth4Bit)
                    tile.rahc.tileData.tiles[i] = pluginHost.Bit8ToBit4(br.ReadBytes(32));
                else
                    tile.rahc.tileData.tiles[i] = br.ReadBytes(64);
                tile.rahc.tileData.nPalette[i] = 0;
            }
            NCGR[] tiles;

            // Map
            if (mapOffset == 0x00)
            {
                tiles = new NCGR[1];
                tiles[0] = tile;
                tiles[0].rahc.nTilesX = 0x08;
                tiles[0].rahc.nTilesY = (ushort)(tiles[0].rahc.nTiles / tiles[0].rahc.nTilesX);
                goto Fin;
            }

            br.BaseStream.Position = mapOffset;
            uint mCabeceraSize = br.ReadUInt32() * 4; // Indica el número de subimages
            uint[] mSize = new uint[(int)mCabeceraSize / 4];
            for (int i = 0; i < mSize.Length; i++)
                mSize[i] = (br.ReadUInt32() * 4) - mCabeceraSize - 4;

            NSCR[] maps = new NSCR[mSize.Length];
            tiles = new NCGR[mSize.Length];
            for (int i = 0; i < maps.Length; i++)
            {
                maps[i] = new NSCR();
                maps[i].section.width = br.ReadUInt16();
                maps[i].section.height = br.ReadUInt16();

                if (i != 0)
                    maps[i].section.mapData = new NTFS[((mSize[i] - mSize[i - 1]) - 4) / 2];
                else
                    maps[i].section.mapData = new NTFS[(mSize[i] - 4) / 2];

                for (int j = 0; j < maps[i].section.mapData.Length; j++)
                {
                    ushort parameters = br.ReadUInt16();

                    maps[i].section.mapData[i] = new NTFS();
                    maps[i].section.mapData[i].nTile = (ushort)(parameters & 0x3FF);
                    maps[i].section.mapData[i].xFlip = (byte)((parameters >> 10) & 1);
                    maps[i].section.mapData[i].yFlip = (byte)((parameters >> 11) & 1);
                    maps[i].section.mapData[i].nPalette = (byte)((parameters >> 12) & 0xF);
                }

                tiles[i] = tile;
                tiles[i].rahc.tileData = pluginHost.Transform_NSCR(maps[i], tile.rahc.tileData);
                tiles[i].rahc.nTilesX = maps[i].section.width;
                tiles[i].rahc.nTilesY = maps[i].section.height;
                tiles[i].rahc.nTiles = (ushort)(tiles[i].rahc.nTilesX * tiles[i].rahc.nTilesY);
            }

            Fin:
            br.Close();
            return new BinControl(pluginHost, paleta, tiles);
        }


        public static byte[] defaultPaletteData = {
	0xE0, 0x7F, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x01, 0x00, 0x21, 0x00,
	0x20, 0x00, 0x20, 0x00, 0x21, 0x00, 0x41, 0x00, 0x40, 0x00, 0x60, 0x00,
	0x60, 0x00, 0x41, 0x00, 0x42, 0x00, 0x61, 0x00, 0x80, 0x00, 0x43, 0x04,
	0x45, 0x00, 0x81, 0x00, 0x63, 0x00, 0xA0, 0x00, 0x82, 0x04, 0xA1, 0x00,
	0xC0, 0x00, 0x84, 0x04, 0x47, 0x00, 0xA2, 0x00, 0x65, 0x04, 0xC1, 0x00,
	0x49, 0x00, 0xE0, 0x00, 0xA3, 0x00, 0xC2, 0x04, 0xA5, 0x04, 0xE2, 0x00,
	0x87, 0x04, 0x20, 0x01, 0x89, 0x04, 0xC4, 0x04, 0xE3, 0x04, 0x02, 0x01,
	0x20, 0x01, 0xC6, 0x04, 0x03, 0x09, 0x8B, 0x04, 0xE5, 0x04, 0x40, 0x01,
	0xC8, 0x04, 0x23, 0x05, 0x42, 0x05, 0x04, 0x05, 0x60, 0x01, 0x8D, 0x08,
	0xE7, 0x04, 0x80, 0x01, 0xE9, 0x04, 0x43, 0x05, 0x25, 0x05, 0x80, 0x01,
	0xCC, 0x08, 0x64, 0x05, 0xA0, 0x01, 0x82, 0x05, 0x28, 0x05, 0xC0, 0x01,
	0xC0, 0x01, 0x66, 0x05, 0xEE, 0x08, 0x0C, 0x09, 0xE0, 0x01, 0xD0, 0x08,
	0xC2, 0x05, 0xA5, 0x05, 0x68, 0x09, 0x00, 0x02, 0x4B, 0x09, 0x2E, 0x09,
	0x20, 0x02, 0xA8, 0x05, 0xE5, 0x05, 0x40, 0x02, 0x6D, 0x0D, 0x31, 0x0D,
	0x8B, 0x09, 0x60, 0x02, 0x43, 0x02, 0x6F, 0x0D, 0x80, 0x02, 0xCA, 0x09,
	0x26, 0x06, 0xAD, 0x0D, 0x36, 0x0D, 0x08, 0x06, 0xA0, 0x02, 0xC0, 0x02,
	0x92, 0x0D, 0xCF, 0x0D, 0xE0, 0x02, 0x0D, 0x0A, 0x66, 0x06, 0x94, 0x11,
	0x4B, 0x06, 0x00, 0x03, 0x89, 0x0A, 0x99, 0x11, 0xF3, 0x0D, 0x30, 0x0E,
	0xE7, 0x0A, 0xD7, 0x11, 0x8D, 0x0E, 0xA0, 0x03, 0x91, 0x06, 0xEC, 0x06,
	0xDD, 0x15, 0x39, 0x16, 0x48, 0x0B, 0x75, 0x12, 0x3F, 0x1A, 0xF5, 0x0E,
	0xBA, 0x0E, 0xC9, 0x17, 0x92, 0x13, 0x59, 0x17, 0xFF, 0x1E, 0xF8, 0x1B,
	0x9F, 0x17, 0xFF, 0x23, 0xE0, 0x7F, 0x03, 0x18, 0x03, 0x20, 0x04, 0x24,
	0x04, 0x2C, 0x05, 0x28, 0x04, 0x34, 0x05, 0x30, 0x04, 0x38, 0x05, 0x38,
	0x04, 0x40, 0x04, 0x48, 0x05, 0x4C, 0x8C, 0x31, 0x39, 0x67, 0xFF, 0x7F,
	0xE0, 0x7F, 0x65, 0x24, 0xC8, 0x2C, 0x0A, 0x31, 0x4C, 0x39, 0x8E, 0x41,
	0xCF, 0x45, 0x11, 0x4E, 0x53, 0x52, 0x95, 0x5A, 0xD7, 0x62, 0x19, 0x67,
	0x5A, 0x6F, 0x9C, 0x73, 0xDE, 0x7B, 0xFF, 0x7F, 0x04, 0xE5, 0x04, 0x40,
    0xE0, 0x7F, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x01, 0x00, 0x21, 0x00,
	0x20, 0x00, 0x20, 0x00, 0x21, 0x00, 0x41, 0x00, 0x40, 0x00, 0x60, 0x00,
	0x60, 0x00, 0x41, 0x00, 0x42, 0x00, 0x61, 0x00, 0x80, 0x00, 0x43, 0x04,
	0x45, 0x00, 0x81, 0x00, 0x63, 0x00, 0xA0, 0x00, 0x82, 0x04, 0xA1, 0x00,
	0xC0, 0x00, 0x84, 0x04, 0x47, 0x00, 0xA2, 0x00, 0x65, 0x04, 0xC1, 0x00,
	0x49, 0x00, 0xE0, 0x00, 0xA3, 0x00, 0xC2, 0x04, 0xA5, 0x04, 0xE2, 0x00,
	0x87, 0x04, 0x20, 0x01, 0x89, 0x04, 0xC4, 0x04, 0xE3, 0x04, 0x02, 0x01,
	0x20, 0x01, 0xC6, 0x04, 0x03, 0x09, 0x8B, 0x01
};


    }
}
