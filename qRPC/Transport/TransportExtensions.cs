using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace qRPC.Transport
{
    public static class TransportExtensions
    {
        public static void WriteObjectToStream(this Stream stream, object obj, Encoding encoding)
        {
            using(var bw = new BinaryWriter(stream, encoding, true))
            {
                bw.Write(JsonSerializer.Serialize(obj));
                bw.Flush();
            }
        }

        public static object ReadObjectFromStream(this Stream stream, Encoding encoding, Type T)
        {
            string data = null;
            var br = new BinaryReader(stream, encoding, true);
            data = br.ReadString();
            if (T == typeof(void))
                return default;
            return JsonSerializer.Deserialize(data,T);
        }

        public static T ReadObjectFromStream<T>(this Stream stream, Encoding encoding)
        {
            string data = null;
            var br = new BinaryReader(stream, encoding, true);
            data = br.ReadString();

            if (typeof(T) == typeof(void))
                return default;

            return JsonSerializer.Deserialize<T>(data);
        }
    }
}
