using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using PluginInterface;
using System.IO;

namespace Comun
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
            PictureBox pic = new PictureBox();
            pic.SizeMode = PictureBoxSizeMode.AutoSize;
            pic.Image = Leer();
            pic.BorderStyle = BorderStyle.FixedSingle;
            return pic;
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
            	br.ReadBytes(tga.header.colorMap_spec.length);
            	// TODO: Añadir campo, generalmente no aparece
            }
            imagen = new Bitmap(tga.header.image_spec.width, tga.header.image_spec.height);
            #region Obtener colores de pixels
            Color[] colores = new Color[tga.header.image_spec.height * tga.header.image_spec.width];
            
            switch (tga.header.image_type) {

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
            			colores[j] = Color.FromArgb(255, tga.imageData.image[3 * j],
            			                            tga.imageData.image[3 * j + 1], tga.imageData.image[3 * j + 2]);
					break;
					
            	case TGA.ImageType.Uncompressed_TrueColor:
            		colores = pluginHost.BGR555(br.ReadBytes(tga.header.image_spec.height * tga.header.image_spec.width * 2));
            		break;
            		
            	case TGA.ImageType.Uncompressed_BlackWhite:            		
            	case TGA.ImageType.RLE_BlackWhite:
            	case TGA.ImageType.RLE_ColorMapped:  
            	case TGA.ImageType.Uncompressed_ColorMapped:            		
            	case TGA.ImageType.noSopported:
            	default:
            		throw new Exception("Invalid value for ImageType");
            }
            #endregion
            int i = 0;
            for (int y = tga.header.image_spec.height - 1; y > 0; y--)
            {
            	for (int x = 0; x < tga.header.image_spec.width; x++)
            	{
            		imagen.SetPixel(x, y, colores[i]);
            		i++;
            	}
            }
            
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
            public byte[] colorMap;
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
