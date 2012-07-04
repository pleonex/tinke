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
using Ekona;
using Ekona.Images;

namespace TETRIS_DS
{
    public class CLZ : ImageBase
    {
        IPluginHost pluginHost;

        public CLZ(IPluginHost pluginHost, string file, int id) : base(file, id) { this.pluginHost = pluginHost; }

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
            Byte[] tiles = br.ReadBytes((int)br.BaseStream.Length);

            br.Close();
            Set_Tiles(tiles, 0x100, tiles.Length / 0x100, ColorFormat.colors16, TileForm.Horizontal, false);
            pluginHost.Set_Image(this);
        }
        public override void Write(string fileOut, PaletteBase palette)
        {
            throw new NotImplementedException();
        }
    }
}
