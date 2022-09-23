using System;

namespace WC2.Unpacker
{
    class PackageHeader
    {
        public UInt32 dwMagic { get; set; } // 0x304B4350 (PCK0)
        public Int16 wVersion { get; set; } // 20
        public Int32 dwPatchVersion { get; set; } // 4
        public Int32 bFlag1 { get; set; } // 0 (byte)
        public Int32 dwTotalFiles { get; set; }
        public Int32 dwTableCompressedSize { get; set; }
        public Int32 dwTableDecompressedSize { get; set; }
        public Int32 bFlag2 { get; set; } // 0 (byte)
    }
}
