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
 * Modified (DSi): MetLob, 13.10.2017
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
            public byte unitCode;               // product code. 0=NDS, 2=NDS+DSi, 3=DSi
            public byte encryptionSeed;
            public UInt32 tamaño;
            public byte[] reserved;
            public byte twlInternalFlags;       // bit0 - Has TWL-Exclusive Region, bit1 - Modcrypted, bit2 - Debug, bit3 - Disable Debug
            public byte permitsFlags;           // bit0 - Permit Receiving Normal Jump, bit1 - Permit Receiving Temporary Jump, bit6 - For Korea, bit7 - For China
            public byte ROMversion;             // Game version
            public byte internalFlags;          // bit0 - ARM9 Compressed, bit1 - ARM7 Compressed, bit2 - Auto Boot, bit3 - Disable/Clear Initial Program Loader Memory Pad(?), bit4 - Cache ROM Reads, bit6 - ROM Type Not Specified, bit7 - RomPulledOutType = ROMID?
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
            public UInt32 ARM9overlayOffset;    // ARM9 overlay file offset
            public UInt32 ARM9overlaySize;
            public UInt32 ARM7overlayOffset;
            public UInt32 ARM7overlaySize;
            public UInt32 flagsRead;            // Control register flags for read
            public UInt32 flagsInit;            // Control register flags for init
            public UInt32 bannerOffset;         // Icon + titles offset
            public UInt16 secureCRC16;          // Secure area CRC16 0x4000 - 0x7FFF
            public UInt16 ROMtimeout;
            public UInt32 ARM9autoload;
            public UInt32 ARM7autoload;
            public UInt64 secureDisable;        // Magic number for unencrypted mode
            public UInt32 ROMsize;              // NITRO ROM data size (Excluding TWL-Exclusive Region)
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
            public byte[] reserved4;            // 0x10 bytes => Zero filled transfered but not stored in RAM

            // DSi extended stuff below
            public byte[][] global_mbk_setting; //[5][4];
            public uint[] arm9_mbk_setting;     //[3];
            public uint[] arm7_mbk_setting;     //[3];
            public uint mbk9_wramcnt_setting;

            public uint region_flags;           // Region flags (bit0=JPN, bit1=USA, bit2=EUR, bit3=AUS, bit4=CHN, bit5=KOR, bit6-31=Reserved) (FFFFFFFFh=Region Free)
            public uint access_control;
            public uint scfg_ext_mask;
            public byte[] appflags;             //[4]; bit24 - Use TWL Sound Codec bit25 Require EULA Agreement, bit26 - Has Sub Banner, bit27 - Show Nintendo Wi-Fi Connection icon in Launcher, bit28 - Show DS Wireless icon in Launcher, bit31 - Developer App

            public uint dsi9_rom_offset;
            public uint offset_0x1C4;
            public uint dsi9_ram_address;
            public uint dsi9_size;
            public uint dsi7_rom_offset;
            public uint offset_0x1D4;
            public uint dsi7_ram_address;
            public uint dsi7_size;

            public uint digest_ntr_start;       // 0x4000
            public uint digest_ntr_size;        // ntr rom size without header 0x4000
            public uint digest_twl_start;       // padding to digest_sector_size
            public uint digest_twl_size;        // arm9i and arm7i include

            public uint sector_hashtable_start; // padding to digest_sector_size
            public uint sector_hashtable_size;  // padding to (0x14 * digest_block_sectorcount)
            public uint block_hashtable_start;
            public uint block_hashtable_size;

            public uint digest_sector_size;
            public uint digest_block_sectorcount;
            public uint banner_size;
            public uint offset_0x20C;

            public uint total_rom_size;
            public uint offset_0x214;
            public uint offset_0x218;
            public uint offset_0x21C;

            public uint modcrypt1_start;        // dsi9_rom_offset
            public uint modcrypt1_size;
            public uint modcrypt2_start;        // dsi7_rom_offset;
            public uint modcrypt2_size;

            public uint tid_low;                // inversed GAME ID
            public uint tid_high;
            public uint public_sav_size;
            public uint private_sav_size;

            public byte[] reserved5;             //[0xB0];
            public byte[] age_ratings;           //[0x10];
            public byte[] hmac_arm9;             //[20];
            public byte[] hmac_arm7;             //[20];
            public byte[] hmac_digest_master;    //[20];
            public byte[] hmac_icon_title;       //[20];
            public byte[] hmac_arm9i;            //[20];
            public byte[] hmac_arm7i;            //[20];
            public byte[] reserved6;             //[40];
            public byte[] hmac_arm9_no_secure;   //[20];
            public byte[] reserved7;             //[0xA4C];
            public byte[] debug_args;            //[0x180];
            public byte[] rsa_signature;         //[0x80];

            public bool trimmedRom;
            public bool doublePadding;

            /* Parental Control Age Ratings (for different countries/areas)
             *     Bit7: Rating exists for local country/area
             *     Bit6: Game is prohibited in local country/area?
             *     Bit5-0: Age rating for local country/area(years)
             *     2F0h 1    CERO(Japan)       (0=None/A, 12=B, 15=C, 17=D, 18=Z)
             *     2F1h 1    ESRB(US/Canada)   (0=None, 3=EC, 6=E, 10=E10+, 13=T, 17=M)
             *     2F2h 1    Reserved          (0=None)
             *     2F3h 1    USK(Germany)      (0=None, 6=6+, 12=12+, 16=16+, 18=18+)
             *     2F4h 1    PEGI(Pan-Europe)  (0=None, 3=3+, 7=7+, 12=12+, 16=16+, 18=18+)
             *     2F5h 1    Reserved          (0=None)
             *     2F6h 1    PEGI(Portugal)    (0=None, 4=4+, 6=6+, 12=12+, 16=16+, 18=18+)
             *     2F7h 1    PEGI and BBFC(UK) (0=None, 3, 4=4+/U, 7, 8=8+/PG, 12, 15, 16, 18)
             *     2F8h 1    AGCB(Australia)   (0=None/G, 7=PG, 14=M, 15=MA15+, plus 18=R18+?)
             *     2F9h 1    GRB(South Korea)  (0=None, 12=12+, 15=15+, 18=18+)
             *     2FAh 6    Reserved(6x)      (0=None) */
        }
        public struct Banner
        {
            public UInt16 version;      // 1 - Standard 0x840, 2 - with Cninese, 3 - add Cninese and Korean titles, 0x0103 - add DSi animated icon
            public UInt16 CRC16;        // CRC-16 across entries 0020h..083Fh (all versions)
            public UInt16 CRC162;        // CRC-16 across entries 0020h..093Fh (Version 0002h and up)
            public UInt16 CRC163;        // CRC-16 across entries 0020h..0A3Fh (Version 0003h and up)
            public UInt16 CRC16i;        // CRC-16 across entries 1240h..23BFh (Version 0103h and up)
            public bool checkCRC;
            public byte[] reserved;     // 22 bytes
            public byte[] tileData;     // 512 bytes
            public byte[] palette;      // 32 bytes
            public string japaneseTitle;// 256 bytes
            public string englishTitle; // 256 bytes
            public string frenchTitle;  // 256 bytes
            public string germanTitle;  // 256 bytes
            public string italianTitle; // 256 bytes
            public string spanishTitle; // 256 bytes

            // DSi Enchansed
            public string cnineseTitle; // 256 bytes
            public string koreanTitle;  // 256 bytes
            public byte[] reservedDsi;  // 0x800 bytes reserved for Title 8..15
            public byte[] aniIconData;  // 0x1180 bytes (8 * tiles + 8 * palette + 8 * 16 bytes of animation sequence)

            public uint GetDefSize(uint hardBannerSize = 0)
            {
                if (this.version == 1) return 0x840;
                if (this.version == 2) return 0x940;
                if (this.version == 3) return 0xA40;
                if (this.version == 0x0103) return (hardBannerSize > 0 && (int)hardBannerSize != -1) ? hardBannerSize : 0x23C0;
                return 0x840;
            }
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