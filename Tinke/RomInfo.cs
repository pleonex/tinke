using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Tinke
{
    public partial class RomInfo : Form
    {
        Nitro.Estructuras.ROMHeader cabecera;
        string[] titulos;

        public RomInfo(string archivo)
        {
            InitializeComponent();
            this.Location = new Point(670, 10);

            try 
            {
                Nitro.Estructuras.ROMHeader cabecera = Nitro.NDS.LeerCabecera(archivo);
                Nitro.Estructuras.Banner banner = Nitro.NDS.LeerBanner(archivo, cabecera.bannerOffset);
                Mostrar_Informacion(cabecera, banner);
            }
            catch
            {
                MessageBox.Show("No se puedo leer el archivo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.WriteLine("No se pudo abrir la ROM: " + archivo);
                return;
            }
        }

        private void Mostrar_Informacion(Nitro.Estructuras.ROMHeader cabecera, Nitro.Estructuras.Banner banner)
        {
            this.cabecera = cabecera;

            #region Muestra la información de la cabecera
            listInfo.Items[0].SubItems.Add(new String(cabecera.gameTitle));
            listInfo.Items[1].SubItems.Add(new string(cabecera.gameCode));
            listInfo.Items[2].SubItems.Add(Nitro.NDS.CodeToString(Nitro.Makercode.Nintendo.GetType(), cabecera.makerCode));
            listInfo.Items[3].SubItems.Add(Nitro.NDS.CodeToString(Nitro.Unitcode.NintendoDS.GetType(), cabecera.unitCode));
            listInfo.Items[4].SubItems.Add(Convert.ToString(cabecera.encryptionSeed));
            listInfo.Items[5].SubItems.Add((cabecera.tamaño / 8388608).ToString() + " MB");
            listInfo.Items[6].SubItems.Add(Tools.Helper.BytesToHexString(cabecera.reserved));
            listInfo.Items[7].SubItems.Add(Convert.ToString(cabecera.ROMversion));
            listInfo.Items[8].SubItems.Add(Convert.ToString(cabecera.internalFlags));
            listInfo.Items[9].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9romOffset));
            listInfo.Items[10].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9entryAddress));
            listInfo.Items[11].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9ramAddress));
            listInfo.Items[12].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9size) + " bytes");
            listInfo.Items[13].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7romOffset));
            listInfo.Items[14].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7entryAddress));
            listInfo.Items[15].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7ramAddress));
            listInfo.Items[16].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7size) + " bytes");
            listInfo.Items[17].SubItems.Add("0x" + String.Format("{0:X}", cabecera.fileNameTableOffset));
            listInfo.Items[18].SubItems.Add("0x" + String.Format("{0:X}", cabecera.fileNameTableSize) + " bytes");
            listInfo.Items[19].SubItems.Add("0x" + String.Format("{0:X}", cabecera.FAToffset));
            listInfo.Items[20].SubItems.Add("0x" + String.Format("{0:X}", cabecera.FATsize) + " bytes");
            listInfo.Items[21].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9overlayOffset));
            listInfo.Items[22].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9overlaySize) + " bytes");
            listInfo.Items[23].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7overlayOffset));
            listInfo.Items[24].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7overlaySize) + " bytes");
            listInfo.Items[25].SubItems.Add(Convert.ToString(cabecera.flagsRead, 2));
            listInfo.Items[26].SubItems.Add(Convert.ToString(cabecera.flagsInit, 2));
            listInfo.Items[27].SubItems.Add("0x" + String.Format("{0:X}", cabecera.bannerOffset));
            listInfo.Items[28].SubItems.Add(cabecera.secureCRC16.ToString() + " (" + Convert.ToString(cabecera.secureCRC) + ")");
            listInfo.Items[29].SubItems.Add(cabecera.ROMtimeout.ToString());
            listInfo.Items[30].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM9autoload));
            listInfo.Items[31].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ARM7autoload));
            listInfo.Items[32].SubItems.Add(cabecera.secureDisable.ToString());
            listInfo.Items[33].SubItems.Add("0x" + String.Format("{0:X}", cabecera.ROMsize) + " bytes");
            listInfo.Items[34].SubItems.Add("0x" + String.Format("{0:X}", cabecera.headerSize) + " bytes");
            listInfo.Items[35].SubItems.Add(Tools.Helper.BytesToHexString(cabecera.reserved2));
            listInfo.Items[36].SubItems.Add(cabecera.logoCRC16.ToString() + " (" + Convert.ToString(cabecera.logoCRC) + ")");
            listInfo.Items[37].SubItems.Add(cabecera.headerCRC16.ToString() + " (" + Convert.ToString(cabecera.headerCRC) + ")");
            listInfo.Items[38].SubItems.Add("0x" + String.Format("{0:X}", cabecera.debug_romOffset));
            listInfo.Items[39].SubItems.Add("0x" + String.Format("{0:X}", cabecera.debug_size) + " bytes");
            listInfo.Items[40].SubItems.Add("0x" + String.Format("{0:X}", cabecera.debug_ramAddress));
            listInfo.Items[41].SubItems.Add("0x" + String.Format("{0:X}", cabecera.reserved3));
            #endregion
            #region Muestra la información del banner
            picIcon.BorderStyle = BorderStyle.None;
            picIcon.Image = Nitro.NDS.IconoToBitmap(banner.tileData, banner.palette);

            txtBannerVer.Text = banner.version.ToString();
            txtBannerCRC.Text = String.Format("{0:X}", banner.CRC16) + " (" + (banner.checkCRC ? "OK)" : "Inválido)");
            txtBannerReserved.Text = Tools.Helper.BytesToHexString(banner.reserved);

            titulos = new string[] { banner.japaneseTitle, banner.englishTitle, banner.frenchTitle, banner.germanTitle, banner.italianTitle, banner.spanishTitle };
            txtBannerTitle.Text = titulos[0];
            comboBannerLang.SelectedIndex = 0;
            #endregion
        }

        public Nitro.Estructuras.ROMHeader Cabecera
        {
            get { return cabecera; }
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
                    txtBannerTitle.Text = titulos[0];
                    break;
                case 1:
                    txtBannerTitle.Text = titulos[1];
                    break;
                case 2:
                    txtBannerTitle.Text = titulos[2];
                    break;
                case 3:
                    txtBannerTitle.Text = titulos[3];
                    break;
                case 4:
                    txtBannerTitle.Text = titulos[4];
                    break;
                case 5:
                    txtBannerTitle.Text = titulos[5];
                    break;
            }
        }
    }
}
