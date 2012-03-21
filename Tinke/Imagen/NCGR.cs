using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using PluginInterface;

namespace Tinke
{
    public static class Imagen_NCGR
    {
        public static Byte[][] MergeImage(Byte[][] originalTile, Byte[][] newTiles, int startTile)
        {
            List<Byte[]> data = new List<byte[]>();


            for (int i = 0; i < startTile; i++)
            {
                if (i >= originalTile.Length)
                {
                    Byte[] nullTile = new byte[64];
                    for (int t = 0; t < 64; t++)
                        nullTile[t] = 0;
                    data.Add(nullTile);
                    continue;
                }
                data.Add(originalTile[i]);
            }

            data.AddRange(newTiles);

            for (int i = startTile + newTiles.Length; i < originalTile.Length; i++)
                data.Add(originalTile[i]);

            return data.ToArray();
        }
    }
}
