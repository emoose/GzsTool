using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GzsTool.Common;
using GzsTool.Common.Interfaces;
using GzsTool.Utility;

namespace GzsTool.Fpk
{
    [XmlType("FpkFile")]
    public class FpkFile : ArchiveFile
    {
        private const int FpkMagicNumber = 0x66786F66; // foxf

        public FpkFile()
        {
            Entries = new List<FpkEntry>();
            References = new List<FpkReference>();
        }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("FpkType")]
        public FpkType FpkType { get; set; }

        [XmlAttribute("Platform")]
        public string Platform { get; set; }

        [XmlAttribute("UnknownValue")]
        public uint UnknownValue { get; set; }

        [XmlArray("Entries")]
        public List<FpkEntry> Entries { get; private set; }

        [XmlArray("References")]
        public List<FpkReference> References { get; private set; }

        public static FpkFile ReadFpkFile(Stream input)
        {
            FpkFile fpkFile = new FpkFile();
            fpkFile.Read(input);
            return fpkFile;
        }

        public static bool IsBigEndianPlatform(string platform)
        {
            return platform == "x36" || platform == "ps3";
        }

        public override void Read(Stream input)
        {
            X360Reader reader = new X360Reader(input, Encoding.Default, true, false);
            uint magicNumber1 = reader.ReadUInt32(); // foxf
            if (magicNumber1 != FpkMagicNumber)
                return;

            ushort magicNumber2 = reader.ReadUInt16(); // pk
            FpkType = (FpkType) reader.ReadByte(); // ' ' or 'd'
            Platform = reader.ReadAsciiString(3);
            uint fileSize = reader.ReadUInt32();
            reader.Skip(18);

            reader.FlipEndian = IsBigEndianPlatform(Platform);

            UnknownValue = reader.ReadUInt32(); // 2 (4 on some console fpks?)
            uint fileCount = reader.ReadUInt32();
            uint referenceCount = reader.ReadUInt32();
            reader.Skip(4);

            for (int i = 0; i < fileCount; i++)
            {
                Entries.Add(FpkEntry.ReadFpkEntry(reader));
            }

            for (int i = 0; i < referenceCount; i++)
            {
                References.Add(FpkReference.ReadFpkReference(reader));
            }
        }

        public override IEnumerable<FileDataStreamContainer> ExportFiles(Stream input)
        {
            return Entries.Select(fpkEntry => fpkEntry.Export(input));
        }

        public override void Write(Stream output, IDirectory inputDirectory)
        {
            X360Writer writer = new X360Writer(output, Encoding.Default, true, IsBigEndianPlatform(Platform));
            const int headerSize = 48;
            int indicesSize = 48*Entries.Count;
            int referenceSize = 16*References.Count;

            long startPosition = output.Position;
            output.Position += headerSize + indicesSize + referenceSize;

            foreach (var fpkEntry in Entries)
            {
                fpkEntry.WriteFilePath(writer);
            }
            foreach (var fpkReference in References)
            {
                fpkReference.WriteFilePath(writer);
            }
            output.AlignWrite(16, 0x00);

            foreach (var fpkEntry in Entries)
            {
                fpkEntry.WriteData(writer, inputDirectory);
                output.AlignWrite(16, 0x00);
            }

            uint fileSize = (uint) output.Position;

            output.Position = startPosition;

            if (writer.FlipEndian)
                writer.FlipEndian = false;
            writer.Write(0x66786f66); // foxf
            writer.Write((ushort) 0x6B70); //pk
            writer.Write((byte) FpkType);
            writer.WriteAsciiString(Platform, 3);
            writer.Write(fileSize);

            writer.FlipEndian = IsBigEndianPlatform(Platform);

            writer.WriteZeros(18);
            writer.Write(UnknownValue);
            writer.Write(Entries.Count);
            writer.Write(References.Count);
            writer.WriteZeros(4);

            foreach (var fpkEntry in Entries)
            {
                fpkEntry.Write(writer);
            }
            foreach (var fpkReference in References)
            {
                fpkReference.Write(writer);
            }
        }
    }
}
