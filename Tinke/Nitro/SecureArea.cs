// ----------------------------------------------------------------------
// <copyright file="SecureArea.cs" company="none">

// Copyright (C) 2019
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

// <author>MetLob</author>
// <email>metlob@mail.ru</email>
// <date>05/10/2019 23:50:14</date>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tinke.Nitro
{
    using Ekona.Helper;
    using Tinke.Tools.Cryptography;

    class SecureArea
    {
        byte[] sa; // Decrypted SA data
        byte[] saEnc; // Encrypted SA data

        public uint CurrentKey { get; private set; }
        public bool OriginalEncrypted { get; private set; }

        public SecureArea(string romFile)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(romFile));
            br.BaseStream.Position = 0xC;
            uint gameCode = br.ReadUInt32();

            br.BaseStream.Position = 0x4000;
            byte[] data = br.ReadBytes(0x4000);
            br.Close();

            this.CurrentKey = gameCode;
            if (BitConverter.ToUInt64(data, 0) == 0xE7FFDEFFE7FFDEFF)
            {
                // Encrypt SA
                OriginalEncrypted = false;
                sa = (byte[])data.Clone();
                SAEncryptor.EncryptSecureArea(gameCode, data);
                saEnc = data;
            }
            else
            {
                // Decrypt SA
                OriginalEncrypted = true;
                saEnc = (byte[])data.Clone();
                SAEncryptor.DecryptSecureArea(gameCode, data);
                sa = data;
            }
        }

        public byte[] EncryptedData
        {
            get { return this.saEnc; }
        }

        public void Encrypt(uint key)
        {
            if (key != this.CurrentKey)
            {
                this.CurrentKey = key;
                saEnc = (byte[])sa.Clone();
                SAEncryptor.EncryptSecureArea(key, saEnc);
            }
        }

        public static ushort CalcCRC(byte[] data, uint gameCode)
        {
            if (BitConverter.ToUInt64(data, 0) == 0xE7FFDEFFE7FFDEFF) SAEncryptor.EncryptSecureArea(gameCode, data);
            return (ushort)CRC16.Calculate(data);
        }
    }
}
