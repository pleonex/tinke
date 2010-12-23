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
    public partial class Espera : Form
    {
        public Espera()
        {
            InitializeComponent();
        }
        public Espera(string label, bool step)
        {
            InitializeComponent();
            label1.Text = label;
            if (step)
                progressBar1.Style = ProgressBarStyle.Continuous;
        }
        public Espera(string label, int step)
        {
            InitializeComponent();
            label1.Text = label;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.Step = step;
        }

        public void Set_ProgressValue(int porcentaje)
        {
            progressBar1.Value = porcentaje;
        }
        public void Step()
        {
            progressBar1.PerformStep();
        }
    }
}
