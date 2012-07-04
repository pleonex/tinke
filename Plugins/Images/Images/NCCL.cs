using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using Ekona;
using Ekona.Images;

namespace Images
{
    public class NCCL : PaletteBase
    {
        sNCCL nccl;

        public NCCL(string file, int id, string fileName = "") : base(file, id, fileName)
        {
        }

        public override void Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            nccl = new sNCCL();

            // Generic header
            nccl.generic.id = br.ReadChars(4);           // Should be NCCL
            nccl.generic.endianess = br.ReadUInt16();
            nccl.generic.constant = br.ReadUInt16();
            nccl.generic.file_size = br.ReadUInt32();
            nccl.generic.header_size = br.ReadUInt16();
            nccl.generic.nSection = br.ReadUInt16();
            

            // PALT (PALeTte) section
            nccl.palt.type = br.ReadChars(4);            // Should be PALT
            nccl.palt.size = br.ReadUInt32();
            nccl.palt.num_colors = br.ReadUInt32();      // Number of colors per palette
            nccl.palt.num_palette = br.ReadUInt32();

            Color[][] palette = new Color[nccl.palt.num_palette][];
            for (int i = 0; i < nccl.palt.num_palette; i++)
            {
                // Each color is 2bytes (BGR555 encoding)
                palette[i] = Actions.BGR555ToColor(br.ReadBytes((int)nccl.palt.num_colors * 2));
            }

            // CMNT section
            if (nccl.generic.nSection == 2)
            {
                nccl.cmnt.type = br.ReadChars(4);
                nccl.cmnt.size = br.ReadUInt32();
                nccl.cmnt.unknown = br.ReadBytes((int)nccl.cmnt.size - 8);
            }

            br.Close();

            Set_Palette(palette, false);
            this.fileName = Path.GetFileName(file);
        }

        public override void Write(string fileOut)
        {
            System.Windows.Forms.MessageBox.Show("Not supported");
        }

        public struct sNCCL
        {
            public NitroHeader generic;     // Generic header
            public PALT palt;
            public CMNT cmnt;

            public struct PALT
            {
                public char[] type;     // Should be PALT
                public uint size;
                public uint num_colors; // Number of colors per palette
                public uint num_palette;
            }
            public struct CMNT
            {
                public char[] type;     // Should be CMNT
                public uint size;       // Should be 0x0C
                public byte[] unknown;
            }
        }
    }
}
