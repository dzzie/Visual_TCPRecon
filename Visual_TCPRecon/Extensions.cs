using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Visual_TCPRecon
{
    /*
        While this is a nice capability to write an extension method,
        how the Fuck could they overlook not having a simple strrepeat method, 
        or split method that takes a single string as input. 
        They've taken all of the simplicity out, and made it a pain in the ass.
        vb6 was just way more direct and intuitive. Now the logic is scattered and subtle.
        i do not believe in this...
     */
    public static class Extensions
    {
        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
        /*public static string Repeat(this char chatToRepeat, int repeat)
        {
            return new string(chatToRepeat, repeat);
        }*/
        public static string Repeat(this string stringToRepeat, int repeat)
        {
            var builder = new StringBuilder(repeat * stringToRepeat.Length);
            for (int i = 0; i < repeat; i++)
            {
                builder.Append(stringToRepeat);
            }
            return builder.ToString();
        }
        public static string[] Split(this string s, string splitAt)
        {
            string[] fuckDotNet = new string[] { splitAt };
            return s.Split(fuckDotNet, StringSplitOptions.None);
        }
    }
}
