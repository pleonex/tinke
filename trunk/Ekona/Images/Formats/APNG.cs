// ----------------------------------------------------------------------
// <copyright file="APNG.cs" company="none">

// Copyright (C) 2012
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
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>24/06/2012 14:47:38</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ekona.Images.Formats
{
    public static class APNG
    {
        // Info from:
        // https://wiki.mozilla.org/APNG_Specification
        // http://www.w3.org/TR/PNG/

        /// <summary>
        /// Save an animation in a APNG file (Firefox supported)
        /// </summary>
        /// <param name="pngs">All frames (path of files or bitmaps)</param>
        /// <param name="apng">The path of the output file</param>
        /// <param name="delay">The delay between frames (delay/1000)</param>
        /// <param name="loops">The number of  loops (if 0 = infinite)</param>
        public static void Create(string[] pngs, string apng, int delay, int loops)
        {
            byte[] pngSignature = new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 };
            IHDR ihdr = Read_IHDR(pngs[0]);
            
            #region Section acTL
            acTL actl = new acTL();
            actl.length = BitConverter.GetBytes(8).Reverse().ToArray();
            actl.id = Encoding.ASCII.GetBytes(new char[] { 'a', 'c', 'T', 'L' });
            actl.num_frames = BitConverter.GetBytes(pngs.Length).Reverse().ToArray();
            actl.num_plays = BitConverter.GetBytes(loops).Reverse().ToArray(); // Loop
            List<byte> stream = new List<byte>();
            stream.AddRange(actl.id);
            stream.AddRange(actl.num_frames);
            stream.AddRange(actl.num_plays);
            actl.crc = Helper.CRC32.Calculate(stream.ToArray());
            stream.Clear();
            #endregion

            List<fcTL> fctl = new List<fcTL>();
            List<fdAT> fdat = new List<fdAT>();
            int i = 0;
            fctl.Add(Read_fcTL(pngs[0], i, delay)); 
            i++;
            byte[] IDAT = Read_IDAT(pngs[0]);

            foreach (string png in pngs)
            {
                if (png == pngs[0])
                    continue;

                fctl.Add(Read_fcTL(png, i, delay));
                i++;
                fdat.Add(Read_fdAT(png, i));
                i++;
            }

            IEND iend = new IEND();
            iend.id = Encoding.ASCII.GetBytes(new char[] { 'I', 'E', 'N', 'D' });
            iend.length = BitConverter.GetBytes(0);
            iend.crc = Helper.CRC32.Calculate(iend.id);

            Write(apng, pngSignature, ihdr, actl, IDAT, fctl.ToArray(), fdat.ToArray(), iend);
        }
        public static void Create(System.Drawing.Bitmap[] pngs, string apng, int delay, int loops)
        {
            string[] files = new string[pngs.Length];

            for (int i = 0; i < pngs.Length; i++)
            {
                files[i] = Path.GetTempFileName();
                pngs[i].Save(files[i]);
            }

            Create(files, apng, delay, loops);
            for (int i = 0; i < files.Length; i++)
                File.Delete(files[i]);
        }

        private static void Write(string apng, byte[] signature, IHDR ihdr, acTL actl, byte[] idat,
            fcTL[] fctl, fdAT[] fdat, IEND iend)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(apng, FileMode.Create));

            bw.Write(signature);

            bw.Write(ihdr.length);
            bw.Write(ihdr.id);
            bw.Write(ihdr.width);
            bw.Write(ihdr.height);
            bw.Write(ihdr.depth);
            bw.Write(ihdr.colour_type);
            bw.Write(ihdr.compression);
            bw.Write(ihdr.filter);
            bw.Write(ihdr.interlace);
            bw.Write(ihdr.crc);

            bw.Write(actl.length);
            bw.Write(actl.id);
            bw.Write(actl.num_frames);
            bw.Write(actl.num_plays);
            bw.Write(actl.crc);

            bw.Write(fctl[0].length);
            bw.Write(fctl[0].id);
            bw.Write(fctl[0].sequence_numer);
            bw.Write(fctl[0].width);
            bw.Write(fctl[0].height);
            bw.Write(fctl[0].x_offset);
            bw.Write(fctl[0].y_offset);
            bw.Write(fctl[0].delay_num);
            bw.Write(fctl[0].delay_den);
            bw.Write(fctl[0].dispose_op);
            bw.Write(fctl[0].blend_op);
            bw.Write(fctl[0].crc);

            bw.Write(idat);

            for (int i = 0; i < fdat.Length; i++)
            {
                bw.Write(fctl[i + 1].length);
                bw.Write(fctl[i + 1].id);
                bw.Write(fctl[i + 1].sequence_numer);
                bw.Write(fctl[i + 1].width);
                bw.Write(fctl[i + 1].height);
                bw.Write(fctl[i + 1].x_offset);
                bw.Write(fctl[i + 1].y_offset);
                bw.Write(fctl[i + 1].delay_num);
                bw.Write(fctl[i + 1].delay_den);
                bw.Write(fctl[i + 1].dispose_op);
                bw.Write(fctl[i + 1].blend_op);
                bw.Write(fctl[i + 1].crc);

                bw.Write(fdat[i].length);
                bw.Write(fdat[i].id);
                bw.Write(fdat[i].sequence_number);
                bw.Write(fdat[i].data);
                bw.Write(fdat[i].crc);
            }

            bw.Write(iend.length);
            bw.Write(iend.id);
            bw.Write(iend.crc);

            bw.Flush();
            bw.Close();
        }

        private static IHDR Read_IHDR(string png)
        {
            BinaryReader br = new BinaryReader(new FileStream(png, FileMode.Open));
            br.BaseStream.Position = 0x08;
            IHDR ihdr = new IHDR();

            ihdr.length = br.ReadBytes(4);
            ihdr.id = br.ReadBytes(4);
            ihdr.width = br.ReadUInt32();
            ihdr.height = br.ReadUInt32();
            ihdr.depth = br.ReadByte();
            ihdr.colour_type = br.ReadByte();
            ihdr.compression = br.ReadByte();
            ihdr.filter = br.ReadByte();
            ihdr.interlace = br.ReadByte();
            ihdr.crc = br.ReadBytes(4);
            
            br.Close();
            return ihdr;
        }
        private static fcTL Read_fcTL(string png, int seq, int delay)
        {
            BinaryReader br = new BinaryReader(new FileStream(png, FileMode.Open));
            br.BaseStream.Position = 0x10;
            fcTL fctl = new fcTL();

            fctl.length = BitConverter.GetBytes(26).Reverse().ToArray();
            fctl.id = Encoding.ASCII.GetBytes(new char[] { 'f', 'c', 'T', 'L' });
            fctl.sequence_numer = BitConverter.GetBytes(seq).Reverse().ToArray();
            fctl.width = br.ReadBytes(4);
            fctl.height = br.ReadBytes(4);
            fctl.x_offset = BitConverter.GetBytes(0);
            fctl.y_offset = BitConverter.GetBytes(0);
            fctl.delay_num = BitConverter.GetBytes((ushort)delay).Reverse().ToArray();
            fctl.delay_den = BitConverter.GetBytes((ushort)1000).Reverse().ToArray();
            fctl.dispose_op = 1;
            fctl.blend_op = 0;

            List<Byte> stream = new List<byte>();
            stream.AddRange(fctl.id);
            stream.AddRange(fctl.sequence_numer);
            stream.AddRange(fctl.width);
            stream.AddRange(fctl.height);
            stream.AddRange(fctl.x_offset);
            stream.AddRange(fctl.y_offset);
            stream.AddRange(fctl.delay_num);
            stream.AddRange(fctl.delay_den);
            stream.Add(fctl.dispose_op);
            stream.Add(fctl.blend_op);
            fctl.crc = Helper.CRC32.Calculate(stream.ToArray());
            stream.Clear();

            br.Close();
            return fctl;
        }
        private static byte[] Read_IDAT(string png)
        {
            BinaryReader br = new BinaryReader(new FileStream(png, FileMode.Open));
            byte[] buffer;

            bool encontrado = false;
            string c = "\0\0\0\0";
            while (!encontrado)
            {
                c = c.Remove(0, 1);

                c += (char)br.ReadByte();
                if (c == "IDAT")
                    encontrado = true;
            }

            br.BaseStream.Position -= 8;
            int length = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
            br.BaseStream.Position -= 4;
            buffer = br.ReadBytes(length + 12);

            br.Close();
            return buffer;
        }
        private static fdAT Read_fdAT(string png, int i)
        {
            BinaryReader br = new BinaryReader(new FileStream(png, FileMode.Open));
            fdAT fdat = new fdAT();

            fdat.id = Encoding.ASCII.GetBytes(new char[] { 'f', 'd', 'A', 'T' });
            fdat.sequence_number = BitConverter.GetBytes(i).Reverse().ToArray();

            bool encontrado = false;
            string c = "\0\0\0\0";
            while (!encontrado)
            {
                c = c.Remove(0, 1);

                c += (char)br.ReadByte();
                if (c == "IDAT")
                    encontrado = true;
            }

            br.BaseStream.Position -= 8;
            int length = BitConverter.ToInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
            fdat.length = BitConverter.GetBytes(length + 4).Reverse().ToArray();         
            br.BaseStream.Position += 4;
            fdat.data = br.ReadBytes(length);
            List<Byte> stream = new List<byte>();
            stream.AddRange(fdat.id);
            stream.AddRange(fdat.sequence_number);
            stream.AddRange(fdat.data);
            fdat.crc = Helper.CRC32.Calculate(stream.ToArray());

            br.Close();
            return fdat;
        }

        private struct IHDR
        {
            public byte[] length;
            public byte[] id;
            public uint width;
            public uint height;
            public byte depth;
            public byte colour_type;
            public byte compression;
            public byte filter;
            public byte interlace;
            public byte[] crc;
        }
        private struct acTL
        {
            public byte[] length;
            public byte[] id;
            public byte[] num_frames;
            public byte[] num_plays;
            public byte[] crc;
        }
        private struct fcTL
        {
            public byte[] length;
            public byte[] id;
            public byte[] sequence_numer;
            public byte[] width;
            public byte[] height;
            public byte[] x_offset;
            public byte[] y_offset;
            public byte[] delay_num;
            public byte[] delay_den;
            public byte dispose_op;
            public byte blend_op;
            public byte[] crc;
        }
        private struct fdAT
        {
            public byte[] length;
            public byte[] id;
            public byte[] sequence_number;
            public byte[] data;
            public byte[] crc;
        }
        private struct IEND
        {
            public byte[] length;
            public byte[] id;
            public byte[] crc;
        }
    }
}