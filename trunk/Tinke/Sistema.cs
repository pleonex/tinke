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

        public Sistema()
        {
            InitializeComponent();
            this.Location = new Point(10, 10);
            this.Text = "Tinke V " + Application.ProductVersion + " - NDScene by pleoNeX";

            // Modo debug donde se muestran los mensajes en otra ventana
            sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            tw.NewLine = "<br>";
            Console.SetOut(tw);

            #region Idioma
            if (!File.Exists(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml"))
            {
                File.WriteAllText(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                 "\n<Tinke>\n  <Options>\n    <Language>Español</Language>\n  </Options>\n</Tinke>",
                                 Encoding.UTF8);
            }

            // TODO: controlar excepción de fallo al leer xml
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
            espera.Start();
            #region Lectura del archivo
            romInfo = new RomInfo(o.FileName);
            accion = new Acciones(o.FileName, new String(romInfo.Cabecera.gameCode));
            // Obtenemos el sistema de archivos
            Carpeta root = FNT(o.FileName, romInfo.Cabecera.fileNameTableOffset, romInfo.Cabecera.fileNameTableSize);
            // Añadimos los Overlays
            root.folders[root.folders.Count - 1].files.AddRange(
                ARMOverlay(o.FileName, romInfo.Cabecera.ARM9overlayOffset, romInfo.Cabecera.ARM9overlaySize, true)
                );
            root.folders[root.folders.Count - 1].files.AddRange(
                ARMOverlay(o.FileName, romInfo.Cabecera.ARM7overlayOffset, romInfo.Cabecera.ARM7overlaySize, false)
                );
            // Añadimos los offset a cada archivo
            root = FAT(o.FileName, romInfo.Cabecera.FAToffset, romInfo.Cabecera.FATsize, root);

            accion.Root = root;
            treeSystem.Nodes.Add(Jerarquizar_Nodos(root, root)); // Mostramos el árbol
            #endregion
            espera.Abort();

            debug = new Debug();
            debug.Show();

            o.Dispose();
            debug.Añadir_Texto(sb.ToString());
            sb.Remove(0, sb.Length - 1);

            this.Activate();
            debug.Activate();
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
        }
        private void ToolStripLang_Click(Object sender, EventArgs e)
        {
            string idioma = ((ToolStripMenuItem)sender).Text;
            System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");
            xml.Element("Options").Element("Language").Value = idioma;
            xml.Save(Application.StartupPath + Path.DirectorySeparatorChar + "Tinke.xml");
            // TODO: quitar esto
            LeerIdioma();
            romInfo.LeerIdioma();
            debug.LeerIdioma();

            MessageBox.Show(Tools.Helper.ObtenerTraduccion("Messages", "S07"));
        }

        private Carpeta FNT(string file, UInt32 offset, UInt32 size)
        {
            Carpeta root = Nitro.FNT.LeerFNT(file, offset);
            accion.Root = root;

            Archivo fnt = new Archivo();
            fnt.name = "fnt.bin";
            fnt.offset = offset;
            fnt.size = size;
            accion.LastFileID++;
            fnt.id = (ushort)accion.LastFileID;
            accion.LastFileID++;

            if (!(root.folders is List<Carpeta>))
                root.folders = new List<Carpeta>();
            Carpeta ftc = new Carpeta();
            ftc.name = "ftc";
            ftc.id = (ushort)accion.LastFolderID;
            accion.LastFolderID++;
            ftc.files = new List<Archivo>();
            ftc.files.Add(fnt);
            root.folders.Add(ftc);

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
        private TreeNode Jerarquizar_Nodos(Carpeta root, Carpeta currFolder)
        {
            TreeNode currNode = new TreeNode();

            currNode = new TreeNode(currFolder.name, 0, 0);
            currNode.Tag = currFolder.id;
            currNode.Name = currFolder.name;


            if (currFolder.folders is List<Carpeta>)
                foreach (Carpeta subFolder in currFolder.folders)
                    currNode.Nodes.Add(Jerarquizar_Nodos(root, subFolder));


            if (currFolder.files is List<Archivo>)
            {
                foreach (Archivo archivo in currFolder.files)
                {
                    Formato fmt = accion.Get_Formato(archivo.id);
                    int nImage = accion.ImageFormatFile(fmt);
                    string ext = "";
                    if (fmt == Formato.Desconocido)
                    {
                        ext = accion.Get_MagicIDS(archivo.id);
                        if (ext != "")
                            ext = " [" + ext + ']'; // Previene extensiones vacías
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


            if (carpeta.folders is List<Carpeta>)
            {
                foreach (Carpeta subFolder in carpeta.folders)
                {
                    TreeNode newNodo = new TreeNode();
                   CarpetaANodo(subFolder, ref newNodo);
                    nodo.Nodes.Add(newNodo);
                }
            }


            if (carpeta.files is List<Archivo>)
            {
                foreach (Archivo archivo in carpeta.files)
                {
                    Formato fmt = accion.Get_Formato(archivo.id);
                    int nImage = accion.ImageFormatFile(fmt);
                    string ext = "";
                    if (fmt == Formato.Desconocido)
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

        private void ThreadEspera()
        {
            Espera espera = new Espera("S02", false);
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
                btnExtraer.Enabled = false;
                btnDescomprimir.Enabled = false;
            }
            else if (Convert.ToUInt16(e.Node.Tag) < 0xF000)
            {
                Archivo selectFile = accion.Select_File();

                listFile.Items[0].SubItems.Add(selectFile.name);
                listFile.Items[1].SubItems.Add(selectFile.id.ToString());
                listFile.Items[2].SubItems.Add("0x" + String.Format("{0:X}", selectFile.offset));
                listFile.Items[3].SubItems.Add(selectFile.size.ToString());
                #region Obtener tipo de archivo traducido
                switch (accion.Get_Formato())
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
                }
                #endregion
                listFile.Items[5].SubItems.Add(selectFile.path);
                btnHex.Enabled = true;
                btnExtraer.Enabled = true;

                PluginInterface.Formato tipo = accion.Get_Formato();
                if (tipo != PluginInterface.Formato.Desconocido)
                {
                    btnSee.Enabled = true;
                    btnDescomprimir.Enabled = false;
                }
                else
                {
                    btnSee.Enabled = false;
                    btnDescomprimir.Enabled = false;
                }
                if (tipo == PluginInterface.Formato.Comprimido)
                    btnDescomprimir.Enabled = true;

            }
            else
            {
                Carpeta selectFolder = accion.Select_Folder();

                listFile.Items[0].SubItems.Add(selectFolder.name);
                listFile.Items[1].SubItems.Add("0x" + String.Format("{0:X}", selectFolder.id));
                listFile.Items[2].SubItems.Add("");
                listFile.Items[3].SubItems.Add("");
                listFile.Items[4].SubItems.Add(Tools.Helper.ObtenerTraduccion("Sistema", "S1F"));
                listFile.Items[5].SubItems.Add("");

                btnHex.Enabled = false;
                btnSee.Enabled = false;
                btnExtraer.Enabled = false;
                btnDescomprimir.Enabled = false;
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
                panelObj.Controls.Add(accion.See_File());
                if (btnDesplazar.Text == ">>>>>")
                    btnDesplazar.PerformClick();
            }
                debug.Añadir_Texto(sb.ToString());
                            sb.Remove(0, sb.Length - 1);;
        }
        private void btnUncompress_Click(object sender, EventArgs e)
        {
            Carpeta descomprimidos = accion.Extract();
            TreeNode selected = treeSystem.SelectedNode;
            CarpetaANodo(descomprimidos, ref selected);

            TreeNode[] nodos = new TreeNode[selected.Nodes.Count]; selected.Nodes.CopyTo(nodos, 0);
            treeSystem.SelectedNode.Tag = selected.Tag;
            accion.IDSelect = Convert.ToInt32(selected.Tag);
            selected.Nodes.Clear();
            treeSystem.SelectedNode.Nodes.AddRange((TreeNode[])nodos);

            btnDescomprimir.Enabled = false;
            btnSee.Enabled = false;
            btnExtraer.Enabled = false;
            btnHex.Enabled = false;

            debug.Añadir_Texto(sb.ToString());
                        sb.Remove(0, sb.Length - 1);;
        }
        private void btnExtraer_Click(object sender, EventArgs e)
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
        private void treeSystem_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (accion.IDSelect < 0xF000)   // Comprobación de que la selección no sea un directorio
            {
                accion.Set_Data();
                debug.Añadir_Texto(sb.ToString());
                            sb.Remove(0, sb.Length - 1);;
            }
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
                        sb.Remove(0, sb.Length - 1);;
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

        private void linkAboutBox_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkAboutBox.LinkVisited = false;
            Autores ven = new Autores();
            ven.ShowDialog();
        }


    }
}
