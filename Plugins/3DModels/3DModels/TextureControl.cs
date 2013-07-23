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
 * Programador: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Ekona;
using Ekona.Images;

namespace _3DModels
{
    public partial class TextureControl : UserControl
    {
        IPluginHost pluginHost;
        sBTX0 btx0;
        //                                       0     1      2     3     4      5     6     7
        static int[] PaletteSize = new int[] { 0x00, 0x40, 0x08, 0x20, 0x200, 0x200, 0x10, 0x00 };

        public TextureControl()
        {
            InitializeComponent();
        }
        public TextureControl(IPluginHost pluginHost, sBTX0 btx0)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.btx0 = btx0;
            ReadLanguage();

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
        public TextureControl(IPluginHost pluginHost, sBTX0.Texture tex, uint texOffset, string filePath)
        {
            InitializeComponent();

            sBTX0 btx0 = new sBTX0();
            btx0.texture = tex;
            btx0.file = filePath;
            btx0.header.offset = new uint[1];
            btx0.header.offset[0] = texOffset;

            this.pluginHost = pluginHost;
            this.btx0 = btx0;
            ReadLanguage();

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

        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                    "Plugins" + Path.DirectorySeparatorChar + "3DModelsLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("TextureControl");

                columnProperty.Text = xml.Element("S00").Value;
                columnValue.Text = xml.Element("S01").Value;
                listProp.Items[0].Text = xml.Element("S02").Value;
                listProp.Items[0].SubItems[1].Text = xml.Element("S03").Value;
                listProp.Items[1].Text = xml.Element("S04").Value;
                listProp.Items[2].Text = xml.Element("S05").Value;
                listProp.Items[3].Text = xml.Element("S06").Value;
                listProp.Items[4].Text = xml.Element("S07").Value;
                listProp.Items[5].Text = xml.Element("S08").Value;
                listProp.Items[6].Text = xml.Element("S09").Value;
                listProp.Items[7].Text = xml.Element("S0A").Value;
                listProp.Items[8].Text = xml.Element("S0B").Value;
                listProp.Items[9].Text = xml.Element("S0C").Value;
                listProp.Items[10].Text = xml.Element("S0D").Value;
                listProp.Items[11].Text = xml.Element("S02").Value;
                listProp.Items[11].SubItems[1].Text = xml.Element("S0E").Value;
                listProp.Items[12].Text = xml.Element("S04").Value;
                label2.Text = xml.Element("S0F").Value;
                label1.Text = xml.Element("S10").Value;
                label3.Text = xml.Element("S11").Value;
                btnSetTransparent.Text = xml.Element("S12").Value;
                btnSave.Text = xml.Element("S13").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }

        private void UpdateTexture(int num_tex)
        {
            int num_pal;
            bool foundPal = false;
            for (num_pal = 0; num_pal < btx0.texture.palInfo.num_objs; num_pal++)
            {
                if (btx0.texture.palInfo.names[num_pal] == btx0.texture.texInfo.names[num_tex])
                {
                    foundPal = true;
                    break;
                }
                else if (btx0.texture.palInfo.names[num_pal].Replace("_pl", "") == btx0.texture.texInfo.names[num_tex])
                {
                    foundPal = true;
                    break;
                }
            }
            if (!foundPal)
            {
                num_pal = listPalettes.SelectedIndex;
                UpdateTexture(num_tex, num_pal);
            }
            else
                listPalettes.SelectedIndex = num_pal;

        }
        private void UpdateTexture(int num_tex, int num_pal)
        {
            sBTX0.Texture.TextInfo texInfo = (sBTX0.Texture.TextInfo)btx0.texture.texInfo.infoBlock.infoData[num_tex];
            sBTX0.Texture.PalInfo palInfo = (sBTX0.Texture.PalInfo)btx0.texture.palInfo.infoBlock.infoData[num_pal];

            // Get texture data
            BinaryReader br = new BinaryReader(File.OpenRead(btx0.file));
            if (texInfo.format != 5)
                br.BaseStream.Position = texInfo.tex_offset * 8 + btx0.header.offset[0] + btx0.texture.header.textData_offset;
            else
                br.BaseStream.Position = btx0.header.offset[0] + btx0.texture.header.textCompressedData_offset + texInfo.tex_offset * 8;
            Byte[] tile_data = br.ReadBytes((int)(texInfo.width * texInfo.height * texInfo.depth / 8));

            // Get palette data
            br.BaseStream.Position = btx0.header.offset[0] + btx0.texture.header.paletteData_offset;
            br.BaseStream.Position += palInfo.palette_offset * 8;
            Byte[] palette_data = br.ReadBytes((int)PaletteSize[texInfo.format]);
            Color[] palette = Actions.BGR555ToColor(palette_data);
            br.Close();

            picTex.Image = Draw_Texture(tile_data, texInfo, palette);
            Clipboard.SetImage(picTex.Image);

            PaletteBase p = new RawPalette(new Color[][] { palette }, false, ColorFormat.colors256);
            picPalette.Image = p.Get_Image(0);
            pluginHost.Set_Palette(p);

            Info(num_tex, num_pal);
        }

        private Bitmap Draw_Texture(byte[] data, sBTX0.Texture.TextInfo info, Color[] palette)
        {
            if (info.format == 5)
                return Draw_CompressedTexture(data, info);

            Bitmap imagen = new Bitmap(info.width, info.height);
            if (info.format == 3) // 16-color 4 bits
                data = Ekona.Helper.BitsConverter.BytesToBit4(data);
            else if (info.format == 2) // 4-color 2 bits
                data = Bit8ToBit2(data);

            for (int h = 0; h < info.height; h++)
            {
                for (int w = 0; w < info.width; w++)
                {
                    Color color = Color.Black;
                    try
                    {
                        if (info.format == 2 || info.format == 3 || info.format == 4) // 2-4-8 bits per color
                            color = palette[data[w + h * info.width]];
                        else if (info.format == 1) // A3I5 8-bit
                        {
                            int colorIndex = data[w + h * info.width] & 0x1F;
                            int alpha = (data[w + h * info.width] >> 5);
                            alpha = ((alpha * 4) + (alpha / 2)) * 8;
                            color = Color.FromArgb(alpha,
                                palette[colorIndex].R,
                                palette[colorIndex].G,
                                palette[colorIndex].B);
                        }
                        else if (info.format == 6) // A5I3 8-bit
                        {
                            int colorIndex = data[w + h * info.width] & 0x7;
                            int alpha = (data[w + h * info.width] >> 3);
                            alpha *= 8;
                            color = Color.FromArgb(alpha,
                                palette[colorIndex].R,
                                palette[colorIndex].G,
                                palette[colorIndex].B);
                        }
                        else if (info.format == 7) // Direct texture 16-bit (not tested)
                        {
                            ushort byteColor = BitConverter.ToUInt16(data, (w + h * info.width) * 2);
                            color = Color.FromArgb(
                                (byteColor >> 15) * 255,
                                (byteColor & 0x1F) * 8,
                                ((byteColor >> 5) & 0x1F) * 8,
                                ((byteColor >> 10) & 0x1F) * 8);
                        }
                    }
                    catch { }

                    imagen.SetPixel(w, h, color);
                }
            }

            if (info.color0 == 1)
                imagen.MakeTransparent(palette[0]);

            return imagen;
        }
        private Bitmap Draw_CompressedTexture(byte[] data, sBTX0.Texture.TextInfo info)
        {
            sBTX0.Texture.PalInfo palInfo = (sBTX0.Texture.PalInfo)btx0.texture.palInfo.infoBlock.infoData[listPalettes.SelectedIndex];


            BinaryReader br = new BinaryReader(File.OpenRead(btx0.file));
            br.BaseStream.Position = btx0.header.offset[0] + btx0.texture.header.textCompressedInfoData_offset + info.compressedDataStart;

            Bitmap image = new Bitmap(info.width, info.height);

            for (int h = 0; h < info.height; h += 4)
            {
                for (int w = 0; w < info.width; w += 4)
                {
                    uint texData = BitConverter.ToUInt32(data, w + h * info.width / 4);

                    // Get palette data for this block
                    ushort pal_info = br.ReadUInt16();
                    int pal_offset = pal_info & 0x3FFF;
                    int pal_mode = (pal_info >> 14);

                    long currPos = br.BaseStream.Position;
                    br.BaseStream.Position = btx0.header.offset[0] + btx0.texture.header.paletteData_offset + palInfo.palette_offset * 8;
                    br.BaseStream.Position += pal_offset * 4;
                    if (br.BaseStream.Position >= br.BaseStream.Length)
                        br.BaseStream.Position -= pal_offset * 4;

                    Color[] palette = Actions.BGR555ToColor(br.ReadBytes(0x08));
                    br.BaseStream.Position = currPos;

                    for (int hTex = 0; hTex < 4; hTex++)
                    {
                        byte texel_row = (byte)((texData >> (hTex * 8)) & 0xFF);
                        for (int wTex = 0; wTex < 4; wTex++)
                        {
                            byte texel = (byte)((texel_row >> (wTex * 2)) & 0x3);

                            #region Get color from Texel and mode values
                            Color color = Color.Black;
                            if (palette.Length < 4 && pal_mode != 1 && pal_mode != 3)
                                goto Draw;

                            switch (pal_mode)
                            {
                                case 0:
                                    if (texel == 0) color = palette[0];
                                    else if (texel == 1) color = palette[1];
                                    else if (texel == 2) color = palette[2];
                                    else if (texel == 3) color = Color.FromArgb(0, 0, 0, 0);  // Transparent color
                                    break;

                                case 1:
                                    if (texel == 0) color = palette[0];
                                    else if (texel == 1) color = palette[1];
                                    else if (texel == 2) color = SumColors(palette[0], palette[1], 1, 1);
                                    else if (texel == 3) color = Color.FromArgb(0, 0, 0, 0); // Transparent color
                                    break;

                                case 2:
                                    if (texel == 0) color = palette[0];
                                    else if (texel == 1) color = palette[1];
                                    else if (texel == 2) color = palette[2];
                                    else if (texel == 3) color = palette[3];
                                    break;

                                case 3:
                                    if (texel == 0) color = palette[0];
                                    else if (texel == 1) color = palette[1];
                                    else if (texel == 2) color = SumColors(palette[0], palette[1], 5, 3);
                                    else if (texel == 3) color = SumColors(palette[0], palette[1], 3, 5);
                                    break;
                            }
                            #endregion

                        Draw:
                            image.SetPixel(
                                w + wTex,
                                h + hTex,
                                color);
                        }
                    }
                }
            }

            br.Close();
            return image;
        }
        private Color SumColors(Color a, Color b, int wa, int wb)
        {
            return Color.FromArgb(
                (a.R * wa + b.R * wb) / (wa + wb),
                (a.G * wa + b.G * wb) / (wa + wb),
                (a.B * wa + b.B * wb) / (wa + wb));
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

        private void Info(int num_tex, int num_pal)
        {
            for (int i = 1; i < 11; i++)
                if (listProp.Items[i].SubItems.Count > 1)
                    listProp.Items[i].SubItems.RemoveAt(1);
            if (listProp.Items[12].SubItems.Count > 1)
                listProp.Items[12].SubItems.RemoveAt(1);

            sBTX0.Texture.TextInfo texInfo = (sBTX0.Texture.TextInfo)btx0.texture.texInfo.infoBlock.infoData[num_tex];
            listProp.Items[1].SubItems.Add((texInfo.tex_offset * 8).ToString("x"));
            listProp.Items[2].SubItems.Add(texInfo.repeat_X.ToString());
            listProp.Items[3].SubItems.Add(texInfo.repeat_Y.ToString());
            listProp.Items[4].SubItems.Add(texInfo.flip_X.ToString());
            listProp.Items[5].SubItems.Add(texInfo.flip_Y.ToString());
            listProp.Items[6].SubItems.Add(texInfo.width.ToString());
            listProp.Items[7].SubItems.Add(texInfo.height.ToString());
            listProp.Items[8].SubItems.Add(texInfo.format.ToString() + " (" + (TextureFormat)texInfo.format + ')');
            listProp.Items[9].SubItems.Add(texInfo.color0.ToString());
            listProp.Items[10].SubItems.Add(texInfo.coord_transf.ToString() + " (" + (TextureCoordTransf)texInfo.coord_transf + ')');

            sBTX0.Texture.PalInfo palInfo = (sBTX0.Texture.PalInfo)btx0.texture.palInfo.infoBlock.infoData[num_pal];
            int palOffset = palInfo.palette_offset;
            palOffset *= (texInfo.format == 2 ? 8 : 16);
            listProp.Items[12].SubItems.Add(palOffset.ToString("x"));

            listProp.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void btnSetTransparent_Click(object sender, EventArgs e)
        {
            ColorDialog o = new ColorDialog();
            o.AllowFullOpen = true;
            o.AnyColor = true;
            o.Color = panelTex.BackColor;
            o.FullOpen = true;
            if (o.ShowDialog() == DialogResult.OK)
                panelTex.BackColor = o.Color;
        }

        private void listTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTexture(listTextures.SelectedIndex);
        }
        private void listPalettes_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTexture(listTextures.SelectedIndex, listPalettes.SelectedIndex);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.OverwritePrompt = true;
            o.FileName = btx0.texture.texInfo.names[listTextures.SelectedIndex] + ".png";
            o.DefaultExt = ".png";
            o.Filter = "Portable Network Graphic (*.png)|*.png|" +
                        "BitMaP (*.bmp)|*.bmp|" +
                       "JPEG (*.jpg)|*.jpg;*.jpeg|" +
                       "Tagged Image File Format (*.tiff)|*.tiff;*.tif|" +
                       "Graphic Interchange Format (*.gif)|*.gif|" +
                       "Icon (*.ico)|*.ico;*.icon";

            if (o.ShowDialog() == DialogResult.OK)
            {
                if (o.FilterIndex == 2)
                    picTex.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                else if (o.FilterIndex == 1)
                    picTex.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Png);
                else if (o.FilterIndex == 3)
                    picTex.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                else if (o.FilterIndex == 4)
                    picTex.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                else if (o.FilterIndex == 5)
                    picTex.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                else if (o.FilterIndex == 6)
                    picTex.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Icon);
            }
            o.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int num_tex = listTextures.SelectedIndex;
            int num_pal = listPalettes.SelectedIndex;

            sBTX0.Texture.TextInfo texInfo = (sBTX0.Texture.TextInfo)btx0.texture.texInfo.infoBlock.infoData[num_tex];
            sBTX0.Texture.PalInfo palInfo = (sBTX0.Texture.PalInfo)btx0.texture.palInfo.infoBlock.infoData[num_pal];

            // Get palette data
            BinaryReader br = new BinaryReader(File.OpenRead(btx0.file));
            br.BaseStream.Position = btx0.header.offset[0] + btx0.texture.header.paletteData_offset;
            br.BaseStream.Position += palInfo.palette_offset * 8;
            Byte[] palette_data = br.ReadBytes((int)PaletteSize[texInfo.format]);
            Color[] palette = Actions.BGR555ToColor(palette_data);
            br.Close();


            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".pal";
            o.Filter = "Windows Palette for Gimp 2.8 (*.pal)|*.pal|" +
                        "Windows Palette (*.pal)|*.pal|" +
                        "Portable Network Graphics (*.png)|*.png|" +
                        "Adobe COlor (*.aco)|*.aco";
            o.OverwritePrompt = true;

            if (o.ShowDialog() != DialogResult.OK)
                return;

            if (o.FilterIndex == 3)
            {
                RawPalette p = new RawPalette(palette, false, ColorFormat.colors256);
                p.Get_Image(0).Save(o.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
            else if (o.FilterIndex == 1 || o.FilterIndex == 2)
            {
                Ekona.Images.Formats.PaletteWin palwin = new Ekona.Images.Formats.PaletteWin(palette);
                if (o.FilterIndex == 1)
                    palwin.Gimp_Error = true;
                palwin.Write(o.FileName);
            }
            else if (o.FilterIndex == 4)
            {
                Ekona.Images.Formats.ACO palaco = new Ekona.Images.Formats.ACO(palette);
                palaco.Write(o.FileName);
            }

            o.Dispose();
        }

    }
}
