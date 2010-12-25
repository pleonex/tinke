using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Tinke
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            if (args.Length != 2)
                Application.Run(new Form1());
            else if (args.Length == 2)      // Primer argumento archivo ROM, segundo id del archivo.
                Application.Run(new Form1(args[0], Convert.ToInt32(args[1])));
            
        }
    }
}
