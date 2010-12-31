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
            ncer.cebk.unknown1 = br.ReadUInt32();               // Si es 1 (activo) el tileoffset hay que multiplicarlo por dos
            ncer.cebk.unknown2 = br.ReadUInt32();
            ncer.cebk.unknown3 = br.ReadUInt64();
            ncer.cebk.banks = new Bank[ncer.cebk.nBanks];
            // Lee cada banco
            for (int i = 0; i < ncer.cebk.nBanks; i++)
            {
                ncer.cebk.banks[i].nCells = br.ReadUInt16();
                ncer.cebk.banks[i].unknown1 = br.ReadUInt16();
                ncer.cebk.banks[i].cell_offset = br.ReadUInt32();
                long posicion = br.BaseStream.Position;

                br.BaseStream.Position += (ncer.cebk.nBanks - (i + 1)) * 8 + ncer.cebk.banks[i].cell_offset;
                ncer.cebk.banks[i].cells = new Cell[ncer.cebk.banks[i].nCells];
                // Lee la información de cada banco
                for (int j = 0; j < ncer.cebk.banks[i].nCells; j++)
                {
                    byte y = br.ReadByte();
                    ncer.cebk.banks[i].cells[j].yOffset = (y < 0x7F) ? y : y - 0x100;
                    byte byte1 = br.ReadByte();
                    byte x = br.ReadByte();
                    ncer.cebk.banks[i].cells[j].xOffset = (x < 0x7F) ? x : x - 0x100;
                    byte byte2 = br.ReadByte();
                    Size tamaño = Obtener_Tamaño(Tools.Helper.ByteTo4Bits(byte1)[0], Tools.Helper.ByteTo4Bits(byte2)[0]);
                    ncer.cebk.banks[i].cells[j].height = (ushort)tamaño.Height;
                    ncer.cebk.banks[i].cells[j].width = (ushort)tamaño.Width;
                    ncer.cebk.banks[i].cells[j].tileOffset = (uint)(br.ReadUInt16());
                    ncer.cebk.banks[i].cells[j].tileOffset = (uint)(ncer.cebk.banks[i].cells[j].tileOffset * (ncer.cebk.unknown1 == 0x01 ? 2 : 1));
                }
                br.BaseStream.Position = posicion;
            }
            br.BaseStream.Position = ncer.header.header_size + ncer.cebk.section_size;

            // Lee la segunda sección LABL
            ncer.labl.id = br.ReadChars(4);
            ncer.labl.section_size = br.ReadUInt32();
            ncer.labl.offset = new uint[ncer.cebk.nBanks];
            ncer.labl.names = new string[ncer.cebk.nBanks];
            for (int i = 0; i < ncer.cebk.nBanks; i++)
            {
                ncer.labl.offset[i] = br.ReadUInt32();
                ncer.labl.names[i] = "";
                if (br.BaseStream.Position >= ncer.header.header_size + ncer.cebk.section_size + ncer.labl.section_size)
                    break;
                long posicion = br.BaseStream.Position;
                br.BaseStream.Position += (ncer.cebk.nBanks - (i + 1)) * 4 + ncer.labl.offset[i];
                for (; ; )
                {
                    char c = br.ReadChar();
                    if (c == '\x0')
                        break;
                    ncer.labl.names[i] += c;
                }
                if (br.BaseStream.Position >= ncer.header.header_size + ncer.cebk.section_size + ncer.labl.section_size)
                    break;
                br.BaseStream.Position = posicion;
            }

            // Lee la tercera sección UEXT
            br.BaseStream.Position = ncer.header.header_size + ncer.cebk.section_size + ncer.labl.section_size;
            ncer.uext.id = br.ReadChars(4);
            ncer.uext.section_size = br.ReadUInt32();
            ncer.uext.unknown = br.ReadUInt32();

            br.Close();
            br.Dispose();

            return ncer;
        }
        public static Size Obtener_Tamaño(byte byte1, byte byte2)
        {
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
            }

            return tamaño;
        }

        public static Bitmap Obtener_Imagen(Bank banco, NCGR tile, NCLR paleta)
        {
            if (banco.cells.Length == 0)
                return new Bitmap(1, 1);
            Size tamaño = new Size((0 - banco.cells[0].xOffset) * 2, (0 - banco.cells[0].yOffset) * 2);
            Bitmap imagen = new Bitmap(tamaño.Width, tamaño.Height);
            Graphics grafico = Graphics.FromImage(imagen);

            /* DEBUG
            // Dibuja el entorno
            for (int i = -256; i < 256; i += 8)
            {
                grafico.DrawLine(Pens.LightBlue, i + tamaño.Width / 2, 0, i + tamaño.Width / 2, tamaño.Height);
                grafico.DrawLine(Pens.LightBlue, 0, i + tamaño.Height / 2, tamaño.Width, i + tamaño.Height / 2);
            }*/

            Bitmap[] celdas = new Bitmap[banco.nCells];
            for (int i = 0; i < banco.nCells; i++)
            {
                celdas[i] = Imagen_NCGR.Crear_Imagen(tile, paleta, (int)banco.cells[i].tileOffset, banco.cells[i].width / 8, banco.cells[i].height / 8);
                grafico.DrawImageUnscaled(celdas[i], tamaño.Width / 2 + banco.cells[i].xOffset, tamaño.Height / 2 + banco.cells[i].yOffset);
                // DEBUG, dibuja los bordes de las celdas
                //grafico.DrawRectangle(Pens.Black, tamaño.Width / 2 + banco.cells[i].xOffset, tamaño.Height / 2 + banco.cells[i].yOffset, 
                //    banco.cells[i].width, banco.cells[i].height);
            }

            return imagen;
        }
    }
}
