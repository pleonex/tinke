using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tinke.Imagen.Screen
{
    public static class Estructuras
    {
        public struct NSCR
        {
            public char[] id;                   // RCSN = 0x5243534E
            public UInt16 endianess;            // 0xFFFE -> indica little endian
            public UInt16 constant;             // Siempre es 0x0100
            public UInt32 file_size;            // Tamaño total del archivo
            public UInt16 header_size;          // Siempre es 0x10
            public UInt16 nSection;             // Número de secciones
            public NSCR_Section section;        // Sección NSCR
        }
        public struct NSCR_Section
        {
            public char[] id;                   // NRCS = 0x4E524353
            public UInt32 section_size;         // Tamaño del archivo total
            public UInt16 width;                // Ancho de la imagen
            public UInt16 height;               // Alto de la imagen
            public UInt32 padding;              // Siempre 0x0
            public UInt32 data_size;            //
            public Imagen.NTFS[] screenData;
        }
    }
}
