using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Tinke.Nitro
{
    public static class NDS
    {
        public static Estructuras.ROMHeader LeerCabecera(string file)
        {
            Estructuras.ROMHeader nds = new Estructuras.ROMHeader();

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            Console.WriteLine("Analizando ROM: " + new FileInfo(file).Name);

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

            br.BaseStream.Position = 0x4000;
            nds.secureCRC = (Tools.CRC16.Calcular(br.ReadBytes(0x4000)) == nds.secureCRC16) ? true : false;
            br.BaseStream.Position = 0xC0;
            nds.logoCRC = (Tools.CRC16.Calcular(br.ReadBytes(156)) == nds.logoCRC16) ? true : false;
            br.BaseStream.Position = 0x0;
            nds.headerCRC = (Tools.CRC16.Calcular(br.ReadBytes(0x15E)) == nds.headerCRC16) ? true : false;

            br.Close();
            br.Dispose();

            return nds;
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
            br.Dispose();

            return bn;
        }
        public static string TitleToString(byte[] data)
        {
            string title = "";
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0x00)
                    continue;
                if (data[i] == 0x0A)
                    title += '\r';      // Nueva línea

                title += Char.ConvertFromUtf32(data[i]);
            }

            return title;
        }
        public static Bitmap IconoToBitmap(byte[] tileData, byte[] paletteData)
        {
            Bitmap imagen = new Bitmap(32, 32);
            Color[] paleta = Imagen.Paleta.Convertidor.BGR555(paletteData);

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

    }
}
