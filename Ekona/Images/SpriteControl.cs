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
using Ekona.Images.Formats;

namespace Ekona.Images
{
    public partial class SpriteControl : UserControl
    {
        SpriteBase sprite;
        ImageBase image;
        PaletteBase palette;
        IPluginHost pluginHost;
        XElement lang;
        string trans;

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
            Update_Info();
        }
        public SpriteControl(IPluginHost pluginHost, SpriteBase sprite)
        {
            InitializeComponent();

            this.sprite = sprite;
            this.image = pluginHost.Get_Image();
            this.palette = pluginHost.Get_Palette();
            this.pluginHost = pluginHost;

            Read_Language();
            Update_Info();
        }
        public SpriteControl(IPluginHost pluginHost, SpriteBase sprite, ImageBase image, PaletteBase palette)
        {
            InitializeComponent();

            this.sprite = sprite;
            this.image = image;
            this.palette = palette;
            this.pluginHost = pluginHost;

            Read_Language();
            Update_Info();
        }
        public SpriteControl(XElement lang, SpriteBase sprite, ImageBase image, PaletteBase palette)
        {
            InitializeComponent();

            this.sprite = sprite;
            this.image = image;
            this.palette = palette;
            this.lang = lang;

            Read_Language(lang);
            Update_Info();
        }


        private void Read_Language()
        {
            try
            {
                XElement xml = XElement.Load(pluginHost.Get_LangXML());
                xml = xml.Element("Ekona");
                Read_Language(xml);
            }
            catch { throw new Exception("There was an error reading the XML language file."); }
        }
        private void Read_Language(XElement xml)
        {
            try
            {
                xml = xml.Element("SpriteControl");

                label1.Text = xml.Element("S01").Value;
                btnShowAll.Text = xml.Element("S02").Value;
                label3.Text = xml.Element("S03").Value.Remove(0, 1);
                checkBatch.Text = xml.Element("S04").Value;
                btnOAMeditor.Text = xml.Element("S05").Value;
                btnExport.Text = xml.Element("S06").Value;
                btnImport.Text = xml.Element("S07").Value;
                btnSetTrans.Text = xml.Element("S08").Value;
                btnBgd.Text = xml.Element("S09").Value;
                btnBgdTrans.Text = xml.Element("S0A").Value;
                groupBox1.Text = xml.Element("S0B").Value;
                checkGrid.Text = xml.Element("S0C").Value;
                checkCellBorder.Text = xml.Element("S0D").Value;
                checkImage.Text = xml.Element("S0E").Value;
                checkTransparency.Text = xml.Element("S0F").Value;
                checkNumber.Text = xml.Element("S10").Value;
                radioSwapPal.Text = xml.Element("S11").Value;
                trans = xml.Element("S12").Value;
                label4.Text = "of " + sprite.NumBanks.ToString();
                radioReplacePal.Text = xml.Element("S14").Value;
                radioOriginalPal.Text = xml.Element("S13").Value;
                groupBox2.Text = xml.Element("S15").Value;
                label2.Text = xml.Element("S16").Value;
                groupBox3.Text = xml.Element("S17").Value;
                radioImgAdd.Text = xml.Element("S18").Value;
                radioImgReplace.Text = xml.Element("S19").Value;
            }
            catch { throw new Exception("There was an error reading the XML language file."); }
        }

        private Image Update_Image()
        {
            int[] index = new int[checkListOAM.CheckedIndices.Count];
            for (int i = 0; i < index.Length; i++)
                index[i] = checkListOAM.CheckedIndices[i];

            imgBox.Image = sprite.Get_Image(image, palette, comboBank.SelectedIndex, 512, 256,
                checkGrid.Checked, checkCellBorder.Checked, checkNumber.Checked, checkTransparency.Checked,
                checkImage.Checked, (checkSelectOAM.Checked ? checkListOAM.SelectedIndex : -1), 
                index);

            Clipboard.SetImage(imgBox.Image);

            return imgBox.Image;
        }
        private void Update_Info()
        {
            this.btnImport.Enabled = (sprite.CanEdit && image.CanEdit && palette.CanEdit ? true : false);
            this.btnOAMeditor.Enabled = sprite.CanEdit;
            groupBox2.Enabled = sprite.CanEdit;
            groupBox3.Enabled = sprite.CanEdit;

            for (ushort i = 0; i < sprite.NumBanks; i++)
                if (sprite.Banks[i].name is String)
                    comboBank.Items.Add(sprite.Banks[i].name);
                else
                    comboBank.Items.Add("Bank " + i.ToString());
            comboBank.SelectedIndex = 0;
            txtBatch.Text = Path.GetFileNameWithoutExtension(sprite.FileName) + "_%s";

            Update_BankInfo(0);
        }
        private void Update_BankInfo(int i)
        {
            checkListOAM.Items.Clear();
            for (int k = 0; k < sprite.Banks[i].oams.Length; k++)
                checkListOAM.Items.Add("OAM " + k.ToString(), true);
        }

        private void comboBank_SelectedIndexChanged(object sender, EventArgs e)
        {
            Update_BankInfo(comboBank.SelectedIndex);
            Update_Image();
        }
        private void check_CheckedChanged(object sender, EventArgs e)
        {
            Update_Image();
        }

        private void btnShowAll_Click(object sender, EventArgs e)
        {
            Form win = new Form();
            int xMax = 516 * 2;
            int x = 0;
            int y = 15;

            for (int i = 0; i < sprite.NumBanks; i++)
            {
                PictureBox pic = new PictureBox();
                pic.Size = new Size(512, 256);
                pic.Location = new Point(x, y);
                pic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                pic.Image = sprite.Get_Image(image, palette, i, 512, 256,
                    checkGrid.Checked, checkCellBorder.Checked, checkNumber.Checked,
                    checkTransparency.Checked, checkImage.Checked);

                Label lbl = new Label();
                lbl.Text = sprite.Banks[i].name;
                lbl.Location = new Point(x, y - 15);

                win.Controls.Add(pic);
                win.Controls.Add(lbl);

                x += 516;
                if (x >= xMax)
                {
                    x = 0;
                    y += 275;
                }
            }

            win.Text = trans;
            win.BackColor = SystemColors.GradientInactiveCaption;
            win.AutoScroll = true;
            win.AutoSize = true;
            win.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            win.MaximumSize = new System.Drawing.Size(1050, 700);
            win.FormBorderStyle = FormBorderStyle.Sizable;
            win.ShowIcon = false;
            win.MaximizeBox = true;
            win.Show();
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
        private void imgBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (selectColor && imgBox.Image is Image)
            {
                Color color = ((Bitmap)imgBox.Image).GetPixel(e.X, e.Y);
                SetTransFromImage(color);
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
            o.SelectedPath = Directory.GetParent(Helper.IOutil.LastSelectedFile()).FullName;
            if (o.ShowDialog() != DialogResult.OK)
                return;

            // TODO: Only export to PNG
            for (int i = 0; i < sprite.NumBanks; i++)
            {
                Image img = sprite.Get_Image(image, palette, i, 512, 256, checkGrid.Checked, checkCellBorder.Checked,
                    checkNumber.Checked, checkTransparency.Checked, checkImage.Checked);

                string path = o.SelectedPath + Path.DirectorySeparatorChar;
                path += txtBatch.Text.Replace("%s", i.ToString()) + ".png";
                img.Save(path, System.Drawing.Imaging.ImageFormat.Png);
            }

            o.Dispose();
            o = null;
        }

        private void checkBatch_CheckedChanged(object sender, EventArgs e)
        {
            txtBatch.Enabled = checkBatch.Checked;
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

            Import_File(o.FileName, comboBank.SelectedIndex);

            o.FileName = null;
            o.Dispose();
            o = null;

            Save_Files();
            Update_Image();
        }
        private void Import_All()
        {
            //FolderBrowserDialog o = new FolderBrowserDialog();
            //o.Description = "Select the folder where the images are.";
            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.Multiselect = true;
            if (o.ShowDialog() != DialogResult.OK)
                return;

            //string[] imgs = Directory.GetFiles(o.SelectedPath);
            string[] imgs = o.FileNames;

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

                Import_File(img, i);
            }

            Save_Files();
            Update_Image();

            o.Dispose();
            o = null;
        }
        private void Import_File(string path, int banki)
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(path);
            Console.WriteLine("Importing image {0} to bank {1}", path, banki.ToString());

            OAM[] oams = (OAM[])sprite.Banks[banki].oams.Clone();
            Color[][] pals = (Color[][])palette.Palette.Clone();
            byte[] imgData = (byte[])image.Tiles.Clone();

            // Get data from image
            byte[] tiles = new byte[0];
            Color[] pal = new Color[0];

            if (radioOriginalPal.Checked)
            {
                BMP bmp = new BMP(path);
                tiles = bmp.Tiles;
                if (image.FormatColor != bmp.FormatColor)
                    if (image.FormatColor == ColorFormat.colors16)
                        tiles = Helper.BitsConverter.Bits4ToByte(tiles);
                    else if (image.FormatColor == ColorFormat.colors256)
                        tiles = Helper.BitsConverter.BytesToBit4(tiles);
                pal = bmp.Palette.Palette[0];
            }

            uint addedSize = 0;

            // Get the data of a oam and add to the end of the image
            for (int i = 0; i < oams.Length; i++)
            {
                if (!checkListOAM.GetItemChecked(i))
                    continue;

                Console.WriteLine("Processing cell {0}", oams[i].num_cell.ToString());
                byte[] cellImg;
                if (!radioOriginalPal.Checked)
                {
                    Bitmap subImg = (Bitmap)bitmap.Clone(new Rectangle(
                        oams[i].obj1.xOffset + 256, oams[i].obj0.yOffset + 128,
                        oams[i].width, oams[i].height),
                        System.Drawing.Imaging.PixelFormat.DontCare);
                    Actions.Indexed_Image(subImg, image.FormatColor, out cellImg, out pal);
                }
                else
                    cellImg = Actions.Get_OAMdata(oams[i], tiles, image.FormatColor);

                // Swap palettes if "Swap palette" is checked. Try to change the colors to the old palette
                if (radioSwapPal.Checked)
                {
                    try { Actions.Swap_Palette(ref cellImg, palette.Palette[oams[i].obj2.index_palette], pal, image.FormatColor, numThreshold.Value); }
                    catch (Exception ex) { MessageBox.Show(ex.Message); Console.WriteLine(ex.Message); return; }
                }
                else if (radioReplacePal.Checked) // Set the palette
                    pals[oams[i].obj2.index_palette] = pal;

                if (image.FormTile == TileForm.Horizontal)
                    cellImg = Actions.HorizontalToLineal(cellImg, oams[i].width, oams[i].height, image.BPP, 8);

                // If Add image is checked add the new image to the end of the original file and change the tileOffset
                if (radioImgAdd.Checked)
                {
                    uint added = 0;
                    uint size = (sprite.Banks[banki].data_size > 0) ? sprite.Banks[banki].data_size + addedSize : (uint)imgData.Length;
                    uint offset = Actions.Add_Image(ref imgData, cellImg, sprite.Banks[banki].data_offset, size, (uint)(0x20 << (int)sprite.BlockSize), out added) - sprite.Banks[banki].data_offset;
                    addedSize += added;

                    offset = (offset / 0x20) >> (int)this.sprite.BlockSize;
                    if (offset >= 0x400)
                    {
                        MessageBox.Show(
                            "The characters data size has exceeded the boundaries of what is permitted!\r\nSome characters will not be displayed.");
                        break;
                    }

                    oams[i].obj2.tileOffset = offset;
                    oams[i].obj1.flipX = 0;
                    oams[i].obj1.flipY = 0;
                }
                else   // Replace the old image
                {
                    uint tileOffset = oams[i].obj2.tileOffset;
                    tileOffset = (uint)(tileOffset << (byte)sprite.BlockSize) * 0x20 + sprite.Banks[banki].data_offset;
                    Array.Copy(cellImg, 0, imgData, tileOffset, cellImg.Length);
                }
            }

            if (sprite.Banks[banki].data_size > 0)
            {
                sprite.Banks[banki].data_size += addedSize;
                for (int i = banki + 1; i < sprite.Banks.Length; i++) sprite.Banks[i].data_offset += addedSize;
            }

            // If everthing goes right then set the new data
            int height = (imgData.Length * 8 / image.BPP) / image.Width;
            image.Set_Tiles(imgData, image.Width, height, image.FormatColor, image.FormTile, image.CanEdit);
            sprite.Banks[banki].oams = oams;
            palette.Set_Palette(pals);

            bitmap.Dispose();
            bitmap = null;
        }
        void Save_Files()
        {
            if (sprite.ID >= 0)
            {
                try
                {
                    string spriteFile = "";
                    if (pluginHost is IPluginHost)
                        spriteFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + sprite.FileName;
                    else
                    {
                        SaveFileDialog o = new SaveFileDialog();
                        o.Title = "Save OAMs";
                        o.FileName = sprite.FileName;
                        if (o.ShowDialog() != DialogResult.OK)
                            return;
                        spriteFile = o.FileName;
                    }

                    sprite.Write(spriteFile, image, palette);

                    if (pluginHost is IPluginHost)
                        pluginHost.ChangeFile(sprite.ID, spriteFile);
                }
                catch (Exception e) { MessageBox.Show("Error writing new sprite:\n" + e.Message); };
            }
            if (image.ID >= 0)
            {
                try
                {
                    string imageFile = "";
                    if (pluginHost is IPluginHost)
                        imageFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + image.FileName;
                    else
                    {
                        SaveFileDialog o = new SaveFileDialog();
                        o.Title = "Save image";
                        o.FileName = image.FileName;
                        if (o.ShowDialog() != DialogResult.OK)
                            return;
                        imageFile = o.FileName;
                    }

                    image.Write(imageFile, palette);
                    
                    if (pluginHost is IPluginHost)
                        pluginHost.ChangeFile(image.ID, imageFile);
                }
                catch (Exception e) { MessageBox.Show("Error writing new image:\n" + e.Message); };
            }
            if (radioReplacePal.Checked && palette.ID >= 0)
            {
                try
                {
                    string paletteFile = "";
                    if (pluginHost is IPluginHost)
                        paletteFile = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName() + palette.FileName;
                    else
                    {
                        SaveFileDialog o = new SaveFileDialog();
                        o.Title = "Save palette";
                        o.FileName = palette.FileName;
                        if (o.ShowDialog() != DialogResult.OK)
                            return;
                        paletteFile = o.FileName;
                    }

                    palette.Write(paletteFile);

                    if (pluginHost is IPluginHost)
                        pluginHost.ChangeFile(palette.ID, paletteFile);
                }
                catch (Exception e) { MessageBox.Show("Error writing new palette:\n" + e.Message); };
            }
        }

        private void btnOAMeditor_Click(object sender, EventArgs e)
        {
            Dialogs.OAMEditor editor;
            if (pluginHost is IPluginHost)
                editor = new Dialogs.OAMEditor(pluginHost.Get_LangXML(), sprite.Banks[comboBank.SelectedIndex], sprite, image, palette);
            else
                editor = new Dialogs.OAMEditor(lang.Element("OAMEditor"), sprite.Banks[comboBank.SelectedIndex], sprite, image, palette);
            
            if (editor.ShowDialog() != DialogResult.OK)
                return;

            Update_BankInfo(comboBank.SelectedIndex);
            sprite.Banks[comboBank.SelectedIndex] = editor.Bank;
            Update_Image();
            Save_Files();
        }

        public int SelectedBank
        {
            get { return comboBank.SelectedIndex; }
        }
        public SpriteBase Sprite
        {
            get { return sprite; }
        }
        public ImageBase Tiles
        {
            get { return image; }
        }
        public PaletteBase Palette
        {
            get { return palette; }
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
