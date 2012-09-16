// ----------------------------------------------------------------------
// <copyright file="TextControl.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>28/08/2012 12:18:58</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ekona;

namespace NINOKUNI.Text
{
    public partial class TextControl : UserControl
    {
        // To support a new file create the class and add it to Create_IText
        IText itext;
        IPluginHost pluginHost;
        string[] text;
        int id;
        string fileName;

        public TextControl()
        {
            InitializeComponent();
        }
        public TextControl(IPluginHost pluginHost, sFile file)
        {
            if (!IsSupported(file.id))
                throw new NotSupportedException();

            InitializeComponent();
            this.pluginHost = pluginHost;
            this.id = file.id;
            this.fileName = System.IO.Path.GetFileNameWithoutExtension(file.name);

            itext = Create_IText(id);
            itext.Read(file.path);
            text = itext.Get_Text();

            numBlock.Maximum = text.Length - 1;
            label3.Text = "of " + numBlock.Maximum.ToString();
            numBlock_ValueChanged(null, null);
        }

        public static IText Create_IText(int id)
        {
            IText it = null;
            switch (id)
            {
                case 0x69: it = new BattleHelp(); break;
                case 0x243F: it = new StageInfoData(); break;
                case 0x2A8D: it = new DownloadParam(); break;
                case 0x2A8E: it = new DungeonList(); break;

                case 0x6A: it = new BlockText(true, false, false, 0x30, 0x50, "DebugBattleSettings"); break;
                case 0x2A94: it = new BlockText(false, false, false, 0x40, 0, "ImagenArea"); break;
                case 0x2A95: it = new BlockText(false, false, true, 0x08, 0, "ImagenName"); break;
                case 0x2A96: it = new BlockText(true, true, true, 0x10, 0xA4, "ImagenParam"); break; 
                case 0x2A97: it = new BlockText(false, false, false, 0x100, 0, "ImagenText"); break;
                case 0x2A98: it = new BlockText(false, false, false, 0x72, 0, "EquipGetInfo"); break;
                case 0x2A99: it = new BlockText(false, false, false, 0xC4, 0, "EquipItemInfo"); break;
                case 0x2A9A: it = new BlockText(false, false, false, 0x51, 0, "EquipItemLinkInfo"); break;
                case 0x2A9B: it = new BlockText(true, true, true, 0x20, 0x30, "EquipItemParam"); break;
                case 0x2A9C: it = new BlockText(false, false, false, 0x72, 0, "ItemGetInfo"); break;
                case 0x2A9D: it = new BlockText(false, false, false, 0xC4, 0, "ItemInfo"); break;
                case 0x2A9E: it = new BlockText(false, false, false, 0x51, 0, "ItemLinkInfo"); break;
                case 0x2A9F: it = new BlockText(true, true, true, 0x20, 0x4C, "ItemParam"); break;
                case 0x2AA0: it = new BlockText(false, false, false, 0x72, 0, "SpItemGetInfo"); break;
                case 0x2AA1: it = new BlockText(false, false, false, 0xC4, 0, "SpItemInfo"); break;
                case 0x2AA2: it = new BlockText(true, true, true, 0x20, 0x10, "SpItemParam"); break;
                case 0x2AA3: it = new BlockText(true, false, false, 0xC4, 0, "MagicInfo"); break;
                case 0x2AA4: it = new BlockText(true, false, true, 0x12, 0x2E, "MagicParam"); break;
                case 0x2AA6: it = new BlockText(true, false, true, 0x11, 0x2C, "PlayerName"); break;
                case 0x2AAB: it = new BlockText(true, false, false, 0xC4, 0, "SkillInfo"); break;
                case 0x2AAC: it = new BlockText(true, false, true, 0x12, 0x2A, "SkillParam"); break;

                default: it = null; break;
            }
            return it;
        }
        public static bool IsSupported(int id)
        {
            if (Create_IText(id) == null)
                return false;
            else
                return true;
        }

        private void numBlock_ValueChanged(object sender, EventArgs e)
        {
            txtBlock.Text = text[(int)numBlock.Value].Replace("\n", "\r\n");
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.DefaultExt = ".xml";
            o.Filter = "eXtensive Markup Language (*.xml)|*.xml";
            o.FileName = fileName + ".xml";
            if (o.ShowDialog() != DialogResult.OK)
                return;

            itext.Export(o.FileName);

            o.Dispose();
            o = null;
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.AddExtension = true;
            o.DefaultExt = ".xml";
            o.Filter = "eXtensive Markup Language (*.xml)|*.xml";
            o.FileName = fileName + ".xml";
            if (o.ShowDialog() != DialogResult.OK)
                return;

            itext.Import(o.FileName);

            string outFile = pluginHost.Get_TempFile();
            itext.Write(outFile);
            pluginHost.ChangeFile(id, outFile);

            o.Dispose();
            o = null;

            text = itext.Get_Text();
            numBlock.Maximum = text.Length - 1;
            label3.Text = "of " + numBlock.Maximum.ToString();
            numBlock_ValueChanged(null, null);
        }
    }
}
