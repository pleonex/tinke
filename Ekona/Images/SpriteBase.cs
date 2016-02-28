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

namespace Ekona.Images
{
    public abstract class SpriteBase
    {
        #region Variables
        protected IPluginHost pluginHost;
        protected string fileName;
        protected int id;
        bool loaded;
        bool canEdit;

        Bank[] banks;
        uint block_size;
        int zoom;

        Object obj;
        #endregion

        #region Properties
        public String FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public int ID
        {
            get { return id; }
        }
        public bool Loaded
        {
            get { return loaded; }
        }
        public bool CanEdit
        {
            get { return canEdit; }
        }

        public Bank[] Banks
        {
            get { return banks; }
            set { banks = value; }
        }
        public int NumBanks
        {
            get { return banks.Length; }
        }
        public uint BlockSize
        {
            get { return block_size; }
        }
        #endregion

        public SpriteBase()
        {
        }
        public SpriteBase(string file, int id, string fileName = "")
        {
            if (fileName == "")
                this.fileName = Path.GetFileName(file);
            else
                this.fileName = fileName;
            this.id = id;

            Read(file);
        }
        public SpriteBase(string file, int id,  IPluginHost pluginHost, string fileName = "")
        {
            this.pluginHost = pluginHost;
            if (fileName == "")
                this.fileName = Path.GetFileName(file);
            else
                this.fileName = fileName;
            this.id = id;

            Read(file);
        }


        public abstract void Read(string fileIn);
        public abstract void Write(string fileOut, ImageBase image, PaletteBase palette);

        public void Set_Banks(Bank[] banks, uint block_size, bool editable)
        {
            this.banks = banks;
            this.block_size = block_size;
            this.canEdit = editable;
            loaded = true;

            // Sort the cell using the priority value
            for (int b = 0; b < banks.Length; b++)
            {
                List<OAM> cells = new List<OAM>();
                cells.AddRange(banks[b].oams);
                cells.Sort(Actions.Comparision_OAM);
                banks[b].oams = cells.ToArray();
            }
        }

        public Image Get_Image(ImageBase image, PaletteBase pal, int index, int width, int height,
                               bool grid, bool cell, bool number, bool trans, bool img)
        {
            return Actions.Get_Image(banks[index], block_size, image, pal, width, height,
                                     grid, cell, number, trans, img);
        }
        public Image Get_Image(ImageBase image, PaletteBase pal, Bank bank, int width, int height,
                       bool grid, bool cell, bool number, bool trans, bool img)
        {
            return Actions.Get_Image(bank, block_size, image, pal, width, height,
                                     grid, cell, number, trans, img);
        }
        public Image Get_Image(ImageBase image, PaletteBase pal, Bank bank, int width, int height,
               bool grid, bool cell, bool number, bool trans, bool img, int currOAM)
        {
            return Actions.Get_Image(bank, block_size, image, pal, width, height,
                                     grid, cell, number, trans, img, currOAM);
        }
        public Image Get_Image(ImageBase image, PaletteBase pal, int index, int width, int height,
               bool grid, bool cell, bool number, bool trans, bool img, int currOAM)
        {
            return Actions.Get_Image(banks[index], block_size, image, pal, width, height,
                                     grid, cell, number, trans, img, currOAM);
        }
        public Image Get_Image(ImageBase image, PaletteBase pal, int index, int width, int height,
               bool grid, bool cell, bool number, bool trans, bool img, int currOAM, int[] draw_index)
        {
            return Actions.Get_Image(banks[index], block_size, image, pal, width, height,
                                     grid, cell, number, trans, img, currOAM, 1, draw_index);
        }

    }

    public struct Bank
    {
        public OAM[] oams;
        public string name;

        public ushort height;
        public ushort width;

        public uint data_offset;
        public uint data_size;
    }
    public struct OAM
    {
        public Obj0 obj0;
        public Obj1 obj1;
        public Obj2 obj2;

        public ushort width;
        public ushort height;
        public ushort num_cell;
    }

    public struct Obj0  // 16 bits
    {
        public Int32 yOffset;       // Bit0-7 -> signed
        public byte rs_flag;        // Bit8 -> Rotation / Scale flag
        public byte objDisable;     // Bit9 -> if r/s == 0
        public byte doubleSize;     // Bit9 -> if r/s != 0
        public byte objMode;        // Bit10-11 -> 0 = normal; 1 = semi-trans; 2 = window; 3 = invalid
        public byte mosaic_flag;    // Bit12 
        public byte depth;          // Bit13 -> 0 = 4bit; 1 = 8bit
        public byte shape;          // Bit14-15 -> 0 = square; 1 = horizontal; 2 = vertial; 3 = invalid
    }
    public struct Obj1  // 16 bits
    {
        public Int32 xOffset;   // Bit0-8 (unsigned)

        // If R/S == 0
        public byte unused; // Bit9-11
        public byte flipX;  // Bit12
        public byte flipY;  // Bit13
        // If R/S != 0
        public byte select_param;   //Bit9-13 -> Parameter selection

        public byte size;   // Bit14-15
    }
    public struct Obj2  // 16 bits
    {
        public uint tileOffset;     // Bit0-9
        public byte priority;       // Bit10-11
        public byte index_palette;  // Bit12-15
    }
}
