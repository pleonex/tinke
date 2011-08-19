using System;
using System.Collections.Generic;
using System.Text;

namespace DSDecmp
{
    public class TooMuchInputException : Exception
    {
        /// <summary>
        /// Gets the number of bytes read by the decompressed to decompress the stream.
        /// </summary>
        public long ReadBytes { get; private set; }

        /// <summary>
        /// Creates a new exception indicating that the input has more data than necessary for
        /// decompressing th stream. It may indicate that other data is present after the compressed
        /// stream.
        /// </summary>
        /// <param name="readBytes">The number of bytes read by the decompressor.</param>
        /// <param name="totLength">The indicated length of the input stream.</param>
        public TooMuchInputException(long readBytes, long totLength)
            : base(String.Format(Main.Get_Traduction("S0F"), readBytes.ToString("X"), totLength.ToString("X")))
        {
            this.ReadBytes = readBytes;
        }
    }
}
