using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
namespace Exif
{
    public class BMPFile : LOCALIZED_DATA
    {
        BITMAPFILEHEADER head;
        BITMAPINFOHEADER infoHeader;
        List<Bmp_Line> lines = new List<Bmp_Line>();

        public BITMAPFILEHEADER Head
        {
            get { return head; }
            set { head = value; }
        }

        public BITMAPINFOHEADER InfoHeader
        {
            get { return infoHeader; }
            set { infoHeader = value; }
        }
        public List<Bmp_Line> Lines
        {
            get { return lines; }
            set { lines = value; }
        }
        public BMPFile(string FileName)
        {
            BitStreamReader sw = new BitStreamReader(FileName, false);
            #region  :
            #endregion
            #region BITMAPINFOHEADER
            head = new BITMAPFILEHEADER(sw);
            infoHeader = new BITMAPINFOHEADER(sw);
            sw.Position = (int)head.Offset.Value;
            for (int i = 0; i < (int)infoHeader.Height.Value; i++)
            {
                Bmp_Line line = new Bmp_Line(sw, (int)infoHeader.Width.Value, (ushort)infoHeader.Bits_per_Pixel.Value / 8);
                lines.Add(line);
            }
            #endregion
            sw.Close();
        }
    }
    public class Bmp_Pixel : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE pixel;
        public ELEMENTARY_TYPE Pixel
        {
            get { return pixel; }
            set { pixel = value; }
        }
        public Bmp_Pixel(BitStreamReader sw, int size)
        {
            PositionOfStructureInFile=sw.Position;
            pixel = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), size);
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class Bmp_Line : LOCALIZED_DATA
    {
        List<Bmp_Pixel> line;

        public List<Bmp_Pixel> Line
        {
            get { return line; }
            set { line = value; }
        }
         public Bmp_Line(BitStreamReader sw, int number_Pixel, int pixSize)
        {
            PositionOfStructureInFile = sw.Position;
             line = new List<Bmp_Pixel>();
            for (int i = 0; i < number_Pixel;i++ )
            {
                Bmp_Pixel pix = new Bmp_Pixel(sw, pixSize);
                line.Add(pix);
            }
                LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class BITMAPFILEHEADER : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE header;
        ELEMENTARY_TYPE fileSize;
        ELEMENTARY_TYPE reserved;
        ELEMENTARY_TYPE offset;
        public ELEMENTARY_TYPE Header
        {
            get { return header; }
            set { header = value; }
        }
        public ELEMENTARY_TYPE FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }
        public ELEMENTARY_TYPE Reserved
        {
            get { return reserved; }
            set { reserved = value; }
        }
        public ELEMENTARY_TYPE Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public BITMAPFILEHEADER(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            header = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 2);
            fileSize = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            reserved = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
    public class BITMAPINFOHEADER : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE headerSize;
        ELEMENTARY_TYPE width;
        ELEMENTARY_TYPE height;
        ELEMENTARY_TYPE number_color_planes;
        ELEMENTARY_TYPE bits_per_Pixel;
        ELEMENTARY_TYPE compression_Method;
        ELEMENTARY_TYPE image_Size;
        ELEMENTARY_TYPE horizontal_resolution;
        ELEMENTARY_TYPE vertical_resolution;
        ELEMENTARY_TYPE colors_in_palette;
        ELEMENTARY_TYPE number_important_colors;
        public ELEMENTARY_TYPE HeaderSize
        {
            get { return headerSize; }
            set { headerSize = value; }
        }
        public ELEMENTARY_TYPE Width
        {
            get { return width; }
            set { width = value; }
        }
        public ELEMENTARY_TYPE Height
        {
            get { return height; }
            set { height = value; }
        }
        public ELEMENTARY_TYPE Number_color_planes
        {
            get { return number_color_planes; }
            set { number_color_planes = value; }
        }

        public ELEMENTARY_TYPE Bits_per_Pixel
        {
            get { return bits_per_Pixel; }
            set { bits_per_Pixel = value; }
        }
        public ELEMENTARY_TYPE Compression_Method
        {
            get { return compression_Method; }
            set { compression_Method = value; }
        }
        public ELEMENTARY_TYPE Image_Size
        {
            get { return image_Size; }
            set { image_Size = value; }
        }
        public ELEMENTARY_TYPE Horizontal_resolution
        {
            get { return horizontal_resolution; }
            set { horizontal_resolution = value; }
        }
        public ELEMENTARY_TYPE Vertical_resolution
        {
            get { return vertical_resolution; }
            set { vertical_resolution = value; }
        }
        public ELEMENTARY_TYPE Colors_in_palette
        {
            get { return colors_in_palette; }
            set { colors_in_palette = value; }
        }
        public ELEMENTARY_TYPE Number_important_colors
        {
            get { return number_important_colors; }
            set { number_important_colors = value; }
        }
        public BITMAPINFOHEADER(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            headerSize = new ELEMENTARY_TYPE(sw, 0, typeof(uint));
            width = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            height = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            number_color_planes = new ELEMENTARY_TYPE(sw, 0, typeof(ushort));
            bits_per_Pixel = new ELEMENTARY_TYPE(sw, 0, typeof(ushort));
            compression_Method = new ELEMENTARY_TYPE(sw, 0, typeof(uint));
            image_Size = new ELEMENTARY_TYPE(sw, 0, typeof(uint));
            horizontal_resolution = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            vertical_resolution = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            colors_in_palette = new ELEMENTARY_TYPE(sw, 0, typeof(uint));
            number_important_colors = new ELEMENTARY_TYPE(sw, 0, typeof(uint));
            LengthInFile = sw.Position - PositionOfStructureInFile;
        }
    }
}
