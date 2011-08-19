using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using DSDecmp.Utils;

namespace DSDecmp.Formats
{
    /// <summary>
    /// The LZ-Overlay compression format. Compresses part of the file from end to start.
    /// Is used for the 'overlay' files in NDS games, as well as arm9.bin.
    /// Note that the last 12 bytes should not be included in the 'inLength' argument when
    /// decompressing arm9.bin. This is done automatically if a file is given instead of a stream.
    /// </summary>
    public class LZOvl : CompressionFormat
    {
        private static bool lookAhead = false;
        /// <summary>
        /// Sets the flag that determines if 'look-ahead'/DP should be used when compressing
        /// with the LZ-Ovl format. The default is false, which is what is used in the original
        /// implementation.
        /// </summary>
        public static bool LookAhead
        {
            set { lookAhead = value; }
        }

        #region Method: Supports(string file)
        public override bool Supports(string file)
        {
            using (FileStream fstr = File.OpenRead(file))
            {
                long fLength = fstr.Length;
                // arm9.bin is special in the sense that the last 12 bytes should/can be ignored.
                if (Path.GetFileName(file) == "arm9.bin")
                    fLength -= 0xC;
                return this.Supports(fstr, fLength);
            }
        }
        #endregion

        #region Method: Supports(Stream, long)
        public override bool Supports(System.IO.Stream stream, long inLength)
        {
            // assume the 'inLength' does not include the 12 bytes at the end of arm9.bin

            // only allow integer-sized files
            if (inLength > 0xFFFFFFFFL)
                return false;
            // the header is 4 bytes minimum
            if (inLength < 4)
                return false;
            long streamStart = stream.Position;
            byte[] header = new byte[Math.Min(inLength, 0x20)];
            stream.Position += inLength - header.Length;
            stream.Read(header, 0, header.Length);
            // reset the stream
            stream.Position = streamStart;

            uint extraSize = IOUtils.ToNDSu32(header, header.Length - 4);
            if (extraSize == 0)
                return false; // do not decompress whenevr the last 4 bytes are 0; too many files have that.
            // if the extrasize is nonzero, the minimum header length is 8  bytes
            if (header.Length < 8)
                return false;
            byte headerLen = header[header.Length - 5];
            if (inLength < headerLen)
                return false;

            // the compressed length should fit in the input file
            int compressedLen = header[header.Length - 6] << 16
                                | header[header.Length - 7] << 8
                                | header[header.Length - 8];
            if (compressedLen >= inLength - headerLen && compressedLen != inLength)
                return false;

            // verify that the rest of the header is filled with 0xFF
            for (int i = header.Length - 9; i >= header.Length - headerLen; i--)
                if (header[i] != 0xFF)
                    return false;
            return true;
        }
        #endregion

        #region Method: Decompress(string, string)
        public override void Decompress(string infile, string outfile)
        {
            // make sure the output directory exists
            string outDirectory = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(outDirectory))
                Directory.CreateDirectory(outDirectory);
            // open the two given files, and delegate to the format-specific code.
            using (FileStream inStream = new FileStream(infile, FileMode.Open),
                             outStream = new FileStream(outfile, FileMode.Create))
            {
                long fLength = inStream.Length;
                // arm9.bin needs special attention
                if (Path.GetFileName(infile) == "arm9.bin")
                    fLength -= 0xC;
                this.Decompress(inStream, fLength, outStream);
            }
        }
        #endregion

        #region Decompression method
        public override long Decompress(System.IO.Stream instream, long inLength, System.IO.Stream outstream)
        {
            #region Format description
            // Overlay LZ compression is basically just LZ-0x10 compression.
            // however the order if reading is reversed: the compression starts at the end of the file.
            // Assuming we start reading at the end towards the beginning, the format is:
            /*
             * u32 extraSize; // decompressed data size = file length (including header) + this value
             * u8 headerSize;
             * u24 compressedLength; // can be less than file size (w/o header). If so, the rest of the file is uncompressed.
             *                       // may also be the file size
             * u8[headerSize-8] padding; // 0xFF-s
             * 
             * 0x10-like-compressed data follows (without the usual 4-byte header).
             * The only difference is that 2 should be added to the DISP value in compressed blocks
             * to get the proper value.
             * the u32 and u24 are read most significant byte first.
             * if extraSize is 0, there is no headerSize, decompressedLength or padding.
             * the data starts immediately, and is uncompressed.
             * 
             * arm9.bin has 3 extra u32 values at the 'start' (ie: end of the file),
             * which may be ignored. (and are ignored here) These 12 bytes also should not
             * be included in the computation of the output size.
             */
            #endregion

            #region First read the last 4 bytes of the stream (the 'extraSize')

            // first go to the end of the stream, since we're reading from back to front
            // read the last 4 bytes, the 'extraSize'
            instream.Position += inLength - 4;

            byte[] buffer = new byte[4];
            try
            {
                instream.Read(buffer, 0, 4);
            }
            catch (System.IO.EndOfStreamException)
            {
                // since we're immediately checking the end of the stream, 
                // this is the only location where we have to check for an EOS to occur.
                throw new StreamTooShortException();
            }
            uint extraSize = IOUtils.ToNDSu32(buffer, 0);

            #endregion

            // if the extra size is 0, there is no compressed part, and the header ends there.
            if (extraSize == 0)
            {
                #region just copy the input to the output

                // first go back to the start of the file. the current location is after the 'extraSize',
                // and thus at the end of the file.
                instream.Position -= inLength;
                // no buffering -> slow
                buffer = new byte[inLength - 4];
                instream.Read(buffer, 0, (int)(inLength - 4));
                outstream.Write(buffer, 0, (int)(inLength - 4));

                // make sure the input is positioned at the end of the file
                instream.Position += 4;

                return inLength - 4;

                #endregion
            }
            else
            {
                // get the size of the compression header first.
                instream.Position -= 5;
                int headerSize = instream.ReadByte();

                // then the compressed data size.
                instream.Position -= 4;
                instream.Read(buffer, 0, 3);
                int compressedSize = buffer[0] | (buffer[1] << 8) | (buffer[2] << 16);

                // the compressed size sometimes is the file size.
                if (compressedSize + headerSize >= inLength)
                    compressedSize = (int)(inLength - headerSize);

                #region copy the non-compressed data

                // copy the non-compressed data first.
                buffer = new byte[inLength - headerSize - compressedSize];
                instream.Position -= (inLength - 5);
                instream.Read(buffer, 0, buffer.Length);
                outstream.Write(buffer, 0, buffer.Length);

                #endregion

                // buffer the compressed data, such that we don't need to keep
                // moving the input stream position back and forth
                buffer = new byte[compressedSize];
                instream.Read(buffer, 0, compressedSize);

                // we're filling the output from end to start, so we can't directly write the data.
                // buffer it instead (also use this data as buffer instead of a ring-buffer for
                // decompression)
                byte[] outbuffer = new byte[compressedSize + headerSize + extraSize];

                int currentOutSize = 0;
                int decompressedLength = outbuffer.Length;
                int readBytes = 0;
                byte flags = 0, mask = 1;
                while (currentOutSize < decompressedLength)
                {
                    // (throws when requested new flags byte is not available)
                    #region Update the mask. If all flag bits have been read, get a new set.
                    // the current mask is the mask used in the previous run. So if it masks the
                    // last flag bit, get a new flags byte.
                    if (mask == 1)
                    {
                        if (readBytes >= compressedSize)
                            throw new NotEnoughDataException(currentOutSize, decompressedLength);
                        flags = buffer[buffer.Length - 1 - readBytes]; readBytes++;
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
                            throw new NotEnoughDataException(currentOutSize, decompressedLength);
                        }
                        int byte1 = buffer[compressedSize - 1 - readBytes]; readBytes++;
                        int byte2 = buffer[compressedSize - 1 - readBytes]; readBytes++;

                        // the number of bytes to copy
                        int length = byte1 >> 4;
                        length += 3;

                        // from where the bytes should be copied (relatively)
                        int disp = ((byte1 & 0x0F) << 8) | byte2;
                        disp += 3;

                        if (disp > currentOutSize)
                        {
                            if (currentOutSize < 2)
                                throw new InvalidDataException(String.Format(Main.Get_Traduction("S0D"), disp.ToString("X"),
                                    currentOutSize.ToString("X")));
                            // HACK. this seems to produce valid files, but isn't the most elegant solution.
                            // although this _could_ be the actual way to use a disp of 2 in this format,
                            // as otherwise the minimum would be 3 (and 0 is undefined, and 1 is less useful).
                            disp = 2;
                        }
                        #endregion

                        int bufIdx = currentOutSize - disp;
                        for (int i = 0; i < length; i++)
                        {
                            byte next = outbuffer[outbuffer.Length - 1 - bufIdx];
                            bufIdx++;
                            outbuffer[outbuffer.Length - 1 - currentOutSize] = next;
                            currentOutSize++;
                        }
                    }
                    else
                    {
                        if (readBytes >= inLength)
                            throw new NotEnoughDataException(currentOutSize, decompressedLength);
                        byte next = buffer[buffer.Length - 1 - readBytes]; readBytes++;

                        outbuffer[outbuffer.Length - 1 - currentOutSize] = next;
                        currentOutSize++;
                    }
                }

                // write the decompressed data
                outstream.Write(outbuffer, 0, outbuffer.Length);

                // make sure the input is positioned at the end of the file; the stream is currently
                // at the compression header.
                instream.Position += headerSize;

                return decompressedLength + (inLength - headerSize - compressedSize);
            }
        }
        #endregion

        #region Compression method; delegates to CompressNormal
        public override int Compress(System.IO.Stream instream, long inLength, System.IO.Stream outstream)
        {
            // don't bother trying to get the optimal not-compressed - compressed ratio for now.
            // Either compress fully or don't compress (as the format cannot handle decompressed
            // sizes that are smaller than the compressed file).

            if (inLength > 0xFFFFFF)
                throw new InputTooLargeException();

            // read the input and reverse it
            byte[] indata = new byte[inLength];
            instream.Read(indata, 0, (int)inLength);
            Array.Reverse(indata);

            MemoryStream inMemStream = new MemoryStream(indata);
            MemoryStream outMemStream = new MemoryStream();
            int compressedLength = this.CompressNormal(inMemStream, inLength, outMemStream);

            int totalCompFileLength = (int)outMemStream.Length + 8;
            // make the file 4-byte aligned with padding in the header
            if (totalCompFileLength % 4 != 0)
                totalCompFileLength += 4 - totalCompFileLength % 4;

            if (totalCompFileLength < inLength)
            {
                byte[] compData = outMemStream.ToArray();
                Array.Reverse(compData);
                outstream.Write(compData, 0, compData.Length);
                int writtenBytes = compData.Length;
                // there always seem to be some padding FFs. Let's pad to make the file 4-byte aligned
                while (writtenBytes % 4 != 0)
                {
                    outstream.WriteByte(0xFF);
                    writtenBytes++;
                }

                outstream.WriteByte((byte)((compressedLength) & 0xFF));
                outstream.WriteByte((byte)((compressedLength >> 8) & 0xFF));
                outstream.WriteByte((byte)((compressedLength >> 16) & 0xFF));

                int headerLength = totalCompFileLength - compData.Length;
                outstream.WriteByte((byte)headerLength);

                int extraSize = (int)inLength - totalCompFileLength;
                outstream.WriteByte((byte)((extraSize) & 0xFF));
                outstream.WriteByte((byte)((extraSize >> 8) & 0xFF));
                outstream.WriteByte((byte)((extraSize >> 16) & 0xFF));
                outstream.WriteByte((byte)((extraSize >> 24) & 0xFF));

                return totalCompFileLength;
            }
            else
            {
                Array.Reverse(indata);
                outstream.Write(indata, 0, (int)inLength);
                outstream.WriteByte(0); outstream.WriteByte(0); outstream.WriteByte(0); outstream.WriteByte(0);
                return (int)inLength + 4;
            }
        }
        #endregion

        #region 'Normal' compression method. Delegates to CompressWithLA when LookAhead is set
        /// <summary>
        /// Compresses the given input stream with the LZ-Ovl compression, but compresses _forward_
        /// instad of backwards.
        /// </summary>
        /// <param name="instream">The input stream to compress.</param>
        /// <param name="inLength">The length of the input stream.</param>
        /// <param name="outstream">The stream to write to.</param>
        private unsafe int CompressNormal(Stream instream, long inLength, Stream outstream)
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

            int compressedLength = 0;

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
                    int oldLength = Math.Min(readBytes, 0x1001);
                    int length = LZUtil.GetOccurrenceLength(instart + readBytes, (int)Math.Min(inLength - readBytes, 0x12),
                                                          instart + readBytes - oldLength, oldLength, out disp);

                    // disp = 1 cannot be stored.
                    if (disp == 1)
                    {
                        length = 1;
                    }
                    // disp = 2 cannot be saved properly. use a too large disp instead.
                    // however since I'm not sure if that's actually how that's handled, don't compress instead.
                    else if (disp == 2)
                    {
                        length = 1;
                        /*if (readBytes < 0x1001)
                            disp = readBytes + 1;
                        else
                            length = 1;/**/
                    }

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
                        outbuffer[bufferlength] |= (byte)(((disp - 3) >> 8) & 0x0F);
                        bufferlength++;
                        outbuffer[bufferlength] = (byte)((disp - 3) & 0xFF);
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

            int compressedLength = 0;

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

                int optCompressionLength = this.GetOptimalCompressionPartLength(lengths);

                while (readBytes < optCompressionLength)
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
                        outbuffer[bufferlength] |= (byte)(((disps[readBytes] - 3) >> 8) & 0x0F);
                        bufferlength++;
                        outbuffer[bufferlength] = (byte)((disps[readBytes] - 3) & 0xFF);
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
                }

                while (readBytes < inLength)
                    outstream.WriteByte(*(instart + (readBytes++)));
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
        /// <param name="disps">The 'disp' values of the compressed blocks. May be less than 3, in which case the
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
                int oldLength = Math.Min(0x1001, i);
                // get the appropriate disp while at it. Takes at most O(n) time if oldLength is considered O(n)
                // be sure to bound the input length with 0x12, as that's the maximum length for LZ-Ovl compressed blocks.
                int maxLen = LZUtil.GetOccurrenceLength(indata + i, Math.Min(inLength - i, 0x12),
                                                 indata + i - oldLength, oldLength, out disps[i]);
                if (disps[i] > i)
                    throw new Exception(Main.Get_Traduction("S02"));
                // disp < 3 cannot be stored explicitly.
                if (disps[i] < 3)
                    maxLen = 1;
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

        #region DP compression helper method: GetOptimalCompressionPartLength
        /// <summary>
        /// Gets the 'optimal' length of the compressed part of the file.
        /// Or rather: the length in such a way that compressing any more will not
        /// result in a shorter file.
        /// </summary>
        /// <param name="blocklengths">The lengths of the compressed blocks, as gotten from GetOptimalCompressionLengths.</param>
        /// <returns>The 'optimal' length of the compressed part of the file.</returns>
        private int GetOptimalCompressionPartLength(int[] blocklengths)
        {
            // first determine the actual total compressed length using the optimal compression.
            int block8Idx = 0;
            int insideBlockIdx = 0;
            int totalCompLength = 0;
            for (int i = 0; i < blocklengths.Length; )
            {
                if (insideBlockIdx == 8)
                {
                    block8Idx++;
                    insideBlockIdx = 0;
                    totalCompLength++;
                }
                insideBlockIdx++;

                if (blocklengths[i] >= 3)
                    totalCompLength += 2;
                else
                    totalCompLength++;
                i += blocklengths[i];
            }

            int[] actualRestCompLengths = new int[blocklengths.Length];
            block8Idx = 0;
            insideBlockIdx = 0;
            for (int i = 0; i < blocklengths.Length; )
            {
                if (insideBlockIdx == 8)
                {
                    block8Idx++;
                    insideBlockIdx = 0;
                    totalCompLength--;
                }
                if (blocklengths[i] >= 3)
                    totalCompLength -= 2;
                else
                    totalCompLength--;
                actualRestCompLengths[i] = totalCompLength;
                i += blocklengths[i];
                insideBlockIdx++;

                if (totalCompLength > (blocklengths.Length - i))
                    return i;
            }
            return blocklengths.Length;
        }
        #endregion
    }
}
