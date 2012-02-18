using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Sounds
{
    public class SADL : SoundBase
    {
        sSADL sadl;
        string lang;

        public SADL(string lang, string file, int id)
            : base(file, id, "SADL", "vgmstream", false) { this.lang = lang; }

        public override byte[] Read_File()
        {
            XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" +
                Path.DirectorySeparatorChar + "SoundLang.xml");
            xml = xml.Element(lang).Element("Messages");

            sadl = new sSADL();
            BinaryReader br = new BinaryReader(File.OpenRead(soundFile));

            sadl.id = br.ReadChars(4);

            br.BaseStream.Position = 0x31;
            sadl.loopFlag = br.ReadByte();
            sadl.channel = br.ReadByte();

            byte coding = br.ReadByte();
            sadl.coding = (Coding)(coding & 0xF0);
            Console.WriteLine(xml.Element("S03").Value + ' ' + sadl.coding.ToString());
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

            // Set the data
            total_samples = sadl.num_samples;
            sample_rate = sadl.sample_rate;
            channels = sadl.channel;
            block_size = sadl.interleave_block_size;
            sample_bitdepth = 4;

            loop_enabled = (sadl.loopFlag != 0 ? true : false);
            loop_begin_sample = sadl.loopOffset;
            loop_end_sample = sadl.num_samples;

            br.BaseStream.Position = 0;
            byte[] buffer = br.ReadBytes((int)br.BaseStream.Length);
            br.Close();

            return buffer;
        }
        public override byte[] Decode(byte[] encoded, bool loop_enabled)
        {
            if (sadl.coding == Coding.NDS_PROCYON)
                return Decode_Procyon(encoded);

            // IMA_ADPCM encoding:
            int start_offset = 0x100;
            int pos;

            if (!loop_enabled)
                pos = start_offset;
            else
                pos = (int)(start_offset + loop_begin_sample * 2 * block_size);

            // Getting channel data
            List<byte> left_channel = new List<byte>();
            List<byte> right_channel = new List<byte>();
            List<byte> data = new List<byte>();

            for (; pos < encoded.Length; )
            {
                if (sadl.channel == 2)   // Stereo
                {
                    Byte[] buffer = new byte[sadl.interleave_block_size];

                    Array.Copy(encoded, pos, buffer, 0, buffer.Length);
                    pos += buffer.Length;
                    left_channel.AddRange(buffer);

                    Array.Copy(encoded, pos, buffer, 0, buffer.Length);
                    pos += buffer.Length;
                    right_channel.AddRange(buffer);
                }
                else   // Mono
                {
                    Byte[] buffer = new byte[sadl.interleave_block_size * 2];
                    Array.Copy(encoded, pos, buffer, 0, buffer.Length);
                    pos += buffer.Length;
                    data.AddRange(buffer);
                }
            }

            // Decompressing channels
            if (sadl.coding == Coding.INT_IMA)
            {
                if (sadl.channel == 2)
                {
                    Byte[] dLeft_channel = new Byte[1]; // Make the compiler happy :)
                    Byte[] dRight_channel = new Byte[1];

                    dLeft_channel = Compression.IMA_ADPCM.Decompress(left_channel.ToArray());

                    dRight_channel = Compression.IMA_ADPCM.Decompress(right_channel.ToArray());

                    data.AddRange(Helper.MergeChannels(dLeft_channel, dRight_channel));
                }
                else
                {
                    Byte[] buffer = Compression.IMA_ADPCM.Decompress(data.ToArray());
                    data.Clear();
                    data.AddRange(buffer);
                }
            }

            return data.ToArray();
        }

        public byte[] Decode_Procyon(byte[] encoded)
        {
            int start_offset = 0x100;

            byte[][] buffer = new byte[2][];
            int[][] hist = new int[channels][];
            int[] length = new int[2];
            int[] offset = new int[channels];
            for (int i = 0; i < channels; i++)
            {
                offset[i] = (int)(start_offset + block_size * i);
                buffer[i] = new byte[NumberSamples * 2];
                length[i] = 0;
                hist[i] = new int[2];
            }

            int samples_written = 0;
            int samples_to_do = 0;

            while (samples_written < NumberSamples)
            {
                samples_to_do = 30;
                if (samples_written + samples_to_do > NumberSamples)
                    samples_to_do = (int)total_samples - samples_written;

                for (int chan = 0; chan < channels; chan++)
                {
                    byte[] temp = Compression.Procyon.Decode(encoded,
                        offset[chan],
                        samples_to_do,
                        ref hist[chan],
                        (int)channels);

                    Array.Copy(temp, 0, buffer[chan], length[chan], temp.Length);
                    length[chan] += temp.Length;

                    offset[chan] += (int)(block_size * channels);
                }

                samples_written += samples_to_do;
            }

            byte[] mus;
            if (channels == 1)
                mus = buffer[0];
            else
                mus = Helper.MergeChannels(buffer[0], buffer[1]);

            return mus;
        }

        public override void Write_File(string fileOut, byte[] data)
        {
            throw new NotImplementedException();
        }
        public override byte[] Encode()
        {
            throw new NotImplementedException();
        }
    }

    public struct sSADL
    {
        // Get from vgmstream
        public char[] id;
        public uint file_size;
        public byte loopFlag;
        public uint loopOffset;
        public uint channel;
        public Coding coding;
        public uint sample_rate;
        public uint num_samples;
        public uint interleave_block_size;
    }
    public enum Coding : byte
    {
        INT_IMA = 0x70,
        NDS_PROCYON = 0xB0,
    }
}
