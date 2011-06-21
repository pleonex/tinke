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
using PluginInterface;

namespace Tinke
{
    public static class Imagen_NANR
    {
        public static NANR Leer(string archivo)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));
            NANR nanr = new NANR();

            // Cabecera genérica
            nanr.cabecera.id = br.ReadChars(4);
            nanr.cabecera.endianess = br.ReadUInt16();
            if (nanr.cabecera.endianess == 0xFFFE)
                nanr.cabecera.id.Reverse<char>();
            nanr.cabecera.constant = br.ReadUInt16();
            nanr.cabecera.file_size = br.ReadUInt32();
            nanr.cabecera.header_size = br.ReadUInt16();
            nanr.cabecera.nSection = br.ReadUInt16();

            #region Sección ABNK
            // Primera sección ABNK (Animation BaNK)
            nanr.abnk.id = br.ReadChars(4);
            nanr.abnk.length = br.ReadUInt32();
            nanr.abnk.nBanks = br.ReadUInt16();
            nanr.abnk.tFrames = br.ReadUInt16();
            nanr.abnk.constant = br.ReadUInt32();
            nanr.abnk.offset1 = br.ReadUInt32();
            nanr.abnk.offset2 = br.ReadUInt32();
            nanr.abnk.padding = br.ReadUInt64();
            nanr.abnk.anis = new Animation[nanr.abnk.nBanks];

            // Cabecera de cada Bank
            for (int i = 0; i < nanr.abnk.nBanks; i++)
            {
                br.BaseStream.Position = 0x30 + i * 0x10;

                Animation ani = new Animation();
                ani.nFrames = br.ReadUInt32();
                ani.dataType = br.ReadUInt16();
                if (ani.dataType != 0x00)
                    System.Windows.Forms.MessageBox.Show("Desconocido en parte");
                ani.unknown1 = br.ReadUInt16();
                ani.unknown2 = br.ReadUInt16();
                ani.unknown3 = br.ReadUInt16();
                ani.offset_frame = br.ReadUInt32();
                ani.frames = new Frame[ani.nFrames];

                // Cabecera de cada frame
                for (int j = 0; j < ani.nFrames; j++)
                {
                    br.BaseStream.Position = 0x18 + nanr.abnk.offset1 + j * 0x08 + ani.offset_frame;

                    Frame frame = new Frame();
                    frame.offset_data = br.ReadUInt32();
                    frame.unknown1 = br.ReadUInt16();
                    frame.constant = br.ReadUInt16();

                    // Datos de cada frame
                    br.BaseStream.Position = 0x18 + nanr.abnk.offset2 + frame.offset_data;
                    frame.data.nCell = br.ReadUInt16();

                    ani.frames[j] = frame;
                }

                nanr.abnk.anis[i] = ani;
            }
            #endregion
            #region Sección LABL
            // Lee la segunda LABL
            br.BaseStream.Position = nanr.cabecera.header_size + nanr.abnk.length;
            List<uint> offsets = new List<uint>();
            List<String> names = new List<string>();

            nanr.labl.id = br.ReadChars(4);
            if (new String(nanr.labl.id) != "LBAL")
                goto Tercera;
            nanr.labl.section_size = br.ReadUInt32();

            nanr.labl.names = new string[nanr.abnk.nBanks];
            // Primero se encuentran los offset a los nombres.
            for (int i = 0; i < nanr.abnk.nBanks; i++)
            {
                uint offset = br.ReadUInt32();
                if (offset >= nanr.labl.section_size - 8)
                {
                    br.BaseStream.Position -= 4;
                    break;
                }

                offsets.Add(offset);
            }
            nanr.labl.offset = offsets.ToArray();
            // Ahora leemos los nombres
            for (int i = 0; i < nanr.labl.offset.Length; i++)
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
            for (int i = 0; i < nanr.abnk.nBanks; i++)
                if (names.Count > i)
                    nanr.labl.names[i] = names[i];
                else
                    nanr.labl.names[i] = i.ToString();
            #endregion
            #region Sección UEXT
        // Lee la tercera sección UEXT
            nanr.uext.id = br.ReadChars(4);
            if (new String(nanr.uext.id) != "TXEU")
                goto Fin;

            nanr.uext.section_size = br.ReadUInt32();
            nanr.uext.unknown = br.ReadUInt32();
            #endregion

        Fin:
            br.Close();
            return nanr;
        }
    }
}
