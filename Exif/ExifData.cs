using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Utils;

namespace ExifLibrary
{
    [Serializable]
    public class EXIFTag : LOCALIZED_DATA
    {
        #region Private members
        private ELEMENTARY_TYPE tagID;
        private ELEMENTARY_TYPE tagDataType;
        private ELEMENTARY_TYPE tagCount;
        private ELEMENTARY_TYPE offset;
        private ELEMENTARY_TYPE tagData;
        private EXIFType tagType;
        private List<MPFIMAGE> mP_Images;
        MakerNote makerNote;
        public int numericData;
        private bool littleIndian;
        private EXIFRational rational;

        #endregion
        #region Basic Properties
        public ELEMENTARY_TYPE TagID
        {
            get { return tagID; }
            set { tagID = value; }
        }
        public ELEMENTARY_TYPE TagDataType
        {
            get { return tagDataType; }
            set { tagDataType = value; }
        }
        public ELEMENTARY_TYPE TagCount
        {
            get { return tagCount; }
            set { tagCount = value; }
        }
        public ELEMENTARY_TYPE Offset
        {
            get { return offset; }
            set { offset = value; }
        }
        public ELEMENTARY_TYPE TagData
        {
            get { return tagData; }
            set { tagData = value; }
        }
        #endregion
        public MakerNote MakerNote
        {
            get { return makerNote; }
            set { makerNote = value; }
        }
        public List<MPFIMAGE> MP_Images
        {
            get { return mP_Images; }
            set { mP_Images = value; }
        }
        public int TagLength
        {
            get
            {
                return ((int)tagCount.Value);
            }
        }
        public string TagName
        {
            get
            {
                switch (tagType)
                {
                    case EXIFType.Exif:
                        return ((EXIFCode)(ushort)tagID.Value).ToString();
                    case EXIFType.MPF:
                        return ((EXIFMPF)(ushort)tagID.Value).ToString();
                    case EXIFType.Nikon:
                        return ((EXIFNikon)(ushort)tagID.Value).ToString();
                    case EXIFType.GPS:
                        return ((EXIFGPSCode)(ushort)tagID.Value).ToString();
                    case EXIFType.MinoltaDX:
                    case EXIFType.MinoltaDimage:
                        return ((EXIFDimageX)(ushort)tagID.Value).ToString();
                    case EXIFType.FujiFilm:
                        return ((EXIFFujiCode)(ushort)tagID.Value).ToString();
                    case EXIFType.Olympus:
                        return ((EXIFOlympusCode)(ushort)tagID.Value).ToString();
                    case EXIFType.Sony:
                        return ((EXIFSONYCode)(ushort)tagID.Value).ToString("");
                    case EXIFType.Casio_Type2:
                        return ((EXIFCasio_Type2)(ushort)tagID.Value).ToString("");
                }
                return "";
            }
            set { ;}
        }
        public EXIFDataType TagType
        { get { return (EXIFDataType)(short)tagDataType.Value; } }
        public string TagValue
        {
            get
            {
                return tagDetails;
            }
        }
        private string tagDetails;
        public EXIFCode tagCode;
        public string[] TagDetails
        {
            get
            {
                string[] value = new string[2] { "", "" };
                switch (tagType)
                {
                    case EXIFType.GPS:
                        value[0] = ((EXIFGPSCode)(int)tagCode).ToString();
                        value[1] = tagDetails;
                        switch ((EXIFGPSCode)(int)tagCode)
                        {
                            #region GPS stringData
                            case EXIFGPSCode.GPSVersionID:
                            case EXIFGPSCode.GPSLatitudeRef://'N' = North 'S' = South
                            case EXIFGPSCode.GPSLatitude:
                            case EXIFGPSCode.GPSLongitudeRef://'E' = East ,'W' = West
                            case EXIFGPSCode.GPSLongitude:
                            case EXIFGPSCode.GPSAltitudeRef://0 = Above Sea Level ,1 = Below Sea Level
                            case EXIFGPSCode.GPSAltitude:
                            case EXIFGPSCode.GPSTimeStamp://(when writing, date is stripped off if present, and time is adjusted to UTC if it includes a timezone)
                            case EXIFGPSCode.GPSSatellites:
                            case EXIFGPSCode.GPSStatus://'A' = Measurement Active ,'outTime' = Measurement Void
                            case EXIFGPSCode.GPSMeasureMode://2 = 2-Dimensional Measurement ,3 = 3-Dimensional Measurement
                            case EXIFGPSCode.GPSDOP:
                            case EXIFGPSCode.GPSSpeedRef://'K' = km/h ,'M' = mph ,'N' = knots
                            case EXIFGPSCode.GPSSpeed:
                            case EXIFGPSCode.GPSTrackRef://'M' = Magnetic North ,'T' = True North
                            case EXIFGPSCode.GPSTrack:
                            case EXIFGPSCode.GPSImgDirectionRef://M' = Magnetic North ,'T' = True North
                            case EXIFGPSCode.GPSImgDirection:
                            case EXIFGPSCode.GPSMapDatum:
                            case EXIFGPSCode.GPSDestLatitudeRef://'N' = North ,'S' = South
                            case EXIFGPSCode.GPSDestLatitude:
                            case EXIFGPSCode.GPSDestLongitudeRef://'E' = East ,'W' = West
                            case EXIFGPSCode.GPSDestLongitude:
                            case EXIFGPSCode.GPSDestBearingRef://'M' = Magnetic North ,'T' = True North
                            case EXIFGPSCode.GPSDestBearing:
                            case EXIFGPSCode.GPSDestDistanceRef://'K' = Kilometers ,'M' = Miles ,'N' = Nautical Miles
                            case EXIFGPSCode.GPSDestDistance:
                            case EXIFGPSCode.GPSProcessingMethod://(values of "GPS", "CELLID", "WLAN" or "MANUAL" by the EXIF spec.)
                            case EXIFGPSCode.GPSAreaInformation:
                            case EXIFGPSCode.GPSDateStamp://(when writing, date is stripped off if present, and time is adjusted to UTC if it includes a timezone)
                            case EXIFGPSCode.GPSDifferential://0 = No Correction ,1 = Differential Corrected
                            case EXIFGPSCode.GPSHPositioningError:
                            default:
                                break;
                            #endregion
                        }
                        break;
                    case EXIFType.Sony:
                    case EXIFType.FujiFilm:
                    case EXIFType.MinoltaDX:
                    case EXIFType.MinoltaDimage:
                    case EXIFType.Olympus:
                        #region
                        switch (TagType)
                        {
                            case EXIFDataType.ExifTypeSignedRational:
                            case EXIFDataType.ExifTypeUnsignedRational:
                                value[1] = tagDetails;
                               // value[1] = floatValue;
                                break;
                            default:
                                value[1] = tagDetails;
                                break;
                        }
                        break;
                    default:
                        value[0] = ((EXIFGPSCode)(int)tagCode).ToString();
                        value[1] = tagDetails;
                        break;
                        #endregion
                    case EXIFType.Exif:
                        try
                        {
                            switch (tagCode)
                            {
                                #region Expand Tag Data
                                case EXIFCode.Copyright:
                                    value[0] = "Copyright";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.Image_Description: //TradeMark
                                    value[0] = "Description_Image";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.Manufacturer: //TradeMark
                                    value[0] = "Marque";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.Model: //Model
                                    value[0] = "Modèle";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.Software: //Logiciel
                                    value[0] = "Logiciel";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.Orientation:
                                    #region Orientation
                                    value[0] = "Orientation";
                                    switch (numericData)
                                    {
                                        case 1:
                                            value[1] = "top left side";
                                            break;
                                        case 2:
                                            value[1] = "top right side";
                                            break;
                                        case 3:
                                            value[1] = "bottom right side";
                                            break;
                                        case 4:
                                            value[1] = "bottom left side";
                                            break;
                                        case 5:
                                            value[1] = "left side top";
                                            break;
                                        case 6:
                                            value[1] = "right side top";
                                            break;
                                        case 7:
                                            value[1] = "left side bottom";
                                            break;
                                        case 8:
                                            value[1] = "right side bottom";
                                            break;
                                    }
                                    break;
                                    #endregion
                                case EXIFCode.X_Resolution:
                                    value[0] = "Résolution horizontale";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.Y_Resolution:
                                    value[0] = "Résolution verticale";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.Resolution_Unit:
                                    #region Resolution
                                    value[0] = "Unité de résolution";
                                    switch (numericData)
                                    {
                                        case 1:
                                            value[1] = "aucune";
                                            break;
                                        case 2:
                                            value[1] = "pouces";
                                            break;
                                        case 3:
                                            value[1] = "centimètres";
                                            break;
                                    }
                                    #endregion
                                    break;
                                case EXIFCode.YCbCr_Positioning: //'1' means the center of pixel array, '2' means the datum point
                                    #region
                                    value[0] = "YCbCrPositioning";
                                    if (numericData == 1)
                                    {
                                        value[1] = "center of pixel array";
                                    }
                                    else
                                    {
                                        value[1] = "datum point";
                                    }
                                    break;
                                    #endregion
                                case EXIFCode.DateTime:
                                    value[0] = "Date prise de vue";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.Exposure_Time: //ur
                                    value[0] = "Temps d'exposition";
                                    value[1] = tagDetails;
                                    value[1] = tagDetails + "s";
                                    break;
                                case EXIFCode.F_Number: //ur
                                    value[0] = "Ouverture";
                                    value[1] = tagDetails;
                              //      value[1] = floatValue;
                                    break;
                                case EXIFCode.Exposure_Program:
                                    value[0] = "Programme";
                                    value[1] = tagDetails;
                                    switch (numericData)
                                    {
                                        case 0: value[1] = " Not Defined ";
                                            break;
                                        case 1: value[1] = " Manual";
                                            break;
                                        case 2: value[1] = " Program AE";
                                            break;
                                        case 3: value[1] = " Aperture-priority AE ";
                                            break;
                                        case 4: value[1] = " Shutter speed priority AE";
                                            break;
                                        case 5: value[1] = " Creative (Slow speed)";
                                            break;
                                        case 6: value[1] = " Action (High speed)";
                                            break;
                                        case 7: value[1] = " Portrait";
                                            break;
                                        case 8: value[1] = " Landscape";
                                            break;
                                        case 9: value[1] = " Bulb";
                                            break;
                                    }
                                    break;
                                case EXIFCode.ISOSpeedRatings:
                                    value[0] = "Sensibilité ISO";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.ExifVersion:
                                    value[0] = "Version EXIF";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.DateTimeOriginal: //Date de prise de vue
                                    value[0] = "Date Original";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.DateTimeDigitized: //Date de prise de vue
                                    value[0] = "Date Numérisation";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.ComponentsConfiguration:
                                    value[0] = "Configuration";
                                    #region
                                    string tmp = "";
                                    byte[] data = BitConverter.GetBytes((int)offset.Value);
                                    for (int i = 0; i < data.Length; i++)
                                    {
                                        switch (data[i])
                                        {
                                            case 1: tmp += "Y";
                                                break;
                                            case 2: tmp += "Cb";
                                                break;
                                            case 3: tmp += "Cr";
                                                break;
                                            case 4: tmp += "R";
                                                break;
                                            case 5: tmp += "G";
                                                break;
                                            case 6: tmp += "B";
                                                break;
                                        }
                                    }
                                    value[1] = tmp;
                                    #endregion
                                    break;
                                case EXIFCode.CompressedBitsPerPixel:
                                    value[0] = "Compression bits par pixel";
                                    value[1] = tagDetails;
                         //           value[1] = floatValue;
                                    break;
                                case EXIFCode.ExposureBiasValue:
                                    value[0] = "Exposure Bias Value";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.MaxApertureValue:
                                    value[0] = "Max Aperture Value";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.MeteringMode:
                                    #region Mode de Mesure
                                    value[0] = "Mode de mesure";
                                    switch (numericData)
                                    {
                                        case 0:
                                            value[1] = "inconnu";
                                            break;
                                        case 1:
                                            value[1] = "moyen";
                                            break;
                                        case 2:
                                            value[1] = "moyenne centrée";
                                            break;
                                        case 3:
                                            value[1] = "spot";
                                            break;
                                        case 4:
                                            value[1] = "multi-spot";
                                            break;
                                        case 5:
                                            value[1] = "multi-segment";
                                            break;
                                        case 6:
                                            value[1] = "partiel";
                                            break;
                                        case 255:
                                            value[1] = "inconnu";
                                            break;
                                    }
                                    #endregion
                                    break;
                                case EXIFCode.LightSource:
                                    #region Balance_des_blancs
                                    value[0] = "Balance des blancs";
                                    switch (numericData)
                                    {
                                        case 0:
                                            value[1] = "auto";
                                            break;
                                        case 1:
                                            value[1] = "lumière du jour";
                                            break;
                                        case 2:
                                            value[1] = "fluorescent";
                                            break;
                                        case 3:
                                            value[1] = "tugstène";
                                            break;
                                        case 10:
                                            value[1] = "flash";
                                            break;
                                        case 17:
                                            value[1] = "standard A";
                                            break;
                                        case 18:
                                            value[1] = "standard B";
                                            break;
                                        case 19:
                                            value[1] = "standard C";
                                            break;
                                        case 29:
                                            value[1] = "D55";
                                            break;
                                        case 21:
                                            value[1] = "D65";
                                            break;
                                        case 22:
                                            value[1] = "D75";
                                            break;
                                        case 255:
                                            value[1] = "inconnu";
                                            break;
                                    }
                                    break;
                                    #endregion
                                case EXIFCode.Flash:
                                    #region
                                    value[0] = "Flash";
                                    if (numericData == 0)
                                        value[1] = "Sans flash";
                                    else
                                        value[1] = "Avec flash";
                                    break;
                                    #endregion
                                case EXIFCode.FocalLength:
                                    value[0] = "Longueur focale";
                                    value[1] = tagDetails + " mm";
                           //         value[1] = floatValue.ToString() + " mm";
                                    break;
                                case EXIFCode.MakerNote:
                                    value[0] = "Note Fabricant";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.UserComment:
                                    value[0] = "Commentaire Utilisateur";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.FlashPixVersion:
                                    value[0] = "Version Flash Pix";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.ColorSpace:
                                    value[0] = "Espace de couleur";
                                    if (numericData == 01)
                                        value[1] = "sRGB";
                                    break;
                                case EXIFCode.ExifImageWidth:
                                    value[0] = "Largeur image";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.ExifImageHeight:
                                    value[0] = "Hauteur image";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.FileSource:
                                    value[0] = "Source image";
                                    data = BitConverter.GetBytes((int)offset.Value);
                                    if (data[3].ToString() == "3")
                                        value[1] = "Appareil photo";
                                    break;
                                case EXIFCode.SceneType:
                                    value[0] = "Type de scène";
                                    data = BitConverter.GetBytes((int)offset.Value);
                                    if (data[3].ToString() == "1")
                                        value[1] = "Photographie directe";
                                    break;
                                case EXIFCode.Jpeg_IF_Offset:
                                    value[0] = "JpegIFOffset";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.Jpeg_IF_Byte_Count:
                                    value[0] = "JpegIFByteCount";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.ExposureMode:
                                    value[0] = "Mode d'exposition";
                                    switch (numericData)
                                    {
                                        case 0: value[1] = "Auto";
                                            break;
                                        case 1: value[1] = "Manual";
                                            break;
                                        case 2: value[1] = "Auto bracket";
                                            break;
                                    }
                                    break;
                                case EXIFCode.White_Balance:
                                    value[0] = "Balance des blancs";
                                    switch (numericData)
                                    {
                                        case 0: value[1] = "Auto";
                                            break;
                                        case 1: value[1] = "Manual";
                                            break;
                                    }
                                    break;
                                case EXIFCode.Digital_Zoom_Ratio:
                                    value[0] = "Zoom digital";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.FocalLengthIn35mmFormat:
                                    value[0] = "Focale 35 mm";
                                    break;
                                case EXIFCode.SceneCaptureType:
                                    value[0] = "Type de scene";
                                    switch (numericData)
                                    {
                                        case 0: value[1] = "Standard";
                                            break;
                                        case 1: value[1] = "Landscape";
                                            break;
                                        case 2: value[1] = "Portrait";
                                            break;
                                    }
                                    break;
                                case EXIFCode.Exif_IFD_Pointer:
                                    value[0] = "ExifIFDPointer";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.ExifInteroperabilityOffset:
                                    value[0] = "ExifInteroperabilityOffset";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.ApertureValue:
                                    value[0] = "Ouverture";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.RelatedSoundFile:
                                    value[0] = "Fichier son";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.FocalPlaneXResolution:
                                    value[0] = "Résolution dans le plan focal X";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.FocalPlaneYResolution:
                                    value[0] = "Résolution dans le plan focal Y";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.FocalPlaneResolutionUnit:
                                    value[0] = "Unite de Résolution dans le plan focal";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.ExposureIndex:
                                    value[0] = "Indice exposition";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.SensingMethod:
                                    value[0] = "Sensing Method";
                                    switch (numericData)
                                    {
                                        case 1: value[1] = "Monochrome area";
                                            break;
                                        case 2: value[1] = "One-chip color area";
                                            break;
                                        case 3: value[1] = "Two-chip color area";
                                            break;
                                        case 4: value[1] = "Three-chip color area";
                                            break;
                                        case 5: value[1] = "Color sequential area";
                                            break;
                                        case 6: value[1] = "Monochrome linear";
                                            break;
                                        case 7: value[1] = "Trilinear";
                                            break;
                                        case 8: value[1] = "Color sequential linear";
                                            break;
                                    }
                                    break;
                                case EXIFCode.Padding:
                                    value[0] = "Padding";
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.XPTitle:
                                    value[0] = TagName;
                                    break;
                                case EXIFCode.XPComment:
                                    value[0] = TagName;
                                    break;
                                case EXIFCode.XPAuthor:
                                    value[0] = TagName;
                                    break;
                                case EXIFCode.XPKeywords:
                                    value[0] = TagName;
                                    break;
                                case EXIFCode.XPSubject:
                                    value[0] = TagName;
                                    break;
                                case EXIFCode.GainControl:
                                    value[0] = "Contrôle de gain";
                                    switch (numericData)
                                    {
                                        case 0: value[1] = "None";
                                            break;
                                        case 1: value[1] = "Low gain up";
                                            break;
                                        case 2: value[1] = "High gain up";
                                            break;
                                        case 3: value[1] = "Low gain down";
                                            break;
                                        case 4: value[1] = "High gain down";
                                            break;
                                    }
                                    break;
                                case EXIFCode.Contrast:
                                    value[0] = TagName;
                                    switch (numericData)
                                    {
                                        case 0: value[1] = "Normal";
                                            break;
                                        case 1: value[1] = "Faible";
                                            break;
                                        case 2: value[1] = "Elevé";
                                            break;
                                    }
                                    break;
                                case EXIFCode.BrightnessValue:
                                    value[0] = TagName;
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.Saturation:
                                    value[0] = TagName;
                                    switch (numericData)
                                    {
                                        case 0: value[1] = "Normal";
                                            break;
                                        case 1: value[1] = "Faible";
                                            break;
                                        case 2: value[1] = "Elevée";
                                            break;
                                    }
                                    break;
                                case EXIFCode.Sharpness:
                                    value[0] = "Netteté";
                                    switch (numericData)
                                    {
                                        case 0: value[1] = "Normal";
                                            break;
                                        case 1: value[1] = "Doux";
                                            break;
                                        case 2: value[1] = "Dur";
                                            break;
                                    }
                                    break;
                                case EXIFCode.SubjectDistanceRange:
                                    value[0] = "Distance sujet";
                                    switch (numericData)
                                    {
                                        case 0: value[1] = "Inconnu";
                                            break;
                                        case 1: value[1] = "Macro";
                                            break;
                                        case 2: value[1] = "Proche";
                                            break;
                                        case 3: value[1] = "Distant";
                                            break;
                                    }
                                    break;
                                case EXIFCode.DeviceSettingDescription:
                                    value[0] = TagName;
                                    value[1] = tagDetails;
                                    break;
                                case EXIFCode.ImageUniqueID:
                                    value[0] = TagName;
                                    break;
                                case EXIFCode.OwnerName:
                                    value[0] = TagName;
                                    break;
                                case EXIFCode.SerialNumber:
                                    value[0] = TagName;
                                    break;
                                case EXIFCode.LensInfo:
                                    value[0] = TagName;
                                    break;
                                case EXIFCode.LensMake:
                                    value[0] = TagName;
                                    break;
                                case EXIFCode.LensModel:
                                    value[0] = TagName;
                                    break;
                                case EXIFCode.LensSerialNumber:
                                    value[0] = TagName;
                                    break;
                                case EXIFCode.PrintIM:
                                    value[0] = TagName;
                                    value[1] = tagDetails;
                                    byte[] buffer = new byte[14];
                                    Buffer.BlockCopy(((byte[])tagData.Value), 0, buffer, 0, buffer.Length);
                                    string s = Encoding.Default.GetString(buffer);
                                    buffer = new byte[2];
                                    Buffer.BlockCopy(((byte[])tagData.Value), 14, buffer, 0, buffer.Length);
                             //       int a = BufferConvert.ByteToInteger(buffer);
                                    buffer = new byte[4];
                                  Buffer.BlockCopy(((byte[])tagData.Value), 16, buffer, 0, buffer.Length);
                              //        int b = BufferConvert.ByteToInteger(buffer);
                                    break;
                                case EXIFCode.CustomRendered:
                                    value[0] = TagName;
                                    switch (numericData)
                                    {
                                        case 0: value[1] = "Normal";
                                            break;
                                        case 1: value[1] = "Personnalisé";
                                            break;
                                    }
                                    break;
                                case EXIFCode.Compression:
                                    value[0] = TagName;
                                    switch (numericData)
                                    {
                                        case 1: value[1] = "TIFF";
                                            break;
                                        case 6: value[1] = "JPEG";
                                            break;
                                        default: value[1] = "Autre";
                                            break;
                                    }

                                    break;
                                default:
                                    value[0] = TagName;
                                    value[1] = tagDetails;
                                    break;
                                #endregion
                            }
                        }
                        catch { }
                        break;
                    case EXIFType.Panasonic:
                        break;
                }
                return value;
            }
        }
        public EXIFTag(BitStreamReader sw, int startIndex, string manufacturer, EXIFType exifType)
        {
            littleIndian = sw.LittleEndian;
            tagType = exifType;
            #region Read tag stringData
            PositionOfStructureInFile = sw.Position;
            tagID = new ELEMENTARY_TYPE(sw, 0, typeof(ushort));        //Bytes 0-1 Tag
            tagDataType = new ELEMENTARY_TYPE(sw, 0, typeof(short));  //Bytes 2-3 Data Type
            tagCount = new ELEMENTARY_TYPE(sw, 0, typeof(int));       //Bytes 4-7 Count
            offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));         // Bytes 8-11 Value Offset from start of Tiff header
            LengthInFile = sw.Position - PositionOfStructureInFile;
            #endregion
            int offsetToData = (int)offset.Value + startIndex + 1;
            tagCode = (EXIFCode)(ushort)tagID.Value;
            #region OEM specific
            switch (exifType)
            {
                case EXIFType.Olympus:
                    break;
                case EXIFType.MinoltaDX:
                    break;
                case EXIFType.FujiFilm:
                    break;
                case EXIFType.Sony:
                    offsetToData -= 0x2C9; //Rechercher pourquoi
                    break;
                case EXIFType.MPF:
                    if ((EXIFMPF)((ushort)tagID.Value) == EXIFMPF.MPImageList)
                    {
                        sw.Position = offsetToData;
                        mP_Images = new List<MPFIMAGE>();
                        while (sw.Position < offsetToData + (int)tagCount.Value)
                            mP_Images.Add(new MPFIMAGE(sw, startIndex));
                        return;
                    }
                    break;
            }
            #endregion
            try
            {
                #region Reads actual stringData
                switch (TagType)
                {
                    //One byte value
                    case EXIFDataType.ExifTypeSignedByte:
                    case EXIFDataType.ExifTypeUnsignedByte:
                        #region byte
                        tagData = offset;
                        sw.Position -= 4;
                        if (sw.LittleEndian)
                        {
                            numericData = sw.ReadByte();
                            sw.Position += 3;
                        }
                        else
                        {
                            sw.Position += 3;
                            numericData = sw.ReadByte();
                        }
                        tagDetails = numericData.ToString("x2");
                        break;
                        #endregion
                    //Two byte value
                    case EXIFDataType.ExifTypeSignedChar:
                    case EXIFDataType.ExifTypeUnsignedChar:
                        #region Short
                        tagData = offset;
                         sw.Position -= 4;
                         if (sw.LittleEndian)
                         {
                             numericData = sw.ReadShort();
                             sw.Position += 2;
                         }
                         else
                         {
                             sw.Position += 2;
                             numericData = sw.ReadShort();
                         }
                        tagDetails = numericData.ToString("x4");
                        break;
                        #endregion
                    //Four byte value
                    case EXIFDataType.ExifTypeFloat:
                    case EXIFDataType.ExifTypeUnsignedInt:
                    case EXIFDataType.ExifTypeSignedInt:
                        tagData = offset;
                        numericData = (int)offset.Value;
                        tagDetails = numericData.ToString("x4");
                        break;
                    //Eight byte float
                    case EXIFDataType.ExifTypeDouble:
                        int pso = sw.Position;
                        sw.Position = offsetToData;
                        tagData = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 8);
                        tagDetails =((int) tagData.Value).ToString("x4");
                        sw.Position = pso;
                        break;
                    //Eight byte rational
                    case EXIFDataType.ExifTypeSignedRational:
                    case EXIFDataType.ExifTypeUnsignedRational:
                        #region Rational
                        pso = sw.Position;
                        sw.Position = offsetToData;
                        tagData = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), 8);
                        sw.Position -= 8;
                        rational = new EXIFRational(sw);

                        tagDetails = rational.ToString();
                        sw.Position = pso;
                        break;
                        #endregion
                    //Any size value
                    case EXIFDataType.ExifTypeUndefined:
                    case EXIFDataType.ExifTypeString:
                        #region string
                        if ((int)tagCount.Value < 5)
                        {
                            sw.Position -= 4;
                            tagDetails = sw.ReadString(4, Encoding.Default);
                        }
                        else
                        {
                            int ps = sw.Position;
                            sw.Position = offsetToData;
                            tagData = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (int)tagCount.Value);
                            sw.Position -= (int)tagCount.Value;
                            tagDetails = sw.ReadString((int)tagCount.Value, Encoding.Default);
                            #region MakerNote
                            if (tagCode == EXIFCode.MakerNote)
                            {
                                sw.Position = offsetToData;
                                makerNote = new MakerNote(sw, manufacturer, startIndex);
                            }
                            #endregion
                            sw.Position = ps;
                        }
                        break;
                        #endregion
                    default:
                        break;
                }
                #endregion
            }
            catch { }
        }
        public override string ToString()
        {
            return TagName + " : " + TagDetails[1];
        }
    }
    [Serializable]
    public class EXIFRational
    {
        public int Denominator;
        public int Numerator;
        /// 
        /// Creates an EXIFRational object from EXIF byte stringData,
        /// starting at the byte specified by ofs
        /// 
        /// EXIF byte stringData
        /// Initial Byte
        public EXIFRational(byte[] data, int ofs)
        {
            Denominator = 0;
            Numerator = 0;
            if (data.Length >= ofs + 8)
            {
                Numerator = data[ofs];
                Numerator |= (data[ofs + 1] >> 8);
                Numerator |= (data[ofs + 2] >> 16);
                Numerator |= (data[ofs + 3] >> 24);
                Denominator = data[ofs + 4];
                Denominator |= (data[ofs + 5] >> 8);
                Denominator |= (data[ofs + 6] >> 16);
                Denominator |= (data[ofs + 7] >> 24);
            }
        }
        public EXIFRational(BitStreamReader sw)
        {
            Numerator = sw.ReadInteger();
            Denominator = sw.ReadInteger();
            if ((Numerator >= 10) & (Denominator >= 10))
            {
                Numerator /= 10;
                Denominator /= 10;
            }
        }
        /// 
        /// Returns the value of the fraction as a string
        /// 
        /// The value of the fraction as a string
        public override string ToString()
        {
            string ret;
            if (this.Denominator == 0)
            {
                ret = "N/A";
            }
            else
            {
                ret = String.Format("{0:F2}", this.Numerator * 1.0 / this.Denominator);
                ret = Numerator.ToString() + "/" + Denominator.ToString();
            }
            return ret;
        }
    }
}
