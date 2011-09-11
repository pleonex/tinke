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
        public static NCER Leer(string file, int id)
        {
            System.Xml.Linq.XElement xml = Tools.Helper.ObtenerTraduccion("NCER");
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
            ncer.cebk.block_size = br.ReadUInt32();
            ncer.cebk.unknown1 = br.ReadUInt32();
            ncer.cebk.unknown2 = br.ReadUInt64();
            ncer.cebk.banks = new Bank[ncer.cebk.nBanks];

            Console.WriteLine(xml.Element("S0B").Value + ": 0x{0:X}", ncer.cebk.block_size);
            Console.WriteLine(xml.Element("S0C").Value + ": 0x{0:X}", ncer.cebk.unknown1);
            Console.WriteLine(xml.Element("S09").Value + ": {0}", ncer.cebk.tBank.ToString());
            Console.WriteLine(xml.Element("S08").Value + ": {0}", ncer.cebk.nBanks.ToString());
            uint tilePos = 0x00; // En caso de que Unknown 1 != 0x00
            // Lee cada banco
            for (int i = 0; i < ncer.cebk.nBanks; i++)
            {
                Console.WriteLine("<br>--------------");
                Console.WriteLine(xml.Element("S01").Value + " {0}:", i.ToString());

                ncer.cebk.banks[i].nCells = br.ReadUInt16();
                ncer.cebk.banks[i].unknown1 = br.ReadUInt16();
                ncer.cebk.banks[i].cell_offset = br.ReadUInt32();

                if (ncer.cebk.tBank == 0x01)
                    br.ReadBytes(8);    // Desconocido por el momento

                long posicion = br.BaseStream.Position;
                if (ncer.cebk.tBank == 0x00)
                    br.BaseStream.Position += (ncer.cebk.nBanks - (i + 1)) * 8 + ncer.cebk.banks[i].cell_offset;
                else
                    br.BaseStream.Position += (ncer.cebk.nBanks - (i + 1)) * 0x10 + ncer.cebk.banks[i].cell_offset;

                ncer.cebk.banks[i].cells = new Cell[ncer.cebk.banks[i].nCells];

                Console.WriteLine("|_" + xml.Element("S19").Value + ": {0}", ncer.cebk.banks[i].nCells.ToString());
                Console.WriteLine("|_" + xml.Element("S1A").Value + ": {0}", ncer.cebk.banks[i].unknown1.ToString());
                Console.WriteLine("|_" + xml.Element("S1B").Value + ": {0}", ncer.cebk.banks[i].cell_offset.ToString());
                // Lee la información de cada banco
                for (int j = 0; j < ncer.cebk.banks[i].nCells; j++)
                {
                    ncer.cebk.banks[i].cells[j].num_cell = (ushort)j;
                    ncer.cebk.banks[i].cells[j].yOffset = br.ReadSByte();
                    byte byte1 = br.ReadByte();
                    int x = br.ReadUInt16() & 0x01FF;
                    ncer.cebk.banks[i].cells[j].xOffset = (x >= 0x100) ? x - 0x200 : x;
                    br.BaseStream.Position -= 1;
                    byte byte2 = br.ReadByte();
                    Size tamaño = Obtener_Tamaño(Tools.Helper.ByteTo4Bits(byte1)[1], Tools.Helper.ByteTo4Bits(byte2)[1]);
                    ncer.cebk.banks[i].cells[j].height = (ushort)tamaño.Height;
                    ncer.cebk.banks[i].cells[j].width = (ushort)tamaño.Width;

                    ushort pos = br.ReadUInt16();
                    ncer.cebk.banks[i].cells[j].nPalette = (byte)((pos & 0xF000) >> 12);
                    ncer.cebk.banks[i].cells[j].priority = (byte)((pos & 0xC00) >> 10);
                    ncer.cebk.banks[i].cells[j].tileOffset = (uint)(pos & 0x03FF);
                    if (ncer.cebk.unknown1 != 0x00)
                        if ((ncer.cebk.unknown1 & 0x100) == 0x00)
                            ncer.cebk.banks[i].cells[j].tileOffset = (uint)(i + ncer.cebk.banks[i].cells[j].tileOffset);
                        else
                            ncer.cebk.banks[i].cells[j].tileOffset += tilePos;

                    ncer.cebk.banks[i].cells[j].yFlip = (Tools.Helper.ByteTo4Bits(byte2)[1] & 2) == 2 ? true : false;
                    ncer.cebk.banks[i].cells[j].xFlip = (Tools.Helper.ByteTo4Bits(byte2)[1] & 1) == 1 ? true : false;

                    Console.WriteLine("|_" + xml.Element("S1C").Value + " {0}:", j.ToString());
                    Console.WriteLine("    " + xml.Element("S1D").Value + ": {0}", ncer.cebk.banks[i].cells[j].yOffset.ToString());
                    Console.WriteLine("    " + xml.Element("S1E").Value + ": {0}", ncer.cebk.banks[i].cells[j].xOffset.ToString());
                    Console.WriteLine("    " + xml.Element("S1F").Value + ": {0}", ncer.cebk.banks[i].cells[j].width.ToString());
                    Console.WriteLine("    " + xml.Element("S20").Value + ": {0}", ncer.cebk.banks[i].cells[j].height.ToString());
                    Console.WriteLine("    " + xml.Element("S21").Value + ": {0}", ncer.cebk.banks[i].cells[j].nPalette.ToString());
                    Console.WriteLine("    " + xml.Element("S22").Value + ": {0}", (pos & 0x03FF).ToString());
                    Console.WriteLine("    " + xml.Element("S23").Value + ": {0}", ncer.cebk.banks[i].cells[j].tileOffset.ToString());
                }
                // Sort the cell using the priority value
                List<Cell> cells = new List<Cell>();
                cells.AddRange(ncer.cebk.banks[i].cells);
                cells.Sort(Comparision_Cell);
                ncer.cebk.banks[i].cells = cells.ToArray();

                if (ncer.cebk.unknown1 != 0x00)
                {
                    Cell ultimaCelda = Get_LastCell(ncer.cebk.banks[i]);
                    int ultimaCeldaSize = (int)(ultimaCelda.height * ultimaCelda.width / (128 * ncer.cebk.block_size));
                    if (ultimaCeldaSize == 0)
                        ultimaCeldaSize = 1;
                    tilePos += (uint)((ultimaCelda.tileOffset - tilePos) + ultimaCeldaSize);
                    if (ncer.cebk.unknown1 == 0x160 && i == 5) // Ni idea porqué pero funciona :)
                        tilePos -= 3;
                    if (ncer.cebk.unknown1 == 0x110 && i == 4) // (ncer.cebk.unknown1 & FC0) >> 6
                        tilePos -= 7;
                }
                br.BaseStream.Position = posicion;
                Console.WriteLine("--------------");
            }

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
        public static Size Obtener_Tamaño(byte byte1, byte byte2)
        {
            byte1 = Convert.ToByte(byte1 & 0x0C);
            byte2 = Convert.ToByte(byte2 & 0x0C);
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

        public static Bitmap Obtener_Imagen(Bank banco, uint blockSize, NCGR tile, NCLR paleta,
            bool entorno, bool celda, bool numero, bool transparencia, bool image)
        {
            if (banco.cells.Length == 0)
                return new Bitmap(1, 1);
            Size tamaño = new Size(256, 256);
            Bitmap imagen = new Bitmap(tamaño.Width, tamaño.Height);
            Graphics grafico = Graphics.FromImage(imagen);

            // Entorno
            if (entorno)
            {
                for (int i = (0 - tamaño.Width); i < tamaño.Width; i += 8)
                {
                    grafico.DrawLine(Pens.LightBlue, i + tamaño.Width / 2, 0, i + tamaño.Width / 2, tamaño.Height);
                    grafico.DrawLine(Pens.LightBlue, 0, i + tamaño.Height / 2, tamaño.Width, i + tamaño.Height / 2);
                }
                grafico.DrawLine(Pens.Blue, 128, 0, 128, 256);
                grafico.DrawLine(Pens.Blue, 0, 128, 256, 128);
            }


            Image[] celdas = new Image[banco.nCells];
            for (int i = 0; i < banco.nCells; i++)
            {
                if (banco.cells[i].width == 0x00 || banco.cells[i].height == 0x00)
                    continue;

                uint tileOffset = banco.cells[i].tileOffset;
                if (tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                {
                    tileOffset *= (uint)((blockSize != 0) ? blockSize * 2 : 1);
                }
                else
                {
                    tileOffset *= (uint)((blockSize != 0) ? blockSize : 1);
                }

                if (image)
                {
                    for (int j = 0; j < tile.rahc.tileData.nPaleta.Length; j++)
                        tile.rahc.tileData.nPaleta[j] = banco.cells[i].nPalette;

                    if (blockSize != 4)
                    {
                        if (tile.orden == Orden_Tiles.No_Tiles)
                            celdas[i] = Imagen_NCGR.Crear_Imagen(tile, paleta, (int)tileOffset * 64, banco.cells[i].width, banco.cells[i].height);
                        else
                            celdas[i] = Imagen_NCGR.Crear_Imagen(tile, paleta, (int)tileOffset * 64, banco.cells[i].width / 8, banco.cells[i].height / 8);
                    }
                    else
                    {
                        //celdas[i] = Imagen_NCGR.Crear_Imagen(tile, paleta).Clone(new RectangleF(
                        grafico.DrawString("No supported NCER file, block size == 0x04", SystemFonts.CaptionFont, Brushes.Black, tamaño.Width / 2, tamaño.Height / 2);
                        goto End;
                    }
                    #region Rotaciones
                    if (banco.cells[i].xFlip && banco.cells[i].yFlip)
                        celdas[i].RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    else if (banco.cells[i].xFlip)
                        celdas[i].RotateFlip(RotateFlipType.RotateNoneFlipX);
                    else if (banco.cells[i].yFlip)
                        celdas[i].RotateFlip(RotateFlipType.RotateNoneFlipY);
                    #endregion
                    if (transparencia)
                        ((Bitmap)celdas[i]).MakeTransparent(paleta.pltt.paletas[tile.rahc.tileData.nPaleta[0]].colores[0]);

                    grafico.DrawImageUnscaled(celdas[i], tamaño.Width / 2 + banco.cells[i].xOffset, tamaño.Height / 2 + banco.cells[i].yOffset);
                }

                if (celda)
                    grafico.DrawRectangle(Pens.Black, tamaño.Width / 2 + banco.cells[i].xOffset, tamaño.Height / 2 + banco.cells[i].yOffset,
                        banco.cells[i].width, banco.cells[i].height);
                if (numero)
                    grafico.DrawString(banco.cells[i].num_cell.ToString(), SystemFonts.CaptionFont, Brushes.Black, tamaño.Width / 2 + banco.cells[i].xOffset,
                        tamaño.Height / 2 + banco.cells[i].yOffset);
            }
        End:
            return imagen;
        }
        public static Bitmap Obtener_Imagen(Bank banco, uint blockSize, NCGR tile, NCLR paleta,
            bool entorno, bool celda, bool numero, bool transparencia, bool image, int maxWidth, int maxHeight)
        {
            if (banco.cells.Length == 0)
                return new Bitmap(1, 1);
            Size tamaño = new Size(maxWidth, maxHeight);
            Bitmap imagen = new Bitmap(tamaño.Width, tamaño.Height);
            Graphics grafico = Graphics.FromImage(imagen);

            // Entorno
            if (entorno)
            {
                for (int i = (0 - tamaño.Width); i < tamaño.Width; i += 8)
                {
                    grafico.DrawLine(Pens.LightBlue, i + tamaño.Width / 2, 0, i + tamaño.Width / 2, tamaño.Height);
                    grafico.DrawLine(Pens.LightBlue, 0, i + tamaño.Height / 2, tamaño.Width, i + tamaño.Height / 2);
                }
                grafico.DrawLine(Pens.Blue, maxWidth / 2, 0, maxWidth / 2, maxHeight);
                grafico.DrawLine(Pens.Blue, 0, maxHeight / 2, maxWidth, maxHeight / 2);
            }


            Image[] celdas = new Image[banco.nCells];
            for (int i = 0; i < banco.nCells; i++)
            {
                uint tileOffset = banco.cells[i].tileOffset;
                if (tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                    tileOffset *= (uint)((blockSize != 0) ? blockSize * 2 : 1);
                else
                    tileOffset *= (uint)((blockSize != 0) ? blockSize : 1);

                if (image)
                {
                    for (int j = 0; j < tile.rahc.tileData.nPaleta.Length; j++)
                        tile.rahc.tileData.nPaleta[j] = banco.cells[i].nPalette;

                    if (blockSize != 0x04)
                    {
                        if (tile.orden == Orden_Tiles.No_Tiles)
                            celdas[i] = Imagen_NCGR.Crear_Imagen(tile, paleta, (int)tileOffset * 64, banco.cells[i].width, banco.cells[i].height);
                        else
                            celdas[i] = Imagen_NCGR.Crear_Imagen(tile, paleta, (int)tileOffset * 64, banco.cells[i].width / 8, banco.cells[i].height / 8);
                    }
                    else
                    {
                        grafico.DrawString("No supported NCER file, block size == 0x04", SystemFonts.CaptionFont, Brushes.Black, tamaño.Width / 2, tamaño.Height / 2);
                        goto End;
                    }
                    #region Rotaciones
                    if (banco.cells[i].xFlip && banco.cells[i].yFlip)
                        celdas[i].RotateFlip(RotateFlipType.RotateNoneFlipXY);
                    else if (banco.cells[i].xFlip)
                        celdas[i].RotateFlip(RotateFlipType.RotateNoneFlipX);
                    else if (banco.cells[i].yFlip)
                        celdas[i].RotateFlip(RotateFlipType.RotateNoneFlipY);
                    #endregion
                    if (transparencia)
                        ((Bitmap)celdas[i]).MakeTransparent(paleta.pltt.paletas[tile.rahc.tileData.nPaleta[0]].colores[0]);

                    grafico.DrawImageUnscaled(celdas[i], tamaño.Width / 2 + banco.cells[i].xOffset, tamaño.Height / 2 + banco.cells[i].yOffset);
                }

                if (celda)
                    grafico.DrawRectangle(Pens.Black, tamaño.Width / 2 + banco.cells[i].xOffset, tamaño.Height / 2 + banco.cells[i].yOffset,
                        banco.cells[i].width, banco.cells[i].height);
                if (numero)
                    grafico.DrawString(i.ToString(), SystemFonts.CaptionFont, Brushes.Black, tamaño.Width / 2 + banco.cells[i].xOffset,
                        tamaño.Height / 2 + banco.cells[i].yOffset);
            }
        End:
            return imagen;
        }

        private static int Comparision_Cell(Cell c1, Cell c2)
        {
            if (c1.priority < c2.priority)
                return -1;
            else if (c1.priority > c2.priority)
                return 1;
            else
                return 0;
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
                    if (ht >= 256 + cell.yOffset && ht < 256 + cell.yOffset + cell.height)
                    {
                        if (wt >= 256 + cell.xOffset && wt < 256 + cell.xOffset + cell.width)
                        {
                            // Get the tile data
                            temp.Add(
                            newTiles.rahc.tileData.tiles[0][wt + ht * 512]);
                        }
                    }
                }
            }
            if (oldImage.orden == Orden_Tiles.Horizontal)
                newImage.AddRange(Convertir.BytesToTiles_NoChanged(temp.ToArray(), cell.width / 0x08, cell.height / 0x08));
            else
                newImage.Add(temp.ToArray());
            temp.Clear();
            #endregion

            if (oldImage.orden == Orden_Tiles.Horizontal)
            {
                uint tileOffset = (oldImage.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit ? cell.tileOffset * 2 : cell.tileOffset);
                tileOffset *= (blockSize != 0x00 ? blockSize : 1);

                for (int i = 0; i < tileOffset; i++)
                    result.Add(oldImage.rahc.tileData.tiles[i]);

                result.AddRange(newImage);

                for (int i = (int)(tileOffset + (cell.width * cell.height) / 0x40); i < oldImage.rahc.tileData.tiles.Length; i++)
                    result.Add(oldImage.rahc.tileData.tiles[i]);
            }
            else if (oldImage.orden == Orden_Tiles.No_Tiles)
            {
                uint tileOffset = (oldImage.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit ? cell.tileOffset * 2 : cell.tileOffset) * 0x40;
                tileOffset *= (blockSize != 0x00 ? blockSize : 1);

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

            if (image.orden == Orden_Tiles.Horizontal)
            {
                uint tileOffset = (image.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit ? cell.tileOffset * 2 : cell.tileOffset);
                tileOffset *= (blockSize != 0x00 ? blockSize : 1);

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
            else if (image.orden == Orden_Tiles.No_Tiles)
            {
                uint tileOffset = (image.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit ? cell.tileOffset * 2 : cell.tileOffset) * 0x40;
                tileOffset *= (blockSize != 0x00 ? blockSize : 1);

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
