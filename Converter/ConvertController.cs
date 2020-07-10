using System;
using System.Collections.Generic;
using System.Linq;

namespace HexToTypeConverter.Converter
{
    public class ConvertController
    {
        public List<byte> CheckListLengthMultipleOf(List<byte> hexDataList, int multipleValue)
        {
            List<byte> newDataList = hexDataList.Select(x => x).ToList();

            int remainingValue = newDataList.Count % multipleValue;
            for (int i = 0; i < remainingValue; i++)
            {
                newDataList.Add(0);
            }

            return newDataList;
        }

        public List<HexToBinaryResult> ConvertHexToBinaryList(List<byte> hexDataList)
        {
            List<HexToBinaryResult> list = new List<HexToBinaryResult>();

            HexToBinaryResult hexToBinaryResult;
            byte hexDataFirst, hexDataSecond;
            string binaryTextFirst, hexTextFirst;
            string binaryTextSecond, hexTextSecond;
            
            for (int i = 0; i <= hexDataList.Count - 2; i = i + 2)
            {
                hexDataFirst = hexDataList[i];
                hexDataSecond = hexDataList[i + 1];
                
                hexTextFirst = string.Format("{0:X2}", hexDataFirst);
                binaryTextFirst = Convert.ToString(hexDataFirst, 2).PadLeft(8, '0');

                hexTextSecond = string.Format("{0:X2}", hexDataSecond);
                binaryTextSecond = Convert.ToString(hexDataSecond, 2).PadLeft(8, '0');

                hexToBinaryResult = new HexToBinaryResult(hexTextFirst + " " + hexTextSecond, binaryTextFirst + " " + binaryTextSecond);
                list.Add(hexToBinaryResult);
            }

            return list;
        }

        public float ConvertToSingle(byte[] hexData, string format, out byte[] orderedHexData)
        {
            orderedHexData = OrderByteArray(hexData, format);
            Array.Copy(orderedHexData, 0, hexData, 0, hexData.Length);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexData);

            float value = BitConverter.ToSingle(hexData, 0);

            return value;
        }

        public UInt32 ConvertToUInt32(byte[] hexData, string format, out byte[] orderedHexData)
        {
            orderedHexData = OrderByteArray(hexData, format);
            Array.Copy(orderedHexData, 0, hexData, 0, hexData.Length);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexData);

            UInt32 value = BitConverter.ToUInt32(hexData, 0);

            return value;
        }

        public Int32 ConvertToInt32(byte[] hexData, string format, out byte[] orderedHexData)
        {
            orderedHexData = OrderByteArray(hexData, format);
            Array.Copy(orderedHexData, 0, hexData, 0, hexData.Length);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexData);

            Int32 value = BitConverter.ToInt32(hexData, 0);

            return value;
        }

        public UInt16 ConvertToUInt16(byte[] hexData, string format, out byte[] orderedHexData)
        {
            orderedHexData = OrderByteArray(hexData, format);
            Array.Copy(orderedHexData, 0, hexData, 0, hexData.Length);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexData);

            UInt16 value = BitConverter.ToUInt16(hexData, 0);

            return value;
        }
        public Int16 ConvertToInt16(byte[] hexData, string format, out byte[] orderedHexData)
        {
            orderedHexData = OrderByteArray(hexData, format);
            Array.Copy(orderedHexData, 0, hexData, 0, hexData.Length);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexData);

            Int16 value = BitConverter.ToInt16(hexData, 0);

            return value;
        }

        public double ConvertToDouble(byte[] hexData, string format, out byte[] orderedHexData)
        {
            orderedHexData = OrderByteArray(hexData, format);
            Array.Copy(orderedHexData, 0, hexData, 0, hexData.Length);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(hexData);

            double value = BitConverter.ToDouble(hexData, 0);

            return value;
        }

        public byte[] OrderByteArray(byte[] hexData, string format)
        {
            byte[] hexDataNew = new byte[hexData.Length];
            switch (format)
            {
                case "ABCD":
                    Array.Copy(hexData, 0, hexDataNew, 0, hexData.Length);
                    break;
                case "BADC":
                    #region Byte Swap
                    if (hexData.Length == 4)
                    {
                        hexDataNew[0] = hexData[1];
                        hexDataNew[1] = hexData[0];
                        hexDataNew[2] = hexData[3];
                        hexDataNew[3] = hexData[2];
                    }
                    else if (hexData.Length == 8)
                    {
                        hexDataNew[0] = hexData[5];
                        hexDataNew[1] = hexData[4];
                        hexDataNew[2] = hexData[7];
                        hexDataNew[3] = hexData[6];

                        hexDataNew[4] = hexData[1];
                        hexDataNew[5] = hexData[0];
                        hexDataNew[6] = hexData[3];
                        hexDataNew[7] = hexData[2];
                    }
                    #endregion
                    break;
                case "CDAB":
                    #region Word Swap
                    if (hexData.Length == 4)
                    {
                        hexDataNew[0] = hexData[2];
                        hexDataNew[1] = hexData[3];
                        hexDataNew[2] = hexData[0];
                        hexDataNew[3] = hexData[1];
                    }
                    else if (hexData.Length == 8)
                    {
                        hexDataNew[0] = hexData[6];
                        hexDataNew[1] = hexData[7];
                        hexDataNew[2] = hexData[4];
                        hexDataNew[3] = hexData[5];

                        hexDataNew[4] = hexData[2];
                        hexDataNew[5] = hexData[3];
                        hexDataNew[6] = hexData[0];
                        hexDataNew[7] = hexData[1];
                    }
                    #endregion
                    break;
                case "DCBA":
                    #region Byte & Word Swap => Reverse
                    if (hexData.Length == 4)
                    {
                        hexDataNew[0] = hexData[3];
                        hexDataNew[1] = hexData[2];
                        hexDataNew[2] = hexData[1];
                        hexDataNew[3] = hexData[0];
                    }
                    else if (hexData.Length == 8)
                    {
                        hexDataNew[0] = hexData[7];
                        hexDataNew[1] = hexData[6];
                        hexDataNew[2] = hexData[5];
                        hexDataNew[3] = hexData[4];

                        hexDataNew[4] = hexData[3];
                        hexDataNew[5] = hexData[2];
                        hexDataNew[6] = hexData[1];
                        hexDataNew[7] = hexData[0];
                    }
                    #endregion
                    break;

                case "AB":
                    Array.Copy(hexData, 0, hexDataNew, 0, hexData.Length);
                    break;
                case "BA":
                    #region Byte Swap
                    hexDataNew[0] = hexData[1];
                    hexDataNew[1] = hexData[0];
                    #endregion
                    break;
            }

            return hexDataNew;
        }


        public DateTime ConvertEpochToDatetime(long epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epoch);
        }
    }
}
