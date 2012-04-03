using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PluginInterface.Images.Dialogs
{
    public partial class OAMEditor : Form
    {
        Bank bank;

        bool preview;
        SpriteBase sprite;
        ImageBase image;
        PaletteBase palette;

        public OAMEditor()
        {
            InitializeComponent();
        }
        public OAMEditor(Bank bank)
        {
            InitializeComponent();
            this.bank = bank;
            numOAM.Maximum = bank.oams.Length - 1;
            label12.Text = "of " + numOAM.Maximum.ToString();

            preview = false;
            picBox.Enabled = false;
            groupPreview.Enabled = false;
        }
        public OAMEditor(Bank bank, SpriteBase sprite, ImageBase image, PaletteBase palette)
        {
            InitializeComponent();
            this.bank = bank;
            numOAM.Maximum = bank.oams.Length - 1;
            label12.Text = "of " + numOAM.Maximum.ToString();

            preview = true;
            this.sprite = sprite;
            this.image = image;
            this.palette = palette;
        }
        private void OAMEditor_Load(object sender, EventArgs e)
        {
            Read_Info(0);
            Update_Image();
        }

        private void Read_Info(int i)
        {
            OAM oam = bank.oams[i];

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
        }

        private void Update_Image()
        {
            OAM oam = bank.oams[(int)numOAM.Value];
            Size size = Actions.Get_OAMSize(oam.obj0.shape, oam.obj1.size);
            oam.width = (ushort)size.Width;
            oam.height = (ushort)size.Height;
            bank.oams[(int)numOAM.Value] = oam;
            

            if (!preview)
                return;

            picBox.Image = sprite.Get_Image(image, palette, bank, 512, 256, checkGrill.Checked, checkOAM.Checked,
                checkNumbers.Checked, checkTrans.Checked, checkImage.Checked, (checkCurrOAM.Checked ? (int)numOAM.Value : -1));
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
            bank.oams[(int)numOAM.Value].obj1.xOffset = (int)numXpos.Value;
            bank.oams[(int)numOAM.Value].obj1.select_param = (byte)numSelectPar.Value;
            bank.oams[(int)numOAM.Value].obj1.flipX = (byte)(checkFlipX.Checked ? 1 : 0);
            bank.oams[(int)numOAM.Value].obj1.flipY = (byte)(checkFlipY.Checked ? 1 : 0);
            bank.oams[(int)numOAM.Value].obj1.size = (byte)numSize.Value;
            Update_Image();
        }
        private void Change_OBJ2(object sender, EventArgs e)
        {
            bank.oams[(int)numOAM.Value].obj2.tileOffset = (uint)numOffset.Value;
            bank.oams[(int)numOAM.Value].obj2.priority = (byte)numPrio.Value;
            bank.oams[(int)numOAM.Value].obj2.index_palette = (byte)numPal.Value;

            Update_Image();
        }
        private void Change_Preview(object sender, EventArgs e)
        {
            Update_Image();
        }

        private void comboSize_SelectedIndexChanged(object sender, EventArgs e)
        {
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
    }
}
