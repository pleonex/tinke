using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Nintendo
{
    public partial class iBMG : UserControl
    {
        sBMG bmg;
        string[] msg;

        public iBMG(sBMG bmg)
        {
            InitializeComponent();

            this.bmg = bmg;
            Informacion();
        }

        private void Informacion()
        {
            listProp.Items[0].SubItems.Add(bmg.uft16 ? "Sí" : "No");
            listProp.Items[2].SubItems.Add(bmg.inf1.nMsg.ToString());
            listProp.Items[3].SubItems.Add(bmg.inf1.offsetLength.ToString());
            listProp.Items[4].SubItems.Add(bmg.inf1.unknown1.ToString());
            listProp.Items[5].SubItems.Add(bmg.inf1.unknown2.ToString());

            msg = bmg.dat1.msgs;
            numericMsg.Maximum = bmg.inf1.nMsg;
            txtMsg.Text = msg[0];
        }

        private void numericMsg_ValueChanged(object sender, EventArgs e)
        {
            txtMsg.Text = msg[(int)numericMsg.Value - 1];
        }
    }
}
