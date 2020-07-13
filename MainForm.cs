using HexToTypeConverter.Converter;
using HexToTypeConverter.CRC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace HexToTypeConverter
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetFormComponentSize();
            
            string epochTime = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            txtEpochTimeToHumanDate.Text = epochTime;

            DateTime dtimeNow = DateTime.Now;
            txtHumanDateYear.Text = dtimeNow.Year.ToString("00");
            txtHumanDateMonth.Text = dtimeNow.Month.ToString("00");
            txtHumanDateDay.Text = dtimeNow.Day.ToString("00");

            txtHumanDateHour.Text = dtimeNow.Hour.ToString("00");
            txtHumanDateMinute.Text = dtimeNow.Minute.ToString("00");
            txtHumanDateSecond.Text = dtimeNow.Second.ToString("00");

            cmbTimeKind.SelectedIndex = 0;

            ConvertTimestampToHumanDate();

            ConvertHumanDateToTimestamp();

            lblHexStats.Text = "";
            lblHexStatsValue.Text = "";
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            SetFormComponentSize();
        }

        private void btnAnalyzeData_Click(object sender, EventArgs e)
        {
            ConvertController convertController = new ConvertController();

            string hexStats = "";
            string hexStatsValue = "";

            #region Validation Control
            string hexDataText = txtHexData.Text.Trim().Replace(" ","").Replace("-", "");
            List<string> hexList = new List<string>();

            hexDataText = hexDataText.Length % 2 == 0 ? hexDataText : "0" + hexDataText;
            string hex;
            for (int i = 0; i <= hexDataText.Length - 2; i = i + 2)
            {
                hex = hexDataText.Substring(i, 2);
                hexList.Add(hex);
            }

            byte hexData;
            List<byte> rawDataList = new List<byte>();
            for (int i = 0; i < hexList.Count; i++)
            {
                if(!byte.TryParse(hexList[i], System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out hexData))
                {
                    rawDataList = new List<byte>();
                    MessageBox.Show("Hex Data value not valid! (" + hexList[i] + ")", "Data Error");
                    break;
                }

                rawDataList.Add(hexData);
            }

            hexStats += "Byte\r\n";
            hexStatsValue += ": "+ rawDataList.Count + "\r\n";
            #endregion

            #region Hex To Data Type
            List<byte> hexDataList;
            hexDataList = convertController.CheckListLengthMultipleOf(rawDataList, 2);
            
            hexStats += "Register\r\n";
            hexStatsValue += ": " + (hexDataList.Count / 2) + "\r\n";

            ConvertHexToAscii(hexDataList);
            ConvertHexToBinary(hexDataList);

            ConvertHexToValue("Int16", 1, hexDataList);
            ConvertHexToValue("Int16", 2, hexDataList);

            ConvertHexToValue("UInt16", 1, hexDataList);
            ConvertHexToValue("UInt16", 2, hexDataList);

            hexDataList = convertController.CheckListLengthMultipleOf(rawDataList, 4);

            hexStats += "Word\r\n";
            hexStatsValue += ": " + (hexDataList.Count / 4) + "\r\n";

            ConvertHexToValue("Float", 1, hexDataList);
            ConvertHexToValue("Float", 2, hexDataList);
            ConvertHexToValue("Float", 3, hexDataList);
            ConvertHexToValue("Float", 4, hexDataList);
            
            ConvertHexToValue("UInt32", 1, hexDataList);
            ConvertHexToValue("UInt32", 2, hexDataList);
            ConvertHexToValue("UInt32", 3, hexDataList);
            ConvertHexToValue("UInt32", 4, hexDataList);

            ConvertHexToValue("Int32", 1, hexDataList);
            ConvertHexToValue("Int32", 2, hexDataList);
            ConvertHexToValue("Int32", 3, hexDataList);
            ConvertHexToValue("Int32", 4, hexDataList);

            hexDataList = convertController.CheckListLengthMultipleOf(rawDataList, 8);

            ConvertHexToValue("Double", 1, hexDataList);
            ConvertHexToValue("Double", 2, hexDataList);
            ConvertHexToValue("Double", 3, hexDataList);
            ConvertHexToValue("Double", 4, hexDataList);
            #endregion

            #region Calculate CheckSum 
            CalculateCheckSum("CheckSum8Xor", rawDataList);
            CalculateCheckSum("CheckSum8Modulo256", rawDataList);
            CalculateCheckSum("CheckSum82sComplement", rawDataList);

            CalculateCheckSum("CheckSumCrc16", rawDataList);
            CalculateCheckSum("CheckSumCrc16CCITT", rawDataList);
            CalculateCheckSum("CheckSumCRC16IBM", rawDataList);
            
            CalculateCheckSum("CheckSumCRC16T10DIF", rawDataList);
            CalculateCheckSum("CheckSumCRC16DNP", rawDataList);
            CalculateCheckSum("CheckSumCRC16DECT", rawDataList);

            CalculateCheckSum("CheckSumCrc32", rawDataList);
            CalculateCheckSum("CheckSumCrc32CCastagnoli", rawDataList);
            CalculateCheckSum("CheckSumCrc32KKoopman", rawDataList);
            CalculateCheckSum("CheckSumCrc32Q", rawDataList);
            #endregion

            lblHexStats.Text = hexStats;
            lblHexStatsValue.Text = hexStatsValue;
        }

        private void SetFormComponentSize()
        {
            int width = tabPage1.Width > tabPage2.Width ? tabPage1.Width : tabPage2.Width;
            int height = tabPage1.Height > tabPage2.Height ? tabPage1.Height : tabPage2.Height;

            tabPage1.Width = tabPage2.Width = width;
            tabPage1.Height = tabPage2.Height = height;

            Application.DoEvents();

            gboxAscii.Width = (flpnlConvertList.Width / 2) - 10;
            gboxBinary.Width = (flpnlConvertList.Width / 2) - 10;

            gboxFloatBigEndianABCD.Width = (flpnlFloat.Width / 4) - 10;
            gboxFloatLittleEndianDCBA.Width = (flpnlFloat.Width / 4) - 10;
            gboxFloatMidBigEndianBADC.Width = (flpnlFloat.Width / 4) - 10;
            gboxFloatMidLittleEndianCDAB.Width = (flpnlFloat.Width / 4) - 10;

            gboxUInt32BigEndianABCD.Width = (flpnlUInt32.Width / 4) - 10;
            gboxUInt32LittleEndianDCBA.Width = (flpnlUInt32.Width / 4) - 10;
            gboxUInt32MidBigEndianBADC.Width = (flpnlUInt32.Width / 4) - 10;
            gboxUInt32MidLittleEndianCDAB.Width = (flpnlUInt32.Width / 4) - 10;

            gboxInt32BigEndianABCD.Width = (flpnlInt32.Width / 4) - 10;
            gboxInt32LittleEndianDCBA.Width = (flpnlInt32.Width / 4) - 10;
            gboxInt32MidBigEndianBADC.Width = (flpnlInt32.Width / 4) - 10;
            gboxInt32MidLittleEndianCDAB.Width = (flpnlInt32.Width / 4) - 10;

            gboxInt16BigEndianAB.Width = (flpnl16.Width / 4) - 10;
            gboxInt16LittleEndianBA.Width = (flpnl16.Width / 4) - 10;
            gboxUInt16BigEndianAB.Width = (flpnl16.Width / 4) - 10;
            gboxUInt16LittleEndianBA.Width = (flpnl16.Width / 4) - 10;

            gboxDoubleBigEndianABCD.Width = (flpnlDouble.Width / 4) - 10;
            gboxDoubleLittleEndianDCBA.Width = (flpnlDouble.Width / 4) - 10;
            gboxDoubleMidBigEndianBADC.Width = (flpnlDouble.Width / 4) - 10;
            gboxDoubleMidLittleEndianCDAB.Width = (flpnlDouble.Width / 4) - 10;

            gboxCheckSum8Xor.Width = (fpnlCrc1.Width / 3) - 15;
            gboxCheckSum8Modulo256.Width = (fpnlCrc1.Width / 3) - 15;
            gboxCheckSum82sComplement.Width = (fpnlCrc1.Width / 3) - 15;

            gboxCRC16.Width = (fpnlCrc2.Width / 3) - 15;
            gboxCRC16CCITT.Width = (fpnlCrc2.Width / 3) - 15;
            gboxCRC16IBM.Width = (fpnlCrc2.Width / 3) - 15;

            gboxCRC16T10DIF.Width = (fpnlCrc3.Width / 3) - 15;
            gboxCRC16DNP.Width = (fpnlCrc3.Width / 3) - 15;
            gboxCRC16DECT.Width = (fpnlCrc3.Width / 3) - 15;

            gboxCRC32.Width = (fpnlCrc4.Width / 3) - 15;
            gboxCR32CCastagnoli.Width = (fpnlCrc4.Width / 3) - 15;
            gboxCRC32KKoopman.Width = (fpnlCrc4.Width / 3) - 15;

            gboxCRC32Q.Width = (fpnlCrc5.Width / 3) - 15;
        }

        public void ConvertHexToAscii(List<byte> hexDataList)
        {
            txtAscii.Text = System.Text.Encoding.ASCII.GetString(hexDataList.ToArray());
        }

        public void ConvertHexToBinary(List<byte> hexDataList)
        {
            ConvertController convertController = new ConvertController();
            List<HexToBinaryResult> hexToBinaryResults = convertController.ConvertHexToBinaryList(hexDataList);

            int rowId = -1;
            dgvBinary.Rows.Clear();

            for (int i = 0; i < hexToBinaryResults.Count; i++)
            {   
                rowId = dgvBinary.Rows.Add();
                
                dgvBinary.Rows[rowId].Cells["dgvBinaryIndex"].Value = rowId;
                dgvBinary.Rows[rowId].Cells["dgvBinaryRaw"].Value = hexToBinaryResults[i].HexData;
                dgvBinary.Rows[rowId].Cells["dgvBinaryValue"].Value = hexToBinaryResults[i].BinaryData;
            }
        }

        public void CalculateCheckSum(string checkSumType, List<byte> hexDataList)
        {
            int rowId = -1;
            string preColumnName = "";
            byte value;
            
            DataGridView dataGridView = null;
            CheckSumController checkSumController = new CheckSumController();
            List<CheckSumResult> checkSumResultList = new List<CheckSumResult>();

            switch (checkSumType)
            {
                case "CheckSum8Xor":
                    #region CheckSum8 Xor
                    {
                        checkSumResultList.Clear();
                        value = checkSumController.CheckSum8Xor(hexDataList.ToArray(), hexDataList.Count);
                        checkSumResultList.Add(new CheckSumResult(value, "Normal", "CRC"));

                        preColumnName = "dgvCrc1x1_";
                        dataGridView = dgvCheckSum8Xor;
                        dataGridView.Rows.Clear();

                        rowId = dataGridView.Rows.Add();

                        foreach (CheckSumResult checkSumResult in checkSumResultList)
                        {
                            dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                            dataGridView.Rows[rowId].Cells[preColumnName + "CRC"].Value = checkSumResult.HexValues;
                        }
                    }
                    #endregion
                    break;
                case "CheckSum8Modulo256":
                    #region CheckSum8 Modulo 256
                    {
                        checkSumResultList.Clear();
                        value = checkSumController.CheckSum8Modulo256(hexDataList.ToArray(), hexDataList.Count);
                        checkSumResultList.Add(new CheckSumResult(value, "Normal", "CRC"));

                        preColumnName = "dgvCrc1x2_";
                        dataGridView = dgvCheckSum8Modulo256;
                        dataGridView.Rows.Clear();

                        rowId = dataGridView.Rows.Add();

                        foreach (CheckSumResult checkSumResult in checkSumResultList)
                        {
                            dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                            dataGridView.Rows[rowId].Cells[preColumnName + "CRC"].Value = checkSumResult.HexValues;
                        }
                    }
                    #endregion
                    break;
                case "CheckSum82sComplement":
                    #region CheckSum8 2s Complement
                    {
                        checkSumResultList.Clear();
                        value = checkSumController.CheckSum82sComplement(hexDataList.ToArray(), hexDataList.Count);
                        checkSumResultList.Add(new CheckSumResult(value, "Normal", "CRC"));

                        preColumnName = "dgvCrc1x3_";
                        dataGridView = dgvCheckSum82sComplement;
                        dataGridView.Rows.Clear();

                        rowId = dataGridView.Rows.Add();

                        foreach (CheckSumResult checkSumResult in checkSumResultList)
                        {
                            dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                            dataGridView.Rows[rowId].Cells[preColumnName + "CRC"].Value = checkSumResult.HexValues;
                        }
                    }
                    #endregion
                    break;
                case "CheckSumCrc16":
                    #region CheckSum Crc16
                    {
                        checkSumResultList.Clear();
                        ushort valueCrc = checkSumController.CheckSumCrc16(hexDataList.ToArray(), hexDataList.Count);
                        byte[] data, bigEndianData = new byte[2], littleEndianData = new byte[2];
                        data = BitConverter.GetBytes(valueCrc);

                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Copy(data, littleEndianData, data.Length);
                            Array.Copy(data, bigEndianData, data.Length);
                            Array.Reverse(littleEndianData);
                        }
                        else 
                        {
                            Array.Copy(data, littleEndianData, data.Length);
                            Array.Copy(data, bigEndianData, data.Length);
                            Array.Reverse(bigEndianData);
                        }

                        checkSumResultList.Add(new CheckSumResult(bigEndianData, "Normal", "Big Endian"));
                        checkSumResultList.Add(new CheckSumResult(littleEndianData, "Normal", "Little Endian"));

                        preColumnName = "dgvCrc2x1_";
                        dataGridView = dgvCRC16;
                        dataGridView.Rows.Clear();


                        var group = checkSumResultList.GroupBy(x => x.Type).ToList();
                        foreach (var groupInfo in group)
                        {
                            rowId = dataGridView.Rows.Add();
                            foreach (CheckSumResult checkSumResult in groupInfo.ToList())
                            {
                                dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                                if (checkSumResult.EndianType == "Big Endian")
                                    dataGridView.Rows[rowId].Cells[preColumnName + "BigEndian"].Value = checkSumResult.HexValues;
                                else
                                    dataGridView.Rows[rowId].Cells[preColumnName + "LittleEndian"].Value = checkSumResult.HexValues;
                            }
                        }
                            

                        
                    }
                    #endregion
                    break;
                case "CheckSumCrc16CCITT":
                    #region CheckSum Crc16 CCITT
                    {
                        ushort valueCrc;
                        byte[] data, bigEndianData = new byte[2], littleEndianData = new byte[2];
                        string typeName;
                        
                        preColumnName = "dgvCrc2x2_";
                        dataGridView = dgvCRC16CCITT;
                        dataGridView.Rows.Clear();
                        checkSumResultList.Clear();
                        Crc16WithMode.Crc16ModeCCITT[] crcModes = (Crc16WithMode.Crc16ModeCCITT[])Enum.GetValues(typeof(Crc16WithMode.Crc16ModeCCITT));
                        foreach (Crc16WithMode.Crc16ModeCCITT crcMode in crcModes)
                        {
                            typeName = Enum.GetName(typeof(Crc16WithMode.Crc16ModeCCITT), crcMode) + "-" + ((ushort)crcMode).ToString("X");
                            valueCrc = checkSumController.CheckSumCrc16Mode(hexDataList.ToArray(), hexDataList.Count, (ushort)crcMode);                            
                            data = BitConverter.GetBytes(valueCrc);

                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(littleEndianData);
                            }
                            else
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(bigEndianData);
                            }

                            checkSumResultList.Add(new CheckSumResult(bigEndianData, typeName, "Big Endian"));
                            checkSumResultList.Add(new CheckSumResult(littleEndianData, typeName, "Little Endian"));
                        }

                        checkSumResultList = checkSumResultList.OrderBy(x => x.Type).ToList();
                        var group = checkSumResultList.GroupBy(x => x.Type).ToList();
                        foreach (var groupInfo in group)
                        {
                            rowId = dataGridView.Rows.Add();
                            foreach (CheckSumResult checkSumResult in groupInfo.ToList())
                            {                                
                                dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                                if (checkSumResult.EndianType == "Big Endian")
                                    dataGridView.Rows[rowId].Cells[preColumnName + "BigEndian"].Value = checkSumResult.HexValues;
                                else
                                    dataGridView.Rows[rowId].Cells[preColumnName + "LittleEndian"].Value = checkSumResult.HexValues;
                            }
                        }
                    }
                    #endregion
                    break;
                case "CheckSumCRC16IBM":
                    #region CheckSum Crc16 IBM
                    {
                        ushort valueCrc;
                        byte[] data, bigEndianData = new byte[2], littleEndianData = new byte[2];
                        string typeName;

                        preColumnName = "dgvCrc2x3_";
                        dataGridView = dgvCRC16IBM;
                        dataGridView.Rows.Clear();
                        checkSumResultList.Clear();
                        Crc16WithMode.Crc16ModeIBM[] crcModes = (Crc16WithMode.Crc16ModeIBM[])Enum.GetValues(typeof(Crc16WithMode.Crc16ModeIBM));
                        foreach (Crc16WithMode.Crc16ModeIBM crcMode in crcModes)
                        {
                            typeName = Enum.GetName(typeof(Crc16WithMode.Crc16ModeIBM), crcMode) + "-" + ((ushort)crcMode).ToString("X");
                            valueCrc = checkSumController.CheckSumCrc16Mode(hexDataList.ToArray(), hexDataList.Count, (ushort)crcMode);
                            data = BitConverter.GetBytes(valueCrc);

                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(littleEndianData);
                            }
                            else
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(bigEndianData);
                            }

                            checkSumResultList.Add(new CheckSumResult(bigEndianData, typeName, "Big Endian"));
                            checkSumResultList.Add(new CheckSumResult(littleEndianData, typeName, "Little Endian"));
                        }

                        checkSumResultList = checkSumResultList.OrderBy(x => x.Type).ToList();
                        var group = checkSumResultList.GroupBy(x => x.Type).ToList();
                        foreach (var groupInfo in group)
                        {
                            rowId = dataGridView.Rows.Add();
                            foreach (CheckSumResult checkSumResult in groupInfo.ToList())
                            {
                                dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                                if (checkSumResult.EndianType == "Big Endian")
                                    dataGridView.Rows[rowId].Cells[preColumnName + "BigEndian"].Value = checkSumResult.HexValues;
                                else
                                    dataGridView.Rows[rowId].Cells[preColumnName + "LittleEndian"].Value = checkSumResult.HexValues;
                            }
                        }
                    }
                    #endregion
                    break;
                case "CheckSumCRC16T10DIF":
                    #region CheckSum Crc16 T10 DIF
                    {
                        ushort valueCrc;
                        byte[] data, bigEndianData = new byte[2], littleEndianData = new byte[2];
                        string typeName;

                        preColumnName = "dgvCrc3x1_";
                        dataGridView = dgvCRC16T10DIF;
                        dataGridView.Rows.Clear();
                        checkSumResultList.Clear();
                        Crc16WithMode.Crc16ModeT10DIF[] crcModes = (Crc16WithMode.Crc16ModeT10DIF[])Enum.GetValues(typeof(Crc16WithMode.Crc16ModeT10DIF));
                        foreach (Crc16WithMode.Crc16ModeT10DIF crcMode in crcModes)
                        {
                            typeName = Enum.GetName(typeof(Crc16WithMode.Crc16ModeT10DIF), crcMode) + "-" + ((ushort)crcMode).ToString("X");
                            valueCrc = checkSumController.CheckSumCrc16Mode(hexDataList.ToArray(), hexDataList.Count, (ushort)crcMode);
                            data = BitConverter.GetBytes(valueCrc);

                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(littleEndianData);
                            }
                            else
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(bigEndianData);
                            }

                            checkSumResultList.Add(new CheckSumResult(bigEndianData, typeName, "Big Endian"));
                            checkSumResultList.Add(new CheckSumResult(littleEndianData, typeName, "Little Endian"));
                        }

                        checkSumResultList = checkSumResultList.OrderBy(x => x.Type).ToList();
                        var group = checkSumResultList.GroupBy(x => x.Type).ToList();
                        foreach (var groupInfo in group)
                        {
                            rowId = dataGridView.Rows.Add();
                            foreach (CheckSumResult checkSumResult in groupInfo.ToList())
                            {
                                dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                                if (checkSumResult.EndianType == "Big Endian")
                                    dataGridView.Rows[rowId].Cells[preColumnName + "BigEndian"].Value = checkSumResult.HexValues;
                                else
                                    dataGridView.Rows[rowId].Cells[preColumnName + "LittleEndian"].Value = checkSumResult.HexValues;
                            }
                        }
                    }
                    #endregion
                    break;
                case "CheckSumCRC16DNP":
                    #region CheckSum Crc16 DNP
                    {
                        ushort valueCrc;
                        byte[] data, bigEndianData = new byte[2], littleEndianData = new byte[2];
                        string typeName;

                        preColumnName = "dgvCrc3x2_";
                        dataGridView = dgvCRC16DNP;
                        dataGridView.Rows.Clear();
                        checkSumResultList.Clear();
                        Crc16WithMode.Crc16ModeDNP[] crcModes = (Crc16WithMode.Crc16ModeDNP[])Enum.GetValues(typeof(Crc16WithMode.Crc16ModeDNP));
                        foreach (Crc16WithMode.Crc16ModeDNP crcMode in crcModes)
                        {
                            typeName = Enum.GetName(typeof(Crc16WithMode.Crc16ModeDNP), crcMode) + "-" + ((ushort)crcMode).ToString("X");
                            valueCrc = checkSumController.CheckSumCrc16Mode(hexDataList.ToArray(), hexDataList.Count, (ushort)crcMode);
                            data = BitConverter.GetBytes(valueCrc);

                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(littleEndianData);
                            }
                            else
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(bigEndianData);
                            }

                            checkSumResultList.Add(new CheckSumResult(bigEndianData, typeName, "Big Endian"));
                            checkSumResultList.Add(new CheckSumResult(littleEndianData, typeName, "Little Endian"));
                        }

                        checkSumResultList = checkSumResultList.OrderBy(x => x.Type).ToList();
                        var group = checkSumResultList.GroupBy(x => x.Type).ToList();
                        foreach (var groupInfo in group)
                        {
                            rowId = dataGridView.Rows.Add();
                            foreach (CheckSumResult checkSumResult in groupInfo.ToList())
                            {
                                dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                                if (checkSumResult.EndianType == "Big Endian")
                                    dataGridView.Rows[rowId].Cells[preColumnName + "BigEndian"].Value = checkSumResult.HexValues;
                                else
                                    dataGridView.Rows[rowId].Cells[preColumnName + "LittleEndian"].Value = checkSumResult.HexValues;
                            }
                        }
                    }
                    #endregion
                    break;
                case "CheckSumCRC16DECT":
                    #region CheckSum Crc16 DECT
                    {
                        ushort valueCrc;
                        byte[] data, bigEndianData = new byte[2], littleEndianData = new byte[2];
                        string typeName;

                        preColumnName = "dgvCrc3x3_";
                        dataGridView = dgvCRC16DECT;
                        dataGridView.Rows.Clear();
                        checkSumResultList.Clear();
                        Crc16WithMode.Crc16ModeDECT[] crcModes = (Crc16WithMode.Crc16ModeDECT[])Enum.GetValues(typeof(Crc16WithMode.Crc16ModeDECT));
                        foreach (Crc16WithMode.Crc16ModeDECT crcMode in crcModes)
                        {
                            typeName = Enum.GetName(typeof(Crc16WithMode.Crc16ModeDECT), crcMode) + "-" + ((ushort)crcMode).ToString("X");
                            valueCrc = checkSumController.CheckSumCrc16Mode(hexDataList.ToArray(), hexDataList.Count, (ushort)crcMode);
                            data = BitConverter.GetBytes(valueCrc);

                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(littleEndianData);
                            }
                            else
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(bigEndianData);
                            }

                            checkSumResultList.Add(new CheckSumResult(bigEndianData, typeName, "Big Endian"));
                            checkSumResultList.Add(new CheckSumResult(littleEndianData, typeName, "Little Endian"));
                        }

                        checkSumResultList = checkSumResultList.OrderBy(x => x.Type).ToList();
                        var group = checkSumResultList.GroupBy(x => x.Type).ToList();
                        foreach (var groupInfo in group)
                        {
                            rowId = dataGridView.Rows.Add();
                            foreach (CheckSumResult checkSumResult in groupInfo.ToList())
                            {
                                dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                                if (checkSumResult.EndianType == "Big Endian")
                                    dataGridView.Rows[rowId].Cells[preColumnName + "BigEndian"].Value = checkSumResult.HexValues;
                                else
                                    dataGridView.Rows[rowId].Cells[preColumnName + "LittleEndian"].Value = checkSumResult.HexValues;
                            }
                        }
                    }
                    #endregion
                    break;
                case "CheckSumCrc32":
                    #region CheckSum Crc32
                    {
                        uint valueCrc;
                        byte[] data, bigEndianData = new byte[4], littleEndianData = new byte[4];
                        string typeName;
                        
                        preColumnName = "dgvCrc4x1_";
                        dataGridView = dgvCRC32;
                        dataGridView.Rows.Clear();
                        checkSumResultList.Clear();
                        Crc32.StandardMode[] polyNodes = (Crc32.StandardMode[])Enum.GetValues(typeof(Crc32.StandardMode));

                        foreach (Crc32.StandardMode poly in polyNodes)
                        {
                            typeName = Enum.GetName(typeof(Crc32.StandardMode), poly) + "-" + ((uint)poly).ToString("X");
                            valueCrc = checkSumController.CheckSumCrc32(hexDataList.ToArray(), hexDataList.Count, (uint)poly);

                            data = BitConverter.GetBytes(valueCrc);

                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(littleEndianData);
                            }
                            else
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(bigEndianData);
                            }

                            checkSumResultList.Add(new CheckSumResult(bigEndianData, typeName, "Big Endian"));
                            checkSumResultList.Add(new CheckSumResult(littleEndianData, typeName, "Little Endian"));
                        }

                        checkSumResultList = checkSumResultList.OrderBy(x => x.Type).ToList();
                        var group = checkSumResultList.GroupBy(x => x.Type).ToList();
                        foreach (var groupInfo in group)
                        {
                            rowId = dataGridView.Rows.Add();
                            foreach (CheckSumResult checkSumResult in groupInfo.ToList())
                            {
                                dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                                if (checkSumResult.EndianType == "Big Endian")
                                    dataGridView.Rows[rowId].Cells[preColumnName + "BigEndian"].Value = checkSumResult.HexValues;
                                else
                                    dataGridView.Rows[rowId].Cells[preColumnName + "LittleEndian"].Value = checkSumResult.HexValues;
                            }
                        }
                    }
                    #endregion
                    break;
                case "CheckSumCrc32CCastagnoli":
                    #region CheckSum Crc32
                    {
                        uint valueCrc;
                        byte[] data, bigEndianData = new byte[4], littleEndianData = new byte[4];
                        string typeName;

                        preColumnName = "dgvCrc4x2_";
                        dataGridView = dgvCR32CCastagnoli;
                        dataGridView.Rows.Clear();
                        checkSumResultList.Clear();
                        Crc32.CastagnoliMode[] polyNodes = (Crc32.CastagnoliMode[])Enum.GetValues(typeof(Crc32.CastagnoliMode));

                        foreach (Crc32.CastagnoliMode poly in polyNodes)
                        {
                            typeName = Enum.GetName(typeof(Crc32.CastagnoliMode), poly) + "-" + ((uint)poly).ToString("X");
                            valueCrc = checkSumController.CheckSumCrc32(hexDataList.ToArray(), hexDataList.Count, (uint)poly);

                            data = BitConverter.GetBytes(valueCrc);

                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(littleEndianData);
                            }
                            else
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(bigEndianData);
                            }

                            checkSumResultList.Add(new CheckSumResult(bigEndianData, typeName, "Big Endian"));
                            checkSumResultList.Add(new CheckSumResult(littleEndianData, typeName, "Little Endian"));
                        }

                        checkSumResultList = checkSumResultList.OrderBy(x => x.Type).ToList();
                        var group = checkSumResultList.GroupBy(x => x.Type).ToList();
                        foreach (var groupInfo in group)
                        {
                            rowId = dataGridView.Rows.Add();
                            foreach (CheckSumResult checkSumResult in groupInfo.ToList())
                            {
                                dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                                if (checkSumResult.EndianType == "Big Endian")
                                    dataGridView.Rows[rowId].Cells[preColumnName + "BigEndian"].Value = checkSumResult.HexValues;
                                else
                                    dataGridView.Rows[rowId].Cells[preColumnName + "LittleEndian"].Value = checkSumResult.HexValues;
                            }
                        }
                    }
                    #endregion
                    break;
                case "CheckSumCrc32KKoopman":
                    #region CheckSum CheckSum Crc32K Koopman
                    {
                        uint valueCrc;
                        byte[] data, bigEndianData = new byte[4], littleEndianData = new byte[4];
                        string typeName;

                        preColumnName = "dgvCrc4x3_";
                        dataGridView = dgvCRC32KKoopman;
                        dataGridView.Rows.Clear();
                        checkSumResultList.Clear();
                        Crc32.KoopmanMode[] polyNodes = (Crc32.KoopmanMode[])Enum.GetValues(typeof(Crc32.KoopmanMode));

                        foreach (Crc32.KoopmanMode poly in polyNodes)
                        {
                            typeName = Enum.GetName(typeof(Crc32.KoopmanMode), poly) + "-" + ((uint)poly).ToString("X");
                            valueCrc = checkSumController.CheckSumCrc32(hexDataList.ToArray(), hexDataList.Count, (uint)poly);

                            data = BitConverter.GetBytes(valueCrc);

                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(littleEndianData);
                            }
                            else
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(bigEndianData);
                            }

                            checkSumResultList.Add(new CheckSumResult(bigEndianData, typeName, "Big Endian"));
                            checkSumResultList.Add(new CheckSumResult(littleEndianData, typeName, "Little Endian"));
                        }

                        checkSumResultList = checkSumResultList.OrderBy(x => x.Type).ToList();
                        var group = checkSumResultList.GroupBy(x => x.Type).ToList();
                        foreach (var groupInfo in group)
                        {
                            rowId = dataGridView.Rows.Add();
                            foreach (CheckSumResult checkSumResult in groupInfo.ToList())
                            {
                                dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                                if (checkSumResult.EndianType == "Big Endian")
                                    dataGridView.Rows[rowId].Cells[preColumnName + "BigEndian"].Value = checkSumResult.HexValues;
                                else
                                    dataGridView.Rows[rowId].Cells[preColumnName + "LittleEndian"].Value = checkSumResult.HexValues;
                            }
                        }
                    }
                    #endregion
                    break;
                case "CheckSumCrc32Q":
                    #region CheckSum CheckSum Crc32Q
                    {
                        uint valueCrc;
                        byte[] data, bigEndianData = new byte[4], littleEndianData = new byte[4];
                        string typeName;

                        preColumnName = "dgvCrc5x1_";
                        dataGridView = dgvCRC32Q;
                        dataGridView.Rows.Clear();
                        checkSumResultList.Clear();
                        Crc32.QMode[] polyNodes = (Crc32.QMode[])Enum.GetValues(typeof(Crc32.QMode));

                        foreach (Crc32.QMode poly in polyNodes)
                        {
                            typeName = Enum.GetName(typeof(Crc32.QMode), poly) + "-" + ((uint)poly).ToString("X");
                            valueCrc = checkSumController.CheckSumCrc32(hexDataList.ToArray(), hexDataList.Count, (uint)poly);

                            data = BitConverter.GetBytes(valueCrc);

                            if (BitConverter.IsLittleEndian)
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(littleEndianData);
                            }
                            else
                            {
                                Array.Copy(data, littleEndianData, data.Length);
                                Array.Copy(data, bigEndianData, data.Length);
                                Array.Reverse(bigEndianData);
                            }

                            checkSumResultList.Add(new CheckSumResult(bigEndianData, typeName, "Big Endian"));
                            checkSumResultList.Add(new CheckSumResult(littleEndianData, typeName, "Little Endian"));
                        }

                        checkSumResultList = checkSumResultList.OrderBy(x => x.Type).ToList();
                        var group = checkSumResultList.GroupBy(x => x.Type).ToList();
                        foreach (var groupInfo in group)
                        {
                            rowId = dataGridView.Rows.Add();
                            foreach (CheckSumResult checkSumResult in groupInfo.ToList())
                            {
                                dataGridView.Rows[rowId].Cells[preColumnName + "Type"].Value = checkSumResult.Type;
                                if (checkSumResult.EndianType == "Big Endian")
                                    dataGridView.Rows[rowId].Cells[preColumnName + "BigEndian"].Value = checkSumResult.HexValues;
                                else
                                    dataGridView.Rows[rowId].Cells[preColumnName + "LittleEndian"].Value = checkSumResult.HexValues;
                            }
                        }
                    }
                    #endregion
                    break;
            }            
        }

        public void ConvertHexToValue(string typeName, int gridOrder, List<byte> hexDataList)
        {
            int rowId = -1;
            object value=null;
            byte[] hexDataBlock = new byte[4];
            byte[] orderedHexDataBlock = new byte[4];
            string hexDataText = "", byteOrder="", preColumnName="";

            DataGridView dataGridView = null;
            ConvertController convertController = new ConvertController();

            switch (typeName)
            {
                case "Float":
                    switch (gridOrder)
                    {
                        case 1:
                            dataGridView = dgvFloatBigEndianABCD;
                            byteOrder = "ABCD";                            
                            break;
                        case 2:
                            dataGridView = dgvFloatLittleEndianDCBA;
                            byteOrder = "DCBA";
                            break;
                        case 3:
                            dataGridView = dgvFloatMidBigEndianBADC;
                            byteOrder = "BADC";
                            break;
                        case 4:
                            dataGridView = dgvFloatMidLittleEndianCDAB;
                            byteOrder = "CDAB";
                            break;
                    }
                    preColumnName = "dgvFloat" + gridOrder;
                    break;
                case "UInt32":
                    switch (gridOrder)
                    {
                        case 1:
                            dataGridView = dgvUInt32BigEndianABCD;
                            byteOrder = "ABCD";
                            break;
                        case 2:
                            dataGridView = dgvUInt32LittleEndianDCBA;
                            byteOrder = "DCBA";
                            break;
                        case 3:
                            dataGridView = dgvUInt32MidBigEndianBADC;
                            byteOrder = "BADC";
                            break;
                        case 4:
                            dataGridView = dgvUInt32MidLittleEndianCDAB;
                            byteOrder = "CDAB";
                            break;
                    }
                    preColumnName = "dgvUInt32" + gridOrder;
                    break;
                case "Int32":
                    switch (gridOrder)
                    {
                        case 1:
                            dataGridView = dgvInt32BigEndianABCD;
                            byteOrder = "ABCD";
                            break;
                        case 2:
                            dataGridView = dgvInt32LittleEndianDCBA;
                            byteOrder = "DCBA";
                            break;
                        case 3:
                            dataGridView = dgvInt32MidBigEndianBADC;
                            byteOrder = "BADC";
                            break;
                        case 4:
                            dataGridView = dgvInt32MidLittleEndianCDAB;
                            byteOrder = "CDAB";
                            break;
                    }
                    preColumnName = "dgvInt32" + gridOrder;
                    break;
                case "Int16":
                    switch (gridOrder)
                    {
                        case 1:
                            dataGridView = dgvInt16BigEndianAB;
                            byteOrder = "AB";
                            break;
                        case 2:
                            dataGridView = dgvInt16LittleEndianBA;
                            byteOrder = "BA";
                            break;
                    }
                    preColumnName = "dgvInt16" + gridOrder;
                    break;
                case "UInt16":
                    switch (gridOrder)
                    {
                        case 1:
                            dataGridView = dgvUInt16BigEndianAB;
                            byteOrder = "AB";
                            break;
                        case 2:
                            dataGridView = dgvUInt16LittleEndianBA;
                            byteOrder = "BA";
                            break;
                    }
                    preColumnName = "dgvUInt16" + gridOrder;
                    break;
                case "Double":
                    switch (gridOrder)
                    {
                        case 1:
                            dataGridView = dgvDoubleBigEndianABCD;
                            byteOrder = "ABCD";
                            break;
                        case 2:
                            dataGridView = dgvDoubleLittleEndianDCBA;
                            byteOrder = "DCBA";
                            break;
                        case 3:
                            dataGridView = dgvDoubleMidBigEndianBADC;
                            byteOrder = "BADC";
                            break;
                        case 4:
                            dataGridView = dgvDoubleMidLittleEndianCDAB;
                            byteOrder = "CDAB";
                            break;
                    }
                    preColumnName = "dgvDouble" + gridOrder;
                    break;
            }
            
            if (dataGridView == null)
                return;

            dataGridView.Rows.Clear();

            switch (typeName)
            {
                case "Float":
                case "Int32":
                case "UInt32":
                    hexDataList = convertController.CheckListLengthMultipleOf(hexDataList, 4);
                    hexDataBlock = new byte[4];
                    for (int i = 0; i <= hexDataList.Count - 4; i = i + 4)
                    {
                        Array.Copy(hexDataList.ToArray(), i, hexDataBlock, 0, 4);
                        switch (typeName)
                        {
                            case "Float":
                                value = convertController.ConvertToSingle(hexDataBlock, byteOrder, out orderedHexDataBlock);
                                break;
                            case "Int32":
                                value = convertController.ConvertToInt32(hexDataBlock, byteOrder, out orderedHexDataBlock);
                                break;
                            case "UInt32":
                                value = convertController.ConvertToUInt32(hexDataBlock, byteOrder, out orderedHexDataBlock);
                                break;
                        }

                        hexDataText = BitConverter.ToString(orderedHexDataBlock).Replace("-", " ");

                        rowId = dataGridView.Rows.Add();

                        dataGridView.Rows[rowId].Cells[preColumnName + "Index"].Value = rowId;
                        dataGridView.Rows[rowId].Cells[preColumnName + "Raw"].Value = hexDataText;
                        dataGridView.Rows[rowId].Cells[preColumnName + "Value"].Value = value;
                    }
                    break;
                case "Int16":
                case "UInt16":
                    hexDataList = convertController.CheckListLengthMultipleOf(hexDataList, 2);
                    hexDataBlock = new byte[2];
                    for (int i = 0; i <= hexDataList.Count - 2; i = i + 2)
                    {
                        Array.Copy(hexDataList.ToArray(), i, hexDataBlock, 0, 2);
                        switch (typeName)
                        {
                            case "Int16":
                                value = convertController.ConvertToInt16(hexDataBlock, byteOrder, out orderedHexDataBlock);
                                break;
                            case "UInt16":
                                value = convertController.ConvertToUInt16(hexDataBlock, byteOrder, out orderedHexDataBlock);
                                break;
                        }

                        hexDataText = BitConverter.ToString(orderedHexDataBlock).Replace("-", " ");

                        rowId = dataGridView.Rows.Add();

                        dataGridView.Rows[rowId].Cells[preColumnName + "Index"].Value = rowId;
                        dataGridView.Rows[rowId].Cells[preColumnName + "Raw"].Value = hexDataText;
                        dataGridView.Rows[rowId].Cells[preColumnName + "Value"].Value = value;
                    }
                    break;
                case "Double":
                    hexDataList = convertController.CheckListLengthMultipleOf(hexDataList, 8);
                    hexDataBlock = new byte[8];
                    for (int i = 0; i <= hexDataList.Count - 8; i = i + 8)
                    {
                        Array.Copy(hexDataList.ToArray(), i, hexDataBlock, 0, 8);
                        switch (typeName)
                        {
                            case "Double":
                                value = convertController.ConvertToDouble(hexDataBlock, byteOrder, out orderedHexDataBlock);
                                break;
                        }

                        hexDataText = BitConverter.ToString(orderedHexDataBlock).Replace("-", " ");

                        rowId = dataGridView.Rows.Add();

                        dataGridView.Rows[rowId].Cells[preColumnName + "Index"].Value = rowId;
                        dataGridView.Rows[rowId].Cells[preColumnName + "Raw"].Value = hexDataText;
                        dataGridView.Rows[rowId].Cells[preColumnName + "Value"].Value = value;
                    }
                    break;
            }
        }

        private void tmrEpochTime_Tick(object sender, EventArgs e)
        {
            string epochTime = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            txtEpochTime.Text = epochTime;
        }

        private void btnTimestampToHumanDate_Click(object sender, EventArgs e)
        {
            ConvertTimestampToHumanDate();
        }

        private void btnHumanDateToTimestamp_Click(object sender, EventArgs e)
        {
            ConvertHumanDateToTimestamp();
        }
    
        private void ConvertTimestampToHumanDate()
        {
            long epochTime = 0;
            if (!long.TryParse(txtEpochTimeToHumanDate.Text.Trim(), out epochTime))
            {
                MessageBox.Show("Epoch time value is not valid!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ConvertController convertController = new ConvertController();
            DateTime dtimeEpoch = convertController.ConvertEpochToDatetime(epochTime);
            DateTime dtimeEpochGmt = dtimeEpoch.ToLocalTime();
            DateTimeOffset local_offset = new DateTimeOffset(dtimeEpochGmt);
            TimeSpan timeDiff = DateTime.UtcNow - dtimeEpoch;

            lblHumanDateGMT.Text = dtimeEpoch.ToUniversalTime().ToString("dddd, d MMMM yyyy HH:mm:ss");
            lblYourTimeZone.Text = string.Format("{0} GMT{1}",
                                            dtimeEpochGmt.ToString("dddd, d MMMM yyyy HH:mm:ss", System.Globalization.CultureInfo.GetCultureInfo("tr-TR")),
                                            local_offset.ToString().Substring(local_offset.ToString().Length - 6));
            lblRelative.Text = string.Format("{0} Days {1} Hours {2} Minutes", timeDiff.Days, timeDiff.Hours, timeDiff.Minutes);
        }

        private void ConvertHumanDateToTimestamp()
        {
            int year, month, day, hour, minute, second;
            if (!int.TryParse(txtHumanDateYear.Text.Trim(), out year))
            {
                MessageBox.Show("Year is not valid!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtHumanDateMonth.Text.Trim(), out month))
            {
                MessageBox.Show("Month is not valid!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtHumanDateDay.Text.Trim(), out day))
            {
                MessageBox.Show("Day is not valid!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtHumanDateHour.Text.Trim(), out hour))
            {
                MessageBox.Show("Hour is not valid!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtHumanDateMinute.Text.Trim(), out minute))
            {
                MessageBox.Show("Minute is not valid!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtHumanDateSecond.Text.Trim(), out second))
            {
                MessageBox.Show("Second is not valid!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string humanDatetime = string.Format("{0}-{1}-{2} {3}:{4}:{5}",
                                        year.ToString("0000"),
                                        month.ToString("00"),
                                        day.ToString("00"),
                                        hour.ToString("00"),
                                        minute.ToString("00"),
                                        second.ToString("00"));

            DateTime dtimeHumanDate;
            System.Globalization.DateTimeStyles dateTimeStyles = cmbTimeKind.SelectedItem != null && cmbTimeKind.SelectedItem.ToString() == "GMT"
                                                                ? System.Globalization.DateTimeStyles.AssumeUniversal :
                                                                System.Globalization.DateTimeStyles.AssumeLocal;
            try
            {
                dtimeHumanDate = DateTime.ParseExact(humanDatetime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, dateTimeStyles);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Human datetime value is not valid!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DateTime gmtTime = dtimeHumanDate.Kind == DateTimeKind.Local ? dtimeHumanDate : dtimeHumanDate.ToLocalTime();
            DateTime utcTime = dtimeHumanDate.Kind == DateTimeKind.Utc ? dtimeHumanDate : dtimeHumanDate.ToUniversalTime();
            DateTimeOffset local_offset = new DateTimeOffset(gmtTime);
            TimeSpan timeDiff = DateTime.UtcNow - utcTime;

            lblEpochTimestampValue.Text = (dtimeHumanDate.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds.ToString();
            lblTimestampMilliSecondValues.Text = (dtimeHumanDate.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds.ToString();
            lblDateAndTimeGMTValue.Text = utcTime.ToString("dddd, d MMMM yyyy HH:mm:ss");
            lblYourTimZoneDatetimeValue.Text = string.Format("{0} GMT{1}",
                                            gmtTime.ToString("dddd, d MMMM yyyy HH:mm:ss", System.Globalization.CultureInfo.GetCultureInfo("tr-TR")),
                                            local_offset.ToString().Substring(local_offset.ToString().Length - 6));
        }
    }
}
