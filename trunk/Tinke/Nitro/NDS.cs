﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using PluginInterface;

namespace Tinke.Nitro
{
    public static class NDS
    {
        public static Estructuras.ROMHeader LeerCabecera(string file)
        {
            Estructuras.ROMHeader nds = new Estructuras.ROMHeader();

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            Console.WriteLine("<b>" + 
                Tools.Helper.ObtenerTraduccion("Messages", "S03") + "</b> "
                + new FileInfo(file).Name);

            nds.gameTitle = br.ReadChars(12);
            nds.gameCode = br.ReadChars(4);
            nds.makerCode = br.ReadChars(2);
            nds.unitCode = br.ReadChar();
            nds.encryptionSeed = br.ReadByte();
            nds.tamaño = (UInt32)Math.Pow(2, 20 + br.ReadByte());
            nds.reserved = br.ReadBytes(9);
            nds.ROMversion = br.ReadByte();
            nds.internalFlags = br.ReadByte();
            nds.ARM9romOffset = br.ReadUInt32();
            nds.ARM9entryAddress = br.ReadUInt32();
            nds.ARM9ramAddress = br.ReadUInt32();
            nds.ARM9size = br.ReadUInt32();
            nds.ARM7romOffset = br.ReadUInt32();
            nds.ARM7entryAddress = br.ReadUInt32();
            nds.ARM7ramAddress = br.ReadUInt32();
            nds.ARM7size = br.ReadUInt32();
            nds.fileNameTableOffset = br.ReadUInt32();
            nds.fileNameTableSize = br.ReadUInt32();
            nds.FAToffset = br.ReadUInt32();
            nds.FATsize = br.ReadUInt32();
            nds.ARM9overlayOffset = br.ReadUInt32();
            nds.ARM9overlaySize = br.ReadUInt32();
            nds.ARM7overlayOffset = br.ReadUInt32();
            nds.ARM7overlaySize = br.ReadUInt32();
            nds.flagsRead = br.ReadUInt32();
            nds.flagsInit = br.ReadUInt32();
            nds.bannerOffset = br.ReadUInt32();
            nds.secureCRC16 = br.ReadUInt16();
            nds.ROMtimeout = br.ReadUInt16();
            nds.ARM9autoload = br.ReadUInt32();
            nds.ARM7autoload = br.ReadUInt32();
            nds.secureDisable = br.ReadUInt64();
            nds.ROMsize = br.ReadUInt32();
            nds.headerSize = br.ReadUInt32();
            nds.reserved2 = br.ReadBytes(56);
            br.BaseStream.Seek(156, SeekOrigin.Current); //nds.logo = br.ReadBytes(156); Logo de Nintendo utilizado para comprobaciones
            nds.logoCRC16 = br.ReadUInt16();
            nds.headerCRC16 = br.ReadUInt16();
            nds.debug_romOffset = br.ReadUInt32();
            nds.debug_size = br.ReadUInt32();
            nds.debug_ramAddress = br.ReadUInt32();
            nds.reserved3 = br.ReadUInt32();

            br.BaseStream.Position = 0x4000;
            nds.secureCRC = (Tools.CRC16.Calcular(br.ReadBytes(0x4000)) == nds.secureCRC16) ? true : false;
            br.BaseStream.Position = 0xC0;
            nds.logoCRC = (Tools.CRC16.Calcular(br.ReadBytes(156)) == nds.logoCRC16) ? true : false;
            br.BaseStream.Position = 0x0;
            nds.headerCRC = (Tools.CRC16.Calcular(br.ReadBytes(0x15E)) == nds.headerCRC16) ? true : false;

            br.Close();

            Console.WriteLine("<b>" +
                Tools.Helper.ObtenerTraduccion("Messages", "S04")
                + "</b><br>" + new String(nds.gameTitle).Replace("\0", "") + 
                " (" + new String(nds.gameCode).Replace("\0", "") + ')');

            return nds;
        }
        public static void EscribirCabecera(string salida, Estructuras.ROMHeader cabecera, string romFile)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(salida, FileMode.Create));
            BinaryReader br = new BinaryReader(File.OpenRead(romFile));
            br.BaseStream.Position = 0xC0;
            Console.Write("Cabecera...");

            bw.Write(cabecera.gameTitle);
            bw.Write(cabecera.gameCode);
            bw.Write(cabecera.makerCode);
            bw.Write(cabecera.unitCode);
            bw.Write(cabecera.encryptionSeed);
            bw.Write((byte)(Math.Log(cabecera.tamaño, 2) - 20));
            bw.Write(cabecera.reserved);
            bw.Write(cabecera.ROMversion);
            bw.Write(cabecera.internalFlags);
            bw.Write(cabecera.ARM9romOffset);
            bw.Write(cabecera.ARM9entryAddress);
            bw.Write(cabecera.ARM9ramAddress);
            bw.Write(cabecera.ARM9size);
            bw.Write(cabecera.ARM7romOffset);
            bw.Write(cabecera.ARM7entryAddress);
            bw.Write(cabecera.ARM7ramAddress);
            bw.Write(cabecera.ARM7size);
            bw.Write(cabecera.fileNameTableOffset);
            bw.Write(cabecera.fileNameTableSize);
            bw.Write(cabecera.FAToffset);
            bw.Write(cabecera.FATsize);
            bw.Write(cabecera.ARM9overlayOffset);
            bw.Write(cabecera.ARM9overlaySize);
            bw.Write(cabecera.ARM7overlayOffset);
            bw.Write(cabecera.ARM7overlaySize);
            bw.Write(cabecera.flagsRead);
            bw.Write(cabecera.flagsInit);
            bw.Write(cabecera.bannerOffset);
            bw.Write(cabecera.secureCRC16);
            bw.Write(cabecera.ROMtimeout);
            bw.Write(cabecera.ARM9autoload);
            bw.Write(cabecera.ARM7autoload);
            bw.Write(cabecera.secureDisable);
            bw.Write(cabecera.ROMsize);
            bw.Write(cabecera.headerSize);
            bw.Write(cabecera.reserved2);
            bw.Write(br.ReadBytes(0x9C));
            bw.Write(cabecera.logoCRC16);
            bw.Write(cabecera.headerCRC16);
            bw.Write(cabecera.debug_romOffset);
            bw.Write(cabecera.debug_size);
            bw.Write(cabecera.debug_ramAddress);
            bw.Write(cabecera.reserved3);
            bw.Flush();

            int relleno = (int)(cabecera.headerSize - bw.BaseStream.Length);
            for (int i = 0; i < relleno; i++)
                bw.Write((byte)0x00);

            bw.Flush();
            bw.Close();
            br.Close();

            Console.WriteLine(" {0} bytes escritos correctamente.", new FileInfo(salida).Length);
        }

        public static string CodeToString(Type enumeracion, char[] id)
        {
            try { return Enum.GetName(enumeracion, Int32.Parse(Char.ConvertToUtf32(Char.ToString(id[0]), 0).ToString() + Char.ConvertToUtf32(char.ToString(id[1]), 0).ToString())); }
            catch { return "Desconocido"; }
        }
        public static string CodeToString(Type enumeracion, char id)
        {
            try { return Enum.GetName(enumeracion, Char.ConvertToUtf32(char.ToString(id), 0)); }
            catch { return "Desconocido"; }
        }

        public static Estructuras.Banner LeerBanner(string file, UInt32 offset)
        {
            Estructuras.Banner bn = new Estructuras.Banner();
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = offset;

            bn.version = br.ReadUInt16();
            bn.CRC16 = br.ReadUInt16();
            bn.reserved = br.ReadBytes(0x1C);
            bn.tileData = br.ReadBytes(0x200);
            bn.palette = br.ReadBytes(0x20);
            bn.japaneseTitle = TitleToString(br.ReadBytes(0x100));
            bn.englishTitle = TitleToString(br.ReadBytes(0x100));
            bn.frenchTitle = TitleToString(br.ReadBytes(0x100));
            bn.germanTitle = TitleToString(br.ReadBytes(0x100));
            bn.italianTitle = TitleToString(br.ReadBytes(0x100));
            bn.spanishTitle = TitleToString(br.ReadBytes(0x100));

            br.BaseStream.Position = offset + 0x20;
            bn.checkCRC = (Tools.CRC16.Calcular(br.ReadBytes(0x820)) == bn.CRC16) ? true : false;

            br.Close();

            Console.WriteLine(bn.englishTitle.Replace("\0", ""));

            return bn;
        }
        public static void EscribirBanner(string salida, Estructuras.Banner banner)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(salida, FileMode.Create));
            Console.Write("Banner...");

            bw.Write(banner.version);
            bw.Write(banner.CRC16);
            bw.Write(banner.reserved);
            bw.Write(banner.tileData);
            bw.Write(banner.palette);
            bw.Write(StringToTitle(banner.japaneseTitle));
            bw.Write(StringToTitle(banner.englishTitle));
            bw.Write(StringToTitle(banner.frenchTitle));
            bw.Write(StringToTitle(banner.germanTitle));
            bw.Write(StringToTitle(banner.italianTitle));
            bw.Write(StringToTitle(banner.spanishTitle));
            bw.Flush();

            Console.WriteLine(" {0} bytes escritos correctamente.", bw.BaseStream.Length);
            bw.Close();
        }
        public static string TitleToString(byte[] data)
        {
            string title = "";
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0x00)
                    continue; // Codificación UNICODE
                if (data[i] == 0x0A)
                    title += '\r';      // Nueva línea

                title += Char.ConvertFromUtf32(data[i]);
            }

            return title;
        }
        public static byte[] StringToTitle(string title)
        {
            List<byte> data = new List<byte>();

            title = title.Replace("\r", "");
            data.AddRange(Encoding.Unicode.GetBytes(title));

            int relleno = 0x100 - data.Count;
            for (int i = 0; i < relleno; i++)
                data.Add(0x00);

            return data.ToArray();
        }
        public static Bitmap IconoToBitmap(byte[] tileData, byte[] paletteData)
        {
            Bitmap imagen = new Bitmap(32, 32);
            Color[] paleta = Convertir.BGR555(paletteData);

            tileData = Tools.Helper.BytesTo4BitsRev(tileData);
            int i = 0;
            for (int hi = 0; hi < 4; hi++)
            {
                for (int wi = 0; wi < 4; wi++)
                {
                    for (int h = 0; h < 8; h++)
                    {
                        for (int w = 0; w < 8; w++)
                        {
                            imagen.SetPixel(w + wi * 8, h + hi * 8, paleta[tileData[i]]);
                            i++;
                        }
                    }
                }
            }

            return imagen;
        }

        public static void EscribirArchivos(string salida, string romFile, Carpeta root, int nFiles)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(salida, FileMode.Create));
            BinaryReader br = new BinaryReader(new FileStream(romFile, FileMode.Open));

            Console.Write("Archivos...");

            for (int i = 0; i < nFiles; i++)
            {
                Archivo currFile = BuscarArchivo(i, root);
                if (currFile.name.StartsWith("overlay")) // Los overlays no van en esta sección
                    continue;

                if (currFile.offset != 0x00)
                {
                    br.BaseStream.Position = currFile.offset;
                    bw.Write(br.ReadBytes((int)currFile.size));
                    bw.Flush();
                }
                else // El archivo es modificado y no está en la ROM
                {
                    bw.Write(File.ReadAllBytes(currFile.path));
                    bw.Flush();
                }
            }

            bw.Flush();
            bw.Close();
            br.Close();
            Console.WriteLine(" {0} archivos escritos correctamente.", nFiles);
        }
        private static Archivo BuscarArchivo(int id, Carpeta currFolder)
        {
            if (currFolder.files is List<Archivo>)
                foreach (Archivo archivo in currFolder.files)
                    if (archivo.id == id)
                        return archivo;


            if (currFolder.folders is List<Carpeta>)
            {
                foreach (Carpeta subFolder in currFolder.folders)
                {
                    Archivo currFile = BuscarArchivo(id, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new Archivo();
        }
    }
}