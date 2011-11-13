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
using System.Drawing;
using PluginInterface;

namespace TETRIS_DS
{
    public static class OBJS
    {
        public static void Read(string file, IPluginHost pluginHost)
        {
            pluginHost.Decompress(file);
            string dec_file;
            sFolder dec_folder = pluginHost.Get_Files();

            if (dec_folder.files is List<sFile>)
                dec_file = dec_folder.files[0].path;
            else
            {
                string tempFile = Path.GetTempFileName();
                Byte[] compressFile = new Byte[(new FileInfo(file).Length) - 0x08];
                Array.Copy(File.ReadAllBytes(file), 0x08, compressFile, 0, compressFile.Length); ;
                File.WriteAllBytes(tempFile, compressFile);

                pluginHost.Decompress(tempFile);
                dec_file = pluginHost.Get_Files().files[0].path;
            }

            BinaryReader br = new BinaryReader(File.OpenRead(dec_file));

            #region Cell
            NCER cells = new NCER();
            cells.header.id = "OBJS".ToCharArray();
            cells.header.file_size = (uint)br.BaseStream.Length;

            cells.cebk.nBanks = (ushort)br.ReadUInt32();
            cells.cebk.banks = new Bank[cells.cebk.nBanks];
            uint num_cells = br.ReadUInt32();
            cells.cebk.unknown1 = br.ReadUInt32(); // Unknonwn
            cells.cebk.block_size = 2;

            // Read bank information
            for (int i = 0; i < cells.cebk.nBanks; i++)
            {
                cells.cebk.banks[i].unknown1 = br.ReadUInt16();
                cells.cebk.banks[i].nCells = br.ReadUInt16();
                cells.cebk.banks[i].cells = new Cell[cells.cebk.banks[i].nCells];
            }

            // Read cell information
            for (int i = 0; i < cells.cebk.nBanks; i++)
            {
                for (int j = 0; j < cells.cebk.banks[i].nCells; j++)
                {
                    cells.cebk.banks[i].cells[j].obj1.xOffset = br.ReadInt16();
                    cells.cebk.banks[i].cells[j].obj0.yOffset = br.ReadInt16();

                    uint sizeByte = br.ReadUInt32();
                    byte byte1 = (byte)((sizeByte & 0xF0) >> 4);
                    byte byte2 = (byte)(sizeByte & 0x0F);
                    System.Drawing.Size size = Obtener_Tamaño(byte1, byte2);
                    cells.cebk.banks[i].cells[j].width = (ushort)size.Width;
                    cells.cebk.banks[i].cells[j].height = (ushort)size.Height;

                    cells.cebk.banks[i].cells[j].obj2.tileOffset = br.ReadUInt32();
                    cells.cebk.banks[i].cells[j].obj2.index_palette = 0;
                    cells.cebk.banks[i].cells[j].num_cell = (ushort)j;
                }
            }
            pluginHost.Set_NCER(cells);
            #endregion

            #region Palette
            NCLR nclr = new NCLR();
            uint file_size = (uint)(br.BaseStream.Length - br.BaseStream.Position);
            nclr.header.id = "OBJS".ToCharArray();
            nclr.header.constant = 0x0100;
            nclr.header.file_size = file_size;
            nclr.header.header_size = 0x10;

            nclr.pltt.ID = "PLZ ".ToCharArray();
            nclr.pltt.length = file_size;
            nclr.pltt.depth = (file_size > 0x20) ? System.Windows.Forms.ColorDepth.Depth8Bit : System.Windows.Forms.ColorDepth.Depth4Bit;
            nclr.pltt.unknown1 = 0x00000000;
            nclr.pltt.paletteLength = file_size;
            nclr.pltt.nColors = file_size / 2;
            nclr.pltt.palettes = new NTFP[1];

            nclr.pltt.palettes[0].colors = pluginHost.BGR555ToColor(br.ReadBytes((int)file_size));

            pluginHost.Set_NCLR(nclr);
            #endregion
            br.Close();
        }

        public static Size Obtener_Tamaño(byte byte1, byte byte2)
        {
            byte1 = (byte)((byte2 & 0x03) << 2);
            byte2 = (byte)(byte2 & 0x0C);
            Size tamaño = new Size();

            switch (byte1)
            {
                case 0x00:
                    switch (byte2)
                    {
                        case 0x00:
                            tamaño = new Size(8, 8);
                            break;
                        case 0x04:
                            tamaño = new Size(16, 16);
                            break;
                        case 0x08:
                            tamaño = new Size(32, 32);
                            break;
                        case 0x0C:
                            tamaño = new Size(64, 64);
                            break;
                    }
                    break;
                case 0x04:
                    switch (byte2)
                    {
                        case 0x00:
                            tamaño = new Size(16, 8);
                            break;
                        case 0x04:
                            tamaño = new Size(32, 8);
                            break;
                        case 0x08:
                            tamaño = new Size(32, 16);
                            break;
                        case 0x0C:
                            tamaño = new Size(64, 32);
                            break;
                    }
                    break;
                case 0x08:
                    switch (byte2)
                    {
                        case 0x00:
                            tamaño = new Size(8, 16);
                            break;
                        case 0x04:
                            tamaño = new Size(8, 32);
                            break;
                        case 0x08:
                            tamaño = new Size(16, 32);
                            break;
                        case 0x0C:
                            tamaño = new Size(32, 64);
                            break;
                    }
                    break;
                case 0x0C:
                    switch (byte2)
                    {
                        case 0x00:
                            tamaño = new Size(64, 128);
                            break;
                        case 0x04:
                            tamaño = new Size(0, 0);
                            break;
                        case 0x08:
                            tamaño = new Size(0, 0);
                            break;
                        case 0x0C:
                            tamaño = new Size(0, 0);
                            break;
                    }
                    break;
            }

            return tamaño;
        }

    }
}
