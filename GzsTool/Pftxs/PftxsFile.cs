using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using GzsTool.Common;
using GzsTool.Common.Interfaces;
using GzsTool.Utility;

namespace GzsTool.Pftxs
{
    [XmlType("PftxsFile")]
    public class PftxsFile : ArchiveFile
    {
        private const int PftxMagicNumber = 0x58544650; // PFTX
        private const int TexlMagicNumber = 0x4C584554; // TEXL
        private const int FtexMagicNumber = 0x58455446; // FTEX

        private const long FtexHeaderSize = 16;
        private const long TexlHeaderSize = 16;

        public PftxsFile()
        {
            Files = new List<PftxsFtexFile>();
        }

        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlAttribute("Endianness")]
        public string Endianness { get; set; }

        [XmlArray("Entries")]
        public List<PftxsFtexFile> Files { get; set; }

        [XmlIgnore]
        public int Size { get; set; }

        [XmlIgnore]
        public int FileCount { get; set; }

        public static PftxsFile ReadPftxsFile(Stream input)
        {
            PftxsFile pftxsFile = new PftxsFile();
            pftxsFile.Read(input);
            return pftxsFile;
        }

        public override void Read(Stream input)
        {
            X360Reader reader = new X360Reader(input, Encoding.Default, true, false);
            int pftxsMagicNumber = reader.ReadInt32(); // PFTXS
            if (pftxsMagicNumber != PftxMagicNumber)
                return;

            int unknown1 = reader.ReadInt32(); // 0x40 00 00 00
            if (unknown1 != 0x40000000)
            { 
                if(unknown1 != 0x40)
                    return;
                reader.BaseStream.Position -= 4;
                reader.FlipEndian = true;
                unknown1 = reader.ReadInt32();
            }
            Endianness = reader.FlipEndian ? "Big" : "Little";

            int unknown2 = reader.ReadInt32(); // 0x10
            if (unknown2 != 0x10)
                return;
            int unknown3 = reader.ReadInt32(); // 0x1
            if (unknown3 != 0x1)
                return;

            int texlistMagicNumber = reader.ReadInt32(); // TEXL
            Size = reader.ReadInt32();
            FileCount = reader.ReadInt32();
            int unknown4 = reader.ReadInt32();

            for (int i = 0; i < FileCount; i++)
            {
                PftxsFtexFile pftxsFtexFile = new PftxsFtexFile();
                pftxsFtexFile.Read(reader);
                Files.Add(pftxsFtexFile);
            }
        }

        public override IEnumerable<FileDataStreamContainer> ExportFiles(Stream input)
        {
            foreach (var file in Files)
            {
                foreach (var entry in file.Entries)
                {
                    var localEntry = entry;
                    yield return new FileDataStreamContainer
                    {
                        DataStream = () => new MemoryStream(localEntry.Data),
                        FileName = entry.FilePath
                    };
                }
            }
        }

        public override void Write(Stream output, IDirectory inputDirectory)
        {
            X360Writer writer = new X360Writer(output, Encoding.Default, true, Endianness == "Big");
            long ftexHeaderPosition = output.Position;
            output.Position += FtexHeaderSize;
            long texlHeaderPosition = output.Position;
            output.Position += TexlHeaderSize;
            foreach (var file in Files)
            {
                file.WriteData(writer, inputDirectory);
            }

            long endPosition = output.Position;
            output.Position = ftexHeaderPosition;
            writer.FlipEndian = false;
            writer.Write(PftxMagicNumber);
            writer.FlipEndian = Endianness == "Big";

            writer.Write(0x40000000);
            writer.Write(0x00000010);
            writer.Write(0x00000001);

            output.Position = texlHeaderPosition;
            writer.FlipEndian = false;
            writer.Write(0x4C584554); // TEXL
            writer.FlipEndian = Endianness == "Big";

            writer.Write(Convert.ToUInt32(endPosition - texlHeaderPosition)); // Size
            writer.Write(Convert.ToUInt32(Files.Count));
            
            output.Position = endPosition;
        }
    }
}
