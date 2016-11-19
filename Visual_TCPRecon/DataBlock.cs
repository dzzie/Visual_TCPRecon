using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using VTcpRecon;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Globalization;

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
        public string EpochTimeStamp;  //For the last packet added to reassembled data block
        public string relativeTimeStamp;
        public string SourceAddress = ""; //this is the IP that started the entire conversation stream
        public string DestinationAddress = ""; //this is the IP it was connecting to as a client..
        //destination address may be redundant..since recon holds both ips..we really only need to know who sent it..but whatever

        public int SourcePort = 0;
        public int DestPort = 0;

        public List<string> el = new List<string>();

        public enum DataTypes { dtBinary=0, dtHttpReq, dtHttpResp };
        public DataTypes DataType = DataTypes.dtBinary;

        public string ContentType = "";
        public bool isGZip = false;
        public bool isChunked = false;
        public string HttpHeader="";
        public string HttpFirstLine = "";

        public string GetDetails()
        {
            string t = recon.GetDetails();
            t += "DataBlock\r\n" + "-".Repeat(20);
            t += "\r\nstartOffset: " + startOffset.ToString("X");
            t += "\r\nendOffset: " + endOffset.ToString("X");
            t += "\r\nlength: " + length.ToString();
            t += "\r\nlast packet EpochTimeStamp: " + EpochTimeStamp;
            t += "\r\nlast packet relativeTimeStamp: " + relativeTimeStamp;
            t += "\r\n\r\nDebug Log:\r\n----------------------------------\r\n" + string.Join("\r\n", el.ToArray()); ;
            return t;
        }

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

            el.Clear();

            if (!dataLoaded)
            {
                if (!LoadData()) return ret;
                iLoaded = true;
            }

            int sz = data.Length - HttpHeader.Length;
            byte[] input = new byte[sz];
            Buffer.BlockCopy(data, HttpHeader.Length, input, 0, sz);

            if (isChunked)
            {
                el.Add("Trying to unchunk");
                MemoryStream unchunked = new MemoryStream();
                if (Unchunk(input, ref unchunked))
                {
                    el.Add("Success! " + unchunked.Length.ToString("X") + " bytes");
                    input = unchunked.GetBuffer();
                }
                else
                {
                    el.Add("Unchunk failed!");
                }
            }

            if(isGZip)
            {
                el.Add("Trying to ungzip");
                try
                {
                    ret = Decompress(input);
                    el.Add("Success!");
                }
                catch (Exception ex) {
                    el.Add("ungzip failed: " + ex.Message);
                }
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

            el.Clear();

            if (!dataLoaded)
            {
                if (!LoadData()) return "";
                iLoaded = true;
            }

            if(isChunked || isGZip)
            {
                
                    int sz = data.Length - HttpHeader.Length;
                    byte[] input = new byte[sz];
                    Buffer.BlockCopy(data, HttpHeader.Length, input, 0, sz);
                    //byte[] b2 = new byte[0];

                    if (isChunked)
                    {
                        el.Add("Trying to unchunk");
                        MemoryStream unchunked = new MemoryStream();
                        if (Unchunk(input, ref unchunked))
                        {
                            el.Add("Success! " + unchunked.Length.ToString() + " bytes");
                            input = unchunked.GetBuffer();
                        }
                        else
                        {
                            el.Add("Unchunk failed!");
                        }
                    }

                    if (isGZip)
                    {
                        el.Add("Trying to ungzip");
                        try
                        {
                            input = Decompress(input);
                            el.Add("Success!");
                        }
                        catch (Exception ex)
                        {
                            el.Add("ungzip failed: " + ex.Message);
                            ret = AsString();
                        }
                    }
            
                    byte[] buf = Encoding.Convert(Encoding.GetEncoding("iso-8859-1"), Encoding.UTF8, input, 0, input.Length);
                    ret = HttpHeader + Encoding.UTF8.GetString(buf, 0, buf.Length);

            }
            else
            {
                ret = AsString();
            }

           /* if(ret.IndexOf("\x0") > 0){ //isbinary 
                if(this.HttpHeader.Length > 0) ret = this.HttpHeader + "\r\n\r\n[Binary data]";
            }*/
             
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

            //byte[] bb = GetBinaryBody(); //handles ungzip and unchunk..
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

        public bool AppendToFile(string path)
        {
            if (data == null) return false;

            if (!File.Exists(path)) return SaveToFile(path);

            try
            {
                using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Append)))
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
            if (!File.Exists(parentFile)){el.Add("Parent file not found"); return false ;}

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

        //mod from mark woan httpkit
        public bool Unchunk(byte[] buf, ref MemoryStream writeStream )
        {

            MemoryStream inFile = new MemoryStream(buf);

            long bytesWritten = 0;
            try
            {
                do
                {
                    string temp = this.ReadLine(inFile);
                    if (temp == null) return true;

                    if (temp.Length == 0)
                    {
                        temp = this.ReadLine(inFile);
                        if (temp == null) return true;
                        if (temp.Length == 0) return true;
                    }

                    // Some chunked encoding has a semi-colon after
                    // the size of the chunk, so lets just remove it
                    temp = temp.Replace(";", string.Empty);

                    // Now try to parse out an INT from the string representation of a hex number
                    int chunkSize = 0;
                    if (int.TryParse(temp, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out chunkSize) == false)
                    {
                        // TODO raise error?
                        return true;
                    }

                    if (chunkSize == 0)
                    {
                        // A zero signifies that we have reached the end of the chunked sections so lets exit
                        return true;
                    }

                    //if (stream.Position + chunkSize > stream.Length)
                    //{
                    //    // TODO raise error? Invalid chunk data e.g. is greater length than rest of stream
                    //    return false;
                    //}

                    byte[] chunk = new byte[chunkSize];
                    int ret = inFile.Read(chunk, 0, chunkSize);
                    bytesWritten += ret;
                    if (ret != chunkSize)
                    {
                        // TODO raise error? e.g. the amount of data read should be equal to the amount in the chunk size?
                        //OutputDebug("GZIP", inputfile, string.Empty);
                        return false;
                    }

                    writeStream.Write(chunk, 0, ret);
                }
                while (inFile.Position < inFile.Length);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                //this.TempFileSize = bytesWritten;
            }
        

                return true;
            
        }

        //mark woan httpkit
        private string ReadLine(Stream stream)
        {
            //int maxStringLength = 16384;
            //  \r = 0x0d = carriage return
            //  \n = 0x0a = line feed
            StringBuilder line = new StringBuilder();
            bool carrigeReturnReceived = false;
            bool lineFeedReceived = false;
            //int indexOffset = 0;
            while (!carrigeReturnReceived || !lineFeedReceived)
            {
                //if (dataIndex + indexOffset >= data.Length || indexOffset >= maxStringLength)
                //    return null;
                //else
                {
                    byte b = (byte)stream.ReadByte();
                    if (b == 0x0d)
                        carrigeReturnReceived = true;
                    else if (carrigeReturnReceived && b == 0x0a)
                        lineFeedReceived = true;
                    else
                    {
                        line.Append((char)b);
                        carrigeReturnReceived = false;
                        lineFeedReceived = false;
                    }
                    //indexOffset++;
                }
            }
            //dataIndex += indexOffset;
            return line.ToString();
        }

        private string GetTempFile()
        {
            return Path.Combine(Path.GetTempPath(), System.Guid.NewGuid().ToString() + ".tmp");
        }

    }
}
