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

        public FontControl()
        {
            InitializeComponent();
        }
        public FontControl(IPluginHost pluginHost, sNFTR font)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.font = font;

            for (int i = 0; i < font.plgc.tiles.Length; i++)
                comboBox1.Items.Add("Char " + i.ToString());

            picFont.Image = NFTR.Get_Chars(font, 250);
            picChar.Size = new System.Drawing.Size(font.plgc.tile_width * 10, font.plgc.tile_height * 10);

            //Create_Controls();
            Fill_CharTile();
        }

        private void Create_Controls()
        {
            chars.Clear();

            for (int i = 0; i < font.plgc.tiles.Length; i++)
            {
                CharControl control = new CharControl(
                     CharView.Image,
                     (from k in charTile where int.Equals(k.Value, i) select k.Key).FirstOrDefault(),
                     font.hdwc.info[i],
                     font.plgc.tiles[i],
                     font.plgc.depth,
                     font.plgc.tile_width,
                     font.plgc.tile_height);
                chars.Add(control);
            }
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

                    for (int j = 0; j < interval; j++)
                        charTile.Add(font.pamc[p].first_char + j, type0.fist_char_code + j);
                }
                else if (font.pamc[p].info is sNFTR.PAMC.Type1)
                {
                    sNFTR.PAMC.Type1 type1 = (sNFTR.PAMC.Type1)font.pamc[p].info;

                    for (int j = 0; j < type1.char_code.Length; j++)
                        charTile.Add(font.pamc[p].first_char + j, type1.char_code[j]);
                }
                else if (font.pamc[p].info is sNFTR.PAMC.Type2)
                {
                    sNFTR.PAMC.Type2 type2 = (sNFTR.PAMC.Type2)font.pamc[p].info;

                    for (int j = 0; j < type2.num_chars; j++)
                        charTile.Add(type2.chars_code[j], type2.chars[j]);
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            picChar.Image = NFTR.Get_Char(font, comboBox1.SelectedIndex, 10);
        }

        private void txtBox_TextChanged(object sender, EventArgs e)
        {
            Bitmap image = new Bitmap(picText.Width, picText.Height);
            Graphics graphic = Graphics.FromImage(image);

            int width = 0;
            int height = 0;
            for (int i = 0; i < txtBox.Text.Length; i++)
            {
                byte[] codes = Encoding.GetEncoding("shift-jis").GetBytes( new char[] { txtBox.Text[i] } ).Reverse().ToArray();
                int charCode = (codes.Length == 2 ? BitConverter.ToUInt16(codes, 0) : codes[0]);

                int tileCode;
                if (!charTile.TryGetValue(charCode, out tileCode))
                {
                    width += font.plgc.tile_width;
                    continue;
                }

                width += font.hdwc.info[tileCode].pixel_start;
                graphic.DrawImageUnscaled(NFTR.Get_Char(font, tileCode), width, height);
                width += font.hdwc.info[tileCode].pixel_length - font.hdwc.info[tileCode].pixel_start;

                if (width + font.plgc.tile_width > image.Width)
                {
                    width = 0;
                    height += font.plgc.tile_height;
                }
            }

            picText.Image = image;
        }

    }
}
