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
 * By: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Ekona;

namespace Tinke
{
    public partial class Sistema : Form
    {
        RomInfo romInfo;
        Debug debug;

        Acciones accion;
        StringBuilder sb;
        int filesSupported;
        int nFiles;
        bool isMono;
        Keys keyDown;
        bool stop;

        public Sistema()
        {
            InitializeComponent();
            this.Text = "Tinke " + Application.ProductVersion + " - romhacking by pleoNeX";

            // The IE control of the Debug windows doesn't work in Mono
            isMono = (Type.GetType("Mono.Runtime") != null) ? true : false;

            sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            tw.NewLine = "<br>";
            if (!isMono)
                Console.SetOut(tw);

            #region Language
            if (!File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml"))
            {
                File.WriteAllText(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                 "\n<Tinke>\n  <Options>" +
                                 "\n    <Language>English</Language>" +
                                 "\n    <InstantSearch>True</InstantSearch>" +
                                 "\n    <WindowDebug>True</WindowDebug>" +
                                 "\n    <WindowInformation>True</WindowInformation>" +
                                 "\n    <ModeWindow>False</ModeWindow>" +
                                 "\n  </Options>\n</Tinke>",
                                 Encoding.UTF8);
            }

            foreach (string langFile in Directory.GetFiles(Application.StartupPath + Path.DirectorySeparatorChar + "langs"))
            {
                if (!langFile.EndsWith(".xml"))
                    continue; ;

                string flag = Application.StartupPath + Path.DirectorySeparatorChar + "langs" + Path.DirectorySeparatorChar + langFile.Substring(langFile.Length - 9, 5) + ".png";
                Image iFlag;
                if (File.Exists(flag))
                    iFlag = Image.FromFile(flag);
                else
                    iFlag = iconos.Images[1];

                XElement xLang = XElement.Load(langFile);
                if (xLang.Name != "Language")
                    continue;

                toolStripLanguage.DropDownItems.Add(
                    xLang.Attribute("name").Value,
                    iFlag,
                    ToolStripLang_Click);
            }

            ReadLanguage();
            #endregion
            this.Load += new EventHandler(Sistema_Load);
            treeSystem.LostFocus += new EventHandler(treeSystem_LostFocus);
            treeSystem.GotFocus += new EventHandler(treeSystem_LostFocus);
            keyDown = Keys.Escape;
        }
        void Sistema_Load(object sender, EventArgs e)
        {
            string[] filesToRead = new string[1];
            if (Environment.GetCommandLineArgs().Length == 1)
            {
                OpenFileDialog o = new OpenFileDialog();
                o.CheckFileExists = true;
                o.Multiselect = true;

                if (o.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    Application.Exit();
                    return;
                }
                filesToRead = o.FileNames;
                o.Dispose();
            }
            else if (Environment.GetCommandLineArgs().Length == 2)
            {
                if (Environment.GetCommandLineArgs()[1] == "-fld")
                {
                    FolderBrowserDialog o = new FolderBrowserDialog();
                    o.ShowNewFolderButton = false;
                    if (o.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        Application.Exit();
                        return;
                    }
                    filesToRead[0] = o.SelectedPath;
                    o.Dispose();
                }
                else
                    filesToRead[0] = Environment.GetCommandLineArgs()[1];
            }
            else if (Environment.GetCommandLineArgs().Length >= 3)
            {
                filesToRead = new String[Environment.GetCommandLineArgs().Length - 1];
                Array.Copy(Environment.GetCommandLineArgs(), 1, filesToRead, 0, filesToRead.Length);
            }

            Thread espera = new System.Threading.Thread(ThreadEspera);
            if (!isMono)
                espera.Start("S02");

            if (filesToRead.Length == 1 &&
                (Path.GetFileName(filesToRead[0]).ToUpper().EndsWith(".NDS") || Path.GetFileName(filesToRead[0]).ToUpper().EndsWith(".SRL")))
                ReadGame(filesToRead[0]);
            else if (filesToRead.Length == 1 && Directory.Exists(filesToRead[0]))
                ReadFolder(filesToRead[0]);
            else
                ReadFiles(filesToRead);

            if (!isMono)
            {
                espera.Abort();

                debug = new Debug();
                debug.FormClosing += new FormClosingEventHandler(debug_FormClosing);
                debug.Add_Text(sb.ToString());
            }
            sb.Length = 0;

            romInfo.FormClosing += new FormClosingEventHandler(romInfo_FormClosing);
            LoadPreferences();

            this.Show();
            if (!isMono)
                debug.ShowInTaskbar = true;
            romInfo.ShowInTaskbar = true;
            this.Activate();
        }
        private void Sistema_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml").Element("Options");

                xml.Element("WindowDebug").Value = toolStripDebug.Checked.ToString();
                xml.Element("WindowInformation").Value = toolStripInfoRom.Checked.ToString();
                xml.Element("InstantSearch").Value = checkSearch.Checked.ToString();
                xml.Element("ModeWindow").Value = toolStripVentana.Checked.ToString();

                xml = xml.Parent;
                xml.Save(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");
            }
            catch { MessageBox.Show(Tools.Helper.GetTranslation("Sistema", "S37"), Tools.Helper.GetTranslation("Sistema", "S3A")); }

            if (accion is Acciones)
            {
                if (accion.IsNewRom & accion.ROMFile != "")
                {
                    if (MessageBox.Show(Tools.Helper.GetTranslation("Sistema", "S39"), Tools.Helper.GetTranslation("Sistema", "S3A"),
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                accion.Dispose();
            }
        }

        private void ReadGame(string file)
        {
            DateTime startTime = DateTime.Now;

            romInfo = new RomInfo(file);  // Read the header and banner
            DateTime t1 = DateTime.Now;
            accion = new Acciones(file, new String(romInfo.Cabecera.gameCode));
            DateTime t2 = DateTime.Now;

            // Read File Allocation Table (offset and size)
            Nitro.Estructuras.sFAT[] fat = Nitro.FAT.ReadFAT(file, romInfo.Cabecera.FAToffset, romInfo.Cabecera.FATsize);
            DateTime t3 = DateTime.Now;

            // Read the File Name Table and get the directory hierarchy
            sFolder root = Nitro.FNT.ReadFNT(file, romInfo.Cabecera.fileNameTableOffset, fat, accion);
            DateTime t4 = DateTime.Now;

            accion.LastFileID = fat.Length;
            accion.LastFolderID = root.id + 0xF000;
            root.id = 0xF000;

            // Add system files (fnt.bin, banner.bin, overlays, arm9 and arm7)
            if (!(root.folders is List<sFolder>))
                root.folders = new List<sFolder>();
            root.folders.Add(Add_SystemFiles(fat));
            DateTime t5 = DateTime.Now;

            accion.Root = root;
            accion.SortedIDs = Nitro.FAT.SortByOffset(fat);
            DateTime t6 = DateTime.Now;

            Stream stream = File.OpenRead(file);
            treeSystem.BeginUpdate();
            treeSystem.Nodes.Add(Create_Nodes(root, stream)); // Get the node hierarchy
            treeSystem.EndUpdate();
            stream.Close();
            stream.Dispose();

            DateTime t7 = DateTime.Now;
            treeSystem.Nodes[0].Expand();

            Get_SupportedFiles();
            DateTime t8 = DateTime.Now;

            XElement xml = Tools.Helper.GetTranslation("Messages");
            Console.Write("<br><u>" + xml.Element("S0F").Value + "</u><font size=\"2\" face=\"consolas\"><ul>");
            Console.WriteLine("<li>" + xml.Element("S10").Value + (t8 - startTime).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S11").Value + (t1 - startTime).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S12").Value + (t2 - t1).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S15").Value + (t3 - t2).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S13").Value + (t4 - t3).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S14").Value + (t5 - t4).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S16").Value + (t6 - t5).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S18").Value + (t7 - t6).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S19").Value + (t8 - t7).ToString() + "</li>");
            Console.WriteLine("</ul>");
            Console.WriteLine("Number of directories: {0}", accion.LastFolderID - 0xF000 - 1);
            Console.WriteLine("Number of files: {0}</font>", fat.Length);

            this.Text += "          " + new String(romInfo.Cabecera.gameTitle).Replace("\0", "") +
                " (" + new String(romInfo.Cabecera.gameCode) + ')';
        }
        private sFolder Add_SystemFiles(Nitro.Estructuras.sFAT[] fatTable)
        {
            sFolder ftc = new sFolder();
            ftc.name = "ftc";
            ftc.id = (ushort)accion.LastFolderID;
            accion.LastFolderID++;
            ftc.files = new List<sFile>();
            ftc.files.AddRange(Nitro.Overlay.ReadBasicOverlays(
                accion.ROMFile, romInfo.Cabecera.ARM9overlayOffset, romInfo.Cabecera.ARM9overlaySize, true, fatTable));
            ftc.files.AddRange(Nitro.Overlay.ReadBasicOverlays(
                accion.ROMFile, romInfo.Cabecera.ARM7overlayOffset, romInfo.Cabecera.ARM7overlaySize, false, fatTable));

            sFile rom = new sFile();
            rom.name = "rom.nds";
            rom.offset = 0x00;
            rom.size = (uint)new FileInfo(accion.ROMFile).Length;
            rom.path = accion.ROMFile;
            rom.id = (ushort)accion.LastFileID;
            accion.LastFileID++;
            ftc.files.Add(rom);

            sFile fnt = new sFile();
            fnt.name = "fnt.bin";
            fnt.offset = romInfo.Cabecera.fileNameTableOffset;
            fnt.size = romInfo.Cabecera.fileNameTableSize;
            fnt.path = accion.ROMFile;
            fnt.id = (ushort)accion.LastFileID;
            accion.LastFileID++;
            ftc.files.Add(fnt);

            sFile fat = new sFile();
            fat.name = "fat.bin";
            fat.offset = romInfo.Cabecera.FAToffset;
            fat.size = romInfo.Cabecera.FATsize;
            fat.path = accion.ROMFile;
            fat.id = (ushort)accion.LastFileID;
            accion.LastFileID++;
            ftc.files.Add(fat);

            sFile banner = new sFile();
            banner.name = "banner.bin";
            banner.offset = romInfo.Cabecera.bannerOffset;
            banner.size = 0x840;
            banner.path = accion.ROMFile;
            banner.id = (ushort)accion.LastFileID;
            accion.LastFileID++;
            ftc.files.Add(banner);

            sFile arm9 = new sFile();
            arm9.name = "arm9.bin";
            arm9.offset = romInfo.Cabecera.ARM9romOffset;
            arm9.size = romInfo.Cabecera.ARM9size;
            arm9.path = accion.ROMFile;
            arm9.id = (ushort)accion.LastFileID;
            accion.LastFileID++;
            ftc.files.Add(arm9);

            sFile arm7 = new sFile();
            arm7.name = "arm7.bin";
            arm7.offset = romInfo.Cabecera.ARM7romOffset;
            arm7.size = romInfo.Cabecera.ARM7size;
            arm7.path = accion.ROMFile;
            arm7.id = (ushort)accion.LastFileID;
            accion.LastFileID++;
            ftc.files.Add(arm7);

            if (romInfo.Cabecera.ARM9overlaySize != 0)
            {
                sFile y9 = new sFile();
                y9.name = "y9.bin";
                y9.offset = romInfo.Cabecera.ARM9overlayOffset;
                y9.size = romInfo.Cabecera.ARM9overlaySize;
                y9.path = accion.ROMFile;
                y9.id = (ushort)accion.LastFileID;
                accion.LastFileID++;
                ftc.files.Add(y9);
            }

            if (romInfo.Cabecera.ARM7overlaySize != 0)
            {
                sFile y7 = new sFile();
                y7.name = "y7.bin";
                y7.offset = romInfo.Cabecera.ARM7overlayOffset;
                y7.size = romInfo.Cabecera.ARM7overlaySize;
                y7.path = accion.ROMFile;
                y7.id = (ushort)accion.LastFileID;
                accion.LastFileID++;
                ftc.files.Add(y7);
            }

            Set_Format(ftc);
            return ftc;
        }
        private void ReadFiles(string[] files)
        {
            toolStripInfoRom.Enabled = false;
            btnSaveROM.Enabled = false;

            romInfo = new RomInfo(); // Para que no se formen errores...
            DateTime startTime = DateTime.Now;

            accion = new Acciones("", "NO GAME");
            DateTime t1 = DateTime.Now;

            accion.LastFileID = files.Length;
            accion.LastFolderID = 0xF000;

            // Obtenemos el sistema de archivos
            sFolder root = new sFolder();
            root.name = "root";
            root.id = 0xF000;
            root.files = new List<sFile>();
            for (int i = 0; i < files.Length; i++)
            {
                sFile currFile = new sFile();
                currFile.id = (ushort)i;
                currFile.name = Path.GetFileName(files[i]);
                currFile.offset = 0x00;
                currFile.path = files[i];

                currFile.size = (uint)new FileInfo(files[i]).Length;
                root.files.Add(currFile);
            }
            DateTime t2 = DateTime.Now;

            accion.Root = root;
            DateTime t3 = DateTime.Now;

            Set_Format(root);
            DateTime t4 = DateTime.Now;
            treeSystem.BeginUpdate();
            treeSystem.Nodes.Add(Create_Nodes(root)); // Show files
            treeSystem.EndUpdate();
            DateTime t5 = DateTime.Now;
            treeSystem.Nodes[0].Expand();

            Get_SupportedFiles();
            DateTime t6 = DateTime.Now;

            XElement xml = Tools.Helper.GetTranslation("Messages");
            Console.Write("<br><u>" + xml.Element("S0F").Value + "</u><ul><font size=\"2\" face=\"consolas\">");
            Console.WriteLine("<li>" + xml.Element("S10").Value + (t6 - startTime).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S12").Value + (t1 - startTime).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S21").Value + (t2 - t1).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S16").Value + (t3 - t2).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S17").Value + (t4 - t3).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S18").Value + (t5 - t4).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S19").Value + (t6 - t5).ToString() + "</li>");
            Console.Write("</font></ul><br>");
        }
        private void ReadFolder(string folder)
        {
            toolStripInfoRom.Enabled = false;
            btnSaveROM.Enabled = false;

            romInfo = new RomInfo(); // Para que no se formen errores...
            DateTime startTime = DateTime.Now;

            accion = new Acciones("", "NO GAME");
            DateTime t1 = DateTime.Now;

            accion.LastFileID = 0;
            accion.LastFolderID = 0xF000;

            // Obtenemos el sistema de archivos
            sFolder root = new sFolder();
            root.name = "root";
            root.id = 0xF000;
            accion.LastFileID = 0x00;
            accion.LastFolderID = 0xF000;
            root = accion.Recursive_GetExternalDirectories(folder, root);
            DateTime t2 = DateTime.Now;

            accion.Root = root;
            DateTime t3 = DateTime.Now;

            Set_Format(root);
            DateTime t4 = DateTime.Now;
            treeSystem.BeginUpdate();
            treeSystem.Nodes.Add(Create_Nodes(root)); // Show files
            treeSystem.EndUpdate();
            DateTime t5 = DateTime.Now;
            treeSystem.Nodes[0].Expand();

            Get_SupportedFiles();
            DateTime t6 = DateTime.Now;

            XElement xml = Tools.Helper.GetTranslation("Messages");
            Console.Write("<br><u>" + xml.Element("S0F").Value + "</u><ul><font size=\"2\" face=\"consolas\">");
            Console.WriteLine("<li>" + xml.Element("S10").Value + (t6 - startTime).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S12").Value + (t1 - startTime).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S21").Value + (t2 - t1).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S16").Value + (t3 - t2).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S17").Value + (t4 - t3).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S18").Value + (t5 - t4).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S19").Value + (t6 - t5).ToString() + "</li>");
            Console.Write("</font></ul><br>");
        }

        private void LoadPreferences()
        {
            try
            {
                XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml").Element("Options");

                toolStripDebug.Enabled = !isMono;
                if (!isMono && xml.Element("WindowDebug").Value == "True")
                {
                    toolStripDebug.Checked = true;
                    debug.Show();
                    debug.Activate();
                }
                if (xml.Element("WindowInformation").Value == "True" && accion.ROMFile != "") // En caso de que se haya abierto una ROM, no archivos sueltos
                {
                    toolStripInfoRom.Checked = true;
                    romInfo.Show();
                    romInfo.Activate();
                }
                if (xml.Element("InstantSearch").Value == "True")
                    checkSearch.Checked = true;
                if (xml.Element("ModeWindow").Value == "True")
                    toolStripVentana.Checked = true;
            }
            catch { MessageBox.Show(Tools.Helper.GetTranslation("Sistema", "S38"), Tools.Helper.GetTranslation("Sistema", "S3A")); }
        }
        private void ReadLanguage()
        {
            try
            {
                XElement xml = Tools.Helper.GetTranslation("Sistema");

                toolStripOpen.Text = xml.Element("S01").Value;
                toolStripInfoRom.Text = xml.Element("S02").Value;
                toolStripDebug.Text = xml.Element("S03").Value;
                toolStripVentana.Text = xml.Element("S04").Value;
                //toolStripPlugin.Text = xml.Element("S05").Value;
                //recargarPluginsToolStripMenuItem.Text = xml.Element("S06").Value;
                toolStripLanguage.Text = xml.Element("S1E").Value;
                columnHeader1.Text = xml.Element("S07").Value;
                columnHeader2.Text = xml.Element("S08").Value;
                listFile.Items[0].Text = xml.Element("S09").Value;
                listFile.Items[1].Text = xml.Element("S0A").Value;
                listFile.Items[2].Text = xml.Element("S0B").Value;
                listFile.Items[3].Text = xml.Element("S0C").Value;
                listFile.Items[4].Text = xml.Element("S0D").Value;
                listFile.Items[5].Text = xml.Element("S0E").Value;
                listFile.Items[6].Text = xml.Element("S40").Value;
                linkAboutBox.Text = xml.Element("S0F").Value;
                //toolStripDeleteChain.Text = xml.Element("S10").Value;
                //borrarPaletaToolStripMenuItem.Text = xml.Element("S11").Value;
                //borrarTileToolStripMenuItem.Text = xml.Element("S12").Value;
                //borrarScreenToolStripMenuItem.Text = xml.Element("S13").Value;
                //borrarCeldasToolStripMenuItem.Text = xml.Element("S14").Value;
                //borrarAnimaciónToolStripMenuItem.Text = xml.Element("S15").Value;
                //s10ToolStripMenuItem.Text = xml.Element("S10").Value;
                toolStripOpenAs.Text = xml.Element("S16").Value;
                toolStripMenuItem1.Text = xml.Element("S17").Value;
                toolStripMenuItem2.Text = xml.Element("S18").Value;
                toolStripMenuItem3.Text = xml.Element("S19").Value;
                btnUnpack.Text = xml.Element("S1A").Value;
                btnExtract.Text = xml.Element("S1B").Value;
                btnSee.Text = xml.Element("S1C").Value;
                btnHex.Text = xml.Element("S1D").Value;
                label1.Text = xml.Element("S2D").Value;
                checkSearch.Text = xml.Element("S2E").Value;
                toolTipSearch.ToolTipTitle = xml.Element("S2F").Value;

                toolTipSearch.SetToolTip(txtSearch,
                    "<Ani> -> " + xml.Element("S24").Value +
                    "\n<Cell> -> " + xml.Element("S23").Value +
                    "\n<Map> -> " + xml.Element("S22").Value +
                    "\n<Image> -> " + xml.Element("S21").Value +
                    "\n<FullImage> -> " + xml.Element("S25").Value +
                    "\n<Palette> -> " + xml.Element("S20").Value +
                    "\n<Text> -> " + xml.Element("S26").Value +
                    "\n<Video> -> " + xml.Element("S27").Value +
                    "\n<Sound> -> " + xml.Element("S28").Value +
                    "\n<Font> -> " + xml.Element("S29").Value +
                    "\n<Compress> -> " + xml.Element("S2A").Value +
                    "\n<Script> -> " + xml.Element("S34").Value +
                    "\n<Pack> -> " + xml.Element("S3D").Value +
                    "\n<Texture> -> " + xml.Element("S3E").Value +
                    "\n<3DModel> -> " + xml.Element("S3F").Value +
                    "\n<Unknown> -> " + xml.Element("S2B").Value
                    );
                btnImport.Text = xml.Element("S32").Value;
                btnSaveROM.Text = xml.Element("S33").Value;
                toolStripMenuComprimido.Text = xml.Element("S2A").Value;
                toolStripAbrirTexto.Text = xml.Element("S26").Value;
                toolStripAbrirFat.Text = xml.Element("S3D").Value;
                btnPack.Text = xml.Element("S42").Value;
                stripRefreshMsg.Text = xml.Element("S45").Value;
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }


        private TreeNode Create_Nodes(sFolder currFolder)
        {
            TreeNode currNode = new TreeNode();

            if (currFolder.id < 0xF000) // Archivo descomprimido
            {
                int imageIndex = accion.ImageFormatFile(accion.Get_Format(currFolder.id));
                currNode = new TreeNode(currFolder.name, imageIndex, imageIndex);
            }
            else
                currNode = new TreeNode(currFolder.name, 0, 0);
            currNode.Tag = currFolder.id;
            currNode.Name = currFolder.name;


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    currNode.Nodes.Add(Create_Nodes(subFolder));


            if (currFolder.files is List<sFile>)
            {
                foreach (sFile archivo in currFolder.files)
                {
                    int nImage = accion.ImageFormatFile(archivo.format);
                    string ext = "";

                    if (archivo.format == Format.Unknown)
                    {
                        ext = accion.Get_MagicIDS(archivo);
                        if (ext != "")
                            ext = " [" + ext + ']';
                    }
                    TreeNode fileNode = new TreeNode(archivo.name + ext, nImage, nImage);
                    fileNode.Name = archivo.name;
                    fileNode.Tag = archivo.id;
                    currNode.Nodes.Add(fileNode);
                }
            }

            return currNode;
        }
        private TreeNode Create_Nodes(sFolder currFolder, Stream stream)
        {
            TreeNode currNode = new TreeNode();

            if (currFolder.id < 0xF000) // Archivo descomprimido
            {
                int imageIndex = accion.ImageFormatFile(accion.Get_Format(currFolder.id));
                currNode = new TreeNode(currFolder.name, imageIndex, imageIndex);
            }
            else
                currNode = new TreeNode(currFolder.name, 0, 0);
            currNode.Tag = currFolder.id;
            currNode.Name = currFolder.name;


            if (currFolder.folders is List<sFolder>)
                foreach (sFolder subFolder in currFolder.folders)
                    currNode.Nodes.Add(Create_Nodes(subFolder, stream));


            if (currFolder.files is List<sFile>)
            {
                foreach (sFile archivo in currFolder.files)
                {
                    int nImage = accion.ImageFormatFile(archivo.format);
                    string ext = "";

                    if (archivo.format == Format.Unknown)
                    {
                        stream.Position = archivo.offset;
                        ext = accion.Get_MagicIDS(stream, archivo.size);
                        if (ext != "")
                            ext = " [" + ext + ']';
                    }
                    TreeNode fileNode = new TreeNode(archivo.name + ext, nImage, nImage);
                    fileNode.Name = archivo.name;
                    fileNode.Tag = archivo.id;
                    currNode.Nodes.Add(fileNode);
                }
            }

            return currNode;
        }
        private void FolderToNode(sFolder folder, ref TreeNode node)
        {
            if (folder.id < 0xF000)
            {
                node.ImageIndex = accion.ImageFormatFile(accion.Get_Format(folder.id));
                node.SelectedImageIndex = node.ImageIndex;
            }
            else
            {
                node.ImageIndex = 0;
                node.SelectedImageIndex = 0;
            }
            node.Tag = folder.id;
            node.Name = folder.name;

            if (folder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in folder.folders)
                {
                    TreeNode newNodo = new TreeNode(subFolder.name);
                    FolderToNode(subFolder, ref newNodo);
                    node.Nodes.Add(newNodo);
                }
            }


            if (folder.files is List<sFile>)
            {
                foreach (sFile archivo in folder.files)
                {
                    int nImage = accion.ImageFormatFile(archivo.format);
                    string ext = "";
                    if (archivo.format == Format.Unknown)
                    {
                        ext = accion.Get_MagicIDS(archivo);
                        if (ext != "")
                            ext = " [" + ext + ']'; // Previene extensiones vacías
                    }
                    TreeNode fileNode = new TreeNode(archivo.name + ext, nImage, nImage);
                    fileNode.Name = archivo.name;
                    fileNode.Tag = archivo.id;
                    node.Nodes.Add(fileNode);
                }
            }


        }
        private TreeNode[] FilesToNodes(sFile[] files)
        {
            TreeNode[] nodos = new TreeNode[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                int nImage = accion.ImageFormatFile(files[i].format);
                string ext = "";
                if (files[i].format == Format.Unknown)
                {
                    ext = new String(Encoding.ASCII.GetChars(accion.Get_MagicID(files[i].path)));
                    if (ext != "")
                        ext = " [" + ext + ']';
                }
                nodos[i] = new TreeNode(files[i].name + ext, nImage, nImage);
                nodos[i].Name = files[i].name;
                nodos[i].Tag = files[i].id;
            }

            return nodos;
        }

        private void Set_Format(sFolder folder)
        {
            if (folder.files is List<sFile>)
            {
                for (int i = 0; i < folder.files.Count; i++)
                {
                    sFile newFile = folder.files[i];
                    newFile.format = accion.Get_Format(newFile);
                    folder.files[i] = newFile;
                }
            }


            if (folder.folders is List<sFolder>)
                foreach (sFolder subFolder in folder.folders)
                    Set_Format(subFolder);
        }
        private void Get_SupportedFiles()
        {
            filesSupported = nFiles = 0; // Reiniciamos el contador

            Recursive_SupportedFiles(accion.Root);
            if (nFiles == 0)
                nFiles = 1;

            lblSupport.Text = Tools.Helper.GetTranslation("Sistema", "S30") + ' ' + (filesSupported * 100 / nFiles) + '%';
            if ((filesSupported * 100 / nFiles) >= 75)
                lblSupport.Font = new Font("Consolas", 10, FontStyle.Bold | FontStyle.Underline);
            else
                lblSupport.Font = new Font("Consolas", 10, FontStyle.Regular);
        }
        private void Recursive_SupportedFiles(sFolder folder)
        {
            if (folder.files is List<sFile>)
            {
                foreach (sFile archivo in folder.files)
                {
                    if (archivo.format == Format.System || archivo.size == 0x00)
                        continue;

                    if (archivo.format != Format.Unknown)
                        filesSupported++;
                    nFiles++;
                }
            }

            if (folder.folders is List<sFolder>)
                foreach (sFolder subFolder in folder.folders)
                    Recursive_SupportedFiles(subFolder);
        }

        private void ThreadEspera(Object name)
        {
            Espera espera = new Espera((string)name, false);

            try
            {
                espera.ShowDialog();
            }
            catch
            {
            }
        }

        private void treeSystem_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (stop)
                return;

            btnPack.Enabled = false;
            accion.IDSelect = Convert.ToInt32(e.Node.Tag);
            // Clean old information
            for (int i = 0; i < listFile.Items.Count; i++)
                if (listFile.Items[i].SubItems.Count == 2)
                    listFile.Items[i].SubItems.RemoveAt(1);

            if (e.Node.Name == "root")
            {
                listFile.Items[0].SubItems.Add("root");
                listFile.Items[1].SubItems.Add("0xF000");
                listFile.Items[2].SubItems.Add("");
                listFile.Items[3].SubItems.Add("");
                listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S1F"));
                listFile.Items[5].SubItems.Add("");

                btnHex.Enabled = false;
                btnSee.Enabled = false;
                toolStripOpenAs.Enabled = false;
                btnUnpack.Enabled = true;
            }
            else if (Convert.ToUInt16(e.Node.Tag) < 0xF000)
            {
                sFile selectFile = accion.Selected_File();

                listFile.Items[0].SubItems.Add(selectFile.name);
                listFile.Items[1].SubItems.Add("0x" + String.Format("{0:X}", selectFile.id));
                listFile.Items[2].SubItems.Add("0x" + String.Format("{0:X}", selectFile.offset));
                listFile.Items[3].SubItems.Add(selectFile.size.ToString());
                #region Get type of file
                switch (selectFile.format)
                {
                    case Format.Palette:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S20"));
                        break;
                    case Format.Tile:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S21"));
                        break;
                    case Format.Map:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S22"));
                        break;
                    case Format.Cell:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S23"));
                        break;
                    case Format.Animation:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S24"));
                        break;
                    case Format.FullImage:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S25"));
                        break;
                    case Format.Text:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S26"));
                        break;
                    case Format.Video:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S27"));
                        break;
                    case Format.Sound:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S28"));
                        break;
                    case Format.Font:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S29"));
                        break;
                    case Format.Compressed:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S2A"));
                        break;
                    case Format.Unknown:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S2B"));
                        break;
                    case Format.System:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S31"));
                        break;
                    case Format.Script:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S34"));
                        break;
                    case Format.Pack:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S3D"));
                        break;
                    case Format.Texture:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S3E"));
                        break;
                    case Format.Model3D:
                        listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S3F"));
                        break;
                }
                #endregion
                listFile.Items[5].SubItems.Add(selectFile.path);
                listFile.Items[6].SubItems.Add(accion.Get_RelativePath(selectFile.id, "", accion.Root));
                toolStripOpenAs.Enabled = true;

                btnHex.Enabled = true;

                if (selectFile.format != Format.Unknown)
                    btnSee.Enabled = true;
                else
                    btnSee.Enabled = false;
                if (selectFile.format == Format.Compressed || selectFile.format == Format.Pack)
                    btnUnpack.Enabled = true;
                else
                    btnUnpack.Enabled = false;
                if ((String)selectFile.tag == "Descomprimido")
                {
                    toolStripOpenAs.Enabled = false;
                    btnUnpack.Enabled = true;
                    btnPack.Enabled = true;
                }

                if (keyDown != Keys.Escape)
                {
                    KeyEventArgs eventKey = new KeyEventArgs(keyDown);
                    keyDown = Keys.Escape;
                    this.OnKeyDown(eventKey);
                }
            }
            else
            {
                sFolder selectFolder = accion.Selected_Folder();

                listFile.Items[0].SubItems.Add(e.Node.Name);
                listFile.Items[1].SubItems.Add("0x" + String.Format("{0:X}", e.Node.Tag));
                listFile.Items[2].SubItems.Add("");
                listFile.Items[3].SubItems.Add("");
                listFile.Items[4].SubItems.Add(Tools.Helper.GetTranslation("Sistema", "S1F"));
                listFile.Items[5].SubItems.Add("");
                listFile.Items[6].SubItems.Add(accion.Get_RelativePath(selectFolder.id, "", accion.Root));

                btnHex.Enabled = false;
                btnSee.Enabled = false;
                toolStripOpenAs.Enabled = false;
                btnUnpack.Enabled = true;
            }

            listFile.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        #region Key events
        private void Sistema_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == keyDown)
                return;

            stop = true;

            if (e.KeyCode == Keys.Space && treeSystem.Focused)
            {
                e.SuppressKeyPress = true;
                btnSee.PerformClick();
            }
            else if (e.KeyCode == Keys.P && toolStripOpenAs.Enabled && treeSystem.Focused)
            {
                e.SuppressKeyPress = true;
                keyDown = e.KeyCode;
                toolStripMenuItem1.PerformClick();
            }
            else if (e.KeyCode == Keys.T && toolStripOpenAs.Enabled && treeSystem.Focused)
            {
                e.SuppressKeyPress = true;
                keyDown = e.KeyCode;
                toolStripMenuItem2.PerformClick();
            }
            else if (e.KeyCode == Keys.M && toolStripOpenAs.Enabled && treeSystem.Focused)
            {
                e.SuppressKeyPress = true;
                keyDown = e.KeyCode;
                toolStripMenuItem3.PerformClick();
            }
            else if (e.KeyCode == Keys.D && treeSystem.Focused)
            {
                e.SuppressKeyPress = true;
                keyDown = e.KeyCode;

                if (btnUnpack.Enabled)
                    btnUnpack.PerformClick();
            }
            else if (e.KeyCode == Keys.X && treeSystem.Focused)
            {
                e.SuppressKeyPress = true;
                treeSystem.SelectedNode.ExpandAll();
            }
            else if (e.KeyCode == Keys.C && treeSystem.Focused)
            {
                e.SuppressKeyPress = true;
                treeSystem.SelectedNode.Collapse(false);
            }
            else if (e.KeyCode == Keys.H && treeSystem.Focused)
            {
                e.SuppressKeyPress = true;
                btnHex.PerformClick();
            }
            else if (e.KeyCode == Keys.R && treeSystem.Focused)
            {
                this.Cursor = Cursors.WaitCursor;
                e.SuppressKeyPress = true;

                sFolder currFolder = accion.Selected_Folder();
                if (!(currFolder.name is string))
                {
                    this.Cursor = Cursors.Default;
                    stop = false;
                    return;
                }
                
                // Change the temp folder, used to export (if so) the files.
                FolderBrowserDialog o = new FolderBrowserDialog();
                o.Description = "Select new \"temp folder\".";
                if (o.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    this.Cursor = Cursors.Default;
                    stop = false;
                    return;
                }
                accion.Set_TempFolder(o.SelectedPath);
                int a = accion.IDSelect;
                Recursive_ReadFile(currFolder);

                accion.Restore_TempFolder();
                this.Cursor = Cursors.Default;
            }

            stop = false;
        }
        private void Sistema_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P || e.KeyCode == Keys.T || e.KeyCode == Keys.M || e.KeyCode == Keys.D)
                keyDown = Keys.Escape;
        }
        void treeSystem_LostFocus(object sender, EventArgs e)
        {
            keyDown = Keys.Escape;
        }

        private void Recursive_ReadFile(sFolder currFolder)
        {
            if (currFolder.files is List<sFile>)
                for (int f = 0; f < currFolder.files.Count; f++)
                    accion.Read_File(currFolder.files[f]);

            if (currFolder.folders is List<sFolder>)
            {
                for (int f = 0; f < currFolder.folders.Count; f++)
                {
                    string tempFolder = accion.Get_TempFolder() + Path.DirectorySeparatorChar + currFolder.folders[f].name;
                    if (!Directory.Exists(tempFolder))
                        Directory.CreateDirectory(tempFolder);
                    accion.Set_TempFolder(tempFolder);

                    Recursive_ReadFile(currFolder.folders[f]);

                    accion.Set_TempFolder(Directory.GetParent(tempFolder).FullName);
                }
            }
        }
        #endregion

        #region Buttons
        private void btnHex_Click(object sender, EventArgs e)
        {
            sFile file = accion.Selected_File();
            string filePath = accion.Save_File(file);
            Form hex;

            if (!isMono) {
                hex = new VisorHex(filePath, file.id, file.name != "rom.nds");
                hex.FormClosed += hex_FormClosed;
            } else {
                hex = new VisorHexBasic(filePath, 0, file.size);
            }

            hex.Text += " - " + file.name;
            hex.Show();
        }
        void hex_FormClosed(object sender, FormClosedEventArgs e)
        {
            VisorHex hex = sender as VisorHex;
            if (sender != null && hex.Edited)
                accion.Change_File(hex.FileID, hex.NewFile);
        }

        private void BtnSee(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (toolStripVentana.Checked)
            {
                Visor visor = new Visor();
                visor.Controls.Add(accion.See_File());
                visor.Text += " - " + accion.Selected_File().name;
                visor.Show();
            }
            else
            {
                for (int i = 0; i < panelObj.Controls.Count; i++)
                    panelObj.Controls[i].Dispose();
                panelObj.Controls.Clear();

                Control control = accion.See_File();
                if (control.Size.Height != 0 && control.Size.Width != 0)
                {
                    panelObj.Controls.Add(control);
                    if (btnDesplazar.Text == ">>>>>")
                        btnDesplazar.PerformClick();
                }
                else
                    if (btnDesplazar.Text == "<<<<<")
                        btnDesplazar.PerformClick();
            }
            this.Cursor = Cursors.Default;

            if (!isMono)
                debug.Add_Text(sb.ToString());
            sb.Length = 0;
        }
        private void treeSystem_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (accion.IDSelect < 0xF000)   // Comprobación de que la selección no sea un directorio
            {
                accion.Read_File();

                if (!isMono)
                    debug.Add_Text(sb.ToString());
                sb.Length = 0;
            }
        }

        private void btnUnpack_Click(object sender, EventArgs e)
        {
            sFolder uncompress;

            if (accion.IDSelect >= 0x0F000)
            {
                UnpackFolder();
                return;
            }
            if ((String)accion.Selected_File().tag == "Descomprimido")
            {
                UnpackFolder();
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            uncompress = accion.Unpack();

            if (!(uncompress.files is List<sFile>) && !(uncompress.folders is List<sFolder>)) // En caso de que falle la extracción
            {
                if (!isMono)
                    debug.Add_Text(sb.ToString());
                sb.Length = 0;

                this.Cursor = Cursors.Default;
                keyDown = Keys.Escape;

                MessageBox.Show(Tools.Helper.GetTranslation("Sistema", "S36"));
                return;
            }

            toolStripOpenAs.Enabled = false;
            btnPack.Enabled = true;

            Get_SupportedFiles();
            Add_TreeNodes(uncompress);

            if (!isMono)
                debug.Add_Text(sb.ToString());
            sb.Length = 0;
            this.Cursor = Cursors.Default;
        }
        private void Add_TreeNodes(sFolder unpacked)
        {
            // Add new files to the main tree
            treeSystem.BeginUpdate();

            TreeNode selected = treeSystem.SelectedNode;
            selected.Nodes.Clear();
            FolderToNode(unpacked, ref selected);
            selected.ImageIndex = accion.ImageFormatFile(accion.Selected_File().format);
            selected.SelectedImageIndex = selected.ImageIndex;

            TreeNode[] nodos = new TreeNode[selected.Nodes.Count]; selected.Nodes.CopyTo(nodos, 0);
            treeSystem.SelectedNode.Tag = selected.Tag;
            accion.IDSelect = Convert.ToInt32(selected.Tag);
            selected.Nodes.Clear();

            treeSystem.SelectedNode.Nodes.AddRange((TreeNode[])nodos);
            treeSystem.SelectedNode.Expand();

            treeSystem.EndUpdate();
            treeSystem.Focus();
        }
        private void UnpackFolder()
        {
            this.Cursor = Cursors.WaitCursor;
            Thread espera = new System.Threading.Thread(ThreadEspera);
            if (!isMono)
                espera.Start("S04");

            sFolder folderSelected = accion.Selected_Folder();
            if (!(folderSelected.name is String)) // If it's the search folder or similar
                folderSelected = Get_SearchedFiles();

            Recursivo_UnpackFolder(folderSelected);
            Get_SupportedFiles();

            treeSystem.BeginUpdate();

            treeSystem.Nodes.Clear();
            treeSystem.Nodes.Add(Create_Nodes(accion.Root));
            treeSystem.Nodes[0].Expand();

            treeSystem.EndUpdate();

            if (!isMono)
            {
                espera.Abort();
                debug.Add_Text(sb.ToString());
            }
            sb.Length = 0;
            this.Cursor = Cursors.Default;
        }
        private void Recursivo_UnpackFolder(sFolder currFolder)
        {
            if (currFolder.folders is List<sFolder>)
            {
                sFolder[] carpetas = new sFolder[currFolder.folders.Count];
                currFolder.folders.CopyTo(carpetas);
                foreach (sFolder subFolder in carpetas)
                    Recursivo_UnpackFolder(subFolder);
            }

            if (currFolder.files is List<sFile>)
            {
                sFile[] archivos = new sFile[currFolder.files.Count];
                currFolder.files.CopyTo(archivos);
                foreach (sFile archivo in archivos)
                    if (archivo.format == Format.Compressed || archivo.format == Format.Pack)
                        accion.Unpack(archivo.id);
            }
        }
        private void btnPack_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            accion.Pack();

            if (!isMono)
                debug.Add_Text(sb.ToString());
            sb.Length = 0;
            this.Cursor = Cursors.Default;
        }


        private void btnExtraer_Click(object sender, EventArgs e)
        {
            if (Convert.ToUInt16(accion.IDSelect) < 0xF000)
            {
                if ((String)accion.Selected_File().tag == "Descomprimido")
                {
                    if (MessageBox.Show(Tools.Helper.GetTranslation("Sistema", "S3B"), "", MessageBoxButtons.YesNo,
                          MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                        ExtractFolder();
                    else
                        ExtractFile();
                }
                else
                    ExtractFile();
            }
            else
                ExtractFolder();
        }
        private void ExtractFile()
        {
            this.Cursor = Cursors.WaitCursor;
            sFile fileSelect = accion.Selected_File();

            SaveFileDialog o = new SaveFileDialog();
            o.FileName = fileSelect.name;
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BinaryReader br = new BinaryReader(File.OpenRead(fileSelect.path));
                br.BaseStream.Position = fileSelect.offset;
                File.WriteAllBytes(o.FileName, br.ReadBytes((int)fileSelect.size));
                br.Close();
            }
            this.Cursor = Cursors.Default;
        }
        private void ExtractFolder()
        {
            sFolder folderSelect = accion.Selected_Folder();

            if (!(folderSelect.name is String)) // If it's the search folder or similar
            {
                folderSelect = Get_SearchedFiles();
            }

            FolderBrowserDialog o = new FolderBrowserDialog();
            o.ShowNewFolderButton = true;
            o.Description = Tools.Helper.GetTranslation("Sistema", "S2C");
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Directory.CreateDirectory(o.SelectedPath + Path.DirectorySeparatorChar + folderSelect.name);

                Thread espera = new System.Threading.Thread(ThreadEspera);
                if (!isMono)
                    espera.Start("S03");
                RecursivoExtractFolder(folderSelect, o.SelectedPath + Path.DirectorySeparatorChar + folderSelect.name);
                if (!isMono)
                    espera.Abort();

            }

        }
        private void RecursivoExtractFolder(sFolder currFolder, String path)
        {
            if (currFolder.files is List<sFile>)
                foreach (sFile archivo in currFolder.files)
                {
                    string filePath = path + Path.DirectorySeparatorChar + archivo.name;
                    for (int i = 0; File.Exists(filePath); i++)
                    {
                        filePath = path + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(archivo.name) + " (" +
                            i.ToString() + ')' + Path.GetExtension(archivo.name);
                    }

                    BinaryReader br = new BinaryReader(File.OpenRead(archivo.path));
                    br.BaseStream.Position = archivo.offset;
                    File.WriteAllBytes(filePath, br.ReadBytes((int)archivo.size));
                    br.Close();
                }



            if (currFolder.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in currFolder.folders)
                {
                    Directory.CreateDirectory(path + Path.DirectorySeparatorChar + subFolder.name);
                    RecursivoExtractFolder(subFolder, path + Path.DirectorySeparatorChar + subFolder.name);
                }
            }
        }

        private void btnSaveROM_Click(object sender, EventArgs e)
        {
            /* ROM sections:
             * 
             * Header (0x0000-0x4000)
             * ARM9 Binary
             *   |_ARM9
             *   |_ARM9 Overlays Tables
             *   |_ARM9 Overlays
             * ARM7 Binary
             *   |_ARM7
             *   |_ARM7 Overlays Tables
             *   |_ARM7 Overlays
             * FNT (File Name Table)
             *   |_Main tables
             *   |_Subtables (names)
             * FAT (File Allocation Table)
             *   |_Files offset
             *     |_Start offset
             *     |_End offset
             * Banner
             *   |_Header 0x20
             *   |_Icon (Bitmap + palette) 0x200 + 0x20
             *   |_Game titles (Japanese, English, French, German, Italian, Spanish) 6 * 0x100
             * Files...
            */

            Thread espera = new Thread(ThreadEspera);
            if (!isMono)
                espera.Start("S05");

            // Get special files
            sFolder ftc = accion.Search_Folder("ftc");

            sFile fnt = ftc.files.Find(sFile => sFile.name == "fnt.bin");
            sFile fat = ftc.files.Find(sFile => sFile.name == "fat.bin");
            sFile arm9 = ftc.files.Find(sFile => sFile.name == "arm9.bin");
            sFile arm7 = ftc.files.Find(sFile => sFile.name == "arm7.bin");

            int index = ftc.files.FindIndex(sFile => sFile.name == "y9.bin");
            sFile y9 = new sFile();
            List<sFile> ov9 = new List<sFile>();
            if (index != -1)
            {
                y9 = ftc.files[index];
                ov9 = ftc.files.FindAll(sFile => sFile.name.StartsWith("overlay9_"));
            }

            index = ftc.files.FindIndex(sFile => sFile.name == "y7.bin");
            List<sFile> ov7 = new List<sFile>();
            sFile y7 = new sFile();
            if (index != -1)
            {
                y7 = ftc.files[index];
                ov7 = ftc.files.FindAll(sFile => sFile.name.StartsWith("overlay7_"));
            }

            #region Get ROM sections
            BinaryReader br;
            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S08"));
            Nitro.Estructuras.ROMHeader header = romInfo.Cabecera;
            uint currPos = header.headerSize;


            // Write ARM9
            string arm9Binary = Path.GetTempFileName();
            string overlays9 = Path.GetTempFileName();
            Console.Write("\tARM9 Binary...");

            br = new BinaryReader(File.OpenRead(arm9.path));
            br.BaseStream.Position = arm9.offset;
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(arm9Binary));
            bw.Write(br.ReadBytes((int)arm9.size));
            bw.Flush();
            br.Close();

            header.ARM9romOffset = currPos;
            header.ARM9size = arm9.size;
            header.ARM9overlayOffset = 0;
            uint arm9overlayOffset = 0;

            currPos += arm9.size;

            // Write the Nitrocode
            br = new BinaryReader(File.OpenRead(accion.ROMFile));
            br.BaseStream.Position = romInfo.Cabecera.ARM9romOffset + romInfo.Cabecera.ARM9size;
            if (br.ReadUInt32() == 0xDEC00621)
            {
                // Nitrocode found
                bw.Write(0xDEC00621);
                bw.Write(br.ReadUInt32());
                bw.Write(br.ReadUInt32());
                currPos += 0x0C;
                bw.Flush();
            }
            br.Close();

            uint rem = currPos % 0x200;
            if (rem != 0)
            {
                while (rem < 0x200)
                {
                    bw.Write((byte)0xFF);
                    rem++;
                    currPos++;
                }
            }

            if (header.ARM9overlaySize != 0)
            {
                // ARM9 Overlays Tables
                br = new BinaryReader(File.OpenRead(y9.path));
                br.BaseStream.Position = y9.offset;
                Nitro.Overlay.Write_Y9(bw, br, ov9.ToArray());
                bw.Flush();
                br.Close();
                header.ARM9overlayOffset = currPos;
                header.ARM9overlaySize = y9.size;

                currPos += y9.size;
                rem = currPos % 0x200;
                if (rem != 0)
                {
                    while (rem < 0x200)
                    {
                        bw.Write((byte)0xFF);
                        rem++;
                        currPos++;
                    }
                }

                Nitro.Overlay.EscribirOverlays(overlays9, ov9, accion.ROMFile);
                bw.Write(File.ReadAllBytes(overlays9)); // ARM9 Overlays
                arm9overlayOffset = currPos;
                currPos += (uint)new FileInfo(overlays9).Length;
            }
            bw.Flush();
            bw.Close();

            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S09"), new FileInfo(arm9Binary).Length);


            // Escribismo el ARM7 Binary
            string arm7Binary = Path.GetTempFileName();
            string overlays7 = Path.GetTempFileName();
            Console.Write("\tARM7 Binary...");

            br = new BinaryReader(File.OpenRead(arm7.path));
            br.BaseStream.Position = arm7.offset;
            bw = new BinaryWriter(File.OpenWrite(arm7Binary));
            bw.Write(br.ReadBytes((int)arm7.size));
            bw.Flush();
            br.Close();

            header.ARM7romOffset = currPos;
            header.ARM7size = arm7.size;
            header.ARM7overlayOffset = 0x00;
            uint arm7overlayOffset = 0x00;

            currPos += arm7.size;
            rem = currPos % 0x200;
            if (rem != 0)
            {
                while (rem < 0x200)
                {
                    bw.Write((byte)0xFF);
                    rem++;
                    currPos++;
                }
            }

            if (romInfo.Cabecera.ARM7overlaySize != 0x00)
            {
                // ARM7 Overlays Tables
                br = new BinaryReader(File.OpenRead(y7.path));
                br.BaseStream.Position = y7.offset;
                bw.Write(br.ReadBytes((int)y7.size));
                bw.Flush();
                br.Close();
                header.ARM7overlayOffset = currPos;
                header.ARM7overlaySize = y7.size;

                currPos += y7.size;
                rem = currPos % 0x200;
                if (rem != 0)
                {
                    while (rem < 0x200)
                    {
                        bw.Write((byte)0xFF);
                        rem++;
                        currPos++;
                    }
                }

                Nitro.Overlay.EscribirOverlays(overlays7, ov7, accion.ROMFile);
                bw.Write(File.ReadAllBytes(overlays7)); // ARM7 Overlays

                arm7overlayOffset = currPos;
                currPos += (uint)new FileInfo(overlays7).Length;
            }
            bw.Flush();
            bw.Close();
            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S09"), new FileInfo(arm7Binary).Length);


            // Escribimos el FNT (File Name Table)
            string fileFNT = Path.GetTempFileName();
            Console.Write("\tFile Name Table (FNT)...");

            bw = new BinaryWriter(File.OpenWrite(fileFNT));
            br = new BinaryReader(File.OpenRead(fnt.path));
            br.BaseStream.Position = fnt.offset;
            bw.Write(br.ReadBytes((int)fnt.size));
            bw.Flush();
            br.Close();
            header.fileNameTableSize = fnt.size;
            header.fileNameTableOffset = currPos;

            currPos += fnt.size;
            rem = currPos % 0x200;
            if (rem != 0)
            {
                while (rem < 0x200)
                {
                    bw.Write((byte)0xFF);
                    rem++;
                    currPos++;
                }
            }
            bw.Flush();
            bw.Close();

            Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S09"), new FileInfo(fileFNT).Length);


            // Escribimos el FAT (File Allocation Table)
            string fileFAT = Path.GetTempFileName();
            header.FAToffset = currPos;
            Nitro.FAT.Write(fileFAT, accion.Root, header.FAToffset, accion.SortedIDs, arm9overlayOffset, arm7overlayOffset);
            currPos += (uint)new FileInfo(fileFAT).Length;

            // Escribimos el banner
            string banner = Path.GetTempFileName();
            Nitro.NDS.EscribirBanner(banner, romInfo.Banner);
            header.bannerOffset = currPos;
            currPos += (uint)new FileInfo(banner).Length;

            // Escribimos los archivos
            string files = Path.GetTempFileName();
            Nitro.NDS.Write_Files(files, accion.ROMFile, accion.Root, accion.SortedIDs);
            currPos += (uint)new FileInfo(files).Length;

            // Update the ROM size values of the header
            header.ROMsize = currPos;
            header.tamaño = (uint)Math.Ceiling(Math.Log(currPos, 2));
            header.tamaño = (uint)Math.Pow(2, header.tamaño);

            // Get Header CRC
            string tempHeader = Path.GetTempFileName();
            Nitro.NDS.EscribirCabecera(tempHeader, header, accion.ROMFile);
            BinaryReader brHeader = new BinaryReader(File.OpenRead(tempHeader));
            header.headerCRC16 = (ushort)Ekona.Helper.CRC16.Calculate(brHeader.ReadBytes(0x15E));
            brHeader.Close();
            File.Delete(tempHeader);

            // Write header
            string header_file = Path.GetTempFileName();
            Nitro.NDS.EscribirCabecera(header_file, header, accion.ROMFile);


            Console.Write("<br>");
            #endregion

            if (!isMono)
                espera.Abort();


            // Obtenemos el nuevo archivo para guardar
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.DefaultExt = ".nds";
            o.Filter = "Nintendo DS ROM (*.nds)|*.nds";
            o.OverwritePrompt = true;
        Open_Dialog:
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (o.FileName == accion.ROMFile)
                {
                    MessageBox.Show(Tools.Helper.GetTranslation("Sistema", "S44"));
                    goto Open_Dialog;
                }

                espera = new Thread(ThreadEspera);
                if (!isMono)
                    espera.Start("S06");

                Console.WriteLine(Tools.Helper.GetTranslation("Messages", "S0D"), o.FileName);
                bw = new BinaryWriter(new FileStream(o.FileName, FileMode.Create));

                Ekona.Helper.IOutil.Append(ref bw, header_file);
                Ekona.Helper.IOutil.Append(ref bw, arm9Binary);
                Ekona.Helper.IOutil.Append(ref bw, arm7Binary);
                Ekona.Helper.IOutil.Append(ref bw, fileFNT);
                Ekona.Helper.IOutil.Append(ref bw, fileFAT);
                Ekona.Helper.IOutil.Append(ref bw, banner);
                Ekona.Helper.IOutil.Append(ref bw, files);

                rem = header.tamaño - (uint)bw.BaseStream.Position;
                while (rem > 0)
                {
                    bw.Write((byte)0xFF);
                    rem--;
                }
                bw.Flush();
                bw.Close();

                Console.WriteLine("<b>" + Tools.Helper.GetTranslation("Messages", "S09") + "</b>", new FileInfo(o.FileName).Length);
                accion.IsNewRom = false;
            }

            // Borramos archivos ya innecesarios
            File.Delete(header_file);
            File.Delete(arm9Binary);
            File.Delete(overlays9);
            File.Delete(arm7Binary);
            File.Delete(overlays7);
            File.Delete(fileFNT);
            File.Delete(fileFAT);
            File.Delete(banner);
            File.Delete(files);

            if (!isMono)
            {
                espera.Abort();
                debug.Add_Text(sb.ToString());
            }
            sb.Length = 0;
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.CheckPathExists = true;
            o.Multiselect = true;
            if (o.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            // The current selected file will be changed if they only select one file
            if (o.FileNames.Length == 1 && accion.IDSelect < 0xF000)
            {
                accion.Change_File(accion.IDSelect, o.FileNames[0]);
                return;
            }

            // If more than one file is selected, they will be changed by name
            foreach (string currFile in o.FileNames)
            {
                sFolder filesWithSameName = new sFolder();
                if (accion.IDSelect > 0xF000)
                    filesWithSameName = accion.Search_FileName(Path.GetFileName(currFile), accion.Selected_Folder());
                else
                    filesWithSameName = accion.Search_FileName(Path.GetFileName(currFile));

                sFile fileToBeChanged;
                if (filesWithSameName.files.Count == 0)
                    continue;
                else if (filesWithSameName.files.Count == 1)
                    fileToBeChanged = filesWithSameName.files[0];
                else
                {
                    // Get relative path
                    for (int i = 0; i < filesWithSameName.files.Count; i++)
                    {
                        sFile file = filesWithSameName.files[i];
                        file.tag = accion.Get_RelativePath(filesWithSameName.files[i].id, "", accion.Root);
                        filesWithSameName.files[i] = file;
                    }

                    Dialog.SelectFile dialog = new Dialog.SelectFile(filesWithSameName.files.ToArray());
                    if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        continue;

                    fileToBeChanged = dialog.SelectedFile;
                }

                accion.Change_File(fileToBeChanged.id, currFile);
            }
        }
        #endregion

        #region Toolbar buttons
        private void ToolStripLang_Click(Object sender, EventArgs e)
        {
            XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");
            string idioma = ((ToolStripMenuItem)sender).Text;
            xml.Element("Options").Element("Language").Value = idioma;
            xml.Save(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");

            MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S07"));
        }
        private void stripRefreshMsg_Click(object sender, EventArgs e)
        {
            if (!isMono)
                debug.Add_Text(sb.ToString());
            sb.Length = 0;
        }
        private void toolStripInfoRom_Click(object sender, EventArgs e)
        {
            if (toolStripInfoRom.Checked)
                romInfo.Show();
            else
                romInfo.Hide();
        }
        private void toolStripDebug_Click(object sender, EventArgs e)
        {
            if (toolStripDebug.Checked)
                debug.Show();
            else
                debug.Hide();
        }
        private void toolStripOpen_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ExecutablePath);
        }
        private void toolStripVentana_Click(object sender, EventArgs e)
        {
            if (toolStripVentana.Checked)
            {
                this.Width = 650;
                btnDesplazar.Enabled = false;
                if (panelObj.Controls.Count > 0)
                {
                    Visor visor = new Visor();
                    visor.Controls.Add(panelObj.Controls[0]);
                    visor.Show();
                }
            }
            else
            {
                btnDesplazar.Enabled = true;
                btnDesplazar.Text = ">>>>>";
            }

        }
        void romInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            toolStripInfoRom.Checked = romInfo.Visible;
        }
        void debug_FormClosing(object sender, FormClosingEventArgs e)
        {
            toolStripDebug.Checked = debug.Visible;
        }
        #endregion

        #region Menu-> Open as...
        private void ShowControl(Control control, string name)
        {
            if (toolStripVentana.Checked)
            {
                Visor visor = new Visor();
                visor.Controls.Add(control);
                visor.Text += " - " + name;
                visor.Show();
            }
            else if (control is Control)
            {
                panelObj.Controls.Clear();

                if (control.Size.Height != 0 && control.Size.Width != 0)
                {
                    panelObj.Controls.Add(control);
                    if (btnDesplazar.Text == ">>>>>")
                        btnDesplazar.PerformClick();
                }
                else
                    if (btnDesplazar.Text == "<<<<<")
                        btnDesplazar.PerformClick();
            }
        }
        private void toolAbrirComoItemPaleta_Click(object sender, EventArgs e)
        {
            sFile selectedFile = accion.Selected_File();
            String[] pluginList = accion.Get_PluginsList();

            Control control = new Control();

            if (pluginList.Contains("Images.Main"))
                control = (Control)accion.Call_Plugin(selectedFile, "Images.Main", "ntfp", selectedFile.id, "", 1);

            if (!isMono)
                debug.Add_Text(sb.ToString());
            sb.Length = 0;

            ShowControl(control, selectedFile.name);
        }
        private void toolAbrirComoItemTile_Click(object sender, EventArgs e)
        {
            sFile selectedFile = accion.Selected_File();
            String[] pluginList = accion.Get_PluginsList();

            Control control = new Control();

            if (pluginList.Contains("Images.Main"))
                control = (Control)accion.Call_Plugin(selectedFile, "Images.Main", "ntft", selectedFile.id, "", 1);

            if (!isMono)
                debug.Add_Text(sb.ToString());
            sb.Length = 0;

            ShowControl(control, selectedFile.name);
        }
        private void toolAbrirComoItemScreen_Click(object sender, EventArgs e)
        {
            sFile selectedFile = accion.Selected_File();
            String[] pluginList = accion.Get_PluginsList();

            Control control = new Control();

            if (pluginList.Contains("Images.Main"))
                control = (Control)accion.Call_Plugin(selectedFile, "Images.Main", "nbfs", selectedFile.id, "", 1);

            if (!isMono)
                debug.Add_Text(sb.ToString());
            sb.Length = 0;

            ShowControl(control, selectedFile.name);
        }
        private void s2AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialog.SelectOffset dialog = new Dialog.SelectOffset();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            #region Save the new file

            String currFile = Path.GetTempFileName();
            accion.Save_File(accion.IDSelect, currFile);

            string tempFile = Path.GetTempPath() + Path.DirectorySeparatorChar + accion.Selected_File().name;
            Byte[] compressFile = new Byte[(new FileInfo(currFile).Length) - dialog.Offset];
            Array.Copy(File.ReadAllBytes(currFile), dialog.Offset, compressFile, 0, compressFile.Length); ;
            File.WriteAllBytes(tempFile, compressFile);
            #endregion

            sFolder uncompress = accion.Unpack(tempFile, accion.IDSelect);

            if (!(uncompress.files is List<sFile>) && !(uncompress.folders is List<sFolder>))
            {
                MessageBox.Show(Tools.Helper.GetTranslation("Sistema", "S36"));
                return;
            }

            toolStripOpenAs.Enabled = false;
            Get_SupportedFiles();

            TreeNode selected = treeSystem.SelectedNode;
            FolderToNode(uncompress, ref selected);
            selected.ImageIndex = accion.ImageFormatFile(accion.Selected_File().format);
            selected.SelectedImageIndex = selected.ImageIndex;

            // Add nodes
            TreeNode[] nodos = new TreeNode[selected.Nodes.Count]; selected.Nodes.CopyTo(nodos, 0);
            treeSystem.SelectedNode.Tag = selected.Tag;
            accion.IDSelect = Convert.ToInt32(selected.Tag);
            selected.Nodes.Clear();

            treeSystem.SelectedNode.Nodes.AddRange((TreeNode[])nodos);
            treeSystem.SelectedNode.Expand();
            treeSystem.Focus();

            if (!isMono)
                debug.Add_Text(sb.ToString());
            sb.Length = 0;
        }
        private void toolStripAbrirTexto_Click(object sender, EventArgs e)
        {
            sFile selectedFile = accion.Selected_File();
            String[] pluginList = accion.Get_PluginsList();

            Control control = new Control();

            if (pluginList.Contains("TXT.TXT"))
                control = (Control)accion.Call_Plugin(selectedFile, "TXT.TXT", "txt", selectedFile.id, "", 1);

            if (!isMono)
                debug.Add_Text(sb.ToString());
            sb.Length = 0;

            ShowControl(control, selectedFile.name);
        }
        private void toolStripAbrirFat_Click(object sender, EventArgs e)
        {
            sFile currFile = accion.Selected_File();
            string fileToRead = accion.TempFolder + Path.DirectorySeparatorChar + Path.GetRandomFileName();
            accion.Save_File(currFile, fileToRead);

            Dialog.FATExtract dialog = new Dialog.FATExtract(fileToRead);
            dialog.TempFolder = accion.TempFolder;
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            Thread wait = new Thread(ThreadEspera);
            if (!isMono)
                wait.Start("S04");

            sFolder uncompress = dialog.Files;
            dialog.Dispose();

            if (!(uncompress.files is List<sFile>) || dialog.DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                if (!isMono)
                    wait.Abort();

                MessageBox.Show(Tools.Helper.GetTranslation("Sistema", "S36"));
                return;
            }

            currFile.format = Format.Pack;
            bool edit = accion.IsNewRom;
            accion.Change_File(currFile.id, currFile, accion.Root);
            accion.IsNewRom = edit;
            accion.Add_Files(ref uncompress, accion.IDSelect);

            #region Add files to the treeList
            toolStripOpenAs.Enabled = false;
            Get_SupportedFiles();

            TreeNode selected = treeSystem.SelectedNode;
            FolderToNode(uncompress, ref selected);

            selected.ImageIndex = accion.ImageFormatFile(Format.Pack);
            selected.SelectedImageIndex = accion.ImageFormatFile(Format.Pack);

            // Agregamos los nodos al árbol
            TreeNode[] nodos = new TreeNode[selected.Nodes.Count]; selected.Nodes.CopyTo(nodos, 0);
            treeSystem.SelectedNode.Tag = selected.Tag;
            accion.IDSelect = Convert.ToInt32(selected.Tag);
            selected.Nodes.Clear();

            treeSystem.SelectedNode.Nodes.AddRange((TreeNode[])nodos);
            treeSystem.SelectedNode.Expand();
            #endregion

            if (!isMono)
            {
                wait.Abort();
                debug.Add_Text(sb.ToString());
            }
            sb.Length = 0;
        }
        private void callPluginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sFile opFile = accion.Selected_File();

            Dialog.CallPlugin win = new Dialog.CallPlugin(accion.Get_PluginsList());
            if (opFile.name.Contains('.'))
                win.Extension = opFile.name.Substring(opFile.name.IndexOf('.') + 1);
            win.ID = opFile.id;
            win.Header = accion.Get_MagicIDS(opFile);

            if (win.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            this.Cursor = Cursors.WaitCursor;
            Object action = accion.Call_Plugin(opFile, win.Plugin, win.Extension, win.ID, win.Header, win.Action);

            if (!isMono)
                debug.Add_Text(sb.ToString());
            sb.Length = 0;


            switch (win.Action)
            {
                case 1: // Show_Info
                    if (action == null)
                        break;

                    if (toolStripVentana.Checked)
                    {
                        Visor visor = new Visor();
                        visor.Controls.Add((Control)action);
                        visor.Text += " - " + opFile.name;
                        visor.Show();
                    }
                    else
                    {
                        for (int i = 0; i < panelObj.Controls.Count; i++)
                            panelObj.Controls[i].Dispose();
                        panelObj.Controls.Clear();

                        Control control = (Control)action;
                        if (control.Size.Height != 0 && control.Size.Width != 0)
                        {
                            panelObj.Controls.Add(control);
                            if (btnDesplazar.Text == ">>>>>")
                                btnDesplazar.PerformClick();
                        }
                        else
                            if (btnDesplazar.Text == "<<<<<")
                                btnDesplazar.PerformClick();
                    }
                    break;

                case 2:     // Unpack
                    if (action == null)
                        break;

                    sFolder unpacked = (sFolder)action;
                    if (!(unpacked.files is List<sFile>) && !(unpacked.folders is List<sFolder>))
                    {
                        MessageBox.Show(Tools.Helper.GetTranslation("Sistema", "S36"));
                        break;
                    }

                    toolStripOpenAs.Enabled = false;

                    Get_SupportedFiles();
                    Add_TreeNodes(unpacked);
                    break;

                case 4: // Get format
                    MessageBox.Show(((Format)action).ToString());
                    break;
            }

            this.Cursor = Cursors.Default;
            win.Dispose();
        }
        #endregion

        #region Search
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == "")
            {
                treeSystem.BeginUpdate();
                treeSystem.Nodes.Clear();
                treeSystem.Nodes.Add(Create_Nodes(accion.Root));
                treeSystem.Nodes[0].Expand();
                treeSystem.EndUpdate();
                return;
            }

            Thread waiting = new System.Threading.Thread(ThreadEspera);

            sFolder resul = new sFolder();
            resul.files = new List<sFile>();
            resul.folders = new List<sFolder>();

            #region Search type
            if (txtSearch.Text == "<Ani>")
                resul = accion.Search_File(Format.Animation);
            else if (txtSearch.Text == "<Cell>")
                resul = accion.Search_File(Format.Cell);
            else if (txtSearch.Text == "<Map>")
                resul = accion.Search_File(Format.Map);
            else if (txtSearch.Text == "<Image>")
                resul = accion.Search_File(Format.Tile);
            else if (txtSearch.Text == "<FullImage>")
                resul = accion.Search_File(Format.FullImage);
            else if (txtSearch.Text == "<Palette>")
                resul = accion.Search_File(Format.Palette);
            else if (txtSearch.Text == "<Text>")
                resul = accion.Search_File(Format.Text);
            else if (txtSearch.Text == "<Video>")
                resul = accion.Search_File(Format.Video);
            else if (txtSearch.Text == "<Sound>")
                resul = accion.Search_File(Format.Sound);
            else if (txtSearch.Text == "<Font>")
                resul = accion.Search_File(Format.Font);
            else if (txtSearch.Text == "<Compress>")
                resul = accion.Search_File(Format.Compressed);
            else if (txtSearch.Text == "<Script>")
                resul = accion.Search_File(Format.Script);
            else if (txtSearch.Text == "<Pack>")
                resul = accion.Search_File(Format.Pack);
            else if (txtSearch.Text == "<Texture>")
                resul = accion.Search_File(Format.Texture);
            else if (txtSearch.Text == "<3DModel>")
                resul = accion.Search_File(Format.Model3D);
            else if (txtSearch.Text == "<Unknown>")
                resul = accion.Search_File(Format.Unknown);
            else if (txtSearch.Text.StartsWith("Length: ") && txtSearch.Text.Length > 8 && txtSearch.Text.Length < 17)
                resul = accion.Search_FileLength(Convert.ToInt32(txtSearch.Text.Substring(7)));
            else if (txtSearch.Text.StartsWith("ID: ") && txtSearch.Text.Length > 4 && txtSearch.Text.Length < 13)
            {
                sFile searchedFile = accion.Search_File(Convert.ToInt32(txtSearch.Text.Substring(4), 16));
                if (searchedFile.name is String)
                    resul.files.Add(searchedFile);
            }
            else if (txtSearch.Text.StartsWith("Offset: ") && txtSearch.Text.Length > 8 && txtSearch.Text.Length < 17)
                resul = accion.Search_FileOffset(Convert.ToInt32(txtSearch.Text.Substring(8), 16));
            else if (txtSearch.Text.StartsWith("Header: ") && txtSearch.Text.Length > 8)
            {
                if (!isMono)
                    waiting.Start("S07");

                List<byte> search = new List<byte>();
                for (int i = 8; i + 1 < txtSearch.Text.Length; i += 2)
                    search.Add(Convert.ToByte(txtSearch.Text.Substring(i, 2), 16));

                if (search.Count != 0)
                    resul = accion.Search_File(search.ToArray());
            }
            else
                resul = accion.Search_File(txtSearch.Text);
            #endregion

            resul.id = (ushort)accion.LastFolderID;
            accion.LastFolderID++;
            if (resul.folders is List<sFolder>)
            {
                for (int i = 0; i < resul.folders.Count; i++)
                {
                    sFolder newFolder = resul.folders[i];
                    newFolder.id = resul.id;
                    resul.folders[i] = newFolder;
                }
            }

            TreeNode nodo = new TreeNode(Tools.Helper.GetTranslation("Sistema", "S2D"));
            FolderToNode(resul, ref nodo);

            treeSystem.BeginUpdate();
            treeSystem.Nodes.Clear();
            nodo.Name = Tools.Helper.GetTranslation("Sistema", "S2D");
            treeSystem.Nodes.Add(nodo);
            treeSystem.ExpandAll();
            treeSystem.EndUpdate();

            if (!isMono && waiting.ThreadState == ThreadState.Running)
                waiting.Abort();
        }
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            if (!checkSearch.Checked)
                return;
            btnSearch.PerformClick();
        }
        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnSearch.PerformClick();
        }
        private sFolder Get_SearchedFiles()
        {
            sFolder searchFolder = new sFolder();
            searchFolder.files = new List<sFile>();
            searchFolder.folders = new List<sFolder>();
            searchFolder.name = treeSystem.SelectedNode.Name;

            for (int i = 0; i < treeSystem.SelectedNode.Nodes.Count; i++)
            {
                int id = Convert.ToInt32(treeSystem.SelectedNode.Nodes[i].Tag);
                if (id < 0xF000)
                    searchFolder.files.Add(accion.Search_File(id));
                else
                {
                    sFolder carpeta = new sFolder();
                    carpeta.files = new List<sFile>();
                    carpeta.name = treeSystem.SelectedNode.Nodes[i].Name;
                    for (int j = 0; j < treeSystem.SelectedNode.Nodes[i].Nodes.Count; j++)
                        carpeta.files.Add(accion.Search_File(Convert.ToUInt16(treeSystem.SelectedNode.Nodes[i].Nodes[j].Tag)));
                    searchFolder.folders.Add(carpeta);
                }
            }
            return searchFolder;
        }
        #endregion

        private void linkAboutBox_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkAboutBox.LinkVisited = false;
            Autores ven = new Autores();
            ven.ShowDialog();
        }
        private void btnDesplazar_Click(object sender, EventArgs e)
        {
            if (btnDesplazar.Text == ">>>>>")
            {
                this.Width += panelObj.Width + 7;
                btnDesplazar.Text = "<<<<<";
            }
            else
            {
                this.Width -= (panelObj.Width + 7);
                btnDesplazar.Text = ">>>>>";
            }
        }

    }
}
