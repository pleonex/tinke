using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Tinke
{
    public static class Convertir
    {
        #region Paleta
        /// <summary>
        /// A partir de un array de bytes devuelve un array de colores.
        /// </summary>
        /// <param name="bytes">Bytes para convertir</param>
        /// <returns>Colores de la paleta.</returns>
        public static Color[] BGR555(byte[] bytes)
        {
            Color[] paleta = new Color[bytes.Length / 2];

            for (int i = 0; i < bytes.Length / 2; i++)
            {
                paleta[i] = BGR555(bytes[i * 2], bytes[i * 2 + 1]);
            }
            return paleta;
        }
        /// <summary>
        /// Convierte dos bytes en un color.
        /// </summary>
        /// <param name="byte1">Primer byte</param>
        /// <param name="byte2">Segundo byte</param>
        /// <returns>Color convertido</returns>
        public static Color BGR555(byte byte1, byte byte2)
        {
            int r, b; double g;

            r = (byte1 % 0x20) * 0x8;
            g = (byte1 / 0x20 + ((byte2 % 0x4) * 7.96875)) * 0x8;
            b = byte2 / 0x4 * 0x8;

            return System.Drawing.Color.FromArgb(r, (int)g, b);
        }
        #endregion
    }
}
