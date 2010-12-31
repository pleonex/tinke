using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LAYTON
{
    public partial class InfoImage : UserControl
    {
        byte[][] datos;         // Array de datos de la array de partes.
        InfoParte[] infoPartes;
        TabPage[] partes;

        public InfoImage()
        {
            InitializeComponent();
        }

        #region Propiedades
        public Ani.Image Informacion
        {
            set
            {
                Nombre = value.name;
                Imagenes = value.imgs;
                Tamano_Imagen = value.length;
                Ancho = value.width;
                Alto = value.height;
                NPartes = value.imgs;
                Partes = value.segmentos;
            }
            get
            {
                Ani.Image temp = new Ani.Image();

                temp.name = Nombre;
                temp.imgs = (ushort)Imagenes;
                temp.length = Tamano_Imagen;
                temp.width = (ushort)Ancho;
                temp.height = (ushort)Alto;
                temp.segmentos = Partes;
                return temp;
            }
        }
        public string Nombre
        {
            set { txtName.Text = value; }
            get { return txtName.Text; }
        }
        public uint Imagenes
        {
            set { txtImgs.Text = value.ToString(); }
            get { return Convert.ToUInt16(txtImgs.Text); }
        }
        public uint Tamano_Imagen
        {
            set { txtTamanoImg.Text = value.ToString(); }
            get { return Convert.ToUInt16(txtTamanoImg.Text); }
        }
        public uint Ancho
        {
            set { txtAncho.Text = value.ToString(); }
            get { return Convert.ToUInt16(txtAncho.Text); }
        }
        public uint Alto
        {
            set { txtAlto.Text = value.ToString(); }
            get { return Convert.ToUInt16(txtAlto.Text); }
        }
        public uint NPartes
        {
            set
            {
                txtNPartes.Text = value.ToString();
                infoPartes = new InfoParte[value];
                partes = new TabPage[value];

                for (int i = 0; i < value; i++)
                {
                    infoPartes[i] = new InfoParte();
                    partes[i] = new TabPage();
                    partes[i].Text = "Parte " + i.ToString();
                    partes[i].Controls.Add(infoPartes[i]);
                }

                while (tabPartes.Controls.Count != 0)
                {
                    tabPartes.Controls.RemoveAt(0);
                }
                tabPartes.Controls.AddRange(partes);
            }
            get { return Convert.ToUInt32(txtNPartes.Text); }
        }
        public Ani.Parte[] Partes
        {
            set
            {
                datos = new byte[infoPartes.Length][];

                for (int i = 0; i < infoPartes.Length; i++)
                {
                    infoPartes[i].Informacion = value[i];
                    datos[i] = value[i].datos;
                }
            }
            get
            {
                Ani.Parte[] temp = new Ani.Parte[infoPartes.Length];

                for (int i = 0; i < infoPartes.Length; i++)
                {
                    temp[i] = infoPartes[i].Informacion;
                    temp[i].datos = datos[i];
                }

                return temp;
            }
        }
        #endregion
    }
}
