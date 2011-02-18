using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Tinke
{
    public partial class VisorHex : Form
    {
        string file;
        UInt32 offset;
        UInt32 size;

        public VisorHex(string file, UInt32 offset, UInt32 size)
        {
            InitializeComponent();

            this.file = file;
            this.offset = offset;
            this.size = size;

            vScrollBar1.Maximum = (int)size / 0x10;
            Show_Hex(0);

            txtHex.Select(0, 0);
            txtHex.HideSelection = false;

        }

        public void Clear()
        {
            txtHex.Text = "";
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            Show_Hex(e.NewValue);
        }

        private void Show_Hex(int pos)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = offset + pos * 16;

            txtHex.Text = "Offset         00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F\r\n\r\n";

            for (int i = 0; i < txtHex.Height / 17; i++)
            {
                if (br.BaseStream.Length == br.BaseStream.Position ||
                    br.BaseStream.Position >= offset + size)
                    break;

                string text, ascii; text = ascii = "";

                for (int j = 0; j < 0x10; j++)
                {
                    if (br.BaseStream.Position == offset + size)
                        break;

                    byte value = br.ReadByte();
                    string c = String.Format("{0:X}", value);
                    text += (c.Length == 2 ? c : '0' + c) + ' ';
                    ascii += (value > 0x1F && value < 0x7F ? Char.ConvertFromUtf32(value).ToString() + ' ' : ". ");
                }
                txtHex.Text += "0x" + String.Format("{0:X}", (i + pos) * 16).PadLeft(8, '0') + "     " + text.PadRight(52, ' ') + ascii + "\r\n";
                text = "";
                ascii = "";
            }

            br.Close();
            br.Dispose();
        }

        private void VisorHex_Resize(object sender, EventArgs e)
        {
            txtHex.Width = this.Width - 30;
            Show_Hex(vScrollBar1.Value);
        }
    }
}