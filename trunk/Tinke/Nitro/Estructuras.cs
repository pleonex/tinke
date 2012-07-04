/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 * Programador: pleoNeX
 * Programa utilizado: Microsoft Visual C# 2010 Express
 * Fecha: 18/02/2011
 * 
 */

using System;
using System.Collections.Generic;
using Ekona;

namespace Tinke.Nitro
{
    public static class Estructuras
    {
        public static Dictionary<string, string> makerCode;
        public static Dictionary<byte, string> unitCode;

        public struct ROMHeader
        {
            public char[] gameTitle;
            public char[] gameCode;
            public char[] makerCode;
            public byte unitCode;
            public byte encryptionSeed;
            public UInt32 tamaño;
            public byte[] reserved;
            public byte ROMversion;
            public byte internalFlags;
            public UInt32 ARM9romOffset;
            public UInt32 ARM9entryAddress;
            public UInt32 ARM9ramAddress;
            public UInt32 ARM9size;
            public UInt32 ARM7romOffset;
            public UInt32 ARM7entryAddress;
            public UInt32 ARM7ramAddress;
            public UInt32 ARM7size;
            public UInt32 fileNameTableOffset;
            public UInt32 fileNameTableSize;
            public UInt32 FAToffset;            // File Allocation Table offset
            public UInt32 FATsize;              // File Allocation Table size
            public UInt32 ARM9overlayOffset;      // ARM9 overlay file offset
            public UInt32 ARM9overlaySize;
            public UInt32 ARM7overlayOffset;
            public UInt32 ARM7overlaySize;
            public UInt32 flagsRead;            // Control register flags for read
            public UInt32 flagsInit;            // Control register flags for init
            public UInt32 bannerOffset;           // Icon + titles offset
            public UInt16 secureCRC16;          // Secure area CRC16 0x4000 - 0x7FFF
            public UInt16 ROMtimeout;
            public UInt32 ARM9autoload;
            public UInt32 ARM7autoload;
            public UInt64 secureDisable;        // Magic number for unencrypted mode
            public UInt32 ROMsize;
            public UInt32 headerSize;
            public byte[] reserved2;            // 56 bytes
            //public byte[] logo;               // 156 bytes de un logo de nintendo usado para comprobaciones de seguridad
            public UInt16 logoCRC16;
            public UInt16 headerCRC16;
            public bool secureCRC;
            public bool logoCRC;
            public bool headerCRC;
            public UInt32 debug_romOffset;      // only if debug
            public UInt32 debug_size;           // version with
            public UInt32 debug_ramAddress;     // 0 = none, SIO and 8 MB
            public UInt32 reserved3;            // Zero filled transfered and stored but not used
            //public byte[] reserved4;          // 0x90 bytes => Zero filled transfered but not stored in RAM
 
        }
        public struct Banner
        {
            public UInt16 version;      // Always 1
            public UInt16 CRC16;        // CRC-16 of structure, not including first 32 bytes
            public bool checkCRC;
            public byte[] reserved;     // 28 bytes
            public byte[] tileData;     // 512 bytes
            public byte[] palette;      // 32 bytes
            public string japaneseTitle;// 256 bytes
            public string englishTitle; // 256 bytes
            public string frenchTitle;  // 256 bytes
            public string germanTitle;  // 256 bytes
            public string italianTitle; // 256 bytes
            public string spanishTitle; // 256 bytes
        }

        /// <summary>
        /// Estructura de las tablas principales dentro del archivos FNT. Cada una corresponde
        /// a un directorio.
        /// </summary>
        public struct MainFNT
        {
            public UInt32 offset;           // OffSet de la SubTable relativa al archivo FNT
            public UInt16 idFirstFile;      // ID del primer archivo que contiene. Puede corresponder a uno que contenga un directorio interno
            public UInt16 idParentFolder;   // ID del directorio padre de éste
            public sFolder subTable;         // SubTable que contiene los nombres de archivos y carpetas que contiene el directorio
        }

        public struct sFAT
        {
            public uint offset;
            public uint size;
        }

        public struct ARMOverlay
        {
            public UInt32 OverlayID;
            public UInt32 RAM_Adress;   // Point at which to load
            public UInt32 RAM_Size;     // Amount to load
            public UInt32 BSS_Size;     // Size of BSS data region
            public UInt32 stInitStart;  // Static initialiser start address
            public UInt32 stInitEnd;    // Static initialiser end address
            public UInt32 fileID;
            public UInt32 reserved;
            public bool ARM9;           // Si es true es ARM9, sino es ARM7
        }
    }
}