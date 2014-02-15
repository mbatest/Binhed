using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace LowLevel
{
    //The index blocks all start with the same string: 6B 87 08 38 76 C1 48 4E B7 AE 04 04 6E 6C C7 52 01 00.+ 6 bytes
    public class INDEX_BLOCK
    {
        ELEMENTARY_TYPE file_offset;//0x18
        ELEMENTARY_TYPE volume_offset;//020
        ELEMENTARY_TYPE volume_offset_for_next_index_block; //0x28
        List<DATA_BLOCK> data_block_index;  //0x80
    }
   // Each record here is 32 bytes in length.
    public class DATA_BLOCK
    {
        ELEMENTARY_TYPE original_logical_location ;//0x00
        ELEMENTARY_TYPE file_offset;//0x08
        ELEMENTARY_TYPE logical_volume_address ;//0x10
        ELEMENTARY_TYPE other;//0x18
    }
}
