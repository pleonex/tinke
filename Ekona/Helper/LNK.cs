// ----------------------------------------------------------------------
// <copyright file="LNK.cs" company="none">

// Copyright (C) 2012
//
//   This program is free software: you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by 
//   the Free Software Foundation, either version 3 of the License, or
//   (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful, 
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//
//   You should have received a copy of the GNU General Public License
//   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
//
// </copyright>

// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>06/08/2012 14:08:22</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Ekona.Helper
{
    /// <summary>
    /// Specification from http://msdn.microsoft.com/en-us/library/dd871305%28v=prot.13%29.aspx
    /// </summary>
    public class LNK
    {
        SHELL_LINK lnk;

        public LNK(string fileIn)
        {
            Read(fileIn);
        }

        private void Read(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));
            lnk = new SHELL_LINK();
            
            lnk.header = Read_Header(br);

            if (lnk.header.linkFlags.hasLinkTargetIDList)
                lnk.idlist = Read_LinkIDList(br);
            if (lnk.header.linkFlags.hasLinkInfo)
                lnk.info = Read_LinkInfo(br);

            lnk.sdata = Read_StringData(br);
            lnk.extra = Read_Extra(br);

            br.Close();
            br = null;
        }

        private SHELL_LINK_HEADER Read_Header(BinaryReader br)
        {
            SHELL_LINK_HEADER header = new SHELL_LINK_HEADER();

            header.headerSize = br.ReadUInt32();
            if (header.headerSize != 0x4C)
                throw new FormatException("Incorrect file size!");

            header.linkCLSID = br.ReadBytes(0x10);
            for (int i = 0; i < 0x10; i++)
                if (header.linkCLSID[i] != CLSID[i])
                    throw new FormatException("Invalid CLSID!");

            header.linkFlags = Read_LinkFlags(br.ReadUInt32());
            header.fileAttributes = Read_FileAttribute(br.ReadUInt32());
            header.creationTime.dateTime = br.ReadUInt64();
            header.accessTime.dateTime = br.ReadUInt64();
            header.writeTime.dateTime = br.ReadUInt64();
            header.fileSize = br.ReadUInt32();
            header.iconIndex = br.ReadInt32();
            header.showCommand = (SHOW_COMMAND)br.ReadUInt32();
            header.hotKey.low = (HOTKEYS_FLAGS.LOW_BYTE)br.ReadByte();
            header.hotKey.hight = (HOTKEYS_FLAGS.HIGH_BYTE)br.ReadByte();
            header.reserved1 = br.ReadUInt16();
            header.reserved2 = br.ReadUInt32();
            header.reserved3 = br.ReadUInt32();

            return header;
        }
        private LINK_FLAGS Read_LinkFlags(uint value)
        {
            LINK_FLAGS flags = new LINK_FLAGS();

            flags.hasLinkTargetIDList = Get_Boolean(value); value >>= 1;
            flags.hasLinkInfo = Get_Boolean(value); value >>= 1;
            flags.hasName = Get_Boolean(value); value >>= 1;
            flags.hasRelativePath = Get_Boolean(value); value >>= 1;
            flags.hasWorkingDir = Get_Boolean(value); value >>= 1;
            flags.hasArguments = Get_Boolean(value); value >>= 1;
            flags.hasIconLocation = Get_Boolean(value); value >>= 1;
            flags.isUnicode = Get_Boolean(value); value >>= 1;
            flags.forceNoLinkInfo = Get_Boolean(value); value >>= 1;
            flags.hasExpString = Get_Boolean(value); value >>= 1;
            flags.runInSeparateProcess = Get_Boolean(value); value >>= 1;
            flags.unused1 = Get_Boolean(value); value >>= 1;
            flags.hasDarwinID = Get_Boolean(value); value >>= 1;
            flags.runAsUser = Get_Boolean(value); value >>= 1;
            flags.hasExpIcon = Get_Boolean(value); value >>= 1;
            flags.noPidlAlias = Get_Boolean(value); value >>= 1;
            flags.unused2 = Get_Boolean(value); value >>= 1;
            flags.runWithShimLayer = Get_Boolean(value); value >>= 1;
            flags.forceNoLinkTrack = Get_Boolean(value); value >>= 1;
            flags.enableTargetMetadata = Get_Boolean(value); value >>= 1;
            flags.disableLinkPathTracking = Get_Boolean(value); value >>= 1;
            flags.disableKnownFolderAlias = Get_Boolean(value); value >>= 1;
            flags.allowLinkToLink = Get_Boolean(value); value >>= 1;
            flags.unaliasOnSave = Get_Boolean(value); value >>= 1;
            flags.preferEnvironmentPath = Get_Boolean(value); value >>= 1;
            flags.keepLocalIDListForUNCTarget = Get_Boolean(value);

            return flags;
        }
        private FILE_ATTRIBUTE_FLAGS Read_FileAttribute(uint value)
        {
            FILE_ATTRIBUTE_FLAGS flags = new FILE_ATTRIBUTE_FLAGS();

            flags.readOnly = Get_Boolean(value); value >>= 1;
            flags.hidden = Get_Boolean(value); value >>= 1;
            flags.system = Get_Boolean(value); value >>= 1;
            flags.reserved1 = Get_Boolean(value); value >>= 1;
            flags.directory = Get_Boolean(value); value >>= 1;
            flags.archive = Get_Boolean(value); value >>= 1;
            flags.reserved2 = Get_Boolean(value); value >>= 1;
            flags.normal = Get_Boolean(value); value >>= 1;
            flags.temporary = Get_Boolean(value); value >>= 1;
            flags.sparse_file = Get_Boolean(value); value >>= 1;
            flags.compressed = Get_Boolean(value); value >>= 1;
            flags.offline = Get_Boolean(value); value >>= 1;
            flags.offline = Get_Boolean(value); value >>= 1;
            flags.not_content_indexed = Get_Boolean(value); value >>= 1;
            flags.encrypted = Get_Boolean(value); value >>= 1;

            return flags;
        }

        private LINKTARGET_IDLIST Read_LinkIDList(BinaryReader br)
        {
            LINKTARGET_IDLIST idlist = new LINKTARGET_IDLIST();

            idlist.IDListSize = br.ReadUInt16();
            idlist.IDList = Read_IDList(br);

            return idlist;
        }
        private IDLIST Read_IDList(BinaryReader br)
        {
            IDLIST idlist = new IDLIST();
            idlist.itemIDList = new List<ITEM_IDLIST>();

            ushort size = br.ReadUInt16();
            while (size != 0)
            {
                ITEM_IDLIST item = new ITEM_IDLIST();
                item.itemIDSize = size;
                item.data = br.ReadBytes(size - 2);
                idlist.itemIDList.Add(item);

                size = br.ReadUInt16();
            }
            idlist.terminalID = size;

            return idlist;
        }
        private LINKINFO Read_LinkInfo(BinaryReader br)
        {
            LINKINFO info = new LINKINFO();
            uint info_pos = (uint)br.BaseStream.Position;

            info.linkInfoSize = br.ReadUInt32();
            info.linkInfoHeaderSize = br.ReadUInt32();

            uint value = br.ReadUInt32();
            info.volumeIDAndLocalBasePath = Get_Boolean(value); value >>= 1;
            info.commonNetworkRelativeLinkAndPathSuffix = Get_Boolean(value);

            info.volumeIDOffset = br.ReadUInt32();
            info.localBasePathOffset = br.ReadUInt32();
            info.commonNetworkRelativeLinkOffset = br.ReadUInt32();
            info.commonPathSuffixOffset = br.ReadUInt32();

            if (info.linkInfoHeaderSize >= 0x24)
            {
                info.localBasePathOffsetUnicode = br.ReadUInt32();
                info.commonPathSuffixOffsetUnicode = br.ReadUInt32();
            }

            if (info.volumeIDAndLocalBasePath)
            {     
                // Volume ID
                uint volumeID_pos = info_pos + info.volumeIDOffset;
                br.BaseStream.Position = volumeID_pos;
                info.volumeID = new VOLUMEID();

                info.volumeID.volumeIDSize = br.ReadUInt32();
                info.volumeID.driveType = (DRIVE_TYPE)br.ReadUInt32();
                info.volumeID.driveSerialNumber = br.ReadUInt32();
                info.volumeID.volumeLabelOffset = br.ReadUInt32();

                if (info.volumeID.volumeLabelOffset != 0x14)
                    info.volumeID.data = Get_String(br, false, volumeID_pos + info.volumeID.volumeLabelOffset);
                else
                {
                    info.volumeID.volumeLabelOffsetUnicode = br.ReadUInt32();
                    info.volumeID.data = Get_String(br, true, volumeID_pos + info.volumeID.volumeLabelOffsetUnicode);
                }

                // Local Base Path
                info.localBasePath = Get_String(br, false, info_pos + info.localBasePathOffset);
            }

            if (info.commonNetworkRelativeLinkAndPathSuffix)
            {
                // Common Network Relative Link
                uint cnrl_pos = info_pos + info.commonNetworkRelativeLinkOffset;
                info.cnrl = new COMMON_NETWORK_RELATIVE_LINK();

                info.cnrl.cnrl_size = br.ReadUInt32();
                uint flags = br.ReadUInt32();
                info.cnrl.validDevice = Get_Boolean(flags); flags >>= 1;
                info.cnrl.validNetType = Get_Boolean(flags);
                info.cnrl.netNameOffset = br.ReadUInt32();
                info.cnrl.deviceNameOffset = br.ReadUInt32();

                uint networkprovider = br.ReadUInt32();
                if (info.cnrl.validNetType)
                    info.cnrl.networkProviderType = (PROVIDER_TYPE)networkprovider;

                if (info.cnrl.netNameOffset > 0x14)
                {
                    info.cnrl.netNameOffsetUnicode = br.ReadUInt32();
                    info.cnrl.deviceNameOffsetUnicode = br.ReadUInt32();
                }

                info.cnrl.netName = Get_String(br, false, cnrl_pos + info.cnrl.netNameOffset);
                if (info.cnrl.validDevice)
                    info.cnrl.deviceName = Get_String(br, false, cnrl_pos + info.cnrl.deviceNameOffset);
                if (info.cnrl.netNameOffset > 0x14)
                {
                    info.cnrl.netNameUnicode = Get_String(br, true, cnrl_pos + info.cnrl.netNameOffsetUnicode);
                    info.cnrl.deviceNameUnicode = Get_String(br, true, cnrl_pos + info.cnrl.deviceNameOffsetUnicode);
                }
            }

            info.commonPathSuffix = Get_String(br, false, info_pos + info.commonPathSuffixOffset);

            if (info.linkInfoHeaderSize >= 0x24 && info.volumeIDAndLocalBasePath)
                info.localBasePathUnicode = Get_String(br, true, info_pos + info.localBasePathOffsetUnicode);
            if (info.linkInfoHeaderSize >= 0x24)
                info.commonPathSuffixUnicode = Get_String(br, true, info_pos + info.commonPathSuffixOffsetUnicode);

            return info;
        }
        private STRING_DATA Read_StringData(BinaryReader br)
        {
            STRING_DATA sdata = new STRING_DATA();

            if (lnk.header.linkFlags.hasName)
            {
                sdata.nameString = new NAME_STRING();
                sdata.nameString.countCharacters = br.ReadUInt16();
                sdata.nameString.value = Get_String(br, sdata.nameString.countCharacters, true);
            }
            if (lnk.header.linkFlags.hasRelativePath)
            {
                sdata.relativePath = new RELATIVE_PATH();
                sdata.relativePath.countCharacters = br.ReadUInt16();
                sdata.relativePath.value = Get_String(br, sdata.relativePath.countCharacters, true);
            }
            if (lnk.header.linkFlags.hasWorkingDir)
            {
                sdata.workingDir = new WORKING_DIR();
                sdata.workingDir.countCharacters = br.ReadUInt16();
                sdata.workingDir.value = Get_String(br, sdata.workingDir.countCharacters, true);
            }
            if (lnk.header.linkFlags.hasArguments)
            {
                sdata.commandLineArgs = new COMMAND_LINE_ARGUMENTS();
                sdata.commandLineArgs.countCharacters = br.ReadUInt16();
                sdata.commandLineArgs.value = Get_String(br, sdata.commandLineArgs.countCharacters, true);
            }
            if (lnk.header.linkFlags.hasIconLocation)
            {
                sdata.iconLocation = new ICON_LOCATION();
                sdata.iconLocation.countCharacters = br.ReadUInt16();
                sdata.iconLocation.value = Get_String(br, sdata.iconLocation.countCharacters, true);
            }

            return sdata;
        }
        private EXTRA_DATA Read_Extra(BinaryReader br)
        {
            EXTRA_DATA extra = new EXTRA_DATA();

            for (; ; )
            {
                uint size = br.ReadUInt32();
                if (size < 0x04)
                {
                    extra.terminal.terminal = size;
                    return extra;
                }

                uint sign = br.ReadUInt32();
                switch (sign)
                {
                    case 0xA0000001:
                        extra.environment.blockSize = size;
                        extra.environment.blockSignature = sign;
                        extra.environment.targetAnsi = Get_String(br, 260, false);
                        extra.environment.targetUnicode = Get_String(br, 260, true);
                        break;

                    case 0xA0000002:
                        CONSOLE_PROPS cp = new CONSOLE_PROPS();
                        cp.blockSize = size;
                        cp.blockSignature = sign;
                        cp.fillAttributes = (FILL_ATTRIBUTES)br.ReadUInt16();
                        cp.popupFillAttributes = br.ReadUInt16();
                        cp.screenBufferSizeX = br.ReadUInt16();
                        cp.screenBufferSizeY = br.ReadUInt16();
                        cp.windowSizeX = br.ReadUInt16();
                        cp.windowSizeY = br.ReadUInt16();
                        cp.windowOriginX = br.ReadUInt16();
                        cp.windowOriginY = br.ReadUInt16();
                        cp.unused1 = br.ReadUInt32();
                        cp.unused2 = br.ReadUInt32();
                        cp.fontSize = br.ReadUInt32();
                        cp.fontFamily = (FONT_FAMILY)br.ReadUInt32();
                        cp.fontWeight = br.ReadUInt32();
                        cp.faceName = Get_String(br, 32, true);
                        cp.cursorSize = br.ReadUInt32();
                        cp.fullScreen = br.ReadUInt32();
                        cp.quickEdit = br.ReadUInt32();
                        cp.insertMode = br.ReadUInt32();
                        cp.autoPosition = br.ReadUInt32();
                        cp.historyBufferSize = br.ReadUInt32();
                        cp.numberOfHistoryBuffer = br.ReadUInt32();
                        cp.historyNoDup = br.ReadUInt32();
                        cp.colorTable = new uint[0x10];
                        for (int i = 0; i < 0x10; i++)
                            cp.colorTable[i] = br.ReadUInt32();
                        extra.console = cp;
                        break;

                    case 0xA0000003:
                        extra.tracker.blockSize = size;
                        extra.tracker.blockSignature = sign;
                        extra.tracker.length = br.ReadUInt32();
                        extra.tracker.version = br.ReadUInt32();
                        extra.tracker.machineID = Get_String(br, 0x10, false);
                        extra.tracker.droid = br.ReadBytes(0x20);
                        extra.tracker.droidBirth = br.ReadBytes(0x20);
                        break;

                    case 0xA0000004:
                        extra.consoleFe.blockSize = size;
                        extra.consoleFe.blockSignature = sign;
                        extra.consoleFe.codePage = br.ReadUInt32();
                        break;

                    case 0xA0000005:
                        extra.specialFolder.blockSize = size;
                        extra.specialFolder.blockSignature = sign;
                        extra.specialFolder.specialFolderID = br.ReadUInt32();
                        extra.specialFolder.offset = br.ReadUInt32();
                        break;

                    case 0xA0000006:
                        extra.darwin.blockSize = size;
                        extra.darwin.blockSignature = sign;
                        extra.darwin.darwinDataAnsi = Get_String(br, 260, false);
                        extra.darwin.darwinDataUnicode = Get_String(br, 260, true);
                        break;

                    case 0xA0000007:
                        extra.iconEnvironment.blockSize = size;
                        extra.iconEnvironment.blockSignature = sign;
                        extra.iconEnvironment.targetAnsi = Get_String(br, 260, false);
                        extra.iconEnvironment.targetUnicode = Get_String(br, 260, true);
                        break;

                    case 0xA0000008:
                        extra.shim.blockSize = size;
                        extra.shim.blockSignature = sign;
                        extra.shim.layerName = Get_String(br, (int)extra.shim.blockSize - 8,  true);
                        break;

                    case 0xA0000009:
                        extra.propertyStore.blockSize = size;
                        extra.propertyStore.blockSignature = sign;
                        extra.propertyStore.propertyStore = null;
                        break;

                    case 0xA000000B:
                        extra.knownFolder.blockSize = size;
                        extra.knownFolder.blockSignature = sign;
                        extra.knownFolder.knownFolderID = br.ReadBytes(0x10);
                        extra.knownFolder.offset = br.ReadUInt32();
                        break;

                    case 0xA000000C:
                        extra.vistaIDList.blockSize = size;
                        extra.vistaIDList.blockSignature = sign;
                        extra.vistaIDList.idlist = Read_IDList(br);
                        break;
                }
            }
        }

        private bool Get_Boolean(uint value)
        {
            uint v = value & 1;
            return (v == 0 ? false : true);
        }
        private string Get_String(BinaryReader br, bool unicode, uint offset = 0)
        {
            if (offset != 0)
                br.BaseStream.Position = offset;

            string t = "";
            char c;
            for ( ; ; )
            {
                if (unicode)
                    c = Encoding.Unicode.GetChars(br.ReadBytes(2))[0];
                else
                    c = br.ReadChar();

                if (c == '\0')
                    break;

                t += c;
            }

            return t;
        }
        private string Get_String(BinaryReader br, int size, bool unicode)
        {
            if (!unicode)
                return new string(Encoding.Default.GetChars(br.ReadBytes(size)));
            else
                return new string(Encoding.Unicode.GetChars(br.ReadBytes(size * 2)));
        }

        public static bool Check(string fileIn)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(fileIn));

            uint hsize = br.ReadUInt32();
            if (hsize != 0x4C)
                return false;

            byte[] linkCLSID = br.ReadBytes(0x10);
            for (int i = 0; i < 0x10; i++)
                if (linkCLSID[i] != CLSID[i])
                    return false;

            br.Close();
            br = null;

            return true;
        }

        public string Path
        {
            get { return lnk.info.commonPathSuffix + lnk.info.localBasePath; }
        }
        public FILE_ATTRIBUTE_FLAGS FileAttribute
        {
            get { return lnk.header.fileAttributes; }
        }

        #region Structures
        public struct SHELL_LINK
        {
            public SHELL_LINK_HEADER header;
            public LINKTARGET_IDLIST idlist;    // Optional
            public LINKINFO info;               // Optional
            public STRING_DATA sdata;           // Optional
            public EXTRA_DATA extra;          // Optional
        }

        public struct SHELL_LINK_HEADER
        {
            public uint headerSize;             // Must be 0x4C
            public byte[] linkCLSID;            // Must be 00021401-0000-0000-C000-000000000046.
            public LINK_FLAGS linkFlags;
            public FILE_ATTRIBUTE_FLAGS fileAttributes;
            public FILE_TIME creationTime;
            public FILE_TIME accessTime;
            public FILE_TIME writeTime;
            public uint fileSize;
            public int iconIndex;
            public SHOW_COMMAND showCommand;
            public HOTKEYS_FLAGS hotKey;
            public ushort reserved1;            // Must be 00
            public uint reserved2;              // Must be 00
            public uint reserved3;              // Must be 00
        }
        public struct LINK_FLAGS  // 4 bytes
        {
            public bool hasLinkTargetIDList;
            public bool hasLinkInfo;
            public bool hasName;
            public bool hasRelativePath;
            public bool hasWorkingDir;
            public bool hasArguments;
            public bool hasIconLocation;
            public bool isUnicode;          // Should be true
            public bool forceNoLinkInfo;    // LinkInfo ignored
            public bool hasExpString;
            public bool runInSeparateProcess;
            public bool unused1;            // Must be ignored
            public bool hasDarwinID;
            public bool runAsUser;
            public bool hasExpIcon;
            public bool noPidlAlias;
            public bool unused2;            // Must be ignored
            public bool runWithShimLayer;
            public bool forceNoLinkTrack;
            public bool enableTargetMetadata;
            public bool disableLinkPathTracking;
            public bool diableKnownFolderTracking;
            public bool disableKnownFolderAlias;
            public bool allowLinkToLink;
            public bool unaliasOnSave;
            public bool preferEnvironmentPath;
            public bool keepLocalIDListForUNCTarget;
        }
        public struct FILE_ATTRIBUTE_FLAGS  // 4 bytes
        {
            public bool readOnly;
            public bool hidden;
            public bool system;
            public bool reserved1;  // Must be 0
            public bool directory;
            public bool archive;
            public bool reserved2;  // Must be 0
            public bool normal;
            public bool temporary;
            public bool sparse_file;
            public bool reparse_point;
            public bool compressed;
            public bool offline;
            public bool not_content_indexed;
            public bool encrypted;
        }
        public struct FILE_TIME   // 8 bytes
        {
            // FROM: http://msdn.microsoft.com/en-us/library/cc230273%28v=prot.10%29.aspx
            // "The FILETIME structure is a 64-bit value that represents the number of
            // 100-nanosecond intervals that have elapsed since January 1, 1601, Coordinated Universal Time (UTC)."
            
            //uint dwLowDateTime;
            //uint dwHightDateTime;
            public ulong dateTime;
        }
        public enum SHOW_COMMAND : uint
        {
            SW_SHOWNORMAL = 0x01,       // Default
            SW_SHOWMAXIMIZED = 0x03,
            SW_SHOWMINNOACTIVE = 0x07
        }
        public struct HOTKEYS_FLAGS   // 2 bytes
        {
            public LOW_BYTE low;
            public HIGH_BYTE hight;

            public enum LOW_BYTE : byte
            {
                K_0 = 0x30,
                K_1 = 0x31,
                K_2 = 0x32,
                K_3 = 0x33,
                K_4 = 0x34,
                K_5 = 0x35,
                K_6 = 0x36,
                K_7 = 0x37,
                K_8 = 0x38,
                K_9 = 0x39,
                K_A = 0x41,
                K_B = 0x42,
                K_C = 0x43,
                K_D = 0x44,
                K_E = 0x45,
                K_F = 0x46,
                K_G = 0x47,
                K_H = 0x48,
                K_I = 0x49,
                K_J = 0x4A,
                K_K = 0x4B,
                K_L = 0x4C,
                K_M = 0x4D,
                K_N = 0x4E,
                K_O = 0x4F,
                K_P = 0x50,
                K_Q = 0x51,
                K_R = 0x52,
                K_S = 0x53,
                K_T = 0x54,
                K_U = 0x55,
                K_V = 0x56,
                K_W = 0x57,
                K_X = 0x58,
                K_Y = 0x59,
                K_Z = 0x5A,
                VK_F1 = 0x70,
                VK_F2 = 0x71,
                VK_F3 = 0x72,
                VK_F4 = 0x73,
                VK_F5 = 0x74,
                VK_F6 = 0x75,
                VK_F7 = 0x76,
                VK_F8 = 0x77,
                VK_F9 = 0x78,
                VK_F10 = 0x79,
                VK_F11 = 0x7A,
                VK_F12 = 0x7B,
                VK_F13 = 0x7C,
                VK_F14 = 0x7D,
                VK_F15 = 0x7E,
                VK_F16 = 0x7F,
                VK_F17 = 0x80,
                VK_F18 = 0x81,
                VK_F19 = 0x82,
                VK_F20 = 0x83,
                VK_F21 = 0x84,
                VK_F22 = 0x85,
                VK_F23 = 0x86,
                VK_F24 = 0x87,
                VK_NUMLOCK = 0x90,
                VK_SCROLL = 0x91
            }
            public enum HIGH_BYTE : byte
            {
                HOTKEYF_SHIFT = 0x01,
                HOTKEYF_CONTROL = 0x02,
                HOTKEYF_ALT = 0x04
            }
        }

        public struct LINKTARGET_IDLIST
        {
            // The presence of this optional structure is
            // specified by the HasLinkTargetIDList bit
            public ushort IDListSize;
            public IDLIST IDList;
        }
        public struct IDLIST
        {
            public List<ITEM_IDLIST> itemIDList;
            public ushort terminalID;       // Must be 0000
        }
        public struct ITEM_IDLIST
        {
            public ushort itemIDSize;
            public byte[] data;
        }

        public struct LINKINFO
        {
            public uint linkInfoSize;
            public uint linkInfoHeaderSize; // 0x1C->no optional fields; >= 0x24 optional fields

            // Flags, in total 4 bytes
            public bool volumeIDAndLocalBasePath;
            public bool commonNetworkRelativeLinkAndPathSuffix;

            // Offsets
            public uint volumeIDOffset;
            public uint localBasePathOffset;
            public uint commonNetworkRelativeLinkOffset;
            public uint commonPathSuffixOffset;
            public uint localBasePathOffsetUnicode;
            public uint commonPathSuffixOffsetUnicode;

            public VOLUMEID volumeID;
            public string localBasePath;    // NULL-terminated
            public COMMON_NETWORK_RELATIVE_LINK cnrl;
            public string commonPathSuffix; // NULL-terminated
            public string localBasePathUnicode; // UNICODE & NULL-terminated
            public string commonPathSuffixUnicode;  // UNICODE & NULL-terminated
        }
        public struct VOLUMEID
        {
            public uint volumeIDSize;   // MUST be > 0x10
            public DRIVE_TYPE driveType;
            public uint driveSerialNumber;
            public uint volumeLabelOffset;  // NULL-terminated
            public uint volumeLabelOffsetUnicode;   // UNICODE & NULL-terminated
            public string data;
        }
        public enum DRIVE_TYPE : uint
        {
            DRIVE_UNKNOWN = 0x00,
            DRIVE_NO_ROOT_DIR = 0x01,
            DRIVE_REMOVABLE = 0x02,
            DRIVE_FIXED = 0x03,
            DRIVE_REMOTE = 0x04,
            DRIVE_CDROM = 0x05,
            DRIVE_RAMDISK = 0x06
        }
        public struct COMMON_NETWORK_RELATIVE_LINK
        {
            public uint cnrl_size;

            // Flags - 4 bytes
            public bool validDevice;
            public bool validNetType;
            
            // Offsets
            public uint netNameOffset;
            public uint deviceNameOffset;
            public PROVIDER_TYPE networkProviderType;
            public uint netNameOffsetUnicode;
            public uint deviceNameOffsetUnicode;

            public string netName;      // Null-terminated
            public string deviceName;   // Null-terminated
            public string netNameUnicode;   // Unicode & Null-terminated
            public string deviceNameUnicode;    // Unicode & Null-terminated
        }
        public enum PROVIDER_TYPE : uint
        {
            WNNC_NET_AVID = 0x001A0000,
            WNNC_NET_DOCUSPACE = 0x001B0000,
            WNNC_NET_MANGOSOFT = 0x001C0000,
            WNNC_NET_SERNET = 0x001D0000,
            WNNC_NET_RIVERFRONT1 = 0x001E0000,
            WNNC_NET_RIVERFRONT2 = 0x001F0000,
            WNNC_NET_DECORB = 0x00200000,
            WNNC_NET_PROTSTOR = 0x00210000,
            WNNC_NET_FJ_REDIR = 0x00220000,
            WNNC_NET_DISTINCT = 0x00230000,
            WNNC_NET_TWINS = 0x00240000,
            WNNC_NET_RDR2SAMPLE = 0x00250000,
            WNNC_NET_CSC = 0x00260000,
            WNNC_NET_3IN1 = 0x00270000,
            WNNC_NET_EXTENDNET = 0x00290000,
            WNNC_NET_STAC = 0x002A0000,
            WNNC_NET_FOXBAT = 0x002B0000,
            WNNC_NET_YAHOO = 0x002C0000,
            WNNC_NET_EXIFS = 0x002D0000,
            WNNC_NET_DAV = 0x002E0000,
            WNNC_NET_KNOWARE = 0x002F0000,
            WNNC_NET_OBJECT_DIRE = 0x0030000,
            WNNC_NET_MASFAX = 0x00310000,
            WNNC_NET_HOB_NFS = 0x00320000,
            WNNC_NET_SHIVA = 0x00330000,
            WNNC_NET_IBMAL = 0x00340000,
            WNNC_NET_LOCK = 0x00350000,
            WNNC_NET_TERMSRV = 0x00360000,
            WNNC_NET_SRT = 0x00370000,
            WNNC_NET_QUINCY = 0x00380000,
            WNNC_NET_OPENAFS = 0x00390000,
            WNNC_NET_AVID1 = 0x003A0000,
            WNNC_NET_DFS = 0x003B0000,
            WNNC_NET_KWNP = 0x003C0000,
            WNNC_NET_ZENWORKS = 0x003D0000,
            WNNC_NET_DRIVEONWEB = 0x003E0000,
            WNNC_NET_VMWARE = 0x003F0000,
            WNNC_NET_RSFX = 0x00400000,
            WNNC_NET_MFILES = 0x00410000,
            WNNC_NET_MS_NFS = 0x00420000,
            WNNC_NET_GOOGLE = 0x00430000
        }

        public struct STRING_DATA
        {
            public NAME_STRING nameString;
            public RELATIVE_PATH relativePath;
            public WORKING_DIR workingDir;
            public COMMAND_LINE_ARGUMENTS commandLineArgs;
            public ICON_LOCATION iconLocation;
        }
        public struct NAME_STRING
        {
            public ushort countCharacters;
            public string value;
        }
        public struct RELATIVE_PATH
        {
            public ushort countCharacters;
            public string value;
        }
        public struct WORKING_DIR
        {
            public ushort countCharacters;
            public string value;
        }
        public struct COMMAND_LINE_ARGUMENTS
        {
            public ushort countCharacters;
            public string value;
        }
        public struct ICON_LOCATION
        {
            public ushort countCharacters;
            public string value;
        }

        public struct EXTRA_DATA
        {
            public CONSOLE_PROPS console;
            public CONSOLE_FE_PROPS consoleFe;
            public DRAWIN_PROPS darwin;
            public ENVIRONMENT_PROPS environment;
            public ICON_ENVIRONMENT_PROPS iconEnvironment;
            public KNOWN_FOLDER_PROPS knownFolder;
            public PROPERTY_STORE_PROPS propertyStore;
            public SHIM_PROPS shim;
            public SPECIAL_FOLDER_PROPS specialFolder;
            public TRACKER_PROPS tracker;
            public VISTA_AND_ABOVE_IDLIST_PROPS vistaIDList;
            public TERMINAL_BLOCK terminal;
        }
        public struct CONSOLE_PROPS
        {
            public uint blockSize;              // MUST be 0xCC
            public uint blockSignature;         // MUST be 0xA0000002
            public FILL_ATTRIBUTES fillAttributes;
            public ushort popupFillAttributes;
            public ushort screenBufferSizeX;
            public ushort screenBufferSizeY;
            public ushort windowSizeX;
            public ushort windowSizeY;
            public ushort windowOriginX;
            public ushort windowOriginY;
            public uint unused1;
            public uint unused2;
            public uint fontSize;
            public FONT_FAMILY fontFamily;
            public uint fontWeight; // More than 700 bold font
            public string faceName; // 64 bytes UNICODE
            public uint cursorSize;
            public uint fullScreen; // Different to 0 -> Full-screen
            public uint quickEdit;  // Different to 0 -> ON
            public uint insertMode; // Different to 0 -> ON
            public uint autoPosition; // Different to 0 -> Auto
            public uint historyBufferSize;
            public uint numberOfHistoryBuffer;
            public uint historyNoDup;   // Different to 0 -> Allowed
            public uint[] colorTable;    // RGB 32-bits colors
        }
        public enum FILL_ATTRIBUTES : ushort
        {
            FOREGROUND_BLUE = 0x01,
            FOREGROUND_GREEN = 0x02,
            FOREGROUND_RED = 0x04,
            FOREGROUND_INTENSITY = 0x08,
            BACKGROUND_BLUE = 0x10,
            BACKGROUND_GREEN = 0x20,
            BACKGROUND_RED = 0x40,
            BACKGROUND_INTENSITY = 0x80
        }
        public enum FONT_FAMILY : uint
        {
            FF_DONTCARE = 0x00,
            FF_ROMAN = 0x10,
            FF_SWISS = 0x20,
            FF_MODERN = 0x30,
            FF_SCRIPT = 0x40,
            FF_DECORATIVE = 0x50
        }
        public struct CONSOLE_FE_PROPS
        {
            public uint blockSize;  // MUST be 0xC
            public uint blockSignature; // MUST be 0xA0000004
            public uint codePage;
        }
        public struct DRAWIN_PROPS
        {
            public uint blockSize;  // Must be 0x314
            public uint blockSignature; // Must be 0xA0000006
            public string darwinDataAnsi; // 260 bytes, Null-terminated
            public string darwinDataUnicode; // 520 bytes Unicode, Null-terminated
        }
        public struct ENVIRONMENT_PROPS
        {
            public uint blockSize;  // Must be 0x314
            public uint blockSignature; // Must be 0xA0000001
            public string targetAnsi;   // 260 bytes, null-terminated
            public string targetUnicode;    // 520 bytes, null-terminated, unicode
        }
        public struct ICON_ENVIRONMENT_PROPS
        {
            public uint blockSize;      // Must be 0x314
            public uint blockSignature; // Must be 0xA0000007
            public string targetAnsi;   // 260 bytes, null-terminated
            public string targetUnicode;    // 520 bytes, null-terminated, unicode
        }
        public struct KNOWN_FOLDER_PROPS
        {
            public uint blockSize;  // Must be 0x1C
            public uint blockSignature; // Must be 0xA000000B
            public byte[] knownFolderID;
            public uint offset;
        }
        public struct PROPERTY_STORE_PROPS
        {
            public uint blockSize;  // Must be >= 0x0C
            public uint blockSignature; // Mst be 0xA0000009
            public byte[] propertyStore;
        }
        public struct SHIM_PROPS
        {
            public uint blockSize;      // Must be >= 0x88
            public uint blockSignature; // Must be 0xA0000008
            public string layerName;    // Unicode
        }
        public struct SPECIAL_FOLDER_PROPS
        {
            public uint blockSize;      // Must be 0x10
            public uint blockSignature; // Must be 0xA0000005
            public uint specialFolderID;
            public uint offset;
        }
        public struct TRACKER_PROPS
        {
            public uint blockSize;      // Must be 0x60
            public uint blockSignature; // Must be 0xA0000003
            public uint length;         // Must be >= 0x58
            public uint version;        // Must be 0x00
            public string machineID;
            public byte[] droid;        // Two GUID
            public byte[] droidBirth;   // Two GUID
        }
        public struct VISTA_AND_ABOVE_IDLIST_PROPS
        {
            public uint blockSize;      // Must be >= 0x0A
            public uint blockSignature; // Must be 0xA000000C
            public IDLIST idlist;
        }
        public struct TERMINAL_BLOCK
        {
            public uint terminal;  // Less than 0x04
        }

        static byte[] CLSID = {
	        0x01, 0x14, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0x00, 0x00,
	        0x00, 0x00, 0x00, 0x46
        };
        #endregion

    }
}
