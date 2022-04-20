using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CentralService.Utility.Streams
{
    public class BigEndianWriter : BinaryWriter
    {
        public BigEndianWriter(Stream BaseStream) : base(BaseStream) { }

        public void Write(byte value) => base.Write(value);

        public void Write(short value, bool BigEndian = true)
        {
            if (BigEndian)
                base.Write(BitConverter.GetBytes(value).Reverse().ToArray()); 
            else 
                base.Write(value);
        }
       
        public void Write(ushort value, bool BigEndian = true)
        {
            if (BigEndian)
                base.Write(BitConverter.GetBytes(value).Reverse().ToArray());
            else
                base.Write(value);
        }

        public void Write(int value, bool BigEndian = true)
        {
            if (BigEndian)
                base.Write(BitConverter.GetBytes(value).Reverse().ToArray());
            else
                base.Write(value);
        }

        public void Write(uint value, bool BigEndian = true)
        {
            if (BigEndian)
                base.Write(BitConverter.GetBytes(value).Reverse().ToArray());
            else
                base.Write(value);
        }

        public void Write(long value, bool BigEndian = true)
        {
            if (BigEndian)
                base.Write(BitConverter.GetBytes(value).Reverse().ToArray());
            else
                base.Write(value);
        }

        public void Write(ulong value, bool BigEndian = true)
        {
            if (BigEndian)
                base.Write(BitConverter.GetBytes(value).Reverse().ToArray());
            else
                base.Write(value);
        }

    }
}
