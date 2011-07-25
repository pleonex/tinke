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
 * Programa utilizado: Microsoft Visual C# 2010 Express
 * Fecha: 18/02/2011
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

        public Sistema()
        {
            InitializeComponent();
            this.Location = new Point(10, 10);
            this.Text = "Tinke V " + Application.ProductVersion + " - NDScene by pleoNeX";

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
                                 "\n<Tinke>\n  <Options>\n    <Language>Español</Language>\n  </Options>\n</Tinke>",
                                 Encoding.UTF8);
            }

            XElement xml = XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");


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

        }
        void Sistema_Load(object sender, EventArgs e)
        {
            // Iniciamos la lectura del archivo.
            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.Filter = "Nintendo DS rom (*.nds)|*.nds";
            o.DefaultExt = ".nds";
            if (o.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                Application.Exit();
                return;
            }

            Thread espera = new System.Threading.Thread(ThreadEspera);
            if (!isMono)
                espera.Start("S02");
            #region Lectura del archivo
            romInfo = new RomInfo(o.FileName);
            accion = new Acciones(o.FileName, new String(romInfo.Cabecera.gameCode));
            // Obtenemos el sistema de archivos
            Carpeta root = FNT(o.FileName, romInfo.Cabecera.fileNameTableOffset, romInfo.Cabecera.fileNameTableSize);
            if (!(root.folders is List<Carpeta>))
                root.folders = new List<Carpeta>();
            root.folders.Add(Añadir_Sistema());
            // Añadimos los offset a cada archivo
            root = FAT(o.FileName, romInfo.Cabecera.FAToffset, romInfo.Cabecera.FATsize, root);
            accion.Root = root;

            Set_Formato(root);
            treeSystem.Nodes.Add(Jerarquizar_Nodos(root)); // Mostramos el árbol
            treeSystem.Nodes[0].Expand();

            Get_SupportedFiles();
            #endregion
            if (!isMono)
                espera.Abort();


            toolStripDebug.Enabled = !isMono;
            debug = new Debug();
            if (!isMono)
            {
                debug.Show();
                debug.Activate();
            }

            o.Dispose();

            debug.Añadir_Texto(sb.ToString());
            sb.Length = 0;

            this.Show();
            debug.ShowInTaskbar = true;
            this.Activate();
        }

        private void LeerIdioma()
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
            btnDescomprimir.Text = xml.Element("S1A").Value;
            btnExtraer.Text = xml.Element("S1B").Value;
            btnSee.Text = xml.Element("S1C").Value;
            btnHex.Text = xml.Element("S1D").Value;
            checkSearch.Text = xml.Element("S2E").Value;
            toolTipSearch.ToolTipTitle = xml.Element("S2F").Value;

            toolTipSearch.SetToolTip(txtSearch,
                "<Ani> -> " + xml.Element("S24").Value +
                "\n<Cell> -> " + xml.Element("S23").Value +
                "\n<Screen> -> " + xml.Element("S22").Value +
                "\n<Image> -> " + xml.Element("S21").Value +
                "\n<FullImage> -> " + xml.Element("S25").Value +
                "\n<Palette> -> " + xml.Element("S20").Value +
                "\n<Text> -> " + xml.Element("S26").Value +
                "\n<Video> -> " + xml.Element("S27").Value +
                "\n<Sound> -> " + xml.Element("S28").Value +
                "\n<Font> -> " + xml.Element("S29").Value +
                "\n<Compress> -> " + xml.Element("S2A").Value +
                "\n<Unknown> -> " + xml.Element("S2B").Value
                );
            btnImport.Text = xml.Element("S32").Value;
            btnSaveROM.Text = xml.Element("S33").Value;
            toolStripMenuComprimido.Text = xml.Element("S2A").Value;
        }
        private void ToolStripLang_Click(Object sender, EventArgs e)
        {
            string idioma = ((ToolStripMenuItem)sender).Text;
            System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");
            xml.Element("Options").Element("Language").Value = idioma;
            xml.Save(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");

            MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S07"));
        }

        private Carpeta FNT(string file, UInt32 offset, UInt32 size)
        {
            Carpeta root = Nitro.FNT.LeerFNT(file, offset);
            accion.Root = root;

            return root;
        }
        private Archivo[] ARMOverlay(string file, UInt32 offset, UInt32 size, bool ARM9)
        {
            return Nitro.Overlay.LeerOverlaysBasico(file, offset, size, ARM9);
        }
        private Carpeta FAT(string file, UInt32 offset, UInt32 size, Carpeta root)
        {
            return Nitro.FAT.LeerFAT(file, offset, size, root);
        }
        private Carpeta Añadir_Sistema()
        {
            Carpeta ftc = new Carpeta();
            ftc.name = "ftc";
            ftc.id = (ushort)accion.LastFolderID;
            accion.LastFolderID++;
            ftc.files = new List<Archivo>();
            ftc.files.AddRange(
                ARMOverlay(accion.ROMFile, romInfo.Cabecera.ARM9overlayOffset, romInfo.Cabecera.ARM9overlaySize, true)
                );
            ftc.files.AddRange(
                ARMOverlay(accion.ROMFile, romInfo.Cabecera.ARM7overlayOffset, romInfo.Cabecera.ARM7overlaySize, false)
                );

            Archivo fnt = new Archivo();
            fnt.name = "fnt.bin";
            fnt.offset = romInfo.Cabecera.fileNameTableOffset;
            fnt.size = romInfo.Cabecera.fileNameTableSize;
            fnt.id = (ushort)accion.LastFileID;
            accion.LastFileID++;
            ftc.files.Add(fnt);

            Archivo fat = new Archivo();
            fat.name = "fat.bin";
            fat.offset = romInfo.Cabecera.FAToffset;
            fat.size = romInfo.Cabecera.FATsize;
            fat.id = (ushort)accion.LastFileID;
            accion.LastFileID++;
            ftc.files.Add(fat);

            Archivo arm9 = new Archivo();
            arm9.name = "arm9.bin";
            arm9.offset = romInfo.Cabecera.ARM9romOffset;
            arm9.size = romInfo.Cabecera.ARM9size;
            arm9.id = (ushort)accion.LastFileID;
            accion.LastFileID++;
            ftc.files.Add(arm9);

            Archivo arm7 = new Archivo();
            arm7.name = "arm7.bin";
            arm7.offset = romInfo.Cabecera.ARM7romOffset;
            arm7.size = romInfo.Cabecera.ARM7size;
            arm7.id = (ushort)accion.LastFileID;
            accion.LastFileID++;
            ftc.files.Add(arm7);

            if (romInfo.Cabecera.ARM9overlaySize != 0)
            {
                Archivo y9 = new Archivo();
                y9.name = "y9.bin";
                y9.offset = romInfo.Cabecera.ARM9overlayOffset;
                y9.size = romInfo.Cabecera.ARM9overlaySize;
                y9.id = (ushort)accion.LastFileID;
                accion.LastFileID++;
                ftc.files.Add(y9);
            }

            if (romInfo.Cabecera.ARM7overlaySize != 0)
            {
                Archivo y7 = new Archivo();
                y7.name = "y7.bin";
                y7.offset = romInfo.Cabecera.ARM7overlayOffset;
                y7.size = romInfo.Cabecera.ARM7overlaySize;
                y7.id = (ushort)accion.LastFileID;
                accion.LastFileID++;
                ftc.files.Add(y7);
            }

            return ftc;
        }

        private TreeNode Jerarquizar_Nodos(Carpeta currFolder)
        {
            TreeNode currNode = new TreeNode();

            currNode = new TreeNode(currFolder.name, 0, 0);
            currNode.Tag = currFolder.id;
            currNode.Name = currFolder.name;


            if (currFolder.folders is List<Carpeta>)
                foreach (Carpeta subFolder in currFolder.folders)
                    currNode.Nodes.Add(Jerarquizar_Nodos(subFolder));


            if (currFolder.files is List<Archivo>)
            {
                foreach (Archivo archivo in currFolder.files)
                {
                    int nImage = accion.ImageFormatFile(archivo.formato);
                    string ext = "";
                    if (archivo.formato == Formato.Desconocido)
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
        private void CarpetaANodo(Carpeta carpeta, ref TreeNode nodo)
        {
            nodo.ImageIndex = 0;
            nodo.SelectedImageIndex = 0;
            nodo.Tag = carpeta.id;
            nodo.Name = carpeta.name;

            if (carpeta.folders is List<Carpeta>)
            {
                foreach (Carpeta subFolder in carpeta.folders)
                {
                    TreeNode newNodo = new TreeNode(subFolder.name);
                    CarpetaANodo(subFolder, ref newNodo);
                    nodo.Nodes.Add(newNodo);
                }
            }


            if (carpeta.files is List<Archivo>)
            {
                foreach (Archivo archivo in carpeta.files)
                {
                    int nImage = accion.ImageFormatFile(archivo.formato);
                    string ext = "";
                    if (archivo.formato == Formato.Desconocido)
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
        private void Set_Formato(Carpeta carpeta)
        {
            if (carpeta.files is List<Archivo>)
            {
                for (int i = 0; i < carpeta.files.Count; i++)
                {
                    Archivo newFile = carpeta.files[i];
                    newFile.formato = accion.Get_Formato(newFile.id);
                    carpeta.files[i] = newFile;
                }
            }


            if (carpeta.folders is List<Carpeta>)
                foreach (Carpeta subFolder in carpeta.folders)
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
        private void RecursivoSupportFile(Carpeta carpeta)
        {
            if (carpeta.files is List<Archivo>)
            {
                foreach (Archivo archivo in carpeta.files)
                {
                    if (archivo.formato == Formato.Sistema)
                        continue; // Evitamos archivos del sistema

                    if (archivo.formato != Formato.Desconocido)
                        filesSupported++;
                    nFiles++;
                }
            }

            if (carpeta.folders is List<Carpeta>)
                foreach (Carpeta subFolder in carpeta.folders)
                    RecursivoSupportFile(subFolder);
        }

        private void ThreadEspera(Object name)
        {
            Espera espera = new Espera((string)name, false);
            espera.ShowDialog();
        }

        private void treeSystem_AfterSelect(object sender, TreeViewEventArgs e)
        {
            accion.IDSelect = Convert.ToInt32(e.Node.Tag);
            // Limpiar información anterior
            if (listFile.Items[0].SubItems.Count == 2)
                for (int i = 0; i < listFile.Items.Count; i++)
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
                btnDescomprimir.Enabled = true;
                btnImport.Enabled = false;
            }
            else if (Convert.ToUInt16(e.Node.Tag) < 0xF000)
            {
                Archivo selectFile = accion.Select_File();

                listFile.Items[0].SubItems.Add(selectFile.name);
                listFile.Items[1].SubItems.Add(selectFile.id.ToString());
                listFile.Items[2].SubItems.Add("0x" + String.Format("{0:X}", selectFile.offset));
                listFile.Items[3].SubItems.Add(selectFile.size.ToString());
                #region Obtener tipo de archivo traducido
                switch (selectFile.formato)
                {
                    case Formato.Paleta:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S20"));
                        break;
                    case Formato.Imagen:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S21"));
                        break;
                    case Formato.Screen:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S22"));
                        break;
                    case Formato.Celdas:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S23"));
                        break;
                    case Formato.Animación:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S24"));
                        break;
                    case Formato.ImagenCompleta:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S25"));
                        break;
                    case Formato.Texto:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S26"));
                        break;
                    case Formato.Video:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S27"));
                        break;
                    case Formato.Sonido:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S28"));
                        break;
                    case Formato.Fuentes:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S29"));
                        break;
                    case Formato.Comprimido:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S2A"));
                        break;
                    case Formato.Desconocido:
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S2B"));
                        break;
                    case Formato.Sistema :
                        listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S31"));
                        break;
                }
                #endregion
                listFile.Items[5].SubItems.Add(selectFile.path);
                btnHex.Enabled = true;
                toolStripOpenAs.Enabled = true;

                if (selectFile.formato != Formato.Desconocido && selectFile.formato != Formato.Sistema)
                    btnSee.Enabled = true;
                else
                    btnSee.Enabled = false;
                if (selectFile.formato == Formato.Comprimido)
                    btnDescomprimir.Enabled = true;
                else
                    btnDescomprimir.Enabled = false;
                if (selectFile.formato == Formato.Sistema)
                    btnImport.Enabled = false;
                else
                    btnImport.Enabled = true;
            }
            else
            {
                Carpeta selectFolder = accion.Select_Folder();

                listFile.Items[0].SubItems.Add(e.Node.Name);
                listFile.Items[1].SubItems.Add("0x" + String.Format("{0:X}", e.Node.Tag));
                listFile.Items[2].SubItems.Add("");
                listFile.Items[3].SubItems.Add("");
                listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S1F"));
                listFile.Items[5].SubItems.Add("");

                btnHex.Enabled = false;
                btnSee.Enabled = false;
                toolStripOpenAs.Enabled = false;
                btnDescomprimir.Enabled = true;
                btnImport.Enabled = false;
            }
        }
        private void btnHex_Click(object sender, EventArgs e)
        {
            Archivo fileSelect = accion.Select_File();
            Thread hex = new Thread(ThreadHexadecimal);
            hex.Start(fileSelect);
        }
        private void ThreadHexadecimal(Object archivo)
        {
            Archivo file = (Archivo)archivo;
            VisorHex hex;

            if (file.offset != 0x0)
                hex = new VisorHex(accion.ROMFile, file.offset, file.size);
            else
                hex = new VisorHex(file.path, 0, file.size);

            hex.Text += " - " + file.name;
            hex.ShowDialog();
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
                panelObj.Controls.Clear();
                Control control = accion.See_File();
                if (control.Size.Height != 0 && control.Size.Width != 0) // Si no sería nulo
                {
                    panelObj.Controls.Add(control);
                    if (btnDesplazar.Text == ">>>>>")
                        btnDesplazar.PerformClick();
                }
            }
                debug.Añadir_Texto(sb.ToString());
                sb.Length = 0;
        }
        private void btnUncompress_Click(object sender, EventArgs e)
        {
            Carpeta uncompress;

            if (accion.IDSelect >= 0x0F000)
            {
                if (MessageBox.Show("¿Estás seguro de que deseas descomprimir TODOS los archivos\n" +
                    "de una carpeta?. Puede tardar bastante...", "Advertencia", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation)
                    == System.Windows.Forms.DialogResult.Cancel)
                    return;

                Thread espera = new System.Threading.Thread(ThreadEspera);
                if (!isMono)
                    espera.Start("S04");

                Recursivo_UncompressFolder(accion.Select_Folder());
                Set_Formato(accion.Root);
                Get_SupportedFiles();
                treeSystem.Nodes.Clear();
                treeSystem.Nodes.Add(Jerarquizar_Nodos(accion.Root));
                treeSystem.Nodes[0].Expand();

                if (!isMono)
                    espera.Abort();
                return;
            }

            uncompress = accion.Extract();
            if (!(uncompress.files is List<Archivo>) && !(uncompress.folders is List<Carpeta>)) // En caso de que falle la extracción
            {
                MessageBox.Show("Hubo un fallo al descomprimir.\n¿Seguro que es un archivo comprimido?");
                return;
            }

            btnSee.Enabled = false;
            btnHex.Enabled = false;
            Set_Formato(accion.Root);
            Get_SupportedFiles();

            TreeNode selected = treeSystem.SelectedNode;
            CarpetaANodo(uncompress, ref selected);

            // Agregamos los nodos al árbol
            TreeNode[] nodos = new TreeNode[selected.Nodes.Count]; selected.Nodes.CopyTo(nodos, 0);
            treeSystem.SelectedNode.Tag = selected.Tag;
            accion.IDSelect = Convert.ToInt32(selected.Tag);
            selected.Nodes.Clear();

            treeSystem.SelectedNode.Nodes.AddRange((TreeNode[])nodos);
            treeSystem.SelectedNode.Expand();

            debug.Añadir_Texto(sb.ToString());
            sb.Length = 0;
        }
        private void Recursivo_UncompressFolder(Carpeta currFolder)
        {
            if (currFolder.folders is List<Carpeta>)
            {
                Carpeta[] carpetas = new Carpeta[currFolder.folders.Count];
                currFolder.folders.CopyTo(carpetas);
                foreach (Carpeta subFolder in carpetas)
                    Recursivo_UncompressFolder(subFolder);
            }

            if (currFolder.files is List<Archivo>)
            {
                Archivo[] archivos = new Archivo[currFolder.files.Count];
                currFolder.files.CopyTo(archivos);
                foreach (Archivo archivo in archivos)
                    if (archivo.formato == Formato.Comprimido)
                        accion.Extract(archivo.id);
            }
        }
        private void btnExtraer_Click(object sender, EventArgs e)
        {
            if (Convert.ToUInt16(treeSystem.SelectedNode.Tag) < 0xF000)
                ExtractFile();
            else
                ExtractFolder();
        }
        private void ExtractFile()
        {
            Archivo fileSelect = accion.Select_File();

            SaveFileDialog o = new SaveFileDialog();
            o.FileName = fileSelect.name;
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (fileSelect.offset != 0x0)
                {
                    BinaryReader br = new BinaryReader(File.OpenRead(accion.ROMFile));
                    br.BaseStream.Position = fileSelect.offset;
                    File.WriteAllBytes(o.FileName, br.ReadBytes((int)fileSelect.size));
                    br.Close();
                }
                else
                    File.Copy(fileSelect.path, o.FileName);
            }
        }
        private void ExtractFolder()
        {
            Carpeta folderSelect = accion.Select_Folder();

            if (!(folderSelect.name is String)) // En caso que sea el resultado de una búsqueda
            {
                folderSelect.files = new List<Archivo>();
                folderSelect.folders = new List<Carpeta>();
                folderSelect.name = treeSystem.SelectedNode.Name;

                for (int i = 0; i < treeSystem.SelectedNode.Nodes.Count; i++)
                {
                    int id = Convert.ToInt32(treeSystem.SelectedNode.Nodes[i].Tag);
                    if (id < 0xF000)
                        folderSelect.files.Add(accion.Search_File(id));
                    else
                    {
                        Carpeta carpeta = new Carpeta();
                        carpeta.files = new List<Archivo>();
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
                Directory.CreateDirectory(o.SelectedPath + '\\' + folderSelect.name);

                Thread espera = new System.Threading.Thread(ThreadEspera);
                if (!isMono)
                    espera.Start("S03");
                RecursivoExtractFolder(folderSelect, o.SelectedPath + '\\' + folderSelect.name);
                if (!isMono)
                    espera.Abort();
            }

        }
        private void RecursivoExtractFolder(Carpeta currFolder, String path)
        {
            if (currFolder.files is List<Archivo>)
                foreach (Archivo archivo in currFolder.files)
                {
                    if (archivo.offset != 0x0)
                    {
                        BinaryReader br = new BinaryReader(File.OpenRead(accion.ROMFile));
                        br.BaseStream.Position = archivo.offset;
                        File.WriteAllBytes(path + '\\' + archivo.name, br.ReadBytes((int)archivo.size));
                        br.Close();
                    }
                    else
                        File.Copy(archivo.path, path + '\\' + archivo.name);
                }



            if (currFolder.folders is List<Carpeta>)
            {
                foreach (Carpeta subFolder in currFolder.folders)
                {
                    Directory.CreateDirectory(path + '\\' + subFolder.name);
                    RecursivoExtractFolder(subFolder, path + '\\' + subFolder.name);
                }
            }
        }
        private void treeSystem_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (accion.IDSelect < 0xF000)   // Comprobación de que la selección no sea un directorio
            {
                accion.Set_Data();
                debug.Añadir_Texto(sb.ToString());
                sb.Length = 0;
            }
        }
        private void Sistema_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
                btnSee.PerformClick();
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
            accion.Delete_PicturesSaved(Formato.Paleta);
        }
        private void borrarTileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            accion.Delete_PicturesSaved(Formato.Imagen);
        }
        private void borrarScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            accion.Delete_PicturesSaved(Formato.Screen);
        }
        private void borrarCeldasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            accion.Delete_PicturesSaved(Formato.Celdas);
        }
        private void borrarAnimaciónToolStripMenuItem_Click(object sender, EventArgs e)
        {
            accion.Delete_PicturesSaved(Formato.Animación);
        }

        private void AbrirComo(Formato formato)
        {
            if (toolStripVentana.Checked)
            {
                Visor visor = new Visor();
                visor.Controls.Add(accion.Set_PicturesSaved(formato));
                visor.Show();
            }
            else
            {
                panelObj.Controls.Clear();
                panelObj.Controls.Add(accion.Set_PicturesSaved(formato));
                if (btnDesplazar.Text == ">>>>>")
                    btnDesplazar.PerformClick();
            }
            debug.Añadir_Texto(sb.ToString());
            sb.Length = 0;
        }
        private void toolAbrirComoItemPaleta_Click(object sender, EventArgs e)
        {
        	AbrirComo(Formato.Paleta);
        }   
        private void toolAbrirComoItemTile_Click(object sender, EventArgs e)
        {
        	AbrirComo(Formato.Imagen);
        }
        private void toolAbrirComoItemScreen_Click(object sender, EventArgs e)
        {
            AbrirComo(Formato.Screen);
        }
        private void s2AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnUncompress_Click(btnDescomprimir, e);
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

            Carpeta resul;

            if (txtSearch.Text == "<Ani>")
                resul = accion.Search_File(Formato.Animación);
            else if (txtSearch.Text == "<Cell>")
                resul = accion.Search_File(Formato.Celdas);
            else if (txtSearch.Text == "<Screen>")
                resul = accion.Search_File(Formato.Screen);
            else if (txtSearch.Text == "<Image>")
                resul = accion.Search_File(Formato.Imagen);
            else if (txtSearch.Text == "<FullImage")
                resul = accion.Search_File(Formato.ImagenCompleta);
            else if (txtSearch.Text == "<Palette>")
                resul = accion.Search_File(Formato.Paleta);
            else if (txtSearch.Text == "<Text>")
                resul = accion.Search_File(Formato.Texto);
            else if (txtSearch.Text == "<Video>")
                resul = accion.Search_File(Formato.Video);
            else if (txtSearch.Text == "<Sound>")
                resul = accion.Search_File(Formato.Sonido);
            else if (txtSearch.Text == "<Font>")
                resul = accion.Search_File(Formato.Fuentes);
            else if (txtSearch.Text == "<Compress>")
                resul = accion.Search_File(Formato.Comprimido);
            else if (txtSearch.Text == "<Unknown>")
                resul = accion.Search_File(Formato.Desconocido);
            else
                resul = accion.Search_File(txtSearch.Text);

            resul.id = (ushort)accion.LastFolderID;
            accion.LastFolderID++;
            if (resul.folders is List<Carpeta>)
            {
                for (int i = 0; i < resul.folders.Count; i++)
                {
                    Carpeta newFolder = resul.folders[i];
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

            #region Preparativos
            // Copia para no modificar el originial
            List<Archivo> origianlFiles = new List<Archivo>();
            origianlFiles.AddRange((Archivo[])accion.Search_Folder("ftc").files.ToArray().Clone());

            // Quitamos archivos especiales que no cuentan en la ROM
            int id = accion.Search_File("fnt.bin").files[0].id;
            accion.Remove_File("fnt.bin", accion.Root);
            accion.Recursivo_BajarID(id, accion.Root);

            id = accion.Search_File("fat.bin").files[0].id;
            accion.Remove_File("fat.bin", accion.Root);
            accion.Recursivo_BajarID(id, accion.Root);

            id = accion.Search_File("arm9.bin").files[0].id;
            accion.Remove_File("arm9.bin", accion.Root);
            accion.Recursivo_BajarID(id, accion.Root);

            id = accion.Search_File("arm7.bin").files[0].id;
            accion.Remove_File("arm7.bin", accion.Root);
            accion.Recursivo_BajarID(id, accion.Root);
            
            id = accion.Search_File("y9.bin").files[0].id;
            accion.Remove_File("y9.bin", accion.Root);
            accion.Recursivo_BajarID(id, accion.Root);

            Carpeta y7 = accion.Search_File("y7.bin");
            if (y7.files.Count > 0)
            {
                id = y7.files[0].id;
                accion.Remove_File("y7.bin", accion.Root);
                accion.Recursivo_BajarID(id, accion.Root);
            }

            // Obtenemos el último ID de archivo sin los especiales
            accion.LastFileID = 0;
            accion.Set_LastFileID(accion.Root);
            accion.LastFolderID = 0;
            accion.Set_LastFolderID(accion.Root);
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
            string header = Path.GetTempFileName();
            Nitro.NDS.EscribirCabecera(header, cabecera, accion.ROMFile);


            // Escribimos los archivos
            string files = Path.GetTempFileName();
            Nitro.NDS.EscribirArchivos(files, accion.ROMFile, accion.Root, (int)romInfo.Cabecera.FATsize / 0x08);
            Console.Write("<br>");
            #endregion
            
            // Obtenemos el nuevo archivo para guardar
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.DefaultExt = ".nds";
            o.Filter = "Nintendo DS ROM (*.nds)|*.nds";
            o.OverwritePrompt = true;
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
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
            }

            // Devolvemos sus valores buenos
            accion.Set_LastFileID(accion.Root);
            accion.Set_LastFolderID(accion.Root);
            accion.LastFileID++; accion.LastFolderID++;
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
        }
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (accion.IDSelect >= 0xF000) 
                return;

            // Se cambian un archivo por otro
            OpenFileDialog o = new OpenFileDialog();
            o.CheckFileExists = true;
            o.CheckPathExists = true;
            o.Multiselect = false;
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Archivo newFile = new Archivo();
                Archivo selectedFile = accion.Select_File();
                newFile.name = selectedFile.name;
                newFile.id = selectedFile.id;
                newFile.offset = 0x00;
                newFile.path = o.FileName;
                newFile.formato = selectedFile.formato;
                newFile.size = (uint)new FileInfo(o.FileName).Length;

                accion.Change_File(accion.IDSelect, newFile, accion.Root);
            }
        }
        private TreeNode[] FilesToNodes(Archivo[] files)
        {
            TreeNode[] nodos = new TreeNode[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                int nImage = accion.ImageFormatFile(files[i].formato);
                string ext = "";
                if (files[i].formato == Formato.Desconocido)
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
