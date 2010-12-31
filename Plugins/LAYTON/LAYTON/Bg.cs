using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginInterface;

namespace LAYTON
{
    static class Bg
    {
        public static Formato Get_Formato(string nombre, char[] magic, int id)
        {
            if (nombre.EndsWith(".ARC"))
                return Formato.ImagenCompleta;

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
