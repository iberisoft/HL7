using System;
using System.IO;
using System.Text;

namespace HL7.Net
{
    static class Llp
    {
        static Encoding m_Encoding = Encoding.UTF8;

        public static string ReadMessage(Stream stream)
        {
            using (var stream2 = new MemoryStream())
            {
                int b;
                try
                {
                    b = stream.ReadByte();
                }
                catch
                {
                    return null;
                }
                if (b == -1)
                {
                    return null;
                }

                if (b != 0x0b)
                {
                    throw new InvalidOperationException("Header symbol is invalid");
                }
                while (true)
                {
                    b = stream.ReadByte();
                    if (b == 0x1c)
                    {
                        b = stream.ReadByte();
                        if (b == 0x0d)
                        {
                            break;
                        }
                    }
                    stream2.WriteByte((byte)b);
                }
                return m_Encoding.GetString(stream2.ToArray());
            }
        }

        public static void WriteMessage(Stream stream, string message)
        {
            stream.WriteByte(0x0b);
            var bytes = m_Encoding.GetBytes(message);
            stream.Write(bytes, 0, bytes.Length);
            stream.WriteByte(0x1c);
            stream.WriteByte(0x0d);
        }
    }
}
