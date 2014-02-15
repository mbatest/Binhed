using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Utils;

namespace BinHed
{
    public class BinDecoder : LOCALIZED_DATA
    {
        private string FileName;
        public BinDecoder(string FileName)
        {
            this.FileName = FileName;
            switch (Path.GetExtension(FileName).ToUpper())
            {
                case ".BMP":
                    DecodeBMP();
                    break;
                case "PNG":
                    DecodePNG();
                    break;
                case ".CHM":
                    DecodeCHM();
                    break;
            }
        }
        private void DecodeBMP()
        {
            FileStream FS = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[55];
            FS.Read(buffer, 0, 55);
            #region  BITMAPFILEHEADER:
            byte[] c = new byte[4];
            Buffer.BlockCopy(buffer, 2, c, 0, 4);
            int imagesize = ConvertToInt(c);
            Buffer.BlockCopy(buffer, 10, c, 0, 4);
            int offset = ConvertToInt(c);
            #endregion
            #region BITMAPINFOHEADER
            Buffer.BlockCopy(buffer, 18, c, 0, 4);
            int imageWidth = ConvertToInt(c);
            Buffer.BlockCopy(buffer, 22, c, 0, 4);
            int imageHeight = ConvertToInt(c);
            c = new byte[2];
            Buffer.BlockCopy(buffer, 28, c, 0, 2);
            int bitPerPixel = ConvertToInt(c);
            #endregion
            FS.Close();
        }
        private void DecodePNG()
        {
            FileStream FS = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            #region fileHeader
            byte[] buffer = new byte[8];
            FS.Read(buffer, 0, 8);
            #endregion
            FS.Close();
        }
        private void DecodeCHM()
        {
            FileStream FS = new FileStream(FileName, FileMode.Open, FileAccess.Read);
            #region fileHeader
            byte[] buffer = new byte[0x38];
            FS.Read(buffer, 0, 0x32);
            byte[] c = new byte[4];
            Buffer.BlockCopy(buffer, 4, c, 0, 4);
            int versionNumber = ConvertToInt(c);
            Buffer.BlockCopy(buffer, 8, c, 0, 4);
            int headerLength = ConvertToInt(c);
            Buffer.BlockCopy(buffer, 0x10, c, 0, 4);
            int timeStamp = ConvertToInt(c);
            Buffer.BlockCopy(buffer, 0x14, c, 0, 4);
            int languageID = ConvertToInt(c);
            byte[] GUID1 = new byte[0x10];
            Buffer.BlockCopy(buffer, 0x18, GUID1, 0, 0x10);
            Guid g1 = new Guid(GUID1);
            Buffer.BlockCopy(buffer, 0x28, GUID1, 0, 0x10);
            Guid g2 = new Guid(GUID1);
            #endregion
            FS.Close();
        }
        private int ConvertToInt(byte[] c)
        {
            int t = 0;
            for (int w = 0; w < c.Length; w++)
            {
                t = 256 * t + (int)c[c.Length - 1 - w];
            }
            return t;
        }
    }
}