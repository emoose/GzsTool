using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using GzsTool.Common;
using GzsTool.Common.Interfaces;
using GzsTool.Utility;

namespace GzsTool.Fpk
{
    [XmlType("Entry", Namespace = "Fpk")]
    public class FpkEntry
    {
        public FpkEntry()
        {
            FilePathFpkString = new FpkString();
        }

        [XmlIgnore]
        public long DataOffset { get; set; }

        [XmlIgnore]
        public ulong DataSize { get; set; }

        [XmlIgnore]
        public FpkString FilePathFpkString { get; set; }

        [XmlAttribute("FilePath")]
        public string FilePath
        {
            get { return FilePathFpkString.Value; }
            set { FilePathFpkString.Value = value; }
        }

        [XmlAttribute("Hash")]
        public byte[] Md5Hash { get; set; }

        [XmlAttribute("EncryptedFilePath")]
        public byte[] EncryptedFilePath
        {
            get { return FilePathFpkString.EncryptedValue; }
            set { FilePathFpkString.EncryptedValue = value; }
        }

        public bool ShouldSerializeMd5Hash()
        {
            return FilePathFpkString.ValueResolved == false;
        }

        public static FpkEntry ReadFpkEntry(X360Reader reader)
        {
            FpkEntry fpkEntry = new FpkEntry();
            fpkEntry.Read(reader);
            return fpkEntry;
        }

        private void Read(X360Reader reader)
        {
            DataOffset = reader.ReadInt64();
            DataSize = reader.ReadUInt64();
            FpkString fileName = FpkString.ReadFpkString(reader);
            Md5Hash = reader.ReadBytes(16);
            fileName.ResolveString(Md5Hash);
            FilePathFpkString = fileName;
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

        private Stream ReadData(Stream input)
        {
            input.Position = DataOffset;
            byte[] result = new byte[DataSize];
            input.Read(result, 0, (int)DataSize);
            return new MemoryStream(result);
        }

        private string GetFpkEntryFileName()
        {
            string fileName = Hashing.NormalizeFilePath(FilePathFpkString.Value);

            // Some files are prefixed with a drive letter (e.g. "Z:")
            int index = fileName.IndexOf(":", StringComparison.Ordinal);
            if (index != -1)
            {
                fileName = fileName.Substring(index + 1, fileName.Length - index - 1);
            }
            
            return fileName;
        }

        public void WriteFilePath(X360Writer writer)
        {
            if (Md5Hash == null)
                Md5Hash = Hashing.Md5HashText(FilePath);
            FilePathFpkString.WriteString(writer);
        }

        public void Write(X360Writer writer)
        {
            writer.Write(DataOffset);
            writer.Write(DataSize);
            FilePathFpkString.Write(writer);
            writer.Write(Md5Hash);
        }

        public void WriteData(X360Writer writer, IDirectory inputDirectory)
        {
            DataOffset = (uint) writer.BaseStream.Position;
            byte[] data = inputDirectory.ReadFile(GetFpkEntryFileName());
            DataSize = (ulong)data.Length;
            writer.BaseStream.Write(data, 0, data.Length);
        }

        public FileDataStreamContainer Export(Stream input)
        {
            FileDataStreamContainer fileDataStreamContainer = new FileDataStreamContainer
            {
                DataStream = ReadDataLazy(input),
                FileName = GetFpkEntryFileName()
            };
            return fileDataStreamContainer;
        }
    }
}
