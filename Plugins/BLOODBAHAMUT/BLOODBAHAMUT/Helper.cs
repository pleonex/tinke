/*
 * Copyright (C) 2011  Tricky Upgrade
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
 * By: Tricky Upgrade
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BLOODBAHAMUT
{
    public class Helper
    {
        public Helper()
        {
        }

        public static byte[] ReadFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            int length = (int)file.Length;
            byte[] fileData = new byte[length];
            int offset = 0;
            
            while (file.Position < length)
            {
                int read = file.Read(fileData, offset, length);
                if (file.Position > length)
                {
                    throw new EndOfStreamException();
                }
            }
            file.Close();

            return fileData;
        }

        public static byte[] ReadFile(string fileName, int length)
        {
            byte[] streamData = new byte[length];
            streamData = ReadFile(fileName, streamData);

            return streamData;
        }

        public static byte[] ReadFile(string fileName, byte[] data)
        {
            FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            int length = data.Length;
            byte[] fileData = new byte[length];
            int offset = 0;

            while (file.Position < length)
            {
                int read = file.Read(fileData, offset, length);
                if (file.Position > length)
                {
                    throw new EndOfStreamException();
                }
            }
            file.Close();

            return fileData;
        }

        public static void WriteFile(string fileName, byte[] data)
        {
            WriteFile(fileName, data, 0, data.Length);
        }

        public static void WriteFile(string fileName, byte[] data, int offset, int length)
        {
            string directoryName = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            FileStream file = null;
            try
            {
                file = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                file.Write(data, offset, length);
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
        }
    }
}
