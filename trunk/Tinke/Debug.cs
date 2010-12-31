using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tinke
{
    public partial class Debug : Form
    {
        public Debug()
        {
            InitializeComponent();
            this.Location = new Point(10, 585);
        }

        public void Añadir_Texto(string mensaje)
        {
            if (mensaje != "")
                txtInfo.Text += mensaje + "\r\n";
        }
    }
}
