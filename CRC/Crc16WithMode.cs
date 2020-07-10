using System;

namespace HexToTypeConverter.CRC
{
    public class Crc16WithMode
    {
        public enum Crc16ModeIBM : ushort { Normal = 0x8005, Reversed = 0xA001,  ReversedReciprocal = 0xC002 }
        public enum Crc16ModeCCITT : ushort { Normal = 0x1021, Reversed = 0x8408, ReversedReciprocal = 0x8810 }
        public enum Crc16ModeT10DIF : ushort { Normal = 0x8BB7, Reversed = 0xEDD1, ReversedReciprocal = 0xC5DB }
        public enum Crc16ModeDNP : ushort { Normal = 0x3D65, Reversed = 0xA6BC, ReversedReciprocal = 0x9EB2 }
        public enum Crc16ModeDECT : ushort { Normal = 0x0589, Reversed = 0x91A0, ReversedReciprocal = 0x82C4 }

        readonly ushort[] table = new ushort[256];

        public ushort ComputeChecksum(params byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(params byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            return BitConverter.GetBytes(crc);
        }

        public Crc16WithMode(ushort mode)
        {
            ushort polynomial = (ushort)mode;
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
    }
}
