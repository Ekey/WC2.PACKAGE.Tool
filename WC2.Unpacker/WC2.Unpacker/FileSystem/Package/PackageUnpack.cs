using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace WC2.Unpacker
{
    class PackageUnpack
    {
        static List<PackageEntry> m_EntryTable = new List<PackageEntry>();

        public static void iDoIt(String m_Archive, String m_DstFolder)
        {
            using (FileStream TPackageStream = File.OpenRead(m_Archive))
            {
                var m_Header = new PackageHeader();

                m_Header.dwMagic = TPackageStream.ReadUInt32();
                m_Header.wVersion = TPackageStream.ReadInt16();
                m_Header.dwPatchVersion = TPackageStream.ReadInt32();
                m_Header.bFlag1 = TPackageStream.ReadByte();
                m_Header.dwTotalFiles = TPackageStream.ReadInt32();
                m_Header.dwTableCompressedSize = TPackageStream.ReadInt32();
                m_Header.dwTableDecompressedSize = TPackageStream.ReadInt32();
                m_Header.bFlag2 = TPackageStream.ReadByte();

                if (m_Header.dwMagic != 0x304B4350)
                {
                    throw new Exception("[ERROR]: Invalid magic of PACKAGE archive file!");
                }

                if (m_Header.wVersion != 20)
                {
                    throw new Exception("[ERROR]: Invalid version of PACKAGE archive file!");
                }

                if (m_Header.dwPatchVersion != 4)
                {
                    throw new Exception("[ERROR]: Invalid patch version of PACKAGE archive file!");
                }

                var lpVector = TPackageStream.ReadBytes(8);
                var lpSrcBuffer = TPackageStream.ReadBytes(m_Header.dwTableCompressedSize - 8);

                lpSrcBuffer = PackageCipher.iDecryptData(lpSrcBuffer, lpVector);
                var lpDstBuffer = LZ4.iDecompress(lpSrcBuffer, m_Header.dwTableDecompressedSize);

                using (MemoryStream TEntryReader = new MemoryStream(lpDstBuffer))
                {
                    m_EntryTable.Clear();
                    for (Int32 i = 0; i < m_Header.dwTotalFiles; i++)
                    {
                        var m_Entry = new PackageEntry();

                        m_Entry.wFileID = TEntryReader.ReadInt16();
                        m_Entry.dwOffset = TEntryReader.ReadInt64();
                        m_Entry.dwDecompressedSize = TEntryReader.ReadInt32();
                        m_Entry.dwCompressedSize = TEntryReader.ReadInt32();
                        m_Entry.dwCRC = TEntryReader.ReadUInt32();
                        m_Entry.dwUnknown = TEntryReader.ReadUInt32();
                        m_Entry.bFlag = TEntryReader.ReadByte();
                        m_Entry.m_FileName = TEntryReader.ReadString(Encoding.GetEncoding("GB2312"));

                        m_EntryTable.Add(m_Entry);
                    }
                    TEntryReader.Dispose();
                }

                foreach (var m_Entry in m_EntryTable)
                {
                    String m_FullPath = m_DstFolder + m_Entry.m_FileName;

                    Utils.iSetInfo("[UNPACKING]: " + m_Entry.m_FileName);
                    Utils.iCreateDirectory(m_FullPath);

                    TPackageStream.Seek(m_Entry.dwOffset, SeekOrigin.Begin);
                    var lpTemp = TPackageStream.ReadBytes(m_Entry.dwCompressedSize);

                    if (m_Entry.dwDecompressedSize == m_Entry.dwCompressedSize)
                    {
                        File.WriteAllBytes(m_FullPath, lpTemp);
                    }
                    else
                    {
                        var lpBuffer = ZSTD.iDecompress(lpTemp);
                        File.WriteAllBytes(m_FullPath, lpBuffer);
                    }
                }
                TPackageStream.Dispose();
            }
        }
    }
}
