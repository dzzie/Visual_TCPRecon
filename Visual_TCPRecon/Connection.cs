using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Visual_TCPRecon
{
    class Connection
    {

        private string m_fileName;
        public string fileName
        {
            get { return m_fileName; }
        }

        private string m_srcIp;
        public string SourceIp
        {
            get { return m_srcIp; }
        }

        private ushort m_srcPort;
        public ushort SourcePort
        {
            get { return m_srcPort; }
        }

        private string m_dstIp;
        public string DestinationIp
        {
            get { return m_dstIp; }
        }

        private ushort m_dstPort;
        public ushort DestinationPort
        {
            get { return m_dstPort; }
        }

        public Connection(string sourceIP, UInt16 sourcePort, string destinationIP, UInt16 destinationPort)
        {
            m_srcIp = sourceIP;
            m_dstIp = destinationIP;
            m_srcPort = sourcePort;
            m_dstPort = destinationPort;
        }

        public Connection(Tamir.IPLib.Packets.TCPPacket packet)
        {
            m_srcIp = packet.SourceAddress;
            m_dstIp = packet.DestinationAddress;
            m_srcPort = (ushort)packet.SourcePort;
            m_dstPort = (ushort)packet.DestinationPort;
        }

        /// <summary>
        /// Overrided in order to catch both sides of the connection 
        /// with the same connection object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Connection))
                return false;
            Connection con = (Connection)obj;

            bool result = ((con.SourceIp.Equals(m_srcIp)) && (con.SourcePort == m_srcPort) && (con.DestinationIp.Equals(m_dstIp)) && (con.DestinationPort == m_dstPort)) ||
                ((con.SourceIp.Equals(m_dstIp)) && (con.SourcePort == m_dstPort) && (con.DestinationIp.Equals(m_srcIp)) && (con.DestinationPort == m_srcPort));
          
            return result;
        }

        public override int GetHashCode()
        {
            return ((m_srcIp.GetHashCode() ^ m_srcPort.GetHashCode()) as object).GetHashCode() ^
                ((m_dstIp.GetHashCode() ^ m_dstPort.GetHashCode()) as object).GetHashCode();
        }

        public void generateFileName(string baseFolder)
        {
            m_fileName = string.Format("{0}{1,15} - {3,-15}  {2,4} -- {4}", baseFolder, m_srcIp, m_srcPort, m_dstIp, m_dstPort);
        }
    }

}
