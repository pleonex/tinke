/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 * Programador: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PluginInterface;

namespace Pack
{
    public partial class iNARC : UserControl
    {
        ARC narc;
        IPluginHost pluginHost;

        public iNARC()
        {
            InitializeComponent();
        }
        public iNARC(ARC narc, IPluginHost pluginHost)
        {
            InitializeComponent();
            this.narc = narc;
            this.pluginHost = pluginHost;
            ReadLanguage();

            lblNumFiles.Text = narc.btaf.nFiles.ToString();
        }
        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                    "Plugins" + Path.DirectorySeparatorChar + "NARCLang.xml");
                xml = xml.Element(pluginHost.Get_Language());

                btnCompress.Text = xml.Element("S00").Value;
                label1.Text = xml.Element("S01").Value;
            }
            catch
            {
                MessageBox.Show("There was an error reading the lang XML file.");
            }

        }

        private void btnCompress_Click(object sender, EventArgs e)
        {
            String narcFile = Path.GetTempFileName();
            Save_NARC(narcFile, pluginHost.Get_DecompressedFiles(narc.file_id));

            pluginHost.ChangeFile(narc.file_id, narcFile);
        }
        private void Save_NARC(string fileout, sFolder decompressed)
        {
            /* Structure of the file
             * 
             * Common header
             * 
             * BTAF section
             * |_ Start offset
             * |_ End offset
             * 
             * BTNF section
             * 
             * GMIF section
             * |_ Files
             * 
             */

            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));
            BinaryReader br = new BinaryReader(File.OpenRead(narc.file));

            // Write the BTAF section
            String btafTmp = Path.GetTempFileName();
            Write_BTAF(
                btafTmp,
                0x10 + narc.btaf.section_size + narc.btnf.section_size + 0x08,
                decompressed);

            // Write the BTNF section
            String btnfTmp = Path.GetTempFileName();
            br.BaseStream.Position = 0x10 + narc.btaf.section_size;
            File.WriteAllBytes(btnfTmp, br.ReadBytes((int)narc.btnf.section_size));

            // Write the GMIF section
            String gmifTmp = Path.GetTempFileName();
            Write_GMIF(gmifTmp, decompressed);

            // Write the NARC file
            int file_size = (int)(narc.header_size + narc.btaf.section_size + narc.btnf.section_size +
                narc.gmif.section_size);

            // Common header
            bw.Write(narc.id);
            bw.Write(narc.id_endian);
            bw.Write(narc.constant);
            bw.Write(file_size);
            bw.Write(narc.header_size);
            bw.Write(narc.nSections);
            // Write the sections
            bw.Write(File.ReadAllBytes(btafTmp));
            bw.Write(File.ReadAllBytes(btnfTmp));
            bw.Write(narc.gmif.id);
            bw.Write(narc.gmif.section_size);
            bw.Write(File.ReadAllBytes(gmifTmp));

            bw.Flush();
            bw.Close();
            br.Close();

            File.Delete(btafTmp);
            File.Delete(btnfTmp);
            File.Delete(gmifTmp);
        }
        private void Write_BTAF(string fileout, uint startOffset, sFolder decompressed)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));
            uint offset = 0;

            bw.Write(narc.btaf.id);
            bw.Write(narc.btaf.section_size);
            bw.Write(narc.btaf.nFiles);

            for (int i = 0; i < narc.btaf.nFiles; i++)
            {
                sFile currFile = Search_File(i + decompressed.id, decompressed);
                bw.Write(offset);
                offset += currFile.size;
                bw.Write(offset);
            }

            bw.Flush();
            bw.Close();
        }
        private void Write_GMIF(string fileout, sFolder decompressed)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            for (int i = 0; i < narc.btaf.nFiles; i++)
            {
                sFile currFile = Search_File(i + decompressed.id, decompressed);
                BinaryReader br = new BinaryReader(File.OpenRead(currFile.path));
                br.BaseStream.Position = currFile.offset;

                bw.Write(br.ReadBytes((int)currFile.size));
                br.Close();
                bw.Flush();
            }

            bw.Flush();
            bw.Close();
            narc.gmif.section_size = (uint)(new FileInfo(fileout).Length) + 0x08;
        }
        private sFile Search_File(int id, sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                    if (archivo.id == id)
                        return archivo;


            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    sFile currFile = Search_File(id, subFolder);
                    if (currFile.name is string)
                        return currFile;
                }
            }

            return new sFile();
        }

    }
}
