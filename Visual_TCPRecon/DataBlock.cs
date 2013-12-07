using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using VTcpRecon;

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

        public DataBlock(string pFile, int start, int len, TcpRecon pRecon)
        {
            parentFile = pFile;
            startOffset = start;
            length = len;
            endOffset = start + len;
            recon = pRecon;
        }

        public string[] Extract(string startMarker, string endMarker)
        {
            return new string[0];
        }

        public string ToString()
        {
            return "todo";
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

    }
}
