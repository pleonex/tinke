using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace DSDecmp.Formats
{
    /// <summary>
    /// Base class for compression formats.
    /// </summary>
    public abstract class CompressionFormat
    {
        /// <summary>
        /// Checks if the decompressor for this format supports the given file. Assumes the
        /// file exists. Returns false when it is certain that the given file is not supported.
        /// False positives may occur, as this method should not do any decompression, and
        /// may mis-interpret a similar file format as compressed.
        /// </summary>
        /// <param name="file">The name of the file to check.</param>
        /// <returns>False if the file can certainly not be decompressed using this decompressor.
        /// True if the file may potentially be decompressed using this decompressor.</returns>
        public virtual bool Supports(string file)
        {
            // open the file, and delegate to the decompressor-specific code.
            using (FileStream fstr = new FileStream(file, FileMode.Open))
            {
                return this.Supports(fstr, fstr.Length);
            }
        }

        /// <summary>
        /// Checks if the decompressor for this format supports the data from the given stream.
        /// Returns false when it is certain that the given data is not supported.
        /// False positives may occur, as this method should not do any decompression, and may
        /// mis-interpret a similar data format as compressed.
        /// </summary>
        /// <param name="stream">The stream that may or may not contain compressed data. The
        /// position of this stream may change during this call, but will be returned to its
        /// original position when the method returns.</param>
        /// <param name="inLength">The length of the input stream.</param>
        /// <returns>False if the data can certainly not be decompressed using this decompressor.
        /// True if the data may potentially be decompressed using this decompressor.</returns>
        public abstract bool Supports(Stream stream, long inLength);

        /// <summary>
        /// Decompresses the given file, writing the deocmpressed data to the given output file.
        /// The output file will be overwritten if it already exists.
        /// Assumes <code>Supports(infile)</code> returns <code>true</code>.
        /// </summary>
        /// <param name="infile">The file to decompress.</param>
        /// <param name="outfile">The target location of the decompressed file.</param>
        public virtual void Decompress(string infile, string outfile)
        {
            // make sure the output directory exists
            string outDirectory = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(outDirectory))
                Directory.CreateDirectory(outDirectory);
            // open the two given files, and delegate to the format-specific code.
            using (FileStream inStream = new FileStream(infile, FileMode.Open),
                             outStream = new FileStream(outfile, FileMode.Create))
            {
                this.Decompress(inStream, inStream.Length, outStream);
            }
        }

        /// <summary>
        /// Decompresses the given stream, writing the decompressed data to the given output stream.
        /// Assumes <code>Supports(instream)</code> returns <code>true</code>.
        /// After this call, the input stream will be positioned at the end of the compressed stream,
        /// or at the initial position + <code>inLength</code>, whichever comes first.
        /// </summary>
        /// <param name="instream">The stream to decompress. At the end of this method, the position
        /// of this stream is directly after the compressed data.</param>
        /// <param name="inLength">The length of the input data. Not necessarily all of the
        /// input data may be read (if there is padding, for example), however never more than
        /// this number of bytes is read from the input stream.</param>
        /// <param name="outstream">The stream to write the decompressed data to.</param>
        /// <returns>The length of the output data.</returns>
        /// <exception cref="NotEnoughDataException">When the given length of the input data
        /// is not enough to properly decompress the input.</exception>
        public abstract long Decompress(Stream instream, long inLength, Stream outstream);

        /// <summary>
        /// Compresses the given input file, and writes the compressed data to the given
        /// output file.
        /// </summary>
        /// <param name="infile">The file to compress.</param>
        /// <param name="outfile">The file to write the compressed data to.</param>
        /// <returns>The size of the compressed file.</returns>
        public int Compress(string infile, string outfile)
        {
            // make sure the output directory exists
            string outDirectory = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(outDirectory))
                Directory.CreateDirectory(outDirectory);
            // open the proper Streams, and delegate to the format-specific code.
            using (FileStream inStream = File.Open(infile, FileMode.Open),
                             outStream = File.Create(outfile))
            {
                return this.Compress(inStream, inStream.Length, outStream);
            }
        }

        /// <summary>
        /// Compresses the next <code>inLength</code> bytes from the input stream,
        /// and writes the compressed data to the given output stream.
        /// </summary>
        /// <param name="instream">The stream to read plaintext data from.</param>
        /// <param name="inLength">The length of the plaintext data.</param>
        /// <param name="outstream">The stream to write the compressed data to.</param>
        /// <returns>The size of the compressed stream.</returns>
        public abstract int Compress(Stream instream, long inLength, Stream outstream);
    }
}
