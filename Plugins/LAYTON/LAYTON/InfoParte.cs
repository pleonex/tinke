using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LAYTON
{
    public partial class InfoParte : UserControl
    {
        public InfoParte()
        {
            InitializeComponent();
        }

        public Ani.Parte Informacion
        {
            set
            {
                txtAltoP.Text = value.height.ToString();
                txtAnchoP.Text = value.width.ToString();
                txtPos.Text = value.offSet.ToString();
                txtPosX.Text = value.posX.ToString();
                txtPosY.Text = value.posY.ToString();
                txtTamanoP.Text = value.length.ToString();
            }
            get
            {
                Ani.Parte info = new Ani.Parte();

                info.height = Convert.ToUInt16(txtAltoP.Text);
                info.width = Convert.ToUInt16(txtAnchoP.Text);
                info.offSet = Convert.ToUInt64(txtPos.Text);
                info.posX = Convert.ToUInt16(txtPosX.Text);
                info.posY = Convert.ToUInt16(txtPosY.Text);

                return info;
            }
        }
    }
}
