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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml.Linq;
using Ekona;

namespace Tinke.Dialog
{
    public partial class FATExtract : Form
    {
        string file;
        FAT fat;
        String folder;

        public FATExtract(string file)
        {
            InitializeComponent();
            ReadLanguage();

            this.file = file;
            fat = new FAT();

            Read_NumFiles();
        }
        private void ReadLanguage()
        {
            try
            {
                XElement xml = Tools.Helper.GetTranslation("Dialog");

                this.Text = xml.Element("S06").Value;
                groupOptions.Text = xml.Element("S07").Value;
                groupNumFiles.Text = xml.Element("S08").Value;
                label2.Text = xml.Element("S09").Value;
                label3.Text = xml.Element("S0A").Value;
                checkNumBigEndian.Text = xml.Element("S0B").Value;
                label7.Text = xml.Element("S0C").Value;
                btnNumCalculate.Text = xml.Element("S0D").Value;
                groupOffset.Text = xml.Element("S0E").Value;
                label4.Text = xml.Element("S0F").Value;
                label6.Text = xml.Element("S10").Value;
                checkOffsetBigEndian.Text = xml.Element("S11").Value;
                groupOffsetType.Text = xml.Element("S12").Value;
                radioOffsetStart.Text = xml.Element("S13").Value;
                radioOffsetStartEnd.Text = xml.Element("S14").Value;
                radioOffsetStartSize.Text = xml.Element("S15").Value;
                radioOffsetEnd.Text = xml.Element("S16").Value;
                groupOffsetRelative.Text = xml.Element("S17").Value;
                radioRelativeOffset.Text = xml.Element("S18").Value;
                radioRelativeFirstFile.Text = xml.Element("S19").Value;
                btnOffsetCalculate.Text = xml.Element("S1A").Value;
                btnHex.Text = xml.Element("S1B").Value;
                btnAccept.Text = xml.Element("S1C").Value;
            }
            catch { throw new NotImplementedException("There was an error reading the language file"); }
        }

        #region Read values
        private void Read_NumFiles()
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            br.BaseStream.Position = (long)numericNumOffset.Value;

            try
            {
                if (checkNumBigEndian.Checked)
                {
                    switch ((int)numericNumLen.Value)
                    {
                        case 1:
                            fat.num_files = br.ReadByte();
                            break;
                        case 2:
                            fat.num_files = BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0);
                            break;
                        case 4:
                            fat.num_files = BitConverter.ToUInt32(br.ReadBytes(4).Reverse().ToArray(), 0);
                            break;
                        case 8:
                            fat.num_files = (uint)BitConverter.ToUInt64(br.ReadBytes(8).Reverse().ToArray(), 0);
                            break;
                    }
                }
                else
                {
                    switch ((int)numericNumLen.Value)
                    {
                        case 1:
                            fat.num_files = br.ReadByte();
                            break;
                        case 2:
                            fat.num_files = br.ReadUInt16();
                            break;
                        case 4:
                            fat.num_files = br.ReadUInt32();
                            break;
                        case 8:
                            fat.num_files = (uint)br.ReadUInt64();
                            break;
                    }
                }
            }
            catch { fat.num_files = 0; numericNumFiles.Value = 0; }
            finally { br.Close(); }

            numericNumFiles.Value = fat.num_files;

            if (fat.num_files == 0)
                Read_NumFiles2();
        }
        private void Read_NumFiles2()
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = (long)numericOffsetStart.Value;

            try
            {
                uint offsetLen = (uint)numericOffsetLen.Value;
                if (!radioOffsetEnd.Checked && !radioOffsetStart.Checked)
                    offsetLen *= 2;

                    fat.num_files = (uint)(Get_OffsetValue(ref br) - numericOffsetStart.Value) / offsetLen;
            }
            catch { fat.num_files = 0; numericNumFiles.Value = 0; }
            finally { br.Close(); }

            numericNumFiles.Value = fat.num_files;
        }
        private void numericNumFiles_ValueChanged(object sender, EventArgs e)
        {
            fat.num_files = (uint)numericNumFiles.Value;
        }
        private void numericNumLen_ValueChanged(object sender, EventArgs e)
        {
            if (numericNumLen.Value == 1 || numericNumLen.Value == 2 || numericNumLen.Value == 4 || numericNumLen.Value == 8)
                Read_NumFiles();
        }
        private void btnNumCalculate_Click(object sender, EventArgs e)
        {
            Read_NumFiles2();
        }
        private void btnOffsetCalculate_Click(object sender, EventArgs e)
        {
            listBoxFiles.Items.Clear(); // Remove old values

            BinaryReader br = new BinaryReader(File.OpenRead(file));

            try
            {
                // Read all offset
                br.BaseStream.Position = (long)numericOffsetStart.Value;
                fat.files = new List<sFile>();
                uint omit = 0;

                for (int i = 0; i < fat.num_files; i++)
                {
                    sFile currFile = new sFile();
                    currFile.name = "File " + i.ToString(); 
                    currFile.path = file;

                    // Gets the offset and size
                    if (radioOffsetStart.Checked)
                    {
                        // Start offset
                        currFile.offset = Get_OffsetValue(ref br) + (uint)numericRelativeOffset.Value;

                        // Start offset of the next file
                        uint nextOffset;
                        if (i + 1 == fat.num_files)
                            nextOffset = (uint)br.BaseStream.Length;
                        else
                            nextOffset = Get_OffsetValue(ref br) + (uint)numericRelativeOffset.Value;

                        currFile.size = nextOffset - currFile.offset;
                        br.BaseStream.Position -= (long)numericOffsetLen.Value;
                    }
                    else if (radioOffsetStartEnd.Checked)
                    {
                        // Start offset
                        currFile.offset = Get_OffsetValue(ref br) + (uint)numericRelativeOffset.Value;

                        // End offset
                        uint endOffset = Get_OffsetValue(ref br) + (uint)numericRelativeOffset.Value;

                        currFile.size = endOffset - currFile.offset;
                    }
                    else if (radioOffsetStartSize.Checked)
                    {
                        // Start offset
                        currFile.offset = Get_OffsetValue(ref br) + (uint)numericRelativeOffset.Value;

                        // Size
                        currFile.size = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value), 0);
                    }
                    else if (radioOffsetEnd.Checked)
                    {
                        if (i == 0)
                            currFile.offset = (uint)numericRelativeOffset.Value;
                        else
                            currFile.offset = (uint)(fat.files[i - 1].offset + fat.files[i - 1].size);

                        // End offset
                        uint endOffset = Get_OffsetValue(ref br) + (uint)numericRelativeOffset.Value;

                        currFile.size = endOffset - currFile.offset;
                    }

                    if (currFile.size >= br.BaseStream.Length || currFile.size + currFile.offset > br.BaseStream.Length)
                        currFile.size = 0;

                    if (checkOmitZero.Checked && currFile.size == 0)
                    {
                        omit++;
                        continue;
                    }

                    // Get the extension
                    long currPos = br.BaseStream.Position;
                    br.BaseStream.Position = currFile.offset;
                    char[] ext;
                    if (currFile.size < 4)
                        ext = Encoding.ASCII.GetChars(br.ReadBytes((int)currFile.size));
                    else
                        ext = Encoding.ASCII.GetChars(br.ReadBytes(4));

                    String extS = ".";
                    for (int s = 0; s < ext.Length; s++)
                        if (Char.IsLetterOrDigit(ext[s]) || ext[s] == 0x20)
                            extS += ext[s];

                    if (extS != "." && extS.Length == 5 && currFile.size >= 4)
                        currFile.name += extS;
                    else
                        currFile.name += ".bin";
                    br.BaseStream.Position = currPos;

                    fat.files.Add(currFile);
                }

                fat.num_files -= omit;

                for (int i = 0; i < fat.num_files; i++)
                    listBoxFiles.Items.Add(fat.files[i].name);
            }
            catch { }
            finally { br.Close(); }
        }
        private void radioRelativeFirstFile_CheckedChanged(object sender, EventArgs e)
        {
            numericRelativeOffset.ReadOnly = radioRelativeFirstFile.Checked;

            if (radioRelativeFirstFile.Checked)
                if (radioOffsetStart.Checked || radioOffsetEnd.Checked)
                    numericRelativeOffset.Value = (fat.num_files * numericOffsetLen.Value) + numericOffsetStart.Value;
                else
                    numericRelativeOffset.Value = (fat.num_files * numericOffsetLen.Value * 2) + numericOffsetStart.Value;
        }
        #endregion

        private uint Get_OffsetValue(ref BinaryReader br)
        {
            if (checkOffsetBigEndian.Checked)
            {
                switch ((int)numericOffsetLen.Value)
                {
                    case 1:
                        return (uint)(br.ReadByte() * numericOffsetMult.Value);
                    case 2:
                        return BitConverter.ToUInt16(br.ReadBytes(2).Reverse().ToArray(), 0) * (uint)numericOffsetMult.Value;
                    case 4:
                        return BitConverter.ToUInt32(br.ReadBytes(4).Reverse().ToArray(), 0) * (uint)numericOffsetMult.Value;
                    case 8:
                        return (uint)(BitConverter.ToUInt64(br.ReadBytes(8).Reverse().ToArray(), 0) * numericOffsetMult.Value);
                }
            }
            else
            {
                switch ((int)numericOffsetLen.Value)
                {
                    case 1:
                        return br.ReadByte() * (uint)numericOffsetMult.Value;
                    case 2:
                        return br.ReadUInt16() * (uint)numericOffsetMult.Value;
                    case 4:
                        return br.ReadUInt32() * (uint)numericOffsetMult.Value;
                    case 8:
                        return (uint)(br.ReadUInt64() * numericOffsetMult.Value);
                }
            }

            return 0;
        }

        private void btnAddOffset_Click(object sender, EventArgs e)
        {
            if (fat.files == null)
                fat.files = new List<sFile>();

            sFile newFile = new sFile();
            newFile.path = file;
            newFile.name = "File " + (fat.files.Count - 1).ToString();
            newFile.offset = (uint)(numOffset.Value * numericOffsetMult.Value);
            if (numSize.Value == 0)
                newFile.size = (uint)(new FileInfo(file).Length - newFile.offset);
            else
                newFile.size = (uint)numSize.Value;


            // Get the extension
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            br.BaseStream.Position = newFile.offset;
            char[] ext;
            if (newFile.size < 4)
                ext = Encoding.ASCII.GetChars(br.ReadBytes((int)newFile.size));
            else
                ext = Encoding.ASCII.GetChars(br.ReadBytes(4));

            String extS = ".";
            for (int s = 0; s < ext.Length; s++)
                if (Char.IsLetterOrDigit(ext[s]) || ext[s] == 0x20)
                    extS += ext[s];

            if (extS != "." && extS.Length == 5 && newFile.size >= 4)
                newFile.name += extS;
            else
                newFile.name += ".bin";

            fat.files.Add(newFile);
            listBoxFiles.Items.Add(newFile.name);
        }
        private void btnAccept_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnHex_Click(object sender, EventArgs e)
        {
            var selectedFile = fat.files[listBoxFiles.SelectedIndex];
            if (Type.GetType("Mono.Runtime") == null)
                new VisorHex(selectedFile).Show();
            else
                new VisorHexBasic(selectedFile).Show();
        }

        public sFolder Files
        {
            get
            {
                if (DialogResult != System.Windows.Forms.DialogResult.OK)
                    return new sFolder();

                sFolder newFolder = new sFolder();
                newFolder.files = new List<sFile>();
                newFolder.files.AddRange(fat.files);
                return newFolder;
            }
        }
        public String TempFolder
        {
            set
            {
                folder = value + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(file);
                Directory.CreateDirectory(folder);
            }
        }

        public struct FAT
        {
            public uint num_files;
            public List<sFile> files;
        }


    }
}
