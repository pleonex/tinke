using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace Tinke.Nitro
{
    public static class Overlay
    {
        public static Estructuras.ARMOverlay[] LeerOverlays(string file, UInt32 offset, UInt32 size, bool arm9)
        {
            Estructuras.ARMOverlay[] overlays = new Estructuras.ARMOverlay[size / 0x20];
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            for (int i = 0; i < overlays.Length; i++)
            {
                overlays[i] = new Estructuras.ARMOverlay();

                overlays[i].fileID = br.ReadUInt32();
                overlays[i].RAM_Adress = br.ReadUInt32();
                overlays[i].RAM_Size = br.ReadUInt32();
                overlays[i].BSS_Size = br.ReadUInt32();
                overlays[i].stInitStart = br.ReadUInt32();
                overlays[i].stInitEnd = br.ReadUInt32();
                overlays[i].fileID = br.ReadUInt32();
                overlays[i].reserved = br.ReadUInt32();
                overlays[i].ARM9 = arm9;
            }

            return overlays;
        }

        public static Archivo[] LeerOverlaysBasico(string file, UInt32 offset, UInt32 size, bool arm9)
        {
            Archivo[] overlays = new Archivo[size / 0x20];
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = offset;

            for (int i = 0; i < overlays.Length; i++)
            {
                overlays[i] = new Archivo();
                overlays[i].name = "overlay" + (arm9 ? '9' : '7') + '_' + br.ReadUInt32();
                br.ReadBytes(20);
                overlays[i].id = (ushort)br.ReadUInt32();
                br.ReadBytes(4);

            }

            return overlays;

        }

        public static void EscribirOverlays(string salida, Carpeta overlays, string romFile)
        {
            BinaryWriter bw = new BinaryWriter(new FileStream(salida, FileMode.Open));
            BinaryReader br = new BinaryReader(new FileStream(romFile, FileMode.Open));

            for (int i = 0; i < overlays.files.Count; i++)
            {
                if (overlays.files[i].offset != 0x00 && overlays.files[i].packFile == romFile)
                {
                    br.BaseStream.Position = overlays.files[i].offset;
                    bw.Write(br.ReadBytes((int)overlays.files[i].size));
                }
                else // Quien sabe cuándo sucederá esto...
                {
                    BinaryReader br2;
                    if (overlays.files[i].offset != 0x00)
                    {
                        br2 = new BinaryReader(new FileStream(overlays.files[i].packFile, FileMode.Open)); // Very, very, very but very very strange case xD
                        br2.BaseStream.Position = overlays.files[i].offset;
                    }
                    else
                        br2 = new BinaryReader(new FileStream(overlays.files[i].path, FileMode.Open));
                    bw.Write(br2.ReadBytes((int)overlays.files[i].size));
                    br2.Close();
                }
            }

            br.Close();
            bw.Flush();
            bw.Close();
        }
    }
}
