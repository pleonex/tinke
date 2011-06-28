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
 * Programa utilizado: Visual Studio 2010
 * Fecha: 24/06/2011
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;
using System.IO;

namespace SDAT
{
    public class SDAT : IPlugin
    {
        IPluginHost pluginHost;

        #region Plugin
        public Formato Get_Formato(string nombre, byte[] magic)
        {
            nombre = nombre.ToUpper();
            string ext = new String(System.Text.Encoding.ASCII.GetChars(magic));

            if (nombre.EndsWith(".SDAT") && ext == "SDAT")
                return Formato.Sonido;

            return Formato.Desconocido;
        }
        public void Inicializar(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public void Leer(string archivo)
        {
            MessageBox.Show("Este archivo no guarda información.");
        }
        public Control Show_Info(string archivo)
        {
            return new iSDAT(Informacion(archivo));
        }
        #endregion

        private sSDAT Informacion(string archivo)
        {
            BinaryReader br = new BinaryReader(new FileStream(archivo, FileMode.Open));
            sSDAT sdat = new sSDAT();

            #region Cabecera genérica
            sdat.generico.id = br.ReadChars(4);
            sdat.generico.endianess = br.ReadUInt16();
            sdat.generico.constant = br.ReadUInt16();
            sdat.generico.file_size = br.ReadUInt32();
            sdat.generico.header_size = br.ReadUInt16();
            sdat.generico.nSection = br.ReadUInt16();
            #endregion
            #region Cabecera SDAT
            sdat.cabecera.symbOffset = br.ReadUInt32();
            sdat.cabecera.symbSize = br.ReadUInt32();
            sdat.cabecera.infoOffset = br.ReadUInt32();
            sdat.cabecera.infoSize = br.ReadUInt32();
            sdat.cabecera.fatOffset = br.ReadUInt32();
            sdat.cabecera.fatSize = br.ReadUInt32();
            sdat.cabecera.fileOffset = br.ReadUInt32();
            sdat.cabecera.fileSize = br.ReadUInt32();
            sdat.cabecera.reserved = br.ReadBytes(16);
            #endregion
            #region Bloque Symbol
            if (sdat.cabecera.symbSize == 0x00) // no hay sección Symbol
                goto Info;
            br.BaseStream.Position = sdat.cabecera.symbOffset;
            sdat.symbol.id = br.ReadChars(4);
            sdat.symbol.size = br.ReadUInt32();
            sdat.symbol.offsetSeq = br.ReadUInt32();
            sdat.symbol.offsetSeqArc = br.ReadUInt32();
            sdat.symbol.offsetBank = br.ReadUInt32();
            sdat.symbol.offsetWaveArch = br.ReadUInt32();
            sdat.symbol.offsetPlayer = br.ReadUInt32();
            sdat.symbol.offsetGroup = br.ReadUInt32();
            sdat.symbol.offsetPlayer2 = br.ReadUInt32();
            sdat.symbol.offsetStream = br.ReadUInt32();
            sdat.symbol.reserved = br.ReadBytes(24);
            
            // Lectura de las entradas de cada record
            sdat.symbol.records = new Record[7];
            uint[] offsets = new uint[7] { sdat.symbol.offsetSeq,
                sdat.symbol.offsetBank, sdat.symbol.offsetWaveArch, sdat.symbol.offsetPlayer, 
                sdat.symbol.offsetGroup, sdat.symbol.offsetPlayer2, sdat.symbol.offsetStream
            };

            #region Other Record
            for (int i = 0; i < offsets.Length; i++)
            {
                br.BaseStream.Position = 0x40 + offsets[i];
                sdat.symbol.records[i] = new Record();
                sdat.symbol.records[i].nEntries = br.ReadUInt32();
                sdat.symbol.records[i].entriesOffset = new uint[sdat.symbol.records[i].nEntries];
                sdat.symbol.records[i].entries = new string[sdat.symbol.records[i].nEntries];
                for (int j = 0; j < sdat.symbol.records[i].nEntries; j++)
                    sdat.symbol.records[i].entriesOffset[j] = br.ReadUInt32();
                for (int k = 0; k < sdat.symbol.records[i].nEntries; k++)
                {
                    if (sdat.symbol.records[i].entriesOffset[k] == 0x00)
                        continue;
                    br.BaseStream.Position = 0x40 + sdat.symbol.records[i].entriesOffset[k];

                    char c = '\0';
                    do
                    {
                        c = (char)br.ReadByte();
                        sdat.symbol.records[i].entries[k] += c;
                    } while (c != 0x0);
                }
            }
            #endregion
            #region SEQARC Record
            br.BaseStream.Position = 0x40 + sdat.symbol.offsetSeqArc;
            sdat.symbol.record2 = new Record2();
            sdat.symbol.record2.nEntries = br.ReadUInt32();
            sdat.symbol.record2.group = new Group[sdat.symbol.record2.nEntries];
            // Lee los offset de cada grupo
            for (int i = 0; i < sdat.symbol.record2.nEntries; i++)
            {
                sdat.symbol.record2.group[i].groupOffset = br.ReadUInt32();
                sdat.symbol.record2.group[i].subRecOffset = br.ReadUInt32();
            }
            // Lee los subgrupos de cada grupo
            for (int i = 0; i < sdat.symbol.record2.nEntries; i++)
            {
                // Lee el nombre del grupo
                br.BaseStream.Position = 0x40 + sdat.symbol.record2.group[i].groupOffset;
                char c = '\0';
                do
                {
                    c = (char)br.ReadByte();
                    sdat.symbol.record2.group[i].groupName += c;
                } while (c != 0x0);

                // Lee los offset de las entradas del subgrupo
                br.BaseStream.Position = 0x40 + sdat.symbol.record2.group[i].subRecOffset;
                Record subRecord = new Record();
                subRecord.nEntries = br.ReadUInt32();
                subRecord.entriesOffset = new uint[subRecord.nEntries];
                subRecord.entries = new string[subRecord.nEntries];
                for (int j = 0; j < subRecord.nEntries; j++)
                    subRecord.entriesOffset[j] = br.ReadUInt32();
                // Lee las entradas del subgrupo
                for (int j = 0; j < subRecord.nEntries; j++)
                {
                    if (subRecord.entriesOffset[j] == 0x00)
                        continue;
                    br.BaseStream.Position = 0x40 + subRecord.entriesOffset[j];
                    c = '\0';
                    do
                    {
                        c = (char)br.ReadByte();
                        subRecord.entries[j] += c;
                    } while (c != 0x0);
                }
                sdat.symbol.record2.group[i].subRecord = subRecord;
            }
            #endregion
            #endregion
            #region Bloque Info
        Info:

            br.BaseStream.Position = sdat.cabecera.infoOffset;
            Info info = new Info();
            // Header
            info.header.id = br.ReadChars(4);
            info.header.size = br.ReadUInt32();
            info.header.offsetRecords = new uint[8];
            for (int i = 0; i < 8; i++)
                info.header.offsetRecords[i] = br.ReadUInt32();
            info.header.reserved = br.ReadBytes(24);

            // Blocks
            info.block = new Info.Block[8];
            for (int i = 0; i < 8; i++)
            {
                br.BaseStream.Position = sdat.cabecera.infoOffset + info.header.offsetRecords[i];
                info.block[i].nEntries = br.ReadUInt32();
                info.block[i].offsetEntries = new uint[info.block[i].nEntries];
                info.block[i].entries = new object[info.block[i].nEntries];
                for (int j = 0; j < info.block[i].nEntries; j++)
                    info.block[i].offsetEntries[j] = br.ReadUInt32();
            }

            // Entries
            // SEQ
            for (int i = 0; i < info.block[0].nEntries; i++)
            {
                br.BaseStream.Position = sdat.cabecera.infoOffset + info.block[0].offsetEntries[i];

                Info.SEQ seq = new Info.SEQ();
                seq.fileID = br.ReadUInt16();
                seq.unknown = br.ReadUInt16();
                seq.bnk = br.ReadUInt16();
                seq.vol = br.ReadByte();
                seq.cpr = br.ReadByte();
                seq.ppr = br.ReadByte();
                seq.ply = br.ReadByte();
                seq.unknown2 = br.ReadBytes(2);
                info.block[0].entries[i] = seq;
            }
            // SEQARC
            for (int i = 0; i < info.block[1].nEntries; i++)
            {
                br.BaseStream.Position = sdat.cabecera.infoOffset + info.block[1].offsetEntries[i];

                Info.SEQARC seq = new Info.SEQARC();
                seq.fileID = br.ReadUInt16();
                seq.unknown = br.ReadUInt16();
                info.block[1].entries[i] = seq;
            }
            // BANK
            for (int i = 0; i < info.block[2].nEntries; i++)
            {
                br.BaseStream.Position = sdat.cabecera.infoOffset + info.block[2].offsetEntries[i];

                Info.BANK bank = new Info.BANK();
                bank.fileID = br.ReadUInt16();
                bank.unknown = br.ReadUInt16();
                bank.wa = new ushort[4];
                for (int j = 0; j < 4; j++)
                    bank.wa[j] = br.ReadUInt16();
                info.block[2].entries[i] = bank;
            }
            // WAVEARC
            for (int i = 0; i < info.block[3].nEntries; i++)
            {
                br.BaseStream.Position = sdat.cabecera.infoOffset + info.block[3].offsetEntries[i];

                Info.WAVEARC wave = new Info.WAVEARC();
                wave.fileID = br.ReadUInt16();
                wave.unknown = br.ReadUInt16();
                info.block[3].entries[i] = wave;
            }
            // PLAYER
            for (int i = 0; i < info.block[4].nEntries; i++)
            {
                br.BaseStream.Position = sdat.cabecera.infoOffset + info.block[4].offsetEntries[i];

                Info.PLAYER player = new Info.PLAYER();
                player.unknown = br.ReadByte();
                player.padding = br.ReadBytes(3);
                player.unknown2 = br.ReadUInt32();
                info.block[4].entries[i] = player;
            }
            // GROUP
            for (int i = 0; i < info.block[5].nEntries; i++)
            {
                br.BaseStream.Position = sdat.cabecera.infoOffset + info.block[5].offsetEntries[i];

                Info.GROUP group = new Info.GROUP();
                group.nCount = br.ReadUInt32();
                group.subgroup = new Info.GROUP.Subgroup[group.nCount];
                for (int j = 0; j < group.nCount; j++)
                {
                    group.subgroup[j].type = br.ReadUInt32();
                    group.subgroup[j].nEntry = br.ReadUInt32();
                }
                info.block[5].entries[i] = group;
            }
            // PLAYER2
            for (int i = 0; i < info.block[6].nEntries; i++)
            {
                br.BaseStream.Position = sdat.cabecera.infoOffset + info.block[6].offsetEntries[i];

                Info.PLAYER2 player = new Info.PLAYER2();
                player.nCount = br.ReadByte();
                player.v = br.ReadBytes(16);
                player.reserved = br.ReadBytes(7);
                info.block[6].entries[i] = player;
            }
            // STRM
            for (int i = 0; i < info.block[7].nEntries; i++)
            {
                br.BaseStream.Position = sdat.cabecera.infoOffset + info.block[7].offsetEntries[i];

                Info.STRM strm = new Info.STRM();
                strm.fileID = br.ReadUInt16();
                strm.unknown = br.ReadUInt16();
                strm.vol = br.ReadByte();
                strm.pri = br.ReadByte();
                strm.ply = br.ReadByte();
                strm.reserved = br.ReadBytes(5);
                info.block[7].entries[i] = strm;
            }
            sdat.info = info;
            #endregion
            #region Bloque FAT
            #endregion
            #region Bloque File
            #endregion

            br.Close();
            return sdat;
        }

    }

    public struct sSDAT
    {
        public Header generico;
        public Cabecera cabecera;
        public Symbol symbol;
        public Info info;
        public FAT fat;
        public FileBlock files;
        
    }
    public struct Cabecera
    {
        public UInt32 symbOffset;
        public UInt32 symbSize;
        public UInt32 infoOffset;
        public UInt32 infoSize;
        public UInt32 fatOffset;
        public UInt32 fatSize;
        public UInt32 fileOffset;
        public UInt32 fileSize;
        public Byte[] reserved;
    }
    #region SYMBOL
    public struct Symbol
    {
        public Char[] id;
        public UInt32 size;
        public UInt32 offsetSeq;
        public UInt32 offsetSeqArc;
        public UInt32 offsetBank;
        public UInt32 offsetWaveArch;
        public UInt32 offsetPlayer;
        public UInt32 offsetGroup;
        public UInt32 offsetPlayer2;
        public UInt32 offsetStream;
        public Byte[] reserved;
        public Record[] records;
        public Record2 record2;

    }
    public struct Record
    {
        public UInt32 nEntries;
        public UInt32[] entriesOffset;
        public String[] entries;
    }
    public struct Record2
    {
        public UInt32 nEntries;
        public Group[] group;
    }
    public struct Group
    {
        public UInt32 groupOffset;
        public String groupName;
        public UInt32 subRecOffset;
        public Record subRecord;
    }
    #endregion
    public struct Info
    {
        public Header header;
        public Block[] block;

        public struct Header
        {
            public char[] id;
            public uint size;
            public uint[] offsetRecords;
            public byte[] reserved;
        }
        public struct Block
        {
            public uint nEntries;
            public uint[] offsetEntries;
            public object[] entries;
        }
        public struct SEQ
        {
            public ushort fileID;
            public ushort unknown;
            public ushort bnk;
            public byte vol;
            public byte cpr;
            public byte ppr;
            public byte ply;
            public byte[] unknown2;
        }
        public struct SEQARC
        {
            public ushort fileID;
            public ushort unknown;
        }
        public struct BANK
        {
            public ushort fileID;
            public ushort unknown;
            public ushort[] wa;
        }
        public struct WAVEARC
        {
            public ushort fileID;
            public ushort unknown;
        }
        public struct PLAYER
        {
            public byte unknown;
            public byte[] padding;
            public uint unknown2;
        }
        public struct GROUP
        {
            public uint nCount;
            public Subgroup[] subgroup;

            public struct Subgroup
            {
                public uint type;   // 0x0700 => SEQ; 0x0803 => SEQARC; 0x0601 => BANK; 0x0402 => WAVEARC
                public uint nEntry;
            }
        }
        public struct PLAYER2
        {
            public byte nCount;
            public byte[] v;
            public byte[] reserved;
        }
        public struct STRM
        {
            public ushort fileID;
            public ushort unknown;
            public byte vol;
            public byte pri;
            public byte ply;
            public byte[] reserved;
        }
    }
    public struct FAT
    {
    }
    public struct FileBlock
    {
    }
}
