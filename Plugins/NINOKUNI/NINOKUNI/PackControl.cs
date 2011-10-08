using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace NINOKUNI
{
    public partial class PackControl : UserControl
    {
        String file;
        int id;
        IPluginHost pluginHost;

        public PackControl(string file, int id, IPluginHost pluginHost)
        {
            InitializeComponent();

            this.file = file;
            this.id = id;
            this.pluginHost = pluginHost;
        }

        private void btnPack_Click(object sender, EventArgs e)
        {
            string fileOut = pluginHost.Get_TempFolder() + System.IO.Path.DirectorySeparatorChar +
                "pack_" + System.IO.Path.GetFileName(file);
            NPCK.Pack(fileOut, id, pluginHost);
            pluginHost.ChangeFile(id, fileOut);
        }
    }
}
