using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Tinke
{
    public partial class Form1 : Form
    {
        Acciones accion;
        StringBuilder sb;
        string file;
        string[] titles;
        DatosHex hexadecimal;
        List<String> FoldersToDelete;


        public Form1()
        {      
            InitializeComponent();
            this.Text = "Tinke V " + Application.ProductVersion + " - NDScene" + new string(' ', 150) +"by pleoNeX";
            listInfo.Enabled = false;

            sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            Console.SetOut(tw);
            titles = new string[6];
            hexadecimal = new DatosHex();
            hexadecimal.Dock = DockStyle.Fill;
            tabControl1.TabPages[3].Controls.Add(hexadecimal);
            FoldersToDelete = new List<string>();
        }
        public Form1(string file, int id)
        {
            InitializeComponent();
            this.Text = "Tinke V " + Application.ProductVersion + " - NDScene" + new string(' ', 150) + "by pleoNeX";
            listInfo.Enabled = false;

            sb = new StringBuilder();
            TextWriter tw = new StringWriter(sb);
            Console.SetOut(tw);
            titles = new string[6];
            hexadecimal = new DatosHex();
            hexadecimal.Dock = DockStyle.Fill;
            tabControl1.TabPages[3].Controls.Add(hexadecimal);
            FoldersToDelete = new List<string>();
            
            //DEBUG:
            this.file = file;
            HeaderROM(file);
            accion.IDSelect = id;
            tabControl1.TabPages[2].Controls.Add(accion.See_File());
            tabControl1.SelectedTab = tabControl1.TabPages[2];
        }


        private void HeaderROM(string file)
        {
            Nitro.Estructuras.ROMHeader nds;
            try { nds = Nitro.NDS.LeerCabecera(file); }
            catch
            {
                MessageBox.Show("No se puedo leer el archivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("Error al intentar leer el archivo: " + file);
                return;
            }

            listInfo.Items[0].SubItems.Add(new String(nds.gameTitle));
            listInfo.Items[1].SubItems.Add(new string(nds.gameCode));
            listInfo.Items[2].SubItems.Add(Nitro.NDS.CodeToString(Nitro.Makercode.Nintendo.GetType(), nds.makerCode));
            listInfo.Items[3].SubItems.Add(Nitro.NDS.CodeToString(Nitro.Unitcode.NintendoDS.GetType(), nds.unitCode));
            listInfo.Items[4].SubItems.Add(Convert.ToString(nds.encryptionSeed));
            listInfo.Items[5].SubItems.Add((nds.tamaño / 8388608).ToString() + " MB");
            listInfo.Items[6].SubItems.Add(Tools.Helper.BytesToHexString(nds.reserved));
            listInfo.Items[7].SubItems.Add(Convert.ToString(nds.ROMversion));
            listInfo.Items[8].SubItems.Add(Convert.ToString(nds.internalFlags));
            listInfo.Items[9].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM9romOffset));
            listInfo.Items[10].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM9entryAddress));
            listInfo.Items[11].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM9ramAddress));
            listInfo.Items[12].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM9size) + " bytes");
            listInfo.Items[13].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM7romOffset));
            listInfo.Items[14].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM7entryAddress));
            listInfo.Items[15].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM7ramAddress));
            listInfo.Items[16].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM7size) + " bytes");
            listInfo.Items[17].SubItems.Add("0x" + String.Format("{0:X}", nds.fileNameTableOffset));
            listInfo.Items[18].SubItems.Add("0x" + String.Format("{0:X}", nds.fileNameTableSize) + " bytes");
            listInfo.Items[19].SubItems.Add("0x" + String.Format("{0:X}", nds.FAToffset));
            listInfo.Items[20].SubItems.Add("0x" + String.Format("{0:X}", nds.FATsize) + " bytes");
            listInfo.Items[21].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM9overlayOffset));
            listInfo.Items[22].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM9overlaySize) + " bytes");
            listInfo.Items[23].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM7overlayOffset));
            listInfo.Items[24].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM7overlaySize) + " bytes");
            listInfo.Items[25].SubItems.Add(Convert.ToString(nds.flagsRead, 2));
            listInfo.Items[26].SubItems.Add(Convert.ToString(nds.flagsInit, 2));
            listInfo.Items[27].SubItems.Add("0x" + String.Format("{0:X}", nds.bannerOffset));
            listInfo.Items[28].SubItems.Add(nds.secureCRC16.ToString() + " (" + Convert.ToString(nds.secureCRC) + ")");
            listInfo.Items[29].SubItems.Add(nds.ROMtimeout.ToString());
            listInfo.Items[30].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM9autoload));
            listInfo.Items[31].SubItems.Add("0x" + String.Format("{0:X}", nds.ARM7autoload));
            listInfo.Items[32].SubItems.Add(nds.secureDisable.ToString());
            listInfo.Items[33].SubItems.Add("0x" + String.Format("{0:X}", nds.ROMsize) + " bytes");
            listInfo.Items[34].SubItems.Add("0x" + String.Format("{0:X}", nds.headerSize) + " bytes");
            listInfo.Items[35].SubItems.Add(Tools.Helper.BytesToHexString(nds.reserved2));
            listInfo.Items[36].SubItems.Add(nds.logoCRC16.ToString() + " (" + Convert.ToString(nds.logoCRC) + ")");
            listInfo.Items[37].SubItems.Add(nds.headerCRC16.ToString() + " (" + Convert.ToString(nds.headerCRC) + ")");

            accion = new Acciones(file, new String(nds.gameCode));
            BannerROM(file, nds.bannerOffset);
            Nitro.Estructuras.Folder root = FNT(file, nds.fileNameTableOffset, nds.fileNameTableSize);
            root.folders[root.folders.Count - 1].files.AddRange(ARMOverlay(file, nds.ARM9overlayOffset, nds.ARM9overlaySize, true));
            root.folders[root.folders.Count - 1].files.AddRange(ARMOverlay(file, nds.ARM7overlayOffset, nds.ARM7overlaySize, false));
            root = FAT(file, nds.FAToffset, nds.FATsize, root);

            accion.Root = root;
            treeSystem.Nodes.Add(Jerarquizar_Nodos(root, root));
        }
        private void BannerROM(string file, UInt32 offset)
        {
            Nitro.Estructuras.Banner bn = Nitro.NDS.LeerBanner(file, offset);

            picIcon.BorderStyle = BorderStyle.None;
            picIcon.Image = Nitro.NDS.IconoToBitmap(bn.tileData, bn.palette);
            
            txtBannerVer.Text = bn.version.ToString();
            txtBannerCRC.Text = String.Format("{0:X}", bn.CRC16) + " (" + (bn.checkCRC ? "OK)" : "Invalid)");
            txtBannerReserved.Text = Tools.Helper.BytesToHexString(bn.reserved);
            
            titles = new string[] { bn.japaneseTitle, bn.englishTitle, bn.frenchTitle, bn.germanTitle, bn.italianTitle, bn.spanishTitle };
            txtBannerTitle.Text = bn.japaneseTitle;
            comboBannerLang.SelectedIndex = 0;

            
            textURL.BackColor = Color.LightGreen;
        }

        private Nitro.Estructuras.Folder FNT(string file, UInt32 offset, UInt32 size)
        {
            Nitro.Estructuras.Folder root = Nitro.FNT.LeerFNT(file, offset);
            accion.Root = root;

            Nitro.Estructuras.File fnt = new Nitro.Estructuras.File();
            fnt.name = "fnt.bin";
            fnt.offset = offset;
            fnt.size = size;
            //fnt.id = (ushort)accion.LastFileID;
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
                    int nImage = Tipos.ImageFormatFile(accion.Formato(archivo.id));
                    TreeNode fileNode = new TreeNode(archivo.name, nImage, nImage);
                    fileNode.Name = archivo.name;
                    fileNode.Tag = archivo.id;
                    currNode.Nodes.Add(fileNode);
                }
            }

            return currNode;
        }
        private Nitro.Estructuras.File[] ARMOverlay(string file, UInt32 offset, UInt32 size, bool ARM9)
        {
            return Nitro.Overlay.LeerOverlaysBasico(file, offset, size, ARM9);
        }
        private Nitro.Estructuras.Folder FAT(string file, UInt32 offset, UInt32 size, Nitro.Estructuras.Folder root)
        {
            return Nitro.FAT.LeerFAT(file, offset, size, root);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.AutoUpgradeEnabled = true;
            o.CheckFileExists = true;
            o.DefaultExt = ".nds";
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Limpiar posible información anterior
                if (listInfo.Items[0].SubItems.Count == 3)
                    for (int i = 0; i < listInfo.Items.Count; i++)
                        listInfo.Items[i].SubItems.RemoveAt(2);
                txtBannerCRC.Text = "";
                txtBannerReserved.Text = "";
                txtBannerTitle.Text = "";
                txtBannerVer.Text = "";
                picIcon.Image = null;
                picIcon.BorderStyle = BorderStyle.FixedSingle;
                treeSystem.Nodes.Clear();
                tabControl1.TabPages[2].Controls.Clear();
                hexadecimal.Clear();

                file = o.FileName;
                textURL.Text = o.FileName;
                o.Dispose();

                textURL.BackColor = Color.Red;
                listInfo.Enabled = true;

                System.Threading.Thread espera = new System.Threading.Thread(ThreadEspera);
                espera.Start();

                HeaderROM(file);

                espera.Abort();
            }

            output.Items.AddRange(sb.ToString().Split("\n".ToCharArray()));
            sb.Clear();
        }
        private void ThreadEspera()
        {
            Espera espera = new Espera("Cargando sistema de archivos...", false);
            espera.ShowDialog();
        }
        private void btnBannerGuardar_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.AddExtension = true;
            o.AutoUpgradeEnabled = true;
            o.CheckPathExists = true;
            o.DefaultExt = ".bmp";
            o.OverwritePrompt = true;
            o.Filter = "Imagen Bitmap (*.bmp)|*.bmp";
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                picIcon.Image.Save(o.FileName);
        }
        private void comboBannerLang_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBannerLang.SelectedIndex)
            {
                case 0:
                    txtBannerTitle.Text = titles[0];
                    break;
                case 1:
                    txtBannerTitle.Text = titles[1];
                    break;
                case 2:
                    txtBannerTitle.Text = titles[2];
                    break;
                case 3:
                    txtBannerTitle.Text = titles[3];
                    break;
                case 4:
                    txtBannerTitle.Text = titles[4];
                    break;
                case 5:
                    txtBannerTitle.Text = titles[5];
                    break;
            }
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            textURL.Width = this.Width - 115;
            btnOpen.Location = new Point(this.Width - 103, 5);
            listInfo.Height = this.Height - 190;
            listInfo.Width = this.Width - 304;
            treeSystem.Height = this.Height - 190;
            treeSystem.Width = this.Width - 304;
            listFile.Location = new Point(this.Width - 240, 0);
            groupBanner.Location = new Point(this.Width - 295, 42);
            groupBanner.Height = this.Height - 190;

            if (this.Width < 1089)
                this.Text = "Tinke V " + Application.ProductVersion + " - NDScene" + new string(' ', (int)((this.Width - 842) / (3.5)) + 150) + "by pleoNeX";
            else
                this.Text = "Tinke V " + Application.ProductVersion + " - NDScene" + new string(' ', 220) + "by pleoNeX";
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (string folder in FoldersToDelete)
                Directory.Delete(folder, true);
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

                btnHex.Enabled = false;
                btnSee.Enabled = false;
                btnUncompress.Enabled = false;
                btnExtraer.Enabled = false;
                hexadecimal.Clear();
            }
            else if (Convert.ToUInt16(e.Node.Tag) < 0xF000)
            {
                Nitro.Estructuras.File selectFile = accion.Select_File();

                listFile.Items[0].SubItems.Add(selectFile.name);
                listFile.Items[1].SubItems.Add(selectFile.id.ToString());
                listFile.Items[2].SubItems.Add("0x" + String.Format("{0:X}", selectFile.offset));
                listFile.Items[3].SubItems.Add(selectFile.size.ToString());
                listFile.Items[4].SubItems.Add(accion.Formato().ToString());
                btnHex.Enabled = true;
                btnExtraer.Enabled = true;

                Tipos.Role tipo = accion.Formato();
                if (tipo != Tipos.Role.Desconocido && !Tipos.EstaComprimido(tipo))
                    btnSee.Enabled = true;
                else
                    btnSee.Enabled = false;
                if (!Tipos.EstaComprimido(tipo))
                    btnUncompress.Enabled = false;
                else
                    btnUncompress.Enabled = true;
            }
            else
            {
                Nitro.Estructuras.Folder selectFolder = accion.Select_Folder();

                listFile.Items[0].SubItems.Add(selectFolder.name);
                listFile.Items[1].SubItems.Add("0x" + String.Format("{0:X}", selectFolder.id));
                listFile.Items[2].SubItems.Add("");
                listFile.Items[3].SubItems.Add("");
                listFile.Items[4].SubItems.Add("Directorio");

                btnHex.Enabled = false;
                btnSee.Enabled = false;
                btnUncompress.Enabled = false;
                btnExtraer.Enabled = false;
                hexadecimal.Clear();
            }
        }
        private void btnHex_Click(object sender, EventArgs e)
        {
            Nitro.Estructuras.File fileSelect = accion.Select_File();
            if (fileSelect.offset != 0x0)
                hexadecimal.LeerFile(file, fileSelect.offset, fileSelect.size);
            else
                hexadecimal.LeerFile(fileSelect.path, 0, fileSelect.size);
            tabControl1.SelectedTab = tabControl1.TabPages[3];
        }
        private void BtnSee(object sender, EventArgs e)
        {
            tabControl1.TabPages[2].Controls.Clear();
            try
            {
                tabControl1.TabPages[2].Controls.Add(accion.See_File());
                tabControl1.SelectedTab = tabControl1.TabPages[2];
            }
            catch
            {
                MessageBox.Show("No se pudo visualizar este fichero. Formato erróneo");
                output.Items.Add("No se pudo visualizar este fichero. Formato erróneo");
            }

        }
        private void btnUncompress_Click(object sender, EventArgs e)
        {
            // Se crea una carpeta temporal donde almacenar los archivos de salida.
            int n = 0;
            string[] subFolders = Directory.GetDirectories(Application.StartupPath);
            for (; ; n++)
                if (!subFolders.Contains<string>(Application.StartupPath + "\\Temp" + n))
                    break;

            Directory.CreateDirectory(Application.StartupPath + "\\Temp" + n);
            FoldersToDelete.Add(Application.StartupPath + "\\Temp" + n);

            // Guardamos el archivo para descomprimir fuera del sistema de ROM
            string tempFile = Application.StartupPath + "\\temp.dat";               
            Nitro.Estructuras.File selectFile = accion.Select_File();
            BinaryReader br;
            if (selectFile.offset != 0x0)
            {
                br = new BinaryReader(File.OpenRead(file));
                br.BaseStream.Position = selectFile.offset;
            }
            else
                br = new BinaryReader(File.OpenRead(selectFile.path));
            
            BinaryWriter bw = new BinaryWriter(new FileStream(tempFile, FileMode.Create));
            bw.Write(br.ReadBytes((int)selectFile.size));
            bw.Flush();
            bw.Close();
            bw.Dispose();
            br.Close();
            br.Dispose();

            // Determinado el tipo de compresión y descomprimimos
            Tipos.Role tipo = accion.Formato();

            if (tipo == Tipos.Role.Comprimido_NARC)
                Compresion.NARC.Descomprimir(tempFile, Application.StartupPath + "\\Temp" + n);
            else if (tipo == Tipos.Role.Comprimido_LZ77 || tipo == Tipos.Role.Comprimido_Huffman)
                Compresion.Basico.Decompress(tempFile, Application.StartupPath + "\\Temp" + n);
            List<Nitro.Estructuras.File> files = new List<Nitro.Estructuras.File>();

            // Se añaden los archivos descomprimidos al árbol de archivos.
            foreach (string file in Directory.GetFiles(Application.StartupPath + "\\Temp" + n))
            {
                Nitro.Estructuras.File currFile = new Nitro.Estructuras.File();
                currFile.name = new FileInfo(file).Name;
                currFile.path = file;
                currFile.size = (uint)new FileInfo(file).Length;
                files.Add(currFile);
            }

            accion.Add_Files(files);
        
            treeSystem.Nodes.Clear();
            treeSystem.Nodes.Add(Jerarquizar_Nodos(accion.Root, accion.Root));
        }
        private void btnExtraer_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            if (o.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Nitro.Estructuras.File fileSelect = accion.Select_File();
                if (fileSelect.offset != 0x0)
                {
                    BinaryReader br = new BinaryReader(File.OpenRead(file));
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
            try     // No sé cuando pero hay veces en el que con un ID de archivo está seleccionada una carpeta y salta una excepción.
            {
                if (accion.IDSelect < 0xF000)   // Comprobación de que la selección no sea un directorio
                {
                    Tipos.Role tipo = accion.Formato();
                    if (tipo == Tipos.Role.Paleta || tipo == Tipos.Role.Imagen || tipo == Tipos.Role.Screen)
                        accion.Set_Data(accion.Select_File());
                    
                    // Incluye información de DEBUG.
                    output.Items.AddRange(sb.ToString().Split("\n".ToCharArray()));
                    sb.Clear();
                }
            }
            catch
            {
                output.Items.Add("Excepción en treeSystem_MouseDoubleClick: id de archivo seleccionado " + accion.IDSelect.ToString());
            }
        }

    }
}
