using System.IO;
using System.Text;
using GzsTool.Utility;

namespace GzsTool.PathId
{
    internal class FileHash
    {
        public ulong Hash { get; set; }

        public static FileHash ReadFileHash(Stream input)
        {
            FileHash fileHash = new FileHash();
            fileHash.Read(input);
            return fileHash;
        }

        private void Read(Stream input)
        {
            X360Reader reader = new X360Reader(input, Encoding.Default, true, true);
            Hash = reader.ReadUInt64();
        }
    }
}
