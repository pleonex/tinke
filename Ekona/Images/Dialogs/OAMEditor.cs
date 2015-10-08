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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Ekona.Images.Dialogs
{
    public partial class OAMEditor : Form
    {
        Bank bank;
        bool stop;

        bool preview;
        SpriteBase sprite;
        ImageBase image;
        PaletteBase palette;

        public OAMEditor()
        {
            InitializeComponent();
        }
        public OAMEditor(string langxml, Bank bank)
        {
            InitializeComponent();
            this.bank = bank;
            numOAM.Maximum = bank.oams.Length - 1;

            preview = false;
            picBox.Enabled = false;
            groupPreview.Enabled = false;

            Read_Language(langxml);
        }
        public OAMEditor(string langxml, Bank bank, SpriteBase sprite, ImageBase image, PaletteBase palette)
        {
            InitializeComponent();
            this.bank = bank;
            numOAM.Maximum = bank.oams.Length - 1;
            numOffset.Maximum = (bank.data_size == 0)
                ? image.Tiles.Length / (0x20 << (int)sprite.BlockSize) - 1
                : bank.data_size / (0x20 << (int)sprite.BlockSize) - 1;

            preview = true;
            this.sprite = sprite;
            this.image = image;
            this.palette = palette;

            Read_Language(langxml);
        }
        public OAMEditor(XElement langxml, Bank bank, SpriteBase sprite, ImageBase image, PaletteBase palette)
        {
            InitializeComponent();
            this.bank = bank;
            numOAM.Maximum = bank.oams.Length - 1;
            numOffset.Maximum = (bank.data_size == 0)
                ? image.Tiles.Length / (0x20 << (int)sprite.BlockSize) - 1
                : bank.data_size / (0x20 << (int)sprite.BlockSize) - 1;

            preview = true;
            this.sprite = sprite;
            this.image = image;
            this.palette = palette;

            Read_Language(langxml);
        }
        private void OAMEditor_Load(object sender, EventArgs e)
        {
            Read_Info(0);
            Update_Image();
        }

        private void Read_Language(string langxml)
        {
            try
            {
                XElement xml = XElement.Load(langxml);
                xml = xml.Element("Ekona").Element("OAMEditor");
                Read_Language(xml);
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }
        }
        private void Read_Language(XElement xml)
        {
            try
            {
                this.Text = xml.Element("S01").Value;
                label11.Text = xml.Element("S02").Value;
                label12.Text = xml.Element("S03").Value + ' ' + numOAM.Maximum.ToString();
                groupObj0.Text = xml.Element("S04").Value;
                label1.Text = xml.Element("S05").Value;
                checkRSflag.Text = xml.Element("S06").Value;
                checkObjdisable.Text = xml.Element("S07").Value;
                checkDoubleSize.Text = xml.Element("S08").Value;
                label3.Text = xml.Element("S09").Value;
                comboObjMode.Items[0] = xml.Element("S0A").Value;
                comboObjMode.Items[1] = xml.Element("S0B").Value;
                comboObjMode.Items[2] = xml.Element("S0C").Value;
                comboObjMode.Items[3] = xml.Element("S0D").Value;
                checkMosaic.Text = xml.Element("S0E").Value;
                label5.Text = xml.Element("S0F").Value;
                comboDepth.Items[0] = xml.Element("S10").Value;
                comboDepth.Items[1] = xml.Element("S11").Value;
                label4.Text = xml.Element("S12").Value;
                comboShape.Items[0] = xml.Element("S13").Value;
                comboShape.Items[1] = xml.Element("S14").Value;
                comboShape.Items[2] = xml.Element("S15").Value;
                comboShape.Items[3] = xml.Element("S0D").Value;
                label15.Text = xml.Element("S16").Value;
                label13.Text = xml.Element("S17").Value;
                btnAddOAM.Text = xml.Element("S18").Value;
                btnRemOAM.Text = xml.Element("S19").Value;
                groupObj1.Text = xml.Element("S1A").Value;
                label2.Text = xml.Element("S1B").Value;
                label6.Text = xml.Element("S1C").Value;
                checkFlipX.Text = xml.Element("S1D").Value;
                checkFlipY.Text = xml.Element("S1E").Value;
                label7.Text = xml.Element("S1F").Value;
                groupObj2.Text = xml.Element("S20").Value;
                label8.Text = xml.Element("S21").Value;
                label9.Text = xml.Element("S22").Value;
                label10.Text = xml.Element("S23").Value;
                btnSave.Text = xml.Element("S24").Value;
                label14.Text = xml.Element("S25").Value;
                groupPreview.Text = xml.Element("S26").Value;
                checkTrans.Text = xml.Element("S27").Value;
                checkOAM.Text = xml.Element("S28").Value;
                checkImage.Text = xml.Element("S29").Value;
                checkNumbers.Text = xml.Element("S2A").Value;
                checkGrid.Text = xml.Element("S2B").Value;
                checkCurrOAM.Text = xml.Element("S2C").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }
        }

        private void Read_Info(int i)
        {
            OAM oam = bank.oams[i];
            stop = true;

            // Obj0
            numYoffset.Value = oam.obj0.yOffset;
            if (oam.obj0.rs_flag == 0)
            {
                checkRSflag.Checked = false;
                checkObjdisable.Enabled = true;
                checkDoubleSize.Enabled = false;

                checkFlipX.Enabled = true;
                checkFlipY.Enabled = true;
                numSelectPar.Enabled = false;
            }
            else
            {
                checkRSflag.Checked = true;
                checkObjdisable.Enabled = false;
                checkDoubleSize.Enabled = true;

                checkFlipX.Enabled = false;
                checkFlipY.Enabled = false;
                numSelectPar.Enabled = true;
            }
            checkDoubleSize.Checked = (oam.obj0.doubleSize == 0) ? false : true;
            checkObjdisable.Checked = (oam.obj0.objDisable == 0) ? false : true;
            comboObjMode.SelectedIndex = oam.obj0.objMode;
            checkMosaic.Checked = (oam.obj0.mosaic_flag == 0) ? false : true;
            comboDepth.SelectedIndex = oam.obj0.depth;
            comboShape.SelectedIndex = oam.obj0.shape;

            // Obj1
            numXpos.Value = oam.obj1.xOffset;
            numSelectPar.Value = oam.obj1.select_param;
            checkFlipX.Checked = (oam.obj1.flipX == 0) ? false : true;
            checkFlipY.Checked = (oam.obj1.flipY == 0) ? false : true;
            numSize.Value = oam.obj1.size;

            // Obj2
            numOffset.Value = oam.obj2.tileOffset;
            numPrio.Value = oam.obj2.priority;
            numPal.Value = oam.obj2.index_palette;

            // Auto size
            switch (oam.obj0.shape)
            {
                case 0:
                    if (oam.obj1.size == 0) comboSize.SelectedIndex = 0;
                    else if (oam.obj1.size == 1) comboSize.SelectedIndex = 1;
                    else if (oam.obj1.size == 2) comboSize.SelectedIndex = 2;
                    else if (oam.obj1.size == 3) comboSize.SelectedIndex = 3;
                    break;
                case 1:
                    if (oam.obj1.size == 0) comboSize.SelectedIndex = 4;
                    else if (oam.obj1.size == 1) comboSize.SelectedIndex = 5;
                    else if (oam.obj1.size == 2) comboSize.SelectedIndex = 6;
                    else if (oam.obj1.size == 3) comboSize.SelectedIndex = 7;
                    break;
                case 2:
                    if (oam.obj1.size == 0) comboSize.SelectedIndex = 8;
                    else if (oam.obj1.size == 1) comboSize.SelectedIndex = 9;
                    else if (oam.obj1.size == 2) comboSize.SelectedIndex = 10;
                    else if (oam.obj1.size == 3) comboSize.SelectedIndex = 11;
                    break;
            }

            numNumOAM.Value = oam.num_cell;
            stop = false;
        }
        private void Update_Image()
        {
            stop = true;

            OAM oam = bank.oams[(int)numOAM.Value];
            Size size = Actions.Get_OAMSize(oam.obj0.shape, oam.obj1.size);
            oam.width = (ushort)size.Width;
            oam.height = (ushort)size.Height;
            bank.oams[(int)numOAM.Value] = oam;

            ushort[] objs = Actions.OAMInfo(oam);
            numObj0.Value = objs[0];
            numObj1.Value = objs[1];
            numObj2.Value = objs[2];

            if (!preview)
                return;

            picBox.Image = sprite.Get_Image(image, palette, bank, 512, 256, checkGrid.Checked, checkOAM.Checked,
                checkNumbers.Checked, checkTrans.Checked, checkImage.Checked, (checkCurrOAM.Checked ? (int)numOAM.Value : -1));
            stop = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public Bank Bank
        {
            get { return bank; }
        }

        private void numOAM_ValueChanged(object sender, EventArgs e)
        {
            Read_Info((int)numOAM.Value);
            Update_Image();
        }

        private void Change_OBJ0(object sender, EventArgs e)
        {
            if (stop)
                return;

            bank.oams[(int)numOAM.Value].obj0.yOffset = (int)numYoffset.Value;
            bank.oams[(int)numOAM.Value].obj0.rs_flag = (byte)(checkRSflag.Checked ? 1 : 0);
            if (checkRSflag.Checked)
            {
                checkObjdisable.Enabled = false;
                checkDoubleSize.Enabled = true;

                checkFlipX.Enabled = false;
                checkFlipY.Enabled = false;
                numSelectPar.Enabled = true;
            }
            else
            {
                checkObjdisable.Enabled = true;
                checkDoubleSize.Enabled = false;

                checkFlipX.Enabled = true;
                checkFlipY.Enabled = true;
                numSelectPar.Enabled = false;
            }
            bank.oams[(int)numOAM.Value].obj0.objDisable = (byte)(checkObjdisable.Checked ? 1 : 0);
            bank.oams[(int)numOAM.Value].obj0.doubleSize = (byte)(checkDoubleSize.Checked ? 1 : 0);
            bank.oams[(int)numOAM.Value].obj0.objMode = (byte)comboObjMode.SelectedIndex;
            bank.oams[(int)numOAM.Value].obj0.mosaic_flag = (byte)(checkMosaic.Checked ? 1 : 0);
            bank.oams[(int)numOAM.Value].obj0.depth = (byte)comboDepth.SelectedIndex;
            bank.oams[(int)numOAM.Value].obj0.shape = (byte)comboShape.SelectedIndex;

            Update_Image();
        }
        private void Change_OBJ1(object sender, EventArgs e)
        {
            if (stop)
                return;

            bank.oams[(int)numOAM.Value].obj1.xOffset = (int)numXpos.Value;
            bank.oams[(int)numOAM.Value].obj1.select_param = (byte)numSelectPar.Value;
            bank.oams[(int)numOAM.Value].obj1.flipX = (byte)(checkFlipX.Checked ? 1 : 0);
            bank.oams[(int)numOAM.Value].obj1.flipY = (byte)(checkFlipY.Checked ? 1 : 0);
            bank.oams[(int)numOAM.Value].obj1.size = (byte)numSize.Value;
            Update_Image();
        }
        private void Change_OBJ2(object sender, EventArgs e)
        {
            if (stop)
                return;

            bank.oams[(int)numOAM.Value].obj2.tileOffset = (uint)numOffset.Value;
            bank.oams[(int)numOAM.Value].obj2.priority = (byte)numPrio.Value;
            bank.oams[(int)numOAM.Value].obj2.index_palette = (byte)numPal.Value;

            Update_Image();
        }
        private void Change_Preview(object sender, EventArgs e)
        {
            Update_Image();
        }
        private void numNumOAM_ValueChanged(object sender, EventArgs e)
        {
            if (stop)
                return;

            bank.oams[(int)numOAM.Value].num_cell = (ushort)numNumOAM.Value;

            OAM currOAM = bank.oams[(int)numOAM.Value];

            // Reorder the cells due to the new priority
            List<OAM> cells = new List<OAM>();
            cells.AddRange(bank.oams);
            cells.Sort(Actions.Comparision_OAM);
            bank.oams = cells.ToArray();

            numOAM.Value = Array.IndexOf(bank.oams, currOAM);
            Read_Info((int)numOAM.Value);
            Update_Image();
        }

        private void comboSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stop)
                return;

            switch (comboSize.SelectedIndex)
            {
                case 0:
                    bank.oams[(int)numOAM.Value].obj0.shape = 0;
                    bank.oams[(int)numOAM.Value].obj1.size = 0;
                    break;
                case 1:
                    bank.oams[(int)numOAM.Value].obj0.shape = 0;
                    bank.oams[(int)numOAM.Value].obj1.size = 1;
                    break;
                case 2:
                    bank.oams[(int)numOAM.Value].obj0.shape = 0;
                    bank.oams[(int)numOAM.Value].obj1.size = 2;
                    break;
                case 3:
                    bank.oams[(int)numOAM.Value].obj0.shape = 0;
                    bank.oams[(int)numOAM.Value].obj1.size = 3;
                    break;
                case 4:
                    bank.oams[(int)numOAM.Value].obj0.shape = 1;
                    bank.oams[(int)numOAM.Value].obj1.size = 0;
                    break;
                case 5:
                    bank.oams[(int)numOAM.Value].obj0.shape = 1;
                    bank.oams[(int)numOAM.Value].obj1.size = 1;
                    break;
                case 6:
                    bank.oams[(int)numOAM.Value].obj0.shape = 1;
                    bank.oams[(int)numOAM.Value].obj1.size = 2;
                    break;
                case 7:
                    bank.oams[(int)numOAM.Value].obj0.shape = 1;
                    bank.oams[(int)numOAM.Value].obj1.size = 3;
                    break;
                case 8:
                    bank.oams[(int)numOAM.Value].obj0.shape = 2;
                    bank.oams[(int)numOAM.Value].obj1.size = 0;
                    break;
                case 9:
                    bank.oams[(int)numOAM.Value].obj0.shape = 2;
                    bank.oams[(int)numOAM.Value].obj1.size = 1;
                    break;
                case 10:
                    bank.oams[(int)numOAM.Value].obj0.shape = 2;
                    bank.oams[(int)numOAM.Value].obj1.size = 2;
                    break;
                case 11:
                    bank.oams[(int)numOAM.Value].obj0.shape = 2;
                    bank.oams[(int)numOAM.Value].obj1.size = 3;
                    break;
            }
            Read_Info((int)numOAM.Value);
            Update_Image();
        }

        private void btnAddOAM_Click(object sender, EventArgs e)
        {
            int length = bank.oams.Length;

            OAM[] newOAM = new OAM[length + 1];
            Array.Copy(bank.oams, newOAM, length);
            // New oam
            newOAM[length] = new OAM();
            newOAM[length].obj0.yOffset = -128;
            newOAM[length].obj1.xOffset = -256;
            if (checkAddFirst.Checked)
            {
                newOAM[length].num_cell = 0;            // Set this OAM as the first, with more priority in this layer so visible
                for (int i = 0; i < length; i++)        // And increment the number of each OAM to fix that
                    newOAM[i].num_cell++;
            }
            else
                newOAM[length].num_cell = (ushort)length;   // Set to the background of the layer
            bank.oams = newOAM;
            OAM oam = newOAM[length];

            // Reorder the cells due to the new num_cell and priority
            List<OAM> cells = new List<OAM>();
            cells.AddRange(bank.oams);
            cells.Sort(Actions.Comparision_OAM);
            bank.oams = cells.ToArray();

            // Update
            numOAM.Maximum = bank.oams.Length - 1;
            label12.Text = "of " + numOAM.Maximum.ToString();

            numOAM.Value = Array.IndexOf(bank.oams, oam);
            Read_Info((int)numOAM.Value);
            Update_Image();
        }
        private void btnRemOAM_Click(object sender, EventArgs e)
        {
            OAM[] newOAM = new OAM[bank.oams.Length - 1];
            int j = 0;
            for (int i = 0; i < bank.oams.Length; i++)
                if (i != numOAM.Value)
                    newOAM[j++] = bank.oams[i];
            bank.oams = newOAM;

            numOAM.Maximum = bank.oams.Length - 1;
            label12.Text = "of " + numOAM.Maximum.ToString();
            Read_Info((int)numOAM.Value);
            Update_Image();
        }

        private void numObj_ValueChanged(object sender, EventArgs e)
        {
            if (stop)
                return;

            bank.oams[(int)numOAM.Value] = Actions.OAMInfo(
                (ushort)numObj0.Value,
                (ushort)numObj1.Value,
                (ushort)numObj2.Value);

            Read_Info((int)numOAM.Value);
            Update_Image();
        }

    }
}
