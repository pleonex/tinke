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

namespace Tinke
{
    public static class Imagen_NCER
    {
        public static NCER Read(string file, int id)
        {
            System.Xml.Linq.XElement xml = Tools.Helper.GetTranslation("NCER");
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            NCER ncer = new NCER();
            ncer.id = (uint)id;
            Console.WriteLine("NCER {0}<pre>", Path.GetFileName(file));

            // Lee la cabecera genérica
            ncer.header.id = br.ReadChars(4);
            ncer.header.endianess = br.ReadUInt16();
            if (ncer.header.endianess == 0xFFFE)
                ncer.header.id.Reverse<char>();
            ncer.header.constant = br.ReadUInt16();
            ncer.header.file_size = br.ReadUInt32();
            ncer.header.header_size = br.ReadUInt16();
            ncer.header.nSection = br.ReadUInt16();

            // Lee la sección básica CEBK (CEll BanK)
            ncer.cebk.id = br.ReadChars(4);
            ncer.cebk.section_size = br.ReadUInt32();
            ncer.cebk.nBanks = br.ReadUInt16();
            ncer.cebk.tBank = br.ReadUInt16();
            ncer.cebk.constant = br.ReadUInt32();
            ncer.cebk.block_size = br.ReadUInt32() & 0xFF;
            ncer.cebk.unknown1 = br.ReadUInt32();
            ncer.cebk.unknown2 = br.ReadUInt64();
            ncer.cebk.banks = new Bank[ncer.cebk.nBanks];

            Console.WriteLine(xml.Element("S0B").Value + ": 0x{0:X}", ncer.cebk.block_size);
            Console.WriteLine(xml.Element("S0C").Value + ": 0x{0:X}", ncer.cebk.unknown1);
            Console.WriteLine(xml.Element("S09").Value + ": {0}", ncer.cebk.tBank.ToString());
            Console.WriteLine(xml.Element("S08").Value + ": {0}", ncer.cebk.nBanks.ToString());

            uint tilePos = 0x00; // If unknown1 != 0x00

            #region Read banks
            for (int i = 0; i < ncer.cebk.nBanks; i++)
            {
                ncer.cebk.banks[i].nCells = br.ReadUInt16();
                ncer.cebk.banks[i].unknown1 = br.ReadUInt16();
                ncer.cebk.banks[i].cell_offset = br.ReadUInt32();

                if (ncer.cebk.tBank == 0x01)
                {
                    ncer.cebk.banks[i].xMax = br.ReadInt16();
                    ncer.cebk.banks[i].yMax = br.ReadInt16();
                    ncer.cebk.banks[i].xMin = br.ReadInt16();
                    ncer.cebk.banks[i].yMin = br.ReadInt16();
                }

                long posicion = br.BaseStream.Position;
                if (ncer.cebk.tBank == 0x00)
                    br.BaseStream.Position += (ncer.cebk.nBanks - (i + 1)) * 8 + ncer.cebk.banks[i].cell_offset;
                else
                    br.BaseStream.Position += (ncer.cebk.nBanks - (i + 1)) * 0x10 + ncer.cebk.banks[i].cell_offset;

                Console.WriteLine("<br>--------------");
                Console.WriteLine(xml.Element("S01").Value + " {0}:", i.ToString());
                Console.WriteLine("|_" + xml.Element("S19").Value + ": {0}", ncer.cebk.banks[i].nCells.ToString());
                Console.WriteLine("|_" + xml.Element("S1A").Value + ": {0}", ncer.cebk.banks[i].unknown1.ToString());
                Console.WriteLine("|_" + xml.Element("S1B").Value + ": {0}", ncer.cebk.banks[i].cell_offset.ToString());

                ncer.cebk.banks[i].cells = new Cell[ncer.cebk.banks[i].nCells];
                #region Read cells
                for (int j = 0; j < ncer.cebk.banks[i].nCells; j++)
                {
                    ncer.cebk.banks[i].cells[j].num_cell = (ushort)j;

                    ushort obj0 = br.ReadUInt16();
                    ushort obj1 = br.ReadUInt16();
                    ushort obj2 = br.ReadUInt16();

                    // Obj 0
                    ncer.cebk.banks[i].cells[j].obj0.yOffset = (sbyte)(obj0 & 0xFF);
                    ncer.cebk.banks[i].cells[j].obj0.rs_flag = (byte)((obj0 >> 8) & 1);
                    if (ncer.cebk.banks[i].cells[j].obj0.rs_flag == 0)
                        ncer.cebk.banks[i].cells[j].obj0.objDisable = (byte)((obj0 >> 9) & 1);
                    else
                        ncer.cebk.banks[i].cells[j].obj0.doubleSize = (byte)((obj0 >> 9) & 1);
                    ncer.cebk.banks[i].cells[j].obj0.objMode = (byte)((obj0 >> 10) & 3);
                    ncer.cebk.banks[i].cells[j].obj0.mosaic_flag = (byte)((obj0 >> 12) & 1);
                    ncer.cebk.banks[i].cells[j].obj0.depth = (byte)((obj0 >> 13) & 1);
                    ncer.cebk.banks[i].cells[j].obj0.shape = (byte)((obj0 >> 14) & 3);

                    // Obj 1
                    ncer.cebk.banks[i].cells[j].obj1.xOffset = obj1 & 0x01FF;
                    if (ncer.cebk.banks[i].cells[j].obj1.xOffset >= 0x100)
                        ncer.cebk.banks[i].cells[j].obj1.xOffset -= 0x200;
                    if (ncer.cebk.banks[i].cells[j].obj0.rs_flag == 0)
                    {
                        ncer.cebk.banks[i].cells[j].obj1.unused = (byte)((obj1 >> 9) & 7);
                        ncer.cebk.banks[i].cells[j].obj1.flipX = (byte)((obj1 >> 12) & 1);
                        ncer.cebk.banks[i].cells[j].obj1.flipY = (byte)((obj1 >> 13) & 1);
                    }
                    else
                        ncer.cebk.banks[i].cells[j].obj1.select_param = (byte)((obj1 >> 9) & 0x1F);
                    ncer.cebk.banks[i].cells[j].obj1.size = (byte)((obj1 >> 14) & 3);

                    // Obj 2
                    ncer.cebk.banks[i].cells[j].obj2.tileOffset = (uint)(obj2 & 0x03FF);
                    if (ncer.cebk.unknown1 != 0x00)
                        ncer.cebk.banks[i].cells[j].obj2.tileOffset += tilePos;
                    ncer.cebk.banks[i].cells[j].obj2.priority = (byte)((obj2 >> 10) & 3);
                    ncer.cebk.banks[i].cells[j].obj2.index_palette = (byte)((obj2 >> 12) & 0xF);

                    // Calculate the size
                    Size cellSize = Calculate_Size(ncer.cebk.banks[i].cells[j].obj0.shape, ncer.cebk.banks[i].cells[j].obj1.size);
                    ncer.cebk.banks[i].cells[j].height = (ushort)cellSize.Height;
                    ncer.cebk.banks[i].cells[j].width = (ushort)cellSize.Width;
                    if (ncer.cebk.banks[i].cells[j].obj0.doubleSize == 1)
                    {
                        ncer.cebk.banks[i].cells[j].width *= 2;
                        ncer.cebk.banks[i].cells[j].height *= 2;
                    }

                    Console.WriteLine("|_" + xml.Element("S1C").Value + " {0}:", j.ToString());
                    Console.WriteLine("    " + xml.Element("S1D").Value + ": {0}", ncer.cebk.banks[i].cells[j].obj0.yOffset.ToString());
                    Console.WriteLine("    " + xml.Element("S1E").Value + ": {0}", ncer.cebk.banks[i].cells[j].obj1.xOffset.ToString());
                    Console.WriteLine("    " + xml.Element("S1F").Value + ": {0}", ncer.cebk.banks[i].cells[j].width.ToString());
                    Console.WriteLine("    " + xml.Element("S20").Value + ": {0}", ncer.cebk.banks[i].cells[j].height.ToString());
                    Console.WriteLine("    " + xml.Element("S21").Value + ": {0}", ncer.cebk.banks[i].cells[j].obj2.index_palette.ToString());
                    Console.WriteLine("    " + xml.Element("S22").Value + ": {0}", (obj2 & 0x03FF).ToString());
                    Console.WriteLine("    " + xml.Element("S23").Value + ": {0}", ncer.cebk.banks[i].cells[j].obj2.tileOffset.ToString());
                    Console.WriteLine("    " + "Object priority" + ": {0}", ncer.cebk.banks[i].cells[j].obj2.priority.ToString());
                }
                #endregion

                // Sort the cell using the priority value
                List<Cell> cells = new List<Cell>();
                cells.AddRange(ncer.cebk.banks[i].cells);
                cells.Sort(Comparision_Cell);
                ncer.cebk.banks[i].cells = cells.ToArray();

                // Calculate the next tileOffset if unknonw1 != 0
                if (ncer.cebk.unknown1 != 0x00 && ncer.cebk.banks[i].nCells != 0x00)
                {
                    Cell ultimaCelda = Get_LastCell(ncer.cebk.banks[i]);

                    int ultimaCeldaSize = (int)(ultimaCelda.height * ultimaCelda.width);
                    ultimaCeldaSize /= (int)(64 << (byte)ncer.cebk.block_size);
                    if (ultimaCelda.obj0.depth == 1)
                        ultimaCeldaSize *= 2;
                    if (ultimaCeldaSize == 0)
                        ultimaCeldaSize = 1;

                    tilePos += (uint)((ultimaCelda.obj2.tileOffset - tilePos) + ultimaCeldaSize);

                    //if (ncer.cebk.unknown1 == 0x160 && i == 5) // I don't know why it works
                    //    tilePos -= 3;
                    //if (ncer.cebk.unknown1 == 0x110 && i == 4) // (ncer.cebk.unknown1 & FC0) >> 6 (maybe ?)
                    //    tilePos -= 7;
                }
                br.BaseStream.Position = posicion;
                Console.WriteLine("--------------");
            }
            #endregion

            #region Sección LABL
            // Lee la segunda LABL
            br.BaseStream.Position = ncer.header.header_size + ncer.cebk.section_size;
            List<uint> offsets = new List<uint>();
            List<String> names = new List<string>();
            ncer.labl.names = new string[ncer.cebk.nBanks];

            ncer.labl.id = br.ReadChars(4);
            if (new String(ncer.labl.id) != "LBAL")
                goto Tercera;
            ncer.labl.section_size = br.ReadUInt32();

            // Primero se encuentran los offset a los nombres.
            for (int i = 0; i < ncer.cebk.nBanks; i++)
            {
                uint offset = br.ReadUInt32();
                if (offset >= ncer.labl.section_size - 8)
                {
                    br.BaseStream.Position -= 4;
                    break;
                }

                offsets.Add(offset);
            }
            ncer.labl.offset = offsets.ToArray();
            // Ahora leemos los nombres
            for (int i = 0; i < ncer.labl.offset.Length; i++)
            {
                names.Add("");
                byte c = br.ReadByte();
                while (c != 0x00)
                {
                    names[i] += (char)c;
                    c = br.ReadByte();
                }
            }
        Tercera:
            for (int i = 0; i < ncer.cebk.nBanks; i++)
                if (names.Count > i)
                    ncer.labl.names[i] = names[i];
                else
                    ncer.labl.names[i] = i.ToString();
            #endregion
            #region Sección UEXT
            // Lee la tercera sección UEXT
            ncer.uext.id = br.ReadChars(4);
            if (new String(ncer.uext.id) != "TXEU")
                goto Fin;

            ncer.uext.section_size = br.ReadUInt32();
            ncer.uext.unknown = br.ReadUInt32();
            #endregion

        Fin:
            br.Close();
            Console.WriteLine("</pre>EOF");
            return ncer;
        }
        public static Size Calculate_Size(byte shape, byte size)
        {
            Size imageSize = new Size();

            switch (shape)
            {
                case 0x00:  // Square
                    switch (size)
                    {
                        case 0x00:
                            imageSize = new Size(8, 8);
                            break;
                        case 0x01:
                            imageSize = new Size(16, 16);
                            break;
                        case 0x02:
                            imageSize = new Size(32, 32);
                            break;
                        case 0x03:
                            imageSize = new Size(64, 64);
                            break;
                    }
                    break;
                case 0x01:  // Horizontal
                    switch (size)
                    {
                        case 0x00:
                            imageSize = new Size(16, 8);
                            break;
                        case 0x01:
                            imageSize = new Size(32, 8);
                            break;
                        case 0x02:
                            imageSize = new Size(32, 16);
                            break;
                        case 0x03:
                            imageSize = new Size(64, 32);
                            break;
                    }
                    break;
                case 0x02:  // Vertical
                    switch (size)
                    {
                        case 0x00:
                            imageSize = new Size(8, 16);
                            break;
                        case 0x01:
                            imageSize = new Size(8, 32);
                            break;
                        case 0x02:
                            imageSize = new Size(16, 32);
                            break;
                        case 0x03:
                            imageSize = new Size(32, 64);
                            break;
                    }
                    break;
            }

            return imageSize;
        }

        public static Bitmap Get_Image(Bank banco, uint blockSize, NCGR tile, NCLR paleta,
            bool entorno, bool celda, bool numero, bool transparencia, bool image, int zoom = 1)
        {
            if (banco.cells.Length == 0)
                return new Bitmap(1, 1);
            Size tamaño = new Size(256 * zoom, 256 * zoom);
            Bitmap imagen = new Bitmap(tamaño.Width, tamaño.Height);
            Graphics grafico = Graphics.FromImage(imagen);

            // Entorno
            if (entorno)
            {
                for (int i = (0 - tamaño.Width); i < tamaño.Width; i += 8)
                {
                    grafico.DrawLine(Pens.LightBlue, (i + tamaño.Width / 2) * zoom, 0, (i + tamaño.Width / 2) * zoom, tamaño.Height * zoom);
                    grafico.DrawLine(Pens.LightBlue, 0, (i + tamaño.Height / 2) * zoom, tamaño.Width * zoom, (i + tamaño.Height / 2) * zoom);
                }
                grafico.DrawLine(Pens.Blue, 128 * zoom, 0, 128 * zoom, 256 * zoom);
                grafico.DrawLine(Pens.Blue, 0, 128 * zoom, 256 * zoom, 128 * zoom);
            }


            Image[] celdas = new Image[banco.nCells];
            for (int i = 0; i < banco.nCells; i++)
            {
                if (banco.cells[i].width == 0x00 || banco.cells[i].height == 0x00)
                    continue;

                uint tileOffset = banco.cells[i].obj2.tileOffset;
                if (blockSize > 4)
                    blockSize = 4;
                if (tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                    tileOffset = (uint)(tileOffset << (byte)blockSize);
                else
                    tileOffset = (uint)(tileOffset << (byte)blockSize) / 2;

                if (image)
                {
                    if (tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                        for (int j = 0; j < tile.rahc.tileData.nPalette.Length; j++)
                            tile.rahc.tileData.nPalette[j] = banco.cells[i].obj2.index_palette;

                    if (blockSize < 4)
                    {
                        if (tile.order == TileOrder.NoTiled)
                            celdas[i] = Imagen_NCGR.Get_Image(tile, paleta, (int)tileOffset * 64, banco.cells[i].width, banco.cells[i].height, zoom);
                        else
                            celdas[i] = Imagen_NCGR.Get_Image(tile, paleta, (int)tileOffset * 64, banco.cells[i].width / 8, banco.cells[i].height / 8, zoom);
                    }
                    else
                    {
                        tileOffset /= (blockSize / 2);
                        int imageWidth = tile.rahc.nTilesX;
                        int imageHeight = tile.rahc.nTilesY;
                        if (tile.order == TileOrder.Horizontal)
                        {
                            imageWidth *= 8;
                            imageHeight *= 8;
                        }

                        int posX = (int)(tileOffset % imageWidth);
                        int posY = (int)(tileOffset / imageWidth);

                        if (tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                            posY *= (int)blockSize * 2;
                        else
                            posY *= (int)blockSize;
                        if (posY >= imageHeight)
                            posY = posY % imageHeight;

                        celdas[i] = Imagen_NCGR.Get_Image(tile, paleta, zoom).Clone(new Rectangle(posX * zoom, posY * zoom, banco.cells[i].width * zoom, banco.cells[i].height * zoom), System.Drawing.Imaging.PixelFormat.DontCare);
                    }

                    #region Flip
                    if (banco.cells[i].obj1.flipX == 1 && banco.cells[i].obj1.flipY == 1)
                        celdas[i].RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    else if (banco.cells[i].obj1.flipX == 1)
                        celdas[i].RotateFlip(RotateFlipType.RotateNoneFlipX);
                    else if (banco.cells[i].obj1.flipY == 1)
                        celdas[i].RotateFlip(RotateFlipType.RotateNoneFlipY);
                    #endregion

                    if (transparencia)
                        ((Bitmap)celdas[i]).MakeTransparent(paleta.pltt.palettes[tile.rahc.tileData.nPalette[0]].colors[0]);

                    grafico.DrawImageUnscaled(celdas[i], tamaño.Width / 2 + banco.cells[i].obj1.xOffset * zoom, tamaño.Height / 2 + banco.cells[i].obj0.yOffset * zoom);
                }

                if (celda)
                    grafico.DrawRectangle(Pens.Black, tamaño.Width / 2 + banco.cells[i].obj1.xOffset * zoom, tamaño.Height / 2 + banco.cells[i].obj0.yOffset * zoom,
                        banco.cells[i].width * zoom, banco.cells[i].height * zoom);
                if (numero)
                    grafico.DrawString(banco.cells[i].num_cell.ToString(), SystemFonts.CaptionFont, Brushes.Black, tamaño.Width / 2 + banco.cells[i].obj1.xOffset * zoom,
                        tamaño.Height / 2 + banco.cells[i].obj0.yOffset * zoom);
            }
            return imagen;
        }
        public static Bitmap Get_Image(Bank banco, uint blockSize, NCGR tile, NCLR paleta,
            bool entorno, bool celda, bool numero, bool transparencia, bool image, int maxWidth, int maxHeight, int zoom = 1)
        {
            if (banco.cells.Length == 0)
                return new Bitmap(1, 1);
            Size tamaño = new Size(maxWidth * zoom, maxHeight * zoom);
            Bitmap imagen = new Bitmap(tamaño.Width, tamaño.Height);
            Graphics grafico = Graphics.FromImage(imagen);

            // Entorno
            if (entorno)
            {
                for (int i = (0 - tamaño.Width); i < tamaño.Width; i += 8)
                {
                    grafico.DrawLine(Pens.LightBlue, (i + tamaño.Width / 2) * zoom, 0, (i + tamaño.Width / 2) * zoom, tamaño.Height * zoom);
                    grafico.DrawLine(Pens.LightBlue, 0, (i + tamaño.Height / 2) * zoom, tamaño.Width * zoom, (i + tamaño.Height / 2) * zoom);
                }
                grafico.DrawLine(Pens.Blue, (maxWidth / 2) * zoom, 0, (maxWidth / 2) * zoom, maxHeight * zoom);
                grafico.DrawLine(Pens.Blue, 0, (maxHeight / 2) * zoom, maxWidth * zoom, (maxHeight / 2) * zoom);
            }


            Image[] celdas = new Image[banco.nCells];
            for (int i = 0; i < banco.nCells; i++)
            {
                if (banco.cells[i].width == 0x00 || banco.cells[i].height == 0x00)
                    continue;

                uint tileOffset = banco.cells[i].obj2.tileOffset;
                if (blockSize > 4)
                    blockSize = 4;
                if (tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                    tileOffset = (uint)(tileOffset << (byte)blockSize);
                else
                    tileOffset = (uint)(tileOffset << (byte)blockSize) / 2;

                if (image)
                {
                    if (tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                        for (int j = 0; j < tile.rahc.tileData.nPalette.Length; j++)
                            tile.rahc.tileData.nPalette[j] = banco.cells[i].obj2.index_palette;

                    if (blockSize < 4)
                    {
                        if (tile.order == TileOrder.NoTiled)
                            celdas[i] = Imagen_NCGR.Get_Image(tile, paleta, (int)tileOffset * 64, banco.cells[i].width, banco.cells[i].height, zoom);
                        else
                            celdas[i] = Imagen_NCGR.Get_Image(tile, paleta, (int)tileOffset * 64, banco.cells[i].width / 8, banco.cells[i].height / 8, zoom);
                    }
                    else
                    {
                        tileOffset /= (blockSize / 2);
                        int imageWidth = tile.rahc.nTilesX;
                        int imageHeight = tile.rahc.nTilesY;
                        if (tile.order == TileOrder.Horizontal)
                        {
                            imageWidth *= 8;
                            imageHeight *= 8;
                        }

                        int posX = (int)(tileOffset % imageWidth);
                        int posY = (int)(tileOffset / imageWidth);

                        if (tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                            posY *= (int)blockSize * 2;
                        else
                            posY *= (int)blockSize;
                        if (posY >= imageHeight)
                            posY = posY % imageHeight;

                        celdas[i] = Imagen_NCGR.Get_Image(tile, paleta, zoom).Clone(new Rectangle(posX * zoom, posY * zoom, banco.cells[i].width * zoom, banco.cells[i].height * zoom), System.Drawing.Imaging.PixelFormat.DontCare);
                    }

                    #region Flip
                    if (banco.cells[i].obj1.flipX == 1 && banco.cells[i].obj1.flipY == 1)
                        celdas[i].RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    else if (banco.cells[i].obj1.flipX == 1)
                        celdas[i].RotateFlip(RotateFlipType.RotateNoneFlipX);
                    else if (banco.cells[i].obj1.flipY == 1)
                        celdas[i].RotateFlip(RotateFlipType.RotateNoneFlipY);
                    #endregion

                    if (transparencia)
                        ((Bitmap)celdas[i]).MakeTransparent(paleta.pltt.palettes[tile.rahc.tileData.nPalette[0]].colors[0]);

                    grafico.DrawImageUnscaled(celdas[i], tamaño.Width / 2 + banco.cells[i].obj1.xOffset * zoom, tamaño.Height / 2 + banco.cells[i].obj0.yOffset * zoom);
                }

                if (celda)
                    grafico.DrawRectangle(Pens.Black, tamaño.Width / 2 + banco.cells[i].obj1.xOffset * zoom, tamaño.Height / 2 + banco.cells[i].obj0.yOffset * zoom,
                        banco.cells[i].width * zoom, banco.cells[i].height * zoom);
                if (numero)
                    grafico.DrawString(i.ToString(), SystemFonts.CaptionFont, Brushes.Black, tamaño.Width / 2 + banco.cells[i].obj1.xOffset * zoom,
                        tamaño.Height / 2 + banco.cells[i].obj0.yOffset * zoom);
            }

            return imagen;
        }

        private static int Comparision_Cell(Cell c1, Cell c2)
        {
            if (c1.obj2.priority < c2.obj2.priority)
                return 1;
            else if (c1.obj2.priority > c2.obj2.priority)
                return -1;
            else   // Same priority
            {
                if (c1.num_cell < c2.num_cell)
                    return 1;
                else if (c1.num_cell > c2.num_cell)
                    return -1;
                else // Same cell
                    return 0;
            }
        }
        private static Cell Get_LastCell(Bank bank)
        {
            for (int i = 0; i < bank.cells.Length; i++)
                if (bank.cells[i].num_cell == bank.cells.Length - 1)
                    return bank.cells[i];

            return new Cell();
        }

        public static Byte[][] Change_ImageCell(Cell cell, uint blockSize, NCGR newTiles, NCGR oldImage)
        {
            List<Byte[]> result = new List<byte[]>();
            List<Byte[]> newImage = new List<byte[]>();
            List<Byte> temp = new List<byte>();


            #region Get the tile data of the new Cell
            for (int ht = 0; ht < 512; ht++)
            {
                for (int wt = 0; wt < 512; wt++)
                {
                    if (ht >= 256 + cell.obj0.yOffset && ht < 256 + cell.obj0.yOffset + cell.height)
                    {
                        if (wt >= 256 + cell.obj1.xOffset && wt < 256 + cell.obj1.xOffset + cell.width)
                        {
                            // Get the tile data
                            temp.Add(
                            newTiles.rahc.tileData.tiles[0][wt + ht * 512]);
                        }
                    }
                }
            }
            if (oldImage.order == TileOrder.Horizontal)
                newImage.AddRange(Convertir.BytesToTiles_NoChanged(temp.ToArray(), cell.width / 0x08, cell.height / 0x08));
            else
                newImage.Add(temp.ToArray());
            temp.Clear();
            #endregion

            if (oldImage.order == TileOrder.Horizontal)
            {
                uint tileOffset = cell.obj2.tileOffset;
                if (blockSize > 4)
                    blockSize = 4;
                if (oldImage.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                    tileOffset = (uint)(tileOffset << (byte)blockSize);
                else
                    tileOffset = (uint)(tileOffset << (byte)blockSize) / 2;

                for (int i = 0; i < tileOffset; i++)
                    result.Add(oldImage.rahc.tileData.tiles[i]);

                result.AddRange(newImage);

                for (int i = (int)(tileOffset + (cell.width * cell.height) / 0x40); i < oldImage.rahc.tileData.tiles.Length; i++)
                    result.Add(oldImage.rahc.tileData.tiles[i]);
            }
            else if (oldImage.order == TileOrder.NoTiled)
            {
                uint tileOffset = cell.obj2.tileOffset;
                if (blockSize > 4)
                    blockSize = 4;
                if (oldImage.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                    tileOffset = (uint)(tileOffset << (byte)blockSize);
                else
                    tileOffset = (uint)(tileOffset << (byte)blockSize) / 2;

                for (int i = 0; i < tileOffset; i++)
                    temp.Add(oldImage.rahc.tileData.tiles[0][i]);

                temp.AddRange(newImage[0]);

                for (int i = (int)(tileOffset + cell.width * cell.height); i < oldImage.rahc.tileData.tiles[0].Length; i++)
                    temp.Add(oldImage.rahc.tileData.tiles[0][i]);

                result.Add(temp.ToArray());
            }

            return result.ToArray();
        }
        public static Byte[][] Change_ColorCell(Cell cell, uint blockSize, NCGR image, int oldIndex, int newIndex)
        {
            List<Byte[]> result = new List<byte[]>();
            List<Byte> temp = new List<byte>();

            if (image.order == TileOrder.Horizontal)
            {
                uint tileOffset = cell.obj2.tileOffset;
                if (blockSize > 4)
                    blockSize = 4;
                if (image.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                    tileOffset = (uint)(tileOffset << (byte)blockSize);
                else
                    tileOffset = (uint)(tileOffset << (byte)blockSize) / 2;

                for (int i = 0; i < image.rahc.tileData.tiles.Length; i++)
                {
                    if (i >= tileOffset && i < (int)(tileOffset + (cell.width * cell.height) / 0x40))
                    {
                        Byte[] tile = new Byte[64];
                        for (int j = 0; j < 64; j++)
                            if (image.rahc.tileData.tiles[i][j] == oldIndex)
                                tile[j] = (byte)newIndex;
                            else if (image.rahc.tileData.tiles[i][j] == newIndex)
                                tile[j] = (byte)oldIndex;
                            else
                                tile[j] = image.rahc.tileData.tiles[i][j];

                        result.Add(tile);
                    }
                    else
                        result.Add(image.rahc.tileData.tiles[i]);

                }
            }
            else if (image.order == TileOrder.NoTiled)
            {
                uint tileOffset = cell.obj2.tileOffset;
                if (blockSize > 4)
                    blockSize = 4;
                if (image.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                    tileOffset = (uint)(tileOffset << (byte)blockSize);
                else
                    tileOffset = (uint)(tileOffset << (byte)blockSize) / 2;

                for (int i = 0; i < image.rahc.tileData.tiles[0].Length; i++)
                {
                    if (i >= tileOffset && i < (int)(tileOffset + cell.width * cell.height))
                    {
                        if (image.rahc.tileData.tiles[0][i] == oldIndex)
                            temp.Add((byte)newIndex);
                        else if (image.rahc.tileData.tiles[0][i] == newIndex)
                            temp.Add((byte)oldIndex);
                        else
                            temp.Add(image.rahc.tileData.tiles[0][i]);
                    }
                    else
                        temp.Add(image.rahc.tileData.tiles[0][i]);

                }
                result.Add(temp.ToArray());
            }

            return result.ToArray();
        }

    }
}
