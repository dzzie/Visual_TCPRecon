using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using VTcpRecon;
using System.Text.RegularExpressions;
using System.IO.Compression;

//this class stores the details on where the data is located in the parent stream file (of many data blocks)
//and can extract it on demand. it also has a basic http header parser, and gzip decompressor which could be
//seperated out into a handler class if we add more handlers latter on. right now its either binary or http
//so didnt feel like adding any more complexity to it.
//
//some methods can load the data on demand, and will free it when done. you can also load the data yourself,
//do stuff, then free it manually when done. I think its better to load and free, rather than load and hold
//because packet dumps can get really really big and the ram usage could bottle neck real quick. right now
//we can load a 5mb pcap in about 1.2sec on a 2.8gz dual core xp machine that is from like 2007 (6yrs old)

namespace Visual_TCPRecon
{
    public class DataBlock
    {
        public byte[] data = null;
        public string parentFile;
        public int startOffset;
        public int endOffset;
        public int length;
        public bool dataLoaded = false;
        public TcpRecon recon;

        public enum DataTypes { dtBinary=0, dtHttpReq, dtHttpResp };
        public DataTypes DataType = DataTypes.dtBinary;

        public string ContentType = "";
        public bool isGZip = false;
        public bool isChunked = false;
        public string HttpHeader="";
        public string HttpFirstLine = "";

        public DataBlock(string pFile, int start, int len, TcpRecon pRecon)
        {
            parentFile = pFile;
            startOffset = start;
            length = len;
            endOffset = start + len;
            recon = pRecon;
        }

        public string BinaryBodyToTmpFile()
        {
            string tmp = "";
            try
            {
                byte[] b = GetBinaryBody();
                tmp = Path.GetTempFileName();
                File.WriteAllBytes(tmp, b);
            }
            catch (Exception ex) { };

            return tmp;
        }

        public byte[] GetBinaryBody() //content body only no http header - supports un-gzipping.. can return null, used for image view
        {
            bool iLoaded = false;
            byte[] ret = null;

            if (!dataLoaded)
            {
                if (!LoadData()) return ret;
                iLoaded = true;
            }

            int sz = data.Length - HttpHeader.Length;
            byte[] input = new byte[sz];
            Buffer.BlockCopy(data, HttpHeader.Length, input, 0, sz);

            if(isGZip)
            {
                try
                {
                    ret = Decompress(input);                   
                }
                catch (Exception ex) {}
            }
            else
            {
                ret = input;
            }

            if (iLoaded) FreeData();
            return ret;

        }

        public string GetBody() //http header + content body as string. supports un-gzipping
        {
            bool iLoaded = false;
            string ret = "";

            if (!dataLoaded)
            {
                if (!LoadData()) return "";
                iLoaded = true;
            }

            if(isGZip)
            {
                try
                {
                    int sz = data.Length - HttpHeader.Length;
                    byte[] input = new byte[sz];
                    Buffer.BlockCopy(data, HttpHeader.Length, input, 0, sz);
                    byte[] b2 = Decompress(input);
                    byte[] buf = Encoding.Convert(Encoding.GetEncoding("iso-8859-1"), Encoding.UTF8, b2, 0, b2.Length);
                    ret = HttpHeader + Encoding.UTF8.GetString(buf, 0, buf.Length);
                }
                catch (Exception ex) {}
            }
            else
            {
                ret = AsString();
            }

            if (iLoaded) FreeData();
            return ret;

        }

        public void DetectType()
        {
            if (!LoadData()) return;

            int firstNL = 0;
            int HttpHeaderEnd = 0;
            StringComparison sc = StringComparison.CurrentCultureIgnoreCase;

            //bugfix: bitconverter can try to read past end of array..but we need to read to .length to catch all..
            for (Int32 i = 0; i < data.Length; i++)
            {
                if (data[i] == 0) goto cleanup; //binary not a http header

                try
                {
                    if (firstNL == 0 && BitConverter.ToInt16(data, i) == 0x0A0D) firstNL = i;

                    if (BitConverter.ToInt32(data, i) == 0x0A0D0A0D)
                    {
                        HttpHeaderEnd = i;
                        if (i + 4 < data.Length) HttpHeaderEnd += 4;
                        break;
                    }
                }
                catch (Exception ex) { /*whatever*/ }
                 
            }
             
            if (HttpHeaderEnd > 0)
            {
                string firstLine = AsString(0,firstNL);
                int httpIndex = firstLine.IndexOf("HTTP", sc);
                if ( httpIndex >= 0)
                {
                    DataType = httpIndex==0 ? DataTypes.dtHttpResp : DataTypes.dtHttpReq;
                    HttpFirstLine = firstLine;
                    HttpHeader = AsString(0,HttpHeaderEnd);

                    string[] lines = Regex.Split(HttpHeader, "\r\n");
                    foreach (string line in lines)
                    {
                        if (line.IndexOf("Content-Encoding:", sc) != -1)
                        {
                            if (line.IndexOf("gzip", sc) != -1) isGZip = true;
                        }
                        if (line.IndexOf("Transfer-Encoding:", sc) != -1)
                        {
                            if (line.IndexOf("chunked", sc) != -1) isChunked = true;
                        }
                        if (line.IndexOf("Content-Type:", sc) != -1)
                        {
                            int a = line.IndexOf(":")+1;
                            if(a > 0) ContentType = line.Substring(a);
                        }
                    }
                }
            }

        cleanup:
            this.FreeData();
            return;
        }

        public string AsString() {           return AsString(0, this.length); }
        public string AsString(int start) { return AsString(start, this.length - start);      }

        public string AsString(int start, int len)
        {
            bool iLoaded = false;
            if (!dataLoaded)
            {
                if ( this.LoadData() )
                {
                    iLoaded = true;
                }
                else
                {
                    return "";
                }
            }

            if(start + len > this.length) len = this.length - start;

            byte[] buf = Encoding.Convert(Encoding.GetEncoding("iso-8859-1"), Encoding.UTF8, data, start, len);
            string ret = Encoding.UTF8.GetString(buf, 0, buf.Length);

            if (iLoaded) this.FreeData();
            return ret;
        }

        public void FreeData()
        {
            data = null;
            dataLoaded = false;
        }

        public bool SaveToFile(string path)
        {
            if(data == null) return false;

            try
            {
                using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create)))
                {
                    bw.Write(data);
                    bw.Close();
                }
                return true;
            }
            catch (Exception e) { return false; }

        }

        public bool LoadData()
        {

            if (dataLoaded) return true;
            if (!File.Exists(parentFile)) return false ;

            using (BinaryReader br = new BinaryReader(File.Open(parentFile, FileMode.Open)))
            {
                int maxSize = (int)br.BaseStream.Length;
                int selLen = this.length;

                if (startOffset + selLen > maxSize) selLen = maxSize - startOffset;
                 
                if (selLen > 0)
                {
                    data = new byte[selLen];
                    br.BaseStream.Seek(startOffset, SeekOrigin.Begin);
                    data = br.ReadBytes(selLen);
                    dataLoaded = true;
                }

                br.Close();
            }

            return dataLoaded;
        }

        public byte[] Decompress(byte[] gzip)
        {
            // Create a GZIP stream with decompression mode.
            // ... Then create a buffer and write into while reading from the GZIP stream.
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }


    }
}
