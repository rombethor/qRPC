using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using qRPC.Transport;

namespace qRPC.Server
{

    public class QrpcServer<T> : IDisposable
    {
        T _target;

        public delegate T TargetDelegate();

        Task listening;
        TcpListener tcp;
        Encoding _encoding;

        CancellationTokenSource cts;

        public QrpcServer(T target, string hostname, int port, Encoding encoding) 
        {
            Setup(target, hostname, port, encoding);
        }

        public QrpcServer(TargetDelegate targetDelegate, string hostname, int port, Encoding encoding) 
        {
            Setup(targetDelegate.Invoke(), hostname, port, encoding);
        }

        void Setup(T target, string host, int port, Encoding encoding)
        {
            _target = target;
            _encoding = encoding;
            tcp = new TcpListener(IPAddress.Parse(host), port);
            tcp.Start();
            cts = new CancellationTokenSource();
            listening = Listen(cts.Token);
        }

        //Proxy not required for server.  Just use Reflection.

        public async Task Listen(CancellationToken cancellationToken)
        {
            //Listen for connections
            while(!cancellationToken.IsCancellationRequested)
            {
                var client = await tcp.AcceptTcpClientAsync();
                using (NetworkStream stream = client.GetStream())
                {
                    try
                    {
                        while(client.Connected && client.Available == 0)
                        {
                        }
                        if(client.Connected)
                        {
                            var message = stream.ReadObjectFromStream<QrpcRequest>(_encoding);

                            object response = ExecuteTask(message.MethodName, message.Arguments);

                            stream.WriteObjectToStream(response, _encoding);
                            stream.Flush();
                        }
                        client.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            tcp.Stop();
        }
        
        public object ExecuteTask(string methodName, params object[] args)
        {
            var methodInfo = typeof(T).GetMethod(methodName);

            var parameters = methodInfo.GetParameters().OrderBy(p => p.Position).ToArray();

            if (args.Length < parameters.Where(p => !p.IsOptional).Count())
                throw new ArgumentException("The number of arguments provided doesn't fulfill the method requirements");

            for(int i = 0; i < parameters.Count(); i++)
            {
                if (parameters[i].ParameterType == typeof(Guid))
                    args[i] = Guid.Parse(args[i].ToString());
                args[i] = Convert.ChangeType(args[i], parameters[i].ParameterType); 
            }

            var method = typeof(T).GetMethod(methodName);
            var ret = method.Invoke(_target, args);


            return ret; // method.Invoke(_target, args);
        }

        public void Dispose()
        {
            cts.Cancel();
        }
    }
}
