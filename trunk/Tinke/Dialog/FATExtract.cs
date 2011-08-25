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
using PluginInterface;

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

            this.file = file;
            fat = new FAT();

            Read_All();
        }

        #region Read values
        private void Read_All()
        {
            Read_NumFiles();
        }
        private void Read_NumFiles()
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));

            br.BaseStream.Position = (long)numericNumOffset.Value;

            try
            {
                if (checkNumBigEndian.Checked)
                {
                    fat.num_files = BitConverter.ToUInt32(br.ReadBytes((int)numericNumLen.Value).Reverse().ToArray(), 0);
                }
                else
                {
                    fat.num_files = BitConverter.ToUInt32(br.ReadBytes((int)numericNumLen.Value), 0);
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
                uint offsetLen;
                if (radioOffsetEnd.Checked || radioOffsetStart.Checked)
                    offsetLen = 0x04;
                else
                    offsetLen = 0x08;

                if (radioOffsetEnd.Checked)
                {
                    fat.num_files = (uint)(numericRelativeOffset.Value - numericOffsetStart.Value) / offsetLen;
                }
                else
                {
                    if (checkNumBigEndian.Checked)
                    {
                        fat.num_files = BitConverter.ToUInt32(br.ReadBytes(4).Reverse().ToArray(), 0) / offsetLen;
                    }
                    else
                    {
                        fat.num_files = (br.ReadUInt32() / offsetLen);
                    }
                }
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
                fat.files = new List<Archivo>();

                for (int i = 0; i < fat.num_files; i++)
                {
                    Archivo currFile = new Archivo();
                    currFile.name = "File " + i.ToString();

                    // Gets the offset and size
                    if (radioOffsetStart.Checked)
                    {
                        // Start offset
                        if (checkOffsetBigEndian.Checked)
                            currFile.offset = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value).Reverse().ToArray(), 0) +
                                (uint)numericRelativeOffset.Value;
                        else
                            currFile.offset = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value), 0) +
                                (uint)numericRelativeOffset.Value;

                        // Start offset of the next file
                        uint nextOffset;
                        if (i + 1 == fat.num_files)
                            nextOffset = (uint)br.BaseStream.Length;
                        else if (checkOffsetBigEndian.Checked)
                            nextOffset = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value).Reverse().ToArray(), 0) + 
                                (uint)numericRelativeOffset.Value;
                        else
                            nextOffset = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value), 0) +
                                (uint)numericRelativeOffset.Value;

                        currFile.size = nextOffset - currFile.offset;
                        br.BaseStream.Position -= (long)numericOffsetLen.Value;
                    }
                    else if (radioOffsetStartEnd.Checked)
                    {
                        // Start offset
                        if (checkOffsetBigEndian.Checked)
                            currFile.offset = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value).Reverse().ToArray(), 0) +
                                (uint)numericRelativeOffset.Value;
                        else
                            currFile.offset = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value), 0) +
                                (uint)numericRelativeOffset.Value;

                        // End offset
                        uint endOffset;
                        if (checkOffsetBigEndian.Checked)
                            endOffset = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value).Reverse().ToArray(), 0) + 
                                (uint)numericRelativeOffset.Value;
                        else
                            endOffset = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value), 0) +
                                (uint)numericRelativeOffset.Value;

                        currFile.size = endOffset - currFile.offset;
                    }
                    else if (radioOffsetStartSize.Checked)
                    {
                        // Start offset
                        if (checkOffsetBigEndian.Checked)
                            currFile.offset = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value).Reverse().ToArray(), 0) +
                                (uint)numericRelativeOffset.Value;
                        else
                            currFile.offset = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value), 0) +
                                (uint)numericRelativeOffset.Value;

                        // Size
                        uint size;
                        if (checkOffsetBigEndian.Checked)
                            size = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value).Reverse().ToArray(), 0);
                        else
                            size = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value), 0);

                        currFile.size = size;
                    }
                    else if (radioOffsetEnd.Checked)
                    {
                        if (i == 0)
                            currFile.offset = (uint)numericRelativeOffset.Value;
                        else
                            currFile.offset = (uint)(fat.files[i - 1].offset + fat.files[i - 1].size);

                        // End offset
                        uint endOffset;
                        if (checkOffsetBigEndian.Checked)
                            endOffset = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value).Reverse().ToArray(), 0) +
                                (uint)numericRelativeOffset.Value;
                        else
                            endOffset = BitConverter.ToUInt32(br.ReadBytes((int)numericOffsetLen.Value), 0) +
                                (uint)numericRelativeOffset.Value;

                        currFile.size = endOffset - currFile.offset;
                    }

                    if (currFile.size >= br.BaseStream.Length || currFile.size + currFile.offset > br.BaseStream.Length)
                        currFile.size = 0;

                    fat.files.Add(currFile);
                }

                for (int i = 0; i < fat.num_files; i++)
                    listBoxFiles.Items.Add("File " + i.ToString());
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

        private void btnAccept_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void btnHex_Click(object sender, EventArgs e)
        {
            VisorHex hex = new VisorHex(file, fat.files[listBoxFiles.SelectedIndex].offset, fat.files[listBoxFiles.SelectedIndex].size);
            hex.Show();
        }

        public Carpeta Files
        {
            get
            {
                if (DialogResult != System.Windows.Forms.DialogResult.OK)
                    return new Carpeta();

                // Save all extracted files
                BinaryReader br = new BinaryReader(File.OpenRead(file));
                for (int i = 0; i < fat.num_files; i++)
                {
                    Archivo currFile = fat.files[i];
                    if (folder == "")
                        currFile.path = Path.GetTempFileName();
                    else
                        currFile.path = folder + Path.DirectorySeparatorChar + currFile.name;
                    currFile.offset = 0x00;

                    br.BaseStream.Position = fat.files[i].offset;
                    File.WriteAllBytes(currFile.path, br.ReadBytes((int)currFile.size));

                    fat.files[i] = currFile;
                }
                br.Close();

                Carpeta newFolder = new Carpeta();
                newFolder.files = new List<Archivo>();
                newFolder.files.AddRange(fat.files);
                return newFolder;
            }
        }
        public String TempFolder
        {
            set 
            { 
                folder = value + Path.DirectorySeparatorChar + new FileInfo(file).Name;
                Directory.CreateDirectory(folder);
            }
        }

        public struct FAT
        {
            public uint num_files;
            public List<Archivo> files;
        }
    }
}
