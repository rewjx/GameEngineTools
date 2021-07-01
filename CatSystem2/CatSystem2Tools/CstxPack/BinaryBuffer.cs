using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CstxPack
{
    public class BinaryBuffer
    {

        #region DataRegion
        /// <summary>
        /// for writing
        /// </summary>
        private List<byte> writeBuffer;

        /// <summary>
        /// for reading
        /// </summary>
        private byte[] readBuffer;

        public byte[] ReadBuffer
        {
            get
            {
                return readBuffer;
            }
        }

        public int currentPosition { get; private set; }

        public int remainLength
        {
            get
            {
                if (this.readBuffer == null)
                    return 0;
                return this.readBuffer.Length - currentPosition;
            }
        }

        public enum CompressionType
        {
            Raw,
            zlib
        }

        #endregion

        public void Clear()
        {
            if (this.writeBuffer == null)
            {
                this.writeBuffer = new List<byte>();
            }
            else
            {
                this.writeBuffer.Clear();
            }
            this.readBuffer = new byte[0];
            this.currentPosition = 0;
        }

        public BinaryBuffer(byte[] data)
        {
            this.Clear();
            this.readBuffer = data;
        }

        public BinaryBuffer(string path)
        {
            this.Clear();
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    this.readBuffer = new byte[fs.Length];
                    fs.Read(readBuffer, 0, (int)fs.Length);
                }
            }
            catch { }

        }

        public BinaryBuffer()
        {
            this.Clear();
        }

        public byte ReadByte()
        {
            if(this.currentPosition < this.readBuffer.Length)
            {
                byte b = this.readBuffer[this.currentPosition];
                this.currentPosition += 1;
                return b;
            }
            else
            {
                throw new OutOfMemoryException();
            }
        }

        public int ReadInt()
        {
            if (this.currentPosition + 4 <= this.readBuffer.Length)
            {
                int num = this.readBuffer[this.currentPosition];
                num = num | (this.readBuffer[this.currentPosition + 1] << 8);
                num = num | (this.readBuffer[this.currentPosition + 2] << 16);
                num = num | (this.readBuffer[this.currentPosition + 3] << 24);
                this.currentPosition += 4;
                return num;
            }
            else
            {
                throw new OutOfMemoryException();
            }
        }

        public int ReadLength()
        {
            int num = 0;
            int shift = 0;
            while(true)
            {
                if(shift >= 32)
                {
                    return -1;
                }
                byte b = this.ReadByte();
                num |= ((b & 127) << shift);
                shift += 7;
                if(b < 128)
                {
                    return num;
                }
            }
        }

        public string ReadString(Encoding encoding)
        {
            int len = this.ReadLength();
            if (len == 0)
                return "";
            else if(len < 0 || this.remainLength < len)
            {
                throw new Exception("Read String Error!");
            }
            string str = encoding.GetString(this.readBuffer, this.currentPosition, len);
            this.currentPosition += len;
            return str;
        }

        public void WriteInt(int data)
        {
            byte b1 = (byte)(data & 0xff);
            byte b2 = (byte)((data >> 8) & 0xff);
            byte b3 = (byte)((data >> 16) & 0xff);
            byte b4 = (byte)((data >> 24) & 0xff);
            this.writeBuffer.Add(b1);
            this.writeBuffer.Add(b2);
            this.writeBuffer.Add(b3);
            this.writeBuffer.Add(b4);
        }

        public void WriteLength(int len)
        {
            uint num = (uint)len;
            byte b;
            do
            {
                b = (byte)(num & 127U);
                num >>= 7;
                b |= ((num != 0U) ? (byte)128 : (byte)0);
                this.writeBuffer.Add(b);
            }
            while (b >= 128);
        }

        public void WriteData(ICollection<byte> data)
        {
            this.WriteLength(data.Count);
            this.writeBuffer.AddRange(data);
        }

        public void WriteString(string str, Encoding encoding)
        {
            if(string.IsNullOrEmpty(str))
            {
                this.WriteLength(0);
            }
            else
            {
                byte[] bytes = encoding.GetBytes(str);
                this.WriteData(bytes);
            }
        }

        public byte[] GetWriteBufferData()
        {
            return this.writeBuffer.ToArray();
        }

    }
}
