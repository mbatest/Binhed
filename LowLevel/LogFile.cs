using System;
using System.Collections.Generic;
using System.Text;

namespace LowLevel
{
    //http://stderr.org/doc/ntfsdoc/files/
    class NTFS_RECORD { }
    struct RESTART_PAGE_HEADER
    {
        NTFS_RECORD ntfs;        //        The magic is "RSTR".
        ulong chkdsk_lsn;  //      The check disk log file sequence number for  this restart page. Only used when the  magic is changed to "CHKD". = 0
        uint system_page_size;   //     Byte length of system pages, has to be >= 512  and a power of 2. Use this to calculate the  required length of the usa and add this to the ntfs.usa_offset value. Then verify that the  result is less than the value of the    restart_offset. = 0x1000
        uint log_page_size; //       Byte length of log file records, has to be   >= 512 and a power of 2. = 0x1000
        ushort restart_offset;//        Byte Offset from the start of the record to the restart record. Value has to be aligned  to 8-byte boundary. = 0x30
        short minor_ver; //       Log file minor version. Only check if major version is 1. (=1 but >=1 is treated the same and <=0 is also ok)
        ushort major_ver; //       Log file major version (=1 but =0 is ok)
    }

    struct RESTART_AREA
    {
        ulong current_lsn; //       Log file record. = 0x700000, 0x700808
        ushort log_clients;   //     Number of log client records following the restart_area. = 1
        ushort client_free_list; /*       How many clients are free(?). If != 0xffff,
                                       check that log_clients > client_free_list.   = 0xffff*/
        ushort client_in_use_list;/*How many clients are in use(?). If != 0xffff
                                       check that log_clients > client_in_use_list.       = 0*/
        ushort flags;             //   ??? = 0
        uint seq_number_bits;     //   ??? = 0x2c or 0x2d
        ushort restart_area_length; /*Length of the restart area. Following
                                       checks required if version matches.
                                       Otherwise, skip them. restart_offset +
                                       restart_area_length has to be <lt;=
                                       system_page_size. Also, restart_area_length
                                       has to be >= client_array_offset +
                                       (log_clients * 0xa0). = 0xd0*/
        ushort client_array_offset; /*Offset from the start of this record to
                                       the first client record if versions are
                                       matched. The Offset is otherwise assumed to
                                       be (sizeof(RESTART_AREA) + 7) & ~7, i.e.
                                       rounded up to first 8-byte boundary. Either
                                       way, the Offset to the client array has to be
                                       aligned to an 8-byte boundary. Also,
                                       restart_offset + Offset to the client array
                                       have to be <lt;= 510. Also, the Offset to the
                                       client array + (log_clients * 0xa0) have to
                                       be <lt;= SystemPageSize. = 0x30*/
        ulong file_size;  /*      Byte length of the log file. If the
                                       restart_offset + the Offset of the file_size
                                       are > 510 then corruption has occured. This
                                       is the very first check when starting with
                                       the restart_area as if it fails it means
                                       that some of the above values will be
                                       corrupted by the multi Sector transfer
                                       protection! If the structure is deprotected
                                       then these checks are futile of course.
                                       Calculate the file_size bits and check that
                                       seq_number_bits == 0x43 - file_size bits.
                                       = 0x400000*/
        uint last_lsn_data_length;//??? = 0, 0x40
        ushort record_length;  /*      Byte length of this record. If the version
                                       matches then check that the value of
                                       record_length is a multiple of 8, i.e.
                                       (record_length + 7) & ~7 == record_length.
                                       = 0x30*/
        ushort log_page_data_offset;//??? = 0x40
    }

    //   Log file client record. Starts at 0x58 even though AFAIU the above it should
    //   start at 0x60. Something fishy is going on. /-:

    struct RESTART_CLIENT
    {
        ulong oldest_lsn;  //      Oldest log file sequence number for this   client record. = 0xbd16951d
        ulong client_restart_lsn;//??? = 0x700000, 0x700827, 0x700d07
        ushort prev_client;   //     ??? = 0x808, 0xd07, 0xd5d
        ushort next_client;  //      ??? = 0x70
        ushort seq_number;    //    ??? = 0, 4 length uncertain, Regis calls this  "volume clear flag" and gives a length of one  byte.
        ushort client_name;   //    ??? = empty string??? length uncertain
    }
    struct RECORD_PAGE_HEADER
    {
        NTFS_RECORD ntfs;     //                   The magic is "RCRD".
        // union copy{
        ulong last_lsn;
        uint file_offset;
        // }  
        uint flags;
        ushort page_count;
        ushort page_position;
        //      union header{
        struct packed
        {
            ulong next_record_offset;
            ulong last_end_lsn;
        }  ;
    }  ;//

    struct LOG_RECORD
    {
        ulong this_lsn;
        ulong client_previous_lsn;
        ulong client_undo_next_lsn;
        uint client_data_length;
        struct client_id
        {
            ushort seq_number;
            ushort client_index;
        }  ;
        uint record_type;
        uint transaction_id;
        LOG_RECORD_FLAGS flags;
        ushort[] reserved_or_alignment;//[3];
        //   Now are at ofs 0x30 into struct.
        ushort redo_operation;
        ushort undo_operation;
        ushort redo_offset;
        ushort redo_length;
        ushort undo_offset;
        ushort undo_length;
        ushort target_attribute;
        ushort lcns_to_follow;       //            Number of lcn_list entries following this entry.
        ushort record_offset;
        ushort attribute_offset;
        uint alignment_or_reserved;
        uint target_vcn;
        uint alignment_or_reserved1;
        struct lcn_list
        {      //                     Only present if lcns_to_follow is not 0.
            uint lcn;
            uint alignment_or_reserved;
        } ;

        enum LOG_RECORD_FLAGS
        {
            LOG_RECORD_MULTI_PAGE = 1,    //    ???
            LOG_RECORD_SIZE_PLACE_HOLDER = 0xffff, /*
                    This has nothing to do with the log record. It is only so
                       gcc knows to make the flags 16-bit.*/
        }
    }
}


