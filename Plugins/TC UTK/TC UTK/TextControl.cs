// ----------------------------------------------------------------------
// <copyright file="TextControl.cs" company="none">

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
// <date>21/04/2012 11:20:34</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using Ekona;

namespace TC_UTK
{
    public partial class TextControl : UserControl
    {
        IPluginHost pluginHost;
        string table_file;
        ushort[,] table;
        byte[] data;

        public TextControl()
        {
            InitializeComponent();
        }
        public TextControl(IPluginHost pluginHost, string file)
        {
            InitializeComponent();
            this.pluginHost = pluginHost;

            table_file = Application.StartupPath + Path.DirectorySeparatorChar + "Plugins" +
                Path.DirectorySeparatorChar + "TC UTK.tbl";
            data = File.ReadAllBytes(file);
            Read_Table();
            Update_Text();
        }

        private void Read_Table()
        {
            dataTable.Rows.Clear();
            string[] lines = File.ReadAllLines(table_file, Encoding.Default);
            table = new ushort[lines.Length, 2];

            for (int i = 0; i < lines.Length; i++)
            {
                ushort code = Convert.ToUInt16(lines[i].Substring(0, 4), 16);
                ushort charc = BitConverter.ToUInt16(Encoding.Unicode.GetBytes(Regex.Unescape(lines[i].Substring(5)).ToCharArray()), 0);

                table[i, 0] = code;
                table[i, 1] = charc;
                dataTable.Rows.Add(code.ToString("x").PadLeft(4, '0'), (char)charc);
            }
            Update_Text();
        }
        private void Update_Text()
        {
            txtChar.Text = "";

            string text = "";
            for (int i = 0; i < data.Length; i += 2)
            {
                uint code = BitConverter.ToUInt16(data, i);
                text += Get_Char(code);
            }

            txtChar.Text = text.Replace("\n", "\r\n");
        }
        private string Get_Char(uint code)
        {
            for (int i = 0; i < table.Length / 2; i++)
            {
                if (table[i, 0] == code)
                    if (table[i, 1] != 0)
                        return new String(Encoding.Unicode.GetChars(BitConverter.GetBytes(table[i, 1])));
                    else
                        return "&";
            }
            return '{' + code.ToString("x") + '}';
        }

        private void dataTable_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            ushort value = 0;
            if (e.ColumnIndex == 0)
                value = Convert.ToUInt16((string)dataTable.Rows[e.RowIndex].Cells[0].Value, 16);
            else
            {
                string cell_value = Convert.ToString(dataTable.Rows[e.RowIndex].Cells[1].Value);
                cell_value = Regex.Unescape(cell_value);
                value = BitConverter.ToUInt16(Encoding.Unicode.GetBytes(cell_value.ToCharArray()), 0);
            }


            // Three posible cases:
            if (dataTable.RowCount > table.Length / 2)   // Add a new value
            {
                ushort[,] temp = new ushort[dataTable.RowCount, 2];
                Array.Copy(table, temp, table.Length);

                temp[e.RowIndex, e.ColumnIndex] = value;
                table = temp;
            }
            else if (dataTable.RowCount == table.Length / 2)  // Update a value
                table[e.RowIndex, e.ColumnIndex] = value;

            Update_Text();
        }
        private void dataTable_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            table = new ushort[dataTable.RowCount, 2];
            for (int i = 0; i < dataTable.RowCount; i++)
            {
                if (dataTable.Rows[i].Cells[0].Value is object)
                    table[i, 0] = Convert.ToUInt16((string)dataTable.Rows[i].Cells[0].Value, 16);
                if (dataTable.Rows[i].Cells[0].Value is object)
                {
                    string cell_value = Convert.ToString(dataTable.Rows[e.RowIndex].Cells[1].Value);
                    cell_value = Regex.Unescape(cell_value);
                    table[i, 1] = BitConverter.ToUInt16(Encoding.Unicode.GetBytes(cell_value.ToCharArray()), 0);
                }
            }
            Update_Text();
        }

        private void btnSaveTable_Click(object sender, EventArgs e)
        {
            File.Delete(table_file);
            BinaryWriter bw = new BinaryWriter(File.OpenWrite(table_file));
            for (int i = 0; i < table.Length / 2; i++)
            {
                string line = table[i, 0].ToString("x").PadLeft(4, '0').ToUpper();
                string charc = new String(Encoding.Unicode.GetChars(BitConverter.GetBytes(table[i, 1])));
                charc = Regex.Escape(charc);

                line += "=" + charc + "\n";
                bw.Write(Encoding.Unicode.GetBytes(line.ToCharArray()));
            }

            bw.Flush();
            bw.Close();
        }
        private void btnExportText_Click(object sender, EventArgs e)
        {
            SaveFileDialog o = new SaveFileDialog();
            o.DefaultExt = ".txt";
            o.Filter = "TeXT file (*.txt)|*.txt";
            if (o.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            File.WriteAllText(o.FileName, txtChar.Text);
        }
    }
}
