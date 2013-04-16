using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ekona;

namespace TIMEACE
{
    public partial class TextEditor : UserControl
    {
        IPluginHost pluginHost;
        int id;

        public TextEditor()
        {
            InitializeComponent();
        }
        public TextEditor(IPluginHost pluginHost, sFile file)
            : this()
        {
            this.pluginHost = pluginHost;
            this.id = file.id;
            this.txtText.Text = File.ReadAllText(file.path);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string tmpFile = this.pluginHost.Get_TempFile();
            File.WriteAllText(tmpFile, this.txtText.Text);

            this.pluginHost.ChangeFile(this.id, tmpFile);
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.DefaultExt = ".txt";
                ofd.Filter = "Plain text file (*.txt)|*.txt";
                ofd.CheckFileExists = true;
                ofd.Multiselect = false;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    this.txtText.Text = File.ReadAllText(ofd.FileName);
                    this.btnSave.PerformClick();
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Plain text file (*.txt)|*.txt";
                sfd.AddExtension = true;
                sfd.OverwritePrompt = true;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, this.txtText.Text);
                }
            }
        }
    }
}
