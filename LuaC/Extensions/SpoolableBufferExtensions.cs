using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DSCript.Spooling;

namespace Zartex
{
    public static class SpoolableBufferMethodExtension
    {
        public static byte[] GetBuffer(this SpoolableBuffer sb)
        {
            using (var br = new BinaryReader(sb.GetMemoryStream()))
            {
                return br.ReadBytes((int)sb.GetMemoryStream().Length);
            }
        }
        public static void Write(this SpoolableBuffer sb, byte b)
        {
            sb.GetMemoryStream().Write(b);
        }
        public static void Write(this SpoolableBuffer sb, byte[] bytes)
        {
            sb.GetMemoryStream().Write(bytes);
        }
        public static void Write(this SpoolableBuffer sb, int integer)
        {
            sb.GetMemoryStream().Write(integer);
        }
        public static void Write(this SpoolableBuffer sb, uint uinteger)
        {
            sb.GetMemoryStream().Write(uinteger);
        }
        public static void Write(this SpoolableBuffer sb, ushort ushortinteger)
        {
            sb.GetMemoryStream().Write(ushortinteger);
        }
        public static void Write(this SpoolableBuffer sb, short shortinteger)
        {
            sb.GetMemoryStream().Write(shortinteger);
        }
        public static void Write(this SpoolableBuffer sb, long longinteger)
        {
            sb.GetMemoryStream().Write(longinteger);
        }
        public static void Write(this SpoolableBuffer sb, ulong ulonginteger)
        {
            sb.GetMemoryStream().Write(ulonginteger);
        }
        public static void Write(this SpoolableBuffer sb, float floatingnumber)
        {
            sb.GetMemoryStream().Write(floatingnumber);
        }
        public static void Write<T>(this SpoolableBuffer sb, T data)
        {
            sb.GetMemoryStream().Write<T>(data);
        }
    }
}
