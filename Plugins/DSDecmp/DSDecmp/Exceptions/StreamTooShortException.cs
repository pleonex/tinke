using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DSDecmp
{
    /// <summary>
    /// An exception thrown by the compression or decompression function, indicating that the
    /// given input length was too large for the given input stream.
    /// </summary>
    public class StreamTooShortException : EndOfStreamException
    {
        public StreamTooShortException()
            : base(Main.Get_Traduction("S10"))
        { }
    }
}
