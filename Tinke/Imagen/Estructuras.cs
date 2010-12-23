using System;
using System.Drawing;
using System.Collections.Generic;

namespace Tinke.Imagen
{
    public struct NTFP              // Nintendo Tile Format Palette
    {
        public Color[] colores;
    }
    public struct NTFT              // Nintendo Tile Format Tile
    {
        public byte[][] tiles;
    }
    public struct NTFS              // Nintedo Tile Format Screen
    {
        public byte nPalette;        // Junto con los cuatro siguientes forman dos bytes de la siguiente forma (en bits):
        public byte xFlip;           // PPPP X Y NNNNNNNNNN
        public byte yFlip;
        public ushort nTile;
    }

    public enum Tiles_Form
    {
        bpp8 = 0,
        bpp4 = 1
    }
    public enum Tiles_Style
    {
        Tiled,
        Horizontal,
        Vertical
    }
}