using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DSDecmp.Formats.Nitro
{
    /// <summary>
    /// Compressor and decompressor for the RLE format used in several of the games for the
    /// newer Nintendo consoles and handhelds.
    /// </summary>
    public class RLE : NitroCFormat
    {
        public RLE() : base(0x30) { }

        public override long Decompress(Stream instream, long inLength, Stream outstream)
        {
            /*      
                Data header (32bit)
                    Bit 0-3   Reserved
                    Bit 4-7   Compressed type (must be 3 for run-length)
                    Bit 8-31  Size of decompressed data
                Repeat below. Each Flag Byte followed by one or more Data Bytes.
                Flag data (8bit)
                    Bit 0-6   Expanded Data Length (uncompressed N-1, compressed N-3)
                    Bit 7     Flag (0=uncompressed, 1=compressed)
                Data Byte(s) - N uncompressed bytes, or 1 byte repeated N times
             */

            long readBytes = 0;

            byte type = (byte)instream.ReadByte();
            if (type != base.magicByte)
                throw new InvalidDataException(String.Format(Main.Get_Traduction("S0B"), type.ToString("X")));
            byte[] sizeBytes = new byte[3];
            instream.Read(sizeBytes, 0, 3);
            int decompressedSize = IOUtils.ToNDSu24(sizeBytes, 0);
            readBytes += 4;
            if (decompressedSize == 0)
            {
                sizeBytes = new byte[4];
                instream.Read(sizeBytes, 0, 4);
                decompressedSize = IOUtils.ToNDSs32(sizeBytes, 0);
                readBytes += 4;
            }


            int currentOutSize = 0;
            while (currentOutSize < decompressedSize)
            {
                #region (try to) get the flag byte with the length data and compressed flag

                if (readBytes >= inLength)
                    throw new NotEnoughDataException(currentOutSize, decompressedSize);
                int flag = instream.ReadByte(); readBytes++;
                if (flag < 0)
                    throw new StreamTooShortException();

                bool compressed = (flag & 0x80) > 0;
                int length = flag & 0x7F;

                if (compressed)
                    length += 3;
                else
                    length += 1;

                #endregion

                if (compressed)
                {
                    #region compressed: write the next byte (length) times.

                    if (readBytes >= inLength)
                        throw new NotEnoughDataException(currentOutSize, decompressedSize);
                    int data = instream.ReadByte(); readBytes++;
                    if (data < 0)
                        throw new StreamTooShortException();

                    if (currentOutSize + length > decompressedSize)
                        throw new InvalidDataException(Main.Get_Traduction("S0C"));
                    byte bdata = (byte)data;
                    for (int i = 0; i < length; i++)
                    {
                        // Stream.Write(byte[], offset, len) may also work, but only if it is a circular buffer
                        outstream.WriteByte(bdata);
                        currentOutSize++;
                    }

                    #endregion
                }
                else
                {
                    #region uncompressed: copy the next (length) bytes.

                    int tryReadLength = length;
                    // limit the amount of bytes read by the indicated number of bytes available
                    if (readBytes + length > inLength)
                        tryReadLength = (int)(inLength - readBytes);
                    
                    byte[] data = new byte[length];
                    int readLength = instream.Read(data, 0, (int)tryReadLength);
                    readBytes += readLength;
                    outstream.Write(data, 0, readLength);
                    currentOutSize += readLength;

                    // if the attempted number of bytes read is less than the desired number, the given input
                    // length is too small (or there is not enough data in the stream)
                    if (tryReadLength < length)
                        throw new NotEnoughDataException(currentOutSize, decompressedSize);
                    // if the actual number of read bytes is even less, it means that the end of the stream has
                    // bee reached, thus the given input length is larger than the actual length of the input
                    if (readLength < length)
                        throw new StreamTooShortException();

                    #endregion
                }
            }

            if (readBytes < inLength)
            {
                // the input may be 4-byte aligned.
                if ((readBytes ^ (readBytes & 3)) + 4 < inLength)
                    throw new TooMuchInputException(readBytes, inLength);
            }

            return decompressedSize;
        }

        public override int Compress(Stream instream, long inLength, Stream outstream)
        {

            if (inLength > 0xFFFFFF)
                throw new InputTooLargeException();

            List<byte> compressedData = new List<byte>();

            // at most 0x7F+3=130 bytes are compressed into a single block.
            // (and at most 0x7F+1=128 in an uncompressed block, however we need to read 2
            // more, since the last byte may be part of a repetition).
            byte[] dataBlock = new byte[130];
            // the length of the valid content in the current data block
            int currentBlockLength = 0;

            int readLength = 0;
            int nextByte;
            int repCount = 1;
            while (readLength < inLength)
            {
                bool foundRepetition = false;

                while (currentBlockLength < dataBlock.Length && readLength < inLength)
                {
                    nextByte = instream.ReadByte();
                    if (nextByte < 0)
                        throw new StreamTooShortException();
                    readLength++;

                    dataBlock[currentBlockLength++] = (byte)nextByte;
                    if (currentBlockLength > 1)
                    {
                        if (nextByte == dataBlock[currentBlockLength - 2])
                            repCount++;
                        else
                            repCount = 1;
                    }

                    foundRepetition = repCount > 2;
                    if (foundRepetition)
                        break;
                }


                int numUncompToCopy = 0;
                if (foundRepetition)
                {
                    // if a repetition was found, copy block size - 3 bytes as compressed data
                    numUncompToCopy = currentBlockLength - 3;
                }
                else
                {
                    // if no repetition was found, copy min(block size, max block size - 2) bytes as uncompressed data.
                    numUncompToCopy = Math.Min(currentBlockLength, dataBlock.Length - 2);
                }

                #region insert uncompressed block
                if (numUncompToCopy > 0)
                {
                    byte flag = (byte)(numUncompToCopy - 1);
                    compressedData.Add(flag);
                    for (int i = 0; i < numUncompToCopy; i++)
                        compressedData.Add(dataBlock[i]);
                    // shift some possibly remaining bytes to the start
                    for (int i = numUncompToCopy; i < currentBlockLength; i++)
                        dataBlock[i - numUncompToCopy] = dataBlock[i];
                    currentBlockLength -= numUncompToCopy;
                }
                #endregion

                if (foundRepetition)
                {
                    // if a repetition was found, continue until the first different byte
                    // (or until the buffer is full)
                    while (currentBlockLength < dataBlock.Length && readLength < inLength)
                    {
                        nextByte = instream.ReadByte();
                        if (nextByte < 0)
                            throw new StreamTooShortException();
                        readLength++;

                        dataBlock[currentBlockLength++] = (byte)nextByte;

                        if (nextByte != dataBlock[0])
                            break;
                        else
                            repCount++;
                    }

                    // the next repCount bytes are the same.
                    #region insert compressed block
                    byte flag = (byte)(0x80 | (repCount - 3));
                    compressedData.Add(flag);
                    compressedData.Add(dataBlock[0]);
                    // make sure to shift the possible extra byte to the start
                    if (repCount != currentBlockLength)
                        dataBlock[0] = dataBlock[currentBlockLength - 1];
                    currentBlockLength -= repCount;
                    #endregion
                }
            }

            // write any reamaining bytes as uncompressed
            if (currentBlockLength > 0)
            {
                byte flag = (byte)(currentBlockLength - 1);
                compressedData.Add(flag);
                for (int i = 0; i < currentBlockLength; i++)
                    compressedData.Add(dataBlock[i]);
                currentBlockLength = 0;
            }

            // write the RLE marker and the decompressed size
            outstream.WriteByte(0x30);
            int compLen = compressedData.Count;
            outstream.WriteByte((byte)(inLength & 0xFF));
            outstream.WriteByte((byte)((inLength >> 8) & 0xFF));
            outstream.WriteByte((byte)((inLength >> 16) & 0xFF));

            // write the compressed data
            outstream.Write(compressedData.ToArray(), 0, compLen);

            // the total compressed stream length is the compressed data length + the 4-byte header
            return compLen + 4;
        }
    }
}
