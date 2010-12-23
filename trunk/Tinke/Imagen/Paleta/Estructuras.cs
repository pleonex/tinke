using System;
using System.Drawing;

namespace Tinke.Imagen.Paleta
{
    public static class Estructuras
    {
        public struct NCLR
        {
            public char[] ID;
            public UInt16 endianness;        // 0xFEFF
            public UInt16 constante;
            public UInt32 tamaño;           // Incluye lo anterior
            public UInt32 tamañoCabecera;   // Suele ser 0x10
            public UInt16 nSecciones;
            public TTLP pltt;
        }
        public struct TTLP
        {
            public char[] ID;
            public UInt32 tamaño;       // Incluye cabecera
            public Depth profundidad;
            public UInt32 constante;    // Siempre 0x00000000
            public UInt32 tamañoPaletas;
            public UInt32 nColores;     // Suele ser 0x10
            public NTFP[] paletas;
        }
    }
    public enum Depth
    {
        bits4,
        bits8
        
    }
}