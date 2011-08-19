using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DSDecmp.Utils;

namespace DSDecmp.Formats.Nitro
{
    /// <summary>
    /// Compressor and decompressor for the LZ-0x10 format used in many of the games for the
    /// newer Nintendo consoles and handhelds.
    /// </summary>
    public class LZ10 : NitroCFormat
    {
        private static bool lookAhead = false;
        /// <summary>
        /// Sets the flag that determines if 'look-ahead'/DP should be used when compressing
        /// with the LZ-10 format. The default is false, which is what is used in the original
        /// implementation.
        /// </summary>
        public static bool LookAhead
        {
            set { lookAhead = value; }
        }

        public LZ10() : base(0x10) { }

        #region 'Original' Decompression method
        /// <summary>
        /// Decompress a stream that is compressed in the LZ-10 format.
        /// </summary>
        /// <param name="instream">The compressed stream.</param>
        /// <param name="inLength">The length of the input stream.</param>
        /// <param name="outstream">The output stream, where the decompressed data is written to.</param>
        public override long Decompress(Stream instream, long inLength,
                                            Stream outstream)
        {
            #region format definition from GBATEK/NDSTEK
            /*  Data header (32bit)
                  Bit 0-3   Reserved
                  Bit 4-7   Compressed type (must be 1 for LZ77)
                  Bit 8-31  Size of decompressed data
                Repeat below. Each Flag Byte followed by eight Blocks.
                Flag data (8bit)
                  Bit 0-7   Type Flags for next 8 Blocks, MSB first
                Block Type 0 - Uncompressed - Copy 1 Byte from Source to Dest
                  Bit 0-7   One data byte to be copied to dest
                Block Type 1 - Compressed - Copy N+3 Bytes from Dest-Disp-1 to Dest
                  Bit 0-3   Disp MSBs
                  Bit 4-7   Number of bytes to copy (minus 3)
                  Bit 8-15  Disp LSBs
             */
            #endregion

            long readBytes = 0;

            byte type = (byte)instream.ReadByte();
            if (type != base.magicByte)
                throw new InvalidDataException(String.Format(Main.Get_Traduction("S00"), type.ToString("X")));
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

            // the maximum 'DISP-1' is 0xFFF.
            int bufferLength = 0x1000;
            byte[] buffer = new byte[bufferLength];
            int bufferOffset = 0;
            

            int currentOutSize = 0;
            int flags = 0, mask = 1;
            while (currentOutSize < decompressedSize)
            {
                // (throws when requested new flags byte is not available)
                #region Update the mask. If all flag bits have been read, get a new set.
                // the current mask is the mask used in the previous run. So if it masks the
                // last flag bit, get a new flags byte.
                if (mask == 1)
                {
                    if (readBytes >= inLength)
                        throw new NotEnoughDataException(currentOutSize, decompressedSize);
                    flags = instream.ReadByte(); readBytes++;
                    if (flags < 0)
                        throw new StreamTooShortException();
                    mask = 0x80;
                }
                else
                {
                    mask >>= 1;
                }
                #endregion

                // bit = 1 <=> compressed.
                if ((flags & mask) > 0)
                {
                    // (throws when < 2 bytes are available)
                    #region Get length and displacement('disp') values from next 2 bytes
                    // there are < 2 bytes available when the end is at most 1 byte away
                    if (readBytes + 1 >= inLength)
                    {
                        // make sure the stream is at the end
                        if (readBytes < inLength)
                        {
                            instream.ReadByte(); readBytes++;
                        }
                        throw new NotEnoughDataException(currentOutSize, decompressedSize);
                    }
                    int byte1 = instream.ReadByte(); readBytes++;
                    int byte2 = instream.ReadByte(); readBytes++;
                    if (byte2 < 0)
                        throw new StreamTooShortException();

                    // the number of bytes to copy
                    int length = byte1 >> 4;
                    length += 3;

                    // from where the bytes should be copied (relatively)
                    int disp = ((byte1 & 0x0F) << 8) | byte2;
                    disp += 1;

                    if (disp > currentOutSize)
                        throw new InvalidDataException(String.Format(Main.Get_Traduction("S01"),
                                disp.ToString("X"), currentOutSize.ToString("X"), (instream.Position - 2).ToString("X")));
                    #endregion

                    int bufIdx = bufferOffset + bufferLength - disp;
                    for (int i = 0; i < length; i++)
                    {
                        byte next = buffer[bufIdx % bufferLength];
                        bufIdx++;
                        outstream.WriteByte(next);
                        buffer[bufferOffset] = next;
                        bufferOffset = (bufferOffset + 1) % bufferLength;
                    }
                    currentOutSize += length;
                }
                else
                {
                    if (readBytes >= inLength)
                        throw new NotEnoughDataException(currentOutSize, decompressedSize);
                    int next = instream.ReadByte(); readBytes++;
                    if (next < 0)
                        throw new StreamTooShortException();
                        
                    currentOutSize++;
                    outstream.WriteByte((byte)next);
                    buffer[bufferOffset] = (byte)next;
                    bufferOffset = (bufferOffset + 1) % bufferLength;
                }
                outstream.Flush();
            }

            if (readBytes < inLength)
            {
                // the input may be 4-byte aligned.
                if ((readBytes ^ (readBytes & 3)) + 4 < inLength)
                    throw new TooMuchInputException(readBytes, inLength);
            }

            return decompressedSize;
        }
        #endregion
        
        #region Original Compress method
        public unsafe override int Compress(Stream instream, long inLength, Stream outstream)
        {
            // make sure the decompressed size fits in 3 bytes.
            // There should be room for four bytes, however I'm not 100% sure if that can be used
            // in every game, as it may not be a built-in function.
            if (inLength > 0xFFFFFF)
                throw new InputTooLargeException();

            // use the other method if lookahead is enabled
            if (lookAhead)
            {
                return CompressWithLA(instream, inLength, outstream);
            }
            
            // save the input data in an array to prevent having to go back and forth in a file
            byte[] indata = new byte[inLength];
            int numReadBytes = instream.Read(indata, 0, (int)inLength);
            if (numReadBytes != inLength)
                throw new StreamTooShortException();

            // write the compression header first
            outstream.WriteByte(this.magicByte);
            outstream.WriteByte((byte)(inLength & 0xFF));
            outstream.WriteByte((byte)((inLength >> 8) & 0xFF));
            outstream.WriteByte((byte)((inLength >> 16) & 0xFF));

            int compressedLength = 4;

            fixed (byte* instart = &indata[0])
            {
                // we do need to buffer the output, as the first byte indicates which blocks are compressed.
                // this version does not use a look-ahead, so we do not need to buffer more than 8 blocks at a time.
                byte[] outbuffer = new byte[8 * 2 + 1];
                outbuffer[0] = 0;
                int bufferlength = 1, bufferedBlocks = 0;
                int readBytes = 0;
                while (readBytes < inLength)
                {
                    #region If 8 blocks are bufferd, write them and reset the buffer
                    // we can only buffer 8 blocks at a time.
                    if (bufferedBlocks == 8)
                    {
                        outstream.Write(outbuffer, 0, bufferlength);
                        compressedLength += bufferlength;
                        // reset the buffer
                        outbuffer[0] = 0;
                        bufferlength = 1;
                        bufferedBlocks = 0;
                    }
                    #endregion

                    // determine if we're dealing with a compressed or raw block.
                    // it is a compressed block when the next 3 or more bytes can be copied from
                    // somewhere in the set of already compressed bytes.
                    int disp;
                    int oldLength = Math.Min(readBytes, 0x1000);
                    int length = LZUtil.GetOccurrenceLength(instart + readBytes, (int)Math.Min(inLength - readBytes, 0x12),
                                                          instart + readBytes - oldLength, oldLength, out disp);

                    // length not 3 or more? next byte is raw data
                    if (length < 3)
                    {
                        outbuffer[bufferlength++] = *(instart + (readBytes++));
                    }
                    else
                    {
                        // 3 or more bytes can be copied? next (length) bytes will be compressed into 2 bytes
                        readBytes += length;

                        // mark the next block as compressed
                        outbuffer[0] |= (byte)(1 << (7 - bufferedBlocks));

                        outbuffer[bufferlength] = (byte)(((length - 3) << 4) & 0xF0);
                        outbuffer[bufferlength] |= (byte)(((disp - 1) >> 8) & 0x0F);
                        bufferlength++;
                        outbuffer[bufferlength] = (byte)((disp - 1) & 0xFF);
                        bufferlength++;
                    }
                    bufferedBlocks++;
                }

                // copy the remaining blocks to the output
                if (bufferedBlocks > 0)
                {
                    outstream.Write(outbuffer, 0, bufferlength);
                    compressedLength += bufferlength;
                    /*/ make the compressed file 4-byte aligned.
                    while ((compressedLength % 4) != 0)
                    {
                        outstream.WriteByte(0);
                        compressedLength++;
                    }/**/
                }
            }

            return compressedLength;
        }
        #endregion

        #region Dynamic Programming compression method
        /// <summary>
        /// Variation of the original compression method, making use of Dynamic Programming to 'look ahead'
        /// and determine the optimal 'length' values for the compressed blocks. Is not 100% optimal,
        /// as the flag-bytes are not taken into account.
        /// </summary>
        private unsafe int CompressWithLA(Stream instream, long inLength, Stream outstream)
        {
            // save the input data in an array to prevent having to go back and forth in a file
            byte[] indata = new byte[inLength];
            int numReadBytes = instream.Read(indata, 0, (int)inLength);
            if (numReadBytes != inLength)
                throw new StreamTooShortException();

            // write the compression header first
            outstream.WriteByte(this.magicByte);
            outstream.WriteByte((byte)(inLength & 0xFF));
            outstream.WriteByte((byte)((inLength >> 8) & 0xFF));
            outstream.WriteByte((byte)((inLength >> 16) & 0xFF));

            int compressedLength = 4;

            fixed (byte* instart = &indata[0])
            {
                // we do need to buffer the output, as the first byte indicates which blocks are compressed.
                // this version does not use a look-ahead, so we do not need to buffer more than 8 blocks at a time.
                byte[] outbuffer = new byte[8 * 2 + 1];
                outbuffer[0] = 0;
                int bufferlength = 1, bufferedBlocks = 0;
                int readBytes = 0;

                // get the optimal choices for len and disp
                int[] lengths, disps;
                this.GetOptimalCompressionLengths(instart, indata.Length, out lengths, out disps);
                while (readBytes < inLength)
                {
                    // we can only buffer 8 blocks at a time.
                    if (bufferedBlocks == 8)
                    {
                        outstream.Write(outbuffer, 0, bufferlength);
                        compressedLength += bufferlength;
                        // reset the buffer
                        outbuffer[0] = 0;
                        bufferlength = 1;
                        bufferedBlocks = 0;
                    }


                    if (lengths[readBytes] == 1)
                    {
                        outbuffer[bufferlength++] = *(instart + (readBytes++));
                    }
                    else
                    {
                        // mark the next block as compressed
                        outbuffer[0] |= (byte)(1 << (7 - bufferedBlocks));

                        outbuffer[bufferlength] = (byte)(((lengths[readBytes] - 3) << 4) & 0xF0);
                        outbuffer[bufferlength] |= (byte)(((disps[readBytes] - 1) >> 8) & 0x0F);
                        bufferlength++;
                        outbuffer[bufferlength] = (byte)((disps[readBytes] - 1) & 0xFF);
                        bufferlength++;

                        readBytes += lengths[readBytes];
                    }


                    bufferedBlocks++;
                }

                // copy the remaining blocks to the output
                if (bufferedBlocks > 0)
                {
                    outstream.Write(outbuffer, 0, bufferlength);
                    compressedLength += bufferlength;
                    /*/ make the compressed file 4-byte aligned.
                    while ((compressedLength % 4) != 0)
                    {
                        outstream.WriteByte(0);
                        compressedLength++;
                    }/**/
                }
            }

            return compressedLength;
        }
        #endregion

        #region DP compression helper method; GetOptimalCompressionLengths
        /// <summary>
        /// Gets the optimal compression lengths for each start of a compressed block using Dynamic Programming.
        /// This takes O(n^2) time.
        /// </summary>
        /// <param name="indata">The data to compress.</param>
        /// <param name="inLength">The length of the data to compress.</param>
        /// <param name="lengths">The optimal 'length' of the compressed blocks. For each byte in the input data,
        /// this value is the optimal 'length' value. If it is 1, the block should not be compressed.</param>
        /// <param name="disps">The 'disp' values of the compressed blocks. May be 0, in which case the
        /// corresponding length will never be anything other than 1.</param>
        private unsafe void GetOptimalCompressionLengths(byte* indata, int inLength, out int[] lengths, out int[] disps)
        {
            lengths = new int[inLength];
            disps = new int[inLength];
            int[] minLengths = new int[inLength];

            for (int i = inLength - 1; i >= 0; i--)
            {
                // first get the compression length when the next byte is not compressed
                minLengths[i] = int.MaxValue;
                lengths[i] = 1;
                if (i + 1 >= inLength)
                    minLengths[i] = 1;
                else
                    minLengths[i] = 1 + minLengths[i + 1];
                // then the optimal compressed length
                int oldLength = Math.Min(0x1000, i);
                // get the appropriate disp while at it. Takes at most O(n) time if oldLength is considered O(n)
                // be sure to bound the input length with 0x12, as that's the maximum length for LZ-10 compressed blocks.
                int maxLen = LZUtil.GetOccurrenceLength(indata + i, Math.Min(inLength - i, 0x12), 
                                                 indata + i - oldLength, oldLength, out disps[i]);
                if (disps[i] > i)
                    throw new Exception(Main.Get_Traduction("S02"));
                for (int j = 3; j <= maxLen; j++)
                {
                    int newCompLen;
                    if (i + j >= inLength)
                        newCompLen = 2;
                    else
                        newCompLen = 2 + minLengths[i + j];
                    if (newCompLen < minLengths[i])
                    {
                        lengths[i] = j;
                        minLengths[i] = newCompLen;
                    }
                }
            }

            // we could optimize this further to also optimize it with regard to the flag-bytes, but that would require 8 times
            // more space and time (one for each position in the block) for only a potentially tiny increase in compression ratio.
        }
        #endregion
    }
}
