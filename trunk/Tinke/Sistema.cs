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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using PluginInterface;

namespace Tinke
{
    public partial class Sistema : Form
    {
        // Cuadro de ventanas
        RomInfo romInfo;
        Debug debug;

        Acciones accion;
        StringBuilder sb;
        int filesSupported;
        int nFiles;
        bool isMono;
        Keys keyDown;


        public Sistema()
        {
            InitializeComponent();
            this.Location = new Point(10, 10);
            this.Text = "Tinke V " + Application.ProductVersion + " - romhacking by pleoNeX";

            // Modo debug donde se muestran los mensajes en otra ventana en caso de no ejecutarse en Mono
            isMono = (Type.GetType("Mono.Runtime") != null) ? true : false;

            sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            tw.NewLine = "<br>";
            if (!isMono)
                Console.SetOut(tw);

            #region Idioma
            if (!File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml"))
            {
                File.WriteAllText(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                 "\n<Tinke>\n  <Options>" +
                                 "\n    <Language>Español</Language>" +
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

                toolStripLanguage.DropDownItems.Add(
                    xLang.Attribute("name").Value,
                    iFlag,
                    ToolStripLang_Click);
            }

            LeerIdioma();
            #endregion
            this.Load += new EventHandler(Sistema_Load);
            treeSystem.LostFocus += new EventHandler(treeSystem_LostFocus);
            treeSystem.GotFocus += new EventHandler(treeSystem_LostFocus);
            keyDown = Keys.Escape;
        }
        void Sistema_Load(object sender, EventArgs e)
        {
            // Iniciamos la lectura del archivo.
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
                filesToRead = new String[Environment.GetCommandLineArgs().Length - 2];
                Array.Copy(Environment.GetCommandLineArgs(), 2, filesToRead, 0, filesToRead.Length);
            }

            Thread espera = new System.Threading.Thread(ThreadEspera);
            if (!isMono)
                espera.Start("S02");

            if (filesToRead.Length == 1 && Path.GetFileName(filesToRead[0]).ToUpper().EndsWith(".NDS")) // Si se ha seleccionado un juego de la NDS
                ReadGame(filesToRead[0]);
            else if (filesToRead.Length == 1 && Directory.Exists(filesToRead[0]))
                ReadFolder(filesToRead[0]);
            else
                ReadFiles(filesToRead);

            if (!isMono)
                espera.Abort();


            debug = new Debug();
            debug.FormClosing += new FormClosingEventHandler(debug_FormClosing);
            romInfo.FormClosing += new FormClosingEventHandler(romInfo_FormClosing);
            LoadPreferences();

            debug.Añadir_Texto(sb.ToString());
            sb.Length = 0;

            this.Show();
            debug.ShowInTaskbar = true;
            romInfo.ShowInTaskbar = true;
            this.Activate();
        }
        private void ReadGame(string file)
        {
            DateTime startTime = DateTime.Now;

            romInfo = new RomInfo(file);  // Se obtienen datos de la cabecera
            DateTime t1 = DateTime.Now;
            accion = new Acciones(file, new String(romInfo.Cabecera.gameCode));
            DateTime t2 = DateTime.Now;

            // Obtenemos el sistema de archivos
            sFolder root = FNT(file, romInfo.Cabecera.fileNameTableOffset, romInfo.Cabecera.fileNameTableSize);
            DateTime t3 = DateTime.Now;
            if (!(root.folders is List<sFolder>))
                root.folders = new List<sFolder>();
            root.folders.Add(Añadir_Sistema());
            DateTime t4 = DateTime.Now;

            // Añadimos los offset a cada archivo
            root = FAT(file, romInfo.Cabecera.FAToffset, romInfo.Cabecera.FATsize, root);
            DateTime t5 = DateTime.Now;
            accion.Root = root;
            DateTime t6 = DateTime.Now;

            Set_Formato(root);
            DateTime t7 = DateTime.Now;
            treeSystem.Nodes.Add(Jerarquizar_Nodos(root)); // Mostramos el árbol
            DateTime t8 = DateTime.Now;
            treeSystem.Nodes[0].Expand();

            Get_SupportedFiles();
            DateTime t9 = DateTime.Now;

            XElement xml = Tools.Helper.ObtenerTraduccion("Messages");
            Console.Write("<br><u>" + xml.Element("S0F").Value + "</u><ul><font size=\"2\" face=\"consolas\">");
            Console.WriteLine("<li>" + xml.Element("S10").Value + (t9 - startTime).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S11").Value + (t1 - startTime).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S12").Value + (t2 - t1).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S13").Value + (t3 - t2).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S14").Value + (t4 - t3).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S15").Value + (t5 - t4).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S16").Value + (t6 - t5).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S17").Value + (t7 - t6).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S18").Value + (t8 - t7).ToString() + "</li>");
            Console.WriteLine("<li>" + xml.Element("S19").Value + (t9 - t8).ToString() + "</li>");
            Console.Write("</font></ul><br>");
        }
        private void ReadFiles(string[] files)
        {
            toolStripInfoRom.Enabled = false;
            btnSaveROM.Enabled = false;

            romInfo = new RomInfo(); // Para que no se formen errores...
            DateTime startTime = DateTime.Now;

            accion = new Acciones("", "NO GAME");
            DateTime t1 = DateTime.Now;

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

            Set_Formato(root);
            DateTime t4 = DateTime.Now;
            treeSystem.Nodes.Add(Jerarquizar_Nodos(root)); // Mostramos el árbol
            DateTime t5 = DateTime.Now;
            treeSystem.Nodes[0].Expand();

            Get_SupportedFiles();
            DateTime t6 = DateTime.Now;

            XElement xml = Tools.Helper.ObtenerTraduccion("Messages");
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

            Set_Formato(root);
            DateTime t4 = DateTime.Now;
            treeSystem.Nodes.Add(Jerarquizar_Nodos(root)); // Mostramos el árbol
            DateTime t5 = DateTime.Now;
            treeSystem.Nodes[0].Expand();

            Get_SupportedFiles();
            DateTime t6 = DateTime.Now;

            XElement xml = Tools.Helper.ObtenerTraduccion("Messages");
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
            catch { MessageBox.Show(Tools.Helper.ObtenerTraduccion("Sistema", "S37"), Tools.Helper.ObtenerTraduccion("Sistema", "S3A")); }

            if (accion is Acciones)
            {
                if (accion.IsNewRom & accion.ROMFile != "")
                {
                    if (MessageBox.Show(Tools.Helper.ObtenerTraduccion("Sistema", "S39"), Tools.Helper.ObtenerTraduccion("Sistema", "S3A"),
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                accion.Dispose();
            }
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
            catch { MessageBox.Show(Tools.Helper.ObtenerTraduccion("Sistema", "S38"), Tools.Helper.ObtenerTraduccion("Sistema", "S3A")); }
        }
        private void LeerIdioma()
        {
            try
            {
                XElement xml = Tools.Helper.ObtenerTraduccion("Sistema");

                toolStripOpen.Text = xml.Element("S01").Value;
                toolStripInfoRom.Text = xml.Element("S02").Value;
                toolStripDebug.Text = xml.Element("S03").Value;
                toolStripVentana.Text = xml.Element("S04").Value;
                toolStripPlugin.Text = xml.Element("S05").Value;
                recargarPluginsToolStripMenuItem.Text = xml.Element("S06").Value;
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
                toolStripDeleteChain.Text = xml.Element("S10").Value;
                borrarPaletaToolStripMenuItem.Text = xml.Element("S11").Value;
                borrarTileToolStripMenuItem.Text = xml.Element("S12").Value;
                borrarScreenToolStripMenuItem.Text = xml.Element("S13").Value;
                borrarCeldasToolStripMenuItem.Text = xml.Element("S14").Value;
                borrarAnimaciónToolStripMenuItem.Text = xml.Element("S15").Value;
                s10ToolStripMenuItem.Text = xml.Element("S10").Value;
                toolStripOpenAs.Text = xml.Element("S16").Value;
                toolStripMenuItem1.Text = xml.Element("S17").Value;
                toolStripMenuItem2.Text = xml.Element("S18").Value;
                toolStripMenuItem3.Text = xml.Element("S19").Value;
                btnUnpack.Text = xml.Element("S1A").Value;
                btnExtract.Text = xml.Element("S1B").Value;
                btnSee.Text = xml.Element("S1C").Value;
                btnHex.Text = xml.Element("S1D").Value;
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
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }
        private void ToolStripLang_Click(Object sender, EventArgs e)
        {
            XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");
            string idioma = ((ToolStripMenuItem)sender).Text;
            xml.Element("Options").Element("Language").Value = idioma;
            xml.Save(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");

            MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S07"));
        }

        private sFolder FNT(string file, UInt32 offset, UInt32 size)
        {
            sFolder root = Nitro.FNT.LeerFNT(file, offset);
            accion.Root = root;

            return root;
        }
        private sFile[] ARMOverlay(string file, UInt32 offset, UInt32 size, bool ARM9)
        {
            return Nitro.Overlay.LeerOverlaysBasico(file, offset, size, ARM9);
        }
        private sFolder FAT(string file, UInt32 offset, UInt32 size, sFolder root)
        {
            return Nitro.FAT.LeerFAT(file, offset, size, root);
        }
        private sFolder Añadir_Sistema()
        {
            sFolder ftc = new sFolder();
            ftc.name = "ftc";
            ftc.id = (ushort)accion.LastFolderID;
            accion.LastFolderID++;
            ftc.files = new List<sFile>();
            ftc.files.AddRange(
                ARMOverlay(accion.ROMFile, romInfo.Cabecera.ARM9overlayOffset, romInfo.Cabecera.ARM9overlaySize, true)
                );
            ftc.files.AddRange(
                ARMOverlay(accion.ROMFile, romInfo.Cabecera.ARM7overlayOffset, romInfo.Cabecera.ARM7overlaySize, false)
                );

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

            return ftc;
        }

        private TreeNode Jerarquizar_Nodos(sFolder currFolder)
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
                    currNode.Nodes.Add(Jerarquizar_Nodos(subFolder));


            if (currFolder.files is List<sFile>)
            {
                foreach (sFile archivo in currFolder.files)
                {
                    int nImage = accion.ImageFormatFile(archivo.format);
                    string ext = "";
                    if (archivo.format == Format.Unknown)
                    {
                        ext = accion.Get_MagicIDS(archivo.id);
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
        private void CarpetaANodo(sFolder carpeta, ref TreeNode nodo)
        {
            if (carpeta.id < 0xF000)
            {
                nodo.ImageIndex = accion.ImageFormatFile(accion.Get_Format(carpeta.id));
                nodo.SelectedImageIndex = nodo.ImageIndex;
            }
            else
            {
                nodo.ImageIndex = 0;
                nodo.SelectedImageIndex = 0;
            }
            nodo.Tag = carpeta.id;
            nodo.Name = carpeta.name;

            if (carpeta.folders is List<sFolder>)
            {
                foreach (sFolder subFolder in carpeta.folders)
                {
                    TreeNode newNodo = new TreeNode(subFolder.name);
                    CarpetaANodo(subFolder, ref newNodo);
                    nodo.Nodes.Add(newNodo);
                }
            }


            if (carpeta.files is List<sFile>)
            {
                foreach (sFile archivo in carpeta.files)
                {
                    int nImage = accion.ImageFormatFile(archivo.format);
                    string ext = "";
                    if (archivo.format == Format.Unknown)
                    {
                        ext = accion.Get_MagicIDS(archivo.id);
                        if (ext != "")
                            ext = " [" + ext + ']'; // Previene extensiones vacías
                    }
                    TreeNode fileNode = new TreeNode(archivo.name + ext, nImage, nImage);
                    fileNode.Name = archivo.name;
                    fileNode.Tag = archivo.id;
                    nodo.Nodes.Add(fileNode);
                }
            }


        }
        private void Set_Formato(sFolder carpeta)
        {
            if (carpeta.files is List<sFile>)
            {
                for (int i = 0; i < carpeta.files.Count; i++)
                {
                    sFile newFile = carpeta.files[i];
                    newFile.format = accion.Get_Format(newFile.id);
                    carpeta.files[i] = newFile;
                }
            }


            if (carpeta.folders is List<sFolder>)
                foreach (sFolder subFolder in carpeta.folders)
                    Set_Formato(subFolder);
        }
        private void Get_SupportedFiles()
        {
            filesSupported = nFiles = 0; // Reiniciamos el contador

            RecursivoSupportFile(accion.Root);
            lblSupport.Text = Tools.Helper.ObtenerTraduccion("Sistema", "S30") + ' ' + (filesSupported * 100 / nFiles) + '%';
            if ((filesSupported * 100 / nFiles) >= 75)
                lblSupport.Font = new Font("Consolas", 10, FontStyle.Bold | FontStyle.Underline);
            else
                lblSupport.Font = new Font("Consolas", 10, FontStyle.Regular);
        }
        private void RecursivoSupportFile(sFolder carpeta)
        {
            if (carpeta.files is List<sFile>)
            {
                foreach (sFile archivo in carpeta.files)
                {
                    if (archivo.format == Format.System)
                        continue; // Evitamos archivos del sistema

                    if (archivo.format != Format.Unknown)
                        filesSupported++;
                    nFiles++;
                }
            }

            if (carpeta.folders is List<sFolder>)
                foreach (sFolder subFolder in carpeta.folders)
                    RecursivoSupportFile(subFolder);
        }

        private void ThreadEspera(Object name)
        {
            try
            {
                Espera espera = new Espera((string)name, false);
                espera.ShowDialog();
            }
            catch { }
        }

        private void treeSystem_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btnPack.Enabled = false;
            accion.IDSelect = Convert.ToInt32(e.Node.Tag);
            // Limpiar información anterior
            for (int i = 0; i < listFile.Items.Count; i++)
                if (listFile.Items[i].SubItems.Count == 2)
                    listFile.Items[i].SubItems.RemoveAt(1);

            if (e.Node.Name == "root")
            {
                listFile.Items[0].SubItems.Add("root");
                listFile.Items[1].SubItems.Add("0xF000");
                listFile.Items[2].SubItems.Add("");
                listFile.Items[3].SubItems.Add("");
                listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S1F"));
                listFile.Items[5].SubItems.Add("");

                btnHex.Enabled = false;
                btnSee.Enabled = false;
                toolStripOpenAs.Enabled = false;
                btnUnpack.Enabled = true;
            }
            else if (Convert.ToUInt16(e.Node.Tag) < 0xF000)
            {
                sFile selectFile = accion.Select_File();

                listFile.Items[0].SubItems.Add(selectFile.name);
                listFile.Items[1].SubItems.Add("0x" + String.Format("{0:X}", selectFile.id));
                listFile.Items[2].SubItems.Add("0x" + String.Format("{0:X}", selectFile.offset));
                listFile.Items[3].SubItems.Add(selectFile.size.ToString());
                #region Obtener tipo de archivo traducido
                switch (selectFile.format)
                {
                    case Format.Palette:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S20"));
                        break;
                    case Format.Tile:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S21"));
                        break;
                    case Format.Map:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S22"));
                        break;
                    case Format.Cell:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S23"));
                        break;
                    case Format.Animation:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S24"));
                        break;
                    case Format.FullImage:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S25"));
                        break;
                    case Format.Text:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S26"));
                        break;
                    case Format.Video:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S27"));
                        break;
                    case Format.Sound:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S28"));
                        break;
                    case Format.Font:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S29"));
                        break;
                    case Format.Compressed:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S2A"));
                        break;
                    case Format.Unknown:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S2B"));
                        break;
                    case Format.System:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S31"));
                        break;
                    case Format.Script:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S34"));
                        break;
                    case Format.Pack:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S3D"));
                        break;
                    case Format.Texture:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S3E"));
                        break;
                    case Format.Model3D:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S3F"));
                        break;
                }
                #endregion
                listFile.Items[5].SubItems.Add(selectFile.path);
                listFile.Items[6].SubItems.Add(accion.Get_RelativePath(selectFile.id, "", accion.Root));
                btnHex.Enabled = true;
                toolStripOpenAs.Enabled = true;

                if (selectFile.format != Format.Unknown && selectFile.format != Format.System)
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
                sFolder selectFolder = accion.Select_Folder();

                listFile.Items[0].SubItems.Add(e.Node.Name);
                listFile.Items[1].SubItems.Add("0x" + String.Format("{0:X}", e.Node.Tag));
                listFile.Items[2].SubItems.Add("");
                listFile.Items[3].SubItems.Add("");
                listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S1F"));
                listFile.Items[5].SubItems.Add("");

                btnHex.Enabled = false;
                btnSee.Enabled = false;
                toolStripOpenAs.Enabled = false;
                btnUnpack.Enabled = true;
            }

            listFile.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
        private void btnHex_Click(object sender, EventArgs e)
        {
            sFile file = accion.Select_File();
            VisorHex hex = new VisorHex(file, true);
            hex.Text += " - " + file.name;
            hex.Show();
            hex.FormClosed += new FormClosedEventHandler(hex_FormClosed);
        }

        void hex_FormClosed(object sender, FormClosedEventArgs e)
        {
            VisorHex hex = (VisorHex)sender;
            if (hex.Edited)
                accion.Change_File(hex.NewFile.id, hex.NewFile, accion.Root);
        }
        private void BtnSee(object sender, EventArgs e)
        {
            if (toolStripVentana.Checked)
            {
                Visor visor = new Visor();
                visor.Controls.Add(accion.See_File());
                visor.Text += " - " + accion.Select_File().name;
                visor.Show();
            }
            else
            {
                for (int i = 0; i < panelObj.Controls.Count; i++)
                    panelObj.Controls[i].Dispose();
                panelObj.Controls.Clear();
                Control control = accion.See_File();
                if (control.Size.Height != 0 && control.Size.Width != 0) // Si no sería nulo
                {
                    panelObj.Controls.Add(control);
                    if (btnDesplazar.Text == ">>>>>")
                        btnDesplazar.PerformClick();
                }
                else
                    if (btnDesplazar.Text == "<<<<<")
                        btnDesplazar.PerformClick();
            }
            debug.Añadir_Texto(sb.ToString());
            sb.Length = 0;
        }
        private void btnUnpack_Click(object sender, EventArgs e)
        {
            sFolder uncompress;

            if (accion.IDSelect >= 0x0F000)
            {
                UnpackFolder();
                return;
            }
            if ((String)accion.Select_File().tag == "Descomprimido")
            {
                UnpackFolder();
                return;
            }

            this.Cursor = Cursors.WaitCursor;
            uncompress = accion.Unpack();

            if (!(uncompress.files is List<sFile>) && !(uncompress.folders is List<sFolder>)) // En caso de que falle la extracción
            {
                debug.Añadir_Texto(sb.ToString());
                sb.Length = 0;
                this.Cursor = Cursors.Default;
                keyDown = Keys.Escape;

                MessageBox.Show(Tools.Helper.ObtenerTraduccion("Sistema", "S36"));
                return;
            }

            toolStripOpenAs.Enabled = false;
            btnPack.Enabled = true;

            Get_SupportedFiles();

            // Add new files to the main tree
            TreeNode selected = treeSystem.SelectedNode;
            CarpetaANodo(uncompress, ref selected);
            selected.ImageIndex = accion.ImageFormatFile(accion.Select_File().format);
            selected.SelectedImageIndex = selected.ImageIndex;

            TreeNode[] nodos = new TreeNode[selected.Nodes.Count]; selected.Nodes.CopyTo(nodos, 0);
            treeSystem.SelectedNode.Tag = selected.Tag;
            accion.IDSelect = Convert.ToInt32(selected.Tag);
            selected.Nodes.Clear();

            treeSystem.SelectedNode.Nodes.AddRange((TreeNode[])nodos);
            treeSystem.SelectedNode.Expand();
            treeSystem.Focus();

            debug.Añadir_Texto(sb.ToString());
            sb.Length = 0;
            this.Cursor = Cursors.Default;
        }
        private void btnPack_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;

            accion.Pack();

            debug.Añadir_Texto(sb.ToString());
            sb.Length = 0;
            this.Cursor = Cursors.Default;
        }
        private void UnpackFolder()
        {
            this.Cursor = Cursors.WaitCursor;
            Thread espera = new System.Threading.Thread(ThreadEspera);
            if (!isMono)
                espera.Start("S04");

            Recursivo_UnpackFolder(accion.Select_Folder());
            Get_SupportedFiles();
            treeSystem.Nodes.Clear();
            treeSystem.Nodes.Add(Jerarquizar_Nodos(accion.Root));
            treeSystem.Nodes[0].Expand();

            if (!isMono)
                espera.Abort();

            debug.Añadir_Texto(sb.ToString());
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
        private void btnExtraer_Click(object sender, EventArgs e)
        {
            if (Convert.ToUInt16(treeSystem.SelectedNode.Tag) < 0xF000)
            {
                if ((String)accion.Select_File().tag == "Descomprimido")
                {
                    if (MessageBox.Show(Tools.Helper.ObtenerTraduccion("Sistema", "S3B"), "", MessageBoxButtons.YesNo,
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
            sFile fileSelect = accion.Select_File();

            SaveFileDialog o = new SaveFileDialog();
            o.FileName = fileSelect.name;
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BinaryReader br = new BinaryReader(File.OpenRead(fileSelect.path));
                br.BaseStream.Position = fileSelect.offset;
                File.WriteAllBytes(o.FileName, br.ReadBytes((int)fileSelect.size));
                br.Close();
            }
        }
        private void ExtractFolder()
        {
            sFolder folderSelect = accion.Select_Folder();

            if (!(folderSelect.name is String)) // En caso que sea el resultado de una búsqueda
            {
                folderSelect.files = new List<sFile>();
                folderSelect.folders = new List<sFolder>();
                folderSelect.name = treeSystem.SelectedNode.Name;

                for (int i = 0; i < treeSystem.SelectedNode.Nodes.Count; i++)
                {
                    int id = Convert.ToInt32(treeSystem.SelectedNode.Nodes[i].Tag);
                    if (id < 0xF000)
                        folderSelect.files.Add(accion.Search_File(id));
                    else
                    {
                        sFolder carpeta = new sFolder();
                        carpeta.files = new List<sFile>();
                        carpeta.name = treeSystem.SelectedNode.Nodes[i].Name;
                        for (int j = 0; j < treeSystem.SelectedNode.Nodes[i].Nodes.Count; j++)
                            carpeta.files.Add(accion.Search_File(Convert.ToUInt16(treeSystem.SelectedNode.Nodes[i].Nodes[j].Tag)));
                        folderSelect.folders.Add(carpeta);
                    }
                }
            }

            FolderBrowserDialog o = new FolderBrowserDialog();
            o.ShowNewFolderButton = true;
            o.Description = Tools.Helper.ObtenerTraduccion("Sistema", "S2C");
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
                    BinaryReader br = new BinaryReader(File.OpenRead(archivo.path));
                    br.BaseStream.Position = archivo.offset;
                    File.WriteAllBytes(path + Path.DirectorySeparatorChar + archivo.name, br.ReadBytes((int)archivo.size));
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
        private void treeSystem_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (accion.IDSelect < 0xF000)   // Comprobación de que la selección no sea un directorio
            {
                accion.Read_File();
                debug.Añadir_Texto(sb.ToString());
                sb.Length = 0;
            }
        }
        private void Sistema_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == keyDown)
                return;

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
            Application.Restart();
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

        private void btnDesplazar_Click(object sender, EventArgs e)
        {
            if (btnDesplazar.Text == ">>>>>")
            {
                this.Width = 1167;
                btnDesplazar.Text = "<<<<<";
            }
            else
            {
                this.Width = 650;
                btnDesplazar.Text = ">>>>>";
            }
        }

        private void recargarPluginsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            accion.Liberar_Plugins();
            accion.Cargar_Plugins();
        }

        private void s10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            accion.Delete_PicturesSaved();
        }
        private void borrarPaletaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            accion.Delete_PicturesSaved(Format.Palette);
        }
        private void borrarTileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            accion.Delete_PicturesSaved(Format.Tile);
        }
        private void borrarScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            accion.Delete_PicturesSaved(Format.Map);
        }
        private void borrarCeldasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            accion.Delete_PicturesSaved(Format.Cell);
        }
        private void borrarAnimaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            accion.Delete_PicturesSaved(Format.Animation);
        }

        private void AbrirComo(Format formato)
        {
            sFile selectedFile = accion.Select_File();

            if (formato == Format.Text)
                accion.Save_File(accion.IDSelect, selectedFile.path + ".txt");

            if (toolStripVentana.Checked)
            {
                Visor visor = new Visor();
                Control control;
                if (formato == Format.Text)
                    control = accion.See_File(selectedFile.path + ".txt");
                else
                    control = accion.Set_PicturesSaved(formato);
                visor.Controls.Add(control);
                visor.Text += " - " + accion.Select_File().name;
                visor.Show();
            }
            else
            {
                panelObj.Controls.Clear();
                Control control;
                if (formato == Format.Text)
                    control = accion.See_File(selectedFile.path + ".txt");
                else
                    control = accion.Set_PicturesSaved(formato);

                if (control.Size.Height != 0 && control.Size.Width != 0) // Si no sería nulo
                {
                    panelObj.Controls.Add(control);
                    if (btnDesplazar.Text == ">>>>>")
                        btnDesplazar.PerformClick();
                }
                else
                    if (btnDesplazar.Text == "<<<<<")
                        btnDesplazar.PerformClick();
            }
            debug.Añadir_Texto(sb.ToString());
            sb.Length = 0;
        }
        private void toolAbrirComoItemPaleta_Click(object sender, EventArgs e)
        {
            AbrirComo(Format.Palette);
        }
        private void toolAbrirComoItemTile_Click(object sender, EventArgs e)
        {
            AbrirComo(Format.Tile);
        }
        private void toolAbrirComoItemScreen_Click(object sender, EventArgs e)
        {
            AbrirComo(Format.Map);
        }
        private void s2AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dialog.SelectOffset dialog = new Dialog.SelectOffset();
            if (dialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            #region Save the new file

            String currFile = Path.GetTempFileName();
            accion.Save_File(accion.IDSelect, currFile);

            string tempFile = Path.GetTempPath() + Path.DirectorySeparatorChar + accion.Select_File().name;
            Byte[] compressFile = new Byte[(new FileInfo(currFile).Length) - dialog.Offset];
            Array.Copy(File.ReadAllBytes(currFile), dialog.Offset, compressFile, 0, compressFile.Length); ;
            File.WriteAllBytes(tempFile, compressFile);
            #endregion

            sFolder uncompress = accion.Unpack(tempFile, accion.IDSelect);

            if (!(uncompress.files is List<sFile>) && !(uncompress.folders is List<sFolder>)) // En caso de que falle la extracción
            {
                MessageBox.Show(Tools.Helper.ObtenerTraduccion("Sistema", "S36"));
                return;
            }

            toolStripOpenAs.Enabled = false;
            Get_SupportedFiles();

            TreeNode selected = treeSystem.SelectedNode;
            CarpetaANodo(uncompress, ref selected);
            selected.ImageIndex = accion.ImageFormatFile(accion.Select_File().format);
            selected.SelectedImageIndex = selected.ImageIndex;

            // Agregamos los nodos al árbol
            TreeNode[] nodos = new TreeNode[selected.Nodes.Count]; selected.Nodes.CopyTo(nodos, 0);
            treeSystem.SelectedNode.Tag = selected.Tag;
            accion.IDSelect = Convert.ToInt32(selected.Tag);
            selected.Nodes.Clear();

            treeSystem.SelectedNode.Nodes.AddRange((TreeNode[])nodos);
            treeSystem.SelectedNode.Expand();
            treeSystem.Focus();

            debug.Añadir_Texto(sb.ToString());
            sb.Length = 0;
        }
        private void toolStripAbrirTexto_Click(object sender, EventArgs e)
        {
            AbrirComo(Format.Text);
        }
        private void toolStripAbrirFat_Click(object sender, EventArgs e)
        {
            string fileToRead = Path.GetTempFileName();
            accion.Save_File(accion.Select_File(), fileToRead);

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

                MessageBox.Show(Tools.Helper.ObtenerTraduccion("Sistema", "S36"));
                return;
            }

            accion.Add_Files(ref uncompress, accion.IDSelect);

            #region Add files to the treeList
            toolStripOpenAs.Enabled = false;
            Get_SupportedFiles();

            TreeNode selected = treeSystem.SelectedNode;
            CarpetaANodo(uncompress, ref selected);

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
                wait.Abort();

            debug.Añadir_Texto(sb.ToString());
            sb.Length = 0;
        }

        private void linkAboutBox_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkAboutBox.LinkVisited = false;
            Autores ven = new Autores();
            ven.ShowDialog();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == "")
            {
                treeSystem.Nodes.Clear();
                treeSystem.Nodes.Add(Jerarquizar_Nodos(accion.Root));
                treeSystem.Nodes[0].Expand();
                return;
            }

            sFolder resul = new sFolder();
            resul.files = new List<sFile>();
            resul.folders = new List<sFolder>();

            if (txtSearch.Text == "<Ani>")
                resul = accion.Search_File(Format.Animation);
            else if (txtSearch.Text == "<Cell>")
                resul = accion.Search_File(Format.Cell);
            else if (txtSearch.Text == "<Map>")
                resul = accion.Search_File(Format.Map);
            else if (txtSearch.Text == "<Image>")
                resul = accion.Search_File(Format.Tile);
            else if (txtSearch.Text == "<FullImage")
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
            else
                resul = accion.Search_File(txtSearch.Text);

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

            TreeNode nodo = new TreeNode(Tools.Helper.ObtenerTraduccion("Sistema", "S2D"));
            CarpetaANodo(resul, ref nodo);
            treeSystem.Nodes.Clear();
            nodo.Name = Tools.Helper.ObtenerTraduccion("Sistema", "S2D");
            treeSystem.Nodes.Add(nodo);
            treeSystem.ExpandAll();
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

        private void btnSaveROM_Click(object sender, EventArgs e)
        {
            /* Una ROM se compone por las siguientes secciones:
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
             *   |_Icon (Bitmap + palette) 0x200 + 0x20
             *   |_Game titles (Japanese, English, French, German, Italian, Spanish) 6 * 0x100
             * Files...
            */

            Thread espera = new Thread(ThreadEspera);
            if (!isMono)
                espera.Start("S05");

            #region Preparativos
            // Copia para no modificar el originial
            List<sFile> origianlFiles = new List<sFile>();
            origianlFiles.AddRange((sFile[])accion.Search_Folder("ftc").files.ToArray().Clone());

            // Quitamos archivos especiales que no cuentan en la ROM
            int id = accion.Search_File("fnt.bin").files[0].id;
            accion.Remove_File(id, accion.Root);

            id = accion.Search_File("fat.bin").files[0].id;
            accion.Remove_File(id, accion.Root);

            id = accion.Search_File("arm9.bin").files[0].id;
            accion.Remove_File(id, accion.Root);

            id = accion.Search_File("arm7.bin").files[0].id;
            accion.Remove_File(id, accion.Root);

            sFolder y9 = accion.Search_File("y9.bin");
            if (y9.files.Count > 0)
            {
                id = y9.files[0].id;
                accion.Remove_File(id, accion.Root);
            }

            sFolder y7 = accion.Search_File("y7.bin");
            if (y7.files.Count > 0)
            {
                id = y7.files[0].id;
                accion.Remove_File(id, accion.Root);
            }

            // Obtenemos el último ID de archivo sin los especiales
            //accion.LastFileID = 0;
            //accion.Set_LastFileID(accion.Root);
            //accion.LastFolderID = 0;
            //accion.Set_LastFolderID(accion.Root);
            #endregion

            #region Obtención de regiones de la ROM
            BinaryReader br;
            Console.WriteLine(Tools.Helper.ObtenerTraduccion("Messages", "S08"));
            Nitro.Estructuras.ROMHeader cabecera = romInfo.Cabecera;


            // Escribimos el ARM9 Binary
            string arm9Binary = Path.GetTempFileName();
            string overlays9 = Path.GetTempFileName();
            Console.Write("\tARM9 Binary...");

            br = new BinaryReader(new FileStream(accion.ROMFile, FileMode.Open));
            br.BaseStream.Position = romInfo.Cabecera.ARM9romOffset; // ARM9
            BinaryWriter bw = new BinaryWriter(new FileStream(arm9Binary, FileMode.Create));
            bw.Write(br.ReadBytes((int)romInfo.Cabecera.ARM9size));
            bw.Flush();
            cabecera.ARM9romOffset = cabecera.headerSize;

            br.BaseStream.Position = romInfo.Cabecera.ARM9overlayOffset; // ARM9 Overlays Tables
            bw.Write(br.ReadBytes((int)romInfo.Cabecera.ARM9overlaySize));
            bw.Flush();
            br.Close();
            cabecera.ARM9overlayOffset = cabecera.ARM9romOffset + cabecera.ARM9size;

            Nitro.Overlay.EscribirOverlays(overlays9, accion.Search_File("overlay9_"), accion.ROMFile);
            bw.Write(File.ReadAllBytes(overlays9)); // ARM9 Overlays
            bw.Flush();
            bw.Close();
            uint arm9overlayOffset = cabecera.ARM9overlayOffset + cabecera.ARM9overlaySize;

            Console.WriteLine(Tools.Helper.ObtenerTraduccion("Messages", "S09"), new FileInfo(arm9Binary).Length);


            // Escribismo el ARM7 Binary
            string arm7Binary = Path.GetTempFileName();
            string overlays7 = Path.GetTempFileName();

            Console.Write("\tARM7 Binary...");
            br = new BinaryReader(new FileStream(accion.ROMFile, FileMode.Open));
            br.BaseStream.Position = romInfo.Cabecera.ARM7romOffset; // ARM7
            bw = new BinaryWriter(new FileStream(arm7Binary, FileMode.Create));
            bw.Write(br.ReadBytes((int)romInfo.Cabecera.ARM7size));
            bw.Flush();
            br.Close();
            cabecera.ARM7romOffset = cabecera.ARM9romOffset + (uint)new FileInfo(arm9Binary).Length;
            cabecera.ARM7overlayOffset = 0x00;
            uint arm7overlayOffset = 0x00;

            if (romInfo.Cabecera.ARM7overlaySize != 0x00)
            {
                br = new BinaryReader(new FileStream(accion.ROMFile, FileMode.Open));
                br.BaseStream.Position = romInfo.Cabecera.ARM7overlayOffset; // ARM7 Overlays Tables
                bw.Write(br.ReadBytes((int)romInfo.Cabecera.ARM7overlaySize));
                bw.Flush();
                br.Close();
                cabecera.ARM7overlayOffset = cabecera.ARM7romOffset + cabecera.ARM7size;

                Nitro.Overlay.EscribirOverlays(overlays7, accion.Search_File("overlay7_"), accion.ROMFile);
                bw.Write(File.ReadAllBytes(overlays7)); // ARM7 Overlays
                bw.Flush();
                arm7overlayOffset = cabecera.ARM7overlayOffset + cabecera.ARM7overlaySize;
            }
            bw.Close();
            Console.WriteLine(Tools.Helper.ObtenerTraduccion("Messages", "S09"), new FileInfo(arm7Binary).Length);


            // Escribimos el FNT (File Name Table)
            string fileFNT = Path.GetTempFileName();
            Console.Write("\tFile Name Table (FNT)...");
            br = new BinaryReader(new FileStream(accion.ROMFile, FileMode.Open));
            br.BaseStream.Position = romInfo.Cabecera.fileNameTableOffset;
            File.WriteAllBytes(fileFNT, br.ReadBytes((int)romInfo.Cabecera.fileNameTableSize));
            br.Close();
            Console.WriteLine(Tools.Helper.ObtenerTraduccion("Messages", "S09"), new FileInfo(fileFNT).Length);
            cabecera.fileNameTableOffset = cabecera.ARM7romOffset + cabecera.ARM7size;


            // Escribimos el FAT (File Allocation Table)
            cabecera.FAToffset = cabecera.fileNameTableOffset + cabecera.fileNameTableSize;
            string fileFAT = Path.GetTempFileName();
            Nitro.FAT.EscribirFAT(fileFAT, accion.Root, (int)romInfo.Cabecera.FATsize / 0x08,
                cabecera.FAToffset, arm9overlayOffset, arm7overlayOffset);


            // Escribimos el banner
            string banner = Path.GetTempFileName();
            Nitro.NDS.EscribirBanner(banner, romInfo.Banner);
            cabecera.bannerOffset = cabecera.FAToffset + cabecera.FATsize;


            // Escribimos cabecera
            // Get Header CRC
            string tempHeader = Path.GetTempFileName();
            Nitro.NDS.EscribirCabecera(tempHeader, cabecera, accion.ROMFile);
            BinaryReader brHeader = new BinaryReader(File.OpenRead(tempHeader));
            cabecera.headerCRC16 = (ushort)Tools.CRC16.Calcular(brHeader.ReadBytes(0x15E));
            brHeader.Close();
            File.Delete(tempHeader);
            // Write file
            string header = Path.GetTempFileName();
            Nitro.NDS.EscribirCabecera(header, cabecera, accion.ROMFile);


            // Escribimos los archivos
            string files = Path.GetTempFileName();
            Nitro.NDS.EscribirArchivos(files, accion.ROMFile, accion.Root, (int)romInfo.Cabecera.FATsize / 0x08);
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
                    MessageBox.Show(Tools.Helper.ObtenerTraduccion("Sistema", "S44"));
                    goto Open_Dialog;
                }

                espera = new Thread(ThreadEspera);
                if (!isMono)
                    espera.Start("S06");

                Console.WriteLine(Tools.Helper.ObtenerTraduccion("Messages", "S0D"), o.FileName);
                bw = new BinaryWriter(new FileStream(o.FileName, FileMode.Create));

                bw.Write(File.ReadAllBytes(header));
                bw.Flush();
                bw.Write(File.ReadAllBytes(arm9Binary));
                bw.Flush();
                bw.Write(File.ReadAllBytes(arm7Binary));
                bw.Flush();
                bw.Write(File.ReadAllBytes(fileFNT));
                bw.Flush();
                bw.Write(File.ReadAllBytes(fileFAT));
                bw.Flush();
                bw.Write(File.ReadAllBytes(banner));
                bw.Flush();
                bw.Write(File.ReadAllBytes(files));
                bw.Flush();
                bw.Close();

                Console.WriteLine("<b>" + Tools.Helper.ObtenerTraduccion("Messages", "S09") + "</b>", new FileInfo(o.FileName).Length);
                accion.IsNewRom = false;
            }

            // Devolvemos sus valores buenos
            //accion.Set_LastFileID(accion.Root);
            //accion.Set_LastFolderID(accion.Root);
            //accion.LastFileID++; accion.LastFolderID++;
            accion.Search_Folder("ftc").files.Clear(); ;
            accion.Search_Folder("ftc").files.AddRange(origianlFiles);

            // Borramos archivos ya innecesarios
            File.Delete(header);
            File.Delete(arm9Binary);
            File.Delete(overlays9);
            File.Delete(arm7Binary);
            File.Delete(overlays7);
            File.Delete(fileFNT);
            File.Delete(fileFAT);
            File.Delete(banner);
            File.Delete(files);

            debug.Añadir_Texto(sb.ToString());
            sb.Length = 0;

            if (!isMono)
                espera.Abort();

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
                sFile newFile = new sFile();
                sFile fileToBeChanged = accion.Select_File();
                newFile.name = fileToBeChanged.name;
                newFile.id = fileToBeChanged.id;
                newFile.offset = 0x00;
                if (accion.ROMFile == "") // If the user haven't opened a rom, the file is overwritten
                {
                    File.Copy(o.FileNames[0], fileToBeChanged.path, true);
                    newFile.path = fileToBeChanged.path;
                }
                else
                    newFile.path = o.FileNames[0];
                newFile.format = fileToBeChanged.format;
                newFile.size = (uint)new FileInfo(o.FileNames[0]).Length;
                if ((String)newFile.tag == "Descomprimido")
                    newFile.tag = String.Format("{0:X}", newFile.size).PadLeft(8, '0') + newFile.path;

                accion.Change_File(fileToBeChanged.id, newFile, accion.Root);
                return;
            }

            // If more than one file is selected, they will be changed by name
            foreach (string currFile in o.FileNames)
            {
                sFolder filesWithSameName = new sFolder();
                if (accion.IDSelect > 0xF000)
                    filesWithSameName = accion.Search_FileName(Path.GetFileName(currFile), accion.Select_Folder());
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

                sFile newFile = new sFile();
                newFile.name = fileToBeChanged.name;
                newFile.id = fileToBeChanged.id;
                newFile.offset = 0x00;
                if (accion.ROMFile == "") // If the user haven't opened a rom, the file is overwritten
                {
                    File.Copy(currFile, fileToBeChanged.path, true);
                    newFile.path = fileToBeChanged.path;
                }
                else
                    newFile.path = currFile;
                newFile.format = fileToBeChanged.format;
                newFile.size = (uint)new FileInfo(currFile).Length;
                if ((String)newFile.tag == "Descomprimido")
                    newFile.tag = String.Format("{0:X}", newFile.size).PadLeft(8, '0') + newFile.path;

                accion.Change_File(fileToBeChanged.id, newFile, accion.Root);
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
    }
}
