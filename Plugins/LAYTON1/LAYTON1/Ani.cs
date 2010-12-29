using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;

namespace LAYTON1
{
    static  class Ani
    {

        public static Formato Get_Formato(string nombre)
        {
            if (nombre.EndsWith(".ARC"))
                return Formato.Animación;
            else
                return Formato.Desconocido;
        }

        public static void Leer()
        {
            throw new NotImplementedException();
        }

        public static void Show_Info()
        {
            throw new NotImplementedException();
        }
    }
}
