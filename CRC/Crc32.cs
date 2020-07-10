using System;

namespace HexToTypeConverter.CRC
{
    public class Crc32
    {
        uint[] table;

        public enum StandardMode : uint { Normal = 0x04C11DB7, Reversed = 0xEDB88320, ReversedReciprocal = 0x82608EDB };
        public enum CastagnoliMode : uint { Normal = 0x1EDC6F41, Reversed = 0x82F63B78, ReversedReciprocal = 0x8F6E37A0 };
        public enum KoopmanMode : uint { Normal = 0x741B8CD7, Reversed = 0xEB31D82E, ReversedReciprocal = 0xBA0DC66B };
        public enum QMode : uint { Normal = 0x814141AB, Reversed = 0xD5828281, ReversedReciprocal = 0xC0A0A0D5 };

        public uint ComputeChecksum(byte[] bytes)
        {
            uint crc = 0xffffffff;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
                crc = (uint)((crc >> 8) ^ table[index]);
            }
            return ~crc;
        }

        public byte[] ComputeChecksumBytes(byte[] bytes)
        {
            return BitConverter.GetBytes(ComputeChecksum(bytes));
        }

        public Crc32(uint mode)
        {
            table = new uint[256];
            uint temp = 0;
            for (uint i = 0; i < table.Length; ++i)
            {
                temp = i;
                for (int j = 8; j > 0; --j)
                {
                    if ((temp & 1) == 1)
                    {
                        temp = (uint)((temp >> 1) ^ (uint)mode);
                    }
                    else
                    {
                        temp >>= 1;
                    }
                }
                table[i] = temp;
            }
        }
    }
}
