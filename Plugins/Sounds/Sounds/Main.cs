// ----------------------------------------------------------------------
// <copyright file="Main.cs" company="none">

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
// <date>14/05/2012 3:20:19</date>
// -----------------------------------------------------------------------
using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Ekona;
using System.ComponentModel;

namespace Sounds
{
    public class Main : IPlugin
    {
        IPluginHost pluginHost;
        Waiting wait;
        SoundBase sb;

        public void Initialize(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }

        public Format Get_Format(sFile file, byte[] magic)
        {
            string ext = new String(Encoding.ASCII.GetChars(magic));

            if (ext == "sadl")
            {
                byte coding = pluginHost.Get_Bytes(file.path, (int)file.offset + 0x33, 1)[0];
                if ((coding & 0xF0) == 0x70 || (coding & 0xF0) == 0xB0)
                    return Format.Sound;
            }

            if (file.name.ToUpper().EndsWith(".ADX"))
            {
                if (magic[0] != 0x80 || magic[1] != 00)     // Constant
                    return Format.Unknown;

                byte[] checkBytes = pluginHost.Get_Bytes(file.path, (int)file.offset + 4, 0xF); // Version and encoding flags
                if (checkBytes[0] == 0x03 && (checkBytes[0xE] == 0x03 || checkBytes[0xE] == 0x04))
                {
                    byte[] offset = { magic[3], magic[2] };
                    byte[] copyright = pluginHost.Get_Bytes(file.path, (int)file.offset + BitConverter.ToUInt16(offset, 0) - 2, 6);

                    if (new String(Encoding.ASCII.GetChars(copyright)) == "(c)CRI")
                        return Format.Sound;
                }
            }

            return Format.Unknown;
        }

        public void Read(sFile file)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.RunWorkerAsync(file);

            wait = new Waiting("S00", pluginHost.Get_Language());
            wait.ShowDialog();
        }
        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            wait.Close();
            wait.Dispose();
        }
        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            // Get params
            sFile file = (sFile)e.Argument;

            // Get out file
            string wav_file = pluginHost.Get_TempFolder() + Path.DirectorySeparatorChar + file.name + ".wav";
            if (System.IO.File.Exists(wav_file))
            {
                try { File.Delete(wav_file); }
                catch { wav_file += '_' + Path.GetRandomFileName(); }
            }

            // Process
            if (file.name.ToUpper().EndsWith(".SAD"))
            {
                SADL sadl = new SADL(pluginHost.Get_Language(), file.path, file.id);
                sadl.Initialize();
                sadl.Save_WAV(wav_file, false);
            }
            else if (file.name.ToUpper().EndsWith(".ADX"))
            {
                ADX adx = new ADX(file.name, file.id);
                adx.Initialize();
                adx.Save_WAV(wav_file, false);
            }
        }

        public Control Show_Info(sFile file)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWorkRead);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompletedRead);
            bw.RunWorkerAsync(new string[] { file.path, file.id.ToString() });

            wait = new Waiting("S00", pluginHost.Get_Language());
            wait.ShowDialog();

            return new SoundControl(sb, pluginHost);
        }
        void bw_RunWorkerCompletedRead(object sender, RunWorkerCompletedEventArgs e)
        {
            sb = (SoundBase)e.Result;
            wait.Close();
            wait.Dispose();
        }
        void bw_DoWorkRead(object sender, DoWorkEventArgs e)
        {
            // Get params
            string file = ((string[])e.Argument)[0];
            int id = Convert.ToInt32(((string[])e.Argument)[1]);

            BinaryReader br = new BinaryReader(File.OpenRead(file));
            string header = new String(br.ReadChars(4));
            br.Close();

            // Process
            SoundBase sb = null;
            if (header == "sadl")
                sb = new SADL(pluginHost.Get_Language(), file, id);
            else if (file.ToUpper().EndsWith(".ADX"))
                sb = new ADX(file, id);

            sb.Initialize();
            e.Result = sb;
        }

        public String Pack(ref sFolder unpacked, sFile file) { return null; }
        public sFolder Unpack(sFile file) { return new sFolder(); }
    }
}
