using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace _3DModels
{
    public partial class TextureControl : UserControl
    {
        IPluginHost pluginHost;
        sBTX0 btx0;

        public TextureControl()
        {
            InitializeComponent();
        }
        public TextureControl(IPluginHost pluginHost, sBTX0 btx0)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.btx0 = btx0;

            UpdateTexture(0);
            numericTexture.Maximum = btx0.texture.texInfo.num_objs - 1;
            numericPalette.Maximum = btx0.texture.palInfo.num_objs - 1;
        }

        private void UpdateTexture(int num_tex)
        {
            sBTX0.Texture.TextInfo texInfo = (sBTX0.Texture.TextInfo)btx0.texture.texInfo.infoBlock.infoData[num_tex];

            NCLR palette = new NCLR();
            palette.cabecera.id = "BTX0".ToCharArray();
            if (texInfo.paletteID >= btx0.texture.palette_data.Length)
                texInfo.paletteID = 0;
            palette.cabecera.file_size = (uint)btx0.texture.palette_data[texInfo.paletteID].Length;
            palette.pltt.nColores = 0x10;
            palette.pltt.paletas = new NTFP[1];
            palette.pltt.paletas[0].colores = pluginHost.BGR555(btx0.texture.palette_data[texInfo.paletteID]);

            picTex.Image = Draw_Texture(btx0.texture.texture_data[num_tex], texInfo, palette);
            picPalette.Image = pluginHost.Bitmaps_NCLR(palette)[0];
            lblTexName.Text = btx0.texture.texInfo.names[num_tex];
            lblPalName.Text = btx0.texture.palInfo.names[texInfo.paletteID];
            numericPalette.Value = texInfo.paletteID;
        }
        private Bitmap Draw_Texture(byte[] data, sBTX0.Texture.TextInfo info, NCLR palette)
        {
            Bitmap imagen = new Bitmap(info.width, info.height);
            if (info.format == 3) // 16-color 4 bits
                data = pluginHost.Bit8ToBit4(data);
            else if (info.format == 2) // 4-color 2 bits
                data = Bit8ToBit2(data);

            for (int h = 0; h < info.height; h++)
            {
                for (int w = 0; w < info.width; w++)
                {
                    Color color = Color.Black; ;
                    if (info.format == 2 || info.format == 3 || info.format == 4)
                        color = palette.pltt.paletas[0].colores[data[w + h * info.width]];
                    else if (info.format == 1)
                    {
                        int colorIndex = data[w + h * info.width] & 0x1F;
                        int alpha = (data[w + h * info.width] >> 5) * 255 / 7;
                        color = Color.FromArgb(alpha,
                            palette.pltt.paletas[0].colores[colorIndex].R,
                            palette.pltt.paletas[0].colores[colorIndex].G,
                            palette.pltt.paletas[0].colores[colorIndex].B);
                    }

                    imagen.SetPixel(w, h, color);
                }
            }
            return imagen;
        }
        private byte[] Bit8ToBit2(byte[] data)
        {
            List<Byte> bit2 = new List<byte>();

            for (int i = 0; i < data.Length; i++)
            {
                bit2.Add((byte)(data[i] & 0x3));
                bit2.Add((byte)((data[i] >> 2) & 0x3));
                bit2.Add((byte)((data[i] >> 4) & 0x3));
                bit2.Add((byte)((data[i] >> 6) & 0x3));
            }

            return bit2.ToArray();
        }
        private void UpdatePalette(int num_pal)
        {
            NCLR palette = new NCLR();
            palette.cabecera.id = "BTX0".ToCharArray();
            palette.cabecera.file_size = (uint)btx0.texture.palette_data[num_pal].Length;
            palette.pltt.nColores = 0x10;
            palette.pltt.paletas = new NTFP[1];
            palette.pltt.paletas[0].colores = pluginHost.BGR555(btx0.texture.palette_data[num_pal]);

            picPalette.Image = pluginHost.Bitmaps_NCLR(palette)[0];
            lblPalName.Text = btx0.texture.palInfo.names[num_pal];
        }

        private void numericTexture_ValueChanged(object sender, EventArgs e)
        {
            UpdateTexture((int)numericTexture.Value);
        }
        private void numericPalette_ValueChanged(object sender, EventArgs e)
        {
            UpdatePalette((int)numericPalette.Value);
        }

    }
}
