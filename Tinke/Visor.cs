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
    public partial class Visor : Form
    {
        public Visor()
        {
            InitializeComponent();

            this.Text = Tools.Helper.ObtenerTraduccion("Sistema", "S3C");
        }
    }
}
