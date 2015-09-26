using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GzsTool.Common;
using GzsTool.Common.Interfaces;
using GzsTool.Utility;

namespace GzsTool.Qar
{
    [XmlType("Entry", Namespace = "Qar")]
    public class QarEntry
    {
        [XmlAttribute("Hash")]
        public ulong Hash { get; set; }

        [XmlAttribute("Key")]
        public uint Key { get; set; }

        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }

        [XmlAttribute("Compressed")]
        public bool Compressed { get; set; }

        [XmlIgnore]
        public bool FileNameFound { get; set; }

        [XmlIgnore]
        public uint UncompressedSize { get; private set; }

        [XmlIgnore]
        public uint CompressedSize { get; private set; }
        
        [XmlIgnore]
        public long DataOffset { get; set; }

        [XmlIgnore]
        public int QarVersion { get; set; }
        
        // TODO: Enable when the hashing is fixed
        ////public bool ShouldSerializeHash()
        ////{
        ////    return FileNameFound == false;
        ////}

        public bool ShouldSerializeKey()
        {
            return Key != 0;
        }

        public void CalculateHash()
        {
            if (Hash == 0)
            {
                Hash = Hashing.HashFileNameWithExtension(FilePath);
            }
            else
            {
                DebugAssertHashMatches();
            }
        }

        [Conditional("DEBUG")]
        private void DebugAssertHashMatches()
        {
            ulong newHash = Hashing.HashFileNameWithExtension(FilePath);
            Debug.Assert(Hash == newHash);
        }
        
        public void Read(BinaryReader reader, int qarVersion = 3)
        {
            QarVersion = qarVersion;

            const uint xorMask1 = 0x41441043;
            const uint xorMask2 = 0x11C22050;
            const uint xorMask3 = 0xD05608C3;
            const uint xorMask4 = 0x532C7319;

            uint hashLow = reader.ReadUInt32();
            uint hashHigh = reader.ReadUInt32();
            
            if(qarVersion == 3)
            {
                hashLow = hashLow ^ xorMask1;
                hashHigh = hashHigh ^ xorMask1;
            }
            Hash = (ulong)hashHigh << 32 | hashLow;

            if (qarVersion != 3)
                DataOffset = reader.ReadUInt32() * 16;

            UncompressedSize = reader.ReadUInt32();
            if (qarVersion == 3)
            {
                UncompressedSize = UncompressedSize ^ xorMask2;
                CompressedSize = reader.ReadUInt32() ^ xorMask3;
                Compressed = UncompressedSize != CompressedSize;

                uint md51 = reader.ReadUInt32() ^ xorMask4;
                uint md52 = reader.ReadUInt32() ^ xorMask1;
                uint md53 = reader.ReadUInt32() ^ xorMask1;
                uint md54 = reader.ReadUInt32() ^ xorMask2;
            }

            string filePath;
            FileNameFound = TryGetFilePath(out filePath);
            FilePath = filePath;

            if(qarVersion == 3)
                DataOffset = reader.BaseStream.Position;
        }
        
        public FileDataStreamContainer Export(Stream input)
        {
            FileDataStreamContainer fileDataStreamContainer = new FileDataStreamContainer
            {
                DataStream = ReadDataLazy(input),
                FileName = FilePath
            };
            return fileDataStreamContainer;
        }
        
        private Func<Stream> ReadDataLazy(Stream input)
        {
            return () =>
            {
                lock (input)
                {
                    return ReadData(input);
                }
            };
        }

        public static Tuple<uint, bool, Stream> GetOriginalData(Stream input, ulong hash, long dataOffset, uint dataSize, int qarVersion = 3)
        {
            input.Position = dataOffset;
            BinaryReader reader = new BinaryReader(input, Encoding.Default, true);

            int size = (int)dataSize;
            uint key = 0;

            byte[] sectionData = reader.ReadBytes(size);

            if(qarVersion == 1)
            {
                sectionData = Encryption.DeEncryptQar(sectionData, (uint)(dataOffset / 16));
                const uint keyConstant = 0xA0F8EFE6;
                uint peekData = BitConverter.ToUInt32(sectionData, 0);
                if (peekData == keyConstant)
                {
                    key = BitConverter.ToUInt32(sectionData, 4);
                    dataSize -= 8;
                    byte[] data2 = new byte[sectionData.Length - 8];
                    Array.Copy(sectionData, 8, data2, 0, sectionData.Length - 8);
                    sectionData = Encryption.DeEncrypt(data2, key);
                }
                return new Tuple<uint, bool, Stream>(key, false, new MemoryStream(sectionData));
            }

            if (qarVersion == 3)
                Decrypt1(sectionData, hashLow: (uint)(hash & 0xFFFFFFFF));

            bool compressed = false;

            uint magicEntry = BitConverter.ToUInt32(sectionData, 0);
            if (magicEntry == 0xA0F8EFE6)
            {
                const int headerSize = 8;
                key = BitConverter.ToUInt32(sectionData, 4);
                size -= headerSize;
                byte[] newSectionData = new byte[size];
                Array.Copy(sectionData, headerSize, newSectionData, 0, size);
                Decrypt2(newSectionData, key);
            }
            else if (magicEntry == 0xE3F8EFE6)
            {
                const int headerSize = 16;
                key = BitConverter.ToUInt32(sectionData, 4);
                if (qarVersion != 3)
                {
                    uint ucsize = BitConverter.ToUInt32(sectionData, 8);
                    uint csize = BitConverter.ToUInt32(sectionData, 0xC);
                    compressed = ucsize != csize;
                    if (ucsize == 0 || csize == 0)
                        compressed = false;
                }
                size -= headerSize;

                byte[] newSectionData = new byte[size];
                Array.Copy(sectionData, headerSize, newSectionData, 0, size);
                Decrypt2(newSectionData, key);
                sectionData = newSectionData;
            }

            if (compressed)
                sectionData = Compression.Inflate(sectionData);

            return new Tuple<uint, bool, Stream>(key, compressed, new MemoryStream(sectionData));
        }

        private Stream ReadData(Stream input)
        {
            var retVal = GetOriginalData(input, Hash, DataOffset, UncompressedSize, QarVersion);
            Key = retVal.Item1;
            Compressed = retVal.Item2;
            return retVal.Item3;
        }
        
        private bool TryGetFilePath(out string filePath)
        {
            bool filePathFound = Hashing.TryGetFileNameFromHash(Hash, out filePath, QarVersion == 1);
            filePath = Hashing.NormalizeFilePath(filePath);
            return filePathFound;
        }
        
        private static void Decrypt1(byte[] sectionData, uint hashLow)
        {
            // TODO: Use a ulong array instead.
            uint[] decryptionTable =
            {
                0xBB8ADEDB,
                0x65229958,
                0x08453206,
                0x88121302,
                0x4C344955,
                0x2C02F10C,
                0x4887F823,
                0xF3818583,
                //0x40C90FDB,
                //0x3FC90FDB,
                //0x3F490FDB,
                //0x3EA2F983,
                //0x3C8EFA35,
                //0x42652EE0,
                //0x40C90FDB,
                //0x3FC90FDB,
                //0x3F490FDB,
                //0x3EA2F983,
                //0x3C8EFA35,
                //0x42652EE0
            };

            int blocks = sectionData.Length / sizeof(ulong);
            for (int i = 0; i < blocks; i++)
            {
                int offset1 = i * sizeof(ulong);
                int offset2 = i * sizeof(ulong) + sizeof(uint);
                int index = (int)(2 * ((hashLow + offset1 / 11) % 4));
                uint u1 = BitConverter.ToUInt32(sectionData, offset1) ^ decryptionTable[index];
                uint u2 = BitConverter.ToUInt32(sectionData, offset2) ^ decryptionTable[index + 1];
                Buffer.BlockCopy(BitConverter.GetBytes(u1), 0, sectionData, offset1, sizeof(uint));
                Buffer.BlockCopy(BitConverter.GetBytes(u2), 0, sectionData, offset2, sizeof(uint));
            }

            int remaining = sectionData.Length % sizeof(ulong);
            for (int i = 0; i < remaining; i++)
            {
                int offset = blocks * sizeof(long) + i * sizeof(byte);
                int index = (int)(2 * ((hashLow + (offset - (offset % sizeof(long))) / 11) % 4));
                int decryptionIndex = offset % sizeof(long);
                uint xorMask = decryptionIndex < 4 ? decryptionTable[index] : decryptionTable[index + 1];
                byte xorMaskByte = (byte)((xorMask >> (8 * decryptionIndex)) & 0xff);
                byte b1 = (byte)(sectionData[offset] ^ xorMaskByte);
                sectionData[offset] = b1;
            }
        }

        private static unsafe void Decrypt2(byte[] input, uint key)
        {
            int size = input.Length;
            uint currentKey = key | ((key ^ 25974) << 16);

            byte[] output = input.ToArray();
            fixed (byte* pDestBase = output, pSrcBase = input)
            {
                uint* pDest = (uint*)pDestBase;
                uint* pSrc = (uint*)pSrcBase;
                uint i = 278 * key;
                for (; size >= 64; size -= 64)
                {
                    uint j = 16;
                    do
                    {
                        *pDest = currentKey ^ *pSrc;
                        currentKey = i + 48828125 * currentKey;

                        --j;
                        pDest++;
                        pSrc++;
                    }
                    while (j > 0);
                }
                for (; size >= 16; pSrc += 4)
                {
                    *pDest = currentKey ^ *pSrc;
                    uint v7 = i + 48828125 * currentKey;
                    *(pDest + 1) = v7 ^ *(pSrc + 1);
                    uint v8 = i + 48828125 * v7;
                    *(pDest + 2) = v8 ^ *(pSrc + 2);
                    uint v9 = i + 48828125 * v8;
                    *(pDest + 3) = v9 ^ *(pSrc + 3);

                    currentKey = i + 48828125 * v9;
                    size -= 16;
                    pDest += 4;
                }
                for (; size >= 4; pSrc++)
                {
                    *pDest = currentKey ^ *pSrc;

                    currentKey = i + 48828125 * currentKey;
                    size -= 4;
                    pDest++;
                }
            }

            Buffer.BlockCopy(output, 0, input, 0, input.Length);
        }

        public void Write(Stream output, IDirectory inputDirectory)
        {
            const ulong xorMask1Long = 0x4144104341441043;
            const uint xorMask1 = 0x41441043;
            const uint xorMask2 = 0x11C22050;
            const uint xorMask3 = 0xD05608C3;
            const uint xorMask4 = 0x532C7319;

            byte[] data = inputDirectory.ReadFile(Hashing.NormalizeFilePath(FilePath));
            uint uncompressedSize = (uint) data.Length;
            uint compressedSize;
            if (Compressed)
            {
                data = Compression.Deflate(data);
                compressedSize = (uint) data.Length;
            }
            else
            {
                compressedSize = uncompressedSize;
            }

            byte[] hash = Hashing.Md5Hash(data);
            Decrypt1(data, hashLow: (uint)(Hash & 0xFFFFFFFF));
            BinaryWriter writer = new BinaryWriter(output, Encoding.Default, true);
            writer.Write(Hash ^ xorMask1Long);
            writer.Write(compressedSize ^ xorMask2);
            writer.Write(uncompressedSize ^ xorMask3);
            
            writer.Write(BitConverter.ToUInt32(hash, 0) ^ xorMask4);
            writer.Write(BitConverter.ToUInt32(hash, 4) ^ xorMask1);
            writer.Write(BitConverter.ToUInt32(hash, 8) ^ xorMask1);
            writer.Write(BitConverter.ToUInt32(hash, 12) ^ xorMask2);

            // TODO: Maybe reencrypt the lua files.
            writer.Write(data);
        }
    }
}