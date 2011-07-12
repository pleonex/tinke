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
using PluginInterface;

namespace SDAT
{
    public partial class iSDAT : UserControl
    {
        sSDAT sdat;
        SoundPlayer soundPlayer;
        string wavFile = "";
        IPluginHost pluginHost;

        uint lastFolderID;
        uint lastFileID;

        public iSDAT()
        {
            InitializeComponent();
        }
        public iSDAT(sSDAT sdat, IPluginHost pluginHost)
        {
            InitializeComponent();

            this.sdat = sdat;
            this.pluginHost = pluginHost;

            treeFiles.Nodes.Add(CarpetaToNodo(sdat.files.root));
            treeFiles.Nodes[0].Expand();

            Set_LastFileID(sdat.files.root);
            Set_LastFolderID(sdat.files.root);
            lastFileID++;
            lastFolderID++;
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
                    return 7;
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
            btnUncompress.Enabled = false;
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
                else if (fileSelect.type == FormatSound.SWAR)
                {
                    btnUncompress.Enabled = true;
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

        private Sound SearchFile()
        {
            return SearchFile(Convert.ToInt32(treeFiles.SelectedNode.Tag), sdat.files.root);
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
        private void btnUncompress_Click(object sender, EventArgs e)
        {
            btnUncompress.Enabled = false;
            string swar = SaveSelectedFile();

            SWAV.ArchivoSWAV[] archivos = SWAR.ConvertirASWAV(SWAR.LeerArchivo(swar));
            string[] swav = new string[archivos.Length];
            
            Folder carpeta = new Folder();
            carpeta.name = SearchFile().name;
            carpeta.files = new List<Sound>();

            for (int i = 0; i < archivos.Length; i++)
            {
                swav[i] = pluginHost.Get_TempFolder() + '\\' + Path.GetRandomFileName();
                SWAV.EscribirArchivo(archivos[i], swav[i]);

                Sound newSound = new Sound();
                newSound.id = lastFileID;
                lastFileID++;
                newSound.internalID = newSound.id;
                newSound.name = "SWAV_" + i.ToString() + ".swav";
                newSound.offset = 0x00;
                newSound.type = FormatSound.SWAV;
                newSound.path = swav[i];
                newSound.size = (uint)new FileInfo(swav[i]).Length;

                carpeta.files.Add(newSound);
            }

            // Lo añadimos al nodo
            sdat.files.root = Add_Files(carpeta, (int)SearchFile().id, sdat.files.root);
            
            TreeNode selected =  treeFiles.SelectedNode;
            selected = CarpetaToNodo(carpeta);

            // Agregamos los nodos al árbol
            TreeNode[] nodos = new TreeNode[selected.Nodes.Count];
            selected.Nodes.CopyTo(nodos, 0);
            treeFiles.SelectedNode.Tag = selected.Tag;
            selected.Nodes.Clear();

            treeFiles.SelectedNode.Nodes.AddRange((TreeNode[])nodos);
            treeFiles.SelectedNode.Expand();

        }

        public Folder Add_Files(Folder files, int id, Folder currFolder)
        {
            if (currFolder.files is List<Sound>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == id)
                    {
                        files.id = (ushort)lastFolderID;
                        lastFolderID++;
                        currFolder.files.RemoveAt(i);
                        if (!(currFolder.folders is List<Folder>))
                            currFolder.folders = new List<Folder>();
                        currFolder.folders.Add(files);
                        return currFolder;
                    }
                }
            }


            if (currFolder.folders is List<Folder>)
            {
                foreach (Folder subFolder in currFolder.folders)
                {
                    Folder folder = Add_Files(files, id, subFolder);
                    if (folder.name is string)
                    {
                        currFolder.folders.Remove(subFolder);
                        currFolder.folders.Add(folder);
                        currFolder.folders.Sort(Comparacion_Directorios);
                        return currFolder;
                    }
                }
            }

            return new Folder();
        }
        private static int Comparacion_Directorios(Folder f1, Folder f2)
        {
            return String.Compare(f1.name, f2.name);
        }


        private void btnWav_Click(object sender, EventArgs e)
        {
            string swav = SaveSelectedFile();

            SaveFileDialog o = new SaveFileDialog();
            o.FileName = SearchFile().name.Replace(".SWAV", ".wav");
            o.Filter = "Sonido WAVE (*.wav)|*.wav";
            if (o.ShowDialog() == DialogResult.OK)
            {
                string wavSaved = o.FileName;
                WAV.EscribirArchivo(SWAV.ConvertirAWAV(SWAV.LeerArchivo(swav)), wavSaved);
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

            string swav = SaveSelectedFile();
            wavFile = Path.GetTempFileName();
            WAV.EscribirArchivo(SWAV.ConvertirAWAV(SWAV.LeerArchivo(swav)), wavFile);

            File.Delete(swav);

            soundPlayer = new SoundPlayer(wavFile);
            soundPlayer.Play();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (soundPlayer is SoundPlayer)
                soundPlayer.Stop();
        }

        private string SaveSelectedFile()
        {
            Sound fileSelect = SearchFile();

            if (fileSelect.offset == 0x00)
                return fileSelect.path;

            string file = Path.GetTempFileName();
            BinaryReader br = new BinaryReader(new FileStream(sdat.archivo, FileMode.Open));
            br.BaseStream.Position = fileSelect.offset;
            File.WriteAllBytes(file, br.ReadBytes((int)fileSelect.size));
            br.Close();

            return file;
        }

        public void Set_LastFileID(Folder currFolder)
        {
            if (currFolder.files is List<Sound>)
                foreach (Sound archivo in currFolder.files)
                    if (archivo.id > lastFileID)
                        lastFileID = archivo.id;

            if (currFolder.folders is List<Folder>)
                foreach (Folder subFolder in currFolder.folders)
                    Set_LastFileID(subFolder);

        }
        public void Set_LastFolderID(Folder currFolder)
        {
            if (currFolder.id > lastFolderID)
                lastFolderID = currFolder.id;

            if (currFolder.folders is List<Folder>)
                foreach (Folder subFolder in currFolder.folders)
                    Set_LastFolderID(subFolder);
        }

    }
}
