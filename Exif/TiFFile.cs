using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
namespace Exif
{
    public class TiFFile : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE endian;
        ELEMENTARY_TYPE fortytwo;
        ELEMENTARY_TYPE offset_IFD1;
        List<IFD> iFDirectories = new List<IFD>();
        public ELEMENTARY_TYPE Endian
        {
            get { return endian; }
            set { endian = value; }
        }
        public ELEMENTARY_TYPE Fortytwo
        {
            get { return fortytwo; }
            set { fortytwo = value; }
        }
        public ELEMENTARY_TYPE Offset_IFD1
        {
            get { return offset_IFD1; }
            set { offset_IFD1 = value; }
        }
        public List<IFD> IFDirectories
        {
            get { return iFDirectories; }
            set { iFDirectories = value; }
        }

        public TiFFile(string FileName)
        {
            BitStreamReader sw = new BitStreamReader(FileName, false);
            string en = sw.ReadString(2, Encoding.Default);
            if (en == "MM")
            {
                sw.Close();
                sw = new BitStreamReader(FileName, true);
            }
            sw.Position = 0;
            endian = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, 2);
            fortytwo = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            offset_IFD1 = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            int start = (int)offset_IFD1.Value;
            while (start != 0)
            {
                sw.Position = start;
                IFD ifd = new IFD(sw);
                iFDirectories.Add(ifd);
                start = (int)ifd.Offset_Next.Value;
            }
            LengthInFile = sw.Length;
        }
    }
    public class IFD : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE number_entries;
        List<IFDEntry> entries = new List<IFDEntry>();
        ELEMENTARY_TYPE offset_Next;
        public ELEMENTARY_TYPE Number_entries
        {
            get { return number_entries; }
            set { number_entries = value; }
        }
        public List<IFDEntry> Entries
        {
            get { return entries; }
            set { entries = value; }
        }
        public ELEMENTARY_TYPE Offset_Next
        {
            get { return offset_Next; }
            set { offset_Next = value; }
        }
        public IFD(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            number_entries = new ELEMENTARY_TYPE(sw, 0, typeof(short));
            for (int i = 0; i < (short)number_entries.Value; i++)
            {
                IFDEntry ifd = new IFDEntry(sw);
                entries.Add(ifd);
            }
            offset_Next = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;

        }
    }
    public class IFDEntry : LOCALIZED_DATA
    {
        ELEMENTARY_TYPE tag;
        ELEMENTARY_TYPE field_type;
        ELEMENTARY_TYPE number_values;
        ELEMENTARY_TYPE value_offset;
        private ELEMENTARY_TYPE data;
        private TIFTags tagValue;

        public string TagValue
        {
            get { return ((TIFTags)(ushort)tag.Value).ToString(); }
        }
        public string Field_Type
        {
            get { return ((FieldType)(ushort)field_type.Value).ToString(); }
        }
        public ELEMENTARY_TYPE Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        public ELEMENTARY_TYPE Field_type
        {
            get { return field_type; }
            set { field_type = value; }
        }
        public ELEMENTARY_TYPE Number_values
        {
            get { return number_values; }
            set { number_values = value; }
        }
        public ELEMENTARY_TYPE Value_offset
        {
            get { return value_offset; }
            set { value_offset = value; }
        }
        public ELEMENTARY_TYPE Data
        {
            get { return data; }
            set { data = value; }
        }
        List<IFD> subIfd;

        public List<IFD> SubIfd
        {
            get { return subIfd; }
            set { subIfd = value; }
        }
        IFD exif;
        public IFD Exif
        {
            get { return exif; }
            set { exif = value; }
        }
        public IFDEntry(BitStreamReader sw)
        {
            PositionOfStructureInFile = sw.Position;
            tag = new ELEMENTARY_TYPE(sw, 0, typeof(ushort));
            field_type = new ELEMENTARY_TYPE(sw, 0, typeof(ushort));
            number_values = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            value_offset = new ELEMENTARY_TYPE(sw, 0, typeof(int));
            LengthInFile = sw.Position - PositionOfStructureInFile;
            long position = sw.Position;
            sw.Position = (int)value_offset.Value;
            switch ((ushort)field_type.Value)
            {
                case 1://byte
                    if ((ushort)tag.Value == 0x2Bc)
                    {
                        data = new ELEMENTARY_TYPE(sw, 0, Encoding.Default, (int)number_values.Value);
                    }
                    else
                    {
                        sw.Position = value_offset.PositionOfStructureInFile;
                        data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (int)number_values.Value);
                    }
                    break;
                case 2://string
                    data = new ELEMENTARY_TYPE(sw, 0, Encoding.Default);
                    break;
                case 3://short
                    sw.Position = value_offset.PositionOfStructureInFile;
                    data = new ELEMENTARY_TYPE(sw, 0, typeof(short));
                    break;
                case 4://int
                    switch ((TIFTags)(ushort)tag.Value)
                    {
                        case TIFTags.Exif_IFD:
                            exif = new IFD(sw);
                            break;
                        case TIFTags.SubIFDs:
                            subIfd= new List<IFD>();
                            for (int i = 0; i < (int)number_values.Value; i++)
                                subIfd.Add(new IFD(sw));
                            break;
                        default:
                             data = new ELEMENTARY_TYPE(sw, 0, typeof(int));
                             break;
                    }
                   break;
                case 5://rational
                case 7://undefined
                    data = new ELEMENTARY_TYPE(sw, 0, typeof(byte[]), (int)number_values.Value);
                    break;
            }
            sw.Position = position;
        }
    }
    public enum FieldType { Byte = 1, String = 2, Short = 3, Int = 4, Rational = 5, undefined = 7 }
    public enum TIFTags
    {
        NewSubfileType = 0x00FE,
        SubfileType = 0x00FF,
        ImageWidth = 0x0100,
        ImageLength = 0x0101,
        BitsPerSample = 0x0102,
        Compression = 0x0103,
        PhotometricInterpretation = 0x0106,
        Threshholding = 0x0107,
        CellWidth = 0x0108,
        CellLength = 0x0109,
        FillOrder = 0x010A,
        DocumentName = 0x010D,
        ImageDescription = 0x010E,
        Make = 0x010F,
        Model = 0x0110,
        StripOffsets = 0x0111,
        Orientation = 0x0112,
        SamplesPerPixel = 0x0115,
        RowsPerStrip = 0x0116,
        StripByteCounts = 0x0117,
        MinSampleValue = 0x0118,
        MaxSampleValue = 0x0119,
        XResolution = 0x011A,
        YResolution = 0x011B,
        PlanarConfiguration = 0x011C,
        PageName = 0x011D,
        XPosition = 0x011E,
        YPosition = 0x011F,
        FreeOffsets = 0x0120,
        FreeByteCounts = 0x0121,
        GrayResponseUnit = 0x0122,
        GrayResponseCurve = 0x0123,
        T4Options = 0x0124,
        T6Options = 0x0125,
        ResolutionUnit = 0x0128,
        PageNumber = 0x0129,
        TransferFunction = 0x012D,
        Software = 0x0131,
        DateTime = 0x0132,
        Artist = 0x013B,
        HostComputer = 0x013C,
        Predictor = 0x013D,
        WhitePoint = 0x013E,
        PrimaryChromaticities = 0x013F,
        ColorMap = 0x0140,
        HalftoneHints = 0x0141,
        TileWidth = 0x0142,
        TileLength = 0x0143,
        TileOffsets = 0x0144,
        TileByteCounts = 0x0145,
        BadFaxLines = 0x0146,
        CleanFaxData = 0x0147,
        ConsecutiveBadFaxLines = 0x0148,
        SubIFDs = 0x014A,
        InkSet = 0x014C,
        InkNames = 0x014D,
        NumberOfInks = 0x014E,
        DotRange = 0x0150,
        TargetPrinter = 0x0151,
        ExtraSamples = 0x0152,
        SampleFormat = 0x0153,
        SMinSampleValue = 0x0154,
        SMaxSampleValue = 0x0155,
        TransferRange = 0x0156,
        ClipPath = 0x0157,
        XClipPathUnits = 0x0158,
        YClipPathUnits = 0x0159,
        Indexed = 0x015A,
        JPEGTables = 0x015B,
        OPIProxy = 0x015F,
        GlobalParametersIFD = 0x0190,
        ProfileType = 0x0191,
        FaxProfile = 0x0192,
        CodingMethods = 0x0193,
        VersionYear = 0x0194,
        ModeNumber = 0x0195,
        Decode = 0x01B1,
        DefaultImageColor = 0x01B2,
        JPEGProc = 0x0200,
        JPEGInterchangeFormat = 0x0201,
        JPEGInterchangeFormatLength = 0x0202,
        JPEGRestartInterval = 0x0203,
        JPEGLosslessPredictors = 0x0205,
        JPEGPointTransforms = 0x0206,
        JPEGQTables = 0x0207,
        JPEGDCTables = 0x0208,
        JPEGACTables = 0x0209,
        YCbCrCoefficients = 0x0211,
        YCbCrSubSampling = 0x0212,
        YCbCrPositioning = 0x0213,
        ReferenceBlackWhite = 0x0214,
        StripRowCounts = 0x022F,
        XMP = 0x02BC,
        ImageID = 0x800D,
        Wang_Annotation = 0x80A4,
        CFARepeatPatternDim = 0x828D,
        CFAPattern = 0x828E,
        BatteryLevel = 0x828F,
        Copyright = 0x8298,
        ExposureTime = 0x829A,
        FNumber = 0x829D,
        MD_FileTag = 0x82A5,
        MD_ScalePixel = 0x82A6,
        MD_ColorTable = 0x82A7,
        MD_LabName = 0x82A8,
        MD_SampleInfo = 0x82A9,
        MD_PrepDate = 0x82AA,
        MD_PrepTime = 0x82AB,
        MD_FileUnits = 0x82AC,
        ModelPixelScaleTag = 0x830E,
        IPTC_NAA = 0x83BB,
        INGR_Packet_Data_Tag = 0x847E,
        INGR_Flag_Registers = 0x847F,
        IrasB_Transformation_Matrix = 0x8480,
        ModelTiepointTag = 0x8482,
        ModelTransformationTag = 0x85D8,
        Photoshop = 0x8649,
        Exif_IFD = 0x8769,
        InterColorProfile = 0x8773,
        ImageLayer = 0x87AC,
        GeoKeyDirectoryTag = 0x87AF,
        GeoDoubleParamsTag = 0x87B0,
        GeoAsciiParamsTag = 0x87B1,
        ExposureProgram = 0x8822,
        SpectralSensitivity = 0x8824,
        GPSInfo = 0x8825,
        ISOSpeedRatings = 0x8827,
        OECF = 0x8828,
        Interlace = 0x8829,
        TimeZoneOffset = 0x882A,
        SelfTimeMode = 0x882B,
        HylaFAX_FaxRecvParams = 0x885C,
        HylaFAX_FaxSubAddress = 0x885D,
        HylaFAX_FaxRecvTime = 0x885E,
        ExifVersion = 0x9000,
        DateTimeOriginal = 0x9003,
        DateTimeDigitized = 0x9004,
        ComponentsConfiguration = 0x9101,
        CompressedBitsPerPixel = 0x9102,
        ShutterSpeedValue = 0x9201,
        ApertureValue = 0x9202,
        BrightnessValue = 0x9203,
        ExposureBiasValue = 0x9204,
        MaxApertureValue = 0x9205,
        SubjectDistance = 0x9206,
        MeteringMode = 0x9207,
        LightSource = 0x9208,
        Flash = 0x9209,
        FocalLength = 0x920A,
        FlashEnergy = 0x920B,
        SpatialFrequencyResponse = 0x920C,
        Noise = 0x920D,
        FocalPlaneXResolution = 0x920E,
        FocalPlaneYResolution = 0x920F,
        FocalPlaneResolutionUnit = 0x9210,
        ImageNumber = 0x9211,
        SecurityClassification = 0x9212,
        ImageHistory = 0x9213,
        SubjectLocation = 0x9214,
        ExposureIndex = 0x9215,
        TIFF_EPStandardID = 0x9216,
        SensingMethod = 0x9217,
        MakerNote = 0x927C,
        UserComment = 0x9286,
        SubsecTime = 0x9290,
        SubsecTimeOriginal = 0x9291,
        SubsecTimeDigitized = 0x9292,
        U0 = 0x932f,
        U1 = 0x9330,
        U2 = 0x9331,
        ImageSourceData = 0x935C,
        FlashpixVersion = 0xA000,
        ColorSpace = 0xA001,
        PixelXDimension = 0xA002,
        PixelYDimension = 0xA003,
        RelatedSoundFile = 0xA004,
        Interoperability_IFD = 0xA005,
        FlashEnergy_ = 0xA20B,
        SpatialFrequencyResponse_ = 0xA20C,
        FocalPlaneXResolution_ = 0xA20E,
        FocalPlaneYResolution_ = 0xA20F,
        FocalPlaneResolutionUnit_ = 0xA210,
        SubjectLocation_ = 0xA214,
        ExposureIndex_ = 0xA215,
        SensingMethod_ = 0xA217,
        FileSource = 0xA300,
        SceneType = 0xA301,
        CFAPattern_ = 0xA302,
        CustomRendered = 0xA401,
        ExposureMode = 0xA402,
        WhiteBalance = 0xA403,
        DigitalZoomRatio = 0xA404,
        FocalLengthIn35mmFilm = 0xA405,
        SceneCaptureType = 0xA406,
        GainControl = 0xA407,
        Contrast = 0xA408,
        Saturation = 0xA409,
        Sharpness = 0xA40A,
        DeviceSettingDescription = 0xA40B,
        SubjectDistanceRange = 0xA40C,
        ImageUniqueID = 0xA420,
        GDAL_METADATA = 0xA480,
        GDAL_NODATA = 0xA481,
        PixelFormat = 0xBC01,
        Transformation = 0xBC02,
        Uncompressed = 0xBC03,
        ImageType = 0xBC04,
        ImageWidth_ = 0xBC80,
        ImageHeight = 0xBC81,
        WidthResolution = 0xBC82,
        HeightResolution = 0xBC83,
        ImageOffset = 0xBCC0,
        ImageByteCount = 0xBCC1,
        AlphaOffset = 0xBCC2,
        AlphaByteCount = 0xBCC3,
        ImageDataDiscard = 0xBCC4,
        AlphaDataDiscard = 0xBCC5,
        ImageType_ = 0xBC04,
        Oce_Scanjob_Description = 0xC427,
        Oce_Application_Selector = 0xC428,
        Oce_Identification_Number = 0xC429,
        Oce_ImageLogic_Characteristics = 0xC42A,
        DNGVersion = 0xC612,
        DNGBackwardVersion = 0xC613,
        UniqueCameraModel = 0xC614,
        LocalizedCameraModel = 0xC615,
        CFAPlaneColor = 0xC616,
        CFALayout = 0xC617,
        LinearizationTable = 0xC618,
        BlackLevelRepeatDim = 0xC619,
        BlackLevel = 0xC61A,
        BlackLevelDeltaH = 0xC61B,
        BlackLevelDeltaV = 0xC61C,
        WhiteLevel = 0xC61D,
        DefaultScale = 0xC61E,
        DefaultCropOrigin = 0xC61F,
        DefaultCropSize = 0xC620,
        ColorMatrix1 = 0xC621,
        ColorMatrix2 = 0xC622,
        CameraCalibration1 = 0xC623,
        CameraCalibration2 = 0xC624,
        ReductionMatrix1 = 0xC625,
        ReductionMatrix2 = 0xC626,
        AnalogBalance = 0xC627,
        AsShotNeutral = 0xC628,
        AsShotWhiteXY = 0xC629,
        BaselineExposure = 0xC62A,
        BaselineNoise = 0xC62B,
        BaselineSharpness = 0xC62C,
        BayerGreenSplit = 0xC62D,
        LinearResponseLimit = 0xC62E,
        CameraSerialNumber = 0xC62F,
        LensInfo = 0xC630,
        ChromaBlurRadius = 0xC631,
        AntiAliasStrength = 0xC632,
        DNGPrivateData = 0xC634,
        MakerNoteSafety = 0xC635,
        CalibrationIlluminant1 = 0xC65A,
        CalibrationIlluminant2 = 0xC65B,
        BestQualityScale = 0xC65C,
        ColorimetricReference = 0xC6BF,
        CameraCalibrationSignature = 0xC6F3,
        ProfileCalibrationSignature = 0xC6F4,
        ExtraCameraProfiles = 0xC6F5,
        AsShotProfileName = 0xC6F6,
        NoiseReductionApplied = 0xC6F7,
        ProfileName = 0xC6F8,
        ProfileHueSatMapDims = 0xC6F9,
        ProfileHueSatMapData1 = 0xC6FA,
        ProfileHueSatMapData2 = 0xC6FB,
        ProfileToneCurve = 0xC6FC,
        ProfileEmbedPolicy = 0xC6FD,
        ProfileCopyright = 0xC6FE,
        ForwardMatrix1 = 0xC714,
        ForwardMatrix2 = 0xC715,
        PreviewApplicationName = 0xC716,
        PreviewApplicationVersion = 0xC717,
        PreviewSettingsName = 0xC718,
        PreviewSettingsDigest = 0xC719,
        PreviewColorSpace = 0xC71A,
        PreviewDateTime = 0xC71B,
        RawImageDigest = 0xC71C,
        OriginalRawFileDigest = 0xC71D,
        SubTileBlockSize = 0xC71E,
        RowInterleaveFactor = 0xC71F,
        ProfileLookTableDims = 0xC725,
        ProfileLookTableData = 0xC726,
        OpcodeList1 = 0xC740,
        OpcodeList2 = 0xC741,
        OpcodeList3 = 0xC74E,
        NoiseProfile = 0xC761,
        Padding = 0xea1c
    }
}