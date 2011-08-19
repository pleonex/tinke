using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DSDecmp
{
    /// <summary>
    /// An exception that is thrown by the decompression functions when there
    /// is not enough data available in order to properly decompress the input.
    /// </summary>
    public class NotEnoughDataException : IOException
    {
        private long currentOutSize;
        private long totalOutSize;
        /// <summary>
        /// Gets the actual number of written bytes.
        /// </summary>
        public long WrittenLength { get { return this.currentOutSize; } }
        /// <summary>
        /// Gets the number of bytes that was supposed to be written.
        /// </summary>
        public long DesiredLength { get { return this.totalOutSize; } }

        /// <summary>
        /// Creates a new NotEnoughDataException.
        /// </summary>
        /// <param name="currentOutSize">The actual number of written bytes.</param>
        /// <param name="totalOutSize">The desired number of written bytes.</param>
        public NotEnoughDataException(long currentOutSize, long totalOutSize)
            : base(String.Format(Main.Get_Traduction("S11"), currentOutSize.ToString("X"),
                (totalOutSize < 0 ? "???" : ("0x" + totalOutSize.ToString("X")))))
        {
            this.currentOutSize = currentOutSize;
            this.totalOutSize = totalOutSize;
        }
    }
}
