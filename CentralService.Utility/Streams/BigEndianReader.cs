using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Utility.Streams
{
    public class BigEndianReader : BinaryReader
    {
        public BigEndianReader(Stream BaseStream) : base(BaseStream) { }

        public short ReadInt16(bool BigEndian = true) => BigEndian ? BitConverter.ToInt16(base.ReadBytes(2).Reverse().ToArray(), 0) : base.ReadInt16();

        public ushort ReadUInt16(bool BigEndian = true) => BigEndian ? BitConverter.ToUInt16(base.ReadBytes(2).Reverse().ToArray(), 0) : base.ReadUInt16();

        public int ReadInt32(bool BigEndian = true) => BigEndian ? BitConverter.ToInt32(base.ReadBytes(4).Reverse().ToArray(), 0) : base.ReadInt32();

        public uint ReadUInt32(bool BigEndian = true) => BigEndian ? BitConverter.ToUInt32(base.ReadBytes(4).Reverse().ToArray(), 0) : base.ReadUInt32();

        public long ReadInt64(bool BigEndian = true) => BigEndian ? BitConverter.ToInt64(base.ReadBytes(8).Reverse().ToArray(), 0) : base.ReadInt64();

        public ulong ReadUInt64(bool BigEndian = true) => BigEndian ? BitConverter.ToUInt64(base.ReadBytes(8).Reverse().ToArray(), 0) : base.ReadUInt64();

        public override string ReadString()
        {
            List<byte> Buffer = new List<byte>();
            byte CurrentByte = 0;
            while ((CurrentByte = base.ReadByte()) > 0)
                Buffer.Add(CurrentByte);
            return Encoding.UTF8.GetString(Buffer.ToArray());
        }

        public string ReadString(int Length) => Encoding.UTF8.GetString(base.ReadBytes(Length));
    }
}
