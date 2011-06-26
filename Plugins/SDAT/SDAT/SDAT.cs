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
 * Programador: pleoNeX
 * Programa utilizado: Visual Studio 2010
 * Fecha: 24/06/2011
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PluginInterface;

namespace SDAT
{
    public class SDAT : IPlugin
    {
        IPluginHost pluginHost;

        #region Plugin
        public Formato Get_Formato(string nombre, byte[] magic)
        {
            string ext = new String(System.Text.Encoding.ASCII.GetChars(magic));

            if (nombre.EndsWith(".SDAT") && ext == "SDAT")
                return Formato.Sonido;

            return Formato.Desconocido;
        }
        public void Inicializar(IPluginHost pluginHost)
        {
            this.pluginHost = pluginHost;
        }
        public void Leer(string archivo)
        {
            MessageBox.Show("Este archivo no guarda información.");
        }
        public Control Show_Info(string archivo)
        {
            return new iSDAT(Informacion(archivo));
        }
        #endregion

        private sSDAT Informacion(string archivo)
        {
            sSDAT sdat = new sSDAT();

            return sdat;
        }


    }

    public struct sSDAT
    {
        public Header generico;
        public Cabecera cabecera;
        public Symbol symbol;
        public Info info;
        public FAT fat;
        public FileBlock files;
        
    }
    public struct Cabecera
    {
        public UInt32 symbOffset;
        public UInt32 symbSize;
        public UInt32 infoOffset;
        public UInt32 infoSize;
        public UInt32 fatOffset;
        public UInt32 fatSize;
        public UInt32 fileOffset;
        public UInt32 fileSize;
        public Byte[] reserved;
    }
    public struct Symbol
    {
        public Char[] id;
        public UInt32 size;
        public UInt32 offsetSeq;
        public UInt32 offsetSeqArc;
        public UInt32 offsetBank;
        public UInt32 offsetWaveArch;
        public UInt32 offsetPlayer;
        public UInt32 offsetGroup;
        public UInt32 offsetPlayer2;
        public UInt32 offsetStream;
        public Byte[] reserved;
        public Record[] records;
        public Record2 record2;

    }
    public struct Record
    {
        public UInt32 nEntries;
        public UInt32[] entriesOffset;
        public String[] entries;
    }
    public struct Record2
    {
        public UInt32 nEntries;
        public Group[] group;
    }
    public struct Group
    {
        public UInt32 groupOffset;
        public String groupName;
        public UInt32 subRecOffset;
        public Record subRecord;
    }
    public struct Info
    {
    }
    public struct FAT
    {
    }
    public struct FileBlock
    {
    }
}
