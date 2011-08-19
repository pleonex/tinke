using System;
using System.Collections.Generic;
using System.Text;

namespace DSDecmp.Utils
{
    internal static class LZUtil
    {
        /// <summary>
        /// Determine the maximum size of a LZ-compressed block starting at newPtr, using the already compressed data
        /// starting at oldPtr. Takes O(inLength * oldLength) = O(n^2) time.
        /// </summary>
        /// <param name="newPtr">The start of the data that needs to be compressed.</param>
        /// <param name="newLength">The number of bytes that still need to be compressed.</param>
        /// <param name="oldPtr">The start of the raw file.</param>
        /// <param name="oldLength">The number of bytes already compressed.</param>
        /// <param name="disp">The offset of the start of the longest block to refer to.</param>
        /// <returns>The length of the longest sequence of bytes that can be copied from the already decompressed data.</returns>
        internal static unsafe int GetOccurrenceLength(byte* newPtr, int newLength, byte* oldPtr, int oldLength, out int disp)
        {
            disp = 0;
            if (newLength == 0)
                return 0;
            int maxLength = 0;
            // try every possible 'disp' value (disp = oldLength - i)
            for (int i = 0; i < oldLength - 1; i++)
            {
                // work from the start of the old data to the end, to mimic the original implementation's behaviour
                // (and going from start to end or from end to start does not influence the compression ratio anyway)
                byte* currentOldStart = oldPtr + i;
                int currentLength = 0;
                // determine the length we can copy if we go back (oldLength - i) bytes
                // always check the next 'newLength' bytes, and not just the available 'old' bytes,
                // as the copied data can also originate from what we're currently trying to compress.
                for (int j = 0; j < newLength; j++)
                {
                    // stop when the bytes are no longer the same
                    if (*(currentOldStart + j) != *(newPtr + j))
                        break;
                    currentLength++;
                }

                // update the optimal value
                if (currentLength > maxLength)
                {
                    maxLength = currentLength;
                    disp = oldLength - i;
                }
            }
            return maxLength;
        }
    }
}
