using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tamir.IPLib.Packets;
using Tamir.IPLib;
using System.Net;

namespace Visual_TCPRecon
{
    class cDNS
    {
        public ushort transactionID;

        public ushort flags;
            public bool isResponse;
            public ushort OpCode; //bits 1-4
            public bool authAnswer;
            public bool truncation;
            public bool recursionDesired;

        public ushort questions;
        public ushort answersCount;
        public ushort authorityRRs;
        public ushort additionalRRs;
        
        public string dnsName;

        private byte[] data;

        public cDNS(byte[] raw)
        {
            data = raw;
            if (data.Length < 10) return;

            transactionID = ByteSwap(data, 0);

            flags = ByteSwap(data, 2);
            isResponse = getBit(flags, 0);
            OpCode = (ushort)(flags & (ushort)0x7800); //bits 1-4
            authAnswer = getBit(flags, 6);
            truncation = getBit(flags, 7);
            recursionDesired = getBit(flags, 8);

            questions = ByteSwap(data, 4);
            answersCount = ByteSwap(data, 6);
            authorityRRs = ByteSwap(data, 8);
            additionalRRs = ByteSwap(data, 10);

            setName();

        }

        private bool setName()
        {
            if (data.Length < 0xC) return false;
            if (isResponse || questions == 0) return false; //we only handle requests right now..

            StringBuilder name = new StringBuilder();

            int offset = 0xC;
            byte sz = 0;

            do
            {
                sz = Extract(offset, name);
                if(sz > 0) name.Append('.');
                offset += sz + 1;
            } while (sz > 0);

            dnsName = name.ToString();
            dnsName = dnsName.Substring(0, dnsName.Length - 1); //remove trailing dot..
            return (dnsName.Length > 0);

        }

        private byte Extract(int offset, StringBuilder name)
        {
            byte sz = 0;

            try
            {
                if (offset < data.Length)
                {
                    sz = data[offset];
                    if (sz + offset < data.Length)
                    {
                        for (int i = 1; i <= sz; i++)
                        {
                            name.Append((char)data[offset + i]);
                        }
                    }
                }
            }
            catch (Exception e) { sz = 0; }

            return sz;

        }

        bool getBit(ushort bits, int offset)
        {
            int bit = (bits >> offset) & 0xF;
            return bit == 1 ? true : false;
        }

        public ushort ByteSwap(byte[] b, int offset)
        {
            short r = BitConverter.ToInt16(b, offset);
            return (ushort)System.Net.IPAddress.HostToNetworkOrder(r);
        }


    }
}
