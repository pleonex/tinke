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
using System.Linq;
using System.Text;
using System.IO;
using Ekona;

namespace SDAT
{
    public static class SSEQ
    {

        public static sSSEQ Read(string filePath)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(filePath));
            sSSEQ seq = new sSSEQ();

            // Generic header
            seq.generic.id = br.ReadChars(4);
            seq.generic.endianess = br.ReadUInt16();
            seq.generic.constant = br.ReadUInt16();
            seq.generic.file_size = br.ReadUInt32();
            seq.generic.header_size = br.ReadUInt16();
            seq.generic.nSection = br.ReadUInt16();

            // Data section
            seq.data.type = br.ReadChars(4);        // Should be 'DATA'
            seq.data.size = br.ReadUInt32();        
            seq.data.offset = br.ReadUInt32();      // Sould be 0x1C

            seq.data.events = new List<sEvent>();
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                sEvent evn = new sEvent();

                evn.status = br.ReadByte();
                evn.parameters = br.ReadBytes(0);

                seq.data.events.Add(evn);
            }

            br.Close();
            return seq;
        }

    }

    public struct sSSEQ
    {
        public NitroHeader generic;
        public Data data;

        public struct Data
        {
            public char[] type;
            public uint size;
            public uint offset;
            public List<sEvent> events;
        }
    }
    public struct sEvent
    {
        public byte status;
        public byte[] parameters;
    }
    public enum Event : byte
    {
        MULTITRACK_START = 0xFE,
        TRACK_OFFSET = 0x93,
        REST = 0x80,
        BANK_PROGRAM = 0x81,
        JUMP = 0x94,
        CALL =  0x95,
        RETURN = 0xFD,
        PAN = 0xC0,
        VOLUME = 0xC1,
        MASTER_VOLUME = 0xC2,
        TRANSPOSE = 0xC3,
        PITCH_BEND = 0xC4,
        PITCH_BEND_RANGE = 0xC5,
        TRACK_PRIORITY = 0xC6,
        MONO_POLY = 0xC7,
        TIE = 0xC8,
        PORTAMENTO_CONTROL = 0xC9,
        MODULATION_DEPTH = 0xCA,
        MODULATION_SPEED = 0xCB,
        MODULATION_TYPE = 0xCC,
        MODULATION_RANGE = 0xCD,
        PORTAMENTO_STATUS = 0xCE,
        PORTAMENTO_TIME = 0xCF,
        ATTACK_RATE = 0xD0,
        DECAY_RATE = 0xD1,
        SUSTAIN_RATE = 0xD2,
        RELEASE_RATE = 0xD3,
        LOOP_START_MARKER = 0xD4,
        LOOP_END_MARKER = 0xFC,
        EXPRESSION = 0xD5,
        PRINT_VARIABLES = 0xD6,
        MODULATION_DELAY = 0xE0,
        TEMPO = 0xE1,
        SWEEP_PITCH = 0xE3,
        EOT = 0xFF
    }
}
