using System;

namespace HexToTypeConverter.CRC
{
    public class CheckSumResult
    {
        public byte[] Values { get; set; }
        public string Type { get; set; }
        public string EndianType { get; set; }
        public string HexValues { get; set; }

        public CheckSumResult(byte value, string type, string endianType)
        {
            Values = new byte[] { value };
            Type = type;
            EndianType = endianType;

            HexValues = BitConverter.ToString(Values).Replace("-", " ");
        }
        public CheckSumResult(byte[] values, string type, string endianType)
        {
            Values = values;
            Type = type;
            EndianType = endianType;

            HexValues = BitConverter.ToString(values).Replace("-", " ");
        }
    }
}
