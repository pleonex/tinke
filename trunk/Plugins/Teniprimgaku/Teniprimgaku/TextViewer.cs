using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ekona;
using System.IO;

namespace Teniprimgaku
{
    public partial class TextViewer : UserControl
    {
        public TextViewer()
        {
            InitializeComponent();
        }
        public TextViewer(sFile file)
        {
            InitializeComponent();

            this.txtBox.Text = File.ReadAllText(file.path, Encoding.GetEncoding("shift_jis"));
        }
    }
}
