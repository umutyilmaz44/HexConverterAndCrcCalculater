using System;

namespace HexToTypeConverter.CRC
{
    public class CheckSumController
    {
        public byte CheckSum8Xor(byte[] packetData, int PacketLength)
        {
            Byte _CheckSumByte = 0x00;
            for (int i = 0; i < PacketLength; i++)
                _CheckSumByte ^= packetData[i];
            return _CheckSumByte;
        }
        public byte CheckSum8Modulo256(byte[] packetData, int PacketLength)
        {
            Byte _CheckSumByte = 0x00;
            for (int i = 0; i < PacketLength; i++)
                _CheckSumByte += packetData[i];

            return _CheckSumByte;
        }
        public byte CheckSum82sComplement(byte[] packetData, int PacketLength)
        {
            byte complement = 255;
            byte _CheckSumByte = 0x00;
            for (int i = 0; i < PacketLength; i++)
                _CheckSumByte += packetData[i];

            _CheckSumByte = (byte)(complement - _CheckSumByte + 1);

            return _CheckSumByte;
        }
        public ushort CheckSumCrc16(byte[] packetData, int PacketLength)
        {
            Crc16 crc16 = new Crc16();

            byte[] newData = new byte[PacketLength];
            Array.Copy(packetData, 0, newData, 0, PacketLength);
            ushort value = crc16.CalculateCrc(newData);

            return value;
        }

        public ushort CheckSumCrc16Mode(byte[] packetData, int PacketLength, ushort crc16Mode)
        {
            Crc16WithMode crc16WithMode = new Crc16WithMode(crc16Mode);

            byte[] newData = new byte[PacketLength];
            Array.Copy(packetData, 0, newData, 0, PacketLength);
            ushort value = crc16WithMode.ComputeChecksum(newData);

            return value;
        }

        public uint CheckSumCrc32(byte[] packetData, int PacketLength, uint polyMode)
        {
            Crc32 crc32 = new Crc32(polyMode);

            byte[] newData = new byte[PacketLength];
            Array.Copy(packetData, 0, newData, 0, PacketLength);
            uint value = crc32.ComputeChecksum(newData);

            return value;
        }
    }
}
