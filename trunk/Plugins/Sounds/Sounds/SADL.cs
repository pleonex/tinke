using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Sounds
{
    public static class SADL
    {
        public  static sSADL Read(string filein, int id, string lang)
        {
            XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" +
                Path.DirectorySeparatorChar + "SoundLang.xml");
            xml = xml.Element(lang).Element("SoundControl");

            sSADL sadl = new sSADL();
            sadl.file_id = id;
            BinaryReader br = new BinaryReader(File.OpenRead(filein));

            sadl.id = br.ReadChars(4);

            br.BaseStream.Position = 0x31;
            sadl.loopFlag = br.ReadByte();
            sadl.channel = br.ReadByte();

            byte coding = br.ReadByte();
            sadl.coding = (Coding)(coding & 0xF0);
            Console.WriteLine(xml.Element("S0D").Value + ' ' + sadl.coding);
            switch (coding & 0x06)
            {
                case 4:
                    sadl.sample_rate = 32728;
                    break;
                case 2:
                    sadl.sample_rate = 16364;
                    break;
            }

            br.BaseStream.Position = 0x40;
            sadl.file_size = br.ReadUInt32();

            uint startOffset = 0x100;
            if (sadl.coding == Coding.INT_IMA)
                sadl.num_samples = (sadl.file_size - startOffset) / sadl.channel * 2;
            else if (sadl.coding == Coding.NDS_PROCYON)
                sadl.num_samples = (sadl.file_size - startOffset) / sadl.channel / 16 * 30;

            sadl.interleave_block_size = 0x10;

            br.BaseStream.Position = 0x54;
            if (sadl.loopFlag != 0)
            {
                if (sadl.coding == Coding.INT_IMA)
                    sadl.loopOffset = (br.ReadUInt32() - startOffset) / sadl.channel * 2;
                else if (sadl.coding == Coding.NDS_PROCYON)
                    sadl.loopOffset = (br.ReadUInt32() - startOffset) / sadl.channel / 16 * 30;
            }


            // Getting channel data
            br.BaseStream.Position = startOffset;
            sadl.left_channel = new List<byte>();
            sadl.right_channel = new List<byte>();
            sadl.data = new List<byte>();
            for (int i = 0; i < (sadl.file_size - startOffset) / sadl.interleave_block_size; i += 2)
            {
                if (sadl.channel == 2)
                {
                    sadl.left_channel.AddRange(br.ReadBytes((int)sadl.interleave_block_size));
                    sadl.right_channel.AddRange(br.ReadBytes((int)sadl.interleave_block_size));
                }
                else
                    sadl.data.AddRange(br.ReadBytes((int)sadl.interleave_block_size * 2));
            }

            // Decompressing channels
            Byte[] dLeft_channel = new Byte[1]; // Make the compiler happy :)
            Byte[] dRight_channel = new Byte[1];
            if (sadl.coding == Coding.INT_IMA)
            {
                if (sadl.channel == 2)
                {
                    dLeft_channel = Compression.ADPCM.Decompress(sadl.left_channel.ToArray());
                    sadl.left_channel.Clear();
                    sadl.left_channel.AddRange(dLeft_channel);
                    dRight_channel = Compression.ADPCM.Decompress(sadl.right_channel.ToArray());
                    sadl.right_channel.Clear();
                    sadl.right_channel.AddRange(dRight_channel);

                    sadl.data.AddRange(Helper.MergeChannels(dLeft_channel, dRight_channel));
                }
                else
                {
                    dLeft_channel = Compression.ADPCM.Decompress(sadl.data.ToArray());
                    sadl.data.Clear();
                    sadl.data.AddRange(dLeft_channel);
                }
            }


            br.Close();
            return sadl;
        }

        public static sWAV ConvertToWAV(sSADL sadl)
        {
            sWAV wav = WAV.Create((ushort)sadl.channel, sadl.sample_rate, 0x10, sadl.data.ToArray());
            wav.loopOffset = sadl.loopOffset;
            wav.loopFlag = sadl.loopFlag;
            wav.file_id = sadl.file_id;

            return wav;
        }
    }

    public struct sSADL
    {
        // Obtained from vgmstream
        public int file_id;
        public char[] id;
        public uint file_size;
        public byte loopFlag;
        public uint loopOffset;
        public uint channel;
        public Coding coding;
        public uint sample_rate;
        public uint num_samples;
        public uint interleave_block_size;

        public List<byte> left_channel;
        public List<byte> right_channel;
        public List<byte> data;
    }
    public enum Coding : byte
    {
        INT_IMA = 0x70,
        NDS_PROCYON = 0xB0,
    }
}
