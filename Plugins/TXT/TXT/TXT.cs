using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using PluginInterface;

namespace TXT
{
    public class TXT : IPlugin
    {
        IPluginHost pluginHost;

        public void Inicializar(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public Formato Get_Formato(string nombre, byte[] magic)
        {
            nombre = nombre.ToUpper();

            if (nombre.EndsWith(".TXT") || nombre.EndsWith(".SADL") || nombre.EndsWith(".XML"))
                return Formato.Texto;

            return Formato.Desconocido;
        }


        public void Leer(string archivo)
        {
            Console.WriteLine("Este archivo no contiene información para guardar.");
        }

        public Control Show_Info(string archivo)
        {
            TextBox txtBox = new TextBox();
            txtBox.ReadOnly = true;
            txtBox.HideSelection = true;
            txtBox.Dock = DockStyle.Fill;
            txtBox.Multiline = true;
            txtBox.ScrollBars = ScrollBars.Both;

            string txt = "";
            BinaryReader br = new BinaryReader(File.OpenRead(archivo));

            txt = new String(br.ReadChars((int)br.BaseStream.Length));

            #region Convierte caracteres especiales
            txt = txt.Replace("\x0A", "\r\n");
            txt = txt.Replace("<`a>", "à");
            txt = txt.Replace("<'a>", "á");
            txt = txt.Replace("<:a>", "ä");
            txt = txt.Replace("<`e>", "è");
            txt = txt.Replace("<'e>", "é");
            txt = txt.Replace("<^e>", "ê");
            txt = txt.Replace("<:e>", "ë");
            txt = txt.Replace("<`i>", "ì");
            txt = txt.Replace("<'i>", "í");
            txt = txt.Replace("<^i>", "î");
            txt = txt.Replace("<:i>", "ï");
            txt = txt.Replace("<`o>", "ò");
            txt = txt.Replace("<'o>", "ó");
            txt = txt.Replace("<^o>", "ô");
            txt = txt.Replace("<:o>", "ö");
            txt = txt.Replace("<oe>", "œ");
            txt = txt.Replace("<`u>", "ù");
            txt = txt.Replace("<'u>", "ú");
            txt = txt.Replace("<^u>", "û");
            txt = txt.Replace("<:u>", "ü");
            txt = txt.Replace("<,c>", "ç");
            txt = txt.Replace("<~n>", "ñ");
            //txt = txt.Replace("<ss>", ""); Desconocida la equivalencia
            txt = txt.Replace("<`A>", "À");
            txt = txt.Replace("<'A>", "Á");
            txt = txt.Replace("<~A>", "Ã");
            txt = txt.Replace("<^A>", "Â");
            txt = txt.Replace("<:A>", "Ä");
            txt = txt.Replace("<`E>", "È");
            txt = txt.Replace("<'E>", "É");
            txt = txt.Replace("<^E>", "Ê");
            txt = txt.Replace("<:E>", "Ë");
            txt = txt.Replace("<`I>", "Ì");
            txt = txt.Replace("<'I>", "Í");
            txt = txt.Replace("<^I>", "Î");
            txt = txt.Replace("<:I>", "Ï");
            txt = txt.Replace("<`O>", "Ò");
            txt = txt.Replace("<'O>", "Ó");
            txt = txt.Replace("<^O>", "Ô");
            txt = txt.Replace("<:O>", "Ö");
            txt = txt.Replace("<OE>", "Œ");
            txt = txt.Replace("<`U>", "Ù");
            txt = txt.Replace("<'U>", "Ú");
            txt = txt.Replace("<^U>", "Û");
            txt = txt.Replace("<:U>", "Ü");
            txt = txt.Replace("<,C>", "Ç");
            txt = txt.Replace("<~N>", "Ñ");
            txt = txt.Replace("<^!>", "¡");
            txt = txt.Replace("<^?>", "¿");
            //txt = txt.Replace("<a>", ""); Desconocida la equivalencia
            //txt = txt.Replace("<0>", ""); Desconocida la equivalencia
            #endregion

            br.Close();
            br.Dispose();
            txtBox.Text = txt;

            return txtBox;
        }
    }
}
