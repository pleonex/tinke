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
 * Programa utilizado: Visual Studio 2010
 * Fecha: 24/06/2011
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
using System.Media;

namespace SDAT
{
    public partial class iSDAT : UserControl
    {
        sSDAT sdat;
        SoundPlayer soundPlayer;
        string wavFile = "";

        public iSDAT()
        {
            InitializeComponent();
        }
        public iSDAT(sSDAT sdat)
        {
            InitializeComponent();

            this.sdat = sdat;

            treeFiles.Nodes.Add(CarpetaToNodo(sdat.files.root));
            treeFiles.Nodes[0].Expand();
        }

        private TreeNode CarpetaToNodo(Folder carpeta)
        {
            TreeNode currNode = new TreeNode();

            currNode = new TreeNode(carpeta.name, 0, 0);
            currNode.Tag = carpeta.id;
            currNode.Name = carpeta.name;


            if (carpeta.folders is List<Folder>)
                foreach (Folder subFolder in carpeta.folders)
                    currNode.Nodes.Add(CarpetaToNodo(subFolder));


            if (carpeta.files is List<Sound>)
            {
                foreach (Sound archivo in carpeta.files)
                {
                    int formato = FormatoToImage(archivo.type);
                    TreeNode fileNode = new TreeNode(archivo.name, formato, formato);
                    fileNode.Name = archivo.name;
                    fileNode.Tag = archivo.id;
                    currNode.Nodes.Add(fileNode);
                }
            }

            return currNode;

        }
        private int FormatoToImage(FormatSound formato)
        {
            switch (formato)
            {
                case FormatSound.SSEQ:
                    return 2;
                case FormatSound.SSAR:
                    return 2;
                case FormatSound.SBNK:
                    return 4;
                case FormatSound.SWAV:
                    return 1;
                case FormatSound.SWAR:
                    return 1;
                case FormatSound.STRM:
                    return 1;
                default:
                    return 1;
            }
        }

        private void treeFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
             // Limpiar información anterior
            btnReproducir.Enabled = false;
            btnWav.Enabled = false;
            btnMidi.Enabled = false;
            if (listProp.Items[0].SubItems.Count == 2)
                for (int i = 0; i < listProp.Items.Count; i++)
                    listProp.Items[i].SubItems.RemoveAt(1);

            int id = Convert.ToInt32(e.Node.Tag);
            if (id < 0x0F00)
            {
                Sound fileSelect = SearchFile(id, sdat.files.root);

                listProp.Items[0].SubItems.Add("0x" + String.Format("{0:X}", fileSelect.id));
                listProp.Items[1].SubItems.Add(fileSelect.name);
                listProp.Items[2].SubItems.Add("0x" + String.Format("{0:X}", fileSelect.offset));
                listProp.Items[3].SubItems.Add(fileSelect.size.ToString());

                btnExtract.Enabled = true;
                if (fileSelect.type == FormatSound.SWAV)
                {
                    btnReproducir.Enabled = true;
                    btnWav.Enabled = true;
                }
            }
            else
            {
                Folder folSelect = SearchFolder(id, sdat.files.root);

                listProp.Items[0].SubItems.Add("0x" + String.Format("{0:X}", folSelect.id));
                listProp.Items[1].SubItems.Add(folSelect.name);
                listProp.Items[2].SubItems.Add("");
                listProp.Items[3].SubItems.Add("");

                btnExtract.Enabled = true;
            }
        }

        private Sound SearchFile(int id, Folder carpeta)
        {
            if (carpeta.files is List<Sound>)
                foreach (Sound archivo in carpeta.files)
                    if (archivo.id == id)
                        return archivo;


            if (carpeta.folders is List<Folder>)
            {
                foreach (Folder subFolder in carpeta.folders)
                {
                    Sound currFile = SearchFile(id, subFolder);
                    if (currFile.name is String)
                        return currFile;
                }
            }


            return new Sound();
        }
        private Folder SearchFolder(int id, Folder carpeta)
        {
            if (carpeta.id == id)
                return carpeta;

            if (carpeta.folders is List<Folder>)
            {
                foreach (Folder subFolder in carpeta.folders)
                {
                    Folder currFolder = SearchFolder(id, subFolder);
                    if (currFolder.name is String)
                        return currFolder;
                }
            }


            return new Folder();
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(treeFiles.SelectedNode.Tag);
            if (id < 0x0F00)
            {
                Sound currFile = SearchFile(id, sdat.files.root);

                SaveFileDialog o = new SaveFileDialog();
                o.FileName = currFile.name;
                if (o.ShowDialog() == DialogResult.OK)
                {
                    BinaryReader br = new BinaryReader(new FileStream(sdat.archivo, FileMode.Open));
                    br.BaseStream.Position = currFile.offset;
                    File.WriteAllBytes(o.FileName, br.ReadBytes((int)currFile.size));
                    br.Close();
                }
            }
            else
            {
                Folder currFolder = SearchFolder(id, sdat.files.root);

                FolderBrowserDialog o = new FolderBrowserDialog();
                o.ShowNewFolderButton = true;
                o.Description = "Seleccione la carpeta donde se guardarán los archivos.";
                if (o.ShowDialog() == DialogResult.OK)
                {
                    Directory.CreateDirectory(o.SelectedPath + '\\' + currFolder.name);
                    RecursivoExtractFolder(currFolder,o.SelectedPath + '\\' + currFolder.name);
                }
            }
        }
        private void RecursivoExtractFolder(Folder currFolder, String path)
        {
            if (currFolder.files is List<Sound>)
                foreach (Sound archivo in currFolder.files)
                {
                        BinaryReader br = new BinaryReader(File.OpenRead(sdat.archivo));
                        br.BaseStream.Position = archivo.offset;
                        File.WriteAllBytes(path + '\\' + archivo.name, br.ReadBytes((int)archivo.size));
                        br.Close();
                }

            if (currFolder.folders is List<Folder>)
            {
                foreach (Folder subFolder in currFolder.folders)
                {
                    Directory.CreateDirectory(path + '\\' + subFolder.name);
                    RecursivoExtractFolder(subFolder, path + '\\' + subFolder.name);
                }
            }
        }

        private void btnWav_Click(object sender, EventArgs e)
        {
            string swav = Path.GetTempFileName();
            Sound fileSelect = SearchFile(Convert.ToInt32(treeFiles.SelectedNode.Tag), sdat.files.root);
            BinaryReader br = new BinaryReader(new FileStream(sdat.archivo, FileMode.Open));
            br.BaseStream.Position = fileSelect.offset;
            File.WriteAllBytes(swav, br.ReadBytes((int)fileSelect.size));
            br.Close();

            SaveFileDialog o = new SaveFileDialog();
            o.FileName = fileSelect.name.Replace(".SWAV", ".wav");
            o.Filter = "Sonido WAVE (*.wav)|*.wav";
            if (o.ShowDialog() == DialogResult.OK)
            {
                string wavSaved = o.FileName;
                SWAV.EscribirArchivo(SWAV.ConvertirSWAVaWAV(SWAV.LeerSWAV(swav)), wavSaved);
            }

            File.Delete(swav);
        }
        private void btnReproducir_Click(object sender, EventArgs e)
        {
            if (soundPlayer is SoundPlayer)
            {
                soundPlayer.Stop();
                soundPlayer.Dispose();
            }
            if (wavFile != "")
                File.Delete(wavFile);

            string swav = Path.GetTempFileName();
            Sound fileSelect = SearchFile(Convert.ToInt32(treeFiles.SelectedNode.Tag), sdat.files.root);
            BinaryReader br = new BinaryReader(new FileStream(sdat.archivo, FileMode.Open));
            br.BaseStream.Position = fileSelect.offset;
            File.WriteAllBytes(swav, br.ReadBytes((int)fileSelect.size));
            br.Close();

            wavFile = Path.GetTempFileName();
            SWAV.EscribirArchivo(SWAV.ConvertirSWAVaWAV(SWAV.LeerSWAV(swav)), wavFile);

            File.Delete(swav);

            if (soundPlayer is SoundPlayer)
                soundPlayer.Stop();
            soundPlayer = new SoundPlayer(wavFile);
            soundPlayer.Play();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (soundPlayer is SoundPlayer)
                soundPlayer.Stop();
        }

    }
}
