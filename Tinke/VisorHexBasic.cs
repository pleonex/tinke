//
//  VisorHexBasic.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2016 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Windows.Forms;
using Ekona;
using System.Drawing;
using System.Text;

namespace Tinke
{
    public partial class VisorHexBasic : Form
    {
        private const int BytesPerRow = 0x10;   // Not tested with different values.
        private string file;
        private uint offset;
        private uint size;

        public VisorHexBasic(string file, UInt32 offset, UInt32 size)
        {
            this.file = file;
            this.size = size;
            this.offset = offset;

            Initialize();
        }

        public VisorHexBasic(sFile gameFile)
        {
            this.file = gameFile.path;
            this.size = gameFile.size;
            this.offset = gameFile.offset;

            Initialize();
        }

        private void Initialize()
        {
            InitializeComponent();
            Text = Tools.Helper.GetTranslation("Sistema", "S41");
            
            txtHex.Font = new Font(FontFamily.GenericMonospace, 11F);
            txtHex.ReadOnly = true;
            txtHex.HideSelection = false;
            //txtHex.MouseWheel += (sender, e) => 
            //    vScrollBar1.Value += vScrollBar1.SmallChange * e.Delta;
            Resize += VisorHex_Resize;

            vScrollBar1.Maximum = (int)size / BytesPerRow;
            ShowHex(0);
        }

        public void Clear()
        {
            txtHex.Text = string.Empty;
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ShowHex(e.NewValue);
        }

        private void ShowHex(int pos)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = offset + pos * BytesPerRow;

            // Create the header
            StringBuilder hexBuilder = new StringBuilder();
            hexBuilder.Append("Offset".PadRight(13));
            for (int i = 0; i < BytesPerRow; i++)
                hexBuilder.AppendFormat(" {0:X2}", i);
            hexBuilder.AppendLine();
            hexBuilder.AppendLine();

            int numRows = txtHex.Height / txtHex.Font.Height - 2;
            for (int r = 0; r < numRows; r++) {
                hexBuilder.AppendFormat("0x{0:X8}   ", (pos + r) * BytesPerRow);

                var asciiBuilder = new StringBuilder("   ");
                for (int c = 0; c < BytesPerRow; c++) {
                    if (br.BaseStream.Position >= offset + size)
                        break;

                    byte value = br.ReadByte();
                    hexBuilder.AppendFormat(" {0:X2}", value);
                    if (value > 0x1F && value < 0x7F)
                        asciiBuilder.Append(" " + (char)value);
                    else
                        asciiBuilder.Append(" .");
                }

                hexBuilder.Append(asciiBuilder.ToString());
                if (r != numRows - 1)
                    hexBuilder.AppendLine();
            }

            br.Close();

            txtHex.Text = hexBuilder.ToString();
            txtHex.Select(0, 0);
        }

        private void VisorHex_Resize(object sender, EventArgs e)
        {
            ShowHex(vScrollBar1.Value);
        }
    }
}