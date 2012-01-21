using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LAYTON
{
    public partial class ScriptControl : UserControl
    {
        Command[] cmds;

        public ScriptControl()
        {
            InitializeComponent();
        }
        public ScriptControl(Command[] cmds)
        {
            InitializeComponent();
            this.cmds = cmds;

            Set_Tree();
        }

        public void Set_Tree()
        {
            treeCommands.Nodes.Clear();
            for (int i = 0; i < cmds.Length; i++)
            {
                TreeNode node = new TreeNode("0x" + cmds[i].cmd.ToString("x").ToUpper());
                node.Tag = i;
                for (int p = 0; p < cmds[i].param.Count; p++)
                {
                    node.Nodes.Add(cmds[i].param[p].type.ToString("x"));
                }
                treeCommands.Nodes.Add(node);
            }
        }

        private void treeCommands_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeCommands.SelectedNode.Tag is int)
                Get_Text((int)treeCommands.SelectedNode.Tag);
        }

        public void Get_Text(int i)
        {
            Command cmd = cmds[i];
            txtOriginal.Text = "Command: 0x" + cmd.cmd.ToString("x") + "\r\n";
            txtOriginal.Text += "Params: " + cmd.param.Count.ToString() + "\r\n";

            for (int p = 0; p < cmd.param.Count; p++)
            {
                txtOriginal.Text += "\tType " + cmd.param[p].type.ToString() + " - ";
                if (cmd.param[p].type == 0x01)
                    txtOriginal.Text += BitConverter.ToUInt32(cmd.param[p].value, 0);
                else if (cmd.param[p].type == 0x03)
                    txtOriginal.Text += new String(Encoding.ASCII.GetChars(cmd.param[p].value)).Replace("\0", "");
                else if (cmd.param[p].type == 0x02)
                {
                    txtOriginal.Text += "\r\n\t\tValue1: 0x" + BitConverter.ToUInt16(cmd.param[p].value, 0).ToString("x");
                    txtOriginal.Text += "\r\n\t\tValue2: 0x" + BitConverter.ToUInt16(cmd.param[p].value, 2).ToString("x");
                }

                txtOriginal.Text += "\r\n";
            }
        }
    }
}
