using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;

namespace GzsTool.Utility
{
    /// <summary>
    ///   The Reader class.
    /// </summary>
    public class X360Reader : BinaryReader
    {
        /// <summary>
        ///   Determines if we should flip the endianness of read values.
        /// </summary>
        public bool FlipEndian { get; set; }

        /// <summary>
        ///   Initializes a new instance of the X360Reader class with the specified X360IO.BaseIO instance.
        /// </summary>
        public X360Reader(Stream stream, Encoding encoding, bool leaveOpen, bool flipEndian)
            : base(stream, encoding, leaveOpen)
        {
            FlipEndian = flipEndian;
        }

        /// <summary>
        ///   Reads an ASCII string from the parent stream.
        /// </summary>
        /// <param name = "length">The length of the string to read.</param>
        /// <returns>An ASCII encoded string.</returns>
        public string ReadAsciiString(int length)
        {
            string str = "";
            int num = 0;
            for (int i = 0; i < length; i++)
            {
                char ch = ReadChar();
                num++;
                if (ch == '\0')
                {
                    break;
                }
                str = str + ch;
            }
            int num3 = length - num;
            BaseStream.Seek(num3, SeekOrigin.Current);
            return str;
        }
       
        /// <summary>
        ///   Reads a double precision float from the parent stream.
        /// </summary>
        /// <returns>A double precision float.</returns>
        public override double ReadDouble()
        {
            byte[] array = base.ReadBytes(4);
            if (FlipEndian)
                Array.Reverse(array);

            return BitConverter.ToDouble(array, 0);
        }
        /// <summary>
        ///   Reads a 16-bit integer from the parent stream.
        /// </summary>
        /// <returns>A 16-bit integer.</returns>
        public override short ReadInt16()
        {
            byte[] array = base.ReadBytes(2);
            if (FlipEndian)
                Array.Reverse(array);

            return BitConverter.ToInt16(array, 0);
        }
        public byte[] GenerateSHA1Hash(int length)
        {
            return new SHA1CryptoServiceProvider().ComputeHash(ReadBytes(length));
        }
        /// <summary>
        ///   Reads a 24-bit integer from the parent stream.
        /// </summary>
        /// <returns>A 24-bit integer.</returns>
        public int ReadInt24()
        {
            byte[] sourceArray = base.ReadBytes(3);
            byte[] destinationArray = new byte[4];
            Array.Copy(sourceArray, 0, destinationArray, 0, 3);
            if (FlipEndian)
                Array.Reverse(destinationArray);

            return BitConverter.ToInt32(destinationArray, 0);
        }

        /// <summary>
        ///   Reads a 32-bit integer from the parent stream.
        /// </summary>
        /// <returns>A 32-bit integer</returns>
        public override int ReadInt32()
        {
            byte[] array = base.ReadBytes(4);
            if (FlipEndian)
                Array.Reverse(array);

            return BitConverter.ToInt32(array, 0);
        }

        /// <summary>
        ///   Reads a 64-bit integer from the parent stream.
        /// </summary>
        /// <returns>A 64-bit integer.</returns>
        public override long ReadInt64()
        {
            byte[] array = base.ReadBytes(8);
            if (FlipEndian)
                Array.Reverse(array);

            return BitConverter.ToInt64(array, 0);
        }

        /// <summary>
        ///   Reads an ASCII encoded string from the parent stream, terminated with a null byte.
        /// </summary>
        /// <returns>An ASCII encoded string.</returns>
        public string ReadNullTerminatedAsciiString()
        {
            string newString = string.Empty;
            while (true)
            {
                byte tempChar = ReadByte();
                if (tempChar != 0)
                    newString += (char)tempChar;
                else
                    break;
            }
            return newString;
        }

        /// <summary>
        ///   Reads a Unicode encoded string from the parent stream, terminated with a null byte.
        /// </summary>
        /// <returns>A Unicode encoded string.</returns>
        public string ReadNullTerminatedUnicodeString()
        {
            string newString = string.Empty;
            while (true)
            {
                ushort tempChar = ReadUInt16();
                if (tempChar != 0)
                    newString += (char)tempChar;
                else
                    break;
            }
            return newString;
        }

        /// <summary>
        ///   Reads a single precision float from the parent stream.
        /// </summary>
        /// <returns>A single precision float</returns>
        public override float ReadSingle()
        {
            byte[] array = base.ReadBytes(4);
            if (FlipEndian)
                Array.Reverse(array);

            return BitConverter.ToSingle(array, 0);
        }

        /// <summary>
        ///   Reads an ASCII encoded string of the provided length from the parent stream.
        /// </summary>
        /// <param name = "length">The length of the string to read.</param>
        /// <returns>An ASCII encoded string.</returns>
        public string ReadString(int length)
        {
            return ReadAsciiString(length);
        }

        /// <summary>
        ///   Reads an unsigned 16-bit integer from the parent stream
        /// </summary>
        /// <returns>An unsigned 16-bit integer</returns>
        public override ushort ReadUInt16()
        {
            byte[] array = base.ReadBytes(2);
            if (FlipEndian)
                Array.Reverse(array);

            return BitConverter.ToUInt16(array, 0);
        }

        /// <summary>
        ///   Reads an unsigned 32-bit integer from the parent stream
        /// </summary>
        /// <returns>An unsigned 32-bit integer</returns>
        public override uint ReadUInt32()
        {
            byte[] array = base.ReadBytes(4);
            if (FlipEndian)
                Array.Reverse(array);

            return BitConverter.ToUInt32(array, 0);
        }

        /// <summary>
        ///   Reads an unsigned 64-bit integer from the parent stream
        /// </summary>
        /// <returns>An unsigned 64-bit integer</returns>
        public override ulong ReadUInt64()
        {
            byte[] array = base.ReadBytes(8);
            if (FlipEndian)
                Array.Reverse(array);

            return BitConverter.ToUInt64(array, 0);
        }

        /// <summary>
        ///   Reads a Unicode encoded string of the provided length from the parent stream.
        /// </summary>
        /// <param name = "length">The length of the string to read.</param>
        /// <returns>An Unicode encoded string.</returns>
        public string ReadUnicodeString(int length)
        {
            string str = "";
            int num = 0;
            for (int i = 0; i < length; i++)
            {
                char ch = (char)ReadUInt16();
                num++;
                if (ch == '\0')
                {
                    break;
                }
                str = str + ch;
            }
            int num3 = (length - num) * 2;
            BaseStream.Seek(num3, SeekOrigin.Current);
            return str;
        }

        /// <summary>
        ///   Moves the streams position to the specified offset.
        /// </summary>
        /// <param name = "offset">The offset to move to.</param>
        public void SeekTo(int offset)
        {
            SeekTo(offset, SeekOrigin.Begin);
        }

        /// <summary>
        ///   Moves the streams position to the specified offset.
        /// </summary>
        /// <param name = "xOffset">The offset to move to.</param>
        public void SeekTo(long xOffset)
        {
            SeekTo((int)xOffset, SeekOrigin.Begin);
        }

        /// <summary>
        ///   Reads a null terminated ASCII encoded string from the parent stream.
        /// </summary>
        /// <returns>An ASCII encoded string</returns>
        public override string ReadString()
        {
            return ReadNullTerminatedAsciiString();
        }

        /// <summary>
        ///   Moves the streams position to the specified offset.
        /// </summary>
        /// <param name = "offset">The offset to move to.</param>
        public void SeekTo(uint offset)
        {
            SeekTo((int)offset, SeekOrigin.Begin);
        }

        /// <summary>
        ///   Moves the streams position to the specified offset.
        /// </summary>
        /// <param name = "offset">The offset to move to.</param>
        /// <param name = "seekOrigin">The SeekOrigin to use.</param>
        public void SeekTo(int offset, SeekOrigin seekOrigin)
        {
            BaseStream.Seek(offset, seekOrigin);
        }

        /// <summary>
        ///   Attempts to read an Image from the parent stream.
        /// </summary>
        /// <param name = "length">The size of the Image.</param>
        /// <returns>An Image</returns>
        public Image TryParseImage(int length)
        {
            MemoryStream stream = new MemoryStream(base.ReadBytes(length));
            Image img = null;
            try
            {
                img = Image.FromStream(stream);
            }
            catch
            {
            }
            stream.Dispose();
            return img;
        }
    }

    /// <summary>
    ///   The Writer class.
    /// </summary>
    public class X360Writer : BinaryWriter
    {
        /// <summary>
        ///   Determines if we should flip the endianness of written values.
        /// </summary>
        public bool FlipEndian { get; set; }

        /// <summary>
        ///   Initializes a new instance of the X360Writer class with the specified X360IO.BaseIO instance.
        /// </summary>
        public X360Writer(Stream stream, Encoding encoding, bool leaveOpen, bool flipEndian)
            : base(stream, encoding, leaveOpen)
        {
            FlipEndian = flipEndian;
        }

        public X360Writer(Stream stream)
            : base(stream)
        {

        }

        /// <summary>
        ///   Moves the streams position to the specified offset.
        /// </summary>
        /// <param name = "offset">The offset to move to.</param>
        public void SeekTo(int offset)
        {
            SeekTo(offset, SeekOrigin.Begin);
        }

        /// <summary>
        ///   Moves the streams position to the specified offset.
        /// </summary>
        /// <param name = "offset">The offset to move to.</param>
        public void SeekTo(long offset)
        {
            SeekTo((int)offset, SeekOrigin.Begin);
        }

        /// <summary>
        ///   Moves the streams position to the specified offset.
        /// </summary>
        /// <param name = "offset">The offset to move to.</param>
        public void SeekTo(uint offset)
        {
            SeekTo((int)offset, SeekOrigin.Begin);
        }

        /// <summary>
        ///   Moves the streams position to the specified offset.
        /// </summary>
        /// <param name = "offset">The offset to move to.</param>
        /// <param name = "seekOrigin">The SeekOrigin to use.</param>
        public void SeekTo(int offset, SeekOrigin seekOrigin)
        {
            BaseStream.Seek(offset, seekOrigin);
        }

        /// <summary>
        ///   Writes the specified string to the parent stream, in the ASCII format.
        /// </summary>
        /// <param name = "value">The string to write.</param>
        public override void Write(string value)
        {
            int length = value.Length;
            for (int i = 0; i < length; i++)
            {
                byte num3 = (byte)value[i];
                Write(num3);
            }
        }

        /// <summary>
        ///   Writes the specified double precision float to the parent stream.
        /// </summary>
        /// <param name = "value">The double precision float to write.</param>
        public override void Write(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (FlipEndian)
                Array.Reverse(bytes);
            
            base.Write(bytes);
        }

        /// <summary>
        ///   Writes the specified 16-bit integer to the parent stream.
        /// </summary>
        /// <param name = "value">The 16-bit integer to write.</param>
        public override void Write(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (FlipEndian)
                Array.Reverse(bytes);

            base.Write(bytes);
        }

        /// <summary>
        ///   Writes the specified 32-bit integer to the parent stream.
        /// </summary>
        /// <param name = "value">The 32-bit integer to write.</param>
        public override void Write(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (FlipEndian)
                Array.Reverse(bytes);
            
            base.Write(bytes);
        }

        public void WriteInt24(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            byte num = bytes[0];
            byte num2 = bytes[1];
            byte num3 = bytes[2];
            byte[] array = new[] { num, num2, num3 };
            if (FlipEndian)
                Array.Reverse(array);

            Write(array);
        }

        /// <summary>
        ///   Writes the specified 64-bit integer to the parent stream.
        /// </summary>
        /// <param name = "value">The 64-bit integer to write.</param>
        public override void Write(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (FlipEndian)
                Array.Reverse(bytes);

            base.Write(bytes);
        }

        /// <summary>
        ///   Writes the specified single precision float to the parent stream.
        /// </summary>
        /// <param name = "value">The single precision float to write.</param>
        public override void Write(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (FlipEndian)
                Array.Reverse(bytes);
            
            base.Write(bytes);
        }

        /// <summary>
        ///   Writes the specified unsigned 16-bit integer to the parent stream.
        /// </summary>
        /// <param name = "value">The unsigned 16-bit integer to write.</param>
        public override void Write(ushort value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (FlipEndian)
                Array.Reverse(bytes);

            base.Write(bytes);
        }

        /// <summary>
        ///   Writes the specified unsigned 32-bit integer to the parent stream.
        /// </summary>
        /// <param name = "value">The unsigned 32-bit integer to write.</param>
        public override void Write(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (FlipEndian)
                Array.Reverse(bytes);

            base.Write(bytes);
        }

        /// <summary>
        ///   Writes the specified unsigned 64-bit integer to the parent stream.
        /// </summary>
        /// <param name = "value">The unsigned 64-bit integer to write.</param>
        public override void Write(ulong value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (FlipEndian)
                Array.Reverse(bytes);

            base.Write(bytes);
        }

        /// <summary>
        ///   Writes the specified Image to the parent stream.
        /// </summary>
        /// <param name = "value">The Image to write.</param>
        /// <param name = "imageFormat">The ImageFormat to use.</param>
        public void Write(Image value, ImageFormat imageFormat)
        {
            MemoryStream ms = new MemoryStream();
            value.Save(ms, imageFormat);
            byte[] bytes = ms.ToArray();
            ms.Dispose();
            base.Write(bytes);
        }

        /// <summary>
        ///   Writes the specified string to the parent stream in ASCII format, up to the specified length.
        /// </summary>
        /// <param name = "value">The string to write.</param>
        /// <param name = "length">The length to write.</param>
        public void WriteAsciiString(string value, int length)
        {
            int length1 = value.Length;
            for (int i = 0; i < length1; i++)
            {
                if (i > length)
                {
                    break;
                }
                byte num3 = (byte)value[i];
                Write(num3);
            }
            int num4 = length - length1;
            if (num4 > 0)
            {
                Write(new byte[num4]);
            }
        }

        /// <summary>
        ///   Writes the specified string to the parent stream in Unicode format, terminated with two null bytes.
        /// </summary>
        /// <param name = "value">The string to write.</param>
        public void WriteNullTerminatedUnicodeString(string value)
        {
            int strLen = value.Length;
            for (int x = 0; x < strLen; x++)
            {
                ushort val = value[x];
                Write(val);
            }
            Write((ushort)0);
        }

        /// <summary>
        ///   Writes the specified string to the parent stream in ASCII format, terminated with a null byte.
        /// </summary>
        /// <param name = "value">The string to write.</param>
        public void WriteNullTerminatedAsciiString(string value)
        {
            Write(value.ToCharArray());
            Write((byte)0);
        }

        /// <summary>
        ///   Writes the specified string to the parent stream in Unicode format, up to the specified length.
        /// </summary>
        /// <param name = "value">The string to write.</param>
        /// <param name = "length">The length to write.</param>
        public void WriteUnicodeString(string value, int length)
        {
            int length1 = value.Length;
            for (int i = 0; i < length1; i++)
            {
                if (i > length)
                {
                    break;
                }
                ushort num3 = value[i];
                Write(num3);
            }
            int num4 = (length - length1) * 2;
            if (num4 > 0)
            {
                Write(new byte[num4]);
            }
        }
    }
}
