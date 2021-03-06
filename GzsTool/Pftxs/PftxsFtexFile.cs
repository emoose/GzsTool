﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using GzsTool.Common.Interfaces;
using GzsTool.Utility;

namespace GzsTool.Pftxs
{
    [XmlType("Entry", Namespace = "Pftxs")]
    public class PftxsFtexFile
    {
        public const int HeaderSize = 32;
        
        [XmlIgnore]
        public string FileName { get; set; }

        [XmlIgnore]
        public long DataOffset { get; set; }
        
        [XmlAttribute("FilePath")]
        public string FilePath { get; set; }

        [XmlAttribute("Hash")]
        public ulong Hash { get; set; }

        [XmlArray("Entries")]
        public List<PftxsFtexsFileEntry> Entries { get; set; }

        public void Read(X360Reader reader)
        {
            long ftexBaseOffset = reader.BaseStream.Position;
            int magicNumber = reader.ReadInt32(); // FTEX
            int size = reader.ReadInt32();
            Hash = reader.ReadUInt64();
            int count = reader.ReadInt32();
            int unknown1 = reader.ReadInt32(); // 0
            int unknown2 = reader.ReadInt32(); // 0
            int unknown3 = reader.ReadInt32(); // 0

            Entries = new List<PftxsFtexsFileEntry>();
            for (int i = 0; i < count; i++)
            {
                PftxsFtexsFileEntry entry = new PftxsFtexsFileEntry();
                entry.Read(reader);

                string name;
                Hashing.TryGetFileNameFromHash(entry.Hash, out name, false);
                entry.FilePath = Hashing.NormalizeFilePath(name);
                Entries.Add(entry);
            }
            
            foreach (var entry in Entries)
            {
                reader.BaseStream.Position = ftexBaseOffset + entry.Offset;
                entry.Data = reader.ReadBytes(entry.Size);
            }
        }
        
        public void WriteData(X360Writer writer, IDirectory inputDirectory)
        {
            long ftexHeaderPosition = writer.BaseStream.Position;
            writer.BaseStream.Position += HeaderSize + Entries.Count * PftxsFtexsFileEntry.HeaderSize;

            bool flipEndian = writer.FlipEndian;

            foreach (var entry in Entries)
            {
                var data = inputDirectory.ReadFile(entry.FilePath);
                entry.Offset = Convert.ToInt32(writer.BaseStream.Position - ftexHeaderPosition);
                entry.Size = Convert.ToInt32(data.Length);
                writer.Write(data);
            }
            long endPosition = writer.BaseStream.Position;

            writer.BaseStream.Position = ftexHeaderPosition;
            writer.FlipEndian = false;
            writer.Write(Convert.ToUInt32(0x58455446)); // FTEX
            writer.FlipEndian = flipEndian;

            writer.Write(Convert.ToUInt32(endPosition - ftexHeaderPosition)); // Size
            writer.Write(Hash);
            writer.Write(Convert.ToUInt32(Entries.Count));
            writer.Write(0U);
            writer.Write(0U);
            writer.Write(0U);
            foreach (var entry in Entries)
            {
                entry.Write(writer);
            }

            writer.BaseStream.Position = endPosition;
        }
    }
}
