using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Compresion
{
    public static class Huffman
    {
        const int LZ77_TAG = 0x10, LZSS_TAG = 0x11, RLE_TAG = 0x30, HUFF_TAG = 0x20, NONE_TAG = 0x00;

        public static void DecompressHuffman(String filename, String  fileout)
        {
            /*
                   Data Header (32bit)
                       Bit0-3   Data size in bit units (normally 4 or 8)
                       Bit4-7   Compressed type (must be 2 for Huffman)
                       Bit8-31  24bit size of decompressed data in bytes
                   Tree Size (8bit)
                       Bit0-7   Size of Tree Table/2-1 (ie. Offset to Compressed Bitstream)
                   Tree Table (list of 8bit nodes, starting with the root node)
                     Root Node and Non-Data-Child Nodes are:
                       Bit0-5   Offset to next child node,
                                Next child node0 is at (CurrentAddr AND NOT 1)+Offset*2+2
                                Next child node1 is at (CurrentAddr AND NOT 1)+Offset*2+2+1
                       Bit6     Node1 End Flag (1=Next child node is data)
                       Bit7     Node0 End Flag (1=Next child node is data)
                     Data nodes are (when End Flag was set in parent node):
                       Bit0-7   Data (upper bits should be zero if Data Size is less than 8)
                   Compressed Bitstream (stored in units of 32bits)
                       Bit0-31  Node Bits (Bit31=First Bit)  (0=Node0, 1=Node1)
            */
            System.Xml.Linq.XElement xml = Basico.ObtenerTraduccion("Compression");
            BinaryReader br = new BinaryReader(File.OpenRead(filename));

            byte firstByte = br.ReadByte();

            int dataSize = firstByte & 0x0F;

            if ((firstByte & 0xF0) != HUFF_TAG)
            {
                br.BaseStream.Seek(0x4, SeekOrigin.Begin);
                if (br.ReadByte() != HUFF_TAG)
                    throw new InvalidDataException(String.Format(xml.Element("S08").Value, firstByte));
            }

            if (dataSize != 8 && dataSize != 4)
                throw new InvalidDataException(String.Format(xml.Element("S09").Value, dataSize));

            int decomp_size = 0;
            for (int i = 0; i < 3; i++)
            {
                decomp_size |= br.ReadByte() << (i * 8);
            }

            #region Decompress
            byte treeSize = br.ReadByte();
            HuffTreeNode.maxInpos = 4 + (treeSize + 1) * 2;

            HuffTreeNode rootNode = new HuffTreeNode();
            rootNode.parseData(br);

            br.BaseStream.Position = 4 + (treeSize + 1) * 2; // go to start of coded bitstream.
            // read all data
            uint[] indata = new uint[(br.BaseStream.Length - br.BaseStream.Position) / 4];
            for (int i = 0; i < indata.Length; i++)
                indata[i] = br.ReadUInt32();

            long curr_size = 0;
            decomp_size *= dataSize == 8 ? 1 : 2;
            byte[] outdata = new byte[decomp_size];

            int idx = -1;
            string codestr = "";
            LinkedList<byte> code = new LinkedList<byte>();
            int value;
            while (curr_size < decomp_size)
            {
                try
                {
                    codestr += Utils.uint_to_bits(indata[++idx]);
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new IndexOutOfRangeException(xml.Element("S0A").Value, e);
                }
                while (codestr.Length > 0)
                {
                    code.AddFirst(byte.Parse(codestr[0] + ""));
                    codestr = codestr.Remove(0, 1);
                    if (rootNode.getValue(code.Last, out value))
                    {
                        try
                        {
                            outdata[curr_size++] = (byte)value;
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            if (code.First.Value != 0)
                                throw ex;
                        }
                        code.Clear();
                    }
                }
            }
            if (codestr.Length > 0 || idx < indata.Length - 1)
            {
                while (idx < indata.Length - 1)
                    codestr += Utils.uint_to_bits(indata[++idx]);
                codestr = codestr.Replace("0", "");
                if (codestr.Length > 0)
                    Console.WriteLine(xml.Element("S0B").Value, codestr, idx, indata.Length);
            }

            byte[] realout;
            if (dataSize == 4)
            {
                realout = new byte[decomp_size / 2];
                for (int i = 0; i < decomp_size / 2; i++)
                {
                    if ((outdata[i * 2] & 0xF0) > 0
                        || (outdata[i * 2 + 1] & 0xF0) > 0)
                        throw new Exception(xml.Element("S0C").Value);
                    realout[i] = (byte)((outdata[i * 2] << 4) | outdata[i * 2 + 1]);
                }
            }
            else
            {
                realout = outdata;
            }
            #endregion

            BinaryWriter bw = new BinaryWriter(new FileStream(fileout, FileMode.CreateNew));
            bw.Write(realout);
            bw.Flush();
            bw.Close();

            Console.WriteLine(xml.Element("S0D").Value, filename);

            br.Close();
        }
    }

    class HuffTreeNode
    {
        internal static int maxInpos = 0;
        internal HuffTreeNode node0, node1;
        internal int data = -1; // [-1,0xFF]
        /// <summary>
        /// To get a value, provide the last node of a list of bytes &lt; 2. 
        /// the list will be read from back to front.
        /// </summary>
        internal bool getValue(LinkedListNode<byte> code, out int value)
        {
            value = data;
            if (code == null)
                return node0 == null && node1 == null && data >= 0;

            if (code.Value > 1)
                throw new Exception(String.Format("the list should be a list of bytes < 2. got:{0:g}", code.Value));

            byte c = code.Value;
            bool retVal;
            HuffTreeNode n = c == 0 ? node0 : node1;
            retVal = n != null && n.getValue(code.Previous, out value);
            return retVal;
        }

        internal int this[string code]
        {
            get
            {
                LinkedList<byte> c = new LinkedList<byte>();
                foreach (char ch in code)
                    c.AddFirst((byte)ch);
                int val = 1;
                return this.getValue(c.Last, out val) ? val : -1;
            }
        }

        internal void parseData(BinaryReader br)
        {
            /*
             * Tree Table (list of 8bit nodes, starting with the root node)
                     Root Node and Non-Data-Child Nodes are:
                       Bit0-5   Offset to next child node,
                                Next child node0 is at (CurrentAddr AND NOT 1)+Offset*2+2
                                Next child node1 is at (CurrentAddr AND NOT 1)+Offset*2+2+1
                       Bit6     Node1 End Flag (1=Next child node is data)
                       Bit7     Node0 End Flag (1=Next child node is data)
                     Data nodes are (when End Flag was set in parent node):
                       Bit0-7   Data (upper bits should be zero if Data Size is less than 8)
             */
            this.node0 = new HuffTreeNode();
            this.node1 = new HuffTreeNode();
            long currPos = br.BaseStream.Position;
            byte b = br.ReadByte();
            long offset = b & 0x3F;
            bool end0 = (b & 0x80) > 0, end1 = (b & 0x40) > 0;
            // parse data for node0
            br.BaseStream.Position = (currPos - (currPos & 1)) + offset * 2 + 2;
            if (br.BaseStream.Position < maxInpos)
            {
                if (end0)
                    node0.data = br.ReadByte();
                else
                    node0.parseData(br);
            }
            // parse data for node1
            br.BaseStream.Position = (currPos - (currPos & 1)) + offset * 2 + 2 + 1;
            if (br.BaseStream.Position < maxInpos)
            {
                if (end1)
                    node1.data = br.ReadByte();
                else
                    node1.parseData(br);
            }
            // reset position
            br.BaseStream.Position = currPos;
        }

        public override string ToString()
        {
            if (data < 0)
                return "<" + node0.ToString() + ", " + node1.ToString() + ">";
            else
                return String.Format("[{0:x}]", data);
        }

        internal int Depth
        {
            get
            {
                if (data < 0)
                    return 0;
                else
                    return 1 + Math.Max(node0.Depth, node1.Depth);
            }
        }
    }

}
