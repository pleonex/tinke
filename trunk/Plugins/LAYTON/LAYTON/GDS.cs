using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LAYTON
{
    public static class GDS
    {

        public static Command[] Read(string file)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            List<Command> cmds = new List<Command>();

            uint file_size = br.ReadUInt32();

            int current = -1;
            while (br.BaseStream.Position - 4 < file_size)
            {
                ushort id = br.ReadUInt16();
                switch (id)
                {
                    case 0x00:
                        current++;
                        Command cmd = new Command();
                        cmd.param = new List<Param>();

                        cmd.cmd = br.ReadUInt16();
                        cmds.Add(cmd);
                        break;

                    case 0x01:  // uint
                        Param uintp = new Param();
                        uintp.type = id;
                        uintp.value = br.ReadBytes(4);
                        cmds[current].param.Add(uintp);
                        break;

                    case 0x02:  // unknown
                        Param ushortp = new Param();
                        ushortp.type = id;
                        ushortp.value = br.ReadBytes(4);
                        cmds[current].param.Add(ushortp);
                        break;

                    case 0x03:
                        Param variable = new Param();
                        variable.type = id;
                        ushort length = br.ReadUInt16();
                        variable.value = br.ReadBytes(length);
                        cmds[current].param.Add(variable);
                        break;

                    case 0x06:  // unknown
                        Param unk1 = new Param();
                        unk1.type = id;
                        unk1.value = br.ReadBytes(4);
                        cmds[current].param.Add(unk1);
                        break;

                    case 0x07:  // unknown
                        Param unk3 = new Param();
                        unk3.type = id;
                        unk3.value = br.ReadBytes(4);
                        cmds[current].param.Add(unk3);
                        break;


                    case 0x08:  // unknown
                        Param unk2 = new Param();
                        unk2.type = id;
                        unk2.value = new byte[0];
                        cmds[current].param.Add(unk2);
                        break;


                    case 0x0C:
                        Command end = new Command();
                        end.param = new List<Param>();
                        end.cmd = id;
                        cmds.Add(end);

                        goto End;
                    default:
                        System.Windows.Forms.MessageBox.Show("Unknown type of value: " + id.ToString("x") + "\r\n" +
                            "at: 0x" + br.BaseStream.Position.ToString("x"));
                        goto End;

                }
            }
        End:

            br.Close();
            return cmds.ToArray();
        }
    }

    public struct Command
    {
        public ushort cmd;
        public List<Param> param;
    }
    public struct Param
    {
        public ushort type;
        public byte[] value;
    }
}
