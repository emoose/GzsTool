using System.IO;
using System.Linq;
using System.Text;
using GzsTool.Utility;

namespace GzsTool.Fpk
{
    public class FpkString
    {
        public string Value { get; set; }
        public byte[] EncryptedValue { get; set; }
        public long StringOffset { get; set; }
        public int StringLength { get; set; }
        public bool ValueResolved { get; set; }

        public bool ValueEncrypted
        {
            get { return EncryptedValue != null; }
        }

        public static FpkString ReadFpkString(X360Reader reader)
        {
            FpkString fpkString = new FpkString();
            fpkString.Read(reader);
            return fpkString;
        }

        private void Read(X360Reader reader)
        {
            StringOffset = reader.ReadInt64();
            StringLength = reader.ReadInt32();
            reader.Skip(4);

            long endPosition = reader.BaseStream.Position;
            reader.BaseStream.Position = StringOffset;
            Value = reader.ReadString(StringLength);
            reader.BaseStream.Position = endPosition;
        }

        public override string ToString()
        {
            return Value;
        }

        public void ResolveString(byte[] md5Hash)
        {
            bool resolved;
            byte[] entryNameHash = Hashing.Md5HashText(Value);

            if (entryNameHash.SequenceEqual(md5Hash) == false)
            {
                EncryptedValue = Encoding.Default.GetBytes(Value);
                string resolvedValue;
                resolved = Hashing.TryGetFileNameFromMd5Hash(md5Hash, Value, out resolvedValue);
                Value = resolvedValue;
            }
            else
            {
                resolved = true;
            }

            ValueResolved = resolved;
        }

        public void WriteString(X360Writer writer)
        {
            StringOffset = (int) writer.BaseStream.Position;
            string value = ValueEncrypted ? Encoding.Default.GetString(EncryptedValue) : Value;
            StringLength = value.Length;
            writer.WriteNullTerminatedString(value);
        }

        public void Write(X360Writer writer)
        {
            writer.Write(StringOffset);
            writer.Write(StringLength);
            writer.WriteZeros(4);
        }
    }
}
