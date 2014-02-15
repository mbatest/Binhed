using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BluRay
{
    enum BD_TITLE_TYPE
    {
        title_undef = 0,
        title_hdmv,
        title_bdj,
    }
    enum bd_still_mode
    {
        BLURAY_STILL_NONE = 0x00,
        BLURAY_STILL_TIME = 0x01,
        BLURAY_STILL_INFINITE = 0x02,
    }
    struct BLURAY_STREAM_INFO
    {
        byte coding_type;
        byte format;
        byte rate;
        byte char_code;
        byte[] lang;
        short pid;
        byte aspect;
    }
    struct BLURAY_CLIP_INFO
    {
        int pkt_count;
        byte still_mode;
        short still_time;  // seconds 
        byte video_stream_count;
        byte audio_stream_count;
        byte pg_stream_count;
        byte ig_stream_count;
        byte sec_audio_stream_count;
        byte sec_video_stream_count;
        BLURAY_STREAM_INFO video_streams;
        BLURAY_STREAM_INFO audio_streams;
        BLURAY_STREAM_INFO pg_streams;
        BLURAY_STREAM_INFO ig_streams;
        BLURAY_STREAM_INFO sec_audio_streams;
        BLURAY_STREAM_INFO sec_video_streams;
    }
    struct BLURAY_TITLE_CHAPTER
    {
        int idx;
        long start;
        long duration;
        long offset;
    }
    struct BLURAY_TITLE_INFO
    {
        int idx;
        int playlist;
        long duration;
        int clip_count;
        byte angle_count;
        int chapter_count;
        BLURAY_CLIP_INFO clips;
        BLURAY_TITLE_CHAPTER chapters;
    }
    struct NAV_CLIP
    {
        string name;
        int clip_id;
        uint refer;
        int pos;
        int start_pkt;
        int end_pkt;
        byte connection;
        byte angle;

        int start_time;
        int duration;

        int in_time;
        int out_time;

        // Title relative metrics
        int title_pkt;
        int title_time;

        List<NAV_TITLE> title;

        List<CLPI> cl;
    }
    struct NAV_SUB_PATH
    {
        byte type;
        List<NAV_CLIP> clip_list;
    }
    struct NAV_TITLE
    {
        string  root;
        string name;
        byte angle_count;
        byte angle;
        List<NAV_CLIP> clip_list;
        List<NAV_MARK> chap_list;
        List<NAV_MARK> mark_list;

        uint sub_path_count;
        NAV_SUB_PATH sub_path;

        int packets;
        int duration;

        MPLS pl;
    }
    struct NAV_MARK
    {
        int number;
        int mark_type;
        uint clip_ref;
        int clip_pkt;
        int clip_time;

        // Title relative metrics
        int title_pkt;
        int title_time;
        int duration;

        MPLS_PLM plm;
    }
    struct BD_STREAM
    {
        // current clip 
        NAV_CLIP clip;
        FileStream fp;
        long clip_size;
        long clip_block_pos;
        long clip_pos;

        // current aligned unit 
        short int_buf_off;

        BD_UO_MASK uo_mask;

    }
    struct BD_PRELOAD
    {
        NAV_CLIP clip;
        long clip_size;
        byte[] buf;
    }
    struct BLURAY_DISC_INFO
    {
        byte bluray_detected;

        byte first_play_supported;
        byte top_menu_supported;

        int num_hdmv_titles;
        int num_bdj_titles;
        int num_unsupported_titles;

        byte aacs_detected;
        byte libaacs_detected;
        byte aacs_handled;

        byte bdplus_detected;
        byte libbdplus_detected;
        byte bdplus_handled;

    }
    struct META_THUMBNAIL
    {
        string path;
        int xres;
        int yres;
    }
    struct META_TITLE
    {
        int title_number;
        string title_name;
    }
    struct META_DL
    {
        string language_code;
        string filename;
        string di_name;
        string di_alternative;
        byte di_num_sets;
        byte di_set_number;
        int toc_count;
        List<META_TITLE> toc_entries;
        byte thumb_count;
        List<META_THUMBNAIL> thumbnails;
    }
    struct META_ROOT
    {
        byte dl_count;
        List<META_DL> dl_entries;
    }
    struct PSR_CB_DATA
    {
        //  void *handle;
        //  void (*cb)(void *, BD_PSR_EVENT*);
    }
    struct BD_MUTEX
    {
        int lock_count;
        //  pthread_t       owner;
        //  pthread_mutex_t mutex;
    };
    struct BD_REGISTERS
    {
        int[] psr;//[BD_PSR_COUNT];
        int[] gpr;//[BD_GPR_COUNT];

        /* callbacks */
        uint num_cb;
        PSR_CB_DATA cb;

        BD_MUTEX mutex;
    }
    struct HDMV_VM
    {

        BD_MUTEX mutex;

        /* state */
        int pc;            /* program counter */
        List<BD_REGISTERS> regs;          /* player registers */
        MOBJ_OBJECT obj;        /* currently running object code */

        //    HDMV_EVENT []    evnt;      /* pending events to return */

        //    NV_TIMER       nv_timer;      /* navigation timer */

        /* movie objects */
        List<MOBJ_OBJECT> movie_objects; /* disc movie objects */
        MOBJ_OBJECT ig_object;     /* current object from IG stream */

        /* object currently playing playlist */
        MOBJ_OBJECT playing_object;
        int playing_pc;

        /* suspended object */
        MOBJ_OBJECT suspended_object;
        int suspended_pc;

        /* disc index (used to verify CALL_TITLE/JUMP_TITLE) */
        INDEX_BDMV indx;
    }
    struct bluray
    {

        // current disc 
        string device_path;
        BLURAY_DISC_INFO disc_info;
        INDEX_BDMV index;
        META_ROOT meta;
        List<NAV_TITLE> title_list;

        // current playlist 
        NAV_TITLE title;
        int title_idx;
        long s_pos;

        // streams 
        BD_STREAM st0; // main path 
        BD_PRELOAD st_ig; // preloaded IG stream sub path 

        // buffer for bd_read(): current aligned unit of main stream (st0) 
        byte[] int_buf;//[6144];

        // seamless angle change request 
        int seamless_angle_change;
        int angle_change_pkt;
        int angle_change_time;
        uint request_angle;

        // chapter tracking 
        long next_chapter_start;

        // aacs 
   //     void aacs;

        // BD+ 

     //   void bdplus;

        // player state 
        BD_REGISTERS regs;       // player registers
        //    BD_EVENT_QUEUE event_queue; // navigation mode event queue
        BD_TITLE_TYPE title_type;  // type of current title (in navigation mode)

        HDMV_VM hdmv_vm;
        byte hdmv_suspended;

        //     void bdjava;

        // graphics 
        //   GRAPHICS_CONTROLLER* graphics_controller;
    }
}
