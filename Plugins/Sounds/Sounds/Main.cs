using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using PluginInterface;

namespace Sounds
{
    public class Main : IPlugin
    {
        IPluginHost pluginHost;

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public Format Get_Format(string name, byte[] magic, int id)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (ext == "sadl")
            {
                byte coding = pluginHost.Get_Bytes(id, 0x33, 1)[0];
                if ((coding & 0xF0) == 0x70 || (coding & 0xF0) == 0xB0)
                    return Format.Sound;
            }

            if (name.ToUpper().EndsWith(".ADX"))
            {
                if (magic[0] != 0x80 || magic[1] != 00)     // Constant
                    return Format.Unknown;

                byte[] checkBytes = pluginHost.Get_Bytes(id, 4, 0xF); // Version and encoding flags
                if (checkBytes[0] == 0x03 && (checkBytes[0xE] == 0x03 || checkBytes[0xE] == 0x04))
                {
                    byte[] offset = { magic[3], magic[2] };
                    byte[] copyright = pluginHost.Get_Bytes(id, BitConverter.ToUInt16(offset, 0) - 2, 6);
                    
                    if (new String(Encoding.ASCII.GetChars(copyright)) == "(c)CRI")
                        return Format.Sound;
                }
            }

            return Format.Unknown;
        }

        public void Read(string file, int id)
        {
            Thread waiting = new Thread(Thread_Waiting);
            waiting.Start((new String[] { "S00", pluginHost.Get_Language() }));

            System.Media.SoundPlayer sp;
            string wav_file = pluginHost.Get_TempFolder() + System.IO.Path.DirectorySeparatorChar
                + System.IO.Path.GetRandomFileName();

            if (file.ToUpper().EndsWith(".SAD"))
            {
                SADL sadl = new SADL(pluginHost.Get_Language(), file, id);
                sadl.Initialize();

                waiting.Abort();

                sadl.Save_WAV(wav_file, false);
                sp = new System.Media.SoundPlayer(wav_file);
                sp.Play();
                return;
            }
            else if (file.ToUpper().EndsWith(".ADX"))
            {
                ADX adx = new ADX(file, id);
                adx.Initialize();

                waiting.Abort();

                adx.Save_WAV(wav_file, false);
                sp = new System.Media.SoundPlayer(wav_file);
                sp.Play();
                return;
            }

            waiting.Abort();
        }

        public Control Show_Info(string file, int id)
        {
            Thread waiting = new Thread(Thread_Waiting);
            waiting.Start((new String[] { "S00", pluginHost.Get_Language() }));

            if (file.ToUpper().EndsWith(".SAD"))
            {
                SADL sadl = new SADL(pluginHost.Get_Language(), file, id);
                sadl.Initialize();

                waiting.Abort();
                return new SoundControl(sadl, pluginHost);
            }
            else if (file.ToUpper().EndsWith(".ADX"))
            {
                ADX adx = new ADX(file, id);
                adx.Initialize();

                waiting.Abort();
                return new SoundControl(adx, pluginHost);
            }

            waiting.Abort();
            return new Control();
        }
        void Thread_Waiting(Object e)
        {
            String[] args = (String[])e;
            Waiting waitWind = new Waiting(args[0], args[1]);
            waitWind.ShowDialog();
        }

        public String Pack(ref sFolder unpacked, string file) { return null; }
        public sFolder Unpack(string file) { return new sFolder(); }
    }
}
