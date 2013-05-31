/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 * By: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ekona;

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
        const int MAX_WIDTH = 260;
        bool inversePalette = true;


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
            this.palette = NFTR.CalculatePalette(font.plgc.depth, inversePalette);

            for (int i = 0; i < font.plgc.tiles.Length; i++)
                comboChar.Items.Add("Char " + i.ToString());

            picFont.Image = NFTR.Get_Chars(font, MAX_WIDTH, palette, ZOOM);

            Fill_CharTile();

            comboChar.SelectedIndex = 0;
            comboEncoding.SelectedIndex = font.fnif.encoding;
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
                btnAddChar.Text = xml.Element("S03").Value;
                btnRemoveChar.Text = xml.Element("S04").Value;
                btnPalette.Text = xml.Element("S05").Value;
                btnChangeMap.Text = xml.Element("S06").Value;
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
                    {
                        if (type1.char_code[j] == 0xFFFF)
                            continue;

                        try { charTile.Add(font.pamc[p].first_char + j, type1.char_code[j]); }
                        catch { }
                    }
                }
                else if (font.pamc[p].info is sNFTR.PAMC.Type2)
                {
                    sNFTR.PAMC.Type2 type2 = (sNFTR.PAMC.Type2)font.pamc[p].info;

                    for (int j = 0; j < type2.num_chars; j++)
                    {
                        if (type2.charInfo[j].chars == 0xFFFF)
                            continue;
                        try { charTile.Add(type2.charInfo[j].chars_code, type2.charInfo[j].chars); }
                        catch { }
                    }
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
            font.fnif.encoding = (byte)comboEncoding.SelectedIndex;
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
                if (comboEncoding.SelectedIndex == 2)
                    codes = Encoding.GetEncoding("shift_jis").GetBytes(new char[] { txtBox.Text[i] }).Reverse().ToArray();
                else if (comboEncoding.SelectedIndex == 1)
                    codes = Encoding.GetEncoding("utf-16").GetBytes(new char[] { txtBox.Text[i] });
                else if (comboEncoding.SelectedIndex == 0)
                    codes = Encoding.UTF8.GetBytes(new char[] { txtBox.Text[i] });
                else if (comboEncoding.SelectedIndex == 3)
                    codes = Encoding.GetEncoding(1252).GetBytes(new char[] { txtBox.Text[i] });

                int charCode = (codes.Length == 2 ? BitConverter.ToUInt16(codes, 0) : codes[0]);

                int tileCode;
                if (!charTile.TryGetValue(charCode, out tileCode))
                {
                    width += font.plgc.tile_width;
                    continue;
                }

                width += font.hdwc.info[tileCode].pixel_start;
                Bitmap charImage = NFTR.Get_Char(font, tileCode, palette);
                charImage.MakeTransparent(this.palette[0]);
                graphic.DrawImageUnscaled(charImage, width, height);
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
            picFont.Image = NFTR.Get_Chars(font, MAX_WIDTH, palette, ZOOM);
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
            int charX = e.X / (font.plgc.tile_width * ZOOM + NFTR.BORDER_WIDTH);
            int charY = e.Y / (font.plgc.tile_height * ZOOM + NFTR.BORDER_WIDTH);
            int totalX = MAX_WIDTH / (font.plgc.tile_width * ZOOM + 2 * NFTR.BORDER_WIDTH);

            int index = charX + charY * totalX;
            if (index < comboChar.Items.Count)
                comboChar.SelectedIndex = index;
        }

        private void btnPalette_Click(object sender, EventArgs e)
        {
            if (pluginHost.Get_Palette().Loaded)
                palette = pluginHost.Get_Palette().Palette[0];

            int depth = Convert.ToByte(new String('1', font.plgc.depth), 2);
            Color[] palette2 = new System.Drawing.Color[depth + 1];
            Array.Copy(palette, 0, palette2, 0, palette2.Length);
            palette2 = palette2.Reverse().ToArray();
            palette = palette2;

            picFont.Image = NFTR.Get_Chars(font, MAX_WIDTH, palette);
            txtBox_TextChanged(txtBox, null);
        }

        private void btnChangeMap_Click(object sender, EventArgs e)
        {
            MapChar map = new MapChar(font.pamc, pluginHost.Get_Language());
            map.ShowDialog();
            if (map.DialogResult != DialogResult.OK)
                return;

            font.pamc = map.Maps;
            for (int i = 0; i < font.pamc.Count; i++)
            {
                if (font.pamc[i].info is sNFTR.PAMC.Type2)
                {
                    sNFTR.PAMC sec = font.pamc[i];

                    List<sNFTR.PAMC.Type2.CharInfo> infos = new List<sNFTR.PAMC.Type2.CharInfo>();
                    sNFTR.PAMC.Type2 type2 = (sNFTR.PAMC.Type2)sec.info;
                    infos.AddRange(type2.charInfo);
                    infos.Sort(Sort_Font);

                    type2.charInfo = infos.ToArray();
                    sec.info = type2;
                    font.pamc[i] = sec;
                }
            }

            charTile.Clear();
            Fill_CharTile();
            txtBox_TextChanged(txtBox, null);
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

            picFont.Image = NFTR.Get_Chars(font, MAX_WIDTH, palette, ZOOM);
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

            picFont.Image = NFTR.Get_Chars(font, MAX_WIDTH, palette, ZOOM);
            comboChar.Items.RemoveAt(index);
            comboChar.SelectedIndex = index - 1;
        }

        private int Sort_Font(sNFTR.PAMC.Type2.CharInfo c1, sNFTR.PAMC.Type2.CharInfo c2)
        {
            if (c1.chars_code < c2.chars_code)
                return -1;
            else if (c1.chars_code > c2.chars_code)
                return 1;
            else
                return 0;
        }

        private void btnExportInfo_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.DefaultExt = ".xml";
            if (o.ShowDialog() != DialogResult.OK)
                return;

            NFTR.ExportInfo(o.FileName, charTile, font);
        }

        private void btnToImage_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog o = new SaveFileDialog())
            {
                o.Filter = "Portable Network Graphic (*.png)|*.png";

                if (o.ShowDialog() == DialogResult.OK)
                    NFTR.ToImage(this.font, palette).Save(o.FileName);
            }
        }
        private void btnFromImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog o = new OpenFileDialog())
            {
                o.Filter = "Portable Network Graphic (*.png)|*.png";

                if (o.ShowDialog() == DialogResult.OK)
                    NFTR.FromImage((Bitmap)Image.FromFile(o.FileName), this.font, this.palette);
            }

            // Update
            picFont.Image = NFTR.Get_Chars(font, MAX_WIDTH, palette, ZOOM);
            txtBox_TextChanged(txtBox, null);
        }

        private void btnInversePalette_Click(object sender, EventArgs e)
        {
            inversePalette = !inversePalette;
            this.palette = NFTR.CalculatePalette(this.font.plgc.depth, inversePalette);

            // Update
            picFont.Image = NFTR.Get_Chars(font, MAX_WIDTH, palette, ZOOM);
            txtBox_TextChanged(txtBox, null);
        }
    }
}
