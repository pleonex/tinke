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
 * Programa utilizado: Microsoft Visual C# 2010 Express
 * Fecha: 18/02/2011
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
        public static NCER Leer(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            NCER ncer = new NCER();

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
            // Lee cada banco
            for (int i = 0; i < ncer.cebk.nBanks; i++)
            {
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
                // Lee la información de cada banco
                for (int j = 0; j < ncer.cebk.banks[i].nCells; j++)
                {
                    ncer.cebk.banks[i].cells[j].yOffset = br.ReadSByte();
                    byte byte1 = br.ReadByte();
                    int x = br.ReadUInt16() & 0x01FF;
                    ncer.cebk.banks[i].cells[j].xOffset = (x >= 0x100) ? x - 0x200 : x;
                    br.BaseStream.Position -= 1;
                    byte byte2 = br.ReadByte();
                    Size tamaño = Obtener_Tamaño(Tools.Helper.ByteTo4Bits(byte1)[0], Tools.Helper.ByteTo4Bits(byte2)[0]);
                    ncer.cebk.banks[i].cells[j].height = (ushort)tamaño.Height;
                    ncer.cebk.banks[i].cells[j].width = (ushort)tamaño.Width;
                    ncer.cebk.banks[i].cells[j].tileOffset = (uint)(br.ReadUInt16() & 0x03FF);
                    ncer.cebk.banks[i].cells[j].yFlip = (Tools.Helper.ByteTo4Bits(byte2)[0] & 2) == 2 ? true : false;
                    ncer.cebk.banks[i].cells[j].xFlip = (Tools.Helper.ByteTo4Bits(byte2)[0] & 1) == 1 ? true : false;
                }
                br.BaseStream.Position = posicion;
            }

            #region Sección LABL
            // Lee la segunda LABL
            br.BaseStream.Position = ncer.header.header_size + ncer.cebk.section_size;
            List<uint> offsets = new List<uint>();
            List<String> names = new List<string>();

            ncer.labl.id = br.ReadChars(4);
            if (new String(ncer.labl.id) != "LBAL")
                goto Tercera;
            ncer.labl.section_size = br.ReadUInt32();

            ncer.labl.names = new string[ncer.cebk.nBanks];
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
                uint tileOffset = banco.cells[i].tileOffset;
                if (tile.rahc.depth == System.Windows.Forms.ColorDepth.Depth4Bit)
                    tileOffset *= (uint)Math.Pow(blockSize, 2);
                else
                    tileOffset *= (uint)((blockSize != 0) ? blockSize : 1);

                if (image)
                {
                    if (tile.orden == Orden_Tiles.No_Tiles)
                        celdas[i] = Imagen_NCGR.Crear_Imagen(tile, paleta, (int)tileOffset * 64, banco.cells[i].width, banco.cells[i].height);
                    else
                        celdas[i] = Imagen_NCGR.Crear_Imagen(tile, paleta, (int)tileOffset, banco.cells[i].width / 8, banco.cells[i].height / 8);
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
                    tileOffset *= (uint)Math.Pow(blockSize, 2);
                else
                    tileOffset *= (uint)((blockSize != 0) ? blockSize : 1);

                if (image)
                {
                    if (tile.orden == Orden_Tiles.No_Tiles)
                        celdas[i] = Imagen_NCGR.Crear_Imagen(tile, paleta, (int)tileOffset * 64, banco.cells[i].width, banco.cells[i].height);
                    else
                        celdas[i] = Imagen_NCGR.Crear_Imagen(tile, paleta, (int)tileOffset, banco.cells[i].width / 8, banco.cells[i].height / 8);
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

            return imagen;
        }

    }
}
