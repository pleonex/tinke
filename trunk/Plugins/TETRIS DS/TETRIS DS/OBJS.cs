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
 * By: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Ekona;
using Ekona.Images;

namespace TETRIS_DS
{
    public class OBJS : SpriteBase
    {
        IPluginHost pluginHost;

        public OBJS(IPluginHost pluginHost, string file, int id) : base(file, id) { this.pluginHost = pluginHost; }

        public override void Read(string file)
        {
            // It's compressed
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

            // Bank info
            Ekona.Images.Bank[] banks = new Ekona.Images.Bank[br.ReadUInt32()];
            uint num_cells = br.ReadUInt32();
            uint unknown1 = br.ReadUInt32();

            for (int i = 0; i < banks.Length; i++)
            {
                uint unk = br.ReadUInt16();
                banks[i].oams = new OAM[br.ReadUInt16()];
            }

            // Read cell information
            for (int i = 0; i < banks.Length; i++)
            {
                for (int j = 0; j < banks[i].oams.Length; j++)
                {
                    banks[i].oams[j].obj1.xOffset = br.ReadInt16();
                    banks[i].oams[j].obj0.yOffset = br.ReadInt16();

                    uint size_b = br.ReadUInt32();
                    byte b1 = (byte)(size_b & 0x03);
                    byte b2 = (byte)((size_b & 0x0C) >> 2);
                    System.Drawing.Size size = Actions.Get_OAMSize(b1, b2);
                    banks[i].oams[j].width = (ushort)size.Width;
                    banks[i].oams[j].height = (ushort)size.Height;

                    banks[i].oams[j].obj2.tileOffset = br.ReadUInt32();
                    banks[i].oams[j].obj2.index_palette = 0;
                    banks[i].oams[j].num_cell = (ushort)j;
                }
            }
            Set_Banks(banks, 2, false);
            pluginHost.Set_Sprite(this);

            // Palette
            PaletteBase palette;
            int palette_length = (int)(br.BaseStream.Length - br.BaseStream.Position);
            Color[][] colors = new Color[1][];
            colors[0] = Actions.BGR555ToColor(br.ReadBytes(palette_length));

            br.Close();

            palette = new RawPalette(colors, false, (palette_length > 0x20) ? ColorFormat.colors256 : ColorFormat.colors16);
            pluginHost.Set_Palette(palette);
        }
        public override void Write(string fileOut, ImageBase image, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }
}
