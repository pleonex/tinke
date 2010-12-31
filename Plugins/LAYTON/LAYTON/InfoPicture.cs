using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LAYTON
{
    public partial class InfoPicture : UserControl
    {
        #region Variables
        InfoImage[] infoImag;
        TabPage[] tabImag;
        #endregion

        // Constructor
        public InfoPicture()
        {
            InitializeComponent();
        }
        public InfoPicture(Ani.Todo info)
        {
            InitializeComponent();
            Informacion = info;
        }

        public string Tipo
        {
            set { txtTipo.Text = value; }
            get { return txtTipo.Text; }
        }
        public ushort Numero_Imagen
        {
            set 
            { 
                txtNImgs.Text = value.ToString();
                infoImag = new InfoImage[value];
                tabImag = new TabPage[value];

                for (int i = 0; i < value; i++)
                {
                    infoImag[i] = new InfoImage();
                    tabImag[i] = new TabPage();
                    tabImag[i].Text = "Imagen " + i.ToString();
                    tabImag[i].Controls.Add(infoImag[i]);
                }

                while (tabImags.Controls.Count != 0) // Elimina datos de archivos anteriores
                {
                    tabImags.Controls.RemoveAt(0);
                }
                tabImags.Controls.AddRange(tabImag);
            }
            get { return Convert.ToUInt16(txtNImgs.Text); }
        }
        public Ani.Todo Informacion
        {
            set
            {
                Numero_Imagen = value.imgs;
                Tipo = value.imagenes[0].tipo == ColorDepth.Depth4Bit ? "4 bpp" : "8 bpp";
                for (int i = 0; i < value.imgs; i++)
                    infoImag[i].Informacion = value.imagenes[i];
            }
        }
        public int Imagen_Seleccionada
        {
            set
            {
                tabImags.SelectedIndex = value;
            }
        }
    }
}
