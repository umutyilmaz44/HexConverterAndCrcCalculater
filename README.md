# Hex Converter & Crc Calculater & Epoch-Unix Timestamp Conversion
Hex to type convert, CRC calculate and Epoch-Unix Timestamp Conversion

The application consists of three parts.
The first section contains hex & primitive data types conversions. The second part contains crc calculations according to various algorithms from hex data. The third section includes various history transformations.

**Hex To Primitive Types Converting**

The following conversion processes take place in this section:

  * Hex data to Float - Big Endian (ABCD)
  * Hex data to Float - Little Endian (DCBA)
  * Hex data to Float - Mid-Big Endian (BADC)
  * Hex data to Float - Mid Little Endian (CDAB)
  
  * Hex data to UInt32 - Big Endian (ABCD)
  * Hex data to UInt32 - Little Endian (DCBA)
  * Hex data to UInt32 - Mid-Big Endian (BADC)
  * Hex data to UInt32 - Mid Little Endian (CDAB)
  
  * Hex data to Int32 - Big Endian (ABCD)
  * Hex data to Int32 - Little Endian (DCBA)
  * Hex data to Int32 - Mid-Big Endian (BADC)
  * Hex data to Int32 - Mid Little Endian (CDAB)
  
  * Hex data to Int16 - Big Endian (AB)
  * Hex data to Int16 - Little Endian (BA)
  * Hex data to UInt16 - Big Endian (AB)
  * Hex data to UInt16 - Little Endian (BA)
  
  * Hex data to Double - Big Endian (ABCD)
  * Hex data to Double - Little Endian (DCBA)
  * Hex data to Double - Mid-Big Endian (BADC)
  * Hex data to Double - Mid Little Endian (CDAB)
  
SNAPSHOT For Hex To Primitive Types Converting
![SNAPSHOT1](https://user-images.githubusercontent.com/42136540/87153810-bb4b3900-c2c0-11ea-95b0-69382422d5af.PNG)

**CRC Calculating**

The following CRC calculating processes take place in this section:
  
  * CheckSum8 Xor
  * CheckSum8 Modulo 256 (Sum of Bytes % 256)
  * CheckSum8 2s Complement (0x100 - Sum Of Bytes)
  * CRC-16 (MODBUS)
  * CRC-16-CCITT (X.25, V.41, HDLC, XMODEM, Bluetooth, SD, many others; known as CRC-CCITT)
  * CRC-16-IBM (Bisync, Modbus, USB, ANSI X3.28, many others; also known as CRC-16 and CRC-16-ANSI)
  * CRC-16-T10-DIF (SCSI DIF)
  * CRC-16-DNP (DNP, IEC 870, ModBus)
  * CRC-16-DECT (Cordless Telephones)
  * CRC-32 (ISO 3309, ANSI X3.66, FIPS PUB 71, FED-STD-1003, ITU-T V.42, Ethernet, SATA, MPEG-2, Gzip, PKZIP, POSIX cksum, PNG, ZMODEM)
  * CRC-32C (Castagnoli) (iSCSI & SCTP, G.hn payload, SSE4.2)
  * CRC-32K (Koopman) 
  * CRC-32Q (Aviation; AIXM)
 
SNAPSHOT For CRC Calculating 
![SNAPSHOT2](https://user-images.githubusercontent.com/42136540/87153817-bdad9300-c2c0-11ea-99bb-cbf77008787c.PNG)

**Time Converting**

The following Time converting processes take place in this section:

* Live epoch time viewing
* Convert epoch to human-readable date (Local time & GMT)
* Convert human-readable date to epoch time (Local time & GMT)

SNAPSHOT For Time Converting
![SNAPSHOT3](https://user-images.githubusercontent.com/42136540/87153824-bf775680-c2c0-11ea-9668-aa95d3457d82.PNG)
