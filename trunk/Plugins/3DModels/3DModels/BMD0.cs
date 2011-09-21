using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using PluginInterface;

namespace _3DModels
{
    public static class BMD0
    {
        public static sBTX0 Read(string file, int id, IPluginHost pluginHost)
        {
            String btxFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + "un_btx_" + Path.GetFileName(file);

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(btxFile));

            br.BaseStream.Position = 0x0E;
            ushort num_section = br.ReadUInt16();
            if (num_section == 1)
                throw new NotSupportedException("There isn't texture section");

            br.ReadUInt32(); // Model section offset
            uint texture_offset = br.ReadUInt32();

            br.BaseStream.Position = texture_offset;

            // Write generic header
            bw.Write(new char[] { 'B', 'T', 'X', '0' });
            bw.Write((uint)0x0101FFFE);
            bw.Write((uint)(br.BaseStream.Length - br.BaseStream.Position));
            bw.Write((ushort)0x10);
            bw.Write((ushort)0x01);
            bw.Write((uint)0x14);

            bw.Write(br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position)));
            br.Close();
            bw.Flush();
            bw.Close();

            return BTX0.Read(btxFile, id, pluginHost);
        }
    }
}
