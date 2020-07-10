namespace HexToTypeConverter.Converter
{
    public class HexToBinaryResult
    {
        public string HexData { get; set; }
        public string BinaryData { get; set; }

        public HexToBinaryResult(string hexData, string binaryData)
        {
            HexData = hexData;
            BinaryData = binaryData;
        }
    }
}
