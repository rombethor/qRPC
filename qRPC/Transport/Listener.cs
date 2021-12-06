using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace qRPC.Transport
{
    public class Listener<T> where T : new()
    {
        private readonly TcpListener tcp;

        private readonly Encoding encoding;

        readonly T machine = new T();

        public Listener(int port, string ipAddress = "127.0.0.1")
        {
            encoding = Encoding.UTF8;
            tcp = new TcpListener(IPAddress.Parse(ipAddress), port);
            tcp.Start();
        }

        public Listener(int port, string ipAddress, Encoding encoding)
        {
            this.encoding = encoding;
            tcp = new TcpListener(IPAddress.Parse(ipAddress), port);
            tcp.Start();
        }

        public async Task Listen()
        {
            while(true)
            {
                if (Debugger.IsAttached)
                    Console.WriteLine("\rWaiting for connection...");

                TcpClient client = await tcp.AcceptTcpClientAsync();

                if (Debugger.IsAttached)
                    Console.WriteLine("Connected!");

                NetworkStream stream = client.GetStream();

                //1 - length of method name
                //2 - method name
                //3 - number of paramters
                //4 - parameter data, each prefixed with 4 bytes for their length

                int position = 0;
                byte[] buffer4 = new byte[4];
                position += stream.Read(buffer4, position, 4);
                //Convert to int
                int methodNameSize = Convert.ToInt32(buffer4);

                byte[] bufferMethod = new byte[methodNameSize];
                position += stream.Read(bufferMethod, position, methodNameSize);

                string methodName = encoding.GetString(bufferMethod);

                position += stream.Read(buffer4, position, 4);
                int parameterCount = Convert.ToInt32(buffer4);

                List<byte[]> parameterBytes = new List<byte[]>();

                //Loop through parameters to get their lengths and data
                for(int pi = 0; pi < parameterCount; pi++)
                {
                    position += stream.Read(buffer4, position, 4);
                    int parameterSize = Convert.ToInt32(buffer4);

                    byte[] paramBuffer = new byte[parameterSize];
                    position += stream.Read(paramBuffer, position, parameterSize);

                    parameterBytes.Add(paramBuffer);

                }

                //Reflection: get 
                var methodInfo = typeof(T).GetMethod(methodName);

                var methodParameters = methodInfo.GetParameters();

                List<object> paramData = new List<object>();
                //Loop through method parameters
                foreach(var paramInfo in methodParameters.OrderBy(p => p.Position))
                {
                    paramData.Add(Convert.ChangeType(parameterBytes[0], paramInfo.ParameterType));
                }

                ///call the method.  Async methods probably wouldn't work
                var response = methodInfo.Invoke(machine, paramData.ToArray());

                BinaryFormatter bf = new BinaryFormatter();
                byte[] responseBytes;
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, response);
                    responseBytes = ms.ToArray();
                }
                //Return the method response data.
                stream.Write(responseBytes, 0, responseBytes.Length);
            }
        }


    }
}
