using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// http://git.videolan.org/?p=libbluray.git;a=blob;f=src/libbluray/bdnav/clpi_data.h;h=1cf8b95ae74989be0ef2dd1b06a4903d1c6978ed;hb=HEAD
namespace BluRay
{
    enum bd_stream_type_e
    {
        BLURAY_STREAM_TYPE_VIDEO_MPEG1 = 0x01,
        BLURAY_STREAM_TYPE_VIDEO_MPEG2 = 0x02,
        BLURAY_STREAM_TYPE_AUDIO_MPEG1 = 0x03,
        BLURAY_STREAM_TYPE_AUDIO_MPEG2 = 0x04,
        BLURAY_STREAM_TYPE_AUDIO_LPCM = 0x80,
        BLURAY_STREAM_TYPE_AUDIO_AC3 = 0x81,
        BLURAY_STREAM_TYPE_AUDIO_DTS = 0x82,
        BLURAY_STREAM_TYPE_AUDIO_TRUHD = 0x83,
        BLURAY_STREAM_TYPE_AUDIO_AC3PLUS = 0x84,
        BLURAY_STREAM_TYPE_AUDIO_DTSHD = 0x85,
        BLURAY_STREAM_TYPE_AUDIO_DTSHD_MASTER = 0x86,
        BLURAY_STREAM_TYPE_VIDEO_VC1 = 0xea,
        BLURAY_STREAM_TYPE_VIDEO_H264 = 0x1b,
        BLURAY_STREAM_TYPE_SUB_PG = 0x90,
        BLURAY_STREAM_TYPE_SUB_IG = 0x91,
        BLURAY_STREAM_TYPE_SUB_TEXT = 0x92,
        BLURAY_STREAM_TYPE_AUDIO_AC3PLUS_SECONDARY = 0xa1,
        BLURAY_STREAM_TYPE_AUDIO_DTSHD_SECONDARY = 0xa2
    }
    enum bd_video_format_e
    {
        BLURAY_VIDEO_FORMAT_480I = 1,  // ITU-R BT.601-5
        BLURAY_VIDEO_FORMAT_576I = 2,  // ITU-R BT.601-4
        BLURAY_VIDEO_FORMAT_480P = 3,  // SMPTE 293M
        BLURAY_VIDEO_FORMAT_1080I = 4,  // SMPTE 274M
        BLURAY_VIDEO_FORMAT_720P = 5,  // SMPTE 296M
        BLURAY_VIDEO_FORMAT_1080P = 6,  // SMPTE 274M
        BLURAY_VIDEO_FORMAT_576P = 7   // ITU-R BT.1358
    }
    enum bd_video_rate_e
    {
        BLURAY_VIDEO_RATE_24000_1001 = 1,  // 23.976
        BLURAY_VIDEO_RATE_24 = 2,
        BLURAY_VIDEO_RATE_25 = 3,
        BLURAY_VIDEO_RATE_30000_1001 = 4,  // 29.97
        BLURAY_VIDEO_RATE_50 = 6,
        BLURAY_VIDEO_RATE_60000_1001 = 7   // 59.94
    }
    enum bd_video_aspect_e
    {
        BLURAY_ASPECT_RATIO_4_3 = 2,
        BLURAY_ASPECT_RATIO_16_9 = 3
    }
    enum bd_audio_format_e
    {
        BLURAY_AUDIO_FORMAT_MONO = 1,
        BLURAY_AUDIO_FORMAT_STEREO = 3,
        BLURAY_AUDIO_FORMAT_MULTI_CHAN = 6,
        BLURAY_AUDIO_FORMAT_COMBO = 12  // Stereo ac3/dts, 
    }
    enum bd_audio_rate_e
    {
        BLURAY_AUDIO_RATE_48 = 1,
        BLURAY_AUDIO_RATE_96 = 4,
        BLURAY_AUDIO_RATE_192 = 5,
        BLURAY_AUDIO_RATE_192_COMBO = 12, // 48 or 96 ac3/dts,   192 mpl/dts-hd
        BLURAY_AUDIO_RATE_96_COMBO = 14  // 48 ac3/dts   96 mpl/dts-hd
    }
    enum bd_char_code_e
    {
        BLURAY_TEXT_CHAR_CODE_UTF8 = 0x01,
        BLURAY_TEXT_CHAR_CODE_UTF16BE = 0x02,
        BLURAY_TEXT_CHAR_CODE_SHIFT_JIS = 0x03,
        BLURAY_TEXT_CHAR_CODE_EUC_KR = 0x04,
        BLURAY_TEXT_CHAR_CODE_GB18030_20001 = 0x05,
        BLURAY_TEXT_CHAR_CODE_CN_GB = 0x06,
        BLURAY_TEXT_CHAR_CODE_BIG5 = 0x07
    }
    enum bd_still_mode_e
    {
        BLURAY_STILL_NONE = 0x00,
        BLURAY_STILL_TIME = 0x01,
        BLURAY_STILL_INFINITE = 0x02
    }
}
