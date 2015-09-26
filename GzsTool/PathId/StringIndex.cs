using System.IO;
using System.Text;
using GzsTool.Utility;

namespace GzsTool.PathId
{
    internal class StringIndex
    {
        public uint Offset { get; set; }

        public static StringIndex ReadStringIndex(Stream input)
        {
            StringIndex stringIndex = new StringIndex();
            stringIndex.Read(input);
            return stringIndex;
        }

        private void Read(Stream input)
        {
            X360Reader reader = new X360Reader(input, Encoding.Default, true, true);
            Offset = reader.ReadUInt32();
        }
    }
}
