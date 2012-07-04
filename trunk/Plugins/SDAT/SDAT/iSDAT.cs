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
using System.Media;
using System.Threading;
using Ekona;

namespace SDAT
{
    public partial class iSDAT : UserControl
    {
        sSDAT sdat;
        SoundPlayer soundPlayer;
        string wavFile = "";
        string loopFile = "";
        Thread bgdWorker;
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
            ReadLanguage();

            if (sdat.files.root.name == "SWAV" || sdat.files.root.name == "STRM")
                btnCreate.Enabled = false;
            else
                Add_Group();

            treeFiles.Nodes.Add(CarpetaToNodo(sdat.files.root));
            treeFiles.Nodes[0].Expand();

            Set_LastFileID(sdat.files.root);
            Set_LastFolderID(sdat.files.root);
            lastFileID++;
            lastFolderID++;
        }
        private void ReadLanguage()
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                    "Plugins" + Path.DirectorySeparatorChar + "SDATLang.xml");
                xml = xml.Element(pluginHost.Get_Language());
                xml = xml.Element("iSDAT");

                columnCampo.Text = xml.Element("S00").Value;
                columnValor.Text = xml.Element("S01").Value;
                listProp.Items[0].Text = xml.Element("S02").Value;
                listProp.Items[1].Text = xml.Element("S03").Value;
                listProp.Items[2].Text = xml.Element("S04").Value;
                listProp.Items[3].Text = xml.Element("S05").Value;
                listProp.Items[4].Text = xml.Element("S11").Value;
                checkLoop.Text = xml.Element("S06").Value;
                btnUncompress.Text = xml.Element("S07").Value;
                btnExtract.Text = xml.Element("S08").Value;
                btnMidi.Text = xml.Element("S09").Value;
                btnWav.Text = xml.Element("S0A").Value;
                btnImport.Text = xml.Element("S0B").Value;
                btnCreate.Text = xml.Element("S0C").Value;
                btnChangeFile.Text = xml.Element("S0D").Value;
                btnInfo.Text = xml.Element("S0E").Value;
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }
        }

        #region System folder administration
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
                    //return 2; No soportado este formato aún
                    return 8;
                case FormatSound.SSAR:
                    //return 2; No soportado este formato aún
                    return 8;
                case FormatSound.SBNK:
                    //return 4; No soportado este formato aún
                    return 8;
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
            System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                "Plugins" + Path.DirectorySeparatorChar + "SDATLang.xml");
            xml = xml.Element(pluginHost.Get_Language());
            xml = xml.Element("iSDAT");

            // Limpiar información anterior
            btnReproducir.Enabled = false;
            btnWav.Enabled = false;
            btnMidi.Enabled = false;
            btnImport.Enabled = false;
            btnUncompress.Enabled = false;
            btnInfo.Enabled = false;
            btnChangeFile.Enabled = true;
            if (listProp.Items[0].SubItems.Count == 2)
                for (int i = 0; i < listProp.Items.Count; i++)
                    listProp.Items[i].SubItems.RemoveAt(1);

            int id = Convert.ToInt32(e.Node.Tag);
            if (id < 0x0F000)
            {
                Sound fileSelect = SearchFile(id, sdat.files.root);

                listProp.Items[0].SubItems.Add("0x" + String.Format("{0:X}", fileSelect.id));
                listProp.Items[1].SubItems.Add(fileSelect.name);
                listProp.Items[2].SubItems.Add("0x" + String.Format("{0:X}", fileSelect.offset));
                listProp.Items[3].SubItems.Add(fileSelect.size.ToString());
                listProp.Items[4].SubItems.Add(fileSelect.path);

                btnExtract.Enabled = true;
                if (fileSelect.type == FormatSound.SWAV || fileSelect.type == FormatSound.STRM)
                {
                    btnReproducir.Enabled = true;
                    btnWav.Enabled = true;
                    btnInfo.Enabled = true;
                    checkLoop.Enabled = true;
                    btnImport.Enabled = true;
                }
                else if (fileSelect.type == FormatSound.SWAR)
                {
                    btnUncompress.Enabled = true;
                    if ((String)fileSelect.tag == "Descomprimido")
                        btnUncompress.Text = xml.Element("S12").Value;
                    else
                        btnUncompress.Text = xml.Element("S07").Value;
                    btnInfo.Enabled = true;
                }
            }
            else
            {
                Folder folSelect = SearchFolder(id, sdat.files.root);

                listProp.Items[0].SubItems.Add("0x" + String.Format("{0:X}", folSelect.id));
                listProp.Items[1].SubItems.Add(folSelect.name);
                listProp.Items[2].SubItems.Add("");
                listProp.Items[3].SubItems.Add("");
                listProp.Items[4].SubItems.Add("");

                btnExtract.Enabled = true;
                btnChangeFile.Enabled = false;

                if ((String)folSelect.tag == "Descomprimido")
                {
                    btnUncompress.Enabled = true;
                    btnUncompress.Text = xml.Element("S12").Value;
                }
            }
        }

        private Sound SearchFile()
        {
            return SearchFile(Convert.ToInt32(treeFiles.SelectedNode.Tag), sdat.files.root);
        }
        private Sound SearchFile(int id, Folder carpeta)
        {
            if (carpeta.id == id) // Archivos descomprimidos
            {
                Sound folderFile = new Sound();
                folderFile.name = carpeta.name;
                folderFile.id = carpeta.id;
                if (((String)carpeta.tag).Length != 16)
                    folderFile.path = ((string)carpeta.tag).Substring(8);
                else
                    folderFile.offset = Convert.ToUInt32(((String)carpeta.tag).Substring(8), 16);
                folderFile.size = Convert.ToUInt32(((String)carpeta.tag).Substring(0, 8), 16);
                folderFile.type = (carpeta.name.ToUpper().EndsWith(".SWAR") ? FormatSound.SWAR : FormatSound.SSAR);
                folderFile.tag = "Descomprimido"; // Tag para indicar que ya ha sido procesado

                return folderFile;
            }

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
                    if (subFolder.id == id)
                        return subFolder;

                    Folder folder = SearchFolder(id, subFolder);
                    if (folder.name is string)
                        return folder;
                }
            }

            return new Folder();
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

        public void Add_Files(ref Folder files, int id)
        {
            Sound file = SearchFile(id, sdat.files.root); // Archivo descomprimido
            files.name = file.name;
            sdat.files.root = FileToFolder((int)file.id, sdat.files.root);
            // Archivo convertido a medias a carpeta ;)
            files.id = file.id;
            if (file.offset != 0x00) // es medio archivo :)
                files.tag = String.Format("{0:X}", file.size).PadLeft(8, '0') +
                    String.Format("{0:X}", file.offset).PadLeft(8, '0');
            else
                files.tag = String.Format("{0:X}", file.size).PadLeft(8, '0') +
                                file.path;

            files = Add_ID(files);
            Add_Files(files, (ushort)file.id, sdat.files.root);
        }
        public Folder FileToFolder(int id, Folder currFolder)
        {
            if (currFolder.files is List<Sound>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == id)
                    {
                        Folder newFolder = new Folder();
                        newFolder.name = currFolder.files[i].name;
                        newFolder.id = currFolder.files[i].id;
                        if (currFolder.files[i].offset != 0x00) // es medio archivo :)
                            newFolder.tag = String.Format("{0:X}", currFolder.files[i].size).PadLeft(8, '0') +
                                String.Format("{0:X}", currFolder.files[i].offset).PadLeft(8, '0');
                        else
                            newFolder.tag = String.Format("{0:X}", currFolder.files[i].size).PadLeft(8, '0') +
                                            currFolder.files[i].path;

                        currFolder.files.RemoveAt(i);
                        if (!(currFolder.folders is List<Folder>))
                            currFolder.folders = new List<Folder>();
                        currFolder.folders.Add(newFolder);
                        return currFolder;
                    }
                }
            }


            if (currFolder.folders is List<Folder>)
            {
                foreach (Folder subFolder in currFolder.folders)
                {
                    Folder folder = FileToFolder(id, subFolder);
                    if (folder.name is string)
                    {
                        currFolder.folders.Remove(subFolder);
                        currFolder.folders.Add(folder);
                        return currFolder;
                    }
                }
            }

            return new Folder();
        }
        private Folder Add_ID(Folder currFolder)
        {
            if (currFolder.files is List<Sound>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    Sound currFile = currFolder.files[i];
                    currFile.id = (ushort)lastFileID;
                    currFolder.files.RemoveAt(i);
                    currFolder.files.Insert(i, currFile);
                    lastFileID++;
                }
            }

            if (currFolder.folders is List<Folder>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    Folder newFolder = Add_ID(currFolder.folders[i]);
                    newFolder.name = currFolder.folders[i].name;
                    newFolder.id = (ushort)lastFolderID;
                    lastFolderID++;
                    currFolder.folders.RemoveAt(i);
                    currFolder.folders.Insert(i, newFolder);
                }
            }

            return currFolder;
        }
        private static int Comparacion_Directorios(Folder f1, Folder f2)
        {
            return String.Compare(f1.name, f2.name);
        }

        public Folder Add_Files(Folder files, ushort idFolder, Folder currFolder)
        {
            if (currFolder.id == idFolder)
            {
                currFolder = files;
                return currFolder;
            }

            if (currFolder.folders is List<Folder>)   // Si tiene subdirectorios, buscamos en cada uno de ellos
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    Folder folder = Add_Files(files, idFolder, currFolder.folders[i]);
                    if (folder.name is string)  // Comprobamos que se haya devuelto un directorio, en cuyo caso es el buscado que lo devolvemos
                    {
                        currFolder.folders[i] = folder;
                        return currFolder;
                    }
                }
            }

            return new Folder();
        }
        public void ChangeFile(int id, Sound fileChanged, Folder currFolder)
        {
            if (currFolder.files is List<Sound>)
            {
                for (int i = 0; i < currFolder.files.Count; i++)
                {
                    if (currFolder.files[i].id == id)
                    {
                        currFolder.files[i] = fileChanged;
                    }
                }
            }


            if (currFolder.folders is List<Folder>)
            {
                for (int i = 0; i < currFolder.folders.Count; i++)
                {
                    if (currFolder.folders[i].id == id) // It's a decompressed file
                    {
                        Folder newFolder = currFolder.folders[i];
                        newFolder.tag = fileChanged.tag;
                        currFolder.folders[i] = newFolder;
                    }
                    else
                        ChangeFile(id, fileChanged, currFolder.folders[i]);
                }
            }
        }
        public void ChangeFile(int id, string newFilePath)
        {
            Sound newFile = new Sound();
            Sound oldFile = SearchFile(id, sdat.files.root);
            newFile.name = oldFile.name;
            newFile.id = (ushort)id;
            newFile.offset = 0x00;
            newFile.path = newFilePath;
            newFile.type = oldFile.type;
            newFile.size = (uint)new FileInfo(newFilePath).Length;
            newFile.tag = oldFile.tag;
            if ((String)newFile.tag == "Descomprimido")
                newFile.tag = String.Format("{0:X}", newFile.size).PadLeft(8, '0') + newFile.path;

            ChangeFile(id, newFile, sdat.files.root);

            if (sdat.files.root.name == "SWAV")
                pluginHost.ChangeFile(sdat.id, newFile.path);
        }
        #endregion

        private void btnCreate_Click(object sender, EventArgs e)
        {
            String fileout = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            Save_NewSDAT(fileout);
            sdat.archivo = fileout;

            pluginHost.ChangeFile(sdat.id, fileout);
        }
        private void btnExtract_Click(object sender, EventArgs e)
        {
            System.Xml.Linq.XElement xml;

            int id = Convert.ToInt32(treeFiles.SelectedNode.Tag);
            if (id < 0x0F000)
            {
                if ((String)SearchFile().tag == "Descomprimido")
                {
                    xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                        "Plugins" + Path.DirectorySeparatorChar + "SDATLang.xml");
                    xml = xml.Element(pluginHost.Get_Language());
                    xml = xml.Element("iSDAT");

                    if (MessageBox.Show(xml.Element("S10").Value, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == System.Windows.Forms.DialogResult.Yes)
                        goto Folder_Extract;
                }

                Sound currFile = SearchFile(id, sdat.files.root);

                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.FileName = currFile.name;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (currFile.offset == 0x00)
                    {
                        File.Copy(currFile.path, fileDialog.FileName, true);
                        return;
                    }

                    BinaryReader br = new BinaryReader(new FileStream(sdat.archivo, FileMode.Open));
                    br.BaseStream.Position = currFile.offset;
                    File.WriteAllBytes(fileDialog.FileName, br.ReadBytes((int)currFile.size));
                    br.Close();
                }

                return;
            }

        Folder_Extract:
            Folder currFolder = SearchFolder(id, sdat.files.root);

            xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                "Plugins" + Path.DirectorySeparatorChar + "SDATLang.xml");
            xml = xml.Element(pluginHost.Get_Language());
            xml = xml.Element("iSDAT");

            FolderBrowserDialog o = new FolderBrowserDialog();
            o.ShowNewFolderButton = true;
            o.Description = xml.Element("S0F").Value; ;
            if (o.ShowDialog() == DialogResult.OK)
            {
                String folderName = currFolder.name;
                foreach (char c in Path.GetInvalidPathChars())
                    folderName = folderName.Replace(c.ToString(), "");

                if (!o.SelectedPath.EndsWith("\\"))
                    folderName = o.SelectedPath + '\\' + folderName;
                else
                    folderName = o.SelectedPath + folderName;

                Directory.CreateDirectory(folderName);
                RecursivoExtractFolder(currFolder, folderName);
            }
        }
        private void RecursivoExtractFolder(Folder currFolder, String path)
        {
            if (currFolder.files is List<Sound>)
                foreach (Sound archivo in currFolder.files)
                {
                    BinaryReader br = new BinaryReader(File.OpenRead(sdat.archivo));
                    br.BaseStream.Position = archivo.offset;

                    String fileName = archivo.name;
                    foreach (char c in Path.GetInvalidFileNameChars())
                        fileName = fileName.Replace(c.ToString(), "");
                    fileName = path + Path.DirectorySeparatorChar + fileName;

                    File.WriteAllBytes(fileName, br.ReadBytes((int)archivo.size));
                    br.Close();
                }

            if (currFolder.folders is List<Folder>)
            {
                foreach (Folder subFolder in currFolder.folders)
                {
                    string folderName = subFolder.name;
                    foreach (char c in Path.GetInvalidPathChars())
                        folderName = folderName.Replace(c.ToString(), "");
                    folderName = path + Path.DirectorySeparatorChar + folderName;

                    Directory.CreateDirectory(folderName);
                    RecursivoExtractFolder(subFolder, folderName);
                }
            }
        }
        private void btnUncompress_Click(object sender, EventArgs e)
        {
            System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                "Plugins" + Path.DirectorySeparatorChar + "SDATLang.xml");
            xml = xml.Element(pluginHost.Get_Language());
            xml = xml.Element("iSDAT");

            if (btnUncompress.Text == xml.Element("S07").Value)
            {
                #region Unpack SWAR file
                string swar = SaveFile();

                sSWAV[] archivos = SWAR.ConvertToSWAV(SWAR.Read(swar));
                string[] swav = new string[archivos.Length];

                Folder carpeta = new Folder();
                carpeta.name = SearchFile().name;
                carpeta.files = new List<Sound>();

                for (int i = 0; i < archivos.Length; i++)
                {
                    swav[i] = pluginHost.Get_TempFolder() + '\\' + Path.GetRandomFileName();
                    SWAV.Write(archivos[i], swav[i]);

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
                Add_Files(ref carpeta, (int)SearchFile().id);

                TreeNode selected = treeFiles.SelectedNode;
                selected = CarpetaToNodo(carpeta);

                // Agregamos los nodos al árbol
                TreeNode[] nodos = new TreeNode[selected.Nodes.Count];
                selected.Nodes.CopyTo(nodos, 0);
                treeFiles.SelectedNode.Tag = selected.Tag;
                selected.Nodes.Clear();

                treeFiles.SelectedNode.Nodes.AddRange((TreeNode[])nodos);
                treeFiles.SelectedNode.Expand();
                #endregion
                btnUncompress.Text = xml.Element("S12").Value;
            }
            else
            {
                #region Pack SWAR file
                Folder swar = SearchFolder(Convert.ToInt32(treeFiles.SelectedNode.Tag), sdat.files.root);
                List<sSWAV> sounds = new List<sSWAV>();

                for (int i = 0; i < swar.files.Count; i++)
                {
                    String tempFile = SaveFile((int)swar.files[i].id);
                    sSWAV swav = SWAV.Read(tempFile);
                    sounds.Add(swav);
                    File.Delete(tempFile);
                }

                String fileout = Path.GetTempFileName();
                SWAR.Write(sounds.ToArray(), fileout);
                ChangeFile(Convert.ToInt32(treeFiles.SelectedNode.Tag), fileout);
                #endregion
            }

        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            Sound selectedFile = SearchFile();
            String fileout = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + Path.GetRandomFileName();

            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.AddExtension = true;
            o.DefaultExt = "wav";
            o.Filter = "WAVE audio format (*.wav)|*.wav";
            if (o.ShowDialog() != DialogResult.OK)
                return;
            String filein = o.FileName;

            sWAV wav = new sWAV();
            try { wav = WAV.Read(filein); }
            catch (Exception ex) { MessageBox.Show(ex.Message); return; }

            NewAudioOptions dialog = new NewAudioOptions(pluginHost, (selectedFile.type == FormatSound.SWAV ? true : false));

            switch (selectedFile.type)
            {
                case FormatSound.STRM:
                    sSTRM oldStrm = STRM.Read(SaveFile());
                    dialog.Compression = oldStrm.head.waveType;
                    dialog.BlockSize = (int)oldStrm.head.blockLen;
                    dialog.Loop = (oldStrm.head.loop != 0 ? true : false);
                    dialog.LoopOffset = (int)oldStrm.head.loopOffset;
                    dialog.SampleRate = (int)wav.wave.fmt.sampleRate;
                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;

                    sSTRM strm = STRM.ConvertToSTRM(wav, dialog.Compression);
                    strm.head.loop = (byte)(dialog.Loop ? 0x01 : 0x00);
                    strm.head.loopOffset = (uint)dialog.LoopOffset;

                    STRM.Write(strm, fileout);
                    break;

                case FormatSound.SWAV:
                    sSWAV oldSwav = SWAV.Read(SaveFile());
                    dialog.Compression = oldSwav.data.info.nWaveType;
                    dialog.Loop = (oldSwav.data.info.bLoop != 0 ? true : false);
                    dialog.LoopLength = (int)oldSwav.data.info.nNonLoopLen;
                    dialog.LoopOffset = (int)oldSwav.data.info.nLoopOffset;
                    dialog.SampleRate = (int)wav.wave.fmt.sampleRate;
                    if (dialog.ShowDialog() != DialogResult.OK)
                        return;

                    sSWAV swav = SWAV.ConvertToSWAV(wav, dialog.Compression, dialog.Volume);

                    swav.data.info.bLoop = (byte)(dialog.Loop ? 0x01 : 0x00);
                    swav.data.info.nLoopOffset = (ushort)dialog.LoopOffset;
                    swav.data.info.nNonLoopLen = (uint)dialog.LoopLength;

                    SWAV.Write(swav, fileout);
                    break;
                case FormatSound.SSEQ:
                case FormatSound.SSAR:
                case FormatSound.SBNK:
                case FormatSound.SWAR:
                default:
                    break;
            }

            if (!File.Exists(fileout))
                return;
            ChangeFile((int)selectedFile.id, fileout);
        }
        private void btnChangeFile_Click(object sender, EventArgs e)
        {
            Sound selectedFile = SearchFile();

            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.AddExtension = true;
            o.DefaultExt = "strm";
            o.Filter = "STRM (*.strm)|*.strm|" +
                "SSEQ (*.sseq)|*.sseq|" +
                "SSAR (*.ssar)|*.ssar|" +
                "SBNK (*.sbnk)|*.sbnk|" +
                "SWAR (*.swar)|*.swar|" +
                "SWAV (*.swav)|*.swav|" +
                "All files (*.*)|*.*";

            switch (selectedFile.type)
            {
                case FormatSound.SSEQ:
                    o.FilterIndex = 2;
                    break;
                case FormatSound.SSAR:
                    o.FilterIndex = 3;
                    break;
                case FormatSound.SBNK:
                    o.FilterIndex = 4;
                    break;
                case FormatSound.SWAV:
                    o.FilterIndex = 6;
                    break;
                case FormatSound.SWAR:
                    o.FilterIndex = 5;
                    break;
                case FormatSound.STRM:
                    o.FilterIndex = 1;
                    break;
                default:
                    o.FilterIndex = 7;
                    break;
            }

            if (o.ShowDialog() != DialogResult.OK)
                return;

            ChangeFile((int)selectedFile.id, o.FileName);
        }

        private void btnWav_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            string sound = SaveFile();

            SaveFileDialog o = new SaveFileDialog();
            o.FileName = SearchFile().name;
            o.Filter = "WAVE (*.wav)|*.wav";
            if (o.ShowDialog() == DialogResult.OK)
            {
                string wavSaved = o.FileName;

                switch (SearchFile().type)
                {
                    case FormatSound.SWAV:
                        WAV.Write(SWAV.ConvertToWAV(SWAV.Read(sound), false), wavSaved);
                        break;
                    case FormatSound.STRM:
                        WAV.Write(STRM.ConvertToWAV(STRM.Read(sound), false), wavSaved);
                        break;
                }
            }

            File.Delete(sound);
            this.Cursor = Cursors.Default;
        }
        private void btnReproducir_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnStop.PerformClick();

                if (File.Exists(wavFile))
                    File.Delete(wavFile);
                if (File.Exists(loopFile))
                    File.Delete(loopFile);

                string sound = SaveFile();
                wavFile = Path.GetTempFileName();
                if (checkLoop.Checked)
                    loopFile = Path.GetTempFileName();

                switch (SearchFile().type)
                {
                    case FormatSound.SWAV:
                        WAV.Write(SWAV.ConvertToWAV(SWAV.Read(sound), false), wavFile);
                        if (checkLoop.Checked)
                            WAV.Write(SWAV.ConvertToWAV(SWAV.Read(sound), true), loopFile);
                        break;
                    case FormatSound.STRM:
                        WAV.Write(STRM.ConvertToWAV(STRM.Read(sound), false), wavFile);
                        if (checkLoop.Checked)
                            WAV.Write(STRM.ConvertToWAV(STRM.Read(sound), true), loopFile);
                        break;
                }

                File.Delete(sound);

                if (checkLoop.Checked)
                {
                    bgdWorker = new Thread(bgdWorker_DoWork);
                    bgdWorker.Start(new String[] { wavFile, loopFile });
                }
                else
                {
                    soundPlayer = new SoundPlayer(wavFile);
                    soundPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
        private void treeFiles_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (btnReproducir.Enabled == true)
                btnReproducir.PerformClick();
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (soundPlayer is SoundPlayer)
                soundPlayer.Stop();
            if (bgdWorker is Thread)
                if (bgdWorker.ThreadState == ThreadState.Running)
                    bgdWorker.Abort();
        }
        void bgdWorker_DoWork(object files)
        {
            string wave = ((String[])files)[0];
            string loopWave = ((String[])files)[1];

            SoundPlayer soundLoop = new SoundPlayer(loopWave);
            soundPlayer = new SoundPlayer(wave);

            soundPlayer.PlaySync();
            soundLoop.PlayLooping();
        }

        private string SaveFile()
        {
            Sound fileSelect = SearchFile();
            string file = Path.GetTempFileName();

            if (fileSelect.offset == 0x00)
            {
                File.Copy(fileSelect.path, file, true);
                return file;
            }

            BinaryReader br = new BinaryReader(new FileStream(sdat.archivo, FileMode.Open));
            br.BaseStream.Position = fileSelect.offset;
            File.WriteAllBytes(file, br.ReadBytes((int)fileSelect.size));
            br.Close();

            return file;
        }
        private string SaveFile(int id)
        {
            Sound fileSelect = SearchFile(id, sdat.files.root);
            string file = Path.GetTempFileName();

            if (fileSelect.offset == 0x00)
            {
                File.Copy(fileSelect.path, file, true);
                return file;
            }

            BinaryReader br = new BinaryReader(new FileStream(sdat.archivo, FileMode.Open));
            br.BaseStream.Position = fileSelect.offset;
            File.WriteAllBytes(file, br.ReadBytes((int)fileSelect.size));
            br.Close();

            return file;
        }

        private void Save_NewSDAT(string fileout)
        {
            /*
             * File format *
             * 
             * Header
             * |_ Offset of all sections
             * 
             * Symbol block
             *
             * 
             * Info block
             * |_ Records
             * |_ Entries
             * 
             * FAT block
             * |_ Offset
             * |_ Size
             * 
             * File block
             * |_ Files...
             */

            BinaryReader br = new BinaryReader(File.OpenRead(sdat.archivo));

            // Symbol section
            String symbTemp = Path.GetTempFileName();
            br.BaseStream.Position = sdat.cabecera.symbOffset;
            File.WriteAllBytes(symbTemp, br.ReadBytes((int)sdat.symbol.size));

            // Info section
            String infoTemp = Path.GetTempFileName();
            br.BaseStream.Position = sdat.cabecera.infoOffset;
            File.WriteAllBytes(infoTemp, br.ReadBytes((int)sdat.info.header.size));
            br.Close();

            // FAT section
            String fatTemp = Path.GetTempFileName();
            Write_FAT(fatTemp, sdat.cabecera.fileOffset + 0x10);

            // File section
            String fileTemp = Path.GetTempFileName();
            Write_Files(fileTemp); // File without header

            // Write the new SDAT file
            int file_size = (int)(sdat.cabecera.symbSize + sdat.cabecera.infoSize + sdat.cabecera.fatSize + sdat.cabecera.fileSize);
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));

            // Common header
            bw.Write(sdat.generico.id);
            bw.Write(sdat.generico.endianess);
            bw.Write(sdat.generico.constant);
            bw.Write(file_size);
            bw.Write(sdat.generico.header_size);
            bw.Write(sdat.generico.nSection);
            // Header
            bw.Write(sdat.cabecera.symbOffset);
            bw.Write(sdat.cabecera.symbSize);
            bw.Write(sdat.cabecera.infoOffset);
            bw.Write(sdat.cabecera.infoSize);
            bw.Write(sdat.cabecera.fatOffset);
            bw.Write(sdat.cabecera.fatSize);
            bw.Write(sdat.cabecera.fileOffset);
            bw.Write(sdat.cabecera.fileSize);
            bw.Write(sdat.cabecera.reserved);

            // Write other sections
            bw.Write(File.ReadAllBytes(symbTemp));
            bw.Write(File.ReadAllBytes(infoTemp));
            bw.Write(File.ReadAllBytes(fatTemp));

            bw.Write(sdat.files.header.id);
            bw.Write(sdat.files.header.size);
            bw.Write(sdat.files.header.nSounds);
            bw.Write(sdat.files.header.reserved);
            bw.Write(File.ReadAllBytes(fileTemp));

            bw.Flush();
            bw.Close();

            File.Delete(symbTemp);
            File.Delete(infoTemp);
            File.Delete(fatTemp);
            File.Delete(fileTemp);

        }
        private void Write_FAT(string fileout, uint startOffset)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));
            uint currOffset = startOffset;

            bw.Write(sdat.fat.header.id);
            bw.Write(sdat.fat.header.size);
            bw.Write(sdat.fat.header.nRecords);

            for (int i = 0; i < sdat.files.header.nSounds; i++)
            {
                Sound currSound = SearchFile(i, sdat.files.root);
                bw.Write(currOffset);
                bw.Write(currSound.size);
                bw.Write((uint)0x00);
                bw.Write((uint)0x00);
                currOffset += currSound.size;
            }

            bw.Flush();
            bw.Close();
        }
        private void Write_Files(string fileout)
        {
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(fileout));
            BinaryReader br = new BinaryReader(File.OpenRead(sdat.archivo));

            for (int i = 0; i < sdat.files.header.nSounds; i++)
            {
                Sound currSound = SearchFile(i, sdat.files.root);
                if (currSound.offset == 0x00)
                {
                    bw.Write(File.ReadAllBytes(currSound.path));
                    bw.Flush();
                }
                else
                {
                    br.BaseStream.Position = currSound.offset;
                    bw.Write(br.ReadBytes((int)currSound.size));
                    bw.Flush();
                }
            }

            bw.Close();

            sdat.files.header.size = (uint)new FileInfo(fileout).Length + 0x10;
            sdat.cabecera.fileSize = sdat.files.header.size;
        }

        private void btnInfo_Click(object sender, EventArgs e)
        {
            Dictionary<String, String> info = new Dictionary<string, string>();

            switch (SearchFile().type)
            {
                case FormatSound.STRM:
                    info = STRM.Information(STRM.Read(SaveFile()), pluginHost.Get_Language());
                    break;
                case FormatSound.SWAV:
                    info = SWAV.Information(SWAV.Read(SaveFile()), pluginHost.Get_Language());
                    break;
                case FormatSound.SWAR:
                    info = SWAR.Information(SWAR.Read(SaveFile()), pluginHost.Get_Language());
                    break;

                case FormatSound.SSEQ:
                    break;
                case FormatSound.SSAR:
                    break;
                case FormatSound.SBNK:
                    break;
                default:
                    break;
            }


            Form ven = new Form();
            ven.Size = new System.Drawing.Size(260, 440);
            ven.FormBorderStyle = FormBorderStyle.FixedDialog;
            ven.ShowIcon = false;
            ven.MaximizeBox = false;
            ven.MinimizeBox = false;
            ven.Icon = Icon.FromHandle(Properties.Resources.information.GetHicon());

            ListView list = new ListView();
            ColumnHeader column1 = new ColumnHeader();
            column1.Text = columnCampo.Text;
            ColumnHeader column2 = new ColumnHeader();
            column2.Text = columnValor.Text;
            list.Columns.Add(column1);
            list.Columns.Add(column2);

            foreach (String value in info.Keys)
            {
                if (value.StartsWith("/b"))
                {
                    String text = value.Replace("/b", "");
                    list.Items.Add(text, text, 0);
                    list.Items[text].Font = new Font("Microsoft Sans Serif", 9, FontStyle.Bold);
                    list.Items[text].SubItems.Add(info[value]);
                }
                else
                {
                    list.Items.Add(value, value, 0);
                    list.Items[value].SubItems.Add(info[value]);
                }
            }
            list.Size = new System.Drawing.Size(250, 430);
            list.View = View.Details;
            list.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            list.Dock = DockStyle.Fill;
            ven.Controls.Add(list);
            ven.Text = btnInfo.Text;
            ven.Show();
        }

        private void btnInfoSect_Click(object sender, EventArgs e)
        {
            InfoForm win = new InfoForm(sdat.info);
            win.FormClosed += new FormClosedEventHandler(win_FormClosed);
            win.Show();
        }
        void win_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (((InfoForm)sender).DialogResult != DialogResult.OK)
                return;

            sdat.info = ((InfoForm)sender).Info;
            sdat.files.root.folders.RemoveAt(sdat.files.root.folders.Count - 1);
            Add_Group();
            treeFiles.Nodes.Clear();
            treeFiles.Nodes.Add(CarpetaToNodo(sdat.files.root));
            treeFiles.Nodes[0].Expand();

        }

        private void Add_Group()
        {
            Folder group = new Folder();
            group = new Folder();
            group.name = "GROUP";
            group.id = 0x0F007;
            group.folders = new List<Folder>();

            // Add files to GROUP folder
            for (int i = 0; i < sdat.info.block[5].nEntries; i++)
            {
                Info.GROUP entry = (Info.GROUP)sdat.info.block[5].entries[i];
                Folder entryFld = new Folder();
                entryFld.name = "Entry " + i.ToString();
                entryFld.id = (uint)(0x0F008 + i);
                entryFld.files = new List<Sound>();
                for (int n = 0; n < entry.nCount; n++)
                {
                    switch (entry.subgroup[n].type)
                    {
                        case 0x700: // SSEQ
                            if (sdat.files.root.folders[0].files.Count > entry.subgroup[n].nEntry)
                                entryFld.files.Add(sdat.files.root.folders[0].files[(int)entry.subgroup[n].nEntry]);
                            break;

                        case 0x803: // SSAR
                            if (sdat.files.root.folders[1].files.Count > entry.subgroup[n].nEntry)
                                entryFld.files.Add(sdat.files.root.folders[1].files[(int)entry.subgroup[n].nEntry]);
                            break;

                        case 0x601: // SBNK
                            if (sdat.files.root.folders[2].files.Count > entry.subgroup[n].nEntry)
                                entryFld.files.Add(sdat.files.root.folders[2].files[(int)entry.subgroup[n].nEntry]);
                            break;

                        case 0x402: // SWAR
                            if (sdat.files.root.folders[3].files.Count > entry.subgroup[n].nEntry)
                                entryFld.files.Add(sdat.files.root.folders[3].files[(int)entry.subgroup[n].nEntry]);
                            break;

                        default:
                            break;
                    }
                }
                group.folders.Add(entryFld);
            }

            sdat.files.root.folders.Add(group);
        }
    }
}
