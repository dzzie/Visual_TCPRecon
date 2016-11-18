using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Visual_TCPRecon
{
        public static class HexDumper
        {

            private const int LineLen = 16;
            private static int bCount = 0;
            private static byte[] bytes = new byte[LineLen];
            private static StringBuilder buf;

            public static string HexDump(string str)
            {
                buf = new StringBuilder();
                char[] ch = str.ToCharArray();
                for (int i = 0; i < ch.Length; i++) AddByte((byte)ch[i], (i == ch.Length - 1));
                return buf.ToString();
            }

            public static string HexDump(byte[] b)
            {
                if (b == null) return "";
                buf = new StringBuilder();
                for (int i = 0; i < b.Length; i++) AddByte(b[i], (i == b.Length - 1));
                return buf.ToString();
            }

            public static string HexDump(byte[] b, bool showOffset)
            {
                if (b == null) return "";
                buf = new StringBuilder();
                for (int i = 0; i < b.Length; i++)
                {
                    if (showOffset && (i == 0 || i % 16 == 0))
                    {
                        buf.Append(i.ToString("X05") + "   ");
                        Application.DoEvents();
                    }
                    AddByte(b[i], (i == b.Length - 1));
                }
                return buf.ToString();
            }


            private static void AddByte(byte b, bool final)
            {

                bytes[bCount++] = b;
                if (!final) if (bCount != LineLen) return;
                if (bCount <= 0) return;

                //main dump section
                for (int i = 0; i < LineLen; i++)
                {
                    buf.Append(i >= bCount ? "   " : bytes[i].ToString("X2") + " ");
                }

                buf.Append("  ");

                //char display pad
                for (int i = 0; i < LineLen; i++)
                {
                    byte ch = bytes[i] >= 32 && bytes[i] <= 126 ? bytes[i] : (byte)0x2e; //dot
                    buf.Append(i >= bCount ? " " : (char)ch + "");
                }

                buf.Append("\r\n");
                bCount = 0;
            }
        }
}
