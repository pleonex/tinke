/*
 * Copyright (C) 2012  pleoNeX
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
using System.Drawing;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;
using PluginInterface.Images.Formats;

namespace PluginInterface.Images
{
    public partial class SpriteControl : UserControl
    {
        SpriteBase sprite;
        ImageBase image;
        PaletteBase palette;
        IPluginHost pluginHost;

        bool selectColor;

        public SpriteControl()
        {
            InitializeComponent();
        }
        public SpriteControl(IPluginHost pluginHost)
        {
            InitializeComponent();

            this.pluginHost = pluginHost;
            this.sprite = pluginHost.Get_Sprite();
            this.image = pluginHost.Get_Image();
            this.palette = pluginHost.Get_Palette();

            Read_Language();

            for (ushort i = 0; i < sprite.NumBanks; i++)
                if (sprite.Banks[i].name is String)
                    comboBank.Items.Add(sprite.Banks[i].name);
                else
                    comboBank.Items.Add("Bank " + i.ToString());

            comboBank.SelectedIndex = 0;
        }
        public SpriteControl(IPluginHost pluginHost, SpriteBase sprite)
        {
            InitializeComponent();

            this.sprite = sprite;
            this.image = pluginHost.Get_Image();
            this.palette = pluginHost.Get_Palette();
            this.pluginHost = pluginHost;

            Read_Language();

            for (ushort i = 0; i < sprite.NumBanks; i++)
                if (sprite.Banks[i].name is String)
                    comboBank.Items.Add(sprite.Banks[i].name);
                else
                    comboBank.Items.Add("Bank " + i.ToString());
            comboBank.SelectedIndex = 0;
        }

        private void Read_Language()
        {
            try
            {
                XElement xml = XElement.Load(Application.StartupPath + "\\langs\\es-es.xml");
                xml = xml.Element("NCER");

                label1.Text = xml.Element("S01").Value;
                btnShowAll.Text = xml.Element("S02").Value;
                btnSave.Text = xml.Element("S03").Value;
                checkEntorno.Text = xml.Element("S0F").Value;
                checkCelda.Text = xml.Element("S10").Value;
                checkImagen.Text = xml.Element("S11").Value;
                checkTransparencia.Text = xml.Element("S12").Value;
                checkNumber.Text = xml.Element("S13").Value;
                label2.Text = xml.Element("S15").Value;
                btnBgd.Text = xml.Element("S17").Value;
                btnBgdTrans.Text = xml.Element("S18").Value;
                btnImport.Text = xml.Element("S24").Value;
                btnSetTrans.Text = xml.Element("S25").Value;
            }
            catch { throw new Exception("There was an error reading the XML language file."); }
        }

        private Image Update_Image()
        {
            imgBox.Image = sprite.Get_Image(image, palette, comboBank.SelectedIndex, 512, 256,
                checkEntorno.Checked, checkCelda.Checked, checkNumber.Checked, checkTransparencia.Checked,
                checkImagen.Checked);

            Clipboard.SetImage(imgBox.Image);

            return imgBox.Image;
        }

        private void comboBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update_Image();
        }
        private void check_CheckedChanged(object sender, EventArgs e)
        {
            Update_Image();
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            Form ven = new Form();
            int xMax = 4 * 260;
            int x = 0;
            int y = 15;

            for (int i = 0; i < sprite.NumBanks; i++)
            {
                PictureBox pic = new PictureBox();
                pic.Size = new Size(256, 256);
                pic.Location = new Point(x, y);
                pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                pic.Image = Update_Image();
                Label lbl = new Label();
                lbl.Text = sprite.Banks[i].name;
                lbl.Location = new Point(x, y - 15);

                ven.Controls.Add(pic);
                ven.Controls.Add(lbl);

                x += 260;
                if (x >= xMax)
                {
                    x = 0;
                    y += 275;
                }
            }

            // TODO: ven.Text = Tools.Helper.GetTranslation("NCER", "S14");
            ven.BackColor = SystemColors.GradientInactiveCaption;
            ven.AutoScroll = true;
            ven.AutoSize = true;
            ven.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ven.ShowIcon = false;
            ven.MaximizeBox = false;
            ven.MaximumSize = new System.Drawing.Size(1024, 700);
            ven.Location = new Point(20, 20);
            ven.Show();
        }

        private void imgBox_DoubleClick(object sender, EventArgs e)
        {
            Form ventana = new Form();
            PictureBox pic = new PictureBox();

            pic.Location = new Point(0, 0);
            pic.SizeMode = PictureBoxSizeMode.AutoSize;
            ventana.AutoSize = true;
            ventana.BackColor = SystemColors.GradientInactiveCaption;
            ventana.AutoScroll = true;
            ventana.MaximumSize = new Size(1024, 700);
            ventana.ShowIcon = false;
            // TODO: ventana.Text = Tools.Helper.GetTranslation("NCER", "S14");
            ventana.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            ventana.MaximizeBox = false;

            pic.Image = Update_Image();

            ventana.Controls.Add(pic);
            ventana.Show();
        }

        private void btnBgdTrans_Click(object sender, EventArgs e)
        {
            btnBgdTrans.Enabled = false;

            imgBox.BackColor = Color.Transparent;
        }
        private void btnBgd_Click(object sender, EventArgs e)
        {
            ColorDialog o = new ColorDialog();
            o.AllowFullOpen = true;
            o.AnyColor = true;

            if (o.ShowDialog() == DialogResult.OK)
            {
                imgBox.BackColor = o.Color;
                btnBgdTrans.Enabled = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!checkBatch.Checked)
                Export_Single();
            else
                Export_All();
        }
        private void Export_Single()
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".png";
            o.Filter = "Portable Network Graphic (*.png)|*.png|" +
                       "BitMaP (*.bmp)|*.bmp|" +
                       "JPEG (*.jpg)|*.jpg;*.jpeg|" +
                       "Tagged Image File Format (*.tiff)|*.tiff;*.tif|" +
                       "Graphic Interchange Format (*.gif)|*.gif|" +
                       "Icon (*.ico)|*.ico;*.icon";
            o.OverwritePrompt = true;
            o.FileName = sprite.FileName + '_' + comboBank.SelectedIndex;

            if (o.ShowDialog() == DialogResult.OK)
            {
                if (o.FilterIndex == 1)
                    imgBox.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Png);
                else if (o.FilterIndex == 2)
                    imgBox.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                else if (o.FilterIndex == 3)
                    imgBox.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                else if (o.FilterIndex == 4)
                    imgBox.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
                else if (o.FilterIndex == 5)
                    imgBox.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Gif);
                else if (o.FilterIndex == 6)
                    imgBox.Image.Save(o.FileName, System.Drawing.Imaging.ImageFormat.Icon);
            }
        }
        private void Export_All()
        {
            if (txtBatch.Text == "" || txtBatch.Text == null || !txtBatch.Text.Contains("%s"))
            {
                MessageBox.Show("Invalid file name.");
                return;
            }
            for (int i = 0; i < Path.GetInvalidFileNameChars().Length; i++)
            {
                if (txtBatch.Text.Contains(Path.GetInvalidFileNameChars()[i].ToString()))
                {
                    MessageBox.Show("Invalid file name.");
                    return;
                }
            }

            FolderBrowserDialog o = new FolderBrowserDialog();
            o.Description = "Select the folder to extract the sprites.";
            o.ShowNewFolderButton = true;
            if (o.ShowDialog() != DialogResult.OK)
                return;

            // TODO: Only export to PNG
            for (int i = 0; i < sprite.NumBanks; i++)
            {
                Image img = sprite.Get_Image(image, palette, i, 512, 256, checkEntorno.Checked, checkCelda.Checked,
                    checkNumber.Checked, checkTransparencia.Checked, checkImagen.Checked);

                string path = o.SelectedPath + Path.DirectorySeparatorChar;
                path += txtBatch.Text.Replace("%s", i.ToString()) + ".png";
                img.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (checkBatch.Checked)
                Import_All();
            else
                Import_Single();
        }
        private void Import_Single()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.Filter = "Supported images |*.png;*.bmp;*.jpg;*.jpeg;*.tif;*.tiff;*.gif;*.ico;*.icon|" +
                       "BitMaP (*.bmp)|*.bmp|" +
                       "Portable Network Graphic (*.png)|*.png|" +
                       "JPEG (*.jpg)|*.jpg;*.jpeg|" +
                       "Tagged Image File Format (*.tiff)|*.tiff;*.tif|" +
                       "Graphic Interchange Format (*.gif)|*.gif|" +
                       "Icon (*.ico)|*.ico;*.icon";
            o.Multiselect = false;
            if (o.ShowDialog() != DialogResult.OK)
                return;

            // TODO: Ask in a windows to select the index
            Import_File(o.FileName, comboBank.SelectedIndex, 0);

            Save_Files();
            Update_Image();
        }
        private void Import_All()
        {
            FolderBrowserDialog o = new FolderBrowserDialog();
            o.Description = "Select the folder where the images are.";
            if (o.ShowDialog() != DialogResult.OK)
                return;

            string[] imgs = Directory.GetFiles(o.SelectedPath);
            for (int i = 0; i < sprite.NumBanks; i++)
            {
                string img = "";
                for (int j = 0; j < imgs.Length; j++)
                {
                    if (Path.GetFileNameWithoutExtension(imgs[j]) == txtBatch.Text.Replace("%s", i.ToString()))
                    {
                        img = imgs[j];
                        break;
                    }
                }

                if (img == "")
                    continue;

                // TODO: I need to think how to know the palette index
                Import_File(img, i, 0);
            }

            Save_Files();
            Update_Image();
        }
        private void Import_File(string path, int banki, int pali)
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(path);

            // Get data from image
            byte[] tiles = new byte[0];
            Color[] pal = new Color[0];
            try { Actions.Indexed_Image(bitmap, image.FormatColor, out tiles, out pal); }
            catch (Exception ex) { MessageBox.Show(ex.Message); Console.WriteLine(ex.Message); return; }

            // Swap palettes if "Save palette" is not checked. Try to change the colors to the old palette
            if (!checkPalette.Checked)
            {
                try { Actions.Swap_Palette(ref tiles, palette.Palette[pali], pal, image.FormatColor); }
                catch (Exception ex) { MessageBox.Show(ex.Message); Console.WriteLine(ex.Message); return; }
            }

            // Get the data of a oam and add to the end of the image
            byte[] imgData = image.Tiles;
            for (int i = 0; i < sprite.Banks[banki].oams.Length; i++)
            {
                OAM oam = sprite.Banks[banki].oams[i];
                byte[] cellImg = Actions.Get_OAMdata(oam, tiles, image.FormatColor);

                if (image.FormTile == TileForm.Horizontal)
                    cellImg = Actions.HorizontalToLineal(cellImg, oam.width, oam.height, image.BPP, 8);

                uint offset = Actions.Add_Image(ref imgData, cellImg, (uint)(1 << (int)sprite.BlockSize) * 0x20);
                offset /= 0x20;
                offset >>= (int)sprite.BlockSize;
                sprite.Banks[banki].oams[i].obj2.tileOffset = offset;
            }
            image.Set_Tiles(imgData, 0x100, imgData.Length / 0x100, image.FormatColor, image.FormTile, image.CanEdit);

            // Set the palette
            if (checkPalette.Checked)
                palette.Set_Palette(pal, pali);
        }

        void Save_Files()
        {
            if (sprite.ID >= 0)
            {
                try
                {
                    string spriteFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + sprite.FileName;
                    sprite.Write(spriteFile, image, palette);
                    pluginHost.ChangeFile(sprite.ID, spriteFile);
                }
                catch (Exception e) { MessageBox.Show("Error writing new sprite:\n" + e.Message); };
            }
            if (image.ID >= 0)
            {
                try
                {
                    string imageFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + image.FileName;
                    image.Write(imageFile, palette);
                    pluginHost.ChangeFile(image.ID, imageFile);
                }
                catch (Exception e) { MessageBox.Show("Error writing new image:\n" + e.Message); };
            }
            if (checkPalette.Checked && palette.ID >= 0)
            {
                try
                {
                    string paletteFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + palette.FileName;
                    palette.Write(paletteFile);
                    pluginHost.ChangeFile(palette.ID, paletteFile);
                }
                catch (Exception e) { MessageBox.Show("Error writing new palette:\n" + e.Message); };
            }
        }

        private void btnSetTrans_Click(object sender, EventArgs e)
        {
            selectColor = true;
        }
        private void SetTransFromImage(Color color)
        {
            int pal_index = sprite.Banks[comboBank.SelectedIndex].oams[0].obj2.index_palette;  // How can I know that? yeah, I'm too lazy to do a new windows ;)

            Color[] pal = palette.Palette[pal_index];
            byte[] tiles = image.Tiles;

            int index = -1;
            for (int i = 0; i < pal.Length; i++)
            {
                if (pal[i] == color)
                {
                    index = i;
                    break;
                }
            }

            Actions.Swap_Color(ref tiles, ref pal, index, 0, image.FormatColor);

            Color[][] new_pal = palette.Palette;
            new_pal[pal_index] = pal;

            if (image.ID > 0)
                image.Set_Tiles(tiles);
            if (palette.ID > 0)
                palette.Set_Palette(new_pal);

            Save_Files();
        }

        private void btnOAMeditor_Click(object sender, EventArgs e)
        {
            Dialogs.OAMEditor editor = new Dialogs.OAMEditor(sprite.Banks[comboBank.SelectedIndex], sprite, image, palette);
            if (editor.ShowDialog() != DialogResult.OK)
                return;

            sprite.Banks[comboBank.SelectedIndex] = editor.Bank;
            Update_Image();
            Save_Files();
        }

        private void checkPalette_CheckedChanged(object sender, EventArgs e)
        {
            btnSetTrans.Enabled = checkPalette.Checked;
        }

        private void imgBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (selectColor && imgBox.Image is Image)
            {
                Color color = ((Bitmap)imgBox.Image).GetPixel(e.X, e.Y);
                SetTransFromImage(color);
            }
        }

        private void checkBatch_CheckedChanged(object sender, EventArgs e)
        {
            txtBatch.Enabled = checkBatch.Checked;
        }



        //private void btnSetTrans_Click(object sender, EventArgs e)
        //{
        //    Dialog.SelectModeColor dialog = new Dialog.SelectModeColor();
        //    if (dialog.ShowDialog() != DialogResult.OK)
        //        return;

        //    if (dialog.Option == 2)
        //    {
        //        ColorDialog o = new ColorDialog();
        //        o.AllowFullOpen = true;
        //        o.AnyColor = true;
        //        o.FullOpen = true;
        //        if (o.ShowDialog() == DialogResult.OK)
        //            Change_TransparencyColor(o.Color);
        //        o.Dispose();
        //    }
        //    else if (dialog.Option == 1)
        //        selectColor = true;
        //    else if (dialog.Option == 3)
        //    {
        //        Add_TransparencyColor();
        //    }
        //}
        //private void imgBox_MouseClick(object sender, MouseEventArgs e)
        //{
        //    if (selectColor && imgBox.Image is Image)
        //    {
        //        Color color = ((Bitmap)imgBox.Image).GetPixel(e.X, e.Y);
        //        Change_TransparencyColor(color);
        //    }
        //}
        //private void Add_TransparencyColor()
        //{
        //    int paletteIndex = ncer.cebk.banks[comboCelda.SelectedIndex].cells[0].obj2.index_palette;

        //    // Search for unused or duplicated colors to change them with transparency color
        //    // Search for duplicated colors
        //    int result = Convertir.Remove_DuplicatedColors(ref paleta.pltt.palettes[paletteIndex], ref tile.rahc.tileData.tiles);
        //    if (result == -1)
        //    {
        //        // Try another way: search for not used colors
        //        result = Convertir.Remove_NotUsedColors(ref paleta.pltt.palettes[paletteIndex], ref tile.rahc.tileData.tiles);
        //        if (result == -1)
        //        {
        //            MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S24"));
        //            return;  // Nothing found.
        //        }
        //    }

        //    // Now, the palette must have at least one transparency color, we put it in first place.
        //    paleta.pltt.palettes[paletteIndex].colors[result] = paleta.pltt.palettes[paletteIndex].colors[0];
        //    paleta.pltt.palettes[paletteIndex].colors[0] = Color.FromArgb(248, 0, 248);
        //    for (int i = 0; i < ncer.cebk.banks[comboCelda.SelectedIndex].cells.Length; i++)
        //    {
        //        tile.rahc.tileData.tiles = Imagen_NCER.Change_ColorCell(ncer.cebk.banks[comboCelda.SelectedIndex].cells[i],
        //            ncer.cebk.block_size, tile, result, 0);
        //    }

        //    // Save the new palette file
        //    pluginHost.Set_NCLR(paleta);
        //    String paletteFile = System.IO.Path.GetTempFileName();
        //    Imagen_NCLR.Escribir(paleta, paletteFile);
        //    pluginHost.ChangeFile((int)paleta.id, paletteFile);

        //    // Save the new tile file
        //    pluginHost.Set_NCGR(tile);
        //    String tileFile = System.IO.Path.GetTempFileName();
        //    Imagen_NCGR.Write(tile, tileFile);
        //    pluginHost.ChangeFile((int)tile.id, tileFile);

        //    // Refresh the image
        //    ActualizarImagen();
        //    checkTransparencia.Checked = true;
        //}

    }
}
