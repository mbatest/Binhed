using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExifLibrary
{
    #region Exif standard stringData
    public enum EXIFType
    {
        Exif,
        MPF,
        GPS,
        FujiFilm,
        Kodak,
        MinoltaDX,
        MinoltaDimage,
        Nikon,
        Olympus,
        Sony,
        Panasonic,
        FlashPix,
        Casio_Type2
    }
    [Serializable]
    public enum EXIFDataType
    {
        /// 
        /// Data contains unsigned bytes
        /// 
        ExifTypeUnsignedByte = 1,
        /// 
        /// Data contains a string
        /// 
        ExifTypeString = 2,
        /// 
        /// Data contains unsigned 2-byte values
        /// 
        ExifTypeUnsignedChar = 3,
        /// 
        /// Data contains unsigned 4-byte values
        /// 
        ExifTypeUnsignedInt = 4,
        /// 
        /// Data is fractional: each item in the stringData is 8 bytes long.
        /// The first 4 bytes of each item in the stringData contain the
        /// numerator (unsigned int), the second 4 bytes the denominator
        /// (also unsigned int).
        /// 
        ExifTypeUnsignedRational = 5,
        /// 
        /// Data contains signed bytes
        /// 
        ExifTypeSignedByte = 6,
        /// 
        /// Data has arbitrary stringData mPID, tag specific
        /// 
        ExifTypeUndefined = 7,
        /// 
        /// Data contains signed 2-byte values
        /// 
        ExifTypeSignedChar = 8,
        /// 
        /// Data contains signed 4-byte values
        /// 
        ExifTypeSignedInt = 9,
        /// 
        /// Data is fractional: each item in the stringData is 8 bytes long.
        /// The first 4 bytes of each item in the stringData contain the
        /// numerator (signed int), the second 4 bytes the denominator
        /// (also signed int).
        /// 
        ExifTypeSignedRational = 10,
        /// 
        /// Data contains 4-byte floating point values
        /// 
        ExifTypeFloat = 11,
        /// 
        /// Data contains 8-byte floating point values
        /// 
        ExifTypeDouble = 12
    }
    public enum EXIFFujiCode
    {
        Version = 0x000,
        InternalSerialNumber = 0x0010,//(this number is unique, and contains the date of manufacture, but doesn't necessarily correspond to the camera body number -- this needs to be checked)
        Quality = 0x1000,//
        Sharpness = 0x1001,//0x1 = Soft ;0x2 = Soft2 ;0x3 = Normal ;0x4 = Hard ;0x5 = Hard2 ;0x82 = Medium Soft ;0x84 = Medium Hard ;0x8000 = Film Simulation ;0xffff = n/a
        WhiteBalance = 0x1002,//0x0 = Auto ;0x100 = Daylight ;0x200 = Cloudy ;0x300 = Daylight Fluorescent ;0x301 = Day White Fluorescent ;0x302 = White Fluorescent ;0x303 = Warm White Fluorescent ;0x304 = Living Room Warm White Fluorescent ;0x400 = Incandescent ;0x500 = Flash ;0xf00 = Custom ;0xf01 = Custom2 ;0xf02 = Custom3 ;0xf03 = Custom4 ;0xf04 = Custom5 ;0xff0 = Kelvin
        Saturation = 0x1003,//0x0 = Normal ;0x80 = Medium High ;0x100 = High ;0x180 = Medium Low ;0x200 = Low ;0x300 = None (B&W) ;0x8000 = Film Simulation
        Contrast = 0x1004,//0x0 = Normal ;0x80 = Medium High ;0x100 = High ;0x180 = Medium Low ;0x200 = Low ;0x8000 = Film Simulation
        ColorTemperature = 0x1005,// 
        Contrast2 = 0x1006,//0x0 = Normal ;0x100 = High ;0x300 = Low
        WhiteBalanceFineTune = 0x100a,// 
        NoiseReduction = 0x100b,//0x40 = Low ;0x80 = Normal
        FujiFlashMode = 0x1010,//0 = Auto ;1 = On ;2 = Off ;3 = Red-eye reduction ;4 = External
        FlashExposureComp = 0x1011,
        Macro = 0x1020,//0 = Off ;1 = On
        FocusMode = 0x1021,//0 = Auto ;1 = Manual
        FocusPixel = 0x1023,
        SlowSync = 0x1030,//0 = Off ;1 = On
        PictureMode = 0x1031,//0x0 = Auto ;0x1 = Portrait ;0x2 = Landscape ;0x3 = Macro ;0x4 = Sports ;0x5 = Night Scene ;0x6 = Program AE ;0x7 = Natural Light ;0x8 = Anti-blur ;0x9 = Beach & Snow ;0xa = Sunset ;0xb = Museum ;0xc = Party ;0xd = Flower ;0xe = Text ;0xf = Natural Light & Flash ;0x10 = Beach ;0x11 = Snow ;0x12 = Fireworks ;0x13 = Underwater ;0x100 = Aperture-priority AE ;0x200 = Shutter speed priority AE ;0x300 = Manual
        EXRAuto = 0x1033,//0 = Auto ;1 = Manual
        EXRMode = 0x1034,//0x100 = HR (High Resolution) ;0x200 = SN (Signal to Noise priority) ;0x300 = DR (Dynamic Range priority)
        AutoBracketing = 0x1100,//0 = Off ;1 = On ;2 = No flash & flash
        SequenceNumber = 0x1101,
        ColorMode = 0x1210,//0x0 = Standard ;0x10 = Chrome ;0x30 = B & W
        BlurWarning = 0x1300,//0 = None ;1 = Blur Warning
        FocusWarning = 0x1301,//0 = Good ;1 = Out of focus
        ExposureWarning = 0x1302,//0 = Good ;1 = Bad exposure
        DynamicRange = 0x1400,//1 = Standard ;3 = Wide
        FilmMode = 0x1401,//0x0 = F0/Standard ;0x100 = F1/Studio Portrait ;0x110 = F1a/Studio Portrait Enhanced Saturation ;0x120 = F1b/Studio Portrait Smooth Skin Tone ;0x130 = F1c/Studio Portrait Increased Sharpness ;0x200 = F2/Fujichrome ;0x300 = F3/Studio Portrait Ex ;0x400 = F4/Velvia
        DynamicRangeSetting = 0x1402,//0x0 = Auto (100-400%) ;0x1 = Manual ;0x100 = Standard (100%) ;0x200 = Wide1 (230%) ;0x201 = Wide2 (400%) ;0x8000 = Film Simulation
        DevelopmentDynamicRange = 0x1403,
        MinFocalLength = 0x1404,// 
        MaxFocalLength = 0x1405,// 
        MaxApertureAtMinFocal = 0x1406,// 
        MaxApertureAtMaxFocal = 0x1407,// 
        AutoDynamicRange = 0x140b,// 
        FacesDetected = 0x4100,//
        FacePositions = 0x4103,//(left, top, right and bottom coordinates in full-sized image for each face detected)
        FileSource = 0x8000,// 
        OrderNumber = 0x8002,// 
        FrameNumber = 0x8003,//
        Parallax = 0xb211,//(only found in MPImage2 of .MPO images)

    }
    public enum EXIFDimageX
    {
        MakerNoteVersion = 0x0000,// 
        MinoltaCameraSettingsOld = 0x0001,//--> Minolta CameraSettings tags
        MinoltaCameraSettings = 0x0003,//--> Minolta CameraSettings tags
        MinoltaCameraSettings7D = 0x0004,//--> Minolta CameraSettings7D tags
        Minolta_0x0010_ = 0x0010,
        ImageStabilization = 0x0018,//(a block of binary stringData which exists in DiMAGE A2 (and A1/X1?) images only if image stabilization is enabled)
        WBInfoA100 = 0x0020,//--> Minolta WBInfoA100 tags , (currently decoded only for the Sony A100)
        CompressedImageSize = 0x0040,// 
        PreviewImage = 0x0081,//
        PreviewImageStart = 0x0088,// 
        PreviewImageLength = 0x0089,//
        SceneMode = 0x0100,//0 = Standard , 1 = Portrait , 2 = Text , 3 = Night Scene , 4 = Sunset , 5 = Sports , 6 = Landscape , 7 = Night Portrait , 8 = Macro , 9 = Super Macro , 16 = Auto , 17 = Night View/Portrait , 18 = Sweep Panorama , 23 = 3D Sweep Panorama
        ColorMode = 0x0101,// 0 = Natural color , 1 = Black & White , 2 = Vivid color , 3 = Solarization , 4 = Adobe RGB , 5 = Sepia , 9 = Natural , 12 = Portrait , 13 = Natural sRGB , 14 = Natural+ sRGB , 15 = Landscape , 16 = Evening , 17 = Night Scene , 18 = Night Portrait , 132 = Embed Adobe RGB , (Sony models) , 0 = Standard , 1 = Vivid , 2 = Portrait , 3 = Landscape , 4 = Sunset , 5 = Night View/Portrait , 6 = B&W , 7 = Adobe RGB , 12 = Neutral , 100 = Neutral , 101 = Clear , 102 = Deep , 103 = Light , 104 = Night View , 105 = Autumn Leaves
        MinoltaQuality = 0x0102,//0 = Raw , 1 = Super Fine , 2 = Fine , 3 = Standard , 4 = Economy , 5 = Extra fine
        MinoltaImageSize = 0x0103,//(quality for DiMAGE A2/7Hi) , 0 = Raw , 1 = Super Fine , 2 = Fine , 3 = Standard , 4 = Economy , 5 = Extra fine , (image size for other models except A200) , 1 = 1600x1200 , 2 = 1280x960 , 3 = 640x480 , 5 = 2560x1920 , 6 = 2272x1704 , 7 = 2048x1536
        FlashExposureComp = 0x0104,// 
        Teleconverter = 0x0105,//0x0 = None , 0x48 = Minolta AF 2x APO (D) , 0x50 = Minolta AF 2x APO II , 0x88 = Minolta AF 1.4x APO (D) , 0x90 = Minolta AF 1.4x APO II
        Minolta_0x0106_ = 0x0106,
        ImageStabilization2 = 0x0107,//1 = Off , 5 = On
        RawAndJpgRecording = 0x0109,//0 = Off , 1 = On
        ZoneMatching = 0x010a,//0 = ISO Setting Used , 1 = High Key , 2 = Low Key
        ColorTemperature = 0x010b,// 
        LensType = 0x010c,//(decimal values differentiate lenses which would otherwise have the same LensType, and are used by the Composite LensID tag when attempting to identify the specific lens model. "New" or "II" appear in brackets if the original version of the lens has the same LensType) , 0 = Minolta AF 28-85mm F3.5-4.5 New , 1 = Minolta AF 80-200mm F2.8 HS-APO G , 2 = Minolta AF 28-70mm F2.8 G , 3 = Minolta AF 28-80mm F4-5.6 , 5 = Minolta AF 35-70mm F3.5-4.5 [II] , 6 = Minolta AF 24-85mm F3.5-4.5 [New] , 7 = Minolta AF 100-300mm F4.5-5.6 APO [New] or 100-400mm or Sigma Lens , 7.1 = Minolta AF 100-400mm F4.5-6.7 APO , 7.2 = Sigma AF 100-300mm F4 EX DG IF , 8 = Minolta AF 70-210mm F4.5-5.6 [II] , 9 = Minolta AF 50mm F3.5 Macro , 10 = Minolta AF 28-105mm F3.5-4.5 [New] , 11 = Minolta AF 300mm F4 HS-APO G , 12 = Minolta AF 100mm F2.8 Soft Focus , 13 = Minolta AF 75-300mm F4.5-5.6 (New or II) , 14 = Minolta AF 100-400mm F4.5-6.7 APO , 15 = Minolta AF 400mm F4.5 HS-APO G , 16 = Minolta AF 17-35mm F3.5 G , 17 = Minolta AF 20-35mm F3.5-4.5 , 18 = Minolta AF 28-80mm F3.5-5.6 II , 19 = Minolta AF 35mm F1.4 G , 20 = Minolta/Sony 135mm F2.8 [T4.5] STF , 22 = Minolta AF 35-80mm F4-5.6 II , 23 = Minolta AF 200mm F4 Macro APO G , 24 = Minolta/Sony AF 24-105mm F3.5-4.5 (D) or Sigma or Tamron Lens , 24.1 = Sigma 18-50mm F2.8 , 24.2 = Sigma 17-70mm F2.8-4.5 (D) , 24.3 = Sigma 20-40mm F2.8 EX DG Aspherical IF , 24.4 = Sigma 18-200mm F3.5-6.3 DC , 24.5 = Sigma 20-40mm F2.8 EX DG Aspherical IF , 24.6 = Tamron SP AF 28-75mm F2.8 XR Di (IF) Macro , 25 = Minolta AF 100-300mm F4.5-5.6 APO (D) or Sigma Lens , 25.1 = Sigma 100-300mm F4 EX (APO (D) or D IF) , 25.2 = Sigma 70mm F2.8 EX DG Macro , 25.3 = Sigma 20mm F1.8 EX DG Aspherical RF , 25.4 = Sigma 30mm F1.4 DG EX , 27 = Minolta AF 85mm F1.4 G (D) , 28 = Minolta/Sony AF 100mm F2.8 Macro (D) or Tamron Lens , 28.1 = Tamron SP AF 90mm F2.8 Di Macro , 29 = Minolta/Sony AF 75-300mm F4.5-5.6 (D) , 30 = Minolta AF 28-80mm F3.5-5.6 (D) or Sigma Lens , 30.1 = Sigma AF 10-20mm F4-5.6 EX DC , 30.2 = Sigma AF 12-24mm F4.5-5.6 EX DG , 30.3 = Sigma 28-70mm EX DG F2.8 , 30.4 = Sigma 55-200mm F4-5.6 DC , 31 = Minolta/Sony AF 50mm F2.8 Macro (D) or F3.5 , 31.1 = Minolta/Sony AF 50mm F3.5 Macro , 32 = Minolta/Sony AF 300mm F2.8 G or 1.5x Teleconverter , 33 = Minolta/Sony AF 70-200mm F2.8 G , 35 = Minolta AF 85mm F1.4 G (D) Limited , 36 = Minolta AF 28-100mm F3.5-5.6 (D) , 38 = Minolta AF 17-35mm F2.8-4 (D) , 39 = Minolta AF 28-75mm F2.8 (D) , 40 = Minolta/Sony AF DT 18-70mm F3.5-5.6 (D) or 18-200m F3.5-6.3 , 40.1 = Sony AF DT 18-200mm F3.5-6.3 , 41 = Minolta/Sony AF DT 11-18mm F4.5-5.6 (D) or Tamron Lens , 41.1 = Tamron SP AF 11-18mm F4.5-5.6 Di II LD Aspherical IF , 42 = Minolta/Sony AF DT 18-200mm F3.5-6.3 (D) , 43 = Sony 35mm F1.4 G (SAL-35F14G) , 44 = Sony 50mm F1.4 (SAL-50F14) , 45 = Carl Zeiss Planar T* 85mm F1.4 ZA , 46 = Carl Zeiss Vario-Sonnar T* DT 16-80mm F3.5-4.5 ZA , 47 = Carl Zeiss Sonnar T* 135mm F1.8 ZA , 48 = Carl Zeiss Vario-Sonnar T* 24-70mm F2.8 ZA SSM (SAL-2470Z) , 49 = Sony AF DT 55-200mm F4-5.6 , 50 = Sony AF DT 18-250mm F3.5-6.3 , 51 = Sony AF DT 16-105mm F3.5-5.6 or 55-200mm F4-5.5 , 51.1 = Sony AF DT 55-200mm F4-5.5 , 52 = Sony 70-300mm F4.5-5.6 G SSM , 53 = Sony AF 70-400mm F4-5.6 G SSM (SAL-70400G) , 54 = Carl Zeiss Vario-Sonnar T* 16-35mm F2.8 ZA SSM (SAL-1635Z) , 55 = Sony DT 18-55mm F3.5-5.6 SAM (SAL-1855) , 56 = Sony AF DT 55-200mm F4-5.6 SAM , 57 = Sony AF DT 50mm F1.8 SAM , 58 = Sony AF DT 30mm F2.8 SAM Macro , 60 = Carl Zeiss Distagon T* 24mm F2 SSM , 61 = Sony 85mm F2.8 SAM (SAL-85F28) , 62 = Sony DT 35mm F1.8 SAM (SAL-35F18) , 128 = Tamron or Sigma Lens (128) , 128.1 = Tamron 18-200mm F3.5-6.3 , 128.2 = Tamron 28-300mm F3.5-6.3 , 128.3 = Tamron 80-300mm F3.5-6.3 , 128.4 = Tamron AF 28-200mm F3.8-5.6 XR Di Aspherical [IF] MACRO , 128.5 = Tamron SP AF 17-35mm F2.8-4 Di LD Aspherical IF , 128.6 = Sigma AF 50-150mm F2.8 EX DC APO HSM II , 128.7 = Sigma 10-20mm F3.5 EX DC HSM , 128.8 = Sigma 70-200mm F2.8 II EX DG APO MACRO HSM , 129 = Tamron Lens (129) , 129.1 = Tamron 200-400mm F5.6 LD , 129.2 = Tamron 70-300mm F4-5.6 LD , 131 = Tamron 20-40mm F2.7-3.5 SP Aspherical IF , 135 = Vivitar 28-210mm F3.5-5.6 , 136 = Tokina EMZ M100 AF 100mm F3.5 , 137 = Cosina 70-210mm F2.8-4 AF , 138 = Soligor 19-35mm F3.5-4.5 , 142 = Voigtlander 70-300mm F4.5-5.6 , 146 = Voigtlander Macro APO-Lanthar 125mm F2.5 SL , 255 = Tamron Lens (255) , 255.1 = Tamron SP AF 17-50mm F2.8 XR Di II LD Aspherical , 255.2 = Tamron AF 18-250mm F3.5-6.3 XR Di II LD , 255.3 = Tamron AF 55-200mm F4-5.6 Di II , 255.4 = Tamron AF 70-300mm F4-5.6 Di LD MACRO 1:2 , 255.5 = Tamron SP AF 200-500mm F5.0-6.3 Di LD IF , 255.6 = Tamron SP AF 10-24mm F3.5-4.5 Di II LD Aspherical IF , 255.7 = Tamron SP AF 70-200mm F2.8 Di LD IF Macro , 255.8 = Tamron SP AF 28-75mm F2.8 XR Di LD Aspherical IF , 25501 = Minolta AF 50mm F1.7 , 25511 = Minolta AF 35-70mm F4 or Other Lens , 25511.1 = Sigma UC AF 28-70mm F3.5-4.5 , 25511.2 = Sigma AF 28-70mm F2.8 , 25511.3 = Sigma M-AF 70-200mm F2.8 EX Aspherical , 25511.4 = Quantaray M-AF 35-80mm F4-5.6 , 25521 = Minolta AF 28-85mm F3.5-4.5 or Other Lens , 25521.1 = Tokina 19-35mm F3.5-4.5 , 25521.2 = Tokina 28-70mm F2.8 AT-X , 25521.3 = Tokina 80-400mm F4.5-5.6 AT-X AF II 840 , 25521.4 = Tokina AF PRO 28-80mm F2.8 AT-X 280 , 25521.5 = Tokina AT-X PRO II AF 28-70mm F2.6-2.8 270 , 25521.6 = Tamron AF 19-35mm F3.5-4.5 , 25521.7 = Angenieux AF 28-70mm F2.6 , 25531 = Minolta AF 28-135mm F4-4.5 or Sigma Lens , 25531.1 = Sigma ZOOM-alpha 35-135mm F3.5-4.5 , 25531.2 = Sigma 28-105mm F2.8-4 Aspherical , 25541 = Minolta AF 35-105mm F3.5-4.5 , 25551 = Minolta AF 70-210mm F4 Macro or Sigma Lens , 25551.1 = Sigma 70-210mm F4-5.6 APO , 25551.2 = Sigma M-AF 70-200mm F2.8 EX APO , 25551.3 = Sigma 75-200mm F2.8-3.5 , 25561 = Minolta AF 135mm F2.8 , 25571 = Minolta/Sony AF 28mm F2.8 , 25581 = Minolta AF 24-50mm F4 , 25601 = Minolta AF 100-200mm F4.5 , 25611 = Minolta AF 75-300mm F4.5-5.6 or Sigma Lens , 25611.1 = Sigma 70-300mm F4-5.6 DL Macro , 25611.2 = Sigma 300mm F4 APO Macro , 25611.3 = Sigma AF 500mm F4.5 APO , 25611.4 = Sigma AF 170-500mm F5-6.3 APO Aspherical , 25611.5 = Tokina AT-X AF 300mm F4 , 25611.6 = Tokina AT-X AF 400mm F5.6 SD , 25611.7 = Tokina AF 730 II 75-300mm F4.5-5.6 , 25621 = Minolta AF 50mm F1.4 [New] , 25631 = Minolta AF 300mm F2.8 APO or Sigma Lens , 25631.1 = Sigma AF 50-500mm F4-6.3 EX DG APO , 25631.2 = Sigma AF 170-500mm F5-6.3 APO Aspherical , 25631.3 = Sigma AF 500mm F4.5 EX DG APO , 25631.4 = Sigma 400mm F5.6 APO , 25641 = Minolta AF 50mm F2.8 Macro or Sigma Lens , 25641.1 = Sigma 50mm F2.8 EX Macro , 25651 = Minolta AF 600mm F4 , 25661 = Minolta AF 24mm F2.8 , 25721 = Minolta/Sony AF 500mm F8 Reflex , 25781 = Minolta/Sony AF 16mm F2.8 Fisheye or Sigma Lens , 25781.1 = Sigma 8mm F4 EX [DG] Fisheye , 25781.2 = Sigma 14mm F3.5 , 25781.3 = Sigma 15mm F2.8 Fisheye , 25791 = Minolta/Sony AF 20mm F2.8 , 25811 = Minolta AF 100mm F2.8 Macro [New] or Sigma or Tamron Lens , 25811.1 = Sigma AF 90mm F2.8 Macro , 25811.2 = Sigma AF 105mm F2.8 EX [DG] Macro , 25811.3 = Sigma 180mm F5.6 Macro , 25811.4 = Tamron 90mm F2.8 Macro , 25851 = Beroflex 35-135mm F3.5-4.5 , 25858 = Minolta AF 35-105mm F3.5-4.5 New or Tamron Lens , 25858.1 = Tamron 24-135mm F3.5-5.6 , 25881 = Minolta AF 70-210mm F3.5-4.5 , 25891 = Minolta AF 80-200mm F2.8 APO or Tokina Lens , 25891.1 = Tokina 80-200mm F2.8 , 25911 = Minolta AF 35mm F1.4 , 25921 = Minolta AF 85mm F1.4 G (D) , 25931 = Minolta AF 200mm F2.8 G APO , 25941 = Minolta AF 3x-1x F1.7-2.8 Macro , 25961 = Minolta AF 28mm F2 , 25971 = Minolta AF 35mm F2 [New] , 25981 = Minolta AF 100mm F2 , 26041 = Minolta AF 80-200mm F4.5-5.6 , 26051 = Minolta AF 35-80mm F4-5.6 , 26061 = Minolta AF 100-300mm F4.5-5.6 , 26071 = Minolta AF 35-80mm F4-5.6 , 26081 = Minolta AF 300mm F2.8 HS-APO G , 26091 = Minolta AF 600mm F4 HS-APO G , 26121 = Minolta AF 200mm F2.8 HS-APO G , 26131 = Minolta AF 50mm F1.7 New , 26151 = Minolta AF 28-105mm F3.5-4.5 xi , 26161 = Minolta AF 35-200mm F4.5-5.6 xi , 26181 = Minolta AF 28-80mm F4-5.6 xi , 26191 = Minolta AF 80-200mm F4.5-5.6 xi , 26201 = Minolta AF 28-70mm F2.8 G , 26211 = Minolta AF 100-300mm F4.5-5.6 xi , 26241 = Minolta AF 35-80mm F4-5.6 Power Zoom , 26281 = Minolta AF 80-200mm F2.8 G , 26291 = Minolta AF 85mm F1.4 New , 26311 = Minolta/Sony AF 100-300mm F4.5-5.6 APO , 26321 = Minolta AF 24-50mm F4 New , 26381 = Minolta AF 50mm F2.8 Macro New , 26391 = Minolta AF 100mm F2.8 Macro , 26411 = Minolta/Sony AF 20mm F2.8 New , 26421 = Minolta AF 24mm F2.8 New , 26441 = Minolta AF 100-400mm F4.5-6.7 APO , 26621 = Minolta AF 50mm F1.4 New , 26671 = Minolta AF 35mm F2 New , 26681 = Minolta AF 28mm F2 New , 26721 = Minolta AF 24-105mm F3.5-4.5 (D) , 45671 = Tokina 70-210mm F4-5.6 , 45741 = 2x Teleconverter or Tamron or Tokina Lens , 45741.1 = Tamron SP AF 90mm F2.5 , 45741.2 = Tokina RF 500mm F8.0 x2 , 45741.3 = Tokina 300mm F2.8 x2 , 45751 = 1.4x Teleconverter , 45851 = Tamron SP AF 300mm F2.8 LD IF , 65535 = T-Mount or Other Lens or no lens , 65535.1 = Arax MC 35mm F2.8 Tilt+Shift , 65535.2 = Arax MC 80mm F2.8 Tilt+Shift , 65535.3 = Zenitar MF 16mm F2.8 Fisheye M42 , 65535.4 = Samyang 500mm Mirror F8.0 , 65535.5 = Pentacon Auto 135mm F2.8 , 65535.6 = Pentacon Auto 29mm F2.8 , 65535.7 = Helios 44-2 58mm F2.0
        Minolta_0x010d_ = 0x010d,
        White_Balance_ = 0x010e,
        Minolta_0x0110_ = 0x0110,
        ColorCompensationFilter = 0x0111,//(ranges from -2 for green to +2 for magenta)
        WhiteBalanceFineTune = 0x0112,// 
        ImageStabilization3 = 0x0113,//(valid for Sony A100 only) , 0 = Off , 1 = On
        MinoltaCameraSettings5D = 0x0114,//--> Minolta CameraSettings5D tags , --> Minolta CameraSettingsA100 tags
        WhiteBalance = 0x0115,//0x0 = Auto , 0x1 = Color Temperature/Color Filter , 0x10 = Daylight , 0x20 = Cloudy , 0x30 = Shade , 0x40 = Tungsten , 0x50 = Flash , 0x60 = Fluorescent , 0x70 = Custom
        Minolta_0x0200_ = 0x0200,
        Quality_ = 0x0201,
        Macro_Mode_ = 0x0202,
        Minolta_0x0203_ = 0x0203,
        Digital_Zoom_ = 0x0204,
        Minolta_0x020e_ = 0x020e,
        Minolta_0x020f_ = 0x020f,
        Minolta_0x0210_ = 0x0210,
        Minolta_0x0211_ = 0x0211,
        Minolta_0x0212_ = 0x0212,
        Minolta_0x0213_ = 0x0213,
        Minolta_0x0214_ = 0x0214,
        Minolta_0x0215_ = 0x0215,
        Minolta_0x0216_ = 0x0216,
        Minolta_0x0217_ = 0x0217,
        Minolta_0x0218_ = 0x0218,
        Minolta_0x0219_ = 0x0219,
        Minolta_0x021a_ = 0x021a,
        Minolta_0x021b_ = 0x021b,
        Minolta_0x021c_ = 0x021c,
        Manual_WB_ = 0x021d,
        Minolta_0x021e_ = 0x021e,
        Minolta_0x021f_ = 0x021f,
        Minolta_0x0220_ = 0x0220,
        Minolta_0x0221_ = 0x0221,
        Minolta_0x0222_ = 0x0222,
        Minolta_0x0224_ = 0x0224,
        Print_IM_Data_ = 0x0e00,
        camera_settings_ = 0x0f00,
        Minolta_0x0f01_ = 0x0f01,
        Minolta_0x0f02_ = 0x0f02,

    }
    public enum EXIFOlympusCode
    {
        MakerNoteVersion = 0x0000,// ,
        MinoltaCameraSettingsOld = 0x0001,//--> Minolta CameraSettings tags,
        MinoltaCameraSettings = 0x0003,//--> Minolta CameraSettings tags,
        CompressedImageSize = 0x0040,// ,
        PreviewImageData = 0x0081,// ,
        PreviewImageStart = 0x0088,// ,
        PreviewImageLength = 0x0089,// ,
        ThumbnailImage = 0x0100,// ,
        BodyFirmwareVersion = 0x0104,// ,
        SpecialMode = 0x0200,//(3 numbers: 1. Shooting mode: 0,//Normal, 2,//Fast, 3,//Panorama; 2. Sequence Number; 3. Panorama Direction: 1,//Left-Right, 2,//Right-Left, 3,//Bottom-Top, 4,//Top-Bottom),
        Quality = 0x0201,//(Quality values are decoded based on the CameraType tag. All types represent SQ, HQ and SHQ as sequential integers, but in general SX-mPID cameras start with a value of 0 for SQ while others start with 1),
        Macro = 0x0202,// = 0Off ,1 ,// On ,2 ,// Super Macro,
        BWMode = 0x0203,//= 0 Off ,1 ,// On,
        DigitalZoom = 0x0204,// ,
        FocalPlaneDiagonal = 0x0205,// ,
        LensDistortionParams = 0x0206,// ,
        CameraType = 0x0207,// ,
        TextInfo = 0x0208,//--> Olympus TextInfo tags,
        CameraID = 0x0209,// ,
        EpsonImageWidth = 0x020b,// ,
        EpsonImageHeight = 0x020c,// ,
        EpsonSoftware = 0x020d,// ,
        PreviewImage = 0x0280,//(found in ERF and JPG images from some Epson models),
        PreCaptureFrames = 0x0300,// ,
        WhiteBoard = 0x0301,// ,
        OneTouchWB = 0x0302,// = 0Off ,1 ,// On ,2 ,// On (Preset),
        WhiteBalanceBracket = 0x0303,// ,
        WhiteBalanceBias = 0x0304,// ,
        SceneMode = 0x0403,// = 0Normal ,1 = Standard ,2 = Auto ,3 = Intelligent Auto ,4 = Portrait ,5 = Landscape+Portrait ,6 = Landscape ,7 = Night Scene ,8 = Night+Portrait ,9 = Sport ,10 = Self Portrait ,11 = Indoor ,12 = Beach & Snow ,13 = Beach ,14 = Snow ,15 = Self Portrait+Self Timer ,16 = Sunset ,17 = Cuisine ,18 = Documents ,19 = Candle ,20 = Fireworks ,21 = Available Light ,22 = Vivid ,23 = Underwater Wide1 ,24 = Underwater Macro ,25 = Museum ,26 = Behind Glass ,27 = Auction ,28 = Shoot & Select1 ,29 = Shoot & Select2 ,30 = Underwater Wide2 ,31 = Digital Image Stabilization ,32 = Face Portrait ,33 = Pet ,34 = Smile Shot ,101 = Magic Filter,
        SerialNumber = 0x0404,// ,
        Firmware = 0x0405,// ,
        PrintIM = 0x0e00,//--> PrintIM tags,
        DataDump = 0x0f00,// ,
        DataDump2 = 0x0f01,// ,
        ZoomedPreviewStart = 0x0f04,// ,
        ZoomedPreviewLength = 0x0f05,// ,
        ZoomedPreviewSize = 0x0f06,// ,
        ShutterSpeedValue = 0x1000,// ,
        ISOValue = 0x1001,// ,
        ApertureValue = 0x1002,// ,
        BrightnessValue = 0x1003,// ,
        FlashMode = 0x1004,//2 ,// On ,3 ,// Off,
        FlashDevice = 0x1005,//= 0 None ,1 = Internal ,4 = External ,5 = Internal + External,
        ExposureCompensation = 0x1006,// ,
        SensorTemperature = 0x1007,// ,
        LensTemperature = 0x1008,// ,
        LightCondition = 0x1009,// ,
        FocusRange = 0x100a,// = 0 Normal ,1 ,// Macro,
        FocusMode = 0x100b,//  = 0 Auto ,1 ,// Manual,
        ManualFocusDistance = 0x100c,// ,
        ZoomStepCount = 0x100d,// ,
        FocusStepCount = 0x100e,// ,
        Sharpness = 0x100f,// = 0Normal ,1 ,// Hard ,2 ,// Soft,
        FlashChargeLevel = 0x1010,// ,
        ColorMatrix = 0x1011,// ,
        BlackLevel = 0x1012,// ,
        ColorTemperatureBG = 0x1013,// ,
        ColorTemperatureRG = 0x1014,// ,
        WBMode = 0x1015,//1 ,// Auto ,'1 0' ,// Auto ,'1 2' ,// Auto (2) ,'1 4' ,// Auto (4) ,'2 2' ,// 3000 Kelvin ,'2 3' ,// 3700 Kelvin ,'2 4' ,// 4000 Kelvin ,'2 5' ,// 4500 Kelvin ,'2 6' ,// 5500 Kelvin ,'2 7' ,// 6500 Kelvin ,'2 8' ,// 7500 Kelvin ,'3 0' ,// One-touch,
        Unknown = 0x1016,
        RedBalance = 0x1017,// ,
        BlueBalance = 0x1018,// ,
        ColorMatrixNumber = 0x1019,// ,
        SerialNumber2 = 0x101a,
        ExternalFlashAE1_0 = 0x101b,// ,
        ExternalFlashAE2_0 = 0x101c,// ,
        InternalFlashAE1_0 = 0x101d,// ,
        InternalFlashAE2_0 = 0x101e,// ,
        ExternalFlashAE1 = 0x101f,// ,
        ExternalFlashAE2 = 0x1020,// ,
        InternalFlashAE1 = 0x1021,// ,
        InternalFlashAE2 = 0x1022,// ,

        FlashExposureComp = 0x1023,// ,
        InternalFlashTable = 0x1024,// ,
        ExternalFlashGValue = 0x1025,// ,
        ExternalFlashBounce = 0x1026,// 0 = No ,1 = Yes,
        ExternalFlashZoom = 0x1027,// ,
        ExternalFlashMode = 0x1028,// ,
        Contrast = 0x1029,//0 = High ,1 ,// Normal ,2 ,// Low,
        SharpnessFactor = 0x102a,// ,
        ColorControl = 0x102b,// ,
        ValidBits = 0x102c,// ,
        CoringFilter = 0x102d,// ,
        OlympusImageWidth = 0x102e,// ,
        OlympusImageHeight = 0x102f,// ,
        SceneDetect = 0x1030,// ,
        SceneArea = 0x1031,// ,
        SceneDetectData = 0x1033,// ,
        CompressionRatio = 0x1034,// ,
        PreviewImageValid = 0x1035,// No=0 ,1 ,// Yes,
        AFResult = 0x1038,// ,
        CCDScanMode = 0x1039,// =0Interlaced ,1 ,// Progressive,
        NoiseReduction = 0x103a,// =0Off ,1 ,// On,
        FocusStepInfinity = 0x103b,// ,
        FocusStepNear = 0x103c,// ,
        LightValueCenter = 0x103d,// ,
        LightValuePeriphery = 0x103e,// ,
    }
    public enum EXIFSONYCode
    {
        Quality = 0x0102,//0 = RAW ,1 = Super Fine ,2 = Fine ,3 = Standard ,4 = Economy ,5 = Extra Fine ,6 = RAW + JPEG ,7 = Compressed RAW ,8 = Compressed RAW + JPEG
        FlashExposureComp = 0x0104,// Teleconverter=0x0105=0x0 = None ,0x48 = Minolta AF 2x APO (D) ,0x50 = Minolta AF 2x APO II ,0x88 = Minolta AF 1.4x APO (D) ,0x90 = Minolta AF 1.4x APO II
        Teleconverter = 0x0105,
        WhiteBalanceFineTune = 0x0112,// 
        CameraSettings = 0x0114,//--> Sony CameraSettings tags ,--> Sony CameraSettings2 tags ,--> Sony CameraSettingsUnknown tags
        WhiteBalance = 0x0115,//0x0 = Auto ,0x1 = Color Temperature/Color Filter ,0x10 = Daylight ,0x20 = Cloudy ,0x30 = Shade ,0x40 = Tungsten ,0x50 = Flash ,0x60 = Fluorescent ,0x70 = Custom
        PrintIM = 0x0e00,//--> PrintIM tags
        MultiBurstMode = 0x1000,//(MultiBurst tags valid only for models with this feature, like the F88) ,0 = Off ,1 = On
        MultiBurstImageWidth = 0x1001,// 
        MultiBurstImageHeight = 0x1002,// 
        Panorama = 0x1003,//--> Sony Panorama tags
        Unknown = 0x2000,
        PreviewImage = 0x2001,// 
        Contrast = 0x2004,// 
        Saturation = 0x2005,// HDR=0x200a=0x0 = Off ,0x10001 = Auto ,0x10010 = 1 ,0x10012 = 2 ,0x10014 = 3 ,0x10016 = 4 ,0x10018 = 5 ,0x1001a = 6
        ShotInfo = 0x3000,//--> Sony ShotInfo tags
        Unknown1 = 0x9000,
        Unknown2 = 0x9001,
        Unknown3 = 0x9002,
        Unknown4 = 0x9003,
        Unknown5 = 0x9004,
        Unknown6 = 0x9005,
        Unknown7 = 0x9006,
        Unknown8 = 0x9007,
        Unknown9 = 0x9008,
        Unknown10 = 0x9009,
        Unknown11 = 0x900a,
        Unknown12 = 0x900b,
        Unknown13 = 0x900C,
        Unknown15 = 0xa000,
        Unknown16 = 0xa001,
        Unknown17 = 0xa100,
        Unknown18 = 0xa101,
        Unknown19 = 0xa200,

        FileFormat = 0xb000,//'0 0 0 2' = JPEG ,'1 0 0 0' = SR2 ,'2 0 0 0' = ARW 1.0 ,'3 0 0 0' = ARW 2.0 ,'3 1 0 0' = ARW 2.1 ,'3 2 0 0' = ARW 2.2
        SonyModelID = 0xb001,//2 = DSC-R1 ,256 = DSLR-A100 ,257 = DSLR-A900 ,258 = DSLR-A700 ,259 = DSLR-A200 ,260 = DSLR-A350 ,261 = DSLR-A300 ,263 = DSLR-A380/A390 ,264 = DSLR-A330 ,265 = DSLR-A230 ,266 = DSLR-A290,//  ,//269 = DSLR-A850 ,273 = DSLR-A550 ,274 = DSLR-A500 ,275 = DSLR-A450 ,278 = NEX-5 ,279 = NEX-3 ,280 = SLT-A33 ,281 = SLT-A55V ,282 = DSLR-A560 ,283 = DSLR-A580,//  
        ColorReproduction = 0xb020,// 
        ColorTemperature = 0xb021,// 
        ColorCompensationFilter = 0xb022,//(negative is green, positive is magenta)
        SceneMode = 0xb023,//0 = Standard ,1 = Portrait ,2 = Text ,3 = Night Scene ,4 = Sunset ,5 = Sports ,6 = Landscape ,7 = Night Portrait ,8 = Macro ,9 = Super Macro ,16 = Auto ,17 = Night View/Portrait ,18 = Sweep Panorama ,23 = 3D Sweep Panorama
        ZoneMatching = 0xb024,//0 = ISO Setting Used ,1 = High Key ,2 = Low Key
        DynamicRangeOptimizer = 0xb025,//0 = Off ,1 = Standard ,2 = Advanced Auto ,3 = Auto ,8 = Advanced Lv1 ,9 = Advanced Lv2 ,10 = Advanced Lv3 ,11 = Advanced Lv4 ,12 = Advanced Lv5 ,16 = 1 ,17 = 2 ,18 = 3 ,19 = 4 ,20 = 5
        ImageStabilization = 0xb026,//0 = Off ,1 = On
        LensType = 0xb027,//(decimal values differentiate lenses which would otherwise have the same LensType, and are used by the Composite LensID tag when attempting to identify the specific lens model. "New" or "II" appear in brackets if the original version of the lens has the same LensType) ,0 = Minolta AF 28-85mm F3.5-4.5 New ,1 = Minolta AF 80-200mm F2.8 HS-APO G ,2 = Minolta AF 28-70mm F2.8 G ,3 = Minolta AF 28-80mm F4-5.6 ,5 = Minolta AF 35-70mm F3.5-4.5 [II] ,6 = Minolta AF 24-85mm F3.5-4.5 [New] ,7 = Minolta AF 100-300mm F4.5-5.6 APO [New] or 100-400mm or Sigma Lens ,7.1 = Minolta AF 100-400mm F4.5-6.7 APO ,7.2 = Sigma AF 100-300mm F4 EX DG IF ,8 = Minolta AF 70-210mm F4.5-5.6 [II] ,9 = Minolta AF 50mm F3.5 Macro ,10 = Minolta AF 28-105mm F3.5-4.5 [New] ,11 = Minolta AF 300mm F4 HS-APO G ,12 = Minolta AF 100mm F2.8 Soft Focus ,13 = Minolta AF 75-300mm F4.5-5.6 (New or II) ,14 = Minolta AF 100-400mm F4.5-6.7 APO ,15 = Minolta AF 400mm F4.5 HS-APO G ,16 = Minolta AF 17-35mm F3.5 G ,17 = Minolta AF 20-35mm F3.5-4.5 ,18 = Minolta AF 28-80mm F3.5-5.6 II ,19 = Minolta AF 35mm F1.4 G ,20 = Minolta/Sony 135mm F2.8 [T4.5] STF ,22 = Minolta AF 35-80mm F4-5.6 II ,23 = Minolta AF 200mm F4 Macro APO G ,24 = Minolta/Sony AF 24-105mm F3.5-4.5 (D) or Sigma or Tamron Lens ,24.1 = Sigma 18-50mm F2.8 ,24.2 = Sigma 17-70mm F2.8-4.5 (D) ,24.3 = Sigma 20-40mm F2.8 EX DG Aspherical IF ,24.4 = Sigma 18-200mm F3.5-6.3 DC ,24.5 = Sigma 20-40mm F2.8 EX DG Aspherical IF ,24.6 = Tamron SP AF 28-75mm F2.8 XR Di (IF) Macro ,25 = Minolta AF 100-300mm F4.5-5.6 APO (D) or Sigma Lens ,25.1 = Sigma 100-300mm F4 EX (APO (D) or D IF) ,25.2 = Sigma 70mm F2.8 EX DG Macro ,25.3 = Sigma 20mm F1.8 EX DG Aspherical RF ,25.4 = Sigma 30mm F1.4 DG EX ,27 = Minolta AF 85mm F1.4 G (D) ,28 = Minolta/Sony AF 100mm F2.8 Macro (D) or Tamron Lens ,28.1 = Tamron SP AF 90mm F2.8 Di Macro ,29 = Minolta/Sony AF 75-300mm F4.5-5.6 (D) ,30 = Minolta AF 28-80mm F3.5-5.6 (D) or Sigma Lens ,30.1 = Sigma AF 10-20mm F4-5.6 EX DC ,30.2 = Sigma AF 12-24mm F4.5-5.6 EX DG ,30.3 = Sigma 28-70mm EX DG F2.8 ,30.4 = Sigma 55-200mm F4-5.6 DC ,31 = Minolta/Sony AF 50mm F2.8 Macro (D) or F3.5 ,31.1 = Minolta/Sony AF 50mm F3.5 Macro ,32 = Minolta/Sony AF 300mm F2.8 G or 1.5x Teleconverter ,33 = Minolta/Sony AF 70-200mm F2.8 G ,35 = Minolta AF 85mm F1.4 G (D) Limited ,36 = Minolta AF 28-100mm F3.5-5.6 (D) ,38 = Minolta AF 17-35mm F2.8-4 (D) ,39 = Minolta AF 28-75mm F2.8 (D) ,40 = Minolta/Sony AF DT 18-70mm F3.5-5.6 (D) or 18-200m F3.5-6.3 ,40.1 = Sony AF DT 18-200mm F3.5-6.3 ,41 = Minolta/Sony AF DT 11-18mm F4.5-5.6 (D) or Tamron Lens ,41.1 = Tamron SP AF 11-18mm F4.5-5.6 Di II LD Aspherical IF ,42 = Minolta/Sony AF DT 18-200mm F3.5-6.3 (D) ,43 = Sony 35mm F1.4 G (SAL-35F14G) ,44 = Sony 50mm F1.4 (SAL-50F14) ,45 = Carl Zeiss Planar T* 85mm F1.4 ZA ,46 = Carl Zeiss Vario-Sonnar T* DT 16-80mm F3.5-4.5 ZA ,47 = Carl Zeiss Sonnar T* 135mm F1.8 ZA ,48 = Carl Zeiss Vario-Sonnar T* 24-70mm F2.8 ZA SSM (SAL-2470Z) ,49 = Sony AF DT 55-200mm F4-5.6 ,50 = Sony AF DT 18-250mm F3.5-6.3 ,51 = Sony AF DT 16-105mm F3.5-5.6 or 55-200mm F4-5.5 ,51.1 = Sony AF DT 55-200mm F4-5.5 ,52 = Sony 70-300mm F4.5-5.6 G SSM ,53 = Sony AF 70-400mm F4-5.6 G SSM (SAL-70400G) ,54 = Carl Zeiss Vario-Sonnar T* 16-35mm F2.8 ZA SSM (SAL-1635Z) ,55 = Sony DT 18-55mm F3.5-5.6 SAM (SAL-1855) ,56 = Sony AF DT 55-200mm F4-5.6 SAM ,57 = Sony AF DT 50mm F1.8 SAM ,58 = Sony AF DT 30mm F2.8 SAM Macro ,60 = Carl Zeiss Distagon T* 24mm F2 SSM ,61 = Sony 85mm F2.8 SAM (SAL-85F28) ,62 = Sony DT 35mm F1.8 SAM (SAL-35F18) ,128 = Tamron or Sigma Lens (128) ,128.1 = Tamron 18-200mm F3.5-6.3 ,128.2 = Tamron 28-300mm F3.5-6.3 ,128.3 = Tamron 80-300mm F3.5-6.3 ,128.4 = Tamron AF 28-200mm F3.8-5.6 XR Di Aspherical [IF] MACRO ,128.5 = Tamron SP AF 17-35mm F2.8-4 Di LD Aspherical IF ,128.6 = Sigma AF 50-150mm F2.8 EX DC APO HSM II ,128.7 = Sigma 10-20mm F3.5 EX DC HSM ,128.8 = Sigma 70-200mm F2.8 II EX DG APO MACRO HSM ,129 = Tamron Lens (129) ,129.1 = Tamron 200-400mm F5.6 LD ,129.2 = Tamron 70-300mm F4-5.6 LD ,131 = Tamron 20-40mm F2.7-3.5 SP Aspherical IF ,135 = Vivitar 28-210mm F3.5-5.6 ,136 = Tokina EMZ M100 AF 100mm F3.5 ,137 = Cosina 70-210mm F2.8-4 AF ,138 = Soligor 19-35mm F3.5-4.5 ,142 = Voigtlander 70-300mm F4.5-5.6 ,146 = Voigtlander Macro APO-Lanthar 125mm F2.5 SL ,255 = Tamron Lens (255) ,255.1 = Tamron SP AF 17-50mm F2.8 XR Di II LD Aspherical ,255.2 = Tamron AF 18-250mm F3.5-6.3 XR Di II LD ,255.3 = Tamron AF 55-200mm F4-5.6 Di II ,255.4 = Tamron AF 70-300mm F4-5.6 Di LD MACRO 1,2 ,255.5 = Tamron SP AF 200-500mm F5.0-6.3 Di LD IF ,255.6 = Tamron SP AF 10-24mm F3.5-4.5 Di II LD Aspherical IF ,255.7 = Tamron SP AF 70-200mm F2.8 Di LD IF Macro ,255.8 = Tamron SP AF 28-75mm F2.8 XR Di LD Aspherical IF ,2550 = Minolta AF 50mm F1.7 ,2551 = Minolta AF 35-70mm F4 or Other Lens ,2551.1 = Sigma UC AF 28-70mm F3.5-4.5 ,2551.2 = Sigma AF 28-70mm F2.8 ,2551.3 = Sigma M-AF 70-200mm F2.8 EX Aspherical ,2551.4 = Quantaray M-AF 35-80mm F4-5.6 ,2552 = Minolta AF 28-85mm F3.5-4.5 or Other Lens ,2552.1 = Tokina 19-35mm F3.5-4.5 ,2552.2 = Tokina 28-70mm F2.8 AT-X ,2552.3 = Tokina 80-400mm F4.5-5.6 AT-X AF II 840 ,2552.4 = Tokina AF PRO 28-80mm F2.8 AT-X 280 ,2552.5 = Tokina AT-X PRO II AF 28-70mm F2.6-2.8 270 ,2552.6 = Tamron AF 19-35mm F3.5-4.5 ,2552.7 = Angenieux AF 28-70mm F2.6 ,2553 = Minolta AF 28-135mm F4-4.5 or Sigma Lens ,2553.1 = Sigma ZOOM-alpha 35-135mm F3.5-4.5 ,2553.2 = Sigma 28-105mm F2.8-4 Aspherical ,2554 = Minolta AF 35-105mm F3.5-4.5 ,2555 = Minolta AF 70-210mm F4 Macro or Sigma Lens ,2555.1 = Sigma 70-210mm F4-5.6 APO ,2555.2 = Sigma M-AF 70-200mm F2.8 EX APO ,2555.3 = Sigma 75-200mm F2.8-3.5 ,2556 = Minolta AF 135mm F2.8 ,2557 = Minolta/Sony AF 28mm F2.8 ,2558 = Minolta AF 24-50mm F4 ,2560 = Minolta AF 100-200mm F4.5 ,2561 = Minolta AF 75-300mm F4.5-5.6 or Sigma Lens ,2561.1 = Sigma 70-300mm F4-5.6 DL Macro ,2561.2 = Sigma 300mm F4 APO Macro ,2561.3 = Sigma AF 500mm F4.5 APO ,2561.4 = Sigma AF 170-500mm F5-6.3 APO Aspherical ,2561.5 = Tokina AT-X AF 300mm F4 ,2561.6 = Tokina AT-X AF 400mm F5.6 SD ,2561.7 = Tokina AF 730 II 75-300mm F4.5-5.6 ,2562 = Minolta AF 50mm F1.4 [New] ,2563 = Minolta AF 300mm F2.8 APO or Sigma Lens ,2563.1 = Sigma AF 50-500mm F4-6.3 EX DG APO ,2563.2 = Sigma AF 170-500mm F5-6.3 APO Aspherical ,2563.3 = Sigma AF 500mm F4.5 EX DG APO ,2563.4 = Sigma 400mm F5.6 APO ,2564 = Minolta AF 50mm F2.8 Macro or Sigma Lens ,2564.1 = Sigma 50mm F2.8 EX Macro ,2565 = Minolta AF 600mm F4 ,2566 = Minolta AF 24mm F2.8 ,2572 = Minolta/Sony AF 500mm F8 Reflex ,2578 = Minolta/Sony AF 16mm F2.8 Fisheye or Sigma Lens ,2578.1 = Sigma 8mm F4 EX [DG] Fisheye ,2578.2 = Sigma 14mm F3.5 ,2578.3 = Sigma 15mm F2.8 Fisheye ,2579 = Minolta/Sony AF 20mm F2.8 ,2581 = Minolta AF 100mm F2.8 Macro [New] or Sigma or Tamron Lens ,2581.1 = Sigma AF 90mm F2.8 Macro ,2581.2 = Sigma AF 105mm F2.8 EX [DG] Macro ,2581.3 = Sigma 180mm F5.6 Macro ,2581.4 = Tamron 90mm F2.8 Macro ,2585 = Minolta AF 35-105mm F3.5-4.5 New or Tamron Lens ,2585.1 = Beroflex 35-135mm F3.5-4.5 ,2585.2 = Tamron 24-135mm F3.5-5.6 ,2588 = Minolta AF 70-210mm F3.5-4.5 ,2589 = Minolta AF 80-200mm F2.8 APO or Tokina Lens ,2589.1 = Tokina 80-200mm F2.8 ,2591 = Minolta AF 35mm F1.4 ,2592 = Minolta AF 85mm F1.4 G (D) ,2593 = Minolta AF 200mm F2.8 G APO ,2594 = Minolta AF 3x-1x F1.7-2.8 Macro ,2596 = Minolta AF 28mm F2 ,2597 = Minolta AF 35mm F2 [New] ,2598 = Minolta AF 100mm F2 ,2604 = Minolta AF 80-200mm F4.5-5.6 ,2605 = Minolta AF 35-80mm F4-5.6 ,2606 = Minolta AF 100-300mm F4.5-5.6 ,2607 = Minolta AF 35-80mm F4-5.6 ,2608 = Minolta AF 300mm F2.8 HS-APO G ,2609 = Minolta AF 600mm F4 HS-APO G ,2612 = Minolta AF 200mm F2.8 HS-APO G ,2613 = Minolta AF 50mm F1.7 New ,2615 = Minolta AF 28-105mm F3.5-4.5 xi ,2616 = Minolta AF 35-200mm F4.5-5.6 xi ,2618 = Minolta AF 28-80mm F4-5.6 xi ,2619 = Minolta AF 80-200mm F4.5-5.6 xi ,2620 = Minolta AF 28-70mm F2.8 G ,2621 = Minolta AF 100-300mm F4.5-5.6 xi ,2624 = Minolta AF 35-80mm F4-5.6 Power Zoom ,2628 = Minolta AF 80-200mm F2.8 G ,2629 = Minolta AF 85mm F1.4 New ,2631 = Minolta/Sony AF 100-300mm F4.5-5.6 APO ,2632 = Minolta AF 24-50mm F4 New ,2638 = Minolta AF 50mm F2.8 Macro New ,2639 = Minolta AF 100mm F2.8 Macro ,2641 = Minolta/Sony AF 20mm F2.8 New ,2642 = Minolta AF 24mm F2.8 New ,2644 = Minolta AF 100-400mm F4.5-6.7 APO ,2662 = Minolta AF 50mm F1.4 New ,2667 = Minolta AF 35mm F2 New ,2668 = Minolta AF 28mm F2 New ,2672 = Minolta AF 24-105mm F3.5-4.5 (D) ,4567 = Tokina 70-210mm F4-5.6 ,4574 = 2x Teleconverter or Tamron or Tokina Lens ,4574.1 = Tamron SP AF 90mm F2.5 ,4574.2 = Tokina RF 500mm F8.0 x2 ,4574.3 = Tokina 300mm F2.8 x2 ,4575 = 1.4x Teleconverter ,4585 = Tamron SP AF 300mm F2.8 LD IF ,6553 = T-Mount or Other Lens or no lens ,6553.1 = Arax MC 35mm F2.8 Tilt+Shift ,6553.2 = Arax MC 80mm F2.8 Tilt+Shift ,6553.3 = Zenitar MF 16mm F2.8 Fisheye M42 ,6553.4 = Samyang 500mm Mirror F8.0 ,6553.5 = Pentacon Auto 135mm F2.8 ,6553.6 = Pentacon Auto 29mm F2.8 ,6553.7 = Helios 44-2 58mm F2.0 ,25501 = Minolta AF 50mm F1.7 ,25511 = Minolta AF 35-70mm F4 or Other Lens ,25511.1 = Sigma UC AF 28-70mm F3.5-4.5 ,25511.2 = Sigma AF 28-70mm F2.8 ,25511.3 = Sigma M-AF 70-200mm F2.8 EX Aspherical ,25511.4 = Quantaray M-AF 35-80mm F4-5.6 ,25521 = Minolta AF 28-85mm F3.5-4.5 or Other Lens ,25521.1 = Tokina 19-35mm F3.5-4.5 ,25521.2 = Tokina 28-70mm F2.8 AT-X ,25521.3 = Tokina 80-400mm F4.5-5.6 AT-X AF II 840 ,25521.4 = Tokina AF PRO 28-80mm F2.8 AT-X 280 ,25521.5 = Tokina AT-X PRO II AF 28-70mm F2.6-2.8 270 ,25521.6 = Tamron AF 19-35mm F3.5-4.5 ,25521.7 = Angenieux AF 28-70mm F2.6 ,25531 = Minolta AF 28-135mm F4-4.5 or Sigma Lens ,25531.1 = Sigma ZOOM-alpha 35-135mm F3.5-4.5 ,25531.2 = Sigma 28-105mm F2.8-4 Aspherical ,25541 = Minolta AF 35-105mm F3.5-4.5 ,25551 = Minolta AF 70-210mm F4 Macro or Sigma Lens ,25551.1 = Sigma 70-210mm F4-5.6 APO ,25551.2 = Sigma M-AF 70-200mm F2.8 EX APO ,25551.3 = Sigma 75-200mm F2.8-3.5 ,25561 = Minolta AF 135mm F2.8 ,25571 = Minolta/Sony AF 28mm F2.8 ,25581 = Minolta AF 24-50mm F4 ,25601 = Minolta AF 100-200mm F4.5 ,25611 = Minolta AF 75-300mm F4.5-5.6 or Sigma Lens ,25611.1 = Sigma 70-300mm F4-5.6 DL Macro ,25611.2 = Sigma 300mm F4 APO Macro ,25611.3 = Sigma AF 500mm F4.5 APO ,25611.4 = Sigma AF 170-500mm F5-6.3 APO Aspherical ,25611.5 = Tokina AT-X AF 300mm F4 ,25611.6 = Tokina AT-X AF 400mm F5.6 SD ,25611.7 = Tokina AF 730 II 75-300mm F4.5-5.6 ,25621 = Minolta AF 50mm F1.4 [New] ,25631 = Minolta AF 300mm F2.8 APO or Sigma Lens ,25631.1 = Sigma AF 50-500mm F4-6.3 EX DG APO ,25631.2 = Sigma AF 170-500mm F5-6.3 APO Aspherical ,25631.3 = Sigma AF 500mm F4.5 EX DG APO ,25631.4 = Sigma 400mm F5.6 APO ,25641 = Minolta AF 50mm F2.8 Macro or Sigma Lens ,25641.1 = Sigma 50mm F2.8 EX Macro ,25651 = Minolta AF 600mm F4 ,25661 = Minolta AF 24mm F2.8 ,25721 = Minolta/Sony AF 500mm F8 Reflex ,25781 = Minolta/Sony AF 16mm F2.8 Fisheye or Sigma Lens ,25781.1 = Sigma 8mm F4 EX [DG] Fisheye ,25781.2 = Sigma 14mm F3.5 ,25781.3 = Sigma 15mm F2.8 Fisheye ,25791 = Minolta/Sony AF 20mm F2.8 ,25811 = Minolta AF 100mm F2.8 Macro [New] or Sigma or Tamron Lens ,25811.1 = Sigma AF 90mm F2.8 Macro ,25811.2 = Sigma AF 105mm F2.8 EX [DG] Macro ,25811.3 = Sigma 180mm F5.6 Macro ,25811.4 = Tamron 90mm F2.8 Macro ,25851 = Beroflex 35-135mm F3.5-4.5 ,25858 = Minolta AF 35-105mm F3.5-4.5 New or Tamron Lens ,25858.1 = Tamron 24-135mm F3.5-5.6 ,25881 = Minolta AF 70-210mm F3.5-4.5 ,25891 = Minolta AF 80-200mm F2.8 APO or Tokina Lens ,25891.1 = Tokina 80-200mm F2.8 ,25911 = Minolta AF 35mm F1.4 ,25921 = Minolta AF 85mm F1.4 G (D) ,25931 = Minolta AF 200mm F2.8 G APO ,25941 = Minolta AF 3x-1x F1.7-2.8 Macro ,25961 = Minolta AF 28mm F2 ,25971 = Minolta AF 35mm F2 [New] ,25981 = Minolta AF 100mm F2 ,26041 = Minolta AF 80-200mm F4.5-5.6 ,26051 = Minolta AF 35-80mm F4-5.6 ,26061 = Minolta AF 100-300mm F4.5-5.6 ,26071 = Minolta AF 35-80mm F4-5.6 ,26081 = Minolta AF 300mm F2.8 HS-APO G ,26091 = Minolta AF 600mm F4 HS-APO G ,26121 = Minolta AF 200mm F2.8 HS-APO G ,26131 = Minolta AF 50mm F1.7 New ,26151 = Minolta AF 28-105mm F3.5-4.5 xi ,26161 = Minolta AF 35-200mm F4.5-5.6 xi ,26181 = Minolta AF 28-80mm F4-5.6 xi ,26191 = Minolta AF 80-200mm F4.5-5.6 xi ,26201 = Minolta AF 28-70mm F2.8 G ,26211 = Minolta AF 100-300mm F4.5-5.6 xi ,26241 = Minolta AF 35-80mm F4-5.6 Power Zoom ,26281 = Minolta AF 80-200mm F2.8 G ,26291 = Minolta AF 85mm F1.4 New ,26311 = Minolta/Sony AF 100-300mm F4.5-5.6 APO ,26321 = Minolta AF 24-50mm F4 New ,26381 = Minolta AF 50mm F2.8 Macro New ,26391 = Minolta AF 100mm F2.8 Macro ,26411 = Minolta/Sony AF 20mm F2.8 New ,26421 = Minolta AF 24mm F2.8 New ,26441 = Minolta AF 100-400mm F4.5-6.7 APO ,26621 = Minolta AF 50mm F1.4 New ,26671 = Minolta AF 35mm F2 New ,26681 = Minolta AF 28mm F2 New ,26721 = Minolta AF 24-105mm F3.5-4.5 (D) ,45671 = Tokina 70-210mm F4-5.6 ,45741 = 2x Teleconverter or Tamron or Tokina Lens ,45741.1 = Tamron SP AF 90mm F2.5 ,45741.2 = Tokina RF 500mm F8.0 x2 ,45741.3 = Tokina 300mm F2.8 x2 ,45751 = 1.4x Teleconverter ,45851 = Tamron SP AF 300mm F2.8 LD IF ,65535 = T-Mount or Other Lens or no lens ,65535.1 = Arax MC 35mm F2.8 Tilt+Shift ,65535.2 = Arax MC 80mm F2.8 Tilt+Shift ,65535.3 = Zenitar MF 16mm F2.8 Fisheye M42 ,65535.4 = Samyang 500mm Mirror F8.0 ,65535.5 = Pentacon Auto 135mm F2.8 ,65535.6 = Pentacon Auto 29mm F2.8 ,65535.7 = Helios 44-2 58mm F2.0
        MinoltaMakerNote = 0xb028,//--> Minolta tags
        ColorMode = 0xb029,//0 = Standard ,1 = Vivid ,2 = Portrait ,3 = Landscape ,4 = Sunset ,5 = Night View/Portrait ,6 = B&W ,7 = Adobe RGB ,12 = Neutral ,100 = Neutral ,101 = Clear ,102 = Deep ,103 = Light ,104 = Night View ,105 = Autumn Leaves
        FullImageSize = 0xb02b,// 
        PreviewImageSize = 0xb02c,// 
        Macro = 0xb040,//0 = Off ,1 = On ,2 = Close Focus ,65535 = n/a
        ExposureMode = 0xb041,//0 = Auto ,1 = Portrait ,2 = Beach ,4 = Snow ,5 = Landscape ,6 = Program ,7 = Aperture Priority ,8 = Shutter Priority ,9 = Night Scene / Twilight ,10 = Hi-Speed Shutter ,11 = Twilight Portrait ,12 = Soft Snap ,13 = Fireworks ,14 = Smile Shutter ,15 = Manual ,18 = High Sensitivity ,20 = Advanced Sports Shooting ,29 = Underwater ,33 = Gourmet ,34 = Panorama ,35 = Handheld Twilight ,36 = Anti Motion Blur ,37 = Pet ,38 = Backlight Correction HDR ,65535 = n/a
        FocusMode = 0xb042,//1 = AF-S ,2 = AF-C ,4 = Permanent-AF ,65535 = n/a
        AFMode = 0xb043,//0 = Default ,1 = Multi AF ,2 = Center AF ,3 = Spot AF ,4 = Flexible Spot AF ,6 = Touch AF ,14 = Manual Focus ,15 = Face Detected ,65535 = n/a
        AFIlluminator = 0xb044,//0 = Off ,1 = Auto ,65535 = n/a
        Unknown24 = 0xb045,
        Unknown25 = 0xb046,
        Quality2 = 0xb047,//0 = Normal ,1 = Fine ,65535 = n/a
        FlashLevel = 0xb048,//-32768 = Low ,-1 = n/a ,0 = Normal ,32767 = High
        ReleaseMode = 0xb049,//0 = Normal ,2 = Burst ,5 = Exposure Bracketing ,6 = White Balance Bracketing ,65535 = n/a
        SequenceNumber = 0xb04a,//(shot number in continuous burst) ,0 = Single ,65535 = n/a
        AntiBlur = 0xb04b,//0 = Off ,1 = On (Continuous) ,2 = On (Shooting) ,65535 = n/a
        Unknown20 = 0xb04c,
        Unknown21 = 0xb04d,
        LongExposureNoiseReduction = 0xb04e,//0 = Off ,1 = On ,65535 = n/a
        DynamicRangeOptimizer2 = 0xb04f,//0 = Off ,1 = Standard ,2 = Plus
        Unknown22 = 0xb050,
        Unknown23 = 0xb051,

        IntelligentAuto = 0xb052,//0 = Off ,1 = On ,2 = Advanced
        WhiteBalance2 = 0xb054,//0 = Auto ,4 = Manual ,5 = Daylight ,6 = Cloudy ,7 = White Flourescent ,8 = Cool White Flourescent ,9 = Day White Flourescent ,14 = Incandescent ,15 = Flash ,17 = Underwater 1 (Blue Water) ,18 = Underwater 2 (Green Water)
    }
    public enum EXIFCasio_Type2
    {
        PreviewImageSize = 0x0002,//int16u[2]	
        PreviewImageLength = 0x0003,//int32u*	
        PreviewImageStart = 0x0004,//int32u*	
        QualityMode = 0x0008,//int16u	0 = Economy ;1 = Normal ;2 = Fine
        CasioImageSize = 0x0009,//int16u	0 = 640x480 ;4 = 1600x1200 ;5 = 2048x1536 ;20 = 2288x1712 ;21 = 2592x1944 ;22 = 2304x1728 ;36 = 3008x2008
        FocusMode = 0x000d,//int16u	0 = Normal ;1 = Macro
        ISO = 0x0014,//int16u	3 = 50 ;4 = 64 ;6 = 100 ;9 = 200
        WhiteBalance = 0x0019,//int16u	0 = Auto ;1 = Daylight ;2 = Shade ;3 = Tungsten ;4 = Fluorescent ;5 = Manual
        FocalLength = 0x001d,//rational64u	
        Saturation = 0x001f,//int16u	0 = Low ;1 = Normal ;2 = High
        Contrast = 0x0020,//int16u	0 = Low ;1 = Normal ;2 = High
        Sharpness = 0x0021,//int16u	0 = Soft ;1 = Normal ;2 = Hard
        PrintIM = 0x0e00,//-	--> PrintIM Tags
        PreviewImage = 0x2000,//undef	
        FirmwareDate = 0x2001,//string[18]	
        WhiteBalanceBias = 0x2011,//int16u[2]	
        WhiteBalance2 = 0x2012,//int16u	0 = Manual ;1 = Daylight ;3 = Shade ;4 = Flash? ;6 = Fluorescent ;9 = Tungsten? ;10 = Tungsten ;12 = Flash
        AFPointPosition = 0x2021,//int16u[4]~	
        ObjectDistance = 0x2022,//int32u	
        FlashDistance = 0x2034,//int16u	
        SpecialEffectMode = 0x2076,//int8u[3]	'0 0 0' = Off ;'1 0 0' = Makeup ;'2 0 0' = Mist Removal ;'3 0 0' = Vivid Landscape
        FaceInfo1 = 0x2089,//-;-;Y	--> Casio FaceInfo1 Tags;--> Casio FaceInfo2 Tags
        FacesDetected = 0x211c,//int8u	
        RecordMode = 0x3000,//int16u	2 = Program AE ;3 = Shutter Priority ;4 = Aperture Priority ;5 = Manual ;6 = Best Shot ;17 = Movie ;19 = Movie (19) ;20 = YouTube Movie ;'2 0' = Program AE ;'3 0' = Shutter Priority ;'4 0' = Aperture Priority ;'5 0' = Manual ;'6 0' = Best Shot
        ReleaseMode = 0x3001,//int16u	1 = Normal ;3 = AE Bracketing ;11 = WB Bracketing ;13 = Contrast Bracketing ;19 = High Speed Burst
        Quality = 0x3002,//int16u	1 = Economy ;2 = Normal ;3 = Fine
        FocusMode2 = 0x3003,//int16u	0 = Manual ;1 = Focus Lock ;2 = Macro ;3 = Single-Area Auto Focus ;5 = Infinity ;6 = Multi-Area Auto Focus ;8 = Super Macro
        HometownCity = 0x3006,//string	
        BestShotMode = 0x3007,//int16u	
        AutoISO = 0x3008,//int16u	1 = On ;2 = Off ;7 = On (high sensitivity) ;8 = On (anti-shake) ;10 = High Speed
        AFMode = 0x3009,//int16u	0 = Off ;1 = Spot ;2 = Multi ;3 = Face Detection ;4 = Tracking ;5 = ,//,//intelligent
        Sharpness2 = 0x3011,//undef[2]	
        Contrast2 = 0x3012,//undef[2]	
        Saturation2 = 0x3013,//undef[2]	
        ISO2 = 0x3014,//int16u	
        ColorMode = 0x3015,//int16u	0 = Off ;2 = Black & White ;3 = Sepia
        Enhancement = 0x3016,//int16u	0 = Off ;1 = Scenery ;3 = Green ;5 = ,//underwater ;9 = Flesh Tones
        ColorFilter = 0x3017,//int16u	0 = Off ;1 = Blue ;3 = Green ;4 = Yellow ;5 = Red ;6 = Purple ;7 = Pink
        UnknownMode = 0x301b,//int16u	0 = Normal ;8 = Silent Movie ;39 = HDR ;45 = Premium Auto ;47 = Pa,//,//inting ;49 = Crayon Drawing ;51 = Panorama ;52 = Art HDR
        SequenceNumber = 0x301c,//int16u	
        BracketSequence = 0x301d,//int16u[2]	
        ImageStabilization = 0x3020,//int16u	0 = Off ;1 = On ;2 = Best Shot ;'0 0' = Off ;'0 3' = CCD Shift ;'2 3' = High Speed Anti-Shake ;'16 0' = Slow Shutter ;'18 0' = Anti-Shake ;'20 0' = High Sensitivity
        LightingMode = 0x302a,//int16u	0 = Off ;1 = High Dynamic Range ;5 = Shadow Enhance Low ;6 = Shadow Enhance High
        PortraitRefiner = 0x302b,//int16u	0 = Off ;1 = +1 ;2 = +2
        SpecialEffectLevel = 0x3030,//int16u	
        SpecialEffectSetting = 0x3031,//int16u	0 = Off ;1 = Makeup ;2 = Mist Removal ;3 = Vivid Landscape ;16 = Art Shot
        DriveMode = 0x3103,//int16u				

        CaptureFrameRate = 0x4001,//int16u[n]	
        VideoQuality = 0x4003,//int16u	1 = Standard ;3 = HD (720p) ;4 = Full HD (1080p) ;5 = Low
    }
    public enum EXIFNikon
    {
        MakerNoteVersion = 0x0001,
        ISO = 0x0002,
        ColorMode = 0x0003,
        Quality = 0x0004,
        WhiteBalance = 0x0005,
        Sharpness = 0x0006,
        FocusMode = 0x0007,
        FlashSetting = 0x0008,
        FlashType = 0x0009,
        WhiteBalanceFineTune = 0x000b,
        WB_RBLevels = 0x000c,
        ProgramShift = 0x000d,
        ExposureDifference = 0x000e,
        ISOSelection = 0x000f,
        DataDump = 0x0010,
        PreviewIFD = 0x0011,
        FlashExposureComp = 0x0012,
        ISOSetting = 0x0013,
        ColorBalanceA = 0x0014,
        ImageBoundary = 0x0016,
        ExternalFlashExposureComp = 0x0017,
        FlashExposureBracketValue = 0x0018,
        ExposureBracketValue = 0x0019,
        ImageProcessing = 0x001a,
        CropHiSpeed = 0x001b,
        ExposureTuning = 0x001c,
        SerialNumber = 0x001d,
        ColorSpace = 0x001e,
        VRInfo = 0x001f,
        ImageAuthentication = 0x0020,
        FaceDetect = 0x0021,
        ActiveD_Lighting = 0x0022,
        PictureControlData = 0x0023,
        WorldTime = 0x0024,
        ISOInfo = 0x0025,
        VignetteControl = 0x002a,
        DistortInfo = 0x002b,
        UnknownInfo = 0x002c,
        UnknownInfo2 = 0x0032,
        LocationInfo = 0x0039,
        ImageAdjustment = 0x0080,
        ToneComp = 0x0081,
        AuxiliaryLens = 0x0082,
        LensType = 0x0083,
        Lens = 0x0084,
        ManualFocusDistance = 0x0085,
        DigitalZoom = 0x0086,
        FlashMode = 0x0087,
        AFInfo = 0x0088,
        ShootingMode = 0x0089,
        LensFStops = 0x008b,
        ContrastCurve = 0x008c,
        ColorHue = 0x008d,
        SceneMode = 0x008f,
        LightSource = 0x0090,
        ShotInfoD40 = 0x0091,
        HueAdjustment = 0x0092,
        NEFCompression = 0x0093,
        Saturation = 0x0094,
        NoiseReduction = 0x0095,
        NEFLinearizationTable = 0x0096,
        ColorBalance0100 = 0x0097,
        LensData0100 = 0x0098,
        RawImageCenter = 0x0099,
        SensorPixelSize = 0x009a,
        SceneAssist = 0x009c,
        RetouchHistory = 0x009e,
        SerialNumber2 = 0x00a0,
        ImageDataSize = 0x00a2,
        ImageCount = 0x00a5,
        DeletedImageCount = 0x00a6,
        ShutterCount = 0x00a7,
        FlashInfo0100 = 0x00a8,
        ImageOptimization = 0x00a9,
        Saturation2 = 0x00aa,
        VariProgram = 0x00ab,
        ImageStabilization = 0x00ac,
        AFResponse = 0x00ad,
        MultiExposure = 0x00b0,
        HighISONoiseReduction = 0x00b1,
        ToningEffect = 0x00b3,
        PowerUpTime = 0x00b6,
        AFInfo2 = 0x00b7,
        FileInfo = 0x00b8,
        AFTune = 0x00b9,
        PictureControlData2 = 0x00bd,
        PrintIM = 0x0e00,
        NikonCaptureData = 0x0e01,
        NikonCaptureVersion = 0x0e09,
        NikonCaptureOffsets = 0x0e0e,
        NikonScanIFD = 0x0e10,
        NikonCaptureEditVersions = 0x0e13,
        NikonICCProfile = 0x0e1d,
        NikonCaptureOutput = 0x0e1e,
        NEFBitDepth = 0x0e22
    }
    [Serializable]
    public enum EXIFGPSCode
    {
        #region GPS tags
        Unknown = 0xffff,
        GPSVersionID = 0x0000,
        GPSLatitudeRef = 0x0001,//'N' = North 'S' = South
        GPSLatitude = 0x0002,
        GPSLongitudeRef = 0x0003,//'E' = East ,'W' = West
        GPSLongitude = 0x0004,
        GPSAltitudeRef = 0x0005,//0 = Above Sea Level ,1 = Below Sea Level
        GPSAltitude = 0x0006,
        GPSTimeStamp = 0x0007,//(when writing, date is stripped off if present, and time is adjusted to UTC if it includes a timezone)
        GPSSatellites = 0x0008,
        GPSStatus = 0x0009,//'A' = Measurement Active ,'outTime' = Measurement Void
        GPSMeasureMode = 0x000a,//2 = 2-Dimensional Measurement ,3 = 3-Dimensional Measurement
        GPSDOP = 0x000b,
        GPSSpeedRef = 0x000c,//'K' = km/h ,'M' = mph ,'N' = knots
        GPSSpeed = 0x000d,
        GPSTrackRef = 0x000e,//'M' = Magnetic North ,'T' = True North
        GPSTrack = 0x000f,
        GPSImgDirectionRef = 0x0010,//M' = Magnetic North ,'T' = True North
        GPSImgDirection = 0x0011,
        GPSMapDatum = 0x0012,
        GPSDestLatitudeRef = 0x0013,//'N' = North ,'S' = South
        GPSDestLatitude = 0x0014,
        GPSDestLongitudeRef = 0x0015,//'E' = East ,'W' = West
        GPSDestLongitude = 0x0016,
        GPSDestBearingRef = 0x0017,//'M' = Magnetic North ,'T' = True North
        GPSDestBearing = 0x0018,
        GPSDestDistanceRef = 0x0019,//'K' = Kilometers ,'M' = Miles ,'N' = Nautical Miles
        GPSDestDistance = 0x001a,
        GPSProcessingMethod = 0x001b,//(values of "GPS", "CELLID", "WLAN" or "MANUAL" by the EXIF spec.)
        GPSAreaInformation = 0x001c,
        GPSDateStamp = 0x001d,//(when writing, date is stripped off if present, and time is adjusted to UTC if it includes a timezone)
        GPSDifferential = 0x001e,//0 = No Correction ,1 = Differential Corrected
        GPSHPositioningError = 0x001f,
        #endregion
    }
    [Serializable]
    public enum EXIFCode
    {
        #region Standard Exif Codes
        Unknown = 0x0000,
        /// <summary>
        /// If this IFD is main image's IFD and the file content is equivalent to ExifR98 v1.0, the value is "R98". If thumbnail image's, value is "THM". 
        /// Ascii string  4  
        /// </summary>
        Interoperability_Index = 0x0001,
        /// <summary>
        ///  Records the interoperability version. "0100" means version 1.00. 
        ///  Undefined  4  
        /// </summary>
        Interoperability_Version = 0x0002,
        /// <summary>
        /// Describes image. Two-byte character code such as Chinese/Korean/Japanese cannot be used. 
        ///  ascii string 
        /// </summary>
        Image_Description = 0x010e,
        /// <summary>
        /// Manfacturer of digicam. In the Exif standard, this tag is optional, but it is mandatory for DCF. 
        /// ascii string 
        /// </summary>
        Manufacturer = 0x010f,
        /// <summary>
        /// Shows model number of digicam. In the Exif standard, this tag is optional, but it is mandatory for DCF. 
        /// ascii string 
        /// </summary>
        Model = 0x0110,
        /// <summary>
        ///  Shows size of thumbnail image. 
        ///  unsigned short/long 1  
        /// </summary>
        Image_Width = 0x0100,
        /// <summary>
        ///  Shows size of thumbnail image. 
        /// unsigned short/long 1  
        /// </summary>
        Image_Length = 0x0101,
        /// <summary>
        ///  When image format is no compression, this value shows the number of bits per component for each pixel. Usually this value is '8,8,8' 
        ///  unsigned short 3  
        /// </summary>
        Bits_Per_Sample = 0x0102,
        /// <summary>
        ///  Shows compression method. '1' means no compression, '6' means JPEG compression. 
        ///  unsigned short 1  
        /// </summary>
        Compression = 0x0103,
        /// <summary>
        ///  Shows the color space of the image stringData components. '1' means monochrome, '2' means RGB, '6' means YCbCr. 
        ///  unsigned short 1  
        /// </summary>
        Photometric_Interpretation = 0x0106,
        /// <summary>
        /// When image format is no compression, this value shows offset to image stringData. In some case image stringData is striped and this value is plural. 
        /// unsigned short/long 
        /// </summary>
        Strip_Offsets = 0x0111,
        /// <summary>
        /// The orientation of the camera relative to the scene, when the image was captured. The relation of the '0th row' and '0th column' to visual position is shown as right. 
        /// 1 top left side 
        /// 2 top right side 
        /// 3 bottom right side 
        /// 4 bottom left side 
        /// 5 left side top 
        /// 6 right side top 
        /// 7 right side bottom 
        /// 8 left side bottom 
        /// unsigned short 1 
        /// </summary>		
        Orientation = 0x0112,
        /// <summary>
        ///  When image format is no compression, this value shows the number of components stored for each pixel. At color image, this value is '3'. 
        ///  unsigned short 1  
        /// </summary>
        Samples_Per_Pixel = 0x0115,
        /// <summary>
        /// When image format is no compression and image has stored as strip, this value shows how many rows stored to each strip. If image has not striped, this value is the same as ImageLength(0x0101). 
        /// unsigned short/long 1  
        /// </summary>
        Rows_Per_Strip = 0x0116,
        /// <summary>
        /// When image format is no compression and stored as strip, this value shows how many bytes used for each strip and this value is plural. If image has not stripped, this value is single and means whole stringData size of image. 
        /// unsigned short/long 
        /// </summary>
        Strip_Byte_Counts = 0x0117,
        /// <summary>
        ///  When image format is no compression YCbCr, this value shows byte aligns of YCbCr stringData. If value is '1', Y/Cb/Cr value is chunky format, contiguous for each subsampling pixel. If value is '2', Y/Cb/Cr value is separated and stored to Y plane/Cb plane/Cr plane format. 
        ///  unsigned short 1  
        /// </summary>
        Planar_Configuration = 0x011c,
        /// <summary>
        /// When image format is JPEG, this value show offset to JPEG stringData stored. 
        /// unsigned long 1  
        /// </summary>
        Jpeg_IF_Offset = 0x0201,
        /// <summary>
        ///   When image format is JPEG, this value shows stringData size of JPEG image. 
        ///   unsigned long 1  
        /// </summary>
        Jpeg_IF_Byte_Count = 0x0202,
        /// <summary>
        ///  When image format is YCbCr and uses subsampling(cropping of chroma stringData, all the digicam do that), this value shows how many chroma stringData subsampled. First value shows horizontal, next value shows vertical subsample rate. 
        ///  unsigned short 2  
        /// </summary>
        YCbCrSubSampling = 0x0212,

        /// <summary>
        /// Display/Print resolution of image. Default value is 1/72inch, but it has no mean because personal computer doesn't use this value to display/print out. 
        /// unsigned rational 1  
        /// </summary>
        X_Resolution = 0x011a,
        /// <summary>
        /// Display/Print resolution of image. Default value is 1/72inch, but it has no mean because personal computer doesn't use this value to display/print out. 
        /// unsigned rational 1  
        /// </summary>
        Y_Resolution = 0x011b,
        /// <summary>
        /// Unit of XResolution(0x011a)/YResolution(0x011b). '1' means no-unit, '2' means inch, '3' means centimeter. Default value is '2'(inch). 
        /// unsigned short 1  
        /// </summary>
        Resolution_Unit = 0x0128,
        /// <summary>
        /// Shows firmware(internal software of digicam) version number. 
        /// ascii string 
        /// </summary>
        Software = 0x0131,
        /// <summary>
        /// Date/Time of image was last modified. Data format is "YYYY:MM:DD HH:MM:SS"+0x00, total 20bytes. If clock has not set or digicam doesn't have clock, the field may be filled with spaces. In usual, it has the same value of DateTimeOriginal(0x9003) 
        /// ascii string 20  
        /// </summary>
        DateTime = 0x0132,
        /// <summary>
        /// Defines chromaticity of white point of the image. If the image uses CIE Standard Illumination D65(known as international standard of 'daylight'), the values are '3127/10000,3290/10000'. 
        /// unsigned rational 2  
        /// </summary>
        White_Point = 0x013e,
        /// <summary>
        /// Defines chromaticity of the primaries of the image. If the image uses CCIR Recommendation 709 primaries, values are '640/1000,330/1000,300/1000,600/1000,150/1000,0/1000'. 
        /// unsigned rational 6  
        /// </summary>
        Primary_Chromaticities = 0x013f,
        /// <summary>
        ///  When image format is YCbCr, this value shows a constant to translate it to RGB format. In usual, values are '0.299/0.587/0.114'. 
        ///  unsigned rational 3  
        /// </summary>
        YCbCr_Coefficients = 0x0211,
        /// <summary>
        /// When image format is YCbCr and uses 'Subsampling'(cropping of chroma stringData, all the digicam do that), defines the chroma sample point of subsampling pixel array. '1' means the center of pixel array, '2' means the datum point. 
        /// unsigned short 1  
        /// </summary>
        YCbCr_Positioning = 0x0213,
        /// <summary>
        ///  Shows reference value of black point/white point. In case of YCbCr format, first 2 show black/white of Y, next 2 are Cb, last 2 are Cr. In case of RGB format, first 2 show black/white of R, next 2 are G, last 2 are B. 
        ///  unsigned rational 6  
        /// </summary>
        Reference_Black_White = 0x0214,
        Rating = 0x04746,
        Microsoft_Custom = 0x4748,
        Rating_Per_Cent = 0x4749,
        /// <summary>
        ///	Shows copyright information 
        ///ascii string 
        /// </summary>
        Copyright = 0x8298,
        /// <summary>
        ///  unsigned long 1  Offset to Exif Sub IFD 
        /// </summary>
        Exif_IFD_Pointer = 0x8769,
        /// <summary>
        ///  Exposure time (reciprocal of shutter speed). Unit is second. 
        ///  unsigned rational 1  
        /// </summary>
        Exposure_Time = 0x829a,
        /// <summary>
        /// The actual F-number(F-stop) of lens when the image was taken. 
        /// unsigned rational 1  
        /// </summary>
        F_Number = 0x829d,
        /// <summary>
        ///  Exposure program that the camera used when image was taken. '1' means manual control, '2' program normal, '3' aperture priority, '4' shutter priority, '5' program creative (slow program), '6' program action(high-speed program), '7' portrait mode, '8' landscape mode. 
        ///  unsigned short 1  
        /// </summary>
        Exposure_Program = 0x8822,
        Info_GPS = 0x8825,
        /// <summary>
        /// CCD sensitivity equivalent to Ag-Hr film speedrate. 
        /// unsigned short 2  
        /// </summary>
        ISOSpeedRatings = 0x8827,
        /// <summary>
        /// Exif version number. Stored as 4bytes of ASCII character. If the picture is based on Exif V2.1, value is "0210". Since the mPID is 'undefined', there is no NULL(0x00) for termination. 
        /// undefined 4  
        /// </summary>
        ExifVersion = 0x9000,
        /// <summary>
        /// Date/Time of original image taken. This value should not be modified by user program. Data format is "YYYY:MM:DD HH:MM:SS"+0x00, total 20bytes. If clock has not set or digicam doesn't have clock, the field may be filled with spaces. In the Exif standard, this tag is optional, but it is mandatory for DCF. 
        /// ascii string 20  
        /// </summary>
        DateTimeOriginal = 0x9003,
        /// <summary>
        ///  Date/Time of image digitized. Usually, it contains the same value of DateTimeOriginal(0x9003). Data format is "YYYY:MM:DD HH:MM:SS"+0x00, total 20bytes. If clock has not set or digicam doesn't have clock, the field may be filled with spaces. In the Exif standard, this tag is optional, but it is mandatory for DCF. 
        ///  ascii string 20  
        /// </summary>
        DateTimeDigitized = 0x9004,
        /// <summary>
        /// Shows the order of pixel stringData. Most of case '0x04,0x05,0x06,0x00' is used for RGB-format and '0x01,0x02,0x03,0x00' for YCbCr-format. 0x00:does not exist, 0x01:Y, 0x02:Cb, 0x03:Cr, 0x04:Red, 0x05:Green, 0x06:Bllue. 
        /// undefined 
        /// </summary>
        ComponentsConfiguration = 0x9101,
        /// <summary>
        ///  The average compression ratio of JPEG (rough estimate). 
        ///  unsigned rational 1  
        /// </summary>
        CompressedBitsPerPixel = 0x9102,
        /// <summary>
        /// Shutter speed by APEX value. To convert this value to ordinary 'Shutter Speed'; calculate this value's power of 2, then reciprocal. For example, if the ShutterSpeedValue is '4', shutter speed is 1/(24)=1/16 second. 
        /// signed rational 1  
        /// </summary>
        ShutterSpeedValue = 0x9201,
        /// <summary>
        ///  The actual aperture value of lens when the image was taken. Unit is APEX. To convert this value to ordinary F-number(F-stop), calculate this value's power of root 2 (=1.4142). For example, if the ApertureValue is '5', F-number is Pow(1.4142,5) = F5.6. 
        ///  unsigned rational 1  
        /// </summary>
        ApertureValue = 0x9202,
        /// <summary>
        ///  Brightness of taken subject, unit is APEX. To calculate Exposure(Ev) from BrigtnessValue(Bv), you must add SensitivityValue(Sv).
        ///  	Ev=Bv+Sv   Sv=log2(ISOSpeedRating/3.125)
        ///  	ISO100:Sv=5, ISO200:Sv=6, ISO400:Sv=7, ISO125:Sv=5.32.  
        ///  signed rational 1  
        /// </summary>
        BrightnessValue = 0x9203,
        /// <summary>
        ///  Exposure bias(compensation) value of taking picture. Unit is APEX(EV). 
        ///  signed rational 1  
        /// </summary>
        ExposureBiasValue = 0x9204,
        /// <summary>
        /// Maximum aperture value of lens. You can convert to F-number by calculating power of root 2 (same process of ApertureValue:0x9202). 
        ///unsigned rational 1  
        /// </summary>
        MaxApertureValue = 0x9205,
        /// <summary>
        ///  Distance to focus point, unit is meter. 
        ///  signed rational 1  
        /// </summary>
        SubjectDistance = 0x9206,
        /// <summary>
        ///  Exposure metering method. '0' means unknown, '1' average, '2' center weighted average, '3' spot, '4' multi-spot, '5' multi-JPEGSegment, '6' partial, '255' other. 
        ///  unsigned short 1  
        /// </summary>
        MeteringMode = 0x9207,
        /// <summary>
        /// Light source, actually this means white balance setting. '0' means unknown, '1' daylight, '2' fluorescent, '3' tungsten, '10' flash, '17' standard light A, '18' standard light B, '19' standard light C, '20' D55, '21' D65, '22' D75, '255' other. 
        /// unsigned short 1  
        /// </summary>
        LightSource = 0x9208,
        /// <summary>
        ///  '0' means flash did not fire, '1' flash fired, '5' flash fired but strobe return light not detected, '7' flash fired and strobe return light detected. 
        ///  unsigned short 1  
        /// </summary>
        Flash = 0x9209,
        /// <summary>
        ///  Focal length of lens used to take image. Unit is millimeter. 
        ///  unsigned rational 1  
        /// </summary>
        FocalLength = 0x920a,
        SubjectArea = 0x9214,
        /// <summary>
        /// Maker dependent internal stringData. Some of maker such as Olympus/Nikon/Sanyo etc. uses IFD format for this area. 
        /// undefined
        /// </summary>
        MakerNote = 0x927c,
        /// <summary>
        /// Stores user comment. This tag allows to use two-byte character code or unicode. First 8 bytes describe the character code. 'JIS' is a Japanese character code (known as Kanji).
        ///	'0x41,0x53,0x43,0x49,0x49,0x00,0x00,0x00':ASCII
        ///'0x4a,0x49,0x53,0x00,0x00,0x00,0x00,0x00':JIS
        ///'0x55,0x4e,0x49,0x43,0x4f,0x44,0x45,0x00':Unicode
        ///'0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00':Undefined
        ///  undefined 
        /// </summary>
        UserComment = 0x9286,
        /// <summary>
        ///Some of digicam can take 2~30 pictures per second, but DateTime/DateTimeOriginal/DateTimeDigitized tag can't record the sub-second time. SubsecTime tag is used to record it.
        /// For example, DateTimeOriginal = "1996:09:01 09:15:30", SubSecTimeOriginal = "130", Combined original time is "1996:09:01 09:15:30.130" 
        /// ascii string 
        /// </summary>
        SubsecTime = 0x9290,
        /// <summary>
        /// See SubsecTime
        ///  ascii string 
        /// </summary>
        SubsecTimeOriginal = 0x9291,
        /// <summary>
        /// See SubsecTime
        /// ascii string 
        /// </summary>
        SubsecTimeDigitized = 0x9292,
        XPTitle = 0x9c9b,
        XPComment = 0x9c9c,
        XPAuthor = 0x9c9d,
        XPKeywords = 0x9c9e,
        XPSubject = 0x9c9f,

        /// <summary>
        /// Stores FlashPix version. If the image stringData is based on FlashPix formar Ver.1.0, value is "0100". Since the mPID is 'undefined', there is no NULL(0x00) for termination.  
        /// undefined 4  
        /// </summary>
        FlashPixVersion = 0xa000,
        /// <summary>
        /// Defines Color Space. DCF image must use sRGB color space so value is always '1'. If the picture uses the other color space, value is '65535':Uncalibrated. 
        /// unsigned short 1  
        /// </summary>
        ColorSpace = 0xa001,
        /// <summary>
        ///  Size of main image. 
        ///  unsigned short/long 1  
        /// </summary>
        ExifImageWidth = 0xa002,
        /// <summary>
        /// Size of main image
        ///  unsigned short/long 1  
        /// </summary>
        ExifImageHeight = 0xa003,
        /// <summary>
        /// If this digicam can record audio stringData with image, shows name of audio stringData. 
        /// ascii string 
        /// </summary>
        RelatedSoundFile = 0xa004,
        /// <summary>
        /// Extension of "ExifR98", detail is unknown. This value is offset to IFD format stringData. Currently there are 2 directory entries, first one is Tag0x0001, value is "R98", next is Tag0x0002, value is "0100". 
        /// unsigned long 1  
        /// </summary>
        ExifInteroperabilityOffset = 0xa005,
        /// <summary>
        ///   Pixel density at CCD's position. If you have MegaPixel digicam and take a picture by lower resolution(e.header.VGA mode), this value is re-sampled by picture resolution. In such case, FocalPlaneResolution is not same as CCD's actual resolution. 
        ///   unsigned rational 1  
        /// </summary>
        FocalPlaneXResolution = 0xa20e,
        /// <summary>
        /// See FocalPlaneXResolution
        ///  unsigned rational 1  
        /// </summary>
        FocalPlaneYResolution = 0xa20f,
        /// <summary>
        ///  Unit of FocalPlaneXResoluton/FocalPlaneYResolution. '1' means no-unit, '2' inch, '3' centimeter. 
        ///  Note:Some of Fujifilm's digicam(e.header.FX2700,FX2900,Finepix4700Z/40i etc) uses value '3' so it must be 'centimeter', but it seems that they use a '8.3mm?'(1/3in.?) to their ResolutionUnit. Fuji's BUG? Finepix4900Z has been changed to use value '2' but it doesn't match to actual value also. 
        ///  unsigned short 1  
        /// </summary>
        FocalPlaneResolutionUnit = 0xa210,
        /// <summary>
        ///  Same as ISOSpeedRatings(0x8827) but stringData mPID is unsigned rational. Only Kodak's digicam uses this tag instead of ISOSpeedRating, I don't know why(historical reason?).  
        ///  unsigned rational 1  
        /// </summary>
        ExposureIndex = 0xa215,
        /// <summary>
        ///  Shows mPID of image sensor unit. '2' means 1 chip color area sensor, most of all digicam use this mPID. 
        ///  unsigned short 1  
        /// </summary>
        SensingMethod = 0xa217,
        /// <summary>
        ///  Indicates the image source. Value '0x03' means the image source is digital still camera. 
        ///  undefined 1  
        /// </summary>
        FileSource = 0xa300,
        /// <summary>
        ///  Indicates the mPID of scene. Value '0x01' means that the image was directly photographed. 
        ///  undefined 1
        /// </summary>
        SceneType = 0xa301,
        /// <summary>
        /// Indicates the Color filter array(CFA) geometric pattern.
        /// Length Type Meaning 
        /// 2 short Horizontal repeat pixel unit = n 
        /// 2 short Vertical repeat pixel unit = m 
        /// 1 byte CFA value[0,0] 
        /// 1 byte CFA value[n-1,0] 
        /// 1 byte CFA value[0,1] 
        /// 1 byte CFA value[n-1,m-1] 
        ///
        ///The relation of filter color to CFA value is shown below.
        /// Red = 0 Green = 1 Blue = 2 Cyan = 3 Magenta = 4 Yellow = 5 White = 6 
        /// 
        /// undefined 
        /// </summary>
        CFAPattern = 0xa302,
        CustomRendered = 0xa401,
        ExposureMode = 0xa402,
        White_Balance = 0xa403,
        Digital_Zoom_Ratio = 0xa404,
        FocalLengthIn35mmFormat = 0xa405,
        SceneCaptureType = 0xa406,
        GainControl = 0xa407,
        Contrast = 0xa408,
        Saturation = 0xa409,
        Sharpness = 0xa40a,
        DeviceSettingDescription = 0xa40b,
        SubjectDistanceRange = 0xa40c,
        ImageUniqueID = 0xa420,
        OwnerName = 0xa430,
        SerialNumber = 0xa431,
        LensInfo = 0xa432,
        LensMake = 0xa433,
        LensModel = 0xa434,
        LensSerialNumber = 0xa435,

        PrintIM = 0xc4a5,
        Padding = 0xea1c,
        OffsetSchema = 0xea1d,
        /// <summary>
        /// Records the file format of image file. Value is ascii string (e.header. "Exif JPEG Ver. 2.1"). 
        /// Ascii string  any
        /// </summary>
        RelatedImageFileFormat = 0x1000,
        /// <summary>
        ///  Records the image size. 
        ///  Short or Long  1  
        /// </summary>
        RelatedImageWidth = 0x1001,
        /// <summary>
        /// Records the image size
        /// Short or Long  1  
        /// </summary>
        RelatedImageLength = 0x1001,
        #endregion
    }
    public enum JPEGMarker : byte
    {
        // BlockLength Of Frame markers, non-differential, Huffman coding
        SOF0 = 0xc0,
        SOF1 = 0xc1,
        SOF2 = 0xc2,
        SOF3 = 0xc3,
        // BlockLength Of Frame markers, differential, Huffman coding
        SOF5 = 0xc5,
        SOF6 = 0xc6,
        SOF7 = 0xc7,
        // BlockLength Of Frame markers, non-differential, arithmetic coding
        JPG = 0xc8,
        SOF9 = 0xc9,
        SOF10 = 0xca,
        SOF11 = 0xcb,
        // BlockLength Of Frame markers, differential, arithmetic coding
        SOF13 = 0xcd,
        SOF14 = 0xce,
        SOF15 = 0xcf,
        // Huffman table specification
        DHT = 0xc4,
        // Arithmetic coding conditioning specification
        DAC = 0xcc,
        // Restart interval termination
        RST0 = 0xd0,
        RST1 = 0xd1,
        RST2 = 0xd2,
        RST3 = 0xd3,
        RST4 = 0xd4,
        RST5 = 0xd5,
        RST6 = 0xd6,
        RST7 = 0xd7,
        // Other markers
        SOI = 0xd8,
        EOI = 0xd9,
        SOS = 0xda,//Start of stream until ffd9 EOI
        DQT = 0xdb,
        DNL = 0xdc,
        DRI = 0xdd,
        DHP = 0xde,
        EXP = 0xdf,
        // application segments
        APP0 = 0xe0,
        APP1 = 0xe1,
        APP2 = 0xe2,
        APP3 = 0xe3,
        APP4 = 0xe4,
        APP5 = 0xe5,
        APP6 = 0xe6,
        APP7 = 0xe7,
        APP8 = 0xe8,
        APP9 = 0xe9,
        APP10 = 0xea,
        APP11 = 0xeb,
        APP12 = 0xec,
        APP13 = 0xed,
        APP14 = 0xee,
        APP15 = 0xef,
        // JPEG extensions
        JPG0 = 0xf0,
        JPG1 = 0xf1,
        JPG2 = 0xf2,
        JPG3 = 0xf3,
        JPG4 = 0xf4,
        JPG5 = 0xf5,
        JPG6 = 0xf6,
        JPG7 = 0xf7,
        JPG8 = 0xf8,
        JPG9 = 0xf9,
        JPG10 = 0xfa,
        JPG11 = 0xfb,
        JP1G2 = 0xfc,
        JPG13 = 0xfd,
        // Comment
        COM = 0xfe,
        // Temporary
        TEM = 0x01,
    }
    public enum EXIFMPF
    {
        MPFVersion = 0xb000,
        NumberOfImages = 0xb001,
        MPImageList = 0xb002,//	--> MPF MPImage Tags
        ImageUIDList = 0xb003,
        TotalFrames = 0xb004,
        MPIndividualNum = 0xb101,
        PanOrientation = 0xb201,//	(long integer is split into 4 bytes),[Value 2] ,0x0 = [unused] ,0x1 = Start at top right ,0x2 = Start at top left ,0x3 = Start at bottom left ,0x4 = Start at bottom right ,[Value 3] ,0x1 = Left to right ,0x2 = Right to left ,0x3 = Top to bottom ,0x4 = Bottom to top ,0x10 = Clockwise ,0x20 = Counter clockwise ,0x30 = Zigzag (row start) ,0x40 = Zigzag (column start)
        PanOverlapH = 0xb202,
        PanOverlapV = 0xb203,
        BaseViewpointNum = 0xb204,
        ConvergenceAngle = 0xb205,
        BaselineLength = 0xb206,
        VerticalDivergence = 0xb207,
        AxisDistanceX = 0xb208,
        AxisDistanceY = 0xb209,
        AxisDistanceZ = 0xb20a,
        YawAngle = 0xb20b,
        PitchAngle = 0xb20c,
        RollAngle = 0xb20d,
    }
    public enum MPImageType { Undefined = 0x0, Large_Thumbnail_VGA_equivalent = 0x10001, Large_Thumbnail_full_HD_equivalent = 0x10002, Multi_frame_Panorama = 0x20001, Multi_frame_Disparity = 0x20002, Multi_angle = 0x20003, Baseline_MP_Primary_Image = 0x30000 }
    public enum FlashPixObjects { Dictionnary = 0x0, Code_Page = 0x1, Category = 0x0002, PresentationTarget = 0x0003, Bytes = 0x0004, Lines = 0x0005, Paragraphs = 0x0006, Slides = 0x0007, Notes = 0x0008, HiddenSlides = 0x0009, MMClips = 0x000a, ScaleCrop = 0x000b, HeadingPairs = 0x000c, TitleOfParts = 0x000d, Manager = 0x000e, Company = 0x000f, LinksUpToDate = 0x0010, CharCountWithSpaces = 0x0011, SharedDoc = 0x0013, HyperlinksChanged = 0x0016, AppVersion = 0x0017, DataObjectID = 0x10000, OperationClassID = 0x10001, LockedPropertyList = 0x10002, DataObjectTitle = 0x10003, LastModifier = 0x10004, RevisionNumber = 0x10005, DataCreateDate = 0x10006, DataModifyDate = 0x10007, CreatingApplication = 0x10008, DataObjectStatus = 0x10100, CreatingTransform = 0x10101, UsingTransforms = 0x10102, CachedImageHeight = 0x10000000, CachedImageWidth = 0x10000001 }
    public enum OLE_Types
    {
        VT_EMPTY = 0,//VT_EMPTY  
        VT_NULL = 1,//VT_NULL  
        VT_I2 = 2,//VT_I2  
        VT_I4 = 3,//VT_I4  
        VT_R4 = 4,//VT_R4  
        VT_R8 = 5,//VT_R8  
        VT_CY = 6,//VT_CY  
        VT_DATE = 7,//VT_DATE(double, //numberofdayssinceDec30, //1899)
        VT_BSTR = 8,//VT_BSTR(int32ucount, //followedbybinarystring) 
        VT_DISPATCH = 9,//  
        VT_ERROR = 10,//VT_ERROR  
        VT_BOOL = 11,//VT_BOOL  
        VT_VARIANT = 12,//VT_VARIANT  
        VT_UNKNOWN = 13,//  
        VT_DECIMAL = 14,//  
        VT_I1 = 16,//VT_I1  
        VT_UI1 = 17,//VT_UI1  
        VT_UI2 = 18,//VT_UI2  
        VT_UI4 = 19,//VT_UI4  
        VT_I8 = 20,//VT_I8  
        VT_UI8 = 21,//VT_UI8  
        VT_INT = 22,//  
        VT_UINT = 23,//  
        VT_VOID = 24,//  
        VT_HRESULT = 25,//  
        VT_PTR = 26,//  
        VT_SAFEARRAY = 27,//  
        VT_CARRAY = 28,//  
        VT_USERDEFINED = 29,//  
        VT_LPSTR = 30,//VT_LPSTR(int32ucount, //followedbystring) 
        VT_LPWSTR = 31,//VT_LPWSTR(int32uwordcount, //followedbyUnicodestring) 
        VT_FILETIME = 64,//VT_FILETIME(int64u, //numberofnanosecondssinceJan1, //1601)
        VT_BLOB = 65,//VT_BLOB  
        VT_STREAM = 66,//  
        VT_STORAGE = 67,//  
        VT_STREAMED_OBJECT = 68,//  
        VT_STORED_OBJECT = 69,//  
        VT_BLOB_OBJECT = 70,//  
        VT_CF = 71,//VT_CF  
        VT_CLSID = 72,//VT_CLSID  
        VT_VECTOR = 0x1000,
        VT_ARRAY = 0x2000
    }
    public enum CodePage
    {
        IBM_EBCDIC_US_Canada = 31,
        DOS_United_States = 437,
        IBM_EBCDIC_International = 500,
        Arabic_ASMO_708 = 708,
        Arabic_ASMO_449_BCON_V4 = 709,
        Arabic__Transparent_Arabic = 710,
        DOS_Arabic_Transparent_ASMO = 720,
        DOS_Greek_formerly_437G = 737,
        DOS_Baltic = 775,
        DOS_Latin_1_Western_European = 850,
        DOS_Latin_2_Central_European = 852,
        DOS_Cyrillic_primarily_Russian = 855,
        DOS_Turkish = 857,
        DOS_Multilingual_Latin_1_with_Euro = 858,
        DOS_Portuguese = 860,
        DOS_Icelandic = 861,
        DOS_Hebrew = 862,
        DOS_French_Canadian = 863,
        DOS_Arabic = 864,
        DOS_Nordic = 865,
        DOS_Russian_Cyrillic = 866,
        DOS_Modern_Greek = 869,
        IBM_EBCDIC_Multilingual_ROECE_Latin_2 = 870,
        Windows_Thai_same_as_28605_ISO_8859_15 = 874,
        IBM_EBCDIC_Greek_Modern = 875,
        Windows_Japanese_Shift_JIS = 932,
        Windows_Simplified_Chinese_PRC_Singapore = 936,
        Windows_Korean_Unified_Hangul_Code = 949,
        Windows_Traditional_Chinese_Taiwan = 950,
        IBM_EBCDIC_Turkish_Latin_5 = 1026,
        IBM_EBCDIC_Latin_1_Open_System = 1047,
        IBM_EBCDIC_US_Canada_with_Euro = 1140,
        IBM_EBCDIC_Germany_with_Euro = 1141,
        IBM_EBCDIC_Denmark_Norway_with_Euro = 1142,
        IBM_EBCDIC_Finland_Sweden_with_Euro = 1143,
        IBM_EBCDIC_Italy_with_Euro = 1144,
        IBM_EBCDIC_Latin_America_Spain_with_Euro = 1145,
        IBM_EBCDIC_United_Kingdom_with_Euro = 1146,
        IBM_EBCDIC_France_with_Euro = 1147,
        IBM_EBCDIC_International_with_Euro = 1148,
        IBM_EBCDIC_Icelandic_with_Euro = 1149,
        Unicode_UTF_16_little_endian = 1200,
        Unicode_UTF_16_big_endian = 1201,
        Windows_Latin_2_Central_European = 1250,
        Windows_Cyrillic = 1251,
        Windows_Latin_1_Western_European = 1252,
        Windows_Greek = 1253,
        Windows_Turkish = 1254,
        Windows_Hebrew = 1255,
        Windows_Arabic = 1256,
        Windows_Baltic = 1257,
        Windows_Vietnamese = 1258,
        Korean_Johab = 1361,
        Mac_Roman_Western_European = 10000,
        Mac_Japanese = 10001,
        Mac_Traditional_Chinese = 10002,
        Mac_Korean = 10003,
        Mac_Arabic = 10004,
        Mac_Hebrew = 10005,
        Mac_Greek = 10006,
        Mac_Cyrillic = 10007,
        Mac_Simplified_Chinese = 10008,
        Mac_Romanian = 10010,
        Mac_Ukrainian = 10017,
        Mac_Thai = 10021,
        Mac_Latin_2_Central_European = 10029,
        Mac_Icelandic = 10079,
        Mac_Turkish = 10081,
        Mac_Croatian = 10082,
        Unicode_UTF_32_little_endian = 12000,
        Unicode_UTF_32_big_endian = 12001,
        CNS_Taiwan = 20000,
        TCA_Taiwan = 20001,
        Eten_Taiwan = 20002,
        IBM5550_Taiwan = 20003,
        TeleText_Taiwan = 20004,
        Wang_Taiwan = 20005,
        IA5_IRV_International_Alphabet_No_5_7_bit = 20105,
        IA5_German_7_bit = 20106,
        IA5_Swedish_7_bit = 20107,
        IA5_Norwegian_7_bit = 20108,
        US_ASCII_7_bit = 20127,
        T_61 = 20261,
        ISO_6937_Non_Spacing_Accent = 20269,
        IBM_EBCDIC_Germany = 20273,
        IBM_EBCDIC_Denmark_Norway = 20277,
        IBM_EBCDIC_Finland_Sweden = 20278,
        IBM_EBCDIC_Italy = 20280,
        IBM_EBCDIC_Latin_America_Spain = 20284,
        IBM_EBCDIC_United_Kingdom = 20285,
        IBM_EBCDIC_Japanese_Katakana_Extended = 20290,
        IBM_EBCDIC_France = 20297,
        IBM_EBCDIC_Arabic = 20420,
        IBM_EBCDIC_Greek = 20423,
        IBM_EBCDIC_Hebrew = 20424,
        IBM_EBCDIC_Korean_Extended = 20833,
        IBM_EBCDIC_Thai = 20838,
        Russian_Cyrillic_KOI8_R = 20866,
        IBM_EBCDIC_Icelandic = 20871,
        IBM_EBCDIC_Cyrillic_Russian = 20880,
        IBM_EBCDIC_Turkish = 20905,
        IBM_EBCDIC_Latin_1_Open_System_with_Euro = 20924,
        Japanese_JIS_0208_1990_and_0121_1990 = 20932,
        Simplified_Chinese_GB2312 = 20936,
        Korean_Wansung = 20949,
        IBM_EBCDIC_Cyrillic_Serbian_Bulgarian = 21025,
        Extended_Alpha_Lowercase_deprecated = 21027,
        Ukrainian_Cyrillic_KOI8_U = 21866,
        ISO_8859_1_Latin_1_Western_European = 28591,
        ISO_8859_2_Central_European = 28592,
        ISO_8859_3_Latin_3 = 28593,
        ISO_8859_4_Baltic = 28594,
        ISO_8859_5_Cyrillic = 28595,
        ISO_8859_6_Arabic = 28596,
        ISO_8859_7_Greek = 28597,
        ISO_8859_8_Hebrew_Visual = 28598,
        ISO_8859_9_Turkish = 28599,
        ISO_8859_13_Estonian = 28603,
        ISO_8859_15_Latin_9 = 28605,
        Europa_3 = 29001,
        ISO_8859_8_Hebrew_Logical = 38598,
        ISO_2022_Japanese_with_no_halfwidth_Katakana_JIS = 50220,
        ISO_2022_Japanese_with_halfwidth_Katakana_JIS_Allow_1_byte_Kana = 50221,
        ISO_2022_Japanese_JIS_X_0201_1989_JIS_Allow_1_byte_Kana___SO_SI = 50222,
        ISO_2022_Korean = 50225,
        ISO_2022_Simplified_Chinese = 50227,
        ISO_2022_Traditional_Chinese = 50229,
        EBCDIC_Japanese_Katakana_Extended = 50930,
        EBCDIC_US_Canada_and_Japanese = 50931,
        EBCDIC_Korean_Extended_and_Korean = 50933,
        EBCDIC_Simplified_Chinese_Extended_and_Simplified_Chinese = 50935,
        EBCDIC_Simplified_Chinese = 50936,
        EBCDIC_US_Canada_and_Traditional_Chinese = 50937,
        EBCDIC_Japanese_Latin_Extended_and_Japanese = 50939,
        EUC_Japanese = 51932,
        EUC_Simplified_Chinese = 51936,
        EUC_Korean = 51949,
        EUC_Traditional_Chinese = 51950,
        HZ_GB2312_Simplified_Chinese = 52936,
        Windows_XP_and_later_GB18030_Simplified_Chinese_4_byte = 54936,
        ISCII_Devanagari = 57002,
        ISCII_Bengali = 57003,
        ISCII_Tamil = 57004,
        ISCII_Telugu = 57005,
        ISCII_Assamese = 57006,
        ISCII_Oriya = 57007,
        ISCII_Kannada = 57008,
        ISCII_Malayalam = 57009,
        ISCII_Gujarati = 57010,
        ISCII_Punjabi = 57011,
        Unicode_UTF_7 = 65000,
        Unicode_UTF_8 = 65001,
    }
    #endregion
}
