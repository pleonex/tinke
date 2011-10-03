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

        public VisorHex(sFile file, bool edit)
        {
            InitializeComponent();
            this.Text = Tools.Helper.ObtenerTraduccion("Sistema", "S41");
            btnSave.Enabled = edit;

            this.file = file;
            BinaryReader br = new BinaryReader(File.OpenRead(file.path));
            br.BaseStream.Position = file.offset;
            hexBox1.ByteProvider = new DynamicByteProvider(br.ReadBytes((int)file.size));
            br.Close();
         }

        private void btnSave_Click(object sender, EventArgs e)
        {
            fileEdited = true;
            String newFilePath = Path.GetTempFileName();
            File.WriteAllBytes(newFilePath,
                ((DynamicByteProvider)hexBox1.ByteProvider).Bytes.ToArray());
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
            hexBox1.Height = this.Height - 74;
        }

        private void comboBoxEncoding_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Obviously it doesn't work with shift-jis and unicode
            //hexBox1.ByteCharConverter = new ByteCharConveter(comboBoxEncoding.Text);
        }

        private void numericOffset_ValueChanged(object sender, EventArgs e)
        {
            hexBox1.Select((long)numericOffset.Value, 1);
        }
    }

    public class ByteCharConveter : IByteCharConverter
    {
        string encoding;

        public ByteCharConveter(string encoding)
        {
            this.encoding = encoding;
        }

        public byte ToByte(char c)
        {
            return Encoding.GetEncoding(encoding).GetBytes(new Char[] { c })[0];
        }

        public char ToChar(byte b)
        {
            return Encoding.GetEncoding(encoding).GetChars(new Byte[] { b })[0];
        }
    }
}