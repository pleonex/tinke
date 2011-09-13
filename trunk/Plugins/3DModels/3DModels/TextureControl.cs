using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using PluginInterface;

namespace _3DModels
{
    public partial class TextureControl : UserControl
    {
        IPluginHost pluginHost;
        sBTX0 btx0;
        static int[] PaletteSize = new int[] { 0x00, 0x200, 0x08, 0x20, 0x200, 0x00, 0x200, 0x00 };

        public TextureControl()
        {
            InitializeComponent();
        }
        public TextureControl(IPluginHost pluginHost, sBTX0 btx0)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.btx0 = btx0;

            for (int i = 0; i < btx0.texture.texInfo.num_objs; i++)
                listTextures.Items.Add(String.Format("{0}: {1}", i.ToString(), btx0.texture.texInfo.names[i]));
            for (int i = 0; i < btx0.texture.palInfo.num_objs; i++)
                listPalettes.Items.Add(String.Format("{0}: {1}", i.ToString(), btx0.texture.palInfo.names[i]));

            listTextures.SelectedIndex = 0;
            listPalettes.SelectedIndex = 0;

            listTextures.SelectedIndexChanged += new EventHandler(listTextures_SelectedIndexChanged);
            listPalettes.SelectedIndexChanged += new EventHandler(listPalettes_SelectedIndexChanged);

            UpdateTexture(0, 0);
        }

        private void UpdateTexture(int num_tex)
        {
            sBTX0.Texture.TextInfo texInfo = (sBTX0.Texture.TextInfo)btx0.texture.texInfo.infoBlock.infoData[num_tex];
            sBTX0.Texture.PalInfo palInfo = (sBTX0.Texture.PalInfo)btx0.texture.palInfo.infoBlock.infoData[texInfo.paletteID];

            BinaryReader br = new BinaryReader(File.OpenRead(btx0.file));
            br.BaseStream.Position = texInfo.tex_offset * 8 + btx0.header.offset[0] + btx0.texture.header.textData_offset;
            Byte[] tile_data = br.ReadBytes((int)(texInfo.width * texInfo.height * texInfo.depth / 8));

            br.BaseStream.Position = btx0.header.offset[0] + btx0.texture.header.paletteData_offset;
            br.BaseStream.Position += (texInfo.format == 2 ? palInfo.palette_offset * 8 : palInfo.palette_offset * 16);
            Byte[] palette_data = br.ReadBytes((int)PaletteSize[texInfo.format]);
            Color[] palette = pluginHost.BGR555(palette_data);
            br.Close();

            picTex.Image = Draw_Texture(tile_data, texInfo, palette);

            NCLR nclr = new NCLR();
            nclr.pltt.nColores = (uint)palette.Length;
            nclr.pltt.paletas = new NTFP[1];
            nclr.pltt.paletas[0].colores = palette;
            picPalette.Image = pluginHost.Bitmaps_NCLR(nclr)[0];

            Info(num_tex);
        }
        private void UpdateTexture(int num_tex, int num_pal)
        {
            sBTX0.Texture.TextInfo texInfo = (sBTX0.Texture.TextInfo)btx0.texture.texInfo.infoBlock.infoData[num_tex];
            sBTX0.Texture.PalInfo palInfo = (sBTX0.Texture.PalInfo)btx0.texture.palInfo.infoBlock.infoData[num_pal];

            BinaryReader br = new BinaryReader(File.OpenRead(btx0.file));
            br.BaseStream.Position = texInfo.tex_offset * 8 + btx0.header.offset[0] + btx0.texture.header.textData_offset;
            Byte[] tile_data = br.ReadBytes((int)(texInfo.width * texInfo.height * texInfo.depth / 8));

            br.BaseStream.Position = btx0.header.offset[0] + btx0.texture.header.paletteData_offset;
            br.BaseStream.Position += (texInfo.format == 2 ? palInfo.palette_offset * 8 : palInfo.palette_offset * 16);
            Byte[] palette_data = br.ReadBytes((int)PaletteSize[texInfo.format]);
            Color[] palette = pluginHost.BGR555(palette_data);
            br.Close();

            picTex.Image = Draw_Texture(tile_data, texInfo, palette);

            NCLR nclr = new NCLR();
            nclr.pltt.nColores = (uint)palette.Length;
            nclr.pltt.paletas = new NTFP[1];
            nclr.pltt.paletas[0].colores = palette;
            picPalette.Image = pluginHost.Bitmaps_NCLR(nclr)[0];

            Info(num_tex);
        }


        private Bitmap Draw_Texture(byte[] data, sBTX0.Texture.TextInfo info, Color[] palette)
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
                        color = palette[data[w + h * info.width]];
                    else if (info.format == 1) // A3I5
                    {
                        int colorIndex = data[w + h * info.width] & 0x1F;
                        int alpha = (data[w + h * info.width] >> 5);
                        alpha = ((alpha * 4) + (alpha / 2)) * 8;
                        color = Color.FromArgb(alpha,
                            palette[colorIndex].R,
                            palette[colorIndex].G,
                            palette[colorIndex].B);
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

        private void Info(int num_tex)
        {
            for (int i = 0; i < listProp.Items.Count; i++)
                if (listProp.Items[i].SubItems.Count > 1)
                    listProp.Items[i].SubItems.RemoveAt(1);
            
            sBTX0.Texture.TextInfo texInfo = (sBTX0.Texture.TextInfo)btx0.texture.texInfo.infoBlock.infoData[num_tex];
            listProp.Items[0].SubItems.Add(texInfo.width.ToString());
            listProp.Items[1].SubItems.Add(texInfo.height.ToString());
            listProp.Items[2].SubItems.Add(texInfo.format.ToString());
            listProp.Items[3].SubItems.Add(texInfo.depth.ToString());
            listProp.Items[4].SubItems.Add(texInfo.paletteID.ToString());
            listProp.Items[5].SubItems.Add("0x" + texInfo.tex_offset.ToString("x"));
        }

        private void btnTransRemove_Click(object sender, EventArgs e)
        {
            panelTex.BackColor = Color.Transparent;
        }
        private void btnSetTransparent_Click(object sender, EventArgs e)
        {
            ColorDialog o = new ColorDialog();
            o.AllowFullOpen = true;
            o.AnyColor = true;
            if (o.ShowDialog() == DialogResult.OK)
                panelTex.BackColor = o.Color;
        }

        private void listTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTexture(listTextures.SelectedIndex, listPalettes.SelectedIndex);
        }
        private void listPalettes_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTexture(listTextures.SelectedIndex, listPalettes.SelectedIndex);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.DefaultExt = ".bmp";
            o.Filter = "BitMaP (*.bmp)|*.bmp";
            o.OverwritePrompt = true;
            if (o.ShowDialog() == DialogResult.OK)
                picTex.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
        }

    }
}
