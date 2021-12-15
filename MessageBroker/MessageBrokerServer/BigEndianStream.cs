using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Linq;

namespace MessageBrokerServer
{
    public class BigEndianStream
    {
        public Stream Stream { get; private set; }
        public bool IsNetworkStream { get; private set; }

        public BigEndianStream(Stream stream)
        {
            this.Stream = stream;
            this.IsNetworkStream = stream is NetworkStream;
        }

        private void write(byte[] buffer)
        {
            this.Stream.Write(buffer, 0, buffer.Length);
            this.Stream.Flush();
        }

        private byte[] read(int length)
        {
            byte[] buffer = new byte[length];
            int read = 0;
            while (read < length)
            {
                read += this.Stream.Read(buffer, read, length - read);
            }
            return buffer;
        }

        public void WriteInt32(int value)
        {
            write(Int32ToBytes(value));
        }
        public void WriteInt16(short value)
        {
            write(Int16ToBytes(value));
        }
        public int ReadInt32()
        {
            return BytesToInt32(read(4));
        }
        public short ReadInt16()
        {
            return BytesToInt16(read(2));
        }

        public void WriteMiniBuffer(byte[] buffer)
        {
            this.Stream.WriteByte((byte)buffer.Length);
            this.write(buffer);
        }
        public byte[] ReadMiniBuffer()
        {
            int length = this.Stream.ReadByte();
            return this.read(length);
        }
        public void WriteSmallBuffer(byte[] buffer)
        {
            this.WriteInt16((short)buffer.Length);
            this.write(buffer);
        }
        public byte[] ReadSmallBuffer()
        {
            int length = this.ReadInt16();
            return this.read(length);
        }
        public void WriteBuffer(byte[] buffer)
        {
            this.WriteInt32(buffer.Length);
            this.write(buffer);
        }
        public byte[] ReadBuffer()
        {
            int length = this.ReadInt32();
            return this.read(length);
        }

        public void WriteString(string value)
        {
            this.WriteSmallBuffer(Encoding.UTF8.GetBytes(value));
        }
        public string ReadString()
        {
            return Encoding.UTF8.GetString(this.ReadSmallBuffer());
        }
        public void WriteShortString(string value)
        {
            this.WriteMiniBuffer(Encoding.UTF8.GetBytes(value));
        }
        public string ReadShortString()
        {
            return Encoding.UTF8.GetString(this.ReadMiniBuffer());
        }


        public void WriteEncrypt(byte[] data, byte[] key, byte[] iv)
        {
            this.WriteInt32(data.Length);
            data = KeyStore.Encrypt(data, key, iv);
            this.WriteBuffer(data);
        }
        public byte[] ReadDecrypt(byte[] key, byte[] iv)
        {
            int l = this.ReadInt32();
            byte[] data = KeyStore.Decrypt(this.ReadBuffer(), key, iv);
            byte[] result = new byte[l];
            for (int i = 0; i < l; i++)
                result[i] = data[i];
            return result;
        }

        public static byte[] Int32ToBytes(int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                ReverseBytes(data);
            return data;
        }
        public static int BytesToInt32(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                ReverseBytes(data);
            return BitConverter.ToInt32(data);
        }
        public static byte[] Int16ToBytes(short value)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
                ReverseBytes(data);
            return data;
        }
        public static short BytesToInt16(byte[] data)
        {
            if (BitConverter.IsLittleEndian)
                ReverseBytes(data);
            return BitConverter.ToInt16(data);
        }

        public static void ReverseBytes(byte[] buffer)
        {
            int i2;
            byte b;
            for (int i = 0; i < buffer.Length / 2; i++)
            {
                i2 = buffer.Length - 1 - i;
                b = buffer[i];
                buffer[i] = buffer[i2];
                buffer[i2] = b;
            }
        }
    }
}
