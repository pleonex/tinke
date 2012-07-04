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
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Ekona;
using Ekona.Images;

namespace Common
{
    class TGA
    {
        string archivo;
        IPluginHost pluginHost;

        public TGA(IPluginHost pluginHost, string archivo)
        {
            this.pluginHost = pluginHost;
            this.archivo = archivo;
        }

        public Control Show_Info()
        {
            return new BasicControl(Leer(), pluginHost);
        }

        private Bitmap Leer()
        {
            Bitmap imagen;

            BinaryReader br = new BinaryReader(File.OpenRead(archivo));
            sTGA tga = new sTGA();

            // Lee la cabecera
            tga.header.image_id_length = br.ReadByte();
            tga.header.colorMap = br.ReadBoolean();
            #region Image Type
            switch (br.ReadByte())
            {
                case 0x00:
                    tga.header.image_type = ImageType.none;
                    break;
                case 0x01:
                    tga.header.image_type = ImageType.Uncompressed_ColorMapped;
                    break;
                case 0x02:
                    tga.header.image_type = ImageType.Uncompressed_TrueColor;
                    break;
                case 0x03:
                    tga.header.image_type = ImageType.Uncompressed_BlackWhite;
                    break;
                case 0x09:
                    tga.header.image_type = ImageType.RLE_ColorMapped;
                    break;
                case 0x0A:
                    tga.header.image_type = ImageType.RLE_TrueColor;
                    break;
                case 0x0B:
                    tga.header.image_type = ImageType.RLE_BlackWhite;
                    break;
                default:
                    tga.header.image_type = ImageType.noSopported;
                    break;
            }
            #endregion
            tga.header.colorMap_spec.first_entry_index = br.ReadUInt16();
            tga.header.colorMap_spec.length = br.ReadUInt16();
            tga.header.colorMap_spec.entry_size = br.ReadByte();
            tga.header.image_spec.posX = br.ReadUInt16();
            tga.header.image_spec.posY = br.ReadUInt16();
            tga.header.image_spec.width = br.ReadUInt16();
            tga.header.image_spec.height = br.ReadUInt16();
            tga.header.image_spec.depth = br.ReadByte();
            tga.header.image_spec.descriptor = br.ReadByte();
            tga.imageData.image_id = new String(br.ReadChars(tga.header.image_id_length));
            if (tga.header.colorMap)
            {
                tga.imageData.colorMap = new Color[tga.header.colorMap_spec.length / (tga.header.colorMap_spec.entry_size / 8)];
                switch (tga.header.colorMap_spec.entry_size)
                {
                    case 24:
                        for (int c = 0; c < tga.imageData.colorMap.Length; c++)
                        {
                            Color newColor = Color.FromArgb(br.ReadByte(), br.ReadByte(), br.ReadByte());
                            tga.imageData.colorMap[c] = newColor;
                        }
                        br.ReadBytes(tga.header.colorMap_spec.length - tga.imageData.colorMap.Length * 3);
                        break;
                }
            }
            imagen = new Bitmap(tga.header.image_spec.width, tga.header.image_spec.height);
            #region Obtener colores de pixels
            Color[] colores = new Color[tga.header.image_spec.height * tga.header.image_spec.width];
            int i = 0;
            switch (tga.header.image_type)
            {

                case TGA.ImageType.RLE_TrueColor:
                    // Primero descomprimimos
                    long pos = br.BaseStream.Position;
                    br.Close();
                    tga.imageData.image = RLE.Descomprimir_Pixel(archivo, ref pos, tga.header.image_spec.depth,
                                                          tga.header.image_spec.width, tga.header.image_spec.height);
                    br = new BinaryReader(File.OpenRead(archivo));
                    br.BaseStream.Position = pos;
                    // Luego convertimos los colores
                    for (int j = 0; j < colores.Length; j++)
                    {
                        if (tga.header.image_spec.depth == 0x20)
                            colores[j] = Color.FromArgb(tga.imageData.image[4 * j + 3], tga.imageData.image[4 * j],
                                                        tga.imageData.image[4 * j + 1], tga.imageData.image[4 * j + 2]);

                        else
                            colores[j] = Color.FromArgb(255, tga.imageData.image[3 * j],
                                                        tga.imageData.image[3 * j + 1], tga.imageData.image[3 * j + 2]);
                    }
                    // Set pixels colors
                    for (int y = tga.header.image_spec.height - 1; y > 0; y--)
                    {
                        for (int x = 0; x < tga.header.image_spec.width; x++)
                        {
                            imagen.SetPixel(x, y, colores[i]);
                            i++;
                        }
                    }
                    break;

                case TGA.ImageType.Uncompressed_TrueColor:
                    colores = Actions.BGR555ToColor(br.ReadBytes(tga.header.image_spec.height * tga.header.image_spec.width * 2));
                    for (int y = tga.header.image_spec.height - 1; y > 0; y--)
                    {
                        for (int x = 0; x < tga.header.image_spec.width; x++)
                        {
                            imagen.SetPixel(x, y, colores[i]);
                            i++;
                        }
                    }

                    break;

                case TGA.ImageType.Uncompressed_ColorMapped:
                    for (int y = 0; y < tga.header.image_spec.height; y++)
                    {
                        for (int x = 0; x < tga.header.image_spec.width; x++)
                        {
                            imagen.SetPixel(x, y, tga.imageData.colorMap[br.ReadByte()]);
                            i++;
                        }
                    }
                    break;
                case TGA.ImageType.Uncompressed_BlackWhite:
                case TGA.ImageType.RLE_BlackWhite:
                case TGA.ImageType.RLE_ColorMapped:
                case TGA.ImageType.noSopported:
                default:
                    throw new Exception("Invalid value for ImageType");
            }
            #endregion

            br.Close();
            return imagen;
        }

        struct sTGA
        {
            public Header header;
            public ImageData imageData;
        }
        struct Header
        {
            public byte image_id_length;
            public bool colorMap;
            public ImageType image_type;
            public ColorMap_Spec colorMap_spec;
            public Image_Spec image_spec;
        }
        struct ImageData
        {
            public string image_id;
            public Color[] colorMap;
            public byte[] image;
        }

        enum ImageType
        {
            none,
            Uncompressed_ColorMapped,
            Uncompressed_TrueColor,
            Uncompressed_BlackWhite,
            RLE_ColorMapped,
            RLE_TrueColor,
            RLE_BlackWhite,
            noSopported
        }
        struct ColorMap_Spec
        {
            public ushort first_entry_index;
            public ushort length;
            public byte entry_size;
        }
        struct Image_Spec
        {
            public ushort posX;
            public ushort posY;
            public ushort width;
            public ushort height;
            public byte depth;
            public byte descriptor;

        }
    }
}
