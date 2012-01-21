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
 * By: pleoNeX
 * 
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDAT
{
    public partial class InfoForm : Form
    {
        Info info;

        public InfoForm()
        {
            InitializeComponent();
        }
        public InfoForm(Info info)
        {
            InitializeComponent();

            this.info = info;
            ReadInfo();
            ReadRecord(0);
        }

        private void ReadInfo()
        {
            // SSEQ record
            if (info.block[0].nEntries != 0)
            {
                numericSSEQrecord.Maximum = info.block[0].nEntries - 1;
                labelSSEQtotal.Text = "of " + (info.block[0].nEntries - 1);
                ReadRecord(0);
            }
            else
            {
                numericSSEQrecord.Maximum = 0;
                labelSSEQtotal.Text = "of 0";
            }

            // SSAR record
            if (info.block[1].nEntries != 0)
            {
                numericSSARrecord.Maximum = info.block[1].nEntries - 1;
                labelSSARtotal.Text = "of " + (info.block[1].nEntries - 1);
            }
            else
            {
                numericSSARrecord.Maximum = 0;
                labelSSARtotal.Text = "of none";
            }
            ReadRecord(1);

            // SBNK record
            if (info.block[2].nEntries != 0)
            {
                numericSBNKrecord.Maximum = info.block[2].nEntries - 1;
                labelSBNKtotal.Text = "of " + (info.block[2].nEntries - 1);
            }
            else
            {
                numericSBNKrecord.Maximum = 0;
                labelSBNKtotal.Text = "of none";
            }
            ReadRecord(2);

            // SWAR record
            if (info.block[3].nEntries != 0)
            {
                numericSWARrecord.Maximum = info.block[3].nEntries - 1;
                labelSWARtotal.Text = "of " + (info.block[3].nEntries - 1);
            }
            else
            {
                numericSWARrecord.Maximum = 0;
                labelSWARtotal.Text = "of none";
            }
            ReadRecord(3);

            // PLAYER record
            if (info.block[4].nEntries != 0)
            {
                numericPLAYERrecord.Maximum = info.block[4].nEntries - 1;
                labelPLAYERtotal.Text = "of " + (info.block[4].nEntries - 1);
            }
            else
            {
                numericPLAYERrecord.Maximum = 0;
                labelPLAYERtotal.Text = "of none";
            }
            ReadRecord(4);

            // GROUP record
            if (info.block[5].nEntries != 0)
            {
                numericGROUPrecord.Maximum = info.block[5].nEntries - 1;
                labelGROUPtotal.Text = "of " + (info.block[5].nEntries - 1);
            }
            else
            {
                numericGROUPrecord.Maximum = 0;
                labelGROUPtotal.Text = "of none";
            }
            ReadRecord(5);

            // PLAYER2 record
            if (info.block[6].nEntries != 0)
            {
                numericPLAYER2record.Maximum = info.block[6].nEntries - 1;
                labelPLAYER2total.Text = "of " + (info.block[6].nEntries - 1);
            }
            else
            {
                numericPLAYER2record.Maximum = 0;
                labelPLAYER2total.Text = "of none";
            }
            ReadRecord(6);

            // STRM record
            if (info.block[7].nEntries != 0)
            {
                numericSTRMrecord.Maximum = info.block[7].nEntries - 1;
                labelSTRMtotal.Text = "of " + (info.block[7].nEntries - 1);
            }
            else
            {
                numericSTRMrecord.Maximum = 0;
                labelSTRMtotal.Text = "of none";
            }
            ReadRecord(7);
        }
        private void ReadRecord(int page)
        {
            switch (page)
            {
                case 0:
                    if (info.block[0].nEntries == 0)
                    {
                        numericSSEQfileID.Enabled = false;
                        numericSSEQunknown.Enabled = false;
                        numericSSEQBnk.Enabled = false;
                        numericSSEQVol.Enabled = false;
                        numericSSEQcpr.Enabled = false;
                        numericSSEQppr.Enabled = false;
                        numericSSEQply.Enabled = false;

                        return;
                    }

                    Info.SEQ seq = (Info.SEQ)info.block[0].entries[(int)numericSSEQrecord.Value];
                    numericSSEQfileID.Value = seq.fileID;
                    numericSSEQunknown.Value = seq.unknown;
                    numericSSEQBnk.Value = seq.bnk;
                    numericSSEQVol.Value = seq.vol;
                    numericSSEQcpr.Value = seq.cpr;
                    numericSSEQppr.Value = seq.ppr;
                    numericSSEQply.Value = seq.ply;
                    break;

                case 1:
                    if (info.block[1].nEntries == 0)
                    {
                        numericSSARfileID.Enabled = false;
                        numericSSARunknown.Enabled = false;

                        return;
                    }

                    Info.SEQARC ssar = (Info.SEQARC)info.block[1].entries[(int)numericSSARrecord.Value];
                    numericSSARfileID.Value = ssar.fileID;
                    numericSSARunknown.Value = ssar.unknown;
                    break;

                case 2:
                    if (info.block[2].nEntries == 0)
                    {
                        numericSBNKfileID.Enabled = false;
                        numericSBNKunknown.Enabled = false;
                        numericSBNKwa1.Enabled = false;
                        numericSBNKwa2.Enabled = false;
                        numericSBNKwa3.Enabled = false;
                        numericSBNKwa4.Enabled = false;

                        return;
                    }

                    Info.BANK sbnk = (Info.BANK)info.block[2].entries[(int)numericSBNKrecord.Value];
                    numericSBNKfileID.Value = sbnk.fileID;
                    numericSBNKunknown.Value = sbnk.unknown;
                    numericSBNKwa1.Value = sbnk.wa[0];
                    numericSBNKwa2.Value = sbnk.wa[1];
                    numericSBNKwa3.Value = sbnk.wa[2];
                    numericSBNKwa4.Value = sbnk.wa[3];
                    break;

                case 3:
                    if (info.block[3].nEntries == 0)
                        return;

                    Info.WAVEARC swar = (Info.WAVEARC)info.block[3].entries[(int)numericSWARrecord.Value];
                    numericSWARfileID.Value = swar.fileID;
                    numericSWARunknown.Value = swar.unknown;
                    break;

                case 4:
                    if (info.block[4].nEntries == 0)
                    {
                        btnPLAYERremove.Enabled = false;
                        numericPLAYERunknown.Enabled = false;
                        numericPLAYERunknown2.Enabled = false;
                        return;
                    }
                    btnPLAYERremove.Enabled = true;
                    numericPLAYERunknown.Enabled = true;
                    numericPLAYERunknown2.Enabled = true;

                    Info.PLAYER player = (Info.PLAYER)info.block[4].entries[(int)numericPLAYERrecord.Value];
                    numericPLAYERunknown.Value = player.unknown;
                    numericPLAYERunknown2.Value = player.unknown2;
                    break;

                case 5:
                    if (info.block[5].nEntries == 0)
                    {
                        btnGROUPremove.Enabled = false;
                        btnGROUPsubgroupAdd.Enabled = false;
                        btnGROUPsubgroupRemove.Enabled = false;
                        labelGROUPsubgroupTotal.Text = "no group";
                        numericGROUPcurrentSubgroup.Enabled = false;
                        comboBoxGROUPtype.Enabled = false;
                        numericGROUPentry.Enabled = false;

                        return;
                    }
                    btnGROUPremove.Enabled = true;
                    btnGROUPsubgroupRemove.Enabled = true;
                    btnGROUPsubgroupAdd.Enabled = true;
                    numericGROUPcurrentSubgroup.Enabled = true;
                    comboBoxGROUPtype.Enabled = true;
                    numericGROUPentry.Enabled = true;

                    Info.GROUP group = (Info.GROUP)info.block[5].entries[(int)numericGROUPrecord.Value];

                    if (group.nCount == 0)
                    {
                        btnGROUPsubgroupRemove.Enabled = false;
                        comboBoxGROUPtype.Enabled = false;
                        numericGROUPentry.Enabled = false;
                        labelGROUPsubgroupTotal.Text = "of none";
                        numericGROUPcurrentSubgroup.Enabled = false;
                        return;
                    }

                    labelGROUPsubgroupTotal.Text = "of " + (group.nCount - 1);
                    numericGROUPcurrentSubgroup.Maximum = group.nCount - 1;
                    numericGROUPcurrentSubgroup.Value = 0;
                    numericGROUPcurrentGroup_ValueChanged(null, null);
                    break;

                case 6:
                    if (info.block[6].nEntries == 0)
                    {
                        btnPLAYER2remove.Enabled = false;
                        numericPLAYER2count.Enabled = false;
                        numericPLAYER2v1.Enabled = false;
                        numericPLAYER2v2.Enabled = false;
                        numericPLAYER2v3.Enabled = false;
                        numericPLAYER2v4.Enabled = false;
                        numericPLAYER2v5.Enabled = false;
                        numericPLAYER2v6.Enabled = false;
                        numericPLAYER2v7.Enabled = false;
                        numericPLAYER2v8.Enabled = false;
                        numericPLAYER2v9.Enabled = false;
                        numericPLAYER2v10.Enabled = false;
                        numericPLAYER2v11.Enabled = false;
                        numericPLAYER2v12.Enabled = false;
                        numericPLAYER2v13.Enabled = false;
                        numericPLAYER2v14.Enabled = false;
                        numericPLAYER2v15.Enabled = false;
                        numericPLAYER2v16.Enabled = false;

                        return;
                    }
                    btnPLAYER2remove.Enabled = true;
                    numericPLAYER2count.Enabled = true;
                    numericPLAYER2v1.Enabled = true;
                    numericPLAYER2v2.Enabled = true;
                    numericPLAYER2v3.Enabled = true;
                    numericPLAYER2v4.Enabled = true;
                    numericPLAYER2v5.Enabled = true;
                    numericPLAYER2v6.Enabled = true;
                    numericPLAYER2v7.Enabled = true;
                    numericPLAYER2v8.Enabled = true;
                    numericPLAYER2v9.Enabled = true;
                    numericPLAYER2v10.Enabled = true;
                    numericPLAYER2v11.Enabled = true;
                    numericPLAYER2v12.Enabled = true;
                    numericPLAYER2v13.Enabled = true;
                    numericPLAYER2v14.Enabled = true;
                    numericPLAYER2v15.Enabled = true;
                    numericPLAYER2v16.Enabled = true;


                    Info.PLAYER2 player2 = (Info.PLAYER2)info.block[6].entries[(int)numericPLAYER2record.Value];
                    numericPLAYER2count.Value = player2.nCount;
                    numericPLAYER2v1.Value = player2.v[0];
                    numericPLAYER2v2.Value = player2.v[1];
                    numericPLAYER2v3.Value = player2.v[2];
                    numericPLAYER2v4.Value = player2.v[3];
                    numericPLAYER2v5.Value = player2.v[4];
                    numericPLAYER2v6.Value = player2.v[5];
                    numericPLAYER2v7.Value = player2.v[6];
                    numericPLAYER2v8.Value = player2.v[7];
                    numericPLAYER2v9.Value = player2.v[8];
                    numericPLAYER2v10.Value = player2.v[9];
                    numericPLAYER2v11.Value = player2.v[10];
                    numericPLAYER2v12.Value = player2.v[11];
                    numericPLAYER2v13.Value = player2.v[12];
                    numericPLAYER2v14.Value = player2.v[13];
                    numericPLAYER2v15.Value = player2.v[14];
                    numericPLAYER2v16.Value = player2.v[15];
                    break;

                case 7:
                    if (info.block[7].nEntries == 0)
                    {
                        numericSTRMfileID.Enabled = false;
                        numericSTRMunknown.Enabled = false;
                        numericSTRMpri.Enabled = false;
                        numericSTRMvol.Enabled = false;
                        numericSTRMply.Enabled = false;

                        return;
                    }

                    Info.STRM strm = (Info.STRM)info.block[7].entries[(int)numericSTRMrecord.Value];
                    numericSTRMfileID.Value = strm.fileID;
                    numericSTRMunknown.Value = strm.unknown;
                    numericSTRMpri.Value = strm.pri;
                    numericSTRMvol.Value = strm.vol;
                    numericSTRMply.Value = strm.ply;
                    break;
            }
        }

        public Info Info
        {
            get { return info; }
        }
        private void btnAccept_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void numericSSEQrecord_ValueChanged(object sender, EventArgs e)
        {
            ReadRecord(0);
        }
        private void numericSSEQ_ValueChanged(object sender, EventArgs e)
        {
            if (info.block[0].nEntries == 0)
                return;

            Info.SEQ seq = new Info.SEQ();
            seq.fileID = (ushort)numericSSEQfileID.Value;
            seq.unknown = (ushort)numericSSEQunknown.Value;
            seq.bnk = (ushort)numericSSEQBnk.Value;
            seq.vol = (byte)numericSSEQVol.Value;
            seq.cpr = (byte)numericSSEQcpr.Value;
            seq.ppr = (byte)numericSSEQppr.Value;
            seq.ply = (byte)numericSSEQply.Value;
            seq.unknown2 = new byte[2];
            info.block[0].entries[(int)numericSSEQrecord.Value] = seq;
        }

        private void numericSSARrecord_ValueChanged(object sender, EventArgs e)
        {
            ReadRecord(1);
        }
        private void numericSSAR_ValueChanged(object sender, EventArgs e)
        {
            if (info.block[1].nEntries == 0)
                return;

            Info.SEQARC ssar = new Info.SEQARC();
            ssar.fileID = (ushort)numericSSARfileID.Value;
            ssar.unknown = (ushort)numericSSARunknown.Value;
            info.block[1].entries[(int)numericSSARrecord.Value] = ssar;
        }

        private void numericSBNKrecord_ValueChanged(object sender, EventArgs e)
        {
            ReadRecord(2);
        }
        private void numericSBNK_ValueChanged(object sender, EventArgs e)
        {
            if (info.block[2].nEntries == 0)
                return;

            Info.BANK sbnk = new Info.BANK();
            sbnk.fileID = (ushort)numericSBNKfileID.Value;
            sbnk.unknown = (ushort)numericSBNKunknown.Value;
            sbnk.wa = new ushort[4];
            sbnk.wa[0] = (ushort)numericSBNKwa1.Value;
            sbnk.wa[1] = (ushort)numericSBNKwa2.Value;
            sbnk.wa[2] = (ushort)numericSBNKwa3.Value;
            sbnk.wa[3] = (ushort)numericSBNKwa4.Value;
            info.block[2].entries[(int)numericSBNKrecord.Value] = sbnk;
        }

        private void numericSWARrecord_ValueChanged(object sender, EventArgs e)
        {
            ReadRecord(3);
        }
        private void numericSWAR_ValueChanged(object sender, EventArgs e)
        {
            if (info.block[3].nEntries == 0)
                return;

            Info.WAVEARC swar = new Info.WAVEARC();
            swar.fileID = (ushort)numericSWARfileID.Value;
            swar.unknown = (ushort)numericSWARunknown.Value;
            info.block[3].entries[(int)numericSWARrecord.Value] = swar;
        }

        private void numericSTRMrecord_ValueChanged(object sender, EventArgs e)
        {
            ReadRecord(7);
        }
        private void numericSTRM_ValueChanged(object sender, EventArgs e)
        {
            if (info.block[7].nEntries == 0)
                return;

            Info.STRM strm = new Info.STRM();
            strm.fileID = (ushort)numericSTRMfileID.Value;
            strm.unknown = (ushort)numericSTRMunknown.Value;
            strm.ply = (byte)numericSTRMply.Value;
            strm.pri = (byte)numericSTRMpri.Value;
            strm.vol = (byte)numericSTRMvol.Value;
            strm.reserved = new byte[5];
            info.block[7].entries[(int)numericSTRMrecord.Value] = strm;
        }

        private void numericPLAYERrecord_ValueChanged(object sender, EventArgs e)
        {
            ReadRecord(4);
        }
        private void numericPLAYER_ValueChanged(object sender, EventArgs e)
        {
            if (info.block[4].nEntries == 0)
                return;

            Info.PLAYER player = new Info.PLAYER();
            player.unknown = (byte)numericPLAYERunknown.Value;
            player.unknown2 = (byte)numericPLAYERunknown2.Value;
            player.padding = new byte[3];
            info.block[4].entries[(int)numericPLAYERrecord.Value] = player;
        }
        private void btnPLAYERadd_Click(object sender, EventArgs e)
        {
            Object[] entries = new object[++info.block[4].nEntries];
            Array.Copy(info.block[4].entries, 0, entries, 0, info.block[4].entries.Length);
            entries[entries.Length - 1] = new Info.PLAYER();
            info.block[4].entries = entries;
            ReadInfo();
            numericPLAYERrecord.Value = numericPLAYERrecord.Maximum;
        }
        private void btnPLAYERremove_Click(object sender, EventArgs e)
        {
            Object[] entries = new object[--info.block[4].nEntries];
            for (int i = 0, j = 0; i < info.block[4].entries.Length; i++)
            {
                if (i == numericPLAYERrecord.Value)
                    continue;
                entries[j++] = info.block[4].entries[i];
            }
            info.block[4].entries = entries;
            ReadInfo();
        }

        private void numericPLAYER2record_ValueChanged(object sender, EventArgs e)
        {
            ReadRecord(6);
        }
        private void numericPLAYER2_ValueChanged(object sender, EventArgs e)
        {
            if (info.block[6].nEntries == 0)
                return;

            Info.PLAYER2 player2 = new Info.PLAYER2();
            player2.nCount = (byte)numericPLAYER2count.Value;
            player2.v = new byte[16];
            player2.v[0] = (byte)numericPLAYER2v1.Value;
            player2.v[1] = (byte)numericPLAYER2v2.Value;
            player2.v[2] = (byte)numericPLAYER2v3.Value;
            player2.v[3] = (byte)numericPLAYER2v4.Value;
            player2.v[4] = (byte)numericPLAYER2v5.Value;
            player2.v[5] = (byte)numericPLAYER2v6.Value;
            player2.v[6] = (byte)numericPLAYER2v7.Value;
            player2.v[7] = (byte)numericPLAYER2v8.Value;
            player2.v[8] = (byte)numericPLAYER2v9.Value;
            player2.v[9] = (byte)numericPLAYER2v10.Value;
            player2.v[10] = (byte)numericPLAYER2v11.Value;
            player2.v[11] = (byte)numericPLAYER2v12.Value;
            player2.v[12] = (byte)numericPLAYER2v13.Value;
            player2.v[13] = (byte)numericPLAYER2v14.Value;
            player2.v[14] = (byte)numericPLAYER2v15.Value;
            player2.v[15] = (byte)numericPLAYER2v16.Value;
            player2.reserved = new byte[7];
            info.block[6].entries[(int)numericPLAYER2record.Value] = player2;
        }
        private void btnPLAYER2add_Click(object sender, EventArgs e)
        {
            Object[] entries = new object[++info.block[6].nEntries];
            Array.Copy(info.block[6].entries, 0, entries, 0, info.block[6].entries.Length);

            Info.PLAYER2 newPlay = new Info.PLAYER2();
            newPlay.v = new byte[0x10];
            entries[entries.Length - 1] = newPlay;

            info.block[6].entries = entries;
            ReadInfo();
            numericPLAYER2record.Value = numericPLAYER2record.Maximum;
        }
        private void btnPLAYER2remove_Click(object sender, EventArgs e)
        {
            Object[] entries = new object[--info.block[6].nEntries];
            for (int i = 0, j = 0; i < info.block[6].entries.Length; i++)
            {
                if (i == numericPLAYER2record.Value)
                    continue;
                entries[j++] = info.block[6].entries[i];
            }
            info.block[6].entries = entries;
            ReadInfo();
        }

        private void numericGROUPrecord_ValueChanged(object sender, EventArgs e)
        {
            ReadRecord(5);
        }
        private void numericGROUPcurrentGroup_ValueChanged(object sender, EventArgs e)
        {
            if (info.block[5].nEntries == 0)
                return;

            Info.GROUP group = (Info.GROUP)info.block[5].entries[(int)numericGROUPrecord.Value];
            numericGROUPentry.Value = group.subgroup[(int)numericGROUPcurrentSubgroup.Value].nEntry;
            switch (group.subgroup[(int)numericGROUPcurrentSubgroup.Value].type)
            {
                case 0x0700:
                    comboBoxGROUPtype.SelectedIndex = 0;
                    break;
                case 0x0803:
                    comboBoxGROUPtype.SelectedIndex = 1;
                    break;
                case 0x0601:
                    comboBoxGROUPtype.SelectedIndex = 2;
                    break;
                case 0x0402:
                    comboBoxGROUPtype.SelectedIndex = 3;
                    break;
            }
        }

        private void btnGROUPentryAdd_Click(object sender, EventArgs e)
        {
            Info.GROUP group = (Info.GROUP)info.block[5].entries[(int)numericGROUPrecord.Value];

            Info.GROUP.Subgroup[] entries = new Info.GROUP.Subgroup[++group.nCount];
            if (group.subgroup is Info.GROUP.Subgroup[])
                Array.Copy(group.subgroup, 0, entries, 0, group.subgroup.Length);
            entries[entries.Length - 1] = new Info.GROUP.Subgroup();
            group.subgroup = entries;

            info.block[5].entries[(int)numericGROUPrecord.Value] = group;
            numericGROUPcurrentSubgroup.Maximum = group.nCount - 1;
            numericGROUPcurrentSubgroup.Value = numericGROUPcurrentSubgroup.Maximum;

            ReadInfo();
        }
        private void btnGROUPentryRemove_Click(object sender, EventArgs e)
        {
            Info.GROUP group = (Info.GROUP)info.block[5].entries[(int)numericGROUPrecord.Value];

            Info.GROUP.Subgroup[] entries = new Info.GROUP.Subgroup[--group.nCount];
            for (int i = 0, j = 0; i < group.subgroup.Length; i++)
            {
                if (i == numericGROUPcurrentSubgroup.Value)
                    continue;
                entries[j++] = group.subgroup[i];
            }
            group.subgroup = entries;
            info.block[5].entries[(int)numericGROUPrecord.Value] = group;

            ReadInfo();
        }

        private void btnGROUPadd_Click(object sender, EventArgs e)
        {
            Object[] entries = new object[++info.block[5].nEntries];
            Array.Copy(info.block[5].entries, 0, entries, 0, info.block[5].entries.Length);
            entries[entries.Length - 1] = new Info.GROUP();
            info.block[5].entries = entries;

            numericGROUPrecord.Value = numericGROUPrecord.Maximum;

            ReadInfo();
        }
        private void btnGROUPremove_Click(object sender, EventArgs e)
        {
            Object[] entries = new object[--info.block[5].nEntries];
            for (int i = 0, j = 0; i < info.block[5].entries.Length; i++)
            {
                if (i == numericGROUPrecord.Value)
                    continue;
                entries[j++] = info.block[5].entries[i];
            }
            info.block[5].entries = entries;
            ReadInfo();
        }

        private void numericGROUPentry_ValueChanged(object sender, EventArgs e)
        {
            Info.GROUP group = (Info.GROUP)info.block[5].entries[(int)numericGROUPrecord.Value];
            group.subgroup[(int)numericGROUPcurrentSubgroup.Value].nEntry = (uint)numericGROUPentry.Value;
            info.block[5].entries[(int)numericGROUPrecord.Value] = group;
        }
        private void comboBoxGROUPtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            Info.GROUP group = (Info.GROUP)info.block[5].entries[(int)numericGROUPrecord.Value];
            switch (comboBoxGROUPtype.SelectedIndex)
            {
                case 0:
                    group.subgroup[(int)numericGROUPcurrentSubgroup.Value].type = 0x0700;
                    break;
                case 1:
                    group.subgroup[(int)numericGROUPcurrentSubgroup.Value].type = 0x0803;
                    break;
                case 2:
                    group.subgroup[(int)numericGROUPcurrentSubgroup.Value].type = 0x0601;
                    break;
                case 3:
                    group.subgroup[(int)numericGROUPcurrentSubgroup.Value].type = 0x0402;
                    break;
            }
            info.block[5].entries[(int)numericGROUPrecord.Value] = group;
        }
    }
}
