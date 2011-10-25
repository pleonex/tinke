using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Be.Windows.Forms;
using PluginInterface;

namespace Tinke
{
    public partial class VisorHex : Form
    {
        sFile file;
        bool fileEdited;
        IByteCharConverter bcc;
        string hexFile;
        bool allowEdit;

        public VisorHex(sFile file, bool edit)
        {
            InitializeComponent();
            this.Text = Tools.Helper.ObtenerTraduccion("Sistema", "S41");
            saveToolStripMenuItem.Enabled = edit;

            this.file = file;
            allowEdit = edit;

            hexFile = Path.GetDirectoryName(file.path) + Path.DirectorySeparatorChar + "hex_" + Path.GetRandomFileName();
            if (new FileInfo(file.path).Length != file.size)
            {
                BinaryReader br = new BinaryReader(File.OpenRead(file.path));
                br.BaseStream.Position = file.offset;
                File.WriteAllBytes(hexFile, br.ReadBytes((int)file.size));
                br.Close();
            }
            else if (!edit)
                hexFile = file.path;
            else
                File.Copy(file.path, hexFile, true);

            hexBox1.ByteProvider = new DynamicFileByteProvider(hexFile, !edit); 
            encodingCombo.SelectedIndex = 0;
         }
        private void VisorHex_FormClosed(object sender, FormClosedEventArgs e)
        {
            hexBox1.Dispose();
            ((DynamicFileByteProvider)hexBox1.ByteProvider).Dispose();
            if (File.Exists(hexFile) && allowEdit)
                File.Delete(hexFile);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!allowEdit)
                return;

            fileEdited = true;
            ((DynamicFileByteProvider)hexBox1.ByteProvider).ApplyChanges();
            String newFilePath = Path.GetDirectoryName(hexFile) + Path.DirectorySeparatorChar + "new_" + Path.GetRandomFileName();
            File.Copy(hexFile, newFilePath, true);
            file.offset = 0x00;
            file.path = newFilePath;
            file.size = (uint)new FileInfo(newFilePath).Length;
        }

        public Boolean Edited
        {
            get { return fileEdited; }
        }
        public sFile NewFile
        {
            get { return file; }
        }

        private void VisorHex_Resize(object sender, EventArgs e)
        {
            hexBox1.Height = this.Height - 83;
        }

        private void comboBoxEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (encodingCombo.SelectedIndex == 0)
                bcc = new DefaultByteCharConverter();
            else
                bcc = new ByteCharConveter(encodingCombo.Text);

            hexBox1.ByteCharConverter = bcc;
        }

        private void numericOffset_ValueChanged(object sender, EventArgs e)
        {
            hexBox1.Select(Convert.ToInt64(toolStripTextBox1.Text, 16), 1);
        }
        private void hexBox1_SelectionLengthChanged(object sender, EventArgs e)
        {
            toolStripSelect.Text = String.Format("Selected at  0x{0}  with length  0x{1}",
                hexBox1.SelectionStart.ToString("x"), hexBox1.SelectionLength.ToString("x"));
        }
        private void goToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            long size = 0;
            long start = Convert.ToInt64(startOffsetSelect.Text, 16);
            if (relativeToolStripMenuItem.Checked)
                size = Convert.ToInt64(endOffsetSelect.Text, 16);
            else
                size = Convert.ToInt64(endOffsetSelect.Text, 16) - start;

            hexBox1.Select(start, size);
        }

        private void rawBytesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            List<byte> search = new List<byte>();
            for (int i = 0; i < toolStripSearchBox.Text.Length; i += 2)
                search.Add(Convert.ToByte(toolStripSearchBox.Text.Substring(i, 2), 16));

            hexBox1.Find(search.ToArray(), hexBox1.SelectionStart + hexBox1.SelectionLength);
            this.Cursor = Cursors.Default;
        }
        private void shiftjisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            byte[] search = Encoding.GetEncoding("shift_jis").GetBytes(toolStripSearchBox.Text.ToCharArray());
            hexBox1.Find(search, hexBox1.SelectionStart + hexBox1.SelectionLength);
            this.Cursor = Cursors.Default;
        }
        private void defaultCharsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            byte[] search = Encoding.Default.GetBytes(toolStripSearchBox.Text.ToCharArray());
            hexBox1.Find(search, hexBox1.SelectionStart + hexBox1.SelectionLength);
            this.Cursor = Cursors.Default;
        }
        private void unicodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            byte[] search = Encoding.Unicode.GetBytes(toolStripSearchBox.Text.ToCharArray());
            hexBox1.Find(search, hexBox1.SelectionStart + hexBox1.SelectionLength);
            this.Cursor = Cursors.Default;
        }
        private void unicodeBigEndianToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            byte[] search = Encoding.BigEndianUnicode.GetBytes(toolStripSearchBox.Text.ToCharArray());
            hexBox1.Find(search, hexBox1.SelectionStart + hexBox1.SelectionLength);
            this.Cursor = Cursors.Default;
        }
    }

    public class ByteCharConveter : IByteCharConverter
    {
        Encoding encoding;
        List<byte> requeridedChar;
        List<char> requeridedByte;

        public ByteCharConveter(string encoding)
        {
            this.encoding = Encoding.GetEncoding(encoding);
            requeridedChar = new List<byte>();
            requeridedByte = new List<char>();
        }

        public byte ToByte(char c)
        {
            if (encoding.WebName == "shift_jis")
                return ToByteShiftJis(c);

            return (byte)c;
        }
        public char ToChar(byte b)
        {
            if (encoding.WebName == "shift_jis")
                return ToCharShiftJis(b);

            return encoding.GetChars(new byte[] { b })[0];
        }

        public byte ToByteShiftJis(char c)
        {
            return (byte)c;
        }
        public char ToCharShiftJis(byte b)
        {
            if (requeridedChar.Count == 0 && b > 0x7F)
            {
                requeridedChar.Add(b);
                return '\x20';
            }

            requeridedChar.Add(b);
            string c = new String(encoding.GetChars(requeridedChar.ToArray()));
            requeridedChar.Clear();
            return (c[0] > '\x1F' ? c[0] : '.');

        }

        // TODO: utf-16, unicodeFFFE, utf-32, utf-32BE
    }
}