using System.IO;
using System.Text;
using GzsTool.Utility;

namespace GzsTool.PathId
{
    internal class FolderInfo
    {
        public ushort Offset1 { get; set; }
        public ushort Offset2 { get; set; }

        public static FolderInfo ReadFolderInfo(Stream input)
        {
            FolderInfo folderInfo = new FolderInfo();
            folderInfo.Read(input);
            return folderInfo;
        }

        private void Read(Stream input)
        {
            X360Reader reader = new X360Reader(input, Encoding.Default, true, true);
            Offset1 = reader.ReadUInt16();
            Offset2 = reader.ReadUInt16();
        }
    }
}
