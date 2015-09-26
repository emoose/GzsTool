using System.IO;
using System.Xml.Serialization;
using GzsTool.Utility;

namespace GzsTool.Fpk
{
    [XmlType("Reference", Namespace = "Fpk")]
    public class FpkReference
    {
        public FpkReference()
        {
            ReferenceFilePath = new FpkString();
        }

        [XmlIgnore]
        public FpkString ReferenceFilePath { get; set; }

        [XmlAttribute("FilePath")]
        public string FilePath
        {
            get { return ReferenceFilePath.Value; }
            set { ReferenceFilePath.Value = value; }
        }

        public static FpkReference ReadFpkReference(X360Reader reader)
        {
            FpkReference fpkReference = new FpkReference();
            fpkReference.Read(reader);
            return fpkReference;
        }

        private void Read(X360Reader reader)
        {
            ReferenceFilePath = FpkString.ReadFpkString(reader);
        }

        public void WriteFilePath(X360Writer writer)
        {
            ReferenceFilePath.WriteString(writer);
        }

        public void Write(X360Writer writer)
        {
            ReferenceFilePath.Write(writer);
        }
    }
}
