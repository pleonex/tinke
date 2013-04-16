// ----------------------------------------------------------------------
// <copyright file="Class1.cs" company="none">
// Copyright (C) 2013
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see "http://www.gnu.org/licenses/". 
//
// </copyright>
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>07/03/2013 21:03:41</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ekona;
using Ekona.Images;

namespace Teniprimgaku
{
    public enum PackTypes
    {
        FileData = 0,
        ProjectData = 1,
        ProjectTable = 2,
    }

    public class Main : IGamePlugin
    {
        private IPluginHost pluginHost;
        private string gameCode;

        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }

        public bool IsCompatible()
        {
            if (this.gameCode == "BTGJ" || this.gameCode == "TENJ")
            {
                return true;
            }

            return false;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = Encoding.ASCII.GetString(magic);

            if (file.id == this.GetPackId(PackTypes.FileData) ||
                file.id == this.GetPackId(PackTypes.ProjectTable))
            {
                return Format.Pack;
            }

            if (file.id == this.GetPackId(PackTypes.ProjectData))
            {
                return Format.System;
            }

            if (ext == "SCRN")
            {
                return Format.FullImage;
            }

            if (ext == "CELL")
            {
                return Format.Cell;
            }

            if (file.name.EndsWith(".inl"))
            {
                return Format.Text;
            }

            return Format.Unknown;
        }

        public int GetPackId(PackTypes type)
        {
            if (this.gameCode == "BTGJ")
            {
                if (type == PackTypes.FileData)
                    return 0x01;
                if (type == PackTypes.ProjectTable)
                    return 0x04;
                if (type == PackTypes.ProjectData)
                    return 0x05;
            }
            else if (this.gameCode == "TENJ")
            {
                if (type == PackTypes.FileData)
                    return 0x05;
                if (type == PackTypes.ProjectTable)
                    return 0x0C;
                if (type == PackTypes.ProjectData)
                    return 0x0D;
            }

            return -1;
        }

        public string Pack(ref sFolder unpacked, sFile file)
        {
            throw new NotImplementedException();
        }

        public sFolder Unpack(sFile file)
        {
            if (file.id == this.GetPackId(PackTypes.FileData))
            {
                if (this.gameCode == "BTGJ")
                    return Filedata.Unpack_Type1(file);
                else
                    return Filedata.Unpack_Type2(file);
            }
            else if (file.id == this.GetPackId(PackTypes.ProjectTable))
            {
                string dataFile = this.pluginHost.Search_File(this.GetPackId(PackTypes.ProjectData));
                return Project.Unpack(file, dataFile);
            }

            return new sFolder();
        }

        public void Read(sFile file)
        {
        }

        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            if (file.name.EndsWith(".inl"))
            {
                return new TextViewer(file);
            }

            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            string ext = new string(br.ReadChars(4));
            br.Close();

            if (ext == "SCRN")
            {
                return new ScrnControl(new Scrn(file));
            }
            else if (ext == "CELL")
            {
                Cell cell = new Cell(file);
                return new SpriteControl(pluginHost, cell.Sprite.Sprite, cell.Sprite.Image, cell.Sprite.Palette);
            }

            return new System.Windows.Forms.Control();
        }
    }
}
