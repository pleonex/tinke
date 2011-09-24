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
using System.Windows.Forms;
using System.IO;
using System.Xml.Linq;
using PluginInterface;

namespace Images
{
    public partial class PaletteControl : UserControl
    {
        NCLR paleta;
        Bitmap[] paletas;
        IPluginHost pluginHost;

        Byte[] data;
        ColorDepth oldDepth;

        public PaletteControl()
        {
            InitializeComponent();
        }
        public PaletteControl(IPluginHost pluginHost)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            LeerIdioma();

            this.paleta = pluginHost.Get_NCLR();
            ShowInfo();

            paletas = pluginHost.Bitmaps_NCLR(paleta);
            paletaBox.Image = paletas[0];
            nPaleta.Maximum = paletas.Length;
            nPaleta.Minimum = 1;
            nPaleta.Value = 1;

            data = pluginHost.ColorToBGR555(paleta.pltt.palettes[0].colors);
            oldDepth = paleta.pltt.depth;
        }

        private void LeerIdioma()
        {
            try
            {
                XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" +
                    Path.DirectorySeparatorChar + "ImagesLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("PaletteControl");

                label1.Text = xml.Element("S01").Value;
                groupProp.Text = xml.Element("S02").Value;
                columnName.Text = xml.Element("S03").Value;
                columnValor.Text = xml.Element("S04").Value;
                listProp.Items[0].Text = xml.Element("S05").Value;
                listProp.Items[1].Text = xml.Element("S06").Value;
                listProp.Items[2].Text = xml.Element("S07").Value;
                listProp.Items[3].Text = xml.Element("S08").Value;
                listProp.Items[4].Text = xml.Element("S09").Value;
                btnSave.Text = xml.Element("S0A").Value;
                btnShow.Text = xml.Element("S0B").Value;
                groupModificar.Text = xml.Element("S11").Value;
                label2.Text = xml.Element("S12").Value;
                btnImport.Text = xml.Element("S13").Value;
                btnConverter.Text = xml.Element("S14").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); } 
        }

        private void ShowInfo()
        {
            if (listProp.Items[0].SubItems.Count == 2)
                for (int i = 0; i < listProp.Items.Count; i++)
                    listProp.Items[i].SubItems.RemoveAt(1);

            listProp.Items[0].SubItems.Add(paleta.pltt.palettes.Length.ToString());
            listProp.Items[1].SubItems.Add(paleta.pltt.depth == ColorDepth.Depth4Bit ?
                "4-bit" : "8-bit");
            listProp.Items[2].SubItems.Add("0x" + String.Format("{0:X}", paleta.pltt.unknown1));
            listProp.Items[3].SubItems.Add(paleta.pltt.nColors.ToString());
            listProp.Items[4].SubItems.Add(paleta.pltt.paletteLength.ToString());
        }

        private void nPaleta_ValueChanged(object sender, EventArgs e)
        {
            paletaBox.Image = paletas[(int)nPaleta.Value - 1];
            data = pluginHost.ColorToBGR555(paleta.pltt.palettes[(int)nPaleta.Value - 1].colors);
            numericStartByte.Value = 0;
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" +
                            Path.DirectorySeparatorChar + "ImagesLang.xml");
            xml = xml.Element(pluginHost.Get_Language()).Element("PaletteControl");
            string trad = xml.Element("S0C").Value;
            Form ven = new Form();
            int xMax = 6 * 170;
            int x = 0;
            int y = 15;

            for (int i = 0; i < paletas.Length; i++)
            {
                PictureBox pic = new PictureBox();
                pic.Size = new Size(160, 160);
                pic.Location = new Point(x, y);
                pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                pic.Image = paletas[i];
                Label lbl = new Label();
                lbl.Text = trad + ' ' + (i + 1).ToString();
                lbl.Location = new Point(x, y - 15);

                ven.Controls.Add(pic);
                ven.Controls.Add(lbl);

                x += 170;
                if (x >= xMax)
                {
                    x = 0;
                    y += 185;
                }
            }

            ven.Text = trad;
            ven.BackColor = SystemColors.GradientInactiveCaption;
            ven.MaximumSize = new Size(1024, 760);
            ven.ShowIcon = false;
            ven.AutoSize = true;
            ven.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ven.MaximizeBox = false;
            ven.Show();
        }
        private void paletaBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (paletaBox.Image is Image)
            {
                Color color = ((Bitmap)paletaBox.Image).GetPixel(e.X, e.Y);
                lblRGB.Text = "RGB: " + color.R + ", " + color.G + ", " + color.B;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".png";
            o.Filter = "Portable Network Graphics (*.png)|*.png|"
                + "Window palette (*.pal)|*.pal";
            o.OverwritePrompt = true;

            if (o.ShowDialog() != DialogResult.OK)
                return;

            if (o.FilterIndex == 1)
                paletaBox.Image.Save(o.FileName);
            else if (o.FilterIndex == 2)
                Write_WinPal(o.FileName, paleta);
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.DefaultExt = "bmp";
            o.Filter = "BitMaP (*.bmp)|*.bmp|" +
                "Windows palette (*.pal)|*.pal";
            o.Multiselect = false;
            if (o.ShowDialog() == DialogResult.OK)
            {
                String paletteFile = System.IO.Path.GetTempFileName() + '.' + new String(paleta.header.id);
                NCLR newPalette;

                if (o.FilterIndex == 1)
                {
                    newPalette = pluginHost.BitmapToPalette(o.FileName);
                    newPalette.id = paleta.id;
                    newPalette.header.id = paleta.header.id;
                    paleta = newPalette;
                }
                else if (o.FilterIndex == 2)
                {
                    newPalette = Read_WinPal(o.FileName, paleta.pltt.depth);
                    newPalette.id = paleta.id;
                    newPalette.header.id = paleta.header.id;
                    paleta = newPalette;
                }

                pluginHost.Set_NCLR(paleta);
                Write_Palette(paletteFile, paleta, pluginHost);
                pluginHost.ChangeFile((int)paleta.id, paletteFile);

                ShowInfo();
                paletas = pluginHost.Bitmaps_NCLR(paleta);
                paletaBox.Image = paletas[0];
                nPaleta.Maximum = paletas.Length;
                nPaleta.Minimum = 1;
                nPaleta.Value = 1;
                data = pluginHost.ColorToBGR555(paleta.pltt.palettes[0].colors);
                oldDepth = paleta.pltt.depth;
            }
        }
        private void Write_Palette(string fileout, NCLR palette, IPluginHost pluginHost)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            for (int i = 0; i < (int)numericStartByte.Value; i++)
                bw.Write((byte)data[i]);
            for (int i = 0; i < palette.pltt.palettes.Length; i++)
                bw.Write(pluginHost.ColorToBGR555(palette.pltt.palettes[i].colors));
            for (int i = (int)bw.BaseStream.Length; i < data.Length; i++)
                bw.Write((byte)data[i]);

            bw.Flush();
            bw.Close();
        }
        public static NCLR Read_WinPal(string file, ColorDepth depth)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            NCLR palette = new NCLR();

            palette.header.id = br.ReadChars(4);  // RIFF
            palette.header.file_size = br.ReadUInt32();
            palette.pltt.ID = br.ReadChars(4);  // PAL
            br.ReadChars(4);    // data
            br.ReadUInt32();   // unknown, always 0x00
            br.ReadUInt16();   // unknown, always 0x0300
            palette.pltt.nColors = br.ReadUInt16();
            palette.pltt.depth = depth;
            uint num_color_per_palette = (depth == ColorDepth.Depth4Bit ? 0x10 : palette.pltt.nColors);
            palette.pltt.paletteLength = num_color_per_palette * 2;

            palette.pltt.palettes = new NTFP[(depth == ColorDepth.Depth4Bit ? palette.pltt.nColors / 0x10 : 1)];
            for (int i = 0; i < palette.pltt.palettes.Length; i++)
            {
                palette.pltt.palettes[i].colors = new Color[num_color_per_palette];
                for (int j = 0; j < num_color_per_palette; j++)
                {
                    Color newColor = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte());
                    br.ReadByte(); // always 0x00
                    palette.pltt.palettes[i].colors[j] = newColor;
                }
            }

            br.Close();
            return palette;
        }
        public static void Write_WinPal(string fileout, NCLR palette)
        {
            if (File.Exists(fileout))
                File.Delete(fileout);
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));
            int num_colors = 0;
            for (int i = 0; i < palette.pltt.palettes.Length; i++)
                num_colors += palette.pltt.palettes[i].colors.Length;

            bw.Write(new char[] { 'R', 'I', 'F', 'F' });
            bw.Write((uint)(0x10 + num_colors * 4));
            bw.Write(new char[] { 'P', 'A', 'L', ' ' });
            bw.Write(new char[] { 'd', 'a', 't', 'a' });
            bw.Write((uint)0x00);
            bw.Write((ushort)0x300);
            bw.Write((ushort)(num_colors));
            for (int i = 0; i < palette.pltt.palettes.Length; i++)
            {
                for (int j = 0; j < palette.pltt.palettes[i].colors.Length; j++)
                {
                    bw.Write(palette.pltt.palettes[i].colors[j].R);
                    bw.Write(palette.pltt.palettes[i].colors[j].G);
                    bw.Write(palette.pltt.palettes[i].colors[j].B);
                    bw.Write((byte)0x00);
                    bw.Flush();
                }
            }

            bw.Close();
        }


        private void numericStartByte_ValueChanged(object sender, EventArgs e)
        {
            Byte[] temp = new Byte[data.Length - (int)numericStartByte.Value];
            Array.Copy(data, (int)numericStartByte.Value, temp, 0, temp.Length);

            paleta.pltt.palettes[(int)nPaleta.Value - 1].colors = pluginHost.BGR555(temp);
            if (paleta.pltt.palettes[0].colors.Length == 0x10)
                paleta.pltt.depth = ColorDepth.Depth4Bit;
            pluginHost.Set_NCLR(paleta);

            ShowInfo();
            paletas = pluginHost.Bitmaps_NCLR(paleta);
            paletaBox.Image = paletas[0];
            nPaleta.Maximum = paleta.pltt.palettes.Length;
            nPaleta.Minimum = 1;
            nPaleta.Value = 1;
        }
        private void btnConverter_Click(object sender, EventArgs e)
        {
            numericStartByte.Value = 0;

            if (oldDepth == ColorDepth.Depth4Bit) // Convert to 8bpp
            {
                paleta.pltt = pluginHost.Palette_4bppTo8bpp(paleta.pltt);
                pluginHost.Set_NCLR(paleta);

                ShowInfo();
                paletas = pluginHost.Bitmaps_NCLR(paleta);
                paletaBox.Image = paletas[0];
                nPaleta.Maximum = 1;
                nPaleta.Minimum = 1;
                nPaleta.Value = 1;

                data = pluginHost.ColorToBGR555(paleta.pltt.palettes[0].colors);
                oldDepth = ColorDepth.Depth8Bit;
            }
            else  // Convert to 4bpp
            {
                paleta.pltt = pluginHost.Palette_8bppTo4bpp(paleta.pltt);
                pluginHost.Set_NCLR(paleta);

                ShowInfo();
                paletas = pluginHost.Bitmaps_NCLR(paleta);
                paletaBox.Image = paletas[0];
                nPaleta.Maximum = paleta.pltt.palettes.Length;
                nPaleta.Minimum = 1;
                nPaleta.Value = 1;

                data = pluginHost.ColorToBGR555(paleta.pltt.palettes[0].colors);
                oldDepth = ColorDepth.Depth4Bit;
            }

        }
    }
}
