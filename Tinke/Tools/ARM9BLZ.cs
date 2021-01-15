using System;
using System.Collections.Generic;
using System.Text;

namespace Tinke.Tools
{
    using System.IO;

    using DSDecmp.Formats;

    using Nitro;

    class ARM9BLZ
    {
        /// <summary>
        /// Decompress ARM9.bin
        /// </summary>
        /// <param name="arm9Data">Compressed ARM9.bin data</param>
        /// <param name="hdr">ROM header</param>
        /// <param name="decompressed">Decompressed data</param>
        /// <returns>True if the decompression was successful.</returns>
        public static bool Decompress(byte[] arm9Data, Estructuras.ROMHeader hdr, out byte[] decompressed)
        {
            decompressed = arm9Data;
            uint initptr = BitConverter.ToUInt32(hdr.reserved2, 0) & 0x3FFF;
            uint hdrptr = BitConverter.ToUInt32(arm9Data, (int)initptr + 14);
            uint postSize = (uint)arm9Data.Length - (hdrptr - hdr.ARM9ramAddress);
            bool cmparm9 = initptr > 0 && hdrptr > hdr.ARM9ramAddress && hdrptr <= hdr.ARM9ramAddress + arm9Data.Length;
            if (cmparm9)
            {
                Stream input = new MemoryStream(arm9Data);
                MemoryStream output = new MemoryStream();
                try
                {
                    LZOvl blz = new LZOvl();
                    blz.Decompress(input, hdrptr - hdr.ARM9ramAddress, output);
                    output.Write(arm9Data, arm9Data.Length - (int)postSize, (int)postSize);
                    input.Close();
                    decompressed = output.ToArray();
                }
                catch (Exception)
                {
                    cmparm9 = false;
                }

                input.Close();
                output.Close();
            }

            return cmparm9;
        }

        /// <summary>
        /// Compress ARM9.bin
        /// </summary>
        /// <param name="arm9Data">Uncompressed ARM9.bin data</param>
        /// <param name="hdr">ROM header</param>
        /// <param name="postSize">Data size from the end what will be ignored.</param>
        /// <returns>Compressed data with uncompressed Secure Area (first 0x4000 bytes).</returns>
        public static byte[] Compress(byte[] arm9Data, Estructuras.ROMHeader hdr, uint postSize = 0)
        {
            Stream input = new MemoryStream(arm9Data);
            input.Position = 0x4000;
            MemoryStream output = new MemoryStream();
            output.Write(arm9Data, 0, 0x4000);
            LZOvl blz = new LZOvl();
            LZOvl.LookAhead = false;
            blz.Compress(input, input.Length - 0x4000 - postSize, output);
            input.Close();
            output.Write(arm9Data, arm9Data.Length - (int)postSize, (int)postSize);
            byte[] result = output.ToArray();
            output.Close();

            // Update size
            uint initptr = BitConverter.ToUInt32(hdr.reserved2, 0) & 0x3FFF;
            if (initptr > 0)
            {
                uint hdrptr = (uint)result.Length - postSize + hdr.ARM9ramAddress;
                Array.Copy(BitConverter.GetBytes(hdrptr), 0, result, initptr + 0x14, 4);
            }

            return result;
        }
    }
}
