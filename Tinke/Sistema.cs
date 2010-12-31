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
            this.Text = "Tinke V " + Application.ProductVersion + " - NDScene";

            // Modo debug donde se muestran los mensajes en otra ventana
            sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            Console.SetOut(tw);

            this.Load += new EventHandler(Sistema_Load);
        }
        void Sistema_Load(object sender, EventArgs e)
        {
            // Iniciamos la lectura del archivo.
            OpenFileDialog o = new OpenFileDialog();
            o.AutoUpgradeEnabled = true;
            o.CheckFileExists = true;
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
            Nitro.Estructuras.Folder root = FNT(o.FileName, romInfo.Cabecera.fileNameTableOffset, romInfo.Cabecera.fileNameTableSize);
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
            sb.Clear();
        }


        private Nitro.Estructuras.Folder FNT(string file, UInt32 offset, UInt32 size)
        {
            Nitro.Estructuras.Folder root = Nitro.FNT.LeerFNT(file, offset);
            accion.Root = root;

            Nitro.Estructuras.File fnt = new Nitro.Estructuras.File();
            fnt.name = "fnt.bin";
            fnt.offset = offset;
            fnt.size = size;
            accion.LastFileID++;
            fnt.id = (ushort)accion.LastFileID;
            accion.LastFileID++;

            if (!(root.folders is List<Nitro.Estructuras.Folder>))
                root.folders = new List<Nitro.Estructuras.Folder>();
            Nitro.Estructuras.Folder ftc = new Nitro.Estructuras.Folder();
            ftc.name = "ftc";
            ftc.id = (ushort)accion.LastFolderID;
            accion.LastFolderID++;
            ftc.files = new List<Nitro.Estructuras.File>();
            ftc.files.Add(fnt);
            root.folders.Add(ftc);

            return root;
        }
        private Nitro.Estructuras.File[] ARMOverlay(string file, UInt32 offset, UInt32 size, bool ARM9)
        {
            return Nitro.Overlay.LeerOverlaysBasico(file, offset, size, ARM9);
        }
        private Nitro.Estructuras.Folder FAT(string file, UInt32 offset, UInt32 size, Nitro.Estructuras.Folder root)
        {
            return Nitro.FAT.LeerFAT(file, offset, size, root);
        }
        private TreeNode Jerarquizar_Nodos(Nitro.Estructuras.Folder root, Nitro.Estructuras.Folder currFolder)
        {
            TreeNode currNode = new TreeNode();

            currNode = new TreeNode(currFolder.name, 0, 0);
            currNode.Tag = currFolder.id;
            currNode.Name = currFolder.name;


            if (currFolder.folders is List<Nitro.Estructuras.Folder>)
                foreach (Nitro.Estructuras.Folder subFolder in currFolder.folders)
                    currNode.Nodes.Add(Jerarquizar_Nodos(root, subFolder));


            if (currFolder.files is List<Nitro.Estructuras.File>)
            {
                foreach (Nitro.Estructuras.File archivo in currFolder.files)
                {
                    int nImage = accion.ImageFormatFile(accion.Get_Formato(archivo.id));
                    TreeNode fileNode = new TreeNode(archivo.name, nImage, nImage);
                    fileNode.Name = archivo.name;
                    fileNode.Tag = archivo.id;
                    currNode.Nodes.Add(fileNode);
                }
            }

            return currNode;
        }

        private void ThreadEspera()
        {
            Espera espera = new Espera("Cargando datos del juego...", false);
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
                listFile.Items[4].SubItems.Add("Directorio");
                listFile.Items[5].SubItems.Add("");

                btnHex.Enabled = false;
                btnSee.Enabled = false;
                btnExtraer.Enabled = false;
                btnDescomprimir.Enabled = false;
            }
            else if (Convert.ToUInt16(e.Node.Tag) < 0xF000)
            {
                Nitro.Estructuras.File selectFile = accion.Select_File();

                listFile.Items[0].SubItems.Add(selectFile.name);
                listFile.Items[1].SubItems.Add(selectFile.id.ToString());
                listFile.Items[2].SubItems.Add("0x" + String.Format("{0:X}", selectFile.offset));
                listFile.Items[3].SubItems.Add(selectFile.size.ToString());
                listFile.Items[4].SubItems.Add(accion.Get_Formato().ToString());
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
                Nitro.Estructuras.Folder selectFolder = accion.Select_Folder();

                listFile.Items[0].SubItems.Add(selectFolder.name);
                listFile.Items[1].SubItems.Add("0x" + String.Format("{0:X}", selectFolder.id));
                listFile.Items[2].SubItems.Add("");
                listFile.Items[3].SubItems.Add("");
                listFile.Items[4].SubItems.Add("Directorio");
                listFile.Items[5].SubItems.Add("");

                btnHex.Enabled = false;
                btnSee.Enabled = false;
                btnExtraer.Enabled = false;
                btnDescomprimir.Enabled = false;
            }
        }
        private void btnHex_Click(object sender, EventArgs e)
        {
            Nitro.Estructuras.File fileSelect = accion.Select_File();
            Thread hex = new Thread(ThreadHexadecimal);
            hex.Start(fileSelect);
        }
        private void ThreadHexadecimal(Object archivo)
        {
            Nitro.Estructuras.File file = (Nitro.Estructuras.File)archivo;
            VisorHex hex;

            if (file.offset != 0x0)
                hex = new VisorHex(accion.ROMFile, file.offset, file.size);
            else
                hex = new VisorHex(file.path, 0, file.size);

            hex.ShowDialog();
        }
        private void BtnSee(object sender, EventArgs e)
        {
            if (toolStripVentana.Checked)
            {
                Visor visor = new Visor();
                visor.Controls.Add(accion.See_File());
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
                sb.Clear();
        }
        private void btnUncompress_Click(object sender, EventArgs e)
        {
            // TODO: Descomprimir todocheck
            accion.Extract();
            treeSystem.Nodes.Clear();
            treeSystem.Nodes.Add(Jerarquizar_Nodos(accion.Root, accion.Root));
            treeSystem.SelectedNode = null;

            debug.Añadir_Texto(sb.ToString());
            sb.Clear();
        }
        private void btnExtraer_Click(object sender, EventArgs e)
        {
            // TODO: Extraer todocheck
            Nitro.Estructuras.File fileSelect = accion.Select_File();

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
                    br.Dispose();
                }
                else
                    File.Copy(fileSelect.path, o.FileName);
            }
        }
        private void btnDeleteChain_Click(object sender, EventArgs e)
        {
            accion.Delete_PicturesSaved();
        }
        private void treeSystem_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (accion.IDSelect < 0xF000)   // Comprobación de que la selección no sea un directorio
            {
                accion.Set_Data();
                debug.Añadir_Texto(sb.ToString());
                sb.Clear();
            }
        }


        private void toolStripInfoRom_Click(object sender, EventArgs e)
        {
            if (!toolStripInfoRom.Checked)
                romInfo.Show();
            else
                romInfo.Hide();
        }
        private void toolStripDebug_Click(object sender, EventArgs e)
        {
            if (!toolStripDebug.Checked)
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
                this.Width = 615;
                if (panelObj.Controls.Count > 0)
                {
                    Visor visor = new Visor();
                    visor.Controls.Add(panelObj.Controls[0]);
                    visor.Show();
                }
            }
            else
                this.Width = 650;

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
    }
}
