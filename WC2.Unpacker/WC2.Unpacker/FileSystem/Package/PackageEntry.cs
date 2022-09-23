using System;

namespace WC2.Unpacker
{
    class PackageEntry
    {
        public Int16 wFileID { get; set; } // ????
        public Int64 dwOffset { get; set; }
        public Int32 dwDecompressedSize { get; set; }
        public Int32 dwCompressedSize { get; set; }
        public UInt32 dwCRC { get; set; }
        public UInt32 dwUnknown { get; set; }
        public Int32 bFlag { get; set; } // 0 (byte)
        public String m_FileName { get; set; }
    }
}
