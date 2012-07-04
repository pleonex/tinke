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
using System.Drawing;
using System.IO;
using Ekona;
using OpenTK;
using OpenTK.Graphics.OpenGL;

// Copied from:
// http://llref.emutalk.net/docs/?file=xml/bmd0.xml#xml-doc
// http://kiwi.ds.googlepages.com/nsbmd.html
// http://nocash.emubase.de/gbatek.htm#ds3dvideo
// Credits to lowlines, Kiwi.ds and Martin Korth, thanks :)

namespace _3DModels
{
    public static class BMD0
    {
        public static sBMD0 Read(string file, int id, IPluginHost pluginHost)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(file));
            sBMD0 bmd = new sBMD0();
            bmd.filePath = Path.GetTempFileName();
            File.Copy(file, bmd.filePath, true);

            // Read generic header
            bmd.header.type = br.ReadChars(4);
            bmd.header.constant = br.ReadUInt32();
            bmd.header.fileSize = br.ReadUInt32();
            bmd.header.headerSize = br.ReadUInt16();
            bmd.header.numSect = br.ReadUInt16();
            bmd.header.offset = new uint[bmd.header.numSect];
            for (int i = 0; i < bmd.header.numSect; i++)
                bmd.header.offset[i] = br.ReadUInt32();

            // Read model section
            br.BaseStream.Position = bmd.header.offset[0];
            bmd.model.type = br.ReadChars(4);
            bmd.model.size = br.ReadUInt32();

            #region Model 3DInfo block
            Info3D mdlInfo = new Info3D();

            // Header
            mdlInfo.dummy = br.ReadByte();
            mdlInfo.num_objs = br.ReadByte();
            mdlInfo.section_size = br.ReadUInt16();

            // Unknown
            mdlInfo.unknownBlock.header_size = br.ReadUInt16();
            mdlInfo.unknownBlock.section_size = br.ReadUInt16();
            mdlInfo.unknownBlock.constant = br.ReadUInt32();

            mdlInfo.unknownBlock.unknown1 = new ushort[mdlInfo.num_objs];
            mdlInfo.unknownBlock.unknown2 = new ushort[mdlInfo.num_objs];
            for (int i = 0; i < mdlInfo.num_objs; i++)
            {
                mdlInfo.unknownBlock.unknown1[i] = br.ReadUInt16();
                mdlInfo.unknownBlock.unknown2[i] = br.ReadUInt16();
            }

            // Data
            mdlInfo.infoBlock.header_size = br.ReadUInt16();
            mdlInfo.infoBlock.data_size = br.ReadUInt16();
            mdlInfo.infoBlock.infoData = new Object[mdlInfo.num_objs];
            for (int i = 0; i < mdlInfo.num_objs; i++)
            {
                mdlInfo.infoBlock.infoData[i] = br.ReadUInt32();
            }

            // Names
            mdlInfo.names = new string[mdlInfo.num_objs];
            for (int i = 0; i < mdlInfo.num_objs; i++)
                mdlInfo.names[i] = new String(br.ReadChars(0x10)).Replace("\0", "");

            bmd.model.mdlInfo = mdlInfo;
            #endregion

            bmd.model.mdlData = new sBMD0.Model.ModelData[bmd.model.mdlInfo.num_objs];
            for (int m = 0; m < bmd.model.mdlInfo.num_objs; m++)
            {
                uint modelOffset = bmd.header.offset[0] + (uint)bmd.model.mdlInfo.infoBlock.infoData[m];
                br.BaseStream.Position = modelOffset;

                #region Read Model Data block
                sBMD0.Model.ModelData data = new sBMD0.Model.ModelData();

                #region Header
                data.header.blockSize = br.ReadUInt32();
                data.header.bonesOffset = br.ReadUInt32();
                data.header.materialOffset = br.ReadUInt32();
                data.header.polygonStartOffset = br.ReadUInt32();
                data.header.polygonEndOffset = br.ReadUInt32();
                data.header.unknown1 = br.ReadByte();
                data.header.unknown2 = br.ReadByte();
                data.header.unknown3 = br.ReadByte();
                data.header.numObjects = br.ReadByte();
                data.header.numMaterial = br.ReadByte();
                data.header.numPolygon = br.ReadByte();
                data.header.unknown4 = br.ReadByte();
                data.header.scaleMode = br.ReadByte();
                data.header.unknown5 = br.ReadUInt64();
                data.header.numVertices = br.ReadUInt16();
                data.header.numSurfaces = br.ReadUInt16();
                data.header.numTriangles = br.ReadUInt16();
                data.header.numQuads = br.ReadUInt16();
                data.header.boundingX = Get_SignedFixedPoint(br.ReadUInt16());
                data.header.boundingY = Get_SignedFixedPoint(br.ReadUInt16());
                data.header.boundingZ = Get_SignedFixedPoint(br.ReadUInt16());
                data.header.boundingWidth = Get_SignedFixedPoint(br.ReadUInt16());
                data.header.boundingHeight = Get_SignedFixedPoint(br.ReadUInt16());
                data.header.boundingDepth = Get_SignedFixedPoint(br.ReadUInt16());
                data.header.unused = br.ReadUInt64();
                #endregion

                // Objetcs section
                int objSecOffset = (int)br.BaseStream.Position;
                #region 3DInfo
                mdlInfo = new Info3D();

                // Header
                mdlInfo.dummy = br.ReadByte();
                mdlInfo.num_objs = br.ReadByte();
                mdlInfo.section_size = br.ReadUInt16();

                // Unknown
                mdlInfo.unknownBlock.header_size = br.ReadUInt16();
                mdlInfo.unknownBlock.section_size = br.ReadUInt16();
                mdlInfo.unknownBlock.constant = br.ReadUInt32();

                mdlInfo.unknownBlock.unknown1 = new ushort[mdlInfo.num_objs];
                mdlInfo.unknownBlock.unknown2 = new ushort[mdlInfo.num_objs];
                for (int i = 0; i < mdlInfo.num_objs; i++)
                {
                    mdlInfo.unknownBlock.unknown1[i] = br.ReadUInt16();
                    mdlInfo.unknownBlock.unknown2[i] = br.ReadUInt16();
                }

                // Data
                mdlInfo.infoBlock.header_size = br.ReadUInt16();
                mdlInfo.infoBlock.data_size = br.ReadUInt16();
                mdlInfo.infoBlock.infoData = new Object[mdlInfo.num_objs];
                for (int i = 0; i < mdlInfo.num_objs; i++)
                {
                    mdlInfo.infoBlock.infoData[i] = br.ReadUInt32();
                }

                // Names
                mdlInfo.names = new string[mdlInfo.num_objs];
                for (int i = 0; i < mdlInfo.num_objs; i++)
                    mdlInfo.names[i] = new String(br.ReadChars(0x10)).Replace("\0", "");

                data.objects.header = mdlInfo;
                #endregion
                #region Object data
                data.objects.objData = new sBMD0.Model.ModelData.Objects.ObjData[data.objects.header.num_objs];
                for (int i = 0; i < data.objects.header.num_objs; i++)
                {
                    sBMD0.Model.ModelData.Objects.ObjData objData = new sBMD0.Model.ModelData.Objects.ObjData();
                    br.BaseStream.Position = objSecOffset + (uint)data.objects.header.infoBlock.infoData[i];

                    objData.transFlag = br.ReadUInt16();
                    objData.padding = br.ReadUInt16();
                    objData.T = (byte)(objData.transFlag & 1);
                    objData.R = (byte)((objData.transFlag >> 1) & 1);
                    objData.S = (byte)((objData.transFlag >> 2) & 1);
                    objData.P = (byte)((objData.transFlag >> 3) & 1);
                    objData.N = (byte)((objData.transFlag >> 4) & 0xF);

                    if (objData.T == 0)
                    {
                        objData.xValue = br.ReadUInt32();
                        objData.yValue = br.ReadUInt32();
                        objData.zValue = br.ReadUInt32();
                    }
                    if (objData.P == 1)
                    {
                        objData.value1 = br.ReadUInt16();
                        objData.value2 = br.ReadUInt16();
                    }
                    if (objData.S == 0)
                    {
                        objData.xScale = br.ReadUInt32();
                        objData.yScale = br.ReadUInt32();
                        objData.zScale = br.ReadUInt32();
                    }
                    if (objData.P == 0 && objData.R == 0)
                    {
                        objData.rot1 = br.ReadUInt16();
                        objData.rot2 = br.ReadUInt16();
                        objData.rot3 = br.ReadUInt16();
                        objData.rot4 = br.ReadUInt16();
                        objData.rot5 = br.ReadUInt16();
                        objData.rot6 = br.ReadUInt16();
                        objData.rot7 = br.ReadUInt16();
                        objData.rot8 = br.ReadUInt16();
                    }

                    data.objects.objData[i] = objData;
                }
                #endregion

                // Bones commands
                #region Commands
                br.BaseStream.Position = data.header.bonesOffset + modelOffset;
                byte cmd = 0;
                data.bones.commands = new List<sBMD0.Model.ModelData.Bones.Command>();
                while (cmd != 0x01)
                {
                    cmd = br.ReadByte();
                    sBMD0.Model.ModelData.Bones.Command command = new sBMD0.Model.ModelData.Bones.Command();
                    command.command = cmd;
                    command.parameters = new byte[0];

                    switch (cmd)
                    {
                        case 0x00:  // Padding
                            command.size = 0;
                            break;
                        case 0x01:  // End of bones
                            command.size = 0;
                            break;
                        case 0x02:
                            command.size = 2;
                            command.parameters = new byte[2];
                            command.parameters[0] = br.ReadByte();  // Node ID
                            command.parameters[1] = br.ReadByte();  // Visibility
                            break;
                        case 0x03:
                            command.size = 1;
                            command.parameters = new byte[1];
                            command.parameters[0] = br.ReadByte();  // Set Polygon Stack ID?
                            break;
                        case 0x04:
                            command.size = 3;
                            command.parameters = new byte[3];
                            command.parameters[0] = br.ReadByte();  // Material ID
                            command.parameters[1] = br.ReadByte();  // 0x05
                            command.parameters[2] = br.ReadByte();  // Polygon ID
                            break;
                        case 0x05:
                            command.size = 1;
                            command.parameters = new byte[1];
                            command.parameters[0] = br.ReadByte();
                            break;
                        case 0x06:
                            command.size = 3;
                            command.parameters = new byte[3];
                            command.parameters[0] = br.ReadByte();  // Object ID
                            command.parameters[1] = br.ReadByte();  // Parent ID
                            command.parameters[2] = br.ReadByte();  // Dummy 0
                            break;
                        case 0x07:
                            command.size = 1;
                            command.parameters = new byte[1];
                            command.parameters[0] = br.ReadByte();
                            break;
                        case 0x08:
                            command.size = 1;
                            command.parameters = new byte[1];
                            command.parameters[0] = br.ReadByte();
                            break;
                        case 0x09:
                            command.size = 8;
                            command.parameters = new byte[8];
                            command.parameters[0] = br.ReadByte();
                            command.parameters[1] = br.ReadByte();
                            command.parameters[2] = br.ReadByte();
                            command.parameters[3] = br.ReadByte();
                            command.parameters[4] = br.ReadByte();
                            command.parameters[5] = br.ReadByte();
                            command.parameters[6] = br.ReadByte();
                            command.parameters[7] = br.ReadByte();
                            break;
                        case 0x0B:
                            command.size = 0;   // Begin Polygon/Material pairing
                            break;
                        case 0x24:
                            command.size = 3;
                            command.parameters = new byte[3];
                            command.parameters[0] = br.ReadByte();  // Material ID
                            command.parameters[1] = br.ReadByte();  // 0x05
                            command.parameters[2] = br.ReadByte();  // Polygon ID
                            break;
                        case 0x26:
                            command.size = 4;
                            command.parameters = new byte[4];
                            command.parameters[0] = br.ReadByte();  // Object ID
                            command.parameters[1] = br.ReadByte();  // Parent ID
                            command.parameters[2] = br.ReadByte();  // Dummy 0
                            command.parameters[3] = br.ReadByte();  // Stack ID
                            break;
                        case 0x2B:
                            command.size = 0;   // End Polygon/Material Pairing
                            break;
                        case 0x44:
                            command.size = 3;
                            command.parameters = new byte[3];
                            command.parameters[0] = br.ReadByte();  // Material ID
                            command.parameters[1] = br.ReadByte();  // 0x05
                            command.parameters[2] = br.ReadByte();  // Polygon ID
                            break;
                        case 0x46:
                            command.size = 4;
                            command.parameters = new byte[4];
                            command.parameters[0] = br.ReadByte();  // Object ID
                            command.parameters[1] = br.ReadByte();  // Parent ID
                            command.parameters[2] = br.ReadByte();  // Dummy 0
                            command.parameters[3] = br.ReadByte();  // Restore ID
                            break;
                        case 0x66:
                            command.size = 5;
                            command.parameters = new byte[5];
                            command.parameters[0] = br.ReadByte();  // Object ID
                            command.parameters[1] = br.ReadByte();  // Parent ID
                            command.parameters[2] = br.ReadByte();  // Dummy 0
                            command.parameters[3] = br.ReadByte();  // Stack ID
                            command.parameters[4] = br.ReadByte();  // Restore ID
                            break;
                    }

                    data.bones.commands.Add(command);
                }
                #endregion

                // Material section
                br.BaseStream.Position = modelOffset + data.header.materialOffset;
                data.material.texOffset = br.ReadUInt16();
                data.material.paletteOffset = br.ReadUInt16();

                #region 3DInfo
                Info3D info = new Info3D();

                // Header
                info.dummy = br.ReadByte();
                info.num_objs = br.ReadByte();
                info.section_size = br.ReadUInt16();

                // Unknown
                info.unknownBlock.header_size = br.ReadUInt16();
                info.unknownBlock.section_size = br.ReadUInt16();
                info.unknownBlock.constant = br.ReadUInt32();

                info.unknownBlock.unknown1 = new ushort[info.num_objs];
                info.unknownBlock.unknown2 = new ushort[info.num_objs];
                for (int i = 0; i < info.num_objs; i++)
                {
                    info.unknownBlock.unknown1[i] = br.ReadUInt16();
                    info.unknownBlock.unknown2[i] = br.ReadUInt16();
                }

                // Data
                info.infoBlock.header_size = br.ReadUInt16();
                info.infoBlock.data_size = br.ReadUInt16();
                info.infoBlock.infoData = new Object[info.num_objs];
                for (int i = 0; i < info.num_objs; i++)
                {
                    info.infoBlock.infoData[i] = br.ReadUInt32();
                }

                // Names
                info.names = new string[info.num_objs];
                for (int i = 0; i < info.num_objs; i++)
                    info.names[i] = new String(br.ReadChars(0x10)).Replace("\0", "");

                data.material.definition = info;
                #endregion
                #region Texture Information
                br.BaseStream.Position = modelOffset + data.header.materialOffset + data.material.texOffset;
                info = new Info3D();

                // Header
                info.dummy = br.ReadByte();
                info.num_objs = br.ReadByte();
                info.section_size = br.ReadUInt16();

                // Unknown
                info.unknownBlock.header_size = br.ReadUInt16();
                info.unknownBlock.section_size = br.ReadUInt16();
                info.unknownBlock.constant = br.ReadUInt32();

                info.unknownBlock.unknown1 = new ushort[info.num_objs];
                info.unknownBlock.unknown2 = new ushort[info.num_objs];
                for (int i = 0; i < info.num_objs; i++)
                {
                    info.unknownBlock.unknown1[i] = br.ReadUInt16();
                    info.unknownBlock.unknown2[i] = br.ReadUInt16();
                }

                // Data
                info.infoBlock.header_size = br.ReadUInt16();
                info.infoBlock.data_size = br.ReadUInt16();
                info.infoBlock.infoData = new Object[info.num_objs];
                for (int i = 0; i < info.num_objs; i++)
                {
                    sBMD0.Model.ModelData.Material.TexPalData texInfo = new sBMD0.Model.ModelData.Material.TexPalData();
                    texInfo.matching_data_offset = br.ReadUInt16();
                    texInfo.num_associated_mat = br.ReadByte();
                    texInfo.dummy0 = br.ReadByte();

                    // Read the Matching data
                    long currPos = br.BaseStream.Position;
                    br.BaseStream.Position = modelOffset + data.header.materialOffset + texInfo.matching_data_offset;
                    texInfo.ID = br.ReadByte();
                    br.BaseStream.Position = currPos;

                    info.infoBlock.infoData[i] = texInfo;
                }

                // Names
                info.names = new string[info.num_objs];
                for (int i = 0; i < info.num_objs; i++)
                    info.names[i] = new String(br.ReadChars(0x10)).Replace("\0", "");

                data.material.texture = info;
                #endregion
                #region Palette Information
                br.BaseStream.Position = modelOffset + data.header.materialOffset + data.material.paletteOffset;
                info = new Info3D();

                // Header
                info.dummy = br.ReadByte();
                info.num_objs = br.ReadByte();
                info.section_size = br.ReadUInt16();

                // Unknown
                info.unknownBlock.header_size = br.ReadUInt16();
                info.unknownBlock.section_size = br.ReadUInt16();
                info.unknownBlock.constant = br.ReadUInt32();

                info.unknownBlock.unknown1 = new ushort[info.num_objs];
                info.unknownBlock.unknown2 = new ushort[info.num_objs];
                for (int i = 0; i < info.num_objs; i++)
                {
                    info.unknownBlock.unknown1[i] = br.ReadUInt16();
                    info.unknownBlock.unknown2[i] = br.ReadUInt16();
                }

                // Data
                info.infoBlock.header_size = br.ReadUInt16();
                info.infoBlock.data_size = br.ReadUInt16();
                info.infoBlock.infoData = new Object[info.num_objs];
                for (int i = 0; i < info.num_objs; i++)
                {
                    sBMD0.Model.ModelData.Material.TexPalData palInfo = new sBMD0.Model.ModelData.Material.TexPalData();
                    palInfo.matching_data_offset = br.ReadUInt16();
                    palInfo.num_associated_mat = br.ReadByte();
                    palInfo.dummy0 = br.ReadByte();

                    // Read the matching data
                    long currPos = br.BaseStream.Position;
                    br.BaseStream.Position = modelOffset + data.header.materialOffset + palInfo.matching_data_offset;
                    palInfo.ID = br.ReadByte();
                    br.BaseStream.Position = currPos;

                    info.infoBlock.infoData[i] = palInfo;
                }

                // Names
                info.names = new string[info.num_objs];
                for (int i = 0; i < info.num_objs; i++)
                    info.names[i] = new String(br.ReadChars(0x10)).Replace("\0", "");

                data.material.palette = info;
                #endregion
                #region Material definition
                data.material.material = new sBMD0.Model.ModelData.Material.MatDef[data.material.definition.num_objs];
                for (int i = 0; i < data.material.definition.num_objs; i++)
                {
                    br.BaseStream.Position = modelOffset + data.header.materialOffset + (uint)data.material.definition.infoBlock.infoData[i];
                    br.BaseStream.Position += 4 + 18;       // 0x04 for Texture and Palette offset, 18 unknown but it works :)
                    data.material.material[i].definition = br.ReadBytes(0x2E);

                    if (i < data.material.texture.names.Length)
                    {
                        int texID = ((sBMD0.Model.ModelData.Material.TexPalData)data.material.texture.infoBlock.infoData[i]).ID;
                        data.material.material[texID].texName = data.material.texture.names[i];
                        data.material.material[texID].texID = (byte)i;
                    }
                    if (i < data.material.palette.names.Length)
                    {
                        int palID = ((sBMD0.Model.ModelData.Material.TexPalData)data.material.palette.infoBlock.infoData[i]).ID;
                        data.material.material[palID].palName = data.material.palette.names[i];
                        data.material.material[palID].palID = (byte)i;
                    }
                }
                for (int i = 0; i < data.material.definition.num_objs; i++)
                {
                    sBMD0.Model.ModelData.Material.MatDef mat = data.material.material[i];
                    if (mat.palName != null && mat.texName != null)
                        continue;

                    Console.WriteLine("Trying to fix...");
                    if (mat.texName == null)
                    {
                        for (int t = 0; t < data.material.texture.infoBlock.infoData.Length; t++)
                        {
                            sBMD0.Model.ModelData.Material.TexPalData tex = (sBMD0.Model.ModelData.Material.TexPalData)data.material.texture.infoBlock.infoData[t];
                            if (tex.num_associated_mat <= 1)
                                continue;

                            mat.texName = data.material.texture.names[t];
                            mat.texID = (byte)t;
                            tex.num_associated_mat--;
                            data.material.texture.infoBlock.infoData[t] = tex;
                            break;
                        }
                    }
                    if (mat.palName == null)
                    {
                        for (int p = 0; p < data.material.palette.infoBlock.infoData.Length; p++)
                        {
                            sBMD0.Model.ModelData.Material.TexPalData pal = (sBMD0.Model.ModelData.Material.TexPalData)data.material.palette.infoBlock.infoData[p];
                            if (pal.num_associated_mat <= 1)
                                continue;

                            mat.palName = data.material.palette.names[p];
                            mat.palID = (byte)p;
                            pal.num_associated_mat--;
                            data.material.palette.infoBlock.infoData[p] = pal;
                            break;
                        }
                    }
                    data.material.material[i] = mat;
                }
                BTX0.Match_Textures(ref data.material.material, data.material.palette.names, data.material.texture.names);
                #endregion

                // Polygon section
                br.BaseStream.Position = modelOffset + data.header.polygonStartOffset;
                #region 3DInfo
                info = new Info3D();

                // Header
                info.dummy = br.ReadByte();
                info.num_objs = br.ReadByte();
                info.section_size = br.ReadUInt16();

                // Unknown
                info.unknownBlock.header_size = br.ReadUInt16();
                info.unknownBlock.section_size = br.ReadUInt16();
                info.unknownBlock.constant = br.ReadUInt32();

                info.unknownBlock.unknown1 = new ushort[info.num_objs];
                info.unknownBlock.unknown2 = new ushort[info.num_objs];
                for (int i = 0; i < info.num_objs; i++)
                {
                    info.unknownBlock.unknown1[i] = br.ReadUInt16();
                    info.unknownBlock.unknown2[i] = br.ReadUInt16();
                }

                // Data
                info.infoBlock.header_size = br.ReadUInt16();
                info.infoBlock.data_size = br.ReadUInt16();
                info.infoBlock.infoData = new Object[info.num_objs];
                for (int i = 0; i < info.num_objs; i++)
                {
                    info.infoBlock.infoData[i] = br.ReadUInt32();
                }

                // Names
                info.names = new string[info.num_objs];
                for (int i = 0; i < info.num_objs; i++)
                    info.names[i] = new String(br.ReadChars(0x10)).Replace("\0", "");

                data.polygon.header = info;
                #endregion
                #region Polygon definition
                data.polygon.definition = new sBMD0.Model.ModelData.Polygon.Definition[data.polygon.header.num_objs];
                for (int i = 0; i < data.polygon.header.num_objs; i++)
                {
                    br.BaseStream.Position = modelOffset + data.header.polygonStartOffset + (uint)data.polygon.header.infoBlock.infoData[i];
                    long currPos = br.BaseStream.Position;

                    sBMD0.Model.ModelData.Polygon.Definition def = new sBMD0.Model.ModelData.Polygon.Definition();
                    def.unknown1 = br.ReadUInt32();
                    def.unknown2 = br.ReadUInt32();
                    def.display_offset = br.ReadUInt32() + (uint)currPos;
                    def.display_size = br.ReadUInt32();
                    data.polygon.definition[i] = def;
                }
                #endregion
                #region Display list
                data.polygon.display = new sBMD0.Model.ModelData.Polygon.Display[data.polygon.header.num_objs];
                for (int i = 0; i < data.polygon.header.num_objs; i++)
                {
                    br.BaseStream.Position = data.polygon.definition[i].display_offset;
                    byte[] display_list = br.ReadBytes((int)data.polygon.definition[i].display_size);

                    data.polygon.display[i].commands = new List<Command>();
                    for (int l = 0; l + 4 < display_list.Length; )
                    {
                        byte[] cmdID = new byte[] { display_list[l], display_list[l + 1], display_list[l + 2], display_list[l + 3] };
                        l += 4;

                        Command[] commands = new Command[4];
                        for (int c = 0; c < 4; c++)
                        {
                            commands[c] = new Command();
                            commands[c].cmd = cmdID[c];

                            int cmd_size = Get_CommandSize(cmdID[c]);
                            commands[c].param = new uint[cmd_size];
                            for (int p = 0; p < cmd_size && l + 4 < display_list.Length; p++, l += 4)
                                commands[c].param[p] = BitConverter.ToUInt32(display_list, l);
                        }

                        data.polygon.display[i].commands.AddRange(commands);
                    }
                }
                #endregion

                bmd.model.mdlData[m] = data;
                #endregion
            }

            Write_Info(bmd, pluginHost.Get_Language());

            if (bmd.header.numSect == 2)    // There is a Tex0 section
            {
                br.BaseStream.Position = bmd.header.offset[1];
                bmd.texture = BTX0.Read_Section(ref br, bmd.header.offset[1], pluginHost.Get_Language());
            }

            br.Close();
            return bmd;
        }
        public static void Write_Info(sBMD0 bmd, string lang)
        {
            try
            {
                System.Xml.Linq.XElement xml = System.Xml.Linq.XElement.Load(System.Windows.Forms.Application.StartupPath + Path.DirectorySeparatorChar +
                    "Plugins" + Path.DirectorySeparatorChar + "3DModelsLang.xml");
                xml = xml.Element(lang).Element("BMD0");

                Console.WriteLine("<pre><b>" + xml.Element("S00").Value + "</b>");
                Console.WriteLine("<b>" + xml.Element("S01").Value + "</b>");
                Console.WriteLine(xml.Element("S02").Value, bmd.header.numSect.ToString());
                Console.WriteLine(xml.Element("S03").Value + "<br>", bmd.model.mdlInfo.num_objs);
                for (int m = 0; m < bmd.model.mdlInfo.num_objs; m++)
                {
                    Console.WriteLine("-------------------<h4>" + xml.Element("S04").Value + "</h4>", m.ToString(), bmd.model.mdlInfo.names[m]);

                    // Header
                    Console.WriteLine("<u>" + xml.Element("S05").Value + "</u>");
                    Console.WriteLine(xml.Element("S06").Value, bmd.model.mdlData[m].header.bonesOffset.ToString("x"));
                    Console.WriteLine(xml.Element("S07").Value, bmd.model.mdlData[m].header.materialOffset.ToString("x"));
                    Console.WriteLine(xml.Element("S08").Value, bmd.model.mdlData[m].header.polygonStartOffset.ToString("x"));
                    Console.WriteLine(xml.Element("S09").Value, bmd.model.mdlData[m].header.polygonEndOffset.ToString("x"));
                    Console.WriteLine(xml.Element("S0A").Value, bmd.model.mdlData[m].header.unknown1.ToString("x"));
                    Console.WriteLine(xml.Element("S0B").Value, bmd.model.mdlData[m].header.unknown2.ToString("x"));
                    Console.WriteLine(xml.Element("S0C").Value, bmd.model.mdlData[m].header.unknown3.ToString("x"));
                    Console.WriteLine(xml.Element("S0D").Value, bmd.model.mdlData[m].header.numObjects.ToString());
                    Console.WriteLine(xml.Element("S0E").Value, bmd.model.mdlData[m].header.numMaterial.ToString());
                    Console.WriteLine(xml.Element("S0F").Value, bmd.model.mdlData[m].header.numPolygon.ToString());
                    Console.WriteLine(xml.Element("S10").Value, bmd.model.mdlData[m].header.unknown4.ToString("x"));
                    Console.WriteLine(xml.Element("S11").Value, bmd.model.mdlData[m].header.scaleMode.ToString("x"));
                    Console.WriteLine(xml.Element("S12").Value, bmd.model.mdlData[m].header.unknown5.ToString("x"));
                    Console.WriteLine(xml.Element("S13").Value, bmd.model.mdlData[m].header.numVertices.ToString());
                    Console.WriteLine(xml.Element("S14").Value, bmd.model.mdlData[m].header.numSurfaces.ToString());
                    Console.WriteLine(xml.Element("S15").Value, bmd.model.mdlData[m].header.numTriangles.ToString());
                    Console.WriteLine(xml.Element("S16").Value, bmd.model.mdlData[m].header.numQuads.ToString());
                    Console.WriteLine(xml.Element("S17").Value, bmd.model.mdlData[m].header.boundingX.ToString());
                    Console.WriteLine(xml.Element("S18").Value, bmd.model.mdlData[m].header.boundingY.ToString());
                    Console.WriteLine(xml.Element("S19").Value, bmd.model.mdlData[m].header.boundingZ.ToString());
                    Console.WriteLine(xml.Element("S1A").Value, bmd.model.mdlData[m].header.boundingWidth.ToString());
                    Console.WriteLine(xml.Element("S1B").Value, bmd.model.mdlData[m].header.boundingHeight.ToString());
                    Console.WriteLine(xml.Element("S1C").Value, bmd.model.mdlData[m].header.boundingDepth.ToString());

                    // Object definition
                    Console.WriteLine("<br><u>" + xml.Element("S24").Value + "</u>");
                    for (int i = 0; i < bmd.model.mdlData[m].objects.header.num_objs; i++)
                    {
                        sBMD0.Model.ModelData.Objects.ObjData obj = bmd.model.mdlData[m].objects.objData[i];
                        Console.WriteLine("|__" + xml.Element("S1D").Value, i.ToString(), bmd.model.mdlData[m].objects.header.names[i]);
                        Console.WriteLine(xml.Element("S1E").Value, obj.transFlag.ToString("x"));
                        Console.WriteLine(xml.Element("S1F").Value, obj.N, obj.P, obj.S, obj.R, obj.T);
                        if (obj.T == 0)
                            Console.WriteLine(xml.Element("S20").Value, obj.xValue, obj.yValue, obj.zValue);
                        if (obj.P == 1)
                            Console.WriteLine(xml.Element("S21").Value, obj.value1, obj.value2);
                        if (obj.S == 0)
                            Console.WriteLine(xml.Element("S22").Value, obj.xScale, obj.yScale, obj.zScale);
                        if (obj.P == 0 && obj.R == 0)
                            Console.WriteLine(xml.Element("S23").Value, obj.rot1, obj.rot2, obj.rot3, obj.rot4, obj.rot5, obj.rot6,
                                obj.rot7, obj.rot8);
                    }

                    // Bones commands
                    Console.WriteLine("<u>" + xml.Element("S25").Value + "</u><ol>");
                    for (int i = 0; i < bmd.model.mdlData[m].bones.commands.Count; i++)
                    {
                        Console.WriteLine("<li>" + xml.Element("S26").Value + "</li>", i, bmd.model.mdlData[m].bones.commands[i].command,
                            bmd.model.mdlData[m].bones.commands[i].size, BitConverter.ToString(bmd.model.mdlData[m].bones.commands[i].parameters, 0));
                    }
                    Console.WriteLine("</ol>");

                    // Material section
                    Console.WriteLine("<u>" + xml.Element("S27").Value + "</u>");
                    for (int i = 0; i < bmd.model.mdlData[m].material.definition.num_objs; i++)
                    {
                        Console.WriteLine(xml.Element("S2A").Value, i, bmd.model.mdlData[m].material.definition.names[i]);
                        Console.WriteLine("|__" + xml.Element("S2B").Value, BitConverter.ToString(bmd.model.mdlData[m].material.material[i].definition, 0));

                    }

                    // Texture section
                    Console.WriteLine("*" + xml.Element("S2C").Value);
                    Console.WriteLine(xml.Element("S28").Value, bmd.model.mdlData[m].material.texOffset.ToString("x"));
                    for (int i = 0; i < bmd.model.mdlData[m].material.texture.num_objs; i++)
                    {
                        sBMD0.Model.ModelData.Material.TexPalData texInfo = (sBMD0.Model.ModelData.Material.TexPalData)bmd.model.mdlData[m].material.texture.infoBlock.infoData[i];
                        Console.WriteLine(xml.Element("S2D").Value, i, bmd.model.mdlData[m].material.texture.names[i]);
                        Console.WriteLine("|__" + xml.Element("S2E").Value, texInfo.matching_data_offset.ToString("x"));
                        Console.WriteLine("|__ID: 0x{0:X}", texInfo.ID);
                        Console.WriteLine("|__" + xml.Element("S2F").Value, texInfo.num_associated_mat.ToString());
                    }

                    // Palette section
                    Console.Write("*" + xml.Element("S30").Value);
                    Console.WriteLine(xml.Element("S29").Value, bmd.model.mdlData[m].material.paletteOffset.ToString("x"));
                    for (int i = 0; i < bmd.model.mdlData[m].material.palette.num_objs; i++)
                    {
                        sBMD0.Model.ModelData.Material.TexPalData palInfo = (sBMD0.Model.ModelData.Material.TexPalData)bmd.model.mdlData[m].material.palette.infoBlock.infoData[i];
                        Console.WriteLine(xml.Element("S31").Value, i, bmd.model.mdlData[m].material.palette.names[i]);
                        Console.WriteLine("|__" + xml.Element("S2E").Value, palInfo.matching_data_offset.ToString("x"));
                        Console.WriteLine("|__ID: 0x{0:X}", palInfo.ID);
                        Console.WriteLine("|__" + xml.Element("S2F").Value, palInfo.num_associated_mat.ToString());
                    }

                    // Polygon section
                    Console.WriteLine("<u>" + xml.Element("S32").Value + "</u>");
                    for (int i = 0; i < bmd.model.mdlData[m].polygon.header.num_objs; i++)
                    {
                        Console.WriteLine(xml.Element("S33").Value, i, bmd.model.mdlData[m].polygon.header.names[i]);
                        Console.WriteLine("|__" + xml.Element("S34").Value,
                            bmd.model.mdlData[m].polygon.definition[i].display_offset.ToString("x"),
                            bmd.model.mdlData[m].polygon.definition[i].display_size.ToString("x"),
                            bmd.model.mdlData[m].polygon.definition[i].unknown1.ToString("x"),
                            bmd.model.mdlData[m].polygon.definition[i].unknown2.ToString("x"));

                        Console.WriteLine("|__" + xml.Element("S35").Value + "<ol>");
                        for (int c = 0; c < bmd.model.mdlData[m].polygon.display[i].commands.Count; c++)
                        {
                            Console.Write("<li>" + (GeometryCmd)bmd.model.mdlData[m].polygon.display[i].commands[c].cmd);
                            Console.Write(" (0x" + bmd.model.mdlData[m].polygon.display[i].commands[c].cmd.ToString("x") + ')');

                            if (bmd.model.mdlData[m].polygon.display[i].commands[c].param.Length != 0)
                            {
                                Console.Write("  - " + xml.Element("S36").Value);
                                for (int p = 0; p < bmd.model.mdlData[m].polygon.display[i].commands[c].param.Length; p++)
                                {
                                    Console.Write(" 0x" + bmd.model.mdlData[m].polygon.display[i].commands[c].param[p].ToString("x"));
                                    if (p + 1 != bmd.model.mdlData[m].polygon.display[i].commands[c].param.Length)
                                        Console.Write(" |");
                                }
                            }
                            Console.WriteLine("</li>");
                        }
                        Console.Write("</ol>");
                    }


                }

                Console.WriteLine("<br>" + xml.Element("S37").Value + "</pre>");
            }
            catch { throw new NotSupportedException("There was an error reading the language file"); }
        }

        public static double Get_SignedFixedPoint(ushort value)
        {
            double point = ((value >> 12) & 7);

            point += (double)(value & 0xFFF) / 0x1000;

            // sign
            if ((value >> 15) == 1)
                point = -point;

            return point;
        }
        public static float Get_Double(int value, bool signed, int integer, int fractional)
        {
            int integerMask = 0;
            float point = 0;

            if (signed)
            {
                if ((value >> (integer + fractional)) == 1)
                {
                    integerMask = (int)Math.Pow(2, integer + 1) - 1;
                    int intPart = ((value >> fractional) & integerMask);
                    point = intPart - (int)Math.Pow(2, integer + 1);
                }
                else
                {
                    integerMask = (int)Math.Pow(2, integer) - 1;
                    point = ((value >> fractional) & integerMask);
                }
            }


            // Fractional part
            int fractionalMask = (int)Math.Pow(2, fractional) - 1;
            point += (float)(value & fractionalMask) / (fractionalMask + 1);


            return point;
        }

        public static int Get_CommandSize(byte cmd)
        {
            switch (cmd)
            {
                case 0: return 0;

                case 0x10: return 1;
                case 0x11: return 0;
                case 0x12: return 1;
                case 0x13: return 1;
                case 0x14: return 1;
                case 0x15: return 0;
                case 0x16: return 16;
                case 0x17: return 12;
                case 0x18: return 16;
                case 0x19: return 12;
                case 0x1A: return 9;
                case 0x1B: return 3;
                case 0x1C: return 3;

                case 0x20: return 1;
                case 0x21: return 1;
                case 0x22: return 1;
                case 0x23: return 2;
                case 0x24: return 1;
                case 0x25: return 1;
                case 0x26: return 1;
                case 0x27: return 1;
                case 0x28: return 1;

                case 0x29: return 1;
                case 0x2A: return 1;
                case 0x2B: return 1;

                case 0x30: return 1;
                case 0x31: return 1;
                case 0x32: return 1;
                case 0x33: return 1;
                case 0x34: return 32;

                case 0x40: return 1;
                case 0x41: return 0;

                case 0x50: return 1;

                case 0x60: return 1;

                case 0x70: return 3;
                case 0x71: return 2;
                case 0x72: return 1;

                default:
                    return 0;
            }
        }

        public static void GeometryCommands(List<Command> geoCmd)
        {
            OpenTK.Vector3 vector = new OpenTK.Vector3();

            for (int i = 0; i < geoCmd.Count; i++)
            {
                switch ((GeometryCmd)geoCmd[i].cmd)
                {
                    case GeometryCmd.NOP:
                        break;
                    case GeometryCmd.MTX_MODE:
                        break;
                    case GeometryCmd.MTX_PUSH:
                        break;
                    case GeometryCmd.MTX_POP:
                        break;
                    case GeometryCmd.MTX_STORE:
                        break;
                    case GeometryCmd.MTX_RESTORE:
                        break;
                    case GeometryCmd.MTX_IDENTITY:
                        break;
                    case GeometryCmd.MTX_LOAD_4x4:
                        break;
                    case GeometryCmd.MTX_LOAD_4x3:
                        break;
                    case GeometryCmd.MTX_MULT_4x4:
                        break;
                    case GeometryCmd.MTX_MULT_4x3:
                        break;
                    case GeometryCmd.MTX_MULT_3x3:
                        break;
                    case GeometryCmd.MTX_SCALE:
                        break;
                    case GeometryCmd.MTX_TRANS:
                        break;

                    #region Vertex commands
                    // Multiply by the clipmatrix
                    case GeometryCmd.VTX_16:
                        vector.X = Get_Double((int)(geoCmd[i].param[0] & 0xFFFF), true, 3, 12);
                        vector.Y = Get_Double((int)(geoCmd[i].param[0] >> 16), true, 3, 12);
                        vector.Z = Get_Double((int)(geoCmd[i].param[1] & 0xFFFF), true, 3, 12);

                        GL.Vertex3(vector);
                        break;

                    case GeometryCmd.VTX_10:
                        vector.X = Get_Double((int)(geoCmd[i].param[0] & 0x3FF), true, 3, 6);
                        vector.Y = Get_Double((int)((geoCmd[i].param[0] >> 10) & 0x3FF), true, 3, 6);
                        vector.Z = Get_Double((int)(geoCmd[i].param[0] >> 20), true, 3, 6);

                        GL.Vertex3(vector);
                        break;

                    case GeometryCmd.VTX_XY:
                        vector.X = Get_Double((int)(geoCmd[i].param[0] & 0xFFFF), true, 3, 12);
                        vector.Y = Get_Double((int)(geoCmd[i].param[0] >> 16), true, 3, 12);

                        GL.Vertex3(vector);

                        break;

                    case GeometryCmd.VTX_XZ:
                        vector.X = Get_Double((int)(geoCmd[i].param[0] & 0xFFFF), true, 3, 12);
                        vector.Z = Get_Double((int)(geoCmd[i].param[0] >> 16), true, 3, 12);

                        GL.Vertex3(vector);

                        break;

                    case GeometryCmd.VTX_YZ:
                        vector.Y = Get_Double((int)(geoCmd[i].param[0] & 0xFFFF), true, 3, 12);
                        vector.Z = Get_Double((int)(geoCmd[i].param[0] >> 16), true, 3, 12);

                        GL.Vertex3(vector);

                        break;

                    case GeometryCmd.VTX_DIFF:
                        float diffX, diffY, diffZ;

                        diffX = Get_Double((int)(geoCmd[i].param[0] & 0x3FF), true, 0, 9);
                        diffY = Get_Double((int)((geoCmd[i].param[0] >> 10) & 0x3FFF), true, 0, 9);
                        diffZ = Get_Double((int)(geoCmd[i].param[0] >> 20), true, 0, 9);

                        vector.X += (diffX / 8);
                        vector.Y += (diffY / 8);
                        vector.Z += (diffZ / 8);

                        GL.Vertex3(vector);
                        break;
                    #endregion

                    case GeometryCmd.COLOR:
                        // Convert the param to RGB555 color
                        int r = (int)(geoCmd[i].param[0] & 0x1F);
                        int g = (int)((geoCmd[i].param[0] >> 5) & 0x1F);
                        int b = (int)((geoCmd[i].param[0] >> 10) & 0x1F);

                        GL.Color3((float)r / 31.0f, (float)g / 31.0f, (float)b / 31.0f);
                        break;
                    case GeometryCmd.POLYGON_ATTR:
                        break;

                    #region Texture attributes
                    case GeometryCmd.TEXCOORD:
                        double s, t;
                        s = Get_Double((int)(geoCmd[i].param[0] & 0xFFFF), true, 11, 4);
                        t = Get_Double((int)(geoCmd[i].param[0] >> 16), true, 11, 4);
                        GL.TexCoord2(s, t);
                        break;

                    case GeometryCmd.TEXIMAGE_PARAM:
                        break;
                    case GeometryCmd.PLTT_BASE:
                        break;
                    #endregion

                    case GeometryCmd.DIF_AMB:
                        break;
                    case GeometryCmd.SPE_EMI:
                        break;
                    case GeometryCmd.LIGHT_VECTOR:
                        break;
                    case GeometryCmd.LIGHT_COLOR:
                        break;
                    case GeometryCmd.SHININESS:
                        break;

                    case GeometryCmd.NORMAL:
                        float x, y, z;
                        x = Get_Double((int)(geoCmd[i].param[0] & 0x3FFF), true, 0, 9);
                        y = Get_Double((int)((geoCmd[i].param[0] >> 10) & 0x3FFF), true, 0, 9);
                        z = Get_Double((int)(geoCmd[i].param[0] >> 20), true, 0, 9);

                        // Multiplay by the directional matrix
                        GL.Normal3(x, y, z);
                        break;

                    case GeometryCmd.BEGIN_VTXS:
                        if (geoCmd[i].param[0] == 0)
                            GL.Begin(BeginMode.Triangles);
                        else if (geoCmd[i].param[0] == 1)
                            GL.Begin(BeginMode.Quads);
                        else if (geoCmd[i].param[0] == 2)
                            GL.Begin(BeginMode.TriangleStrip);
                        else if (geoCmd[i].param[0] == 3)
                            GL.Begin(BeginMode.QuadStrip);
                        break;
                    case GeometryCmd.END_VTXS:
                        GL.End();
                        break;

                    case GeometryCmd.SWAP_BUFFERS:
                        break;
                    case GeometryCmd.VIEWPORT:
                        break;
                    case GeometryCmd.BOX_TEST:
                        break;
                    case GeometryCmd.POS_TEST:
                        break;
                    case GeometryCmd.VEC_TEST:
                        break;
                    default:
                        break;
                }
            }

            GL.Flush();
        }

    }

    public struct sBMD0
    {
        public ushort id;
        public string filePath;
        public Header header;
        public Model model;
        public sBTX0.Texture texture;

        public struct Header
        {
            public char[] type;
            public uint constant;
            public uint fileSize;
            public ushort headerSize;
            public ushort numSect;
            public uint[] offset;
        }
        public struct Model
        {
            public char[] type;
            public uint size;

            public Info3D mdlInfo;
            public ModelData[] mdlData;

            public struct ModelData
            {
                public Header header;
                public Objects objects;
                public Bones bones;

                public Material material;

                public Polygon polygon;

                public struct Header
                {
                    public uint blockSize;
                    public uint bonesOffset;
                    public uint materialOffset;
                    public uint polygonStartOffset;
                    public uint polygonEndOffset;
                    public byte unknown1;
                    public byte unknown2;
                    public byte unknown3;
                    public byte numObjects;
                    public byte numMaterial;
                    public byte numPolygon;
                    public byte unknown4;
                    public byte scaleMode;
                    public ulong unknown5;
                    public ushort numVertices;
                    public ushort numSurfaces;
                    public ushort numTriangles;
                    public ushort numQuads;
                    public double boundingX;
                    public double boundingY;
                    public double boundingZ;
                    public double boundingWidth;
                    public double boundingHeight;
                    public double boundingDepth;
                    public ulong unused;
                }
                public struct Objects
                {
                    public Info3D header;
                    public ObjData[] objData;

                    public struct ObjData
                    {
                        public ushort transFlag;
                        public ushort padding;

                        public byte N;
                        public byte P;
                        public byte S;
                        public byte R;
                        public byte T;

                        // If T=0
                        public uint xValue;
                        public uint yValue;
                        public uint zValue;

                        // If P=1
                        public ushort value1;
                        public ushort value2;

                        // If S=0
                        public uint xScale;
                        public uint yScale;
                        public uint zScale;

                        // If P=0 && R=0
                        public ushort rot1;
                        public ushort rot2;
                        public ushort rot3;
                        public ushort rot4;
                        public ushort rot5;
                        public ushort rot6;
                        public ushort rot7;
                        public ushort rot8;
                    }
                }
                public struct Bones
                {
                    public List<Command> commands;

                    public struct Command
                    {
                        public byte command;
                        public byte size;
                        public byte[] parameters;
                    }
                }
                public struct Material
                {
                    public uint texOffset;
                    public uint paletteOffset;

                    public Info3D definition;
                    public Info3D texture;
                    public Info3D palette;
                    public MatDef[] material;

                    public struct TexPalData
                    {
                        public ushort matching_data_offset;
                        public byte ID;
                        public byte num_associated_mat;
                        public byte dummy0;
                    }
                    public struct MatDef
                    {
                        public byte[] definition;   // Usually 48 bytes

                        public string texName;
                        public byte texID;
                        public string palName;
                        public byte palID;
                    }

                }
                public struct Polygon
                {
                    public Info3D header;
                    public Definition[] definition;
                    public Display[] display;

                    public struct Definition
                    {
                        public uint unknown1;
                        public uint unknown2;
                        public uint display_offset;
                        public uint display_size;
                    }
                    public struct Display
                    {
                        public List<Command> commands;
                        public int materialAssoc;
                        public int materialID;
                    }
                }
            }
        }
    }

    public struct Command
    {
        public byte cmd;
        public uint[] param;
    }
    public enum GeometryCmd : byte
    {
        Unknown,
        NOP = 0x00,
        MTX_MODE = 0x10,
        MTX_PUSH = 0x11,
        MTX_POP = 0x12,
        MTX_STORE = 0x13,
        MTX_RESTORE = 0x14,
        MTX_IDENTITY = 0x15,
        MTX_LOAD_4x4 = 0x16,
        MTX_LOAD_4x3 = 0x17,
        MTX_MULT_4x4 = 0x18,
        MTX_MULT_4x3 = 0x19,
        MTX_MULT_3x3 = 0x1A,
        MTX_SCALE = 0x1B,
        MTX_TRANS = 0x1C,
        COLOR = 0x20,
        NORMAL = 0x21,
        TEXCOORD = 0x22,
        VTX_16 = 0x23,
        VTX_10 = 0x24,
        VTX_XY = 0x25,
        VTX_XZ = 0x26,
        VTX_YZ = 0x27,
        VTX_DIFF = 0x28,
        POLYGON_ATTR = 0x29,
        TEXIMAGE_PARAM = 0x2A,
        PLTT_BASE = 0x2B,
        DIF_AMB = 0x30,
        SPE_EMI = 0x31,
        LIGHT_VECTOR = 0x32,
        LIGHT_COLOR = 0x33,
        SHININESS = 0x34,
        BEGIN_VTXS = 0x40,
        END_VTXS = 0x41,
        SWAP_BUFFERS = 0x50,
        VIEWPORT = 0x60,
        BOX_TEST = 0x70,
        POS_TEST = 0x71,
        VEC_TEST = 0x72,
    }
}
