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

        private int mouseWheelDelta;

        private Stream file;
        private uint offset;
        private uint size;

        public VisorHexBasic(string file, UInt32 offset, UInt32 size)
        {
            this.file = File.OpenRead(file);
            this.size = size;
            this.offset = offset;

            Initialize();
        }

        public VisorHexBasic(sFile gameFile)
        {
            this.file = File.OpenRead(gameFile.path);
            this.size = gameFile.size;
            this.offset = gameFile.offset;

            Initialize();
        }

        private void Initialize()
        {
            InitializeComponent();
            Text = Tools.Helper.GetTranslation("Sistema", "S41");
            FormClosed += (sender, e) => file.Close();
            
            txtHex.Font = new Font(FontFamily.GenericMonospace, 11F);
            txtHex.ReadOnly = true;
            txtHex.HideSelection = false;
            txtHex.MouseWheel += TxtHex_MouseWheel;
            txtHex.KeyDown += TxtHex_KeyDown;
            Resize += VisorHex_Resize;

            vScrollBar1.Maximum = (int)size / BytesPerRow;
            ShowHex(0);
        }

        public void Clear()
        {
            txtHex.Text = string.Empty;
        }


        private void TxtHex_KeyDown(object sender, KeyEventArgs e)
        {
            // Large scrolling with page down and page up.
            if (e.KeyCode == Keys.PageDown)
                UpdateScrollBar(vScrollBar1.Value + vScrollBar1.LargeChange);
            else if (e.KeyCode == Keys.PageUp)
                UpdateScrollBar(vScrollBar1.Value - vScrollBar1.LargeChange);

            // Small scrolling with up and down.
            if (e.KeyCode == Keys.Down) {
                int lastLineIdx = txtHex.GetCharIndexFromPosition(
                    new Point(0, txtHex.Height));
                
                if (txtHex.SelectionStart >= lastLineIdx) {
                    int currentPos = txtHex.SelectionStart; // Keep because it will change
                    UpdateScrollBar(vScrollBar1.Value + vScrollBar1.SmallChange);
                    txtHex.SelectionStart = currentPos;
                }
            } else if (e.KeyCode == Keys.Up) {
                int firstLineIdx = txtHex.GetCharIndexFromPosition(
                    new Point(txtHex.Width, 0));
                
                if (txtHex.SelectionStart <= firstLineIdx) {
                    int currentPos = txtHex.SelectionStart; // Keep because it will change
                    UpdateScrollBar(vScrollBar1.Value - vScrollBar1.SmallChange);
                    txtHex.SelectionStart = currentPos;
                }
            }
        }

        private void TxtHex_MouseWheel(object sender, MouseEventArgs e)
        {
            // Because a mouse wheel notch could be less than 120, sum until it reachs
            // that value and then divide to get the number of notchs.
            // 120 has been the classic mouse wheel notch value but new mouse could have
            // more definition.
            const int WheelDelta = 120;
            mouseWheelDelta = e.Delta * -1; // Normally we go down when scrolling up
            if (Math.Abs(mouseWheelDelta) >= WheelDelta) {
                int notchs = mouseWheelDelta / WheelDelta;
                UpdateScrollBar(vScrollBar1.Value + vScrollBar1.SmallChange * notchs);
                mouseWheelDelta %= WheelDelta;
            }
        }

        private void UpdateScrollBar(int scrollValue)
        {
            // Safety check because we'll get an exception otherwise
            if (scrollValue < vScrollBar1.Minimum)
                scrollValue = vScrollBar1.Minimum;
            else if (scrollValue > vScrollBar1.Maximum)
                scrollValue = vScrollBar1.Maximum;

            // We can get some speed improvement to avoid reading file again.
            if (scrollValue != vScrollBar1.Value) {
                vScrollBar1.Value = scrollValue;
                ShowHex(vScrollBar1.Value);
            }
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            ShowHex(e.NewValue);
        }

        private void ShowHex(int pos)
        {
            BinaryReader br = new BinaryReader(file);
            file.Position = offset + pos * BytesPerRow;

            // Create the header
            StringBuilder hexBuilder = new StringBuilder();
            hexBuilder.Append("Offset".PadRight(13));
            for (int i = 0; i < BytesPerRow; i++)
                hexBuilder.AppendFormat(" {0:X2}", i);
            hexBuilder.AppendLine();
            hexBuilder.AppendLine();

            int numRows = txtHex.Height / txtHex.Font.Height - 2;
            bool eof = false;
            for (int r = 0; r < numRows && !eof; r++) {
                hexBuilder.AppendFormat("0x{0:X8}   ", (pos + r) * BytesPerRow);

                var asciiBuilder = new StringBuilder("   ");
                for (int c = 0; c < BytesPerRow && !eof; c++) {
                    if (file.Position >= offset + size) {
                        eof = true;
                        break;
                    }

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

            txtHex.Text = hexBuilder.ToString();
            txtHex.Select(0, 0);
        }

        private void VisorHex_Resize(object sender, EventArgs e)
        {
            ShowHex(vScrollBar1.Value);
        }
    }
}