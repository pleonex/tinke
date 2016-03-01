//
//  NitroFile.cs
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2016 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Models3D.Structures
{
    public abstract class NitroFile : FileFormat
    {
        protected NitroFile(bool hasHeaderOffsets)
        {
            HasHeaderOffsets = hasHeaderOffsets;
        }

        public NitroHeader Header { get; internal set; }
        public bool HasHeaderOffsets { get; private set; }
        public BlockCollection Blocks { get; internal set; }

        public class BlockCollection : List<NitroBlock>
        {
            public BlockCollection()
            {
            }

            public BlockCollection(int capacity)
                : base(capacity)
            {
            }

            public NitroBlock this[string name, int index] {
                get {
                    return this.FindAll(b => b.Name == name)[index];
                }
            }

            public IEnumerable this[string name] {
                get {
                    foreach (var b in this.FindAll(b => b.Name == name)) {
                        yield return b;
                    }

                    yield break;
                }
            }

            public T GetByType<T>(int index) where T : NitroBlock
            {
                return (T)this.FindAll(b => b is T)[index];
            }

            public IEnumerable<T> GetByType<T>() where T : NitroBlock
            {
                foreach (var b in this.FindAll(b => b is T)) {
                    yield return (T)b;
                }

                yield break;
            }

            public bool ContainsType(string name)
            {
                return this.FindIndex(b => b.Name == name) != -1;
            }
        }
    }
}

