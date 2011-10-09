using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace Fonts
{
    public partial class FontControl : UserControl
    {
        IPluginHost pluginHost;
        sNFTR font;
        List<CharControl> chars = new List<CharControl>();
        Dictionary<int, int> charTile;
        Color[] palette;
        const int ZOOM = 2;

        public FontControl()
        {
            InitializeComponent();
        }
        public FontControl(IPluginHost pluginHost, sNFTR font)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.font = font;
            ReadLanguage();
            this.palette = CalculatePalette();

            for (int i = 0; i < font.plgc.tiles.Length; i++)
                comboChar.Items.Add("Char " + i.ToString());

            picFont.Image = NFTR.Get_Chars(font, 250, palette, ZOOM);

            Fill_CharTile();

            comboChar.SelectedIndex = 0;
            comboEncoding.SelectedIndex = 0;
        }

        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + System.IO.Path.DirectorySeparatorChar +
                    "Plugins" + System.IO.Path.DirectorySeparatorChar + "FontLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("FontControl");

                btnSave.Text = xml.Element("S00").Value;
                btnApply.Text = xml.Element("S01").Value;
                label1.Text = xml.Element("S02").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }
        private void Fill_CharTile()
        {
            charTile = new Dictionary<int, int>();
            for (int p = 0; p < font.pamc.Count; p++)
            {
                if (font.pamc[p].info is sNFTR.PAMC.Type0)
                {
                    sNFTR.PAMC.Type0 type0 = (sNFTR.PAMC.Type0)font.pamc[p].info;
                    int interval = font.pamc[p].last_char - font.pamc[p].first_char;

                    for (int j = 0; j <= interval; j++)
                        try { charTile.Add(font.pamc[p].first_char + j, type0.fist_char_code + j); }
                        catch { }
                }
                else if (font.pamc[p].info is sNFTR.PAMC.Type1)
                {
                    sNFTR.PAMC.Type1 type1 = (sNFTR.PAMC.Type1)font.pamc[p].info;

                    for (int j = 0; j < type1.char_code.Length; j++)
                        try { charTile.Add(font.pamc[p].first_char + j, type1.char_code[j]); }
                        catch { }
                }
                else if (font.pamc[p].info is sNFTR.PAMC.Type2)
                {
                    sNFTR.PAMC.Type2 type2 = (sNFTR.PAMC.Type2)font.pamc[p].info;

                    for (int j = 0; j < type2.num_chars; j++)
                        try { charTile.Add(type2.chars_code[j], type2.chars[j]); }
                        catch { }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelCharEdit.Controls.Clear();
            panelCharEdit.Controls.Add(new CharControl(
                pluginHost.Get_Language(),
                (from k in charTile where int.Equals(k.Value, comboChar.SelectedIndex) select k.Key).FirstOrDefault(),
                font.hdwc.info[comboChar.SelectedIndex],
                font.plgc.tiles[comboChar.SelectedIndex],
                font.plgc.depth,
                font.plgc.tile_width,
                font.plgc.tile_height,
                font.plgc.rotateMode,
                palette));
        }

        private void comboEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtBox_TextChanged(txtBox, null);
        }
        private void txtBox_TextChanged(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(picText.Width, picText.Height);
            Graphics graphic = Graphics.FromImage(image);
            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            int width = 0;
            int height = 0;
            for (int i = 0; i < txtBox.Text.Length; i++)
            {
                if (txtBox.Text[i] == '\n')
                {
                    width = 0;
                    height += font.plgc.tile_height;
                    continue;
                }

                byte[] codes = new byte[1];
                if (comboEncoding.SelectedIndex == 0)
                    codes = Encoding.GetEncoding("shift-jis").GetBytes(new char[] { txtBox.Text[i] }).Reverse().ToArray();
                else if (comboEncoding.SelectedIndex == 1)
                    codes = Encoding.Unicode.GetBytes(new char[] { txtBox.Text[i] });
                else if (comboEncoding.SelectedIndex == 2)
                    codes = Encoding.BigEndianUnicode.GetBytes(new char[] { txtBox.Text[i] });

                int charCode = (codes.Length == 2 ? BitConverter.ToUInt16(codes, 0) : codes[0]);

                int tileCode;
                if (!charTile.TryGetValue(charCode, out tileCode))
                {
                    width += font.plgc.tile_width;
                    continue;
                }

                width += font.hdwc.info[tileCode].pixel_start;
                graphic.DrawImageUnscaled(NFTR.Get_Char(font, tileCode, palette), width, height);
                width += font.hdwc.info[tileCode].pixel_length - font.hdwc.info[tileCode].pixel_start;

                if (width + font.plgc.tile_width > image.Width)
                {
                    width = 0;
                    height += font.plgc.tile_height;
                }
            }

            picText.Image = image;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            CharControl charControl = (CharControl)panelCharEdit.Controls[0];
            font.hdwc.info[comboChar.SelectedIndex] = charControl.TileInfo;
            font.plgc.tiles[comboChar.SelectedIndex] = charControl.Tiles;
            picFont.Image = NFTR.Get_Chars(font, 250, palette, ZOOM);
            txtBox_TextChanged(txtBox, null);
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            String fontFile = System.IO.Path.GetTempFileName();
            NFTR.Write(font, fontFile);
            pluginHost.ChangeFile(font.id, fontFile);
        }

        private void picFont_MouseClick(object sender, MouseEventArgs e)
        {
            int charX = e.X / (font.plgc.tile_width * ZOOM);
            int charY = e.Y / (font.plgc.tile_height * ZOOM);
            int totalX = 250 / (font.plgc.tile_width * ZOOM);

            int index = charX + charY * totalX;
            if (index < comboChar.Items.Count)
                comboChar.SelectedIndex = index;
        }

        private void btnPalette_Click(object sender, EventArgs e)
        {
            if (pluginHost.Get_NCLR().header.file_size != 0x00)
                palette = pluginHost.Get_NCLR().pltt.palettes[0].colors;

            int depth = Convert.ToByte(new String('1', font.plgc.depth), 2);
            Color[] palette2 = new System.Drawing.Color[depth + 1];
            Array.Copy(palette, 0, palette2, 0, palette2.Length);
            palette2 = palette2.Reverse().ToArray();
            palette = palette2;

            picFont.Image = NFTR.Get_Chars(font, 250, palette);
            txtBox_TextChanged(txtBox, null);
        }
        private Color[] CalculatePalette()
        {
            Color[] palette = new Color[(int)Math.Pow(2, font.plgc.depth)];
            for (int i = 0; i < palette.Length; i++)
            {
                int colorIndex = 255 - (i * (255 / (palette.Length - 1)));
                palette[i] = Color.FromArgb(colorIndex, 0, 0, 0);
                //palette[i] = Color.FromArgb(255, colorIndex, colorIndex, colorIndex);
            }
            palette = palette.Reverse().ToArray();
            return palette;
        }

        private void btnChangeMap_Click(object sender, EventArgs e)
        {
            MapChar map = new MapChar(font.pamc);
            map.ShowDialog();
            if (map.DialogResult == DialogResult.OK)
            {
                font.pamc = map.Maps;
                charTile.Clear();
                Fill_CharTile();
                txtBox_TextChanged(txtBox, null);
            }
        }
        private void btnAddChar_Click(object sender, EventArgs e)
        {
            font.hdwc.last_code++;
            sNFTR.HDWC.Info newInfo = new sNFTR.HDWC.Info();
            newInfo.pixel_start = 0;
            newInfo.pixel_width = 9;
            newInfo.pixel_length = 9;
            font.hdwc.info.Add(newInfo);

            List<byte[]> tiles = new List<byte[]>();
            tiles.AddRange(font.plgc.tiles);
            Byte[] newChar = new byte[8 * font.plgc.tile_length];
            tiles.Add(newChar);
            font.plgc.tiles = tiles.ToArray();

            picFont.Image = NFTR.Get_Chars(font, 250, palette, ZOOM);
            comboChar.Items.Add("Char" + comboChar.Items.Count.ToString());
            comboChar.SelectedIndex = comboChar.Items.Count - 1;

        }
        private void btnRemoveChar_Click(object sender, EventArgs e)
        {
            int index = comboChar.SelectedIndex;

            font.hdwc.last_code--;
            font.hdwc.info.RemoveAt(index);

            List<byte[]> tiles = new List<byte[]>();
            tiles.AddRange(font.plgc.tiles);
            tiles.RemoveAt(index);
            font.plgc.tiles = tiles.ToArray();

            picFont.Image = NFTR.Get_Chars(font, 250, palette, ZOOM);
            comboChar.Items.RemoveAt(index);
            comboChar.SelectedIndex = index - 1;
        }

    }
}
