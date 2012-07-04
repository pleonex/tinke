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
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ekona;
using System.IO;

namespace SDAT
{
    public class SDAT : IPlugin
    {
        IPluginHost pluginHost;

        #region Plugin
        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = new String(System.Text.Encoding.ASCII.GetChars(magic));

            if (ext == "SDAT" || ext == "SWAV" || ext == "STRM")
                return Format.Sound;

            return Format.Unknown;
        }
 
        public void Read(sFile file)
        {
        }
        public Control Show_Info(sFile file)
        {
            return new iSDAT(Read_SDAT(file), pluginHost);
        }
  
        public String Pack(ref sFolder unpacked, sFile file) { return null; }
        public sFolder Unpack(sFile file) { return new sFolder(); }
        #endregion

        private sSDAT Read_SDAT(sFile file)
        {
            sSDAT sdat = new sSDAT();
            sdat.id = file.id;
            sdat.archivo = file.path;
            BinaryReader br = new BinaryReader(new FileStream(file.path, FileMode.Open));
            string h = new String(br.ReadChars(4));

            if (h == "SWAV")
            {
                sdat.files.root.id = 0x0F000;
                sdat.files.root.name = "SWAV";
                sdat.files.root.files = new List<Sound>();

                Sound swavFile = new Sound();
                swavFile.id = 0x00;
                swavFile.name = file.name;
                swavFile.offset = 0x00;
                swavFile.size = file.size;
                swavFile.type = FormatSound.SWAV;
                swavFile.path = sdat.archivo;
                sdat.files.root.files.Add(swavFile);

                br.Close();
                return sdat;
            }
            if (h == "STRM")  // Quick fix
            {
                sdat.files.root.id = 0x0F000;
                sdat.files.root.name = "STRM";
                sdat.files.root.files = new List<Sound>();

                Sound swavFile = new Sound();
                swavFile.id = 0x00;
                swavFile.name = file.name;
                swavFile.offset = 0x00;
                swavFile.size = file.size;
                swavFile.type = FormatSound.STRM;
                swavFile.path = sdat.archivo;
                sdat.files.root.files.Add(swavFile);

                br.Close();
                return sdat;
            }


            br.BaseStream.Position = 0x00;

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
                char c = '\0';

                if (sdat.symbol.record2.group[i].groupOffset == 0x00) // En caso de que no exista el nombre
                    sdat.symbol.record2.group[i].groupName = "SEQARC_" + i.ToString();
                else
                {
                    // Lee el nombre del grupo
                    br.BaseStream.Position = 0x40 + sdat.symbol.record2.group[i].groupOffset;
                    c = '\0';
                    do
                    {
                        c = (char)br.ReadByte();
                        sdat.symbol.record2.group[i].groupName += c;
                    } while (c != 0x0);
                }

                // Lee los offset de las entradas del subgrupo
                if (sdat.symbol.record2.group[i].subRecOffset == 0x00) // En caso de que no haya subgrupos
                {
                    sdat.symbol.record2.group[i].subRecord = new Record();
                    continue;
                }

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
                if (info.block[5].offsetEntries[i] == 0x00)
                {
                    info.block[5].entries[i] = new Info.GROUP();
                    continue;
                }

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
            br.BaseStream.Position = sdat.cabecera.fatOffset;
            FAT fat = new FAT();

            // Header
            fat.header.id = br.ReadChars(4);
            fat.header.size = br.ReadUInt32();
            fat.header.nRecords = br.ReadUInt32();
            
            // Records
            fat.records = new FAT.Record[fat.header.nRecords];
            for (int i = 0; i < fat.header.nRecords; i++)
            {
                fat.records[i].offset = br.ReadUInt32();
                fat.records[i].size = br.ReadUInt32();
                fat.records[i].reserved = br.ReadBytes(8);
            }
            sdat.fat = fat;
            #endregion
            #region Bloque File
            br.BaseStream.Position = sdat.cabecera.fileOffset;

            // Header
            sdat.files.header.id = br.ReadChars(4);
            sdat.files.header.size = br.ReadUInt32();
            sdat.files.header.nSounds = br.ReadUInt32();
            sdat.files.header.reserved = br.ReadUInt32();
            #endregion

            br.Close();

            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(Application.StartupPath + Path.DirectorySeparatorChar +
                    "Plugins" + Path.DirectorySeparatorChar + "SDATLang.xml");
                xml = xml.Element(pluginHost.Get_Language()).Element("Messages");
                Console.WriteLine(xml.Element("S00").Value);
                Console.WriteLine(xml.Element("S01").Value, sdat.files.header.nSounds.ToString());
            }
            catch { throw new Exception("There was an error reading the XML file of language."); }

            FileSystem(ref sdat, file.path);

            Set_Data(ref sdat);

            return sdat;
        }

        private Sound SearchFile(int id, Folder currentFolder)
        {
            if (currentFolder.files is List<Sound>)
                foreach (Sound sound in currentFolder.files)
                    if (sound.id == id)
                        return sound;


            if (currentFolder.folders is List<Folder>)
            {
                foreach (Folder subFolder in currentFolder.folders)
                {
                    Sound currFile = SearchFile(id, subFolder);
                    if (currFile.name is String)
                        return currFile;
                }
            }


            return new Sound();
        }


        private void FileSystem(ref sSDAT sdat, string file)
        {
            #region Folder declaration
            Folder root = new Folder();
            root.name = "SDAT";
            root.id = 0x0F000;
            root.folders = new List<Folder>();

            Folder sseq, ssar, sbnk, swar, strm;
            sseq = new Folder();
            sseq.files = new List<Sound>();
            sseq.name = "SSEQ";
            sseq.id = 0x0F001;

            ssar = new Folder();
            ssar.files = new List<Sound>();
            ssar.name = "SSAR";
            ssar.id = 0x0F002;

            sbnk = new Folder();
            sbnk.files = new List<Sound>();
            sbnk.name = "SBNK";
            sbnk.id = 0x0F003;

            swar = new Folder();
            swar.files = new List<Sound>();
            swar.name = "SWAR";
            swar.id = 0x0F005;

            strm = new Folder();
            strm.files = new List<Sound>();
            strm.name = "STRM";
            strm.id = 0x0F006;
            #endregion

            BinaryReader br = new BinaryReader(new FileStream (file, FileMode.Open));
            for (int i = 0; i < sdat.fat.header.nRecords; i++)
            {
                br.BaseStream.Position = sdat.fat.records[i].offset;

                Sound sound = new Sound();
                sound.offset = sdat.fat.records[i].offset;
                sound.size = sdat.fat.records[i].size;
                sound.internalID = (uint)i;

                string magic = new String(Encoding.ASCII.GetChars(br.ReadBytes(4)));
                switch (magic)
                {
                    case "SSEQ":
                        sound.type = FormatSound.SSEQ;
                        sound.name = "SSEQ_" + i.ToString() + ".sseq";                   
                        sseq.files.Add(sound);
                        break;
                    case "SSAR":
                        sound.type = FormatSound.SSAR;
                        sound.name = "SSAR_" + i.ToString() + ".ssar";
                        ssar.files.Add(sound);
                        break;
                    case "SBNK":
                        sound.type = FormatSound.SBNK;
                        sound.name = "SBNK_" + i.ToString() + ".sbnk";                   
                        sbnk.files.Add(sound);
                        break;
                    case "SWAR":
                        sound.type = FormatSound.SWAR;
                        sound.name = "SWAR_" + i.ToString() + ".swar";                   
                        swar.files.Add(sound);
                        break;
                    case "STRM":
                        sound.type = FormatSound.STRM;
                        sound.name = "STRM_" + i.ToString() + ".strm";                   
                        strm.files.Add(sound);
                        break;
                }
            }
            br.Close();

            root.folders.Add(sseq);
            root.folders.Add(ssar);
            root.folders.Add(sbnk);
            root.folders.Add(swar);
            root.folders.Add(strm);

            sdat.files.root = root;
        }
        private void Set_Data(ref sSDAT sdat)
        {
            int nFile = 0;
            List<uint> oldIDs = new List<uint>();

            // SSEQ
            for (int i = 0; i < sdat.info.block[0].nEntries; i++)
            {
                uint id = ((Info.SEQ)sdat.info.block[0].entries[i]).fileID;
                
                if (id != 0x4E49) // Null value
                {
                    if (oldIDs.Contains(id)) // If it exits
                        continue;
                    oldIDs.Add(id);

                    Sound newSound = sdat.files.root.folders[0].files[nFile];
                    newSound.id = id;
                    if (sdat.cabecera.symbSize != 0x00) // If there is SYMBOL section
                        newSound.name = sdat.symbol.records[0].entries[i] + ".SSEQ"; 
                    sdat.files.root.folders[0].files[nFile] = newSound;
                }
                else
                    continue;

                nFile++;
            }

            // SSAR
            nFile = 0;
            oldIDs.Clear();
            for (int i = 0; i < sdat.info.block[1].nEntries; i++)
            {
                uint id = ((Info.SEQARC)sdat.info.block[1].entries[i]).fileID;

                if (id != 0x4E49) // Null value
                {
                    if (oldIDs.Contains(id)) // If it exits
                        continue;
                    oldIDs.Add(id);

                    Sound newSound = sdat.files.root.folders[1].files[nFile];
                    newSound.id = id;
                    if (sdat.cabecera.symbSize != 0x00) // If there is SYMBOL section
                        newSound.name = sdat.symbol.record2.group[i].groupName + ".SSAR";
                    sdat.files.root.folders[1].files[nFile] = newSound;
                }
                else
                    continue;

                nFile++;
            }

            // SBNK
            nFile = 0;
            oldIDs.Clear();
            for (int i = 0; i < sdat.info.block[2].nEntries; i++)
            {
                uint id = ((Info.BANK)sdat.info.block[2].entries[i]).fileID;

                if (id != 0x4E49) // Null value
                {
                    if (oldIDs.Contains(id)) // If it exits
                        continue;
                    oldIDs.Add(id);

                    Sound newSound = sdat.files.root.folders[2].files[nFile];
                    newSound.id = id;
                    if (sdat.cabecera.symbSize != 0x00) // If there is SYMBOL section
                        newSound.name = sdat.symbol.records[1].entries[i] + ".SBNK";
                    sdat.files.root.folders[2].files[nFile] = newSound;
                }
                else
                    continue;

                nFile++;
            }

            // SWAR
            nFile = 0;
            oldIDs.Clear();
            for (int i = 0; i < sdat.info.block[3].nEntries; i++)
            {
                uint id = ((Info.WAVEARC)sdat.info.block[3].entries[i]).fileID;

                if (id != 0x4E49) // Null value
                {
                    if (oldIDs.Contains(id)) // If it exits
                        continue;
                    oldIDs.Add(id);

                    Sound newSound = sdat.files.root.folders[3].files[nFile];
                    newSound.id = id;
                    if (sdat.cabecera.symbSize != 0x00) // If there is SYMBOL section
                        newSound.name = sdat.symbol.records[2].entries[i] + ".SWAR";
                    sdat.files.root.folders[3].files[nFile] = newSound;
                }
                else
                    continue;

                nFile++;
            }

            // STRM
            nFile = 0;
            oldIDs.Clear();
            for (int i = 0; i < sdat.info.block[7].nEntries; i++)
            {
                uint id = ((Info.STRM)sdat.info.block[7].entries[i]).fileID;

                if (id != 0x4E49) // Null value
                {
                    if (oldIDs.Contains(id)) // If it exits
                        continue;
                    oldIDs.Add(id);

                    Sound newSound = sdat.files.root.folders[4].files[nFile];
                    newSound.id = id;
                    if (sdat.cabecera.symbSize != 0x00) // If there is SYMBOL section
                        newSound.name = sdat.symbol.records[6].entries[i] + ".STRM";
                    sdat.files.root.folders[4].files[nFile] = newSound;
                }
                else
                    continue;

                nFile++;
            }
        }
    }

    public struct Folder
    {
        public string name;
        public uint id;
        public List<Sound> files;
        public List<Folder> folders;
        public object tag;
    }
    public struct Sound
    {
        public string name;
        public uint size;
        public uint offset;
        public uint id;
        public FormatSound type;
        public uint internalID;
        public string path;
        public object tag;
    }
    public enum FormatSound
    {
        SSEQ,
        SSAR,
        SBNK,
        SWAV,
        SWAR,
        STRM
    }

    public struct sSDAT
    {
        public String archivo;
        public int id;
        public NitroHeader generico;
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
        public Header header;
        public Record[] records;

        public struct Header
        {
            public char[] id;
            public uint size;
            public uint nRecords;
        }
        public struct Record
        {
            public uint offset;
            public uint size;
            public byte[] reserved;
        }
    }
    public struct FileBlock
    {
        public Header header;
        public Folder root;

        public struct Header
        {
            public char[] id;
            public uint size;
            public uint nSounds;
            public uint reserved;
        }
    }
}
