using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace zeus.API
{
    public class GZipDecompress
    {
        string ec = "";
        public GZipDecompress(string content)
        {
            Stream p = new MemoryStream(System.Text.Encoding.Default.GetBytes(content), false);
            using (GZipStream Decompress = new GZipStream(p,
                           CompressionMode.Decompress))
            {
                // Copy the decompression stream 
                // into the output file.
                try
                {
                    byte[] read = new byte[255];
                    int count = Decompress.Read(read, 0, 255);

                    while (count > 0)
                    {
                        String str = System.Text.Encoding.Default.GetString(read, 0, count);
                        ec += str;
                        count = Decompress.Read(read, 0, 255);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("GZIP: Невозможно разархивировать строку", ex);
                }
                
            }

        }

        public string Decompress
        {
            get { return ec; }
        }
        /// <summary>
        /// Разархивировать строку, пришедшую с сервера
        /// </summary>
        /// <param name="content">GZIP строка, для разархивации</param>
        /// <returns>Разархивированная строка</returns>
        public static string DecompressString(string content)
        {
            Stream p = new MemoryStream(System.Text.Encoding.Default.GetBytes(content), false);
            using (GZipStream Decompress = new GZipStream(p,CompressionMode.Decompress))
            {
                // Copy the decompression stream 
                // into the output file.
                try
                {
                    string decompressedString = "";
                    byte[] read = new byte[255];
                    int count = Decompress.Read(read, 0, 255);

                    while (count > 0)
                    {
                        decompressedString += System.Text.Encoding.Default.GetString(read, 0, count);
                        count = Decompress.Read(read, 0, 255);
                    }

                    return decompressedString;

                }
                catch (Exception ex)
                {
                    throw new Exception("GZIP: Невозможно разархивировать строку", ex);
                }
            }
        }


    }
}
