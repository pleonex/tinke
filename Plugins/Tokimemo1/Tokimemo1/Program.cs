//-----------------------------------------------------------------------
// <copyright file="${FileName}" company="none">
// Copyright (C) 2013
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
//   along with this program.  If not, see "http://www.gnu.org/licenses/". 
// </copyright>
// <author>pleoNeX</author>
// <email>benito356@gmail.com</email>
// <date>01/03/2013</date>
//-----------------------------------------------------------------------
namespace Tokimemo1
{
    using System;
    using Ekona;
    
    public class Main : IGamePlugin
    {
        IPluginHost pluginHost;
        string gameCode;
        
        public void Initialize(IPluginHost pluginHost, string gameCode)
        {
            this.pluginHost = pluginHost;
            this.gameCode = gameCode;
        }
        
        public bool IsCompatible()
        {
            if (this.gameCode == "C4GJ")
            {
                return true;
            }
            
            return false;
        }
        
        public Format Get_Format(sFile file, byte[] magic)
        {
            if (file.name.EndsWith(".pack") || IsPack(file.id))
            {
                return Format.Pack;
            }
            
            return Format.Unknown;
        }
        
        private bool IsPack(int id)
        {
            // Folder: /command
            if (id >= 0x00 && id <= 0x71)
            {
                if (id == 0x00 || id == 0x07 || id == 0x43 ||
                    id == 0x71)
                    return false;
                else
                    return true;
            }

            // Folder: /imai
            if (id >= 0x072 && id <= 0x2B8)
            {
                if (id == 0x078 || id == 0x088 || id == 0x089 ||
                    id == 0x22A || id == 0x230 || id == 0x259 ||
                    id == 0x25E || id == 0x2B7 || id == 0x267)
                    return false;
                else
                    return true;
            }
            
            // Folder: /mail
            if (id >= 0x2B9 && id <= 0x2D0)
            {
                if ((id >= 0x2BB && id <= 0x2C7) ||             // Subfolder: /deco
                    id == 0x2C8 || id == 0x2CE || id == 0x2CF)  // Subfolder: /graphics
                    return true;
                else
                    return false;
            }
            
            // Folder: /mini
            if (id >= 0x2D1 && id <= 0x3B8)
            {
                if (id == 0x36E || id == 0x36F || id == 0x375 ||
                    id == 0x382 || id == 0x383 || id == 0x384 ||
                    id == 0x385 || id == 0x398 || id == 0x3AA ||
                    id == 0x3B1)
                    return false;
                else
                    return true;
            }
            
            // Folder: /omake
            if (id >= 0x3B9 && id <= 0x3CA)
            {
                return true;
            }
            
            // Folder: /script
            if (id >= 0x3CB && id <= 0x76D)
            {
                if (id == 0x3CD)
                    return false;
                else
                    return true;
            }
            
            // Folder: /skinship
            if (id >= 0x1CB0 && id <= 0x1CBF)
            {
                if ((id >= 0x1CB8 && id <= 0x1CBF) ||   // Subfolder: /dialog
                    id == 0x1CB6 || id == 0x1CB7)
                    return true;
                else
                    return false;
            }
            
            // Folder: /staffroll
            if (id >= 0x1CC0 && id <= 0x1CC8)
            {
                if (id == 0x1CC2 || id == 0x1CC6 || id == 0x1CC7 ||
                   id == 0x1CC8)
                    return false;
                else
                    return true;
            }
            
            // Folder: /sys
            if (id >= 0x1CC9 && id <= 0x1CCE)
            {
                if (id == 0x1CCC || id == 0x1CCE)
                    return false;
                else
                    return true;
            }
            
            // Folder: /title
            if (id >= 0x1CCF && id <= 0x1CD4)
                return true;
            
            // Folder: /web
            if (id >= 0x1CD8 && id <= 0x1CEF)
            {
                if (id == 0x1CE0 || id == 0x1CE2 || id == 0x1CED ||
                    id == 0x1CEE)
                    return false;
                else
                    return true;
            }
            
            return false;
        }
        
        public System.Windows.Forms.Control Show_Info(sFile file)
        {
            throw new NotImplementedException();
        }
        
        public void Read(sFile file)
        {
            throw new NotImplementedException();
        }
        
        public sFolder Unpack(sFile file)
        {
            if (!IsPack(file.id) && !file.name.EndsWith(".pack"))
            {
                return new sFolder();
            }
            
            return Packer.Unpack(file, this.pluginHost);
        }
        
        public string Pack(ref sFolder unpacked, sFile file)
        {
            throw new NotImplementedException();
        }
    }
}